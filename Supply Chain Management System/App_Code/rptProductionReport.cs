using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using autouniv;
using System.Data.SqlClient;

/// <summary>
/// Summary description for rptProductionReport
/// </summary>
public class rptProductionReport
{
	public rptProductionReport()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public static DataTable GetProductionInformation(string BrandID, string ItemCode, string UnitPricefrom, string UnitPriceTo,  string BacthNo, string ItemName, string BrandName, string FromDate, string ToDate, string type)
    {

        {

            {
                using (SqlConnection conn = new SqlConnection(DataManager.OraConnString()))
                {
                    SqlCommand sqlComm = new SqlCommand("SP_ProductionInformation", conn);
                    //**** Brand ID  ********************
                    if (string.IsNullOrEmpty(BrandID))
                    {
                        sqlComm.Parameters.AddWithValue("@BrandID", null);
                    }
                    else
                    {
                        sqlComm.Parameters.AddWithValue("@BrandID", BrandID);
                    }
                    //**** Item ID  ******************** 
                    if (!string.IsNullOrEmpty(ItemCode))
                    {
                        sqlComm.Parameters.AddWithValue("@ItemCode", ItemCode);
                    }
                    else
                    {
                        sqlComm.Parameters.AddWithValue("@ItemCode", null);
                    }
                    //**** UnitePrice from  ********************
                    if (!string.IsNullOrEmpty(UnitPricefrom))
                    {
                        sqlComm.Parameters.AddWithValue("@UnitPricefrom", UnitPricefrom);
                    }
                    else
                    {
                        sqlComm.Parameters.AddWithValue("@UnitPricefrom", null);
                    }

                    //**** Unit Price To ID  ********************
                    if (!string.IsNullOrEmpty(UnitPriceTo))
                    {
                        sqlComm.Parameters.AddWithValue("@UnitPriceTo", UnitPriceTo);
                    }
                    else
                    {
                        sqlComm.Parameters.AddWithValue("@UnitPriceTo", null);
                    }





                    //**** Batch No  ********************
                    if (!string.IsNullOrEmpty(BacthNo))
                    {
                        sqlComm.Parameters.AddWithValue("@BacthNo", BacthNo);
                    }
                    else
                    {
                        sqlComm.Parameters.AddWithValue("@BacthNo", null);
                    }

                    //**** Item Name ID  ********************
                    if (!string.IsNullOrEmpty(ItemName))
                    {
                        sqlComm.Parameters.AddWithValue("@ItemName", ItemName);
                    }
                    else
                    {
                        sqlComm.Parameters.AddWithValue("@ItemName", null);
                    }

                    //**** From Date  ********************
                    if (!string.IsNullOrEmpty(FromDate))
                    {
                        sqlComm.Parameters.AddWithValue("@FromDate", DateTime.ParseExact(FromDate, "dd/MM/yyyy",null));
                    }
                    else
                    {
                        sqlComm.Parameters.AddWithValue("@FromDate", null);
                    }

                    //**** To Date  ********************
                    if (!string.IsNullOrEmpty(ToDate))
                    {
                        sqlComm.Parameters.AddWithValue("@ToDate", DateTime.ParseExact(ToDate, "dd/MM/yyyy",null));
                    }
                    else
                    {
                        sqlComm.Parameters.AddWithValue("@ToDate", null);
                    }

                    //**** Type  ********************
                    if (!string.IsNullOrEmpty(type))
                    {
                        sqlComm.Parameters.AddWithValue("@type", type);
                    }
                    else
                    {
                        sqlComm.Parameters.AddWithValue("@type", null);
                    }


                    
                    sqlComm.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter da = new SqlDataAdapter();
                    DataSet ds = new DataSet();
                    da.SelectCommand = sqlComm;
                    da.Fill(ds, "SP_ProductionInformation");
                    DataTable dtStdDtl = ds.Tables["SP_ProductionInformation"];
                    return dtStdDtl;
                }

            }
        }
    }





    public static DataTable GetConsumptionInformation(string FromDate, string ToDate, string FinishGoodITem)
    {
         //where (T2.ItemID=@ItemId or @ItemId is null) and T1.ReceivedDate <= @Enddate
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string Paramater = "";
        if (!string.IsNullOrEmpty(FinishGoodITem))
        {
            Paramater = "And t4.ID='"+FinishGoodITem+"'";
        }
        string query = @"select T3.ItemID,t4.StyleNo,t4.Name as ItemName,'Consumption' as TransType,convert(nvarchar,T1.ReceivedDate,103) as TransDate ,0 StockIN,0 AmountIN,SUM(T3.Qnty) StockOut ,SUM( T3.TotalCost) AmountOut 
  
from dbo.ProductionMst T1
   inner join dbo.ProductionDtl t2 on T1.ID=T2.PNMstID
   INNER JOIN dbo.ProductionItemDtl T3 on T3.ProductionDtlID=T2.id inner join Item t4 on t4.ID=t3.ItemID  where Convert(date,T1.ReceivedDate,103) >=Convert(date,'" + FromDate + "',103) and Convert(date,T1.ReceivedDate,103) <=Convert(date,'" + ToDate + "',103)  " + Paramater + "  group by T3.ItemID,T1.ReceivedDate,t4.Name,t4.StyleNo  UNION ALL select T2.ItemID,t4.StyleNo,t4.Name as ItemName,'Consumption' as TransType,Convert(nvarchar,T1.ConsumptionDate,103) as ConsumptionDate,0,0,T2.Quantity, T2.TotalCost from dbo.ConsumptionManualyMst T1  INNER JOIN dbo.ConsumptionManualyDtl T2 on T1.ID=T2.MstID inner join Item t4 on t4.ID=t2.ItemID  where  t4.Active=1 " + Paramater + " ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ProductionDtl");
        return dt;
    }

    public static DataTable GetStockTransferDtl(string FromDate, string ToDate, string ItemID)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string Paramater = "";
        if (!string.IsNullOrEmpty(ItemID))
        {
            Paramater = " and t1.ID='"+ItemID+"' ";
        }
        string query = @" select t1.Name,t1.StyleNo,t2.BranchName as BranchName,convert(nvarchar,mst.TransferDate,103) as TransferDate,convert(nvarchar,mst.ReceivedDate,103) as ReceivedDate,isnull(dtl.TransferQuantity,0) as TransferQuantity,isnull(dtl.ReceivedQuantity,0) as ReceivedQuantity,isnull(dtl.BranchSalesPrice,0) as BranchSalesPrice from   dbo.ItemStockTransferDtl dtl inner join dbo.ItemStockTransferMst mst on dtl.MstID=mst.ID inner join Item t1 on dtl.ItemId=t1.ID inner join 
dbo.BranchInfo t2 on mst.BranchID=t2.ID where convert(date,TransferDate,103) between convert(date,'" + FromDate + "',103) and convert(date,'" + ToDate + "',103) " + Paramater + "  ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ProductionDtl");
        return dt;
    }
}