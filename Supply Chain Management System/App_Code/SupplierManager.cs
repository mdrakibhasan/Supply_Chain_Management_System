using System;
using System.Data;
using System.Configuration;
using System.Linq;






using System.Xml.Linq;
using System.Data.SqlClient;

using autouniv;

/// <summary>
/// Summary description for SupplierManager
/// </summary>
/// 
namespace OldColor
{    
    public class SupplierManager
    {
        
        public static DataTable GetSuppliers()
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "select t1.*,t2.COUNTRY_DESC from Supplier t1 inner join COUNTRY_INFO t2 on t2.COUNTRY_CODE=t1.Country WHERE t1.[DeleteBy] IS NULL order by ID DESC ";

            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Supplier");
            return dt;
        }
        public static Supplier GetSupplier(string sup)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "select * from Supplier where ID='" + sup + "' ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Supplier");
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new Supplier(dt.Rows[0]);
        }
        public static void DeleteSupplier(Supplier sup)
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

                command.CommandText = @"UPDATE [Supplier]
   SET  [DeleteBy] ='" + sup.LoginBy + "' ,[DeleteDate] ='" + Globals._localTime.ToString() + "'  WHERE ID='" + sup.ID + "'";
                command.ExecuteNonQuery();

                //*********** Auto Coa generate off **********//
                //command.CommandText = @"delete from GL_SEG_COA where SEG_COA_CODE='" + sup.GlCoa + "' ";
                //command.ExecuteNonQuery();

                //command.CommandText = @"delete from GL_COA where COA_NATURAL_CODE='" + sup.GlCoa + "' ";
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
        public static void CreateSupplier(Supplier sup)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);

            string query = @"INSERT INTO [Supplier]
           ([Code],[Name],[ContactName],[Designation],[Email],[Phone],[Fax],[Mobile],[Address1],[Address2],[City],[State],[PostalCode],[Country],[SupplierGroupID],[Active],[AddBy],[AddDate],Gl_CoaCode)
     VALUES
           ('" + sup.SupCode + "','" + sup.ComName + "','" + sup.SupName + "','" + sup.Designation + "','" + sup.Email + "','" + sup.SupPhone + "','" + sup.Fax + "','" + sup.SupMobile + "','" + sup.SupAddr1 + "','" + sup.SupAddr2 + "','" + sup.City + "','" + sup.State + "','" + sup.PostCode + "','" + sup.Country + "','" + sup.SupGroup + "','" + sup.Active + "','" + sup.LoginBy + "','" + Globals._localTime.ToString() + "','" + sup.GlCoa + "')";

            DataManager.ExecuteNonQuery(connectionString, query);

        }
        public static void UpdateSupplier(Supplier sup)
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

                 command.CommandText = @"UPDATE [Supplier]
   SET [Name] ='" + sup.ComName + "' ,[ContactName] ='" + sup.SupName + "' ,[Designation] ='" + sup.Designation + "' ,[Email] ='" + sup.Email + "',[Phone] = '" + sup.SupPhone + "',[Fax] ='" + sup.Fax + "' ,[Mobile] ='" + sup.SupMobile + "' ,[Address1] ='" + sup.SupAddr1 + "' ,[Address2] ='" + sup.SupAddr2 + "' ,[City] ='" + sup.City + "' ,[State] ='" + sup.State + "' ,[PostalCode] ='" + sup.PostCode + "',[Country] ='" + sup.Country + "',[SupplierGroupID] ='" + sup.SupGroup + "' ,[Active] ='" + sup.Active + "' ,[UpdateBy] ='" + sup.LoginBy + "' ,[UpdateDate] ='" + Globals._localTime.ToString() + "'  WHERE ID='" + sup.ID + "'";
                 command.ExecuteNonQuery();
                 //*********** Auto Coa Generate off **********//
                 //command.CommandText = @"UPDATE [GL_SEG_COA] SET [SEG_COA_DESC] ='" + sup.SupName + "'  WHERE [SEG_COA_CODE]='" + sup.GlCoa + "'";
                 //command.ExecuteNonQuery();

                 //command.CommandText = @"UPDATE [GL_COA] SET [COA_DESC] ='" + sup.CoaDesc + "' where [GL_COA_CODE]='1-" + sup.GlCoa + "'";
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
        public static string GetSupplierName(string sup)
        {
            String ConnectionString = DataManager.OraConnString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            string Query = "select sup_desc from supplier where sup_code='" + sup + "'";
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

        //********************* Supplier & Party Payment **************************//

        public static DataTable GetShowSupplierOnPayment(string SupplierName,string PatyName,string SearchType)
        {
            String connectionString = DataManager.OraConnString();
            string query = "";
            if (SearchType == "S")
            {
                query = @"SELECT ID,Code,[ContactName],Gl_CoaCode  FROM [Supplier] where UPPER([Code]+'-'+[ContactName]+'-'+[Mobile])='" + SupplierName + "' and Active='True'";
            }
            else
            {
                query = @"SELECT ID,PartyCode AS Code,PartyName AS ContactName,Gl_CoaCode FROM PartyInfo where upper([PartyCode]+' - '+[PartyName])='" + PatyName + "' ";
            }

            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "party");
            return dt;
        }
        public static DataTable GetShowSupplierHistory(string ID,string P_Type,string SupplierOrParty)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string Parameter = "";
            string query="";
            if (SupplierOrParty == "S")
            {
                if (ID == "") { Parameter = "WHERE t1.Chk_Status not in ('A') and Payment_Type='" + P_Type + "' order by ID desc"; } else { Parameter = "Where t1.Chk_Status not in ('A') and t1.supplier_id='" + ID + "' and Payment_Type='" + P_Type + "' order by ID desc"; }
                query = @"SELECT top(50) t1.[ID]
              ,t2.Code
              ,t2.ContactName
              ,CONVERT(nvarchar,t1.[PmDate],103) AS PmDate           
              ,t1.[PayAmt]
              ,CASE WHEN t1.Chk_Status='P' THEN 'Pending' WHEN t1.Chk_Status='A' THEN 'Approved' WHEN t1.Chk_Status='B' THEN 'Bounce' ELSE '' END AS CHK_Status  
              ,t1.ChequeNo           
          FROM [SupplierPayment] t1 inner join Supplier t2 on t2.ID=t1.supplier_id " + Parameter;
            }
            else
            {
                if (ID == "") { Parameter = "WHERE t1.Chk_Status not in ('A') and Payment_Type='" + P_Type + "' order by ID desc"; } else { Parameter = "Where t1.Chk_Status not in ('A') and t1.supplier_id='" + ID + "' and Payment_Type='" + P_Type + "' order by ID desc"; }

                query = @"SELECT top(50) t1.[ID]
              ,t2.PartyCode AS Code
              ,t2.PartyName AS ContactName
              ,CONVERT(nvarchar,t1.[PmDate],103) AS PmDate           
              ,t1.[PayAmt]
              ,CASE WHEN t1.Chk_Status='P' THEN 'Pending' WHEN t1.Chk_Status='A' THEN 'Approved' WHEN t1.Chk_Status='B' THEN 'Bounce' ELSE '' END AS CHK_Status  
              ,t1.ChequeNo           
          FROM [SupplierPayment] t1 inner join PartyInfo t2 on t2.ID=t1.supplier_id  " + Parameter;
            }
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Supplier");
            return dt;
        }
    }
}