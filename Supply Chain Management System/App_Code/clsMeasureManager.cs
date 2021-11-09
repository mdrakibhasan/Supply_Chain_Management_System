using System;
using System.Data;
using System.Configuration;
using System.Linq;






using System.Xml.Linq;
using System.Data.SqlClient;
using autouniv;


/// <summary>
/// Summary description for clsMeasureManager
/// </summary>
/// 
namespace OldColor
{
    public class clsMeasureManager
    {
        public static DataTable GetMeasures()
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "select ID AS msr_unit_code, Name AS msr_unit_desc from UOM WHERE DeleteBy IS NULL order by ID DESC ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "UOM");
            return dt;
        }
        public static void CreateMeasure(clsMeasure msr)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = " insert into UOM (Name,Active,[AddBy],[AddDate]) values ('" + msr.MsrUnitDesc + "','True','" + msr.LoginBy + "','" + Globals._localTime.ToString() + "')";
            DataManager.ExecuteNonQuery(connectionString, query);
            sqlCon.Close();
        }

        public static void UpdateMeasure(clsMeasure msr)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = " update UOM set Name='" + msr.MsrUnitDesc + "',[UpdateBy]='" + msr.LoginBy + "',[UpdateDate]='" + Globals._localTime.ToString() + "' where ID='" + msr.MsrUnitCode + "'";
            DataManager.ExecuteNonQuery(connectionString, query);
            sqlCon.Close();
        }

        public static void DeleteMeasure(clsMeasure msr)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = " update UOM set [DeleteBy]='" + msr.LoginBy + "',[DeleteDate]='" + Globals._localTime.ToString() + "' where ID='" + msr.MsrUnitCode + "'";
            DataManager.ExecuteNonQuery(connectionString, query);
            sqlCon.Close();
        }

        public static clsMeasure GetMeasure(System.String msr)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "select * from UOM where ID = '" + msr + "'";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "UOM");
            sqlCon.Close();
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new clsMeasure(dt.Rows[0]);
        }
    }
}