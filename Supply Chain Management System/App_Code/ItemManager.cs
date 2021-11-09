using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;
using System.Data.SqlClient;
using OldColor;
using autouniv;


/// <summary>
/// Summary description for ItemManager
/// </summary>
/// 
namespace OldColor
{
    public class ItemManager
    {
        public static void DeleteItems(string item)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "delete from item_mst where item_code='" + item + "'";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void CreateItems(Items item)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);

            string query = " insert into item_mst(item_code,item_desc,msr_unit_code,item_rate,ITEM_DESC_Bang) values ('" + item.ItemCode + "', " +
            " '" + item.ItemDesc + "','" + item.MsrCode + "', convert(decimal(13,2), nullif( '" + item.ItemRate + "' ,'')), " +
            " N'" + item.ItemDescbang + "' )";

            DataManager.ExecuteNonQuery(connectionString, query);
        }

        public static void UpdateItems(Items item)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);

            string query = " update item_mst set item_code='" + item.ItemCode + "' , " +
            " item_desc= '" + item.ItemDesc + "',msr_unit_code='" + item.MsrCode + "', item_rate=convert(decimal(13,2), nullif( '" + item.ItemRate + "' ,'')), " +
            " ITEM_DESC_Bang =N'" + item.ItemDescbang + "' where item_code= '" + item.ItemCode + "'";

            DataManager.ExecuteNonQuery(connectionString, query);
        }

        public static Items GetItem(System.String item)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "select item_code,item_desc,msr_unit_code,convert(varchar,item_rate)item_rate,ITEM_DESC_Bang from item_mst where item_code='" + item + "' ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemMst");
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new Items(dt.Rows[0]);
        }
        public static DataTable GetItems(string criteria)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            //string query = @" SELECT t1.[ID],t1.[Code] AS [item_code],t1.[Name] AS [item_desc],t1.[UOMID] AS [msr_unit_code],t1.[UnitPrice],t1.[Currency],t2.Name AS[UMO] ,t3.BrandName FROM [Item] t1 left join UOM t2 on t2.ID=t1.UOMID left join Brand t3 on t3.ID=t1.Brand  where  upper (t1.Code+ ' - '+t1.Name) = upper('" + criteria + "') and  t1.[Active]=1";
            string query = @"SELECT t1.[ID],t1.[Code],t1.StyleNo AS [item_code],t1.ItemsType,t1.[Name] AS [item_desc],t1.Brand,t1.[UOMID] AS [msr_unit_code],t1.[UnitPrice],t1.[Currency],t2.Name AS[UMO] ,t3.BrandName FROM [Item] t1 left join UOM t2 on t2.ID=t1.UOMID left join Brand t3 on t3.ID=t1.Brand  where  upper ( t1.Name + CASE WHEN (t1.StyleNo IS NULL OR
                      t1.StyleNo = '') THEN '' ELSE ' - ' + t1.StyleNo END + '-' + CONVERT(nvarchar, t1.UnitPrice) + CASE WHEN t3.BrandName IS NULL THEN '' ELSE ' - ' + t3.BrandName END ) = upper('" + criteria + "') and  t1.[Active]=1";

            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Item");
            return dt;
        }
        public static DataTable GetItemGrid()
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "select item_code,item_desc,(select msr_unit_desc from measure_unit where msr_unit_code=a.msr_unit_code)msr_unit_code,convert(varchar,item_rate)item_rate " +
            " from item_mst a order by item_code";            
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemMst");
            return dt;
        }
        public static DataTable GetAllItems()
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "select item_code,item_desc,msr_unit_code,(select msr_unit_desc from measure_unit where msr_unit_code=a.msr_unit_code)msr_unit_desc,convert(varchar,item_rate)item_rate from item_mst a order by item_code";

            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemMst");
            return dt;
        }
        public static DataTable GetMeasure()
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "select ID,Name from UOM";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Measure");
            return dt;
        }

        public static DataTable getItemBalance()
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "select a.item_code,c.item_desc,d.msr_unit_desc,a.qnty-coalesce(b.qnty,0) qnty  from "+
            " (select item_code,msr_unit_code,sum(qnty) qnty from DeedDtl group by item_code,msr_unit_code) a inner join "+
            " (select item_code,msr_unit_code,sum(qnty) qnty from IssueDtl group by item_code,msr_unit_code) b on (a.item_code=b.item_code)"+
            " inner join item_mst c on (a.item_code=c.item_code) inner join measure_unit d on (a.msr_unit_code=d.msr_unit_code)"+
            " where a.qnty-coalesce(b.qnty,0)>0";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Measure");
            return dt;
        }

        public static string GetItemDesc(string itm)
        {
            String ConnectionString = DataManager.OraConnString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            string Query = "select item_desc from item_mst where  item_code='" + itm + "'";
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

        public static DataTable GetItemsBercode(string criteria,string User)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);

            string query = @"SELECT t1.[ID],t1.[Code],t1.[Code]  AS [item_code],t1.SecurityCode,t1.ClosingStock as Quantity,t1.StyleNo ,t1.BranchSalesPrice,t1.ItemsType,t1.[Name] AS [item_desc],t1.Brand,t1.[UOMID] AS [msr_unit_code],t1.[UnitPrice],t1.BranchSalesPrice ,t1.[Currency],t2.Name AS[UMO] ,t3.BrandName FROM [Item] t1 left join UOM t2 on t2.ID=t1.UOMID left join Brand t3 on t3.ID=t1.Brand  where  upper (  t1.Name + CASE WHEN (t1.StyleNo IS NULL OR
                      t1.StyleNo = '') THEN '' ELSE ' - ' + t1.StyleNo END + '-' + CONVERT(nvarchar, t1.UnitPrice) + CASE WHEN t3.BrandName IS NULL THEN '' ELSE ' - ' + t3.BrandName END ) = upper('" + criteria + "') and  t1.[Active]=1";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Item");
            return dt;
        }

    public static DataTable GetItemsBercodeNew(string criteria, string User)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);

            string query = @"SELECT t1.[ID],t1.[Code],t1.[Code]  AS [item_code],t1.SecurityCode,t1.ClosingStock as Quantity,t1.StyleNo ,t1.BranchSalesPrice,t1.ItemsType,case when (t1.ShortName is null or t1.ShortName='') then left(t1.[Name],50) else t1.ShortName end  AS [item_desc],t1.Brand,t1.[UOMID] AS [msr_unit_code],t1.[UnitPrice],t1.BranchSalesPrice ,t1.[Currency],t2.Name AS[UMO] ,t3.BrandName FROM [Item] t1 left join UOM t2 on t2.ID=t1.UOMID left join Brand t3 on t3.ID=t1.Brand  where  upper (  t1.Name + CASE WHEN (t1.StyleNo IS NULL OR
                      t1.StyleNo = '') THEN '' ELSE ' - ' + t1.StyleNo END + '-' + CONVERT(nvarchar, t1.UnitPrice) + CASE WHEN t3.BrandName IS NULL THEN '' ELSE ' - ' + t3.BrandName END ) = upper('" + criteria + "') and  t1.[Active]=1";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Item");
            return dt;
        }

        public DataTable getAllItems(int ckeckAllItems)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query =
                @"SELECT t1.[ID] AS[ItemsID],t1.[Code],t1.[Code]  AS [item_code] ,t1.[StyleNo],case when t1.ShortName is null then t1.[Name] when t1.ShortName='' then t1.[Name] else t1.ShortName end AS [item_desc],t1.[UOMID] AS [msr_unit_code],t1.[UnitPrice] AS Price,t1.[Currency],t2.Name AS[UMO] ,t3.BrandName,t5.Rate,convert(decimal(18,0),t1.ClosingStock) AS StockQty,t1.[description],t1.[StyleNo],t1.[SecurityCode],t6.Name AS Catagory,t7.Name AS SubCatagory,case when '" +
                ckeckAllItems +
                "'='1' then convert(decimal(18,0),t1.ClosingStock) else 0 end AS[TransferQty],'0' AS[ReceivedQuantity],ISNULL(t1.BranchSalesPrice,0) AS BranchSalesPrice  FROM [Item] t1 left join UOM t2 on t2.ID=t1.UOMID left join Brand t3 on t3.ID=t1.Brand left join TaxCategory t5 on t5.ID=t1.TaxCategoryID left join Category t6 on t6.ID=t1.CategoryID left join SubCategory t7 on t7.ID=t1.SubCategoryID where t1.ClosingStock>0 and t1.Active='True' and ItemsType in (2,3) and t1.DeleteBy IS NULL ORDER BY  t1.[Name] ASC";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Item");
            return dt;
        }

        public static DataTable GetStockItems(string criteria)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            //string query = @" SELECT t1.[ID],t1.[Code] AS [item_code],t1.[Name] AS [item_desc],t1.[UOMID] AS [msr_unit_code],t1.[UnitPrice],t1.[Currency],t2.Name AS[UMO] ,t3.BrandName FROM [Item] t1 left join UOM t2 on t2.ID=t1.UOMID left join Brand t3 on t3.ID=t1.Brand  where  upper (t1.Code+ ' - '+t1.Name) = upper('" + criteria + "') and  t1.[Active]=1";
            string query = @"SELECT a1.[ID],a1.[Code],a1.StyleNo AS [item_code],t1.ItemsType,t1.[Name] AS [item_desc],t1.Brand,t1.[UOMID] AS [msr_unit_code],a1.[UnitPrice],t1.[Currency],t2.Name AS[UMO] ,t3.BrandName

 FROM ItemStock a1 inner join [Item] t1 on a1.ItemID=t1.ID  left join UOM t2 on t2.ID=t1.UOMID left join Brand t3 on t3.ID=t1.Brand  where  upper (t1.Name + '-' + a1.Code + '-' + CONVERT(Nvarchar, a1.UnitPrice) ) = upper('" + criteria + "') and  t1.[Active]=1";

            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Item");
            return dt;
        }

    }
}