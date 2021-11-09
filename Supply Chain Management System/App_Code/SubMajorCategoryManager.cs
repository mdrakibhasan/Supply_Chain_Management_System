using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

using System.Data.SqlClient;
using autouniv;
using OldColor;

/// <summary>
/// Summary description for SubMajorCategoryManager
/// </summary>
public class SubMajorCategoryManager
{
	public SubMajorCategoryManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public static DataTable GetSubMajorCategories(string mjr)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = "select ID,Name from SubCategory where CategoryID='" + mjr + "' AND [DeleteBy] IS NULL order by 1";

        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "SubMajorCatInfo");
        return dt;
    }

    public static SubMajorCategory GetSubMajorCat(string submjr)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = "SELECT [ID],[Code],[Name],[Description],[CategoryID],[Active] FROM [SubCategory] where ID= '" + submjr + "' ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "SubCategory");
        if (dt.Rows.Count == 0)
        {
            return null;
        }
        return new SubMajorCategory(dt.Rows[0]);
    }

    public static void DeleteSubMajorCat(SubMajorCategory submajcat)
    {
        String connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = " update SubCategory set [DeleteBy]='" +
                       submajcat.LoginBy + "',[DeleteDate]='" + Globals._localTime.ToString() + "' where  ID= '" + submajcat.ID + "'";
        DataManager.ExecuteNonQuery(connectionString, query);
    }

    public static void CreateSubMajorCat(SubMajorCategory submajcat)
    {
        String connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);

        string query = " insert into SubCategory(Name,Description,CategoryID,Active,Code,[AddBy],[AddDate]) values ('" +
                       submajcat.Name + "', '" + submajcat.Description + "','" + submajcat.Catagory + "','" +
                       submajcat.Active + "','" + submajcat.Code + "','" + submajcat.LoginBy + "','" + Globals._localTime.ToString() + "')";

        DataManager.ExecuteNonQuery(connectionString, query);
    }

    public static void UpdateSubMajorCat(SubMajorCategory submajcat)
    {
        String connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);

        string query = " update SubCategory set Name= '" + submajcat.Name + "',Description='" + submajcat.Description +
                       "',CategoryID='" + submajcat.Catagory + "',Active='" + submajcat.Active + "',[UpdateBy]='" +
                       submajcat.LoginBy + "',[UpdateDate]='" + Globals._localTime.ToString() + "' where  ID= '" + submajcat.ID + "'";

        DataManager.ExecuteNonQuery(connectionString, query);
    }
    //**************************************b New Code *************************************//


    public static object getshowSubCatagory()
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = @"SELECT t1.[ID] AS sub_mjr_code  
      ,t1.[Name] AS sub_mjr_desc
      ,t1.[Description] 
      ,t1.[CategoryID] AS mjr_code
      ,t2.Name AS MJR_DESC
      ,t1.[Active]  
      ,t1.Code    
  FROM [SubCategory] t1 inner join Category t2 on t2.ID=t1.CategoryID WHERE t1.[DeleteBy] IS Null order by t1.ID DESC";

        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "SubMajorCatInfo");
        return dt;
    }

    public static int ShowSubCatagoryId()
    {           
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        connection.Open();
        string selectQuery = @"(SELECT isnull(max([SUB_MJR_CODE]),0) FROM [SUB_MAJ_CAT])";
        SqlCommand command = new SqlCommand(selectQuery,connection);
        return Convert.ToInt32(command.ExecuteScalar()); 
    }
}