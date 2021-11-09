using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using autouniv;
using OldColor;

/// <summary>
/// Summary description for ProductionManager
/// </summary>
public class ProductionManager
{
	public ProductionManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public static void UpdateProduction(ProductionInfo aProductionInfo, DataTable dt, DataTable dtOldStk, VouchMst aVouchMst)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transaction;
        DataTable dtFixCode = VouchManager.GetAllFixGlCode("");
        try
        {
            connection.Open();
            transaction = connection.BeginTransaction();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transaction;

            command.CommandText = @"UPDATE [ProductionMst]
   SET [PN] ='" + aProductionInfo.GoodsReceiveNo + "'  ,[ReceivedDate] =convert(date,'" + aProductionInfo.ReceiveDate +
                                  "',103)  ,[Remarks] ='" + aProductionInfo.Remarks + "'  ,[ModifiedBy] = '" +
                                  aProductionInfo.LoginBy + "' ,[ModifiedDate] ='" + Globals._localTime.ToString() + "', BatchNo='" +
                                  aProductionInfo.BatchNo + "'  WHERE ID='" + aProductionInfo.ID + "' ";
            command.ExecuteNonQuery();

            command.CommandText =
                @"delete from [ProductionItemDtl] where ProductionDtlID in (select id from [ProductionDtl] where [PNMstID]='" +
                aProductionInfo.ID + "')";
            command.ExecuteNonQuery();

            command.CommandText = @"delete from [ProductionDtl] where [PNMstID]='" + aProductionInfo.ID + "'";
            command.ExecuteNonQuery();
            //          
            foreach (DataRow dr in dt.Rows)
            {


                if (dr["ItemID"].ToString() != "")
                {
                    command.CommandText = @"INSERT INTO [ProductionDtl]
           ([PNMstID]
           ,[ItemID]
           ,[BatchNo]           
           ,[UnitPrice]
           ,[Quantity]
           ,[Total]
           ,[CreatedBy]
           ,[CreatedDate],[ExpireDate],MsrUnitCode)
     VALUES
           ('" + aProductionInfo.ID + "','" + dr["ItemID"].ToString() + "','" + aProductionInfo.BatchNo + "','" +
                                          dr["item_rate"].ToString() + "','" + dr["qnty"].ToString() + "','" +
                                          Convert.ToDecimal(dr["item_rate"].ToString())*
                                          Convert.ToDecimal(dr["qnty"].ToString()) + "','" + aProductionInfo.LoginBy +
                                          "','" + Globals._localTime.ToString() + "',Convert(date,'" + dr["ExpireDate"] + "',103),'" + dr["msr_unit_code"].ToString() +
                                          "')";
                    command.ExecuteNonQuery();

//                   

                    //*********************************************************

                    command.CommandText = @"SELECT top(1) [ID]  FROM [ProductionDtl] order by ID desc";
                    string ProductionDtlID = command.ExecuteScalar().ToString();
                    //*************************************************************
                    //command.CommandText = @"Select t2.RMItemID,t2.ItemQuantity from ConsumptionMst t1 inner join ConsumptionDtl t2 on t1.ID=t2.MstID where t1.FGItemID='" + dr["ID"].ToString() + "'";

                    //string QntConsumptionItemMst = command.ExecuteScalar().ToString();
                    string QeryCountQnt =
                        @"Select t2.RMItemID,t2.UnitePrice,t2.ItemQuantity from ConsumptionMst t1 inner join ConsumptionDtl t2 on t1.ID=t2.MstID  where  t1.Status=1 and t1.FGItemID='" + dr["ItemID"].ToString() + "'";
                    DataTable dtQnt = DataManager.ExecuteQuery(connectionString, QeryCountQnt, "ConsumptionMst");
                    if (dtQnt.Rows.Count > 0)
                    {
                        foreach (DataRow drQnt in dtQnt.Rows)
                        {

                            command.CommandText = @"INSERT INTO [ProductionItemDtl]
           ([ProductionDtlID]
           ,[ItemID]
           ,[Qnty]
           ,[TotalCost])
     VALUES
           ('" + ProductionDtlID + "','" + drQnt["RMItemID"].ToString() + "','" +
                                                  Convert.ToDecimal(drQnt["ItemQuantity"].ToString()) *
                                                  Convert.ToDecimal(dr["qnty"].ToString()) + "','" +
                                                  Convert.ToDecimal(drQnt["UnitePrice"].ToString()) *
                                                  Convert.ToDecimal(Convert.ToDecimal(drQnt["ItemQuantity"].ToString()) *
                                                                   Convert.ToDecimal(dr["qnty"].ToString())) + "')";
                            command.ExecuteNonQuery();

                        }
                    }
                    
                }
            }
            //********************* Sales Total *********//

            command.CommandText = "SP_PV_UnitPrice_All";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@MstID", Convert.ToInt32(aProductionInfo.ID));
            decimal ProductionPrice = Convert.ToDecimal(command.ExecuteScalar());

            aVouchMst.ControlAmt = ProductionPrice.ToString().Replace("'", "");

            command.CommandText = VouchManager.SaveVoucherMst(aVouchMst, 2);
            command.ExecuteNonQuery();

            command.CommandText = @"delete from gl_trans_dtl where vch_sys_no=convert(numeric,'" + aVouchMst.VchSysNo + "')";
            command.ExecuteNonQuery();

            VouchDtl vdtl;
            vdtl = new VouchDtl();
            vdtl.VchSysNo = aVouchMst.VchSysNo;
            vdtl.ValueDate = aProductionInfo.ReceiveDate;
            vdtl.LineNo = "3";
            vdtl.GlCoaCode = dtFixCode.Rows[0]["ClosingStock"].ToString(); ;
            vdtl.Particulars = "Closing Stock";
            vdtl.AccType = VouchManager.getAccType(vdtl.GlCoaCode);
            vdtl.AmountDr = aVouchMst.ControlAmt.Replace(",", "");
            vdtl.AmountCr = "0";
            vdtl.AUTHO_USER = "CS";
            vdtl.Status = aVouchMst.Status;
            vdtl.BookName = aVouchMst.BookName;
            VouchManager.CreateVouchDtlForAutoVoucher(aVouchMst, vdtl, command);
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

    public static ProductionInfo GetProductMst(string ID)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = @"Select mst.ID,mst.PN,mst.BatchNo,mst.Remarks,CONVERT(nvarchar,mst.ReceivedDate,103) as ReceiveDate  from ProductionMst mst where mst.ID='" + ID + "'";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurchaseMst");
        sqlCon.Close();
        if (dt.Rows.Count == 0)
        {
            return null;
        }
        return new ProductionInfo(dt.Rows[0]);
    }

    public static void SaveProduct(ProductionInfo productmst, DataTable dt, string ORID, VouchMst aVouchMst)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transaction;
        DataTable dtFixCode = VouchManager.GetAllFixGlCode("");
        try
        {
            connection.Open();

            transaction = connection.BeginTransaction();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transaction;

            command.CommandText = @"INSERT INTO [ProductionMst]
           ([PN]
           ,[ReceivedDate]
           ,[Remarks]
           ,[CreatedBy]
           ,[CreatedDate],[BatchNo])
     VALUES
           ('" + productmst.GoodsReceiveNo + "',convert(date,'" + productmst.ReceiveDate + "',103),'" + productmst.Remarks + "','" + productmst.LoginBy + "','" + Globals._localTime.ToString() + "','" + productmst.BatchNo + "')";
            command.ExecuteNonQuery();

            command.CommandText = @"SELECT top(1) [ID]  FROM [ProductionMst] order by ID desc";
            string ProductionMstID = command.ExecuteScalar().ToString();

            //***************************  ********************************//           

            foreach (DataRow dr in dt.Rows)
            {
                if (dr["ItemID"].ToString() != "")
                {
                    command.CommandText = @"INSERT INTO [ProductionDtl]
           ([PNMstID]
           ,[ItemID]
           ,[BatchNo]           
           ,[UnitPrice]
           ,[Quantity]
           ,[Total]
           ,[CreatedBy]
           ,[CreatedDate],[ExpireDate],MsrUnitCode)
     VALUES
           ('" + ProductionMstID + "','" + dr["ItemID"].ToString() + "','" + productmst.BatchNo + "','" + dr["item_rate"].ToString() + "','" + dr["qnty"].ToString() + "','" + Convert.ToDecimal(dr["item_rate"].ToString()) * Convert.ToDecimal(dr["qnty"].ToString()) + "','" + productmst.LoginBy + "','" + Globals._localTime.ToString() + "',Convert(date,'" + dr["ExpireDate"] + "',103),'" + dr["msr_unit_code"].ToString() + "')";
                    command.ExecuteNonQuery();


                    //*********************************************************

                    command.CommandText = @"SELECT top(1) [ID]  FROM [ProductionDtl] order by ID desc";
                    string ProductionDtlID = command.ExecuteScalar().ToString();

                    string QeryCountQnt = @"Select t2.RMItemID,t2.UnitePrice,t2.ItemQuantity from ConsumptionMst t1 inner join ConsumptionDtl t2 on t1.ID=t2.MstID  where  t1.Status=1 and t1.FGItemID='" + dr["ItemID"].ToString() + "' and t1.DeleteBy is null and t2.DeleteBy is null";
                    DataTable dtQnt = DataManager.ExecuteQuery(connectionString, QeryCountQnt, "ConsumptionMst");
                    if (dtQnt.Rows.Count>0)
                    {
                        foreach (DataRow drQnt in dtQnt.Rows)
                        {
                           
                      command.CommandText = @"INSERT INTO [ProductionItemDtl]
           ([ProductionDtlID]
           ,[ItemID]
           ,[Qnty]
           ,[TotalCost])
     VALUES
           ('" + ProductionDtlID + "','" + drQnt["RMItemID"].ToString() + "','" + Convert.ToDecimal(drQnt["ItemQuantity"].ToString()) * Convert.ToDecimal(dr["qnty"].ToString()) + "','" + Convert.ToDecimal(drQnt["UnitePrice"].ToString()) * Convert.ToDecimal(Convert.ToDecimal(drQnt["ItemQuantity"].ToString()) * Convert.ToDecimal(dr["qnty"].ToString())) + "')";
                                command.ExecuteNonQuery();
                           
                        }
                    }
                    

                }
            }
            //********************* Sales Total *********//

            command.CommandText = "SP_PV_UnitPrice_All";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@MstID", Convert.ToInt32(ProductionMstID));
            command.Parameters.AddWithValue("@option", 1);
            decimal ProductionPrice = Convert.ToDecimal(command.ExecuteScalar());

            aVouchMst.ControlAmt = ProductionPrice.ToString().Replace("'", "");

            command.CommandText = VouchManager.SaveVoucherMst(aVouchMst, 1);
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();
            VouchDtl vdtl;
            vdtl = new VouchDtl();
            vdtl.VchSysNo = aVouchMst.VchSysNo;
            vdtl.ValueDate = productmst.ReceiveDate;
            vdtl.LineNo = "3";
            vdtl.GlCoaCode = dtFixCode.Rows[0]["ClosingStock"].ToString(); ;
            vdtl.Particulars = "Closing Stock";
            vdtl.AccType = VouchManager.getAccType(vdtl.GlCoaCode);
            vdtl.AmountDr = aVouchMst.ControlAmt.Replace(",", "");
            vdtl.AmountCr = "0";
            vdtl.AUTHO_USER = "CS";
            vdtl.Status = aVouchMst.Status;
            vdtl.BookName = aVouchMst.BookName;
            VouchManager.CreateVouchDtlForAutoVoucher(aVouchMst, vdtl, command);

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

    public static DataTable GetPurchaseItemsDetails(string ID)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = @"Select t1.ID,t1.ItemID,t4.ID as UMO,t4.Name,t3.BrandName,t1.MsrUnitCode as msr_unit_code,t2.Name as item_desc,t2.StyleNo as item_code,t1.UnitPrice as item_rate,t1.Quantity as qnty,convert(nvarchar,t1.[ExpireDate],103) as [ExpireDate] from ProductionDtl t1 inner join Item t2 on t2.ID=t1.ItemID  left join Brand t3 on t3.ID=t2.Brand inner join UOM t4 on t4.ID=t1.MsrUnitCode  where t1.PNMstID='" + ID + "' and t1.DeleteBy is null";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ProductionDtl");
        return dt;
    }

    public static DataTable GetShowProductionMst()
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = @"Select mst.ID,mst.PN,CONVERT(nvarchar,mst.ReceivedDate,103) as ReceiveDate,BatchNo,Remarks from ProductionMst mst where DeleteBy is null order by mst.ID desc";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ProductionMst");
        return dt;
    }

    public static DataTable GetShowPVMasterInfo(string PN)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = @"
Select ID,PN from ProductionMst where PN='"+PN+"' ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ProductionMst");
        return dt;
    }

    public static void DeleteProduction(ProductionInfo productmst, DataTable ItemDtl)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transaction;
        try
        {
            connection.Open();

            transaction = connection.BeginTransaction();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transaction;

            command.CommandText = @"UPDATE [ProductionMst]  SET [DeleteBy] = '" + productmst.LoginBy + "' ,[DeleteDate] ='" + Globals._localTime.ToString() + "'  WHERE   ID='" + productmst.ID + "'";
            command.ExecuteNonQuery();

            command.CommandText = @"UPDATE [ProductionItemDtl] SET [DeleteBy] ='" + productmst.LoginBy + "', [DeleteDate] = '" + Globals._localTime.ToString() + "' where ProductionDtlID in (select id from [ProductionDtl] where [PNMstID]='" + productmst.ID + "')";
            command.ExecuteNonQuery();

            command.CommandText = @"UPDATE [ProductionDtl]   SET [DeleteBy] ='" + productmst.LoginBy + "',[DeleteDate] ='" + Globals._localTime.ToString() + "' WHERE  PNMstID='" + productmst.ID + "'";
            command.ExecuteNonQuery();
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



    public static DataTable GetConsumtionItemStock(string FGItemId)
    {
    
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = @"select * from View_ConsumptionItem where FGItemID='" + FGItemId + "'";

        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "View_ConsumptionItem");
        return dt;
    
    }
}