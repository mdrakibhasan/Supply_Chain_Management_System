using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using autouniv;

/// <summary>
/// Summary description for TaxManager
/// </summary>
public class TaxManager
{
	public TaxManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public static DataTable GetShowTaxInformation()
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection oracon = new SqlConnection(connectionString);
        string query = @"SELECT [ID] AS TaxCode ,[Name] AS TaxType ,[Rate] AS TaxRate ,[Active] AS [check] FROM [TaxCategory] WHERE [DeleteBy] IS NULL ORDER BY ID DESC ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "TaxCategory");
        return dt;
    }

    public static void CreateTax(string Code, string Type, string Rate, string Chk,string Login)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection oracon = new SqlConnection(connectionString);
        string query = @"INSERT INTO [TaxCategory]
           ([Name],[Rate],[Active],[AddBy],[AddDate])
     VALUES
           ( '" + Type + "','" + Rate + "' ,'" + Chk + "' , '" + Login + "', '" + Globals._localTime.ToString() + "')";
        DataManager.ExecuteNonQuery(connectionString, query);
    }

    public static void DeleteTax(string ID, string LoginBy)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection oracon = new SqlConnection(connectionString);
        string query = @"UPDATE [TaxCategory] SET [DeleteBy] ='" + LoginBy + "' ,[DeleteDate] ='" + Globals._localTime.ToString() + "'  WHERE ID='" + ID + "'";
        DataManager.ExecuteNonQuery(connectionString, query);
    }

    public static void UpdateTax(string Code, string Type, string Rate, string Chk, string LoginBy)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection oracon = new SqlConnection(connectionString);
        string query = @"UPDATE [TaxCategory] SET [Name] ='" + Type + "',[Rate] ='" + Rate + "' ,[Active] ='" + Chk + "' ,[UpdateBy] ='" + LoginBy + "' ,[UpdateDate] ='" + Globals._localTime.ToString() + "'  WHERE ID='" + Code + "'";
        DataManager.ExecuteNonQuery(connectionString, query);
    }
}