using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

using System.Data.SqlClient;
using autouniv;
using DocumentFormat.OpenXml.Office2010.Excel;


/// <summary>
/// Summary description for ClsItemDetailsManager
/// </summary>
public class ClsItemDetailsManager
{
	public ClsItemDetailsManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public void SaveItemDetailsInformation(ClsItemDetailsInfo aClsItemDetailsInfoObj)
    {
        string connectionString = DataManager.OraConnString();
        string InsertQuery = @"INSERT INTO [RMS].[dbo].[T_ItemDetails]
           ([Item_Name]
           ,[Item_Descriptions]
           ,[Unit_ID]
           ,[Item_Quantity]
           ,[Unit_Price])
     VALUES('" + aClsItemDetailsInfoObj.ItemName + "','" + aClsItemDetailsInfoObj.Description + "','" + aClsItemDetailsInfoObj.UnitId + "','" + aClsItemDetailsInfoObj.Quantity + "','" + aClsItemDetailsInfoObj.Price + "')";

        DataManager.ExecuteNonQuery(connectionString, InsertQuery);
    }

    public void UpdateItemDetailsInformation(ClsItemDetailsInfo aClsItemDetailsInfoObj)
    {
        string connectionString = DataManager.OraConnString();
        string UpdateQuery = @"UPDATE [RMS].[dbo].[T_ItemDetails]
   SET [Item_Name] = '" + aClsItemDetailsInfoObj.ItemName + "',[Item_Descriptions] = '" + aClsItemDetailsInfoObj.Description + "',[Unit_ID] = '" + aClsItemDetailsInfoObj.UnitId + "',[Item_Quantity] = '" + aClsItemDetailsInfoObj.Quantity + "',[Unit_Price] = '" + aClsItemDetailsInfoObj.Price + "' WHERE [Item_ID]= '" + aClsItemDetailsInfoObj.ItemId + "' ";

        DataManager.ExecuteNonQuery(connectionString, UpdateQuery);
    }

    public void DeleteItemDetailsInformation(ClsItemDetailsInfo aClsItemDetailsInfoObj)
    {
        string connectionString = DataManager.OraConnString();
        string DeleteQuery = @"DELETE FROM [RMS].[dbo].[T_ItemDetails]
      WHERE [Item_ID]= '"+aClsItemDetailsInfoObj.ItemId+"' ";

        DataManager.ExecuteNonQuery(connectionString, DeleteQuery);
    }

    public DataTable GetItemDetailsInformation()
    {
        string connectionString = DataManager.OraConnString();
        string SelectQuery = @"select i.Item_ID,i.Item_Name,i.Item_Descriptions,u.Unit,i.Item_Quantity,i.Unit_Price from T_ItemDetails i,T_UnitofMeans u where i.Unit_ID=u.Unit_ID";

        DataTable dt = DataManager.ExecuteQuery(connectionString, SelectQuery, "T_ItemDetails");
        return dt;
    }

    public static DataTable GetProductNameList()
    {
        string connectionString = DataManager.OraConnString();
        string selectQuery = @"select Item_ID,Item_Name from T_ItemDetails";

        DataTable dt = DataManager.ExecuteQuery(connectionString, selectQuery, "T_ItemDetails");
        return dt;
    }

    //************************** New Code **********************************//
    public static void SaveItemsInformation(ClsItemDetailsInfo aClsItemDetailsInfoObj, DataTable dtColor, DataTable dtSize)
    {
        String connectionString = DataManager.OraConnString();
        using (SqlConnection sqlCon = new SqlConnection(connectionString))
        {
            string query = "";
            if (aClsItemDetailsInfoObj.ItemsImage != null)
            {
                query = @"INSERT INTO [Item]
 ([Code],[Name],[ItemSize],[ItemColor],[UOMID],[UnitPrice],[Currency],[OpeningStock],[OpeningAmount],[ClosingStock],[ClosingAmount],[CategoryID],[SubCategoryID],[TaxCategoryID],[Discounted],[DiscountAmount],[Active],[IsNew],[AddBy],[AddDate],ItemImage,description,Brand,ShortName,StyleNo,SecurityCode,ItemsType,BranchSalesPrice)
     VALUES
           ('" + aClsItemDetailsInfoObj.ItemsCode + "','" + aClsItemDetailsInfoObj.ItemsName + "','0','0','" +
                        aClsItemDetailsInfoObj.Umo + "','" + aClsItemDetailsInfoObj.UnitPrice + "','" +
                        aClsItemDetailsInfoObj.Currency + "','" + aClsItemDetailsInfoObj.OpeningStock + "','" +
                        aClsItemDetailsInfoObj.OpeningAmount + "','" + aClsItemDetailsInfoObj.ClosingStock + "','" +
                        aClsItemDetailsInfoObj.ClosingAmount + "','" + aClsItemDetailsInfoObj.Catagory + "','" +
                        aClsItemDetailsInfoObj.SubCatagory + "','" + aClsItemDetailsInfoObj.Text + "','" +
                        aClsItemDetailsInfoObj.DiscountCheck + "','" + aClsItemDetailsInfoObj.Discount + "','" +
                        aClsItemDetailsInfoObj.Active + "','True','" + aClsItemDetailsInfoObj.LoginBy +
                        "','" + Globals._localTime.ToString() + "',@img,'" + aClsItemDetailsInfoObj.Description + "','" + aClsItemDetailsInfoObj.Brand +
                        "','" + aClsItemDetailsInfoObj.ShortName + "','" + aClsItemDetailsInfoObj.StyleNo + "','" +
                        aClsItemDetailsInfoObj.SecurityCode + "','" + aClsItemDetailsInfoObj.ItemsType + "','" +
                        aClsItemDetailsInfoObj.BranchSalesPrice + "')";
            }
            else
            {
                query = @"INSERT INTO [Item]
 ([Code],[Name],[ItemSize],[ItemColor],[UOMID],[UnitPrice],[Currency],[OpeningStock],[OpeningAmount],[ClosingStock],[ClosingAmount],[CategoryID],[SubCategoryID],[TaxCategoryID],[Discounted],[DiscountAmount],[Active],[IsNew],[AddBy],[AddDate],ItemImage,description,Brand,ShortName,StyleNo,SecurityCode,ItemsType,BranchSalesPrice)
     VALUES
           ('" + aClsItemDetailsInfoObj.ItemsCode + "','" + aClsItemDetailsInfoObj.ItemsName + "','0','0','" +
                        aClsItemDetailsInfoObj.Umo + "','" + aClsItemDetailsInfoObj.UnitPrice + "','" +
                        aClsItemDetailsInfoObj.Currency + "','" + aClsItemDetailsInfoObj.OpeningStock + "','" +
                        aClsItemDetailsInfoObj.OpeningAmount + "','" + aClsItemDetailsInfoObj.ClosingStock + "','" +
                        aClsItemDetailsInfoObj.ClosingAmount + "','" + aClsItemDetailsInfoObj.Catagory + "','" +
                        aClsItemDetailsInfoObj.SubCatagory + "','" + aClsItemDetailsInfoObj.Text + "','" +
                        aClsItemDetailsInfoObj.DiscountCheck + "','" + aClsItemDetailsInfoObj.Discount + "','" +
                        aClsItemDetailsInfoObj.Active + "','True','" + aClsItemDetailsInfoObj.LoginBy +
                        "','" + Globals._localTime.ToString() + "',null,'" + aClsItemDetailsInfoObj.Description + "','" + aClsItemDetailsInfoObj.Brand +
                        "','" + aClsItemDetailsInfoObj.ShortName + "','" + aClsItemDetailsInfoObj.StyleNo + "','" +
                        aClsItemDetailsInfoObj.SecurityCode + "','" + aClsItemDetailsInfoObj.ItemsType + "','" +
                        aClsItemDetailsInfoObj.BranchSalesPrice + "')";
            }
            SqlParameter img = new SqlParameter();
            img.SqlDbType = SqlDbType.VarBinary;
            img.ParameterName = "img";
            img.Value = aClsItemDetailsInfoObj.ItemsImage;            
            using (SqlCommand cmnd = new SqlCommand(query, sqlCon))
            {
                cmnd.Parameters.Add(img);
                if (aClsItemDetailsInfoObj.ItemsImage == null)
                {
                    cmnd.Parameters.Remove(img);
                }                
                sqlCon.Open();
                cmnd.ExecuteNonQuery();
                sqlCon.Close();
            }
            int ItemsID = IdManager.GetShowSingleValueInt("TOP(1)ID", "Item ORDER BY ID DESC");
            foreach (DataRow drColor in dtColor.Rows)
            {
                string queryColor = @"INSERT INTO [ItemColor]
                   ([ItemID],[ColorID],[AddBy],[AddDate])
                     VALUES
                   (" + ItemsID + ",'" + drColor["ID"].ToString() + "','" + aClsItemDetailsInfoObj.LoginBy + "','" + Globals._localTime.ToString() + "')";
                SqlCommand command1 = new SqlCommand(queryColor, sqlCon);
                sqlCon.Open();
                command1.ExecuteNonQuery();
                sqlCon.Close();
            }
            foreach (DataRow drSize in dtSize.Rows)
            {
                string queryColor = @"INSERT INTO [ItemSize]
                   ([ItemID],[SizeID],[AddBy],[AddDate])
                     VALUES
                   (" + ItemsID + ",'" + drSize["ID"].ToString() + "','" + aClsItemDetailsInfoObj.LoginBy + "','" + Globals._localTime.ToString() + "')";
                SqlCommand command2 = new SqlCommand(queryColor, sqlCon);
                sqlCon.Open();
                command2.ExecuteNonQuery();
                sqlCon.Close();
            }
        }
    }
    public static void UpdateItemsInformation(ClsItemDetailsInfo aClsItemDetailsInfoObj, DataTable dtColor, DataTable dtSize)
    {
        String connectionString = DataManager.OraConnString();
        using (SqlConnection sqlCon = new SqlConnection(connectionString))
        {
            string query = "";
            if (aClsItemDetailsInfoObj.ItemsImage != null)
            {
                query = @"UPDATE [Item]
   SET [Name] ='" + aClsItemDetailsInfoObj.ItemsName + "',[UOMID] ='" + aClsItemDetailsInfoObj.Umo + "' ,[UnitPrice] ='" + aClsItemDetailsInfoObj.UnitPrice + "',[Currency] ='" + aClsItemDetailsInfoObj.Currency + "',[OpeningStock] ='" + aClsItemDetailsInfoObj.OpeningStock + "',[OpeningAmount] ='" + aClsItemDetailsInfoObj.OpeningAmount + "',[ClosingStock] ='" + aClsItemDetailsInfoObj.ClosingStock + "',[ClosingAmount] ='" + aClsItemDetailsInfoObj.ClosingAmount + "' ,[CategoryID] ='" + aClsItemDetailsInfoObj.Catagory + "',[SubCategoryID] ='" + aClsItemDetailsInfoObj.SubCatagory + "',[TaxCategoryID] ='" + aClsItemDetailsInfoObj.Text + "',[Discounted] ='" + aClsItemDetailsInfoObj.DiscountCheck + "',[DiscountAmount] ='" + aClsItemDetailsInfoObj.Discount + "' ,[Active] ='" + aClsItemDetailsInfoObj.Active + "',[UpdateBy] ='" + aClsItemDetailsInfoObj.LoginBy + "' ,[UpdateDate] ='" + Globals._localTime.ToString() + "' ,[description]='" + aClsItemDetailsInfoObj.Description + "',Brand='" + aClsItemDetailsInfoObj.Brand + "',ShortName='" + aClsItemDetailsInfoObj.ShortName + "',StyleNo='" + aClsItemDetailsInfoObj.StyleNo + "',ItemsType='" + aClsItemDetailsInfoObj.ItemsType + "',BranchSalesPrice='" + aClsItemDetailsInfoObj.BranchSalesPrice + "',SecurityCode='" + aClsItemDetailsInfoObj.SecurityCode + "',[ItemImage] =@img  WHERE [ID] ='" + aClsItemDetailsInfoObj.ID + "'";
            }
            else
            {
                query = @"UPDATE [Item]
   SET [Name] ='" + aClsItemDetailsInfoObj.ItemsName + "',[UOMID] ='" + aClsItemDetailsInfoObj.Umo + "' ,[UnitPrice] ='" + aClsItemDetailsInfoObj.UnitPrice + "',[Currency] ='" + aClsItemDetailsInfoObj.Currency + "',[OpeningStock] ='" + aClsItemDetailsInfoObj.OpeningStock + "',[OpeningAmount] ='" + aClsItemDetailsInfoObj.OpeningAmount + "',[ClosingStock] ='" + aClsItemDetailsInfoObj.ClosingStock + "',[ClosingAmount] ='" + aClsItemDetailsInfoObj.ClosingAmount + "' ,[CategoryID] ='" + aClsItemDetailsInfoObj.Catagory + "',[SubCategoryID] ='" + aClsItemDetailsInfoObj.SubCatagory + "',[TaxCategoryID] ='" + aClsItemDetailsInfoObj.Text + "',[Discounted] ='" + aClsItemDetailsInfoObj.DiscountCheck + "',[DiscountAmount] ='" + aClsItemDetailsInfoObj.Discount + "' ,[Active] ='" + aClsItemDetailsInfoObj.Active + "',[UpdateBy] ='" + aClsItemDetailsInfoObj.LoginBy + "' ,[UpdateDate] ='" + Globals._localTime.ToString() + "' ,[description]='" + aClsItemDetailsInfoObj.Description + "',Brand='" + aClsItemDetailsInfoObj.Brand + "',ShortName='" + aClsItemDetailsInfoObj.ShortName + "',StyleNo='" + aClsItemDetailsInfoObj.StyleNo + "',ItemsType='" + aClsItemDetailsInfoObj.ItemsType + "',BranchSalesPrice='" + aClsItemDetailsInfoObj.BranchSalesPrice + "',SecurityCode='" + aClsItemDetailsInfoObj.SecurityCode + "'  WHERE [ID] ='" + aClsItemDetailsInfoObj.ID + "'";
            }
            SqlParameter img = new SqlParameter();
            img.SqlDbType = SqlDbType.VarBinary;
            img.ParameterName = "img";
            img.Value = aClsItemDetailsInfoObj.ItemsImage;
            using (SqlCommand cmnd = new SqlCommand(query, sqlCon))
            {
                cmnd.Parameters.Add(img);
                if (aClsItemDetailsInfoObj.ItemsImage == null)
                {
                    cmnd.Parameters.Remove(img);
                }
                sqlCon.Open();
                cmnd.ExecuteNonQuery();
                sqlCon.Close();
            }
            string Query = @"UPDATE [ItemColor] SET [DeleteBy] ='" + aClsItemDetailsInfoObj.LoginBy +
                           "' ,[DeleteDate] ='" + Globals._localTime.ToString() + "' WHERE ItemID='" + aClsItemDetailsInfoObj.ID + "'";
            SqlCommand command11 = new SqlCommand(Query, sqlCon);
            sqlCon.Open();
            command11.ExecuteNonQuery();
            sqlCon.Close();
            string Query1 = @"UPDATE [ItemSize] SET [DeleteBy] ='" + aClsItemDetailsInfoObj.LoginBy +
                           "' ,[DeleteDate] ='" + Globals._localTime.ToString() + "' WHERE ItemID='" + aClsItemDetailsInfoObj.ID + "'";
            SqlCommand command12 = new SqlCommand(Query1, sqlCon);
            sqlCon.Open();
            command12.ExecuteNonQuery();
            sqlCon.Close();
            if (dtColor.Rows.Count>0)
            {
                foreach (DataRow drColor in dtColor.Rows)
                {
                    string queryColor = @"INSERT INTO [ItemColor]
                       ([ItemID],[ColorID],[AddBy],[AddDate])
                         VALUES
                       (" + aClsItemDetailsInfoObj.ID + ",'" + drColor["ID"].ToString() + "','" + aClsItemDetailsInfoObj.LoginBy + "','" + Globals._localTime.ToString() + "')";
                    SqlCommand command1 = new SqlCommand(queryColor, sqlCon);
                    sqlCon.Open();
                    command1.ExecuteNonQuery();
                    sqlCon.Close();
                }
            }

            if (dtSize.Rows.Count > 0)
            {

                foreach (DataRow drSize in dtSize.Rows)
                {
                    string queryColor = @"INSERT INTO [ItemSize]
                   ([ItemID],[SizeID],[AddBy],[AddDate])
                     VALUES
                   (" + aClsItemDetailsInfoObj.ID + ",'" + drSize["ID"].ToString() + "','" + aClsItemDetailsInfoObj.LoginBy + "','" + Globals._localTime.ToString() + "')";
                    SqlCommand command2 = new SqlCommand(queryColor, sqlCon);
                    sqlCon.Open();
                    command2.ExecuteNonQuery();
                    sqlCon.Close();
                }
            }

        }
    }
    public static void DeleteItemsInformation(ClsItemDetailsInfo aClsItemDetailsInfoObj)
    {
        string connectionString = DataManager.OraConnString();
        string SelectQuery = @"UPDATE [Item]
            SET [DeleteBy] ='" + aClsItemDetailsInfoObj.LoginBy + "' ,[DeleteDate] ='" + Globals._localTime.ToString() + "'  WHERE [ID] ='" + aClsItemDetailsInfoObj.ID + "'";
             DataManager.ExecuteNonQuery(connectionString, SelectQuery);   
    }
    public static DataTable getShowItemsHistoryDetails()
    {
        string connectionString = DataManager.OraConnString();
        string selectQuery = @"SELECT t1.ID
      ,t1.[Code],t1.StyleNo
      ,t1.[Name]  
      ,t4.BrandName as [Brand Name]        
      ,t2.Name as [Catagory]
      ,t3.Name as[Sub Catagory]
      ,t1.UnitPrice as[Unit Price]  
      ,t1.ClosingStock as [Closing Stock]        
  FROM [Item] t1
  left join Category t2 on t2.ID=t1.CategoryID left join SubCategory t3 on t3.ID=t1.SubCategoryID
  left join Brand t4 on t4.ID=t1.Brand where t1.DeleteBy IS NULL order by t1.ID DESC ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, selectQuery, "Item");
        return dt;
    }

    public static ClsItemDetailsInfo GetShowDetails(string ItemsID)
    {
        String connectionString = DataManager.OraConnString();
        string query = @"SELECT [ID],[ItemsType]
      ,[Code],[Name],[ItemImage],[ItemSize],[ItemColor],[UOMID],[UnitPrice],[Currency],[OpeningStock],[OpeningAmount],[ClosingStock],[ClosingAmount],[CategoryID],[SubCategoryID],[TaxCategoryID],[Discounted]
      ,isnull([DiscountAmount],0) as DiscountAmount
      ,[Active]
      ,[IsNew]      
      ,[description],Brand,ShortName,StyleNo,SecurityCode,BranchSalesPrice     
  FROM [Item] where [ID]='" + ItemsID + "'";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Item");
        if (dt.Rows.Count == 0)
        {
            return null;
        }
        return new ClsItemDetailsInfo(dt.Rows[0]);
    }
    public static DataTable ShowTextCatagory()
    {
        string connectionString = DataManager.OraConnString();
        string selectQuery = @"Select * from View_TexCatagory where Active=1";
        DataTable dt = DataManager.ExecuteQuery(connectionString, selectQuery, "View_TexCatagory");
        return dt;
    }

    public static DataTable GetShowItemsMinibar(string p)
    {
        string connectionString = DataManager.OraConnString();
        string selectQuery = @"SELECT [ID],[Name] 
FROM  [Item] where ([Code]+' - '+[Name]) LIKE '%" +p+"%' and [items_type]='1'";
        DataTable dt = DataManager.ExecuteQuery(connectionString, selectQuery, "Item");
        return dt;
    }

    public static int GetShowItemsDetailsInformation()
    {
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        try
        {
            connection.Open();
            string selectQuery = @"select isnull(max(convert(int,t1.Code)),0)+1  from Item t1";
            SqlCommand command = new SqlCommand(selectQuery,connection);
            return Convert.ToInt32(command.ExecuteScalar());
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
        finally
        {
            connection.Close();
        }
    }

    public static DataTable GetShowItemsInfo(string SR)
    {
        string connectionString = DataManager.OraConnString();
        string Parameter="";
        if (SR != "") { Parameter = " AND UPPER(t1.Code+' - '+t1.Name+' - '+Isnull(t2.BrandName,'')+' - '+Isnull(t3.Name,'')) LIKE '" + SR + "' "; }
        string selectQuery = @"select t1.ID,t1.Code,t1.Name,t1.UnitPrice AS UnitPrice,t1.ClosingStock AS ClosingStock,t1.ClosingAmount from Item t1
left join Brand t2 on t2.ID=t1.Brand
left join Category t3 on t3.ID=t1.CategoryID where t1.UnitPrice>0  and t1.ItemsType in (2,3) " + Parameter;
        //if (SR != "") { Parameter = " AND UPPER(t3.Code+' - '+t3.Name+' - '+Isnull(t1.BrandName,'')+' - '+Isnull(t2.Name,'')) LIKE '" + SR + "' "; }
        //string selectQuery = @"SELECT t3.ID,t3.Code,t3.Name,t.Price AS UnitPrice,t.Quantity AS ClosingStock,t3.ClosingAmount FROM ItemSalesStock t left join Item t3 on t3.ID=t.ItemsID left join Brand t1 on t3.Brand=t1.ID left join Category t2 on t2.ID=t3.CategoryID where t.Price>0 " + Parameter;
        DataTable dt = DataManager.ExecuteQuery(connectionString, selectQuery, "Item");
        return dt;
    }
   
    public static DataTable  GetShowItemsDetails(string Item, string SearchType)
    {
        string connectionString = DataManager.OraConnString();
        string parameter = "";

      
            if (Item != "")
            {
                parameter = " where upper(t1.[Code]+ ' - '+t1.[Name])=upper('" + Item + "') and t1.Active='True' and t1.DeleteBy IS NULL ";
            }

            else if (Item == "" && SearchType != "0")
            {


                parameter = " Where t1.Active='True' and t1.DeleteBy IS NULL   and ItemsType='" + SearchType + "' ";

            }

            else
            {
                parameter = " Where t1.Active='True' and t1.DeleteBy IS NULL ";
            }
            
        
        
        string selectQuery = @"SELECT t1.ID,t1.[Code]+' - '+t1.[Name] AS Items  
      ,t2.Name AS Catagory
      ,t3.Name AS SubCat
      ,t1.[UnitPrice]     
      ,t1.[OpeningStock]
      ,convert(decimal(18,2),t1.[OpeningAmount]) OpeningAmount   
      ,convert(decimal(18,2),t1.[ClosingStock]) ClosingStock
      ,convert(decimal(18,2),(ISNULL(t1.[UnitPrice],0)*ISNULL(t1.[ClosingStock],0))) AS [ClosingAmount]       
  FROM [Item] t1 left join Category t2 on t2.ID=t1.CategoryID left join SubCategory t3 on t3.ID=t1.SubCategoryID " + parameter + " ORDER BY convert(decimal(18,0),t1.[ClosingStock]) desc, t1.[Name] ASC";
        DataTable dt = DataManager.ExecuteQuery(connectionString, selectQuery, "Item");
        return dt;
    }
    public static DataTable GetShowItemsDetaile(string p,string Flag)
    {
        string connectionString = DataManager.OraConnString();
        string parameter = "";
        if (p != "") { parameter = " where upper(t1.[Code]+ ' - '+t1.[Name])=upper('" + p + "')"; }
        string selectQuery = @"SELECT t1.ID,t1.[Code]+' - '+t1.[Name] AS Items,t2.Name AS Catagory,t3.Name AS SubCat,t1.[UnitPrice],ISNULL(t4.Quantity,0) AS [ClosingStock]FROM [Item] t1 left join Category t2 on t2.ID=t1.CategoryID left join SubCategory t3 on t3.ID=t1.SubCategoryID LEFT join ItemSalesStock t4 on t4.ItemsID=t1.ID" + parameter;
        DataTable dt = DataManager.ExecuteQuery(connectionString, selectQuery, "Item");
        return dt;
    }
    public static DataTable GetShowItemsInfoSearch(string ItemsName)
    {
        string connectionString = DataManager.OraConnString();
        string selectQuery = @"SELECT t1.ID,t1.[Name]  FROM [Item] t1 Left  join Category t2 on t2.ID=t1.CategoryID where upper(t1.[Code]+' - '+t1.[Name]+' - '+Isnull(t2.Name,'')) like upper('%" + ItemsName + "%')";
        DataTable dt = DataManager.ExecuteQuery(connectionString, selectQuery, "Item");
        return dt;
    }

    public static DataTable GetBadStockInformation(int i)
    {
        string parameter="";
        if (i == 0)
        {
            parameter = "where t1.Quantity>0 group by t7.ShiftmentNO,t2.CartoonNo,t4.Code,t3.Quantity,t4.Name,t5.Name,t6.Name";
        }
        else if (i == 1)
        {
            parameter = "where t1.Lost_Qty>0 group by t7.ShiftmentNO,t2.CartoonNo,t4.Code,t3.Quantity,t4.Name,t5.Name,t6.Name";
        }
        else if (i == 2)
        {
            parameter = "where t1.Access_Qty>0 group by t7.ShiftmentNO,t2.CartoonNo,t4.Code,t3.Quantity,t4.Name,t5.Name,t6.Name";
        }
        string connectionString = DataManager.OraConnString();
        string selectQuery = @"select t4.Code+' - '+t4.Name ItemsCodeWithName,t5.Name as CategotyName,t6.Name as SubCategotyName,t7.ShiftmentNO,t2.CartoonNo,t3.Quantity as TotalQuantity,sum(t1.Quantity) as BadQuantity from ItemBadStock t1 
inner join ShiftmentBoxingMst t2 on t1.ShiftmentBoxingMstID=t2.ID
inner join ShiftmentBoxingItemsDtl t3 on t2.ID=t3.MasterID
inner join Item t4 on t3.ItemsID=t4.ID
left join Category t5 on t5.ID=t4.CategoryID
left join SubCategory t6 on t4.SubCategoryID=t6.ID
left join ShiftmentAssigen t7 on t7.ID=t1.ItemsID "+ parameter;
        DataTable dt = DataManager.ExecuteQuery(connectionString, selectQuery, "ItemBadStock");
        return dt;
    }

    public static DataTable GetShowSalesItemsInfo(string p)
    {
        string connectionString = DataManager.OraConnString();
        string SelectQuery = @"SELECT t3.ID,t3.Code,t3.Name,t.Price AS UnitPrice,t.Quantity AS ClosingStock,t3.ClosingAmount,tt.ShiftmentNO FROM ItemSalesStock t left join Item t3 on t3.ID=t.ItemsID left join Brand t1 on t3.Brand=t1.ID left join Category t2 on t2.ID=t3.CategoryID inner join ShiftmentAssigen tt on tt.ID=t.Type and t.Flag=1 where t.Quantity>0";

        DataTable dt = DataManager.ExecuteQuery(connectionString, SelectQuery, "T_ItemDetails");
        return dt;
    }

    public static DataTable GetShowItemsSalesStock(string Parameter,string User)
    {
        string connectionString = DataManager.OraConnString();
        string selectQuery = "";
        DataTable dt = null;
       
            selectQuery = @"SELECT t.ID AS[ItemsID],t3.Code AS[item_code],t3.Name AS[item_desc],t.Price AS[Price],t.Quantity AS[StockQty],t3.ClosingAmount,CASE WHEN tt.ShiftmentNO IS NULL THEN 'Local Purchase' Else tt.ShiftmentNO END AS ShiftmentNO ,t.[Flag] AS[Type] FROM ItemSalesStock t left join Item t3 on t3.ID=t.ItemsID left join Brand t1 on t3.Brand=t1.ID left join Category t2 on t2.ID=t3.CategoryID left join ShiftmentAssigen tt on tt.ID=t.[Type]  where t.Quantity>0" + Parameter;
            dt = DataManager.ExecuteQuery(connectionString, selectQuery, "Item");
        
        return dt;
    }

    public static DataTable GetShowItemsManilaStockDetaile(string Parameter)
    {
        string connectionString = DataManager.OraConnString();
        //string Parameter = "";
        //if (SR != "") { Parameter = " AND UPPER(t3.Code+' - '+t3.Name+' - '+Isnull(t1.BrandName,'')+' - '+Isnull(t2.Name,'')) LIKE '" + SR + "' "; }
        string selectQuery = @"select  t1.ID,t1.[Code]+' - '+t1.[Name] AS Items,tt.ShiftmentNO,t2.Name AS Catagory,t3.Name AS SubCat,t1.[UnitPrice],ISNULL(t4.Quantity,0) AS [ClosingStock],tt.ShiftmentNO from ItemSalesStock t4 INNER JOIN  [Item] t1 on t1.ID=t4.ItemsID left join Category t2 on t2.ID=t1.CategoryID left join SubCategory t3 on t3.ID=t1.SubCategoryID inner join ShiftmentAssigen tt on tt.ID=t4.[Type] AND t4.Flag=1    WHERE t4.Quantity>0 " + Parameter;
        DataTable dt = DataManager.ExecuteQuery(connectionString, selectQuery, "Item");
        return dt;
    }

    public DataTable getItemsColor(string ItemsID)
    {
        string connectionString = DataManager.OraConnString();
        string selectQuery = @"SELECT '1' AS CHK,[ColorID] AS[ID],t2.ColorName AS[ColorName]
         FROM [ItemColor] t1 inner join ColorInfo t2 on t2.ID=t1.[ColorID] where t1.[DeleteBy] IS NULL AND t1.[ItemID]='" +
                             ItemsID + "' ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, selectQuery, "Item");
        return dt;
    }

    public DataTable getItemsSize(string ItemsID)
    {
        string connectionString = DataManager.OraConnString();
        string selectQuery = @"SELECT '1' AS CHK,SizeID AS[ID],t2.SizeName AS SizeName
      FROM dbo.ItemSize t1 inner join SizeInfo t2 on t2.ID=t1.SizeID where t1.[DeleteBy] IS NULL AND t1.[ItemID]='" +
                             ItemsID + "' ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, selectQuery, "Item");
        return dt;
    }

    public int getShowItemsHistoryDetails(string ItemID,string Flag,string ItemsType)
    {
        //string connectionString = DataManager.OraConnString();
        string Parameter = "";
        if (!string.IsNullOrEmpty(ItemsType))
        {
            Parameter = " AND t1.ItemsType='" + ItemsType + "' ";
        }
        SqlConnection connection=new SqlConnection(DataManager.OraConnString());
        connection.Open();
        string selectQuery =
            @"select t1.ID from Item t1 left join Brand t4 on t1.Brand=t4.ID where t1.ID='" + ItemID + "' AND t1.DeleteBy IS NULL " + Parameter;
        SqlCommand command = new SqlCommand(selectQuery, connection);
        int ID = Convert.ToInt32(command.ExecuteScalar());
        connection.Close();
        return ID;
    }

    public DataTable getShowItemsHistoryDetailsSearch(string ItemID,string ItemSearch, string p_2, string ItemsType)
    {
        string Parameter = "";
        if (string.IsNullOrEmpty(ItemsType) && !string.IsNullOrEmpty(ItemID))
        {
            Parameter = " and t1.ID='" + ItemID + "' ";
        }
        else if (ItemsType=="4" && !string.IsNullOrEmpty(ItemID))
        {
            Parameter = " and t1.ID='" + ItemID + "' ";
        }
        else if (ItemsType == "4" && string.IsNullOrEmpty(ItemID))
        {
            Parameter = " ";
        }
        else if (!string.IsNullOrEmpty(ItemsType) && !string.IsNullOrEmpty(ItemID))
        {
            Parameter = " and t1.ID='"+ItemID+"' AND t1.ItemsType='" + ItemsType + "' ";
        }
        else if (!string.IsNullOrEmpty(ItemsType) && string.IsNullOrEmpty(ItemID))
        {
            Parameter = "  AND t1.ItemsType='" + ItemsType + "' ";
        }
        else if (ItemsType!="")
        {
            Parameter = " and Upper (Convert(nvarchar,t1.ID)+'-'+t1.[Code]+'-'+t1.[Name]+'-'+t1.StyleNo+'-'+Convert(nvarchar,t1.UnitPrice)) like Upper('%" + ItemSearch + "%') ";
        }

        string connectionString = DataManager.OraConnString();
        string selectQuery = @"SELECT t1.ID
      ,t1.[Code]
      ,t1.[Name],t1.StyleNo  
      ,t4.BrandName as [Brand Name]        
      ,t2.Name as [Catagory]
      ,t3.Name as[Sub Catagory]
      ,t1.UnitPrice as[Unit Price]  
      ,t1.ClosingStock as [Closing Stock]        
  FROM [Item] t1
  left join Category t2 on t2.ID=t1.CategoryID left join SubCategory t3 on t3.ID=t1.SubCategoryID
  left join Brand t4 on t4.ID=t1.Brand where t1.DeleteBy IS NULL  " + Parameter + "  order by t1.ID DESC";
        DataTable dt = DataManager.ExecuteQuery(connectionString, selectQuery, "Item");
        return dt;
    }

    public static DataTable GetBranchInfo()
    {
        

        string connectionString = DataManager.OraConnString();
      string query = "Select * from View_BranchInfo";
      DataTable dt = DataManager.ExecuteQuery(connectionString, query, "View_BranchInfo");
        return dt;
    }

    public static DataTable GetShowBranchWiseItemsDetaile(string Item, string Branch , string type)
    {
        string connectionString = DataManager.OraConnString();
        string parameter = "";
        if (!string.IsNullOrEmpty(Item) && !string.IsNullOrEmpty(Branch) && type!="0")
        {
            parameter = " where items='" + Item + "' and BranchID= '" + Branch + "'  and ItemsType='"+type+"'";
        }
        else
        {
            if (type != "0")
            {
                if (!string.IsNullOrEmpty(Item) && !string.IsNullOrEmpty(Branch))
                {
                    parameter = " where items='" + Item + "' and BranchID= '" + Branch + "'  and ItemsType='" + type + "'";
                }

                else if (string.IsNullOrEmpty(Item) && !string.IsNullOrEmpty(Branch))
                {
                    parameter = " where  BranchID= '" + Branch + "'  and ItemsType='" + type + "'";
                }

                else if (!string.IsNullOrEmpty(Item) && string.IsNullOrEmpty(Branch))
                {
                    parameter = " where items='" + Item + "'  and ItemsType='" + type + "'";
                }
                else
                {
                    parameter = " where  ItemsType='" + type + "'";
                }
            }

            else
            {
                
                if (!string.IsNullOrEmpty(Item) && !string.IsNullOrEmpty(Branch))
                {
                    parameter = " where items='" + Item + "' and BranchID= '" + Branch + "'";
                }

                else if (string.IsNullOrEmpty(Item) && !string.IsNullOrEmpty(Branch))
                {
                    parameter = " where  BranchID= '" + Branch + "'";
                }

                else if (!string.IsNullOrEmpty(Item) && string.IsNullOrEmpty(Branch))
                {
                    parameter = " where items='" + Item + "' ";
                }
                else
                {
                    parameter = "";
                }
            }
            
        }
        string selectQuery = @"SELECT  * from View_ItemStockInfoBranchWise "+ parameter+" " ;
        DataTable dt = DataManager.ExecuteQuery(connectionString, selectQuery, "View_ItemStockInfoBranchWise");
            return dt;
        
    }
}