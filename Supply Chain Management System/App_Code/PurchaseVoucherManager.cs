using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using autouniv;
using OldColor;

/// <summary>
/// Summary description for PurchaseVoucherManager
/// </summary>
public class PurchaseVoucherManager
{
    SqlTransaction transaction;
    SqlConnection Connection = new SqlConnection(DataManager.OraConnString());
	public PurchaseVoucherManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public static PurchaseVoucherInfo GetPurchaseMst(string ID)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = @"SELECT t1.[ID],t1.[GRN],CONVERT(NVARCHAR,t1.[ReceivedDate],103) as ReceivedDate,t1.[PO],CONVERT(NVARCHAR,t1.[PODate],103) as PODate,t1.[ChallanNo],CONVERT(NVARCHAR,t1.[ChallanDate],103) as ChallanDate,t1.[SupplierID],t1.[Remarks],ISNULL(t1.[Total],0)Total,t1.[CarriagePerson],t1.[LaburePerson],ISNULL(t1.[OtherCharge],0) as OtherCharge,ISNULL(t1.[CarriageCharge],0) as CarriageCharge,ISNULL(t1.[LabureCharge],0) as LabureCharge,ISNULL(t1.[TotalPayment],0) as TotalPayment,t1.ShiftmentID,'' as ShiftmentNO,t1.PartyID
,CASE WHEN t2.PayMethod='Q' and (t2.Chk_Status='P' OR t2.Chk_Status='B') THEN isnull(t1.[Total],0) else (isnull(t1.[Total],0)-isnull(t2.PayAmt,0)) end AS Due
,CASE WHEN t2.PayMethod IS NULL THEN 'C' ELSE t2.PayMethod END AS [PaymentMethod],t2.Bank_id AS[BankName],t2.[ChequeNo],CONVERT(NVARCHAR,t2.[ChequeDate],103) as ChequeDate ,t1.[ChequeAmount],t2.Chk_Status FROM [ItemPurchaseMst] t1 left join SupplierPayment t2 on t2.purchase_id=t1.ID and t2.Payment_Type='PV'  Where t1.ID='" + ID + "' ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurchaseMst");
        sqlCon.Close();
        if (dt.Rows.Count == 0)
        {
            return null;
        }
        return new PurchaseVoucherInfo(dt.Rows[0]);
    }

    public static int SavePurchaseVoucher(PurchaseVoucherInfo purmst, DataTable dt, string ORID,string PaymentCoa)
    {
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

            command.CommandText = @"INSERT INTO [ItemPurchaseMst]
           ([GRN],[ReceivedDate],[PO],[PODate],[ChallanNo],[ChallanDate],[SupplierID],[Remarks],[Total],[CreatedBy],[CreatedDate],[CarriagePerson],[LaburePerson],[OtherCharge],[CarriageCharge],[LabureCharge],[TotalPayment],PartyID,ShiftmentID)
     VALUES
          ('" + purmst.GoodsReceiveNo + "',convert(date,'" + purmst.GoodsReceiveDate + "',103),'" +
                                  purmst.PurchaseOrderNo + "',convert(date,'" + purmst.PurchaseOrderDate + "',103),'" +
                                  purmst.ChallanNo + "',convert(date,'" + purmst.ChallanDate + "',103),'" +
                                  purmst.Supplier + "','" + purmst.Remarks + "','" + purmst.TotalAmount + "','" +
                                  purmst.LoginBy + "','" + Globals._localTime.ToString() + "','" + purmst.CarriagePerson + "','" + purmst.LaburePerson +
                                  "','" + purmst.OtherCharge + "','" + purmst.CarriageCharge + "','" +
                                  purmst.LabureCharge + "','" + purmst.TotalPayment + "','" + purmst.PartyID + "','" +
                                  purmst.ShiftmentID + "')";
            command.ExecuteNonQuery();

            command.CommandText = @"SELECT top(1) [ID]  FROM [ItemPurchaseMst] order by ID desc";
            string PurchaseMstID = command.ExecuteScalar().ToString();

            foreach (DataRow dr in dt.Rows)
            {
                if (dr["ID"].ToString() != "" && dr["item_desc"].ToString() != "")
                {
                    command.CommandText = @"INSERT INTO [ItemPurchaseDtl]
           ([ItemPurchaseMstID],[ItemID],[UnitPrice],[Quantity],[Total],[CreatedBy],[CreatedDate],[MsrUnitCode],Additional)
     VALUES
           ('" + PurchaseMstID + "','" + dr["ID"].ToString() + "','" + dr["item_rate"].ToString() + "','" +
                                          dr["qnty"].ToString() + "','" +
                                          Convert.ToDouble(dr["item_rate"].ToString()) *
                                          Convert.ToDouble(dr["qnty"].ToString()) + "','" + purmst.LoginBy +
                                          "','" + Globals._localTime.ToString() + "','" + dr["msr_unit_code"].ToString() + "','" +
                                          dr["Additional"].ToString() + "')";
                    command.ExecuteNonQuery();
                }
            }

           
            

            if (!string.IsNullOrEmpty(ORID))
            {
                command.CommandText = @"UPDATE [ItemPurOrderMst]  SET [OrderStatus] ='C'  WHERE ID='" + ORID + "'";
                command.ExecuteNonQuery();
            }
            transaction.Commit();
            return Convert.ToInt32(PurchaseMstID);
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
    public static void UpdatePurchaseVoucher(PurchaseVoucherInfo purmst, DataTable dt, DataTable OldStk,  string PaymentCoa)
    {
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
            SqlCommand command1 = new SqlCommand();
            command1.Connection = connection;
            command1.Transaction = transaction;
            command.CommandText = @"UPDATE [ItemPurchaseMst]
   SET [ReceivedDate] =convert(date,'" + purmst.GoodsReceiveDate + "',103),[PO] ='" + purmst.PurchaseOrderNo + "',[PODate] =convert(date,'" + purmst.PurchaseOrderDate + "',103),[ChallanNo] ='" + purmst.ChallanNo + "',[ChallanDate] =convert(date,'" + purmst.ChallanDate + "',103),[SupplierID] ='" + purmst.Supplier + "',[Remarks] ='" + purmst.Remarks + "',[Total] ='" + purmst.TotalAmount + "',[ModifiedBy] ='" + purmst.LoginBy + "' ,[ModifiedDate] ='" + Globals._localTime.ToString() + "',[CarriagePerson] ='" + purmst.CarriagePerson + "',[LaburePerson] ='" + purmst.LaburePerson + "',[OtherCharge] ='" + purmst.OtherCharge + "',[CarriageCharge] ='" + purmst.CarriageCharge + "',[LabureCharge] ='" + purmst.LabureCharge + "',[TotalPayment] ='" + purmst.TotalPayment + "',ShiftmentID='" + purmst.ShiftmentID + "'  WHERE ID='" + purmst.ID + "' ";
            command.ExecuteNonQuery();

            command1.CommandText = @"delete from [ItemPurchaseDtl] where ItemPurchaseMstID='" + purmst.ID + "'";
            command1.ExecuteNonQuery();
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["ID"].ToString() != "")
                {
                    command.CommandText = @"INSERT INTO [ItemPurchaseDtl]
           ([ItemPurchaseMstID],[ItemID],[UnitPrice],[Quantity],[Total],[CreatedBy],[CreatedDate],[MsrUnitCode],Additional)
     VALUES
           ('" + purmst.ID + "','" + dr["ID"].ToString() + "','" + dr["item_rate"].ToString() + "','" +
                                          dr["qnty"].ToString() + "','" +
                                          Convert.ToDouble(dr["item_rate"].ToString()) *
                                          Convert.ToDouble(dr["qnty"].ToString()) + "','" + purmst.LoginBy +
                                          "','" + Globals._localTime.ToString() + "','" + dr["msr_unit_code"].ToString() + "','" +
                                          dr["Additional"].ToString() + "')";
                    command.ExecuteNonQuery();

                }
            }

           
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
    public static DataTable GetShowPurchaseMst()
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = @"SELECT t1.ID,t1.[GRN]
      ,CONVERT(NVARCHAR,t1.[ReceivedDate],103) AS ReceivedDate   
      ,t1.[ChallanNo]
      ,CONVERT(NVARCHAR,t1.[ChallanDate],103) AS ChallanDate
      ,t2.ContactName as Name
      ,t1.[Total]  
      ,t3.PartyName AS Party    
      ,convert(decimal(18,0),t4.PurchaseQty) AS PurchaseQty
  FROM [ItemPurchaseMst] t1 inner join Supplier t2 on t2.ID=t1.SupplierID left join PartyInfo t3 on t3.ID=t1.PartyID left join (select t.ItemPurchaseMstID,SUM(t.Quantity) AS[PurchaseQty] from ItemPurchaseDtl t group by t.ItemPurchaseMstID) t4 on t4.ItemPurchaseMstID=t1.ID order By t1.ID desc";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurchaseMst");
        return dt;
    }

    public static DataTable GetPurchaseItemsDetails(string ItemsID)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = @"SELECT t1.[ItemID] AS ID
      ,t2.Code,t2.[StyleNo] AS item_code
      ,t2.Name AS item_desc
      ,ISNULL(t1.[UnitPrice],0) AS item_rate
      ,ISNULL(t1.[Quantity],0) AS qnty
      ,ISNULL(t1.[Total],0) AS Total    
      ,t1.[MsrUnitCode] AS msr_unit_code
      ,t3.Name AS UMO
      ,ISNULL(Additional,0) AS [Additional]
      ,t4.BrandName
  FROM [ItemPurchaseDtl] t1 inner join Item t2 on t2.ID=t1.ItemID inner join UOM t3 on t3.ID=t1.MsrUnitCode left join Brand t4 on t4.ID=t2.Brand where t1.[ItemPurchaseMstID]='" + ItemsID + "'";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurchaseDtl");
        return dt;
    }

    public static void DeletePurchaseVoucher(PurchaseVoucherInfo purmst, DataTable OldStk)
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

            string Query = @"select t1.VCH_SYS_NO  from [GL_TRANS_MST] t1 where SERIAL_NO='" + purmst.GoodsReceiveNo + "'";
            DataTable dt = DataManager.ExecuteQuery(connectionString, Query, "ItemPurchaseMst");
            command.CommandText = @"DELETE FROM [GL_TRANS_MST]  WHERE SERIAL_NO='" + purmst.GoodsReceiveNo + "'";
            command.ExecuteNonQuery();
            foreach (DataRow dr in dt.Rows)
            {
                command.CommandText = @"DELETE FROM [GL_TRANS_DTL]  WHERE VCH_SYS_NO='" + dr["VCH_SYS_NO"].ToString() + "'";
                command.ExecuteNonQuery();
            }

            command.CommandText = @"DELETE FROM [ItemPurchaseMst] WHERE  ID='" + purmst.ID + "'";
            command.ExecuteNonQuery();

            //foreach (DataRow drold in dtOldVchDtl.Rows)
            //{
                command.CommandText = @"delete from [ItemPurchaseDtl] where ItemPurchaseMstID='" + purmst.ID + "'";
                command.ExecuteNonQuery();
            //}

            //command.CommandText = @"DELETE FROM [SupplierPayment] WHERE  purchase_id='" + purmst.ID + "' AND Payment_Type='PV' ";
            //command.ExecuteNonQuery();

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

  

    //*********************************** Report Query ********************//
    public static DataTable GetShowPurchaeReport(string CurTime, string StrDate, string EndDate, string SupploerID)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string Paramater = "";
        if (CurTime != "") { Paramater = "where CONVERT(NVARCHAR,[ReceivedDate],103)='" + CurTime + "'"; }
        if (StrDate != "" && EndDate != "" && SupploerID == "0") { Paramater = "where CONVERT(DATE,[ReceivedDate],103) between CONVERT(DATE,'" + StrDate + "',103) and CONVERT(DATE,'" + EndDate + "',103)"; }
        if (StrDate != "" && EndDate != "" && SupploerID != "0") { Paramater = "where [Sup_ID]='" + SupploerID + "' and CONVERT(DATE,[ReceivedDate],103) between CONVERT(DATE,'" + StrDate + "',103) and CONVERT(DATE,'" + EndDate + "',103)"; }
        if (SupploerID != "0" && StrDate == "" && EndDate == "") { Paramater = "where [Sup_ID]='" + SupploerID + "'"; }
        string query = @"SELECT * FROM [ItemPurchaseForReport]  "+Paramater;
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurchaseDtl");
        return dt;
    }

    //public static DataTable GetShowSalesReportReport(string CurTime, string StrDate, string EndDate, string CustomerID)
    //{
    //    string connectionString = DataManager.OraConnString();
    //    SqlConnection sqlCon = new SqlConnection(connectionString);
    //    string Paramater = "";
    //    if (CurTime != "") { Paramater = "where CONVERT(NVARCHAR,[OrderDate],103)='" + CurTime + "'"; }
    //    if (StrDate != "" && EndDate != "" && CustomerID == "0") { Paramater = "where CONVERT(DATE,[OrderDate],103) between CONVERT(DATE,'" + StrDate + "',103) and CONVERT(DATE,'" + EndDate + "',103)"; }
    //    if (StrDate != "" && EndDate != "" && CustomerID != "0") { Paramater = "where [CustomerID]='" + CustomerID + "' and CONVERT(DATE,[OrderDate],103) between CONVERT(DATE,'" + StrDate + "',103) and CONVERT(DATE,'" + EndDate + "',103)"; }        
    //    if (CustomerID != "0" && StrDate == "" && EndDate == "") { Paramater = "where [CustomerID]='" + CustomerID + "'"; }
    //    string query = @"SELECT * FROM [OrderInformationForReport]  " + Paramater;
    //    DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurchaseDtl");
    //    return dt;
    //}

//    public static DataTable GetShowStock(string Type, string Catagory, string SubCat)
//    {
//        string connectionString = DataManager.OraConnString();
//        string parameter = "";
//        if (Type == "all")
//        {
//            if (Catagory != "" && SubCat == "")
//            {
//                parameter = "where t1.CategoryID='" + Catagory + "'";
//            }
//            else if (Catagory != "" && SubCat != "")
//            {
//                parameter = "where t1.CategoryID='" + Catagory + "' and t1.SubCategoryID='" + SubCat + "'";
//            }
//            else
//            {
//                parameter = "order by t1.ID asc";
//            }
//        }
//        else if (Type == "Available")
//        {
//            if (Catagory != "" && SubCat == "")
//            {
//                parameter = "where t1.ClosingStock>0 and t1.CategoryID='" + Catagory + "'";
//            }
//            else if (Catagory != "" && SubCat != "")
//            {
//                parameter = "where t1.ClosingStock>0 and t1.CategoryID='" + Catagory + "' and t1.SubCategoryID='" + SubCat + "' ";
//            }
//            else
//            {
//                parameter = "where t1.ClosingStock>0 order by t1.ID asc";
//            }
//        }
//        else if (Type == "Unavailable")
//        {
//            if (Catagory != "" && SubCat == "")
//            {
//                parameter = "where t1.ClosingStock<=0 and t1.CategoryID='" + Catagory + "'";
//            }
//            else if (Catagory != "" && SubCat != "")
//            {
//                parameter = "where t1.ClosingStock<=0 and t1.CategoryID='" + Catagory + "' and t1.SubCategoryID='" + SubCat + "' ";
//            }
//            else
//            {
//                parameter = "where t1.ClosingStock<=0 order by t1.ID asc";
//            }
//        }
//        string selectQuery = @"SELECT t1.[Code]+' - '+t1.[Name] AS Items  
//	  ,t4.BrandName
//      ,t2.Name AS Catagory
//      ,t3.Name AS SubCat
//      ,t5.Name AS UMO
//      ,t1.[UnitPrice]     
//      ,t1.[OpeningStock]
//      ,t1.[OpeningAmount]
//      ,t1.[ClosingStock]
//      ,t1.[ClosingAmount]          
//  FROM [Item] t1 left join Category t2 on t2.ID=t1.CategoryID left join SubCategory t3 on t3.ID=t1.SubCategoryID left join Brand t4 on t4.ID=t1.Brand left join UOM t5 on t5.ID=t1.UOMID " + parameter;
//        DataTable dt = DataManager.ExecuteQuery(connectionString, selectQuery, "Item");
//        return dt;
//    }

    //public static DataTable getShowTotalItemsStockByDate(string StrDt, string EndDt,string Date)
    //{
    //    DataSet ds = new DataSet();
    //    string connectionString = DataManager.OraConnString();
    //    using (SqlConnection conn = new SqlConnection(DataManager.OraConnString()))
    //    {

    //        if (EndDt == "" && StrDt == "") 
    //        {
    //            StrDt = Globals._localTime.ToString("dd/MM/yyyy"); 
    //        }
    //        SqlCommand sqlComm = new SqlCommand("Total_Product_Details", conn);
    //        if (Date != "")
    //        {
    //            sqlComm.Parameters.AddWithValue("@StartDate", DataManager.DateEncode(Date));
    //            sqlComm.Parameters.AddWithValue("@EndDate", DataManager.DateEncode(Date));
    //        }
    //        else
    //        {
    //            sqlComm.Parameters.AddWithValue("@StartDate", DataManager.DateEncode(StrDt));
    //            sqlComm.Parameters.AddWithValue("@EndDate", DataManager.DateEncode(EndDt));
    //        }
    //        sqlComm.CommandType = CommandType.StoredProcedure;

    //        SqlDataAdapter da = new SqlDataAdapter();
    //        da.SelectCommand = sqlComm;
    //        da.Fill(ds, "Total_Product_Details");
    //        ds.Tables[0].TableName = "Total_Product_Details";
    //        return ds.Tables[0];
    //    }
       
    //}
    //*********************************** Report Query ********************//
    public static DataTable GetShowPurchaeReport(string CurTime, string StrDate, string EndDate, string SupploerID, string typeID)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string Paramater = "";
        string query = "";
       
           if (CurTime != "")
            {
                Paramater = "where CONVERT(NVARCHAR,[ReceivedDate],103)='" + CurTime + "'";
            }
            if (StrDate != "" && EndDate != "" && SupploerID == "0")
            {
                Paramater = "where CONVERT(DATE,[ReceivedDate],103) between CONVERT(DATE,'" + StrDate +
                            "',103) and CONVERT(DATE,'" + EndDate + "',103)";
            }
            if (StrDate != "" && EndDate != "" && SupploerID != "0")
            {
                Paramater = "where [Sup_ID]='" + SupploerID +
                            "' and CONVERT(DATE,[ReceivedDate],103) between CONVERT(DATE,'" + StrDate +
                            "',103) and CONVERT(DATE,'" + EndDate + "',103)";
            }
            if (SupploerID != "0" && StrDate == "" && EndDate == "")
            {
                Paramater = "where [Sup_ID]='" + SupploerID + "'";
            }

            query = @"SELECT * FROM [ItemPurchaseForReport]  " + Paramater;
        

        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurchaseDtl");
        return dt;
    }

    public static DataTable GetShowSalesReportReport(string CurTime, string StrDate, string EndDate, string CustomerID)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string Paramater = "";
        if (CurTime != "") { Paramater = "where CONVERT(NVARCHAR,[OrderDate],103)='" + CurTime + "'"; }
        if (StrDate != "" && EndDate != "" && CustomerID == "0") { Paramater = "where CONVERT(DATE,[OrderDate],103) between CONVERT(DATE,'" + StrDate + "',103) and CONVERT(DATE,'" + EndDate + "',103)"; }
        if (StrDate != "" && EndDate != "" && CustomerID != "0") { Paramater = "where [CustomerID]='" + CustomerID + "' and CONVERT(DATE,[OrderDate],103) between CONVERT(DATE,'" + StrDate + "',103) and CONVERT(DATE,'" + EndDate + "',103)"; }
        if (CustomerID != "0" && StrDate == "" && EndDate == "") { Paramater = "where [CustomerID]='" + CustomerID + "'"; }
        string query = @"SELECT * FROM [OrderInformationForReport]  " + Paramater;
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurchaseDtl");
        return dt;
    }

    public static DataTable GetShowStock(string Type, string Catagory, string SubCat)
    {
        string connectionString = DataManager.OraConnString();
        string parameter = "";
        if (Type == "all")
        {
            if (Catagory != "" && SubCat == "")
            {
                parameter = "where t1.CategoryID='" + Catagory + "'";
            }
            else if (Catagory != "" && SubCat != "")
            {
                parameter = "where t1.CategoryID='" + Catagory + "' and t1.SubCategoryID='" + SubCat + "'";
            }
            else
            {
                parameter = "order by t1.ID asc";
            }
        }
        else if (Type == "Available")
        {
            if (Catagory != "" && SubCat == "")
            {
                parameter = "where t1.ClosingStock>0 and t1.CategoryID='" + Catagory + "'";
            }
            else if (Catagory != "" && SubCat != "")
            {
                parameter = "where t1.ClosingStock>0 and t1.CategoryID='" + Catagory + "' and t1.SubCategoryID='" + SubCat + "' ";
            }
            else
            {
                parameter = "where t1.ClosingStock>0 order by t1.ID asc";
            }
        }
        else if (Type == "Unavailable")
        {
            if (Catagory != "" && SubCat == "")
            {
                parameter = "where t1.ClosingStock<=0 and t1.CategoryID='" + Catagory + "'";
            }
            else if (Catagory != "" && SubCat != "")
            {
                parameter = "where t1.ClosingStock<=0 and t1.CategoryID='" + Catagory + "' and t1.SubCategoryID='" + SubCat + "' ";
            }
            else
            {
                parameter = "where t1.ClosingStock<=0 order by t1.ID asc";
            }
        }
        string selectQuery = @"SELECT t1.[Code]+' - '+t1.[Name] AS Items  
	  ,t4.BrandName
      ,t2.Name AS Catagory
      ,t3.Name AS SubCat
      ,t5.Name AS UMO
      ,t1.[UnitPrice]     
      ,t1.[OpeningStock]
      ,t1.[OpeningAmount]
      ,t1.[ClosingStock]
      ,t1.[ClosingAmount]          
  FROM [Item] t1 left join Category t2 on t2.ID=t1.CategoryID left join SubCategory t3 on t3.ID=t1.SubCategoryID left join Brand t4 on t4.ID=t1.Brand left join UOM t5 on t5.ID=t1.UOMID " + parameter;
        DataTable dt = DataManager.ExecuteQuery(connectionString, selectQuery, "Item");
        return dt;
    }
    public static DataTable GetShowStockReport(string Type, string Catagory, string SubCat)
    {
        string connectionString = DataManager.OraConnString();
        string parameter = "";
        if (Type == "All")
        {
            if (Catagory != "" && SubCat == "")
            {
                parameter = " and t1.CategoryID='" + Catagory + "'";
            }
            else if (Catagory != "" && SubCat != "")
            {
                parameter = " and t1.CategoryID='" + Catagory + "' and t1.SubCategoryID='" + SubCat + "'";
            }
           
        }
        else if (Type == "RM")
        {
            if (Catagory != "" && SubCat == "")
            {
                parameter = " and t1.CategoryID='" + Catagory + "' and t1.ItemsType =1 ";
            }
            else if (Catagory != "" && SubCat != "")
            {
                parameter = " and  t1.CategoryID='" + Catagory + "' and t1.SubCategoryID='" + SubCat + "' and t1.ItemsType =1 ";
            }
            else
            {
                parameter = " and   t1.ItemsType =1  ";
            }
        }
        else if (Type == "FG")
        {
            if (Catagory != "" && SubCat == "")
            {
                parameter = " and  t1.CategoryID='" + Catagory + "' and t1.ItemsType =2 ";
            }
            else if (Catagory != "" && SubCat != "")
            {
                parameter = " and  t1.CategoryID='" + Catagory + "' and t1.SubCategoryID='" + SubCat + "' and t1.ItemsType =2 ";
            }
            else
            {
                parameter = " and   t1.ItemsType =2  ";
            }
        }

        else if (Type == "CM")
        {
            if (Catagory != "" && SubCat == "")
            {
                parameter = "and  t1.CategoryID='" + Catagory + "' and t1.ItemsType =3 ";
            }
            else if (Catagory != "" && SubCat != "")
            {
                parameter = "and  t1.CategoryID='" + Catagory + "' and t1.SubCategoryID='" + SubCat + "' and t1.ItemsType =3 ";
            }
            else
            {
                parameter = "And  t1.ItemsType =3  ";
            }
        }

        else if (Type == "DM")
        {
            if (Catagory != "" && SubCat == "")
            {
                parameter = "And  t1.CategoryID='" + Catagory + "' and t1.ItemsType =4 ";
            }
            else if (Catagory != "" && SubCat != "")
            {
                parameter = "And  t1.CategoryID='" + Catagory + "' and t1.SubCategoryID='" + SubCat + "' and t1.ItemsType =4 ";
            }
            else
            {
                parameter = " And t1.ItemsType =4  ";
            }
        }

        string selectQuery = @"SELECT t1.[Code],t1.[Name] ,t1.StyleNo	  
      ,t2.Name AS Catagory
      ,t3.Name AS SubCat
      ,t5.Name AS UMO
      ,t1.[UnitPrice] , t1.BranchSalesPrice 
      ,t1.[ClosingStock]
      ,t1.[ClosingAmount]   
      ,t6.[ClosingStock] as Branch1Stock
      ,t6.[ClosingAmount]as Branch1Amount 
      ,t7.[ClosingStock] as Branch2Stock
      ,t7.[ClosingAmount]as Branch2Amount        
  FROM [Item] t1 left join Category t2 on t2.ID=t1.CategoryID left join SubCategory t3 on t3.ID=t1.SubCategoryID left join Brand t4 on t4.ID=t1.Brand left join UOM t5 on t5.ID=t1.UOMID 
  left join BranchItemStock t6 on t1.ID=t6.ItemID and t6.BranchID=5 left join BranchItemStock t7 on t1.ID=t7.ItemID and t7.BranchID=6 where t1.DeleteBy is null " + parameter + " order by t1.ID asc ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, selectQuery, "Item");
        return dt;
    }
    public static DataTable getShowTotalItemsStockByDate(string StrDt, string EndDt, string Date,string ItemID)
    {
        DataSet ds = new DataSet();
        string connectionString = DataManager.OraConnString();
        using (SqlConnection conn = new SqlConnection(DataManager.OraConnString()))
        {

            if (EndDt == "" && StrDt == "")
            {
                StrDt = DateTime.Now.ToString("dd/MM/yyyy");
            }
            SqlCommand sqlComm = new SqlCommand("Total_Product_Details", conn);

            if (!string.IsNullOrEmpty(ItemID))
            {
                sqlComm.Parameters.AddWithValue("@ItemId", ItemID);
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@ItemId", null);
            }
            if (Date != "")
            {
                sqlComm.Parameters.AddWithValue("@StartDate", DataManager.DateEncode(Date));
                sqlComm.Parameters.AddWithValue("@EndDate", DataManager.DateEncode(Date));
            }
            else
            {

                sqlComm.Parameters.AddWithValue("@StartDate", DataManager.DateEncode(StrDt));
                sqlComm.Parameters.AddWithValue("@EndDate", DataManager.DateEncode(EndDt));
            }
            sqlComm.CommandType = CommandType.StoredProcedure;

            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = sqlComm;
            da.Fill(ds, "Total_Product_Details");
            ds.Tables[0].TableName = "Total_Product_Details";
            return ds.Tables[0];
        }

    }


    public static DataTable GetShowPurchaseMst(string GrNo, string SupplierCode, string ReceiveFromDate, string ReceiveToDate)
    {
        string per = "";

        if (!string.IsNullOrEmpty(GrNo))
        {
            per = "where  t1.GRN='" + GrNo + "' ";
        }

        if (!string.IsNullOrEmpty(SupplierCode) && string.IsNullOrEmpty(GrNo) && !string.IsNullOrEmpty(ReceiveFromDate) && !string.IsNullOrEmpty(ReceiveToDate))
        {

            per = "where t2.Code='" + SupplierCode + "' and  (CONVERT(date,t1.ReceivedDate,103) between CONVERT(date,'" + ReceiveFromDate + "',103) and CONVERT(date,'" + ReceiveToDate + "',103))";
        }
        //&& (string.IsNullOrEmpty(GrNo) | string.IsNullOrEmpty(ReceiveFromDate) | string.IsNullOrEmpty(ReceiveFromDate)))
        if (!string.IsNullOrEmpty(SupplierCode) && string.IsNullOrEmpty(GrNo) && (string.IsNullOrEmpty(ReceiveFromDate) | string.IsNullOrEmpty(ReceiveToDate)))
        {
            per = "where t2.Code='" + SupplierCode + "'";
        }

        if (string.IsNullOrEmpty(SupplierCode) && string.IsNullOrEmpty(GrNo) && !string.IsNullOrEmpty(ReceiveFromDate) && !string.IsNullOrEmpty(ReceiveToDate))
        {

            per = "where  (CONVERT(date,t1.ReceivedDate,103) between CONVERT(date,'" + ReceiveFromDate + "',103) and CONVERT(date,'" + ReceiveToDate + "',103))";
        }
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = @"SELECT top(50) t1.ID,t1.[GRN]
      ,CONVERT(NVARCHAR,t1.[ReceivedDate],103) AS ReceivedDate   
      ,t1.[ChallanNo]
      ,CONVERT(NVARCHAR,t1.[ChallanDate],103) AS ChallanDate
      ,t2.ContactName as Name
      ,t1.[Total]  
      ,t3.PartyName AS Party    
  FROM [ItemPurchaseMst] t1 inner join Supplier t2 on t2.ID=t1.SupplierID left join PartyInfo t3 on t3.ID=t1.PartyID  " + per + " order By t1.ID desc";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurchaseMst");
        return dt;
    }

    public static DataTable GetSupplierInfo(string Supplieer)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = @" select ID,Code,Name,ContactName,Gl_CoaCode,Phone from Supplier where Upper(isnull(Code,'')+' - '+ContactName)=UPPER('" + Supplieer + "')";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Supplier");
        return dt;
    }

    public static DataTable GetShowPVMasterInfo(string GRN)
    {
        String connectionString = DataManager.OraConnString();
        string query = @"SELECT t1.[ID] ,t1.[GRN] ,t1.[SupplierID],t2.ContactName ,t2.Gl_CoaCode,PvType
      FROM [ItemPurchaseMst] t1 inner join Supplier t2 on t2.ID=t1.SupplierID WHERE t1.GRN='" + GRN + "'";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurchaseDtl");
        return dt;
    }

    public static DataTable GetPurchaseVoucherHistory(string GRNo, string ChallanNo, string FromDate, string ToDate, string SupplierID)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string parameter = "";
        if (GRNo != "")
        {
            parameter = parameter + " and t1.ID=" + GRNo + " ";
        }
        else
        {
            if (SupplierID != "")
            {
                parameter = parameter + " and t2.ID='" + SupplierID + "' ";
            }

            if (ChallanNo != "")
            {
                parameter = parameter + " and t1.[ChallanNo]='" + ChallanNo + "' ";
            }
            if (FromDate != "" && ToDate != "")
            {
                parameter = parameter + " and Convert(date,t1.[ReceivedDate],103) between Convert(date,'" + FromDate + "',103) AND Convert(date,'" + ToDate + "',103) ";
            }
        }


        string query = @"SELECT t1.ID,t1.[GRN]
      ,CONVERT(NVARCHAR,t1.[ReceivedDate],103) AS ReceivedDate   
      ,t1.[ChallanNo]
      ,CONVERT(NVARCHAR,t1.[ChallanDate],103) AS ChallanDate
      ,t2.ContactName as Name
      ,t1.[Total]  
      ,t3.PartyName AS Party    
      ,convert(decimal(18,0),t4.PurchaseQty) AS PurchaseQty
  FROM [ItemPurchaseMst] t1 inner join Supplier t2 on t2.ID=t1.SupplierID left join PartyInfo t3 on t3.ID=t1.PartyID 
left join (select t.ItemPurchaseMstID,SUM(t.Quantity) AS[PurchaseQty] from ItemPurchaseDtl t group by t.ItemPurchaseMstID) t4 on t4.ItemPurchaseMstID=t1.ID 
where t1.DeleteBy is null " + parameter + " order By t1.ID desc";



        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurchaseMst");
        return dt;
    }

    public static DataTable GetShowREportInformation(string FromDate, string ToDate, string SupplierID, string ItemID, string StyleNo, string CetegoryID, string SubCetegoryID, string DesignNo, string Flag, string ReportType)
    {
        SqlConnection conn = new SqlConnection(DataManager.OraConnString());
        DataSet ds = new DataSet("ReportInformation");
        SqlCommand sqlComm = new SqlCommand("ReportInformation", conn);

        if (!string.IsNullOrEmpty(ItemID))
        {
            sqlComm.Parameters.AddWithValue("@ItemID", ItemID);
        }

        else
        {
            sqlComm.Parameters.AddWithValue("@ItemID", null);
        }

        if (FromDate == "" || ToDate == "")
        {
            sqlComm.Parameters.AddWithValue("@FromDate", null);
            sqlComm.Parameters.AddWithValue("@ToDate", null);
        }

        else
        {
            FromDate = DataManager.DateEncodestring(FromDate);
            ToDate = DataManager.DateEncodestring(ToDate);

            sqlComm.Parameters.AddWithValue("@FromDate", FromDate);
            sqlComm.Parameters.AddWithValue("@ToDate", ToDate);
        }
        if (!string.IsNullOrEmpty(StyleNo))
        {
            sqlComm.Parameters.AddWithValue("@StyleNo", StyleNo);
        }

        else
        {
            sqlComm.Parameters.AddWithValue("@StyleNo", null);
        }

        if (!string.IsNullOrEmpty(SupplierID) && SupplierID != "0")
        {
            sqlComm.Parameters.AddWithValue("@SupplierID", SupplierID);
        }

        else
        {
            sqlComm.Parameters.AddWithValue("@SupplierID", null);
        }

        if (!string.IsNullOrEmpty(CetegoryID))
        {
            sqlComm.Parameters.AddWithValue("@CetegoryID", CetegoryID);
        }

        else
        {
            sqlComm.Parameters.AddWithValue("@CetegoryID", null);
        }

        if (!string.IsNullOrEmpty(SubCetegoryID))
        {
            sqlComm.Parameters.AddWithValue("@SubCetegoryID", SubCetegoryID);
        }

        else
        {
            sqlComm.Parameters.AddWithValue("@SubCetegoryID", null);
        }

        if (!string.IsNullOrEmpty(DesignNo))
        {
            sqlComm.Parameters.AddWithValue("@DesignNo", DesignNo);
        }

        else
        {
            sqlComm.Parameters.AddWithValue("@DesignNo", null);
        }

        if (!string.IsNullOrEmpty(Flag))
        {
            sqlComm.Parameters.AddWithValue("@Flag", Flag);
        }

        if (!string.IsNullOrEmpty(ReportType))
        {
            sqlComm.Parameters.AddWithValue("@ReportType", ReportType);
        }

        else
        {
            sqlComm.Parameters.AddWithValue("@ReportType", null);
        }
        sqlComm.CommandType = CommandType.StoredProcedure;

        SqlDataAdapter da = new SqlDataAdapter();
        da.SelectCommand = sqlComm;

        da.Fill(ds);

        return ds.Tables[0];
    }
    public static DataTable PlGetShowREportInformation(string FromDate, string ToDate, string SupplierID, string ItemID, string StyleNo, string CetegoryID, string SubCetegoryID, string DesignNo, string Flag, string ReportType)
    {
        SqlConnection conn = new SqlConnection(DataManager.OraConnString());
        DataSet ds = new DataSet("lfReportInformation");
        SqlCommand sqlComm = new SqlCommand("lfReportInformation", conn);

        if (!string.IsNullOrEmpty(ItemID))
        {
            sqlComm.Parameters.AddWithValue("@ItemID", ItemID);
        }

        else
        {
            sqlComm.Parameters.AddWithValue("@ItemID", null);
        }

        if (FromDate == "" || ToDate == "")
        {
            sqlComm.Parameters.AddWithValue("@FromDate", null);
            sqlComm.Parameters.AddWithValue("@ToDate", null);
        }

        else
        {
            //FromDate = DataManager.DateEncodestring(FromDate);
            //ToDate = DataManager.DateEncodestring(ToDate);

            sqlComm.Parameters.AddWithValue("@FromDate", FromDate);
            sqlComm.Parameters.AddWithValue("@ToDate", ToDate);
        }
        if (!string.IsNullOrEmpty(StyleNo))
        {
            sqlComm.Parameters.AddWithValue("@StyleNo", StyleNo);
        }

        else
        {
            sqlComm.Parameters.AddWithValue("@StyleNo", null);
        }

        if (!string.IsNullOrEmpty(SupplierID) && SupplierID != "0")
        {
            sqlComm.Parameters.AddWithValue("@SupplierID", SupplierID);
        }

        else
        {
            sqlComm.Parameters.AddWithValue("@SupplierID", null);
        }

        if (!string.IsNullOrEmpty(CetegoryID))
        {
            sqlComm.Parameters.AddWithValue("@CetegoryID", CetegoryID);
        }

        else
        {
            sqlComm.Parameters.AddWithValue("@CetegoryID", null);
        }

        if (!string.IsNullOrEmpty(SubCetegoryID))
        {
            sqlComm.Parameters.AddWithValue("@SubCetegoryID", SubCetegoryID);
        }

        else
        {
            sqlComm.Parameters.AddWithValue("@SubCetegoryID", null);
        }

        if (!string.IsNullOrEmpty(DesignNo))
        {
            sqlComm.Parameters.AddWithValue("@DesignNo", DesignNo);
        }

        else
        {
            sqlComm.Parameters.AddWithValue("@DesignNo", null);
        }

        if (!string.IsNullOrEmpty(Flag))
        {
            sqlComm.Parameters.AddWithValue("@Flag", Flag);
        }

        if (!string.IsNullOrEmpty(ReportType))
        {
            sqlComm.Parameters.AddWithValue("@ReportType", ReportType);
        }

        else
        {
            sqlComm.Parameters.AddWithValue("@ReportType", null);
        }
        sqlComm.CommandType = CommandType.StoredProcedure;

        SqlDataAdapter da = new SqlDataAdapter();
        da.SelectCommand = sqlComm;

        da.Fill(ds);

        return ds.Tables[0];
    }

}