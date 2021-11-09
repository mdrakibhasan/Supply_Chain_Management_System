using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using autouniv;

/// <summary>
/// Summary description for ConsumptionManager
/// </summary>
public class ConsumptionManager
{
	public ConsumptionManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public static DataTable GetItemInformation(string Item)
    {
      
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = @"SELECT t1.[ID],t1.[Code] AS [item_code],t1.ItemsType,t1.[Name] ,t1.Brand,t1.[UOMID] AS UmoID,t1.[UnitPrice],t1.[Currency],t2.Name AS[UMO] ,t3.BrandName FROM [Item] t1 left join UOM t2 on t2.ID=t1.UOMID left join Brand t3 on t3.ID=t1.Brand  where  upper ( t1.Name + CASE WHEN (t1.StyleNo IS NULL OR
                      t1.StyleNo = '') THEN '' ELSE ' - ' + t1.StyleNo END + '-' + CONVERT(nvarchar, t1.UnitPrice) + CASE WHEN t3.BrandName IS NULL THEN '' ELSE ' - ' + t3.BrandName END ) = upper('" + Item + "') and  t1.[Active]=1";

        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Supplier");
        return dt;

    }

    public static void SaveConsumptionInfo(ConsumptionInfo aConsumptionInfo, DataTable dt)
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

            command.CommandText = @"INSERT INTO .[ConsumptionMst]
           ([FGItemID]           
           ,[TotalCost]
           ,[Status]
           ,[DeclareDate]
           ,[AddBy]
           ,[AddDate])
     VALUES ('" + aConsumptionInfo.FGItemIdMst + "','" + aConsumptionInfo.TotalCost + "','" + aConsumptionInfo.StatusMst + "',Convert(date,'" + aConsumptionInfo.DeclareDate + "',103),'" + aConsumptionInfo.LogineBy + "','" + Globals._localTime.ToString() + "') ";
            command.ExecuteNonQuery();

                command.CommandText = @"select top(1) ID from ConsumptionMst order by ID desc";
            int MstID=Convert.ToInt32(command.ExecuteScalar());
            
                //command.ExecuteNonQuery();

                foreach (DataRow drsh in dt.Rows)
                {
                    if (drsh["dtlID"].ToString() == "0")
                    {

                        command.CommandText = @"INSERT INTO [ConsumptionDtl]
           ([RMItemID]
           ,[MstID]
           ,[ItemQuantity]
           ,[AddBy]
           ,[AddDate],UniteTypeId,UnitePrice,Total)
     VALUES ('" + drsh["ItemID"].ToString() + "','" + MstID + "','" + drsh["Quantity"].ToString() + "','" + aConsumptionInfo.LogineBy + "','" + Globals._localTime.ToString() + "','" + drsh["UniteTypeId"].ToString() + "','" + drsh["UnitePrice"].ToString() + "','" + Convert.ToDouble(drsh["Quantity"].ToString()) * Convert.ToDouble(drsh["UnitePrice"].ToString()) + "')";
                        command.ExecuteNonQuery();

                    }
                }
            
            
            transection.Commit();
            connection.Close();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public static void UpdateConsumptionInfo(ConsumptionInfo aConsumptionInfo, DataTable dt, string MstID)
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

            command.CommandText = @"UPDATE [ConsumptionMst] Set [FGItemID]= '" + aConsumptionInfo.FGItemIdMst + "',[TotalCost]=Convert(Decimal,'" + aConsumptionInfo.TotalCost + "'),[Status]='" + aConsumptionInfo.StatusMst + "',[DeclareDate]=Convert(date,'" + aConsumptionInfo.DeclareDate + "',103),[UpdateBy]='" + aConsumptionInfo.LogineBy + "',[UpdateDate]='" + Globals._localTime.ToString() + "' Where ID='" + MstID + "'";
            command.ExecuteNonQuery();

            command.CommandText = @"UPDATE [ConsumptionDtl] SET [DeleteBy]='" + aConsumptionInfo.LogineBy + "',[DeleteDate]='" + Globals._localTime.ToString() + "' where [MstID]='" + MstID + "'";
            
                command.ExecuteNonQuery();

                foreach (DataRow drsh in dt.Rows)
                {
                    if (drsh["dtlID"].ToString() == "0")
                    {

                        command.CommandText = @"INSERT INTO [ConsumptionDtl]
           ([RMItemID]
           ,[MstID]
           ,[ItemQuantity]
           ,[AddBy]
           ,[AddDate],UniteTypeId,UnitePrice,Total)
     VALUES ('" + drsh["ItemID"].ToString() + "','" + MstID + "','" + drsh["Quantity"].ToString() + "','" + aConsumptionInfo.LogineBy + "','" + Globals._localTime.ToString() + "','" + drsh["UniteTypeId"].ToString() + "','" + drsh["UnitePrice"].ToString() + "','" + Convert.ToDouble(drsh["Quantity"].ToString()) * Convert.ToDouble(drsh["UnitePrice"].ToString()) + "')";
                        command.ExecuteNonQuery();

                    }

                    else
                    {
                        command.CommandText = @"UPDATE [ConsumptionDtl] SET [DeleteBy] = null ,[DeleteDate] = null where [MstID]='" + MstID + "' and ID='" + drsh["dtlID"].ToString() + "'";

                        command.ExecuteNonQuery();
                    }
                }
            
            
            transection.Commit();
            connection.Close();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public static DataTable GetConsumptionMst(string MstID)
    {
        string per = "";
        if(!string.IsNullOrEmpty(MstID))
        {
            per = " and t1.ID='"+MstID+"'";
        }
        String connectionString = DataManager.OraConnString();
        string Query = @"select t1.ID ,t2.Name as ItemNamae,t1.TotalCost,t1.[Status],case when t1.Status=1 then 'Active' when t1.[Status]=0 then 'InActive' else null End as StatusName,convert(nvarchar,t1.DeclareDate,103)as DeclareDate,t1.FGItemID from ConsumptionMst t1 inner join Item t2 on t2.ID=t1.FGItemID where t1.DeleteBy is null  " + per + " order by t1.ID desc";
        DataTable dt = DataManager.ExecuteQuery(connectionString, Query, "ConsumptionMst");
        return dt;
    }

    public static DataTable GetConsumptiondtlInfoDetails(string MstID)
    {
        String connectionString = DataManager.OraConnString();
        string Query = @"select t1.ID as dtlID,t1.ItemQuantity as Quantity,t1.UniteTypeId,t1.Total ,t1.UnitePrice as UnitePrice,t1.RMItemID as ItemID,t2.Name as ItemName from ConsumptionDtl  t1 
inner join Item t2 on t2.ID=t1.RMItemID
inner join ConsumptionMst t3 on t3.ID=t1.MstID where t1.MstID='" + MstID + "' and t1.DeleteBy is null order by t1.ID desc ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, Query, "ConsumptionMst");
        return dt;
    }

    public static DataTable CheckActiveItem(string ItemID,string Status)
    {
        String connectionString = DataManager.OraConnString();
        string Query = @"select t1.ID,t1.FGItemID ,t1.Status from ConsumptionMst t1 where t1.DeleteBy is null and t1.FGItemID='" + ItemID + "' and t1.Status='" + Status + "'";
        DataTable dt = DataManager.ExecuteQuery(connectionString, Query, "ConsumptionMst");
        return dt;
    }

    public static string GetConsumptionNo()
    {
        String connectionString = DataManager.OraConnString();
        string Query = @"select top(1) ID from ConsumptionMst order by ID desc";
        object MstID = DataManager.ExecuteScalar(connectionString, Query);

        string ConsumptionNoAuto =Convert.ToString(Convert.ToInt32(MstID) + 1).ToString();
        return ConsumptionNoAuto;
        
    }

    public static DataTable GetMeasure()
    {
        String connectionString = DataManager.OraConnString();
        string query = "select ID,Name from UOM";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "UOM");
        return dt;
       
    }

    public static DataTable GetConsumptionItemCountMst()
    {
        throw new NotImplementedException();
    }

    public static double GetClosingStockInfo( string ItemID)
    {
        String connectionString = DataManager.OraConnString();
        string query = "Select ClosingStock from Item where ID='"+ItemID+"'";
        object ClosingStock = DataManager.ExecuteScalar(connectionString, query);

        double ItemClosingStock = Convert.ToDouble(ClosingStock);
        return ItemClosingStock;
    }

    public static void DeleteConsumptionInfo(string MstID, ConsumptionInfo aConsumptionInfo)
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

            command.CommandText = @"UPDATE [ConsumptionMst] SET [DeleteBy]='" + aConsumptionInfo.LogineBy + "',[DeleteDate]='" + Globals._localTime.ToString() + "' where [ID]='" + MstID + "'";

            command.ExecuteNonQuery();

            command.CommandText = @"UPDATE [ConsumptionDtl] SET [DeleteBy]='" + aConsumptionInfo.LogineBy + "',[DeleteDate]='" + Globals._localTime.ToString() + "' where [MstID]='" + MstID + "'";

            command.ExecuteNonQuery();        
                

            transection.Commit();
            connection.Close();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    public static DataTable GetStatusBYItemStyleNo(string StyleNo, string FromDate, string ToDate)
    {
        String connectionString = DataManager.OraConnString();
        string query = @"Select a.StyleNo,a.Description,a.purqty,a.DmQty,a.CnQty,a.PRQty,a.Date,SUM((isnull(b.purqty,0)-isnull(b.DmQty,0)-isnull(b.CnQty,0)-isnull(b.PRQty,0))) as Stock from 
dbo.View_ItemStatuss a cross join 
dbo.View_ItemStatuss b where b.StyleNo=a.styleNo 
 and Convert(date,a.Date,103)>=Convert(date,b.Date,103) and a.StyleNo='" + StyleNo + "' " +
" group by a.StyleNo,a.Description,a.purqty,a.DmQty,a.CnQty,a.PRQty,a.Date order BY a.StyleNo asc,Convert(date,a.Date,103) asc  ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "UOM");
        return dt;
    }

    public static DataTable GetItemInfobyStyleNo(string StyleNo)
    {
        String connectionString = DataManager.OraConnString();
        string query = @" SELECT top(1) a1.[Code],a1.[Name] ,a1.StyleNo 
      ,isnull(t2.Name,'N/A') AS Category,tt.ContactName
      ,isnull(t3.Name,'N/A') AS SubCategory,a1.UnitPrice
      ,isnull(t5.Name,'N/A') AS UOM,'N/A' DesighNo
	  ,('N/A') as ColorName,('N/A') as SizeName,t4.BrandName      
  FROM   [Item] a1 
    left join Category t2 on t2.ID=a1.CategoryID 
   left join SubCategory t3 on t3.ID=a1.SubCategoryID
	left join UOM t5 on t5.ID=a1.UOMID 
	
  left join Brand t4 on t4.ID=a1.Brand
  inner join
 ( SELECT ItemID,ColorID,SizeID,DesighNo,UnitPrice,b3.ContactName FROM [dbo].[ItemPurchaseDtl] b1 inner join  [dbo].[ItemPurchaseMst] b2 on b2.ID=b1.ItemPurchaseMstID inner join Supplier b3 on b3.ID=b2.SupplierID) tt on  tt.ItemID=a1.ID and tt.UnitPrice=a1.UnitPrice
Where  a1.StyleNo='" + StyleNo + "'";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "UOM");
        return dt;
    }
}