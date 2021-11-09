using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

using autouniv;

namespace OldColor
{
    public class clsClientInfoManager
    {
        public static DataTable GetClientInfos()
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "select client_id,client_name,national_id,address1,address2,phone,mobile,fax,email,url,status from client_info order by client_id";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "client_info");
            return dt;
        }
        public static DataTable GetClientInfosGrid()
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "select * from Customer a where a.DeleteBy IS NULL order by ID";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Customer");
            return dt;
        }
        public static void CreateClientInfo(clsClientInfo ci)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = @"INSERT INTO [Customer]
           ([Code]           
           ,[ContactName]
           ,[Email]
           ,[Mobile]
           ,[Phone]
           ,[Fax]
           ,[Address1]
           ,[Address2]
           ,[NationalId]          
           ,[PostalCode]
           ,[Country]
           ,[Active],CommonCus,[AddBy],[AddDate])
     VALUES
           ('" + ci.Code + "','" + ci.CustomerName + "','" + ci.Email + "','" + ci.Mobile + "','" + ci.Phone + "','" + ci.Fax + "','" + ci.Address1 + "','" + ci.Address2 + "','" + ci.NationalId + "','" + ci.PostalCode + "','" + ci.Country + "','" + ci.Active + "','" + ci.CommonCus + "','" + ci.LoginBy + "','" + Globals._localTime.ToString() + "')";
            DataManager.ExecuteNonQuery(connectionString, query);
            sqlCon.Close();
        }

        public static void UpdateClientInfo(clsClientInfo ci)
        {
            SqlConnection connection = new SqlConnection(DataManager.OraConnString());
            SqlTransaction transaction;
            try
            {
                connection.Open();
                transaction = connection.BeginTransaction();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.Transaction = transaction;

                command.CommandText = @"update Customer set ContactName='" + ci.CustomerName + "',City='" +
                                      ci.NationalId + "', Address1= '" + ci.Address1 + "', Address2= '" + ci.Address2 +
                                      "', Phone= '" + ci.Phone + "', Mobile='" + ci.Mobile + "',Fax='" + ci.Fax +
                                      "', Active= '" + ci.Active + "' ,[UpdateBy]='" + ci.LoginBy +
                                      "',[UpdateDate]='" + Globals._localTime.ToString() + "',CommonCus='" + ci.CommonCus + "' where ID='" + ci.ID +
                                      "' ";
                command.ExecuteNonQuery();

                //*********** Auto Coa generate off **********//
                //command.CommandText = @"UPDATE [GL_SEG_COA] SET [SEG_COA_DESC] ='Accounts Receivable from-Customer-" + ci.CustomerName + "'  WHERE [SEG_COA_CODE]='" + ci.GlCoa + "'";
                //command.ExecuteNonQuery();

                //command.CommandText = @"UPDATE [GL_COA] SET [COA_DESC] ='" + ci.GlCoaDesc + "' where [GL_COA_CODE]='1-" + ci.GlCoa + "'";
                //command.ExecuteNonQuery();

                transaction.Commit();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            } 

        }

        public static void DeleteClientInfo(clsClientInfo ci)
        {
            SqlConnection connection = new SqlConnection(DataManager.OraConnString());
            SqlTransaction transaction;
            try
            {
                connection.Open();
                transaction = connection.BeginTransaction();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.Transaction = transaction;

                command.CommandText = @"update Customer set [DeleteBy]='" + ci.LoginBy +
                                      "',[DeleteDate]='" + Globals._localTime.ToString() + "' where ID='" + ci.ID +
                                      "' ";
                command.ExecuteNonQuery();

                //*********** Auto Coa generate off **********//
                //command.CommandText = @"delete from GL_SEG_COA where SEG_COA_CODE='" + ci.GlCoa + "' ";
                //command.ExecuteNonQuery();

                //command.CommandText = @"delete from GL_COA where COA_NATURAL_CODE='" + ci.GlCoa + "' ";
                //command.ExecuteNonQuery();

                transaction.Commit();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

        }

        public static clsClientInfo GetClientInfo(string ci)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "select * from Customer where ID = '" + ci + "' ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "client_info");
            sqlCon.Close();
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new clsClientInfo(dt.Rows[0]);
        }
        public static clsClientInfo GetClientInfoIdName(string ci)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "select client_id,client_name,national_id,address1,address2,phone,mobile,fax,email,url,status from client_info where upper(client_id + ' - '+client_name) = upper('" + ci + "') ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Client");
            sqlCon.Close();
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new clsClientInfo(dt.Rows[0]);
        }
        public static clsClientInfo GetClientInfoPp(string ci,string pp)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "select client_id,client_name,national_id,address1,address2,phone,mobile,fax,email,url,status from client_info where client_id = '" + ci + "' or passport='"+pp+"'";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Client");
            sqlCon.Close();
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new clsClientInfo(dt.Rows[0]);
        }
        public static string getClientName(string cid)
        {
            String ConnectionString = DataManager.OraConnString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            string Query = "select client_name from client_info where client_id='" + cid + "'";
            myConnection.Open();
            SqlCommand myCommand = new SqlCommand(Query, myConnection);
            object maxValue = myCommand.ExecuteScalar();
            myConnection.Close();
            if (maxValue != null)
            {
                return maxValue.ToString();
            }
            return "";
        }

        public static DataTable GetCommonClient()
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "select ID,ContactName,Code from Customer where CommonCus='1'";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Client");
            return dt;
        }
      
        public static DataTable GetShowSupplierOnPayment(string p)
        {
            String connectionString = DataManager.OraConnString();
            string query = @"SELECT ID,Code,[ContactName],Gl_CoaCode  FROM [Customer] where UPPER([Code]+'-'+[ContactName]+'-'+[Mobile])=UPPER('" + p + "') and Active='True'";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Customer");
            return dt;
        }
        public static int GetShowPaymentID()
        {
            SqlConnection connection = new SqlConnection(DataManager.OraConnString());
            try
            {
                connection.Open();
                string Query = @"SELECT top(1)[ID]  FROM [SupplierPayment] order by [ID] desc";
                SqlCommand command = new SqlCommand(Query, connection);
                return Convert.ToInt32(command.ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        public static DataTable GetShowCustomerHistory(string P,string P_Type)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string Parameter = "";
            if (P != "") { Parameter = "Where t1.Chk_Status not in ('A') and t1.Customer_id='" + P + "' and Payment_Type='" + P_Type + "' order by ID desc"; } else { Parameter = "Where  t1.Chk_Status not in ('A') and  Payment_Type='" + P_Type + "' order by ID desc"; }
            string query = @"SELECT top(50) t1.[ID]
              ,t2.Code
              ,t2.ContactName
              ,CONVERT(nvarchar,t1.[Date],103) AS PmDate           
              ,t1.[PayAmt] 
              ,t1.ChequeNo
              ,CASE WHEN t1.Chk_Status='P' THEN 'Pending' WHEN t1.Chk_Status='A' THEN 'Approved' WHEN t1.Chk_Status='B' THEN 'Bounce' ELSE '' END AS[Chk_Status]
          FROM [CustomerPaymentReceive] t1 inner join Customer t2 on t2.ID=t1.Customer_id " + Parameter;
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "CustomerPaymentReceive");
            return dt;
        }
        public static DataTable GetShowCheckNubber(string ChkId)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = @"SELECT t1.[ID]
      ,convert(nvarchar,t1.Date,103) AS PmDate
      ,t1.Invoice
      ,t1.Customer_id
      ,t3.ContactName
      ,t3.Gl_CoaCode
      ,t1.[PayAmt]
      ,t1.[PayMethod]
      ,t1.[Bank_id]
      ,t1.[ChequeNo]
      ,convert(nvarchar,t1.[ChequeDate],103) AS [ChequeDate]      
      ,t1.Chk_Status
  FROM CustomerPaymentReceive t1 inner join [Order] t2 on t2.ID=t1.Invoice inner join Customer t3 on t3.ID=t2.CustomerID where t1.[ChequeNo]='" + ChkId + "'";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "SupplierPayment");
            return dt;
        }

        public static DataTable GetShowPartyInfo(string p)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = @"SELECT  tt.ID,tt.ContactName FROM [Customer] tt where UPPER([ContactName]+' - '+[Mobile]) LIKE UPPER('%" + p + "%')";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "SupplierPayment");
            return dt;
        }
       
    }
}
