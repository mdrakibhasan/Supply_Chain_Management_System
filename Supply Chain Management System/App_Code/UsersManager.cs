using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Data.SqlClient;
using autouniv;

/// <summary>
/// Summary description for UsersManager
/// </summary>
/// 
namespace autouniv
{
    public class UsersManager
    {
        public static void CreateUser(Users usr)
        {
            SqlConnection connection = new SqlConnection(DataManager.OraConnString());
            SqlTransaction transection;
            try
            {
                connection.Open();
                transection = connection.BeginTransaction();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.Transaction = transection;

                command.CommandText = @"insert into utl_userinfo (user_name,password,description,user_grp,status,emp_no,BranchID,Email,Type) values ('" + usr.UserName.Replace("'", "") + "', " + " '" + usr.Password.Replace("'", "") + "', " + " '" + usr.Description.Replace("'", "") + "', " + " '" + usr.UserGrp + "', " + " '" + usr.Status + "', " + " '" + usr.EmpNo + "','" + usr.BranchID + "' ,'" + usr.Email.Replace("'", "") + "' , '" + usr.Type + "'  )";
                command.ExecuteNonQuery();
                transection.Commit();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static void UpdateUser(Users usr)
        {
            SqlConnection connection = new SqlConnection(DataManager.OraConnString());
            int CheckGroup = IdManager.GetShowSingleValueInt("USER_GRP", "USER_NAME", "UTL_USERINFO", usr.UserName);
            SqlTransaction transection;
            try
            {
                connection.Open();
                transection = connection.BeginTransaction();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.Transaction = transection;

                string Password = "";
                if (!string.IsNullOrEmpty(usr.Password))
                {
                    Password = " password= '" + usr.Password + "', ";
                }

                 command.CommandText = @"update utl_userinfo set " + Password.Replace("'", "") + " description='" + usr.Description.Replace("'", "") + "', " +
                       " user_grp= '" + usr.UserGrp + "', status= '" + usr.Status + "', emp_no= '" + usr.EmpNo + "',BranchID='" + usr.BranchID + "',Email='" + usr.Email.Replace("'", "") + "',Type='" + usr.Type + "' where upper(user_name)=upper('" + usr.UserName + "')  ";
                command.ExecuteNonQuery();

                if (!usr.UserName.Equals(CheckGroup.ToString()))
                {
                    command.CommandText = @"DELETE FROM [UTL_USERGRANT]  WHERE [USER_NAME]='" + usr.UserName + "' ";
                    command.ExecuteNonQuery();
                }
                transection.Commit();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }        

        private static void InsertUserPermission(Users usr, SqlCommand command, DataRow drr)
        {
            command.CommandText = @"INSERT INTO [UTL_USERGRANT]
                   ([USER_NAME],[USER_GRP],[MOD_ID],[ALLOW_ADD],[ALLOW_EDIT],[ALLOW_VIEW],[ALLOW_DELETE],[ALLOW_PRINT],[ALLOW_AUTHO],Flag)
                     VALUES
                   ('" + usr.UserName + "','" + usr.UserGrp + "','" + drr["SUB_MOD_ID"].ToString() + "','" + drr["ALLOW_ADD"].ToString() + "','" + drr["ALLOW_EDIT"].ToString() + "','" + drr["ALLOW_VIEW"].ToString() + "','" + drr["ALLOW_DELETE"].ToString() + "','" + drr["ALLOW_PRINT"].ToString() + "','" + drr["ALLOW_AUTHO"].ToString() + "',1)";
            command.ExecuteNonQuery();
        }
        public static void DeleteUser(Users usr)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "delete from utl_userinfo where upper(user_name)=upper('"+usr.UserName+"')  ";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static Users getUser(string usr)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);

            string query = "select * from utl_userinfo where upper(user_name)=upper('" + usr + "')";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "UserInfo");
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new Users(dt.Rows[0]);
        }
        public static DataTable GetUsers()
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = @"select [user_name],[description],t2.GROUP_DESC AS user_grp,t1.user_grp usergrp from  utl_userinfo t1 inner join  dbo.UTL_GROUPINFO t2 on t2.USER_GRP=t1.USER_GRP where [status]='A' order by [user_name]";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Users");
            return dt;
        }
        public static string getUserName(string user)
        {
            String ConnectionString = DataManager.OraConnString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            string Query = "select dbo.initcap(description)  from utl_userinfo where user_name='" + user + "' ";
            myConnection.Open();
            SqlCommand myCommand = new SqlCommand(Query, myConnection);
            object maxValue = myCommand.ExecuteScalar();
            myConnection.Close();
            string a = "";
            if (maxValue != null)
            {
                a = maxValue.ToString();
            }
            return a;
        }

        public static void SaveGroupInformation(Users aUsers)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = @"INSERT INTO [UTL_GROUPINFO]
           ([GROUP_DESC],AddBy,AddDate)
     VALUES
           ('" + aUsers.UserGrp + "','" + aUsers.LoginBy + "','" + Globals._localTime.ToString() + "')";
            DataManager.ExecuteNonQuery(connectionString, query);
        }

        public static void UpdateGroupInformation(Users aUsers)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = @"UPDATE [UTL_GROUPINFO]
            SET [GROUP_DESC] ='" + aUsers.UserGrp + "',UpdateBy='" + aUsers.LoginBy + "',UpdateDate='" + Globals._localTime.ToString() + "' WHERE USER_GRP='" + aUsers.GroupID + "'";
            DataManager.ExecuteNonQuery(connectionString, query);           
        }

        public static DataTable getShowUserInfo(string ID)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = @"SELECT [USER_GRP],[GROUP_DESC] FROM [UTL_GROUPINFO] " + ID;
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Users");
            return dt;
        }

        public static void DeleteGroupInformation(Users aUsers)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = @"UPDATE [UTL_GROUPINFO]
            SET DeleteBy='" + aUsers.LoginBy + "',DeleteDate='" + Globals._localTime.ToString() + "' WHERE USER_GRP='" + aUsers.GroupID + "'";
            DataManager.ExecuteNonQuery(connectionString, query);
        }

        public static DataTable GetShowUser(string val)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string Parameter = " WHERE Type='" + val + "' ";
            string query = @"SELECT t1.[USER_NAME],t1.[PASSWORD],t1.[DESCRIPTION],t1.[USER_GRP],t1.[STATUS],t1.[EMP_NO],t2.BranchName AS BranchName   FROM [UTL_USERINFO] t1 LEFT join BranchInfo t2 on t2.ID=t1.BranchID" + Parameter;
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Users");
            return dt;
        }
        // ***************** Reset Passwoprd *****************//
        public static void GetResetPassword(Users usr)
        {
            string OldPass = IdManager.GetShowSingleValueString("PASSWORD", "USER_NAME", "UTL_USERINFO", usr.UserName);
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = @"UPDATE [UTL_USERINFO]
            SET ChangeBy='" + usr.LoginBy + "',ChangeDate='" + Globals._localTime.ToString() + "',OldPassword='" + OldPass + "',PASSWORD='" + usr.Password + "' WHERE USER_NAME='" + usr.UserName + "'";
            DataManager.ExecuteNonQuery(connectionString, query);
        }

        public static object GetUsersSetSearchUser(string User)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = @"select [user_name],[description],t2.GROUP_DESC AS user_grp,t1.user_grp usergrp from  utl_userinfo t1 inner join  dbo.UTL_GROUPINFO t2 on t2.USER_GRP=t1.USER_GRP where [status]='A'  and upper([USER_NAME]+' - '+[DESCRIPTION]) = upper('" + User + "') order by [user_name]";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Users");
            return dt;
        }
    }
}

