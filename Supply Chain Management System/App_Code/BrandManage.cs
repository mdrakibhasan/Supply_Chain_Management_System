using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;
using System.Data.SqlClient;
using autouniv;

/// <summary>
/// Summary description for CompanyManage
/// </summary>
/// 
namespace OldColor
{
    public class BrandManage
    {
        public static DataTable GetCompanies()
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "select ID AS com_id,BrandName AS com_desc,[Active] AS [check] from Brand where DeleteBy IS NULL order by ID";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Brand");
            return dt;
        }
        public static void CreateCompany(Brand comp)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);

            string query = " insert into Brand(BrandName,Active,AddBy,AddDate) values ('" + comp.CompanyDesc + "','" + comp.Active + "','" + comp.LoginBy + "','" + Globals._localTime.ToString() + "')";
            
            DataManager.ExecuteNonQuery(connectionString, query);
           
        }

        public static void UpdateCompany(Brand comp)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);

            string query = " update Brand set BrandName='" + comp.CompanyDesc + "',Active='" + comp.Active + "',UpdateBy='" + comp.LoginBy + "',UpdateDate='" + Globals._localTime.ToString() + "' where ID='" + comp.CompanyId + "'";
           
            DataManager.ExecuteNonQuery(connectionString, query);
           
        }

        public static void DeleteCompany(Brand comp)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);

            string query = " update Brand set DeleteBy='" + comp.LoginBy + "',DeleteDate='" + Globals._localTime.ToString() + "' where ID='" + comp.CompanyId + "'";

            DataManager.ExecuteNonQuery(connectionString, query);
            
        }

        public static Brand GetCompany(System.String comid)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "select * from Brand where ID = '" + comid + "'";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Brand");
            sqlCon.Close();
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new Brand(dt.Rows[0]);
        }
    }
}