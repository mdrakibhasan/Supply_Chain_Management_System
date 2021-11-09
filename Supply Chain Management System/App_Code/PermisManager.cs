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
/// Summary description for PermisManager
/// </summary>
/// 
namespace autouniv
{
    public class PermisManager
    {
        public static void CreatePermis(Permis per)
        {
            String connectionString = DataManager.OraConnString();
            string query = " insert into utl_usergrant (user_name,mod_id,allow_add,allow_edit,allow_view, " +
                   " allow_delete,allow_print,allow_autho) values ( "+" '" + per.UserName + "', "+ 
                   " " + " '" + per.ModId + "', " + " '" + per.AllowAdd + "',  " + " '" + per.AllowEdit + "', "+
                   " " + " '" + per.AllowView + "', " + " '" + per.AllowDelete + "',  " + " '" + per.AllowPrint + "', " + " '" + per.AllowAutho + "')";

            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void UpdatePermis(Permis per)
        {
            String connectionString = DataManager.OraConnString();
            string query = " update utl_usergrant set allow_add= '" + per.AllowAdd + "',  allow_edit= '" + per.AllowEdit + "', " +
                   " allow_view= '" + per.AllowView + "', allow_delete= '" + per.AllowDelete + "',  allow_print= '" + per.AllowPrint + "', "+
                   " allow_autho= '" + per.AllowAutho + "' where upper(user_name)='"+per.UserName+"' and mod_id='"+per.ModId+"'";

            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static DataTable GetPermiss(string user)
        {
            string connectionString = DataManager.OraConnString();
            string Parameter = "";
            if (!string.IsNullOrEmpty(user))
            {
                Parameter = " and a.user_name='" + user + "'";
            }
            string query = "select distinct a.user_name,a.mod_id,b.description mod_name,a.allow_add,a.allow_edit,a.allow_view, " +
            " a.allow_delete,a.allow_print,a.allow_autho from utl_usergrant a, utl_modules b where a.mod_id=b.mod_id " +
            " " + Parameter + " order by a.mod_id";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Permiss");
            return dt;
        }
        public static Permis getPermis(string user, string modid)
        {
            String connectionString = DataManager.OraConnString();
            string query = "select * from utl_usergrant where upper(user_name)='" + user + "' and mod_id='"+modid+"'";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Permis");
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new Permis(dt.Rows[0]);
        }
        public static DataTable getModules()
        {
            string connectionString = DataManager.OraConnString();
            string query = "select * from utl_modules";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Modules");
            return dt;
        }
        public static DataTable getModulesUser(string user)
        {
            string connectionString = DataManager.OraConnString();
            string query = "select * from utl_modules where mod_id not in (select mod_id from utl_usergrant where upper(user_name)=upper('"+user+"')) order by mod_id";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Modules");
            return dt;
        }
        public static Permis getUsrPermis(string user, string modname)
        {
            String connectionString = DataManager.OraConnString();
            string query = "";
            DataTable dt = null;
            int count = IdManager.GetShowSingleValueInt("COUNT(*)", "upper(user_name)", "utl_usergrant", user.ToUpper());
            if (count > 0)
            {
                query = "select distinct user_name,a.mod_id,a.allow_add,a.allow_edit,a.allow_view,a.allow_delete,a.allow_print,a.allow_autho " +
            " from utl_usergrant a, utl_modules b where a.mod_id=b.mod_id and upper(a.user_name)=upper('" + user + "') and a.mod_id='" + modname + "'";
            }
            else
            {
                Users usr = UsersManager.getUser(user.ToString().ToUpper());
                 if (usr != null)
                 {
                     query = @"SELECT '" + user + "' AS[user_name],[ID],[USER_GRP],[SUB_MOD_ID] AS[mod_id],[ALLOW_ADD] AS[allow_add],[ALLOW_EDIT] AS[allow_edit],[ALLOW_VIEW] AS[allow_view],[ALLOW_DELETE] AS[allow_delete],[ALLOW_PRINT] AS[allow_print],[ALLOW_AUTHO] AS[allow_autho] FROM [UTL_GROUPGRANT] where USER_GRP='" + usr.UserGrp + "' And SUB_MOD_ID='" + modname + "'  ";
                 }
            }            
            dt = DataManager.ExecuteQuery(connectionString, query, "Permis");
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new Permis(dt.Rows[0]);
        }
        public static string getModuleId(string modname)
        {
            string modName = "";
            string connectionString = DataManager.OraConnString();
            SqlDataReader dReader;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = connectionString;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "Select mod_id from utl_modules where rtrim(upper(mod_name))=rtrim(upper('" +modname + "'))";
            conn.Open();
            try
            {
                dReader = cmd.ExecuteReader();
                if (dReader.HasRows == true)
                {
                    while (dReader.Read())
                        modName = dReader["mod_id"].ToString();
                }
            }
            finally
            {
                conn.Close();
            }
            return modName;
        }

        public static DataTable getShowGroupPriviledge(string Parameter)
        {
            string connectionString = DataManager.OraConnString();
            string query = @"SELECT t2.MOD_ID AS [ID],t1.[USER_GRP],t1.[SUB_MOD_ID] AS[11ID],t2.[DESCRIPTION] AS[ModelName],t1.[ALLOW_ADD] AS[Add] ,t1.[ALLOW_EDIT] AS[Edit] ,t1.[ALLOW_VIEW] AS[View],t1.[ALLOW_DELETE] AS[Delete] ,t1.[ALLOW_PRINT] AS[Print] ,t1.[ALLOW_AUTHO] AS[Authoriz]
  FROM [UTL_GROUPGRANT] t1 INNER JOIN UTL_MODULES t2 on t2.MOD_ID=t1.SUB_MOD_ID " + Parameter;
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Modules");
            return dt;
        }

        public static void SavePermisPrivilidge(DataTable dt, string GroupID)
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

                SqlCommand command1 = new SqlCommand();
                command1.Connection = connection;
                command1.Transaction = transection;

                command.CommandText = @"SELECT COUNT(*) FROM [UTL_GROUPGRANT] where [USER_GRP]='" + GroupID + "' ";
                int count = Convert.ToInt32(command.ExecuteScalar());
                if (count > 0)
                {
                    command1.CommandText = @"DELETE FROM [UTL_GROUPGRANT] where [USER_GRP]='" + GroupID + "' ";
                    command1.ExecuteNonQuery();
                }
                foreach (DataRow dr in dt.Rows)
                {
                    command.CommandText = @"INSERT INTO [UTL_GROUPGRANT]
           ([USER_GRP],[SUB_MOD_ID],[ALLOW_ADD],[ALLOW_EDIT],[ALLOW_VIEW],[ALLOW_DELETE],[ALLOW_PRINT],[ALLOW_AUTHO])
            VALUES
           ('" + GroupID + "','" + dr["ID"].ToString() + "','" + dr["Add"].ToString() + "','" + dr["Edit"].ToString() + "','" + dr["View"].ToString() + "','" + dr["Delete"].ToString() + "','Y','" + dr["Authoriz"].ToString() + "')";
                    command.ExecuteNonQuery();
                }
                transection.Commit();
                connection.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void DeletePermisPrivilidge(string GroupID)
        {
            String connectionString = DataManager.OraConnString();
            string query = @"DELETE FROM [UTL_GROUPGRANT] where [USER_GRP]='" + GroupID + "' ";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
    }
}