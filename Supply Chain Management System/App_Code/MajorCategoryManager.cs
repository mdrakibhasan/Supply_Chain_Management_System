using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

using System.Data.SqlClient;
using autouniv;


/// <summary>
/// Summary description for MajorCategoryManager
/// </summary>
public class MajorCategoryManager
{
	public MajorCategoryManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public static DataTable GetMajorCats()
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = "Select * from [View_GetItemCatagory]  ";

        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "View_GetItemCatagory");
        return dt;
    }

    public static MajorCategory GetMajorCat(string mjr)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = "SELECT [ID],[Code],[Name],[Description],[Active] FROM [Category] where ID='"+mjr+"' order by ID desc  ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Mjr");
        if (dt.Rows.Count == 0)
        {
            return null;
        }
        return new MajorCategory(dt.Rows[0]);
    }

    public static void DeleteMajorCat(MajorCategory majcat)
    {
        String connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = " update Category set [DeleteBy]='" + majcat.LoginBy + "',[DeleteDate]='" + Globals._localTime.ToString() + "' where ID= '" + majcat.ID + "'";
        DataManager.ExecuteNonQuery(connectionString, query);
    }

    public static void CreateMajorCat(MajorCategory majcat)
    {
        String connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);

        string query = " insert into Category(Name,Description,Active,Code,[AddBy],[AddDate]) " +
                       " values ('" + majcat.Name + "', '" + majcat.Description + "', '" + majcat.Active + "','" + majcat.Code + "','" + majcat.LoginBy + "','" + Globals._localTime.ToString() + "')";

        DataManager.ExecuteNonQuery(connectionString, query);
    }

    public static void UpdateMajorCat(MajorCategory majcat)
    {
        String connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);

        string query = " update Category set Name='" + majcat.Name + "' , Description= '" + majcat.Description + "',Active='" + majcat.Active + "',[UpdateBy]='" + majcat.LoginBy + "',[UpdateDate]='" + Globals._localTime.ToString() + "' where ID= '" + majcat.ID + "'";

        DataManager.ExecuteNonQuery(connectionString, query);
    }

    //***************************** New Code Catagory And Sub Catagory Code **************************//
   
}