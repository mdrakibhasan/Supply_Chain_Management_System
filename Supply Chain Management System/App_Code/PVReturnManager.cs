using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using autouniv;
using OldColor;

/// <summary>
/// Summary description for PVReturnManager
/// </summary>
public class PVReturnManager
{
	public PVReturnManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public static DataTable GetShowPurchaseItems(string PVMst)
    {
        String connectionString = DataManager.OraConnString();
        string query = @"SELECT * FROM [View_PurchaseItems]  WHERE [ItemPurchaseMstID]='" + PVMst + "'";
        //string query = @"SELECT  t2.ID as ItemID ,t2.Name AS Items_Name FROM  Item t2 where t2.ItemsType in (1 ,3)";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "View_PurchaseItems");
        return dt;
    }

    public static DataTable GetShowPVMasterInfo(string GRN)
    {
        String connectionString = DataManager.OraConnString();
        string query = @"SELECT t1.[ID] ,t1.[GRN] ,t1.[SupplierID],t2.ContactName ,t2.Gl_CoaCode
      FROM [ItemPurchaseMst] t1 inner join Supplier t2 on t2.ID=t1.SupplierID WHERE t1.GRN='" + GRN + "'";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurchaseDtl");
        return dt;
    }

    public static DataTable GetPVItems(string ItemsID,string PVID)
    {
        String connectionString = DataManager.OraConnString();
        string query = @"SELECT t1.[ID]    
      ,'' AS item_code   
      ,t1.[ItemID]  AS item_desc   
      ,t1.[UnitPrice]  AS item_rate
      ,'' AS qnty     
      ,t1.[MsrUnitCode]  AS msr_unit_code   
      ,isnull(t1.Quantity,0)-isnull(t1.ReturnQuantity,0) AS[PvQty]   
  FROM [ItemPurchaseDtl] t1 where t1.[ItemPurchaseMstID]='" + PVID + "' and  t1.[ItemID]='" + ItemsID + "' ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurchaseDtl");
        return dt;
    }

    public static PVReturn getShowRetirnItems(string RId)
    {
        String connectionString = DataManager.OraConnString();
        string query = @"SELECT t1.[ID],t1.[GRN],CONVERT(nvarchar,t1.[ReturnDate],103) AS ReturnDate,t1.[Remarks],t1.[Return_No],t1.TotalAmount,CASE WHEN t2.PayMethod IS NULL THEN 'C' ELSE t2.PayMethod END AS [PaymentMethod],t2.Bank_id AS[BankName],t2.[ChequeNo],CONVERT(NVARCHAR,t2.[ChequeDate],103) as ChequeDate ,t2.Chk_Status,ISNULL(t2.PayAmt,0) AS Pay_Amount FROM [PurReturnMst] t1 left join SupplierPayment t2 on t2.purchase_id=t1.ID and t2.Payment_Type='PR' Where t1.[ID]='" + RId + "'";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "OtMaster");
        if (dt.Rows.Count == 0)
        {
            return null;
        }
        return new PVReturn(dt.Rows[0]);
    }

    public static void SavePurchaseReturn(PVReturn rtn, DataTable dt, VouchMst vmst, string SupplierGlCode)
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

            command.CommandText = @"INSERT INTO [PurReturnMst]
           ([GRN],[ReturnDate],[Remarks],[CreatedBy],[CreatedDate],Return_No,[TotalAmount],[Pay_Amount])
     VALUES
           ('" + rtn.GRN + "',convert(date,'" + rtn.ReturnDate + "',103),'" + rtn.Remarks + "','" + rtn.LogonBy + "','" + Globals._localTime.ToString() + "','" + rtn.Return_No + "','" + rtn.TotalAmount + "','" + rtn.Pay_Amount + "')";
            command.ExecuteNonQuery();

            command.CommandText = @"SELECT top(1) [ID]  FROM [PurReturnMst] order by ID desc";
            string PurchaseMstID = command.ExecuteScalar().ToString();           
            decimal totPay = 0;
            //***************************  ********************************// 
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["item_desc"].ToString() != "")
                {
                    command.CommandText = @"INSERT INTO [PurReturnDl]
                    ([PurReturnMstID],[ItemID],[UnitPrice],[Quantity],[Total],[CreatedBy],[CreatedDate])  
                    VALUES ('" + PurchaseMstID + "','" + dr["item_desc"].ToString() + "','" + dr["item_rate"].ToString() + "','" + dr["qnty"].ToString() + "','" + Convert.ToDouble(dr["item_rate"]) * Convert.ToDouble(dr["qnty"]) + "','" + rtn.LogonBy + "','" + Globals._localTime.ToString() + "')";
                    command.ExecuteNonQuery();
                }
            }
            //***************************  Jurnal Voucher ********************************//           

            command.CommandText = VouchManager.SaveVoucherMst(vmst, 1);
            command.ExecuteNonQuery();

            VouchDtl vdtl;
            for (int j = 0; j < 3; j++)
            {
                if (j == 0)
                {
                    //DataRow            

                    vdtl = new VouchDtl();
                    vdtl.VchSysNo = vmst.VchSysNo;
                    vdtl.ValueDate = rtn.ReturnDate;
                    vdtl.LineNo = "1";
                    vdtl.GlCoaCode = "1-" + SupplierGlCode;
                    vdtl.Particulars = rtn.SupplierName;
                    vdtl.AccType = VouchManager.getAccType("1-" + SupplierGlCode);
                    vdtl.AmountDr = vmst.ControlAmt.Replace(",", "");
                    vdtl.AmountCr = "0";
                    vdtl.Status = vmst.Status;
                    vdtl.BookName = vmst.BookName;
                    VouchManager.CreateVouchDtlForAutoVoucher(vmst, vdtl, command);
                }
                else if (j == 1)
                {
                    vdtl = new VouchDtl();
                    vdtl.VchSysNo = vmst.VchSysNo;
                    vdtl.ValueDate = rtn.ReturnDate;
                    vdtl.LineNo = "2";
                    vdtl.GlCoaCode = dtFixCode.Rows[0]["PurchaseCode"].ToString(); //**** Purchase Code *******//
                    vdtl.AccType = VouchManager.getAccType(vdtl.GlCoaCode); //**** Purchase Code *******//
                    vdtl.Particulars = "Item Purchas";
                    vdtl.AmountDr = "0";
                    vdtl.AmountCr = vmst.ControlAmt.Replace(",", "");
                    vdtl.Status = vmst.Status;
                    vdtl.BookName = vmst.BookName;
                    VouchManager.CreateVouchDtlForAutoVoucher(vmst, vdtl, command);
                }
                else if (j == 2)
                {
                    vdtl = new VouchDtl();
                    vdtl.VchSysNo = vmst.VchSysNo;
                    vdtl.ValueDate = rtn.ReturnDate;
                    vdtl.LineNo = "3";
                    vdtl.GlCoaCode = dtFixCode.Rows[0]["ClosingStock"].ToString(); ;
                    vdtl.Particulars = "Closing Stock";
                    vdtl.AccType = VouchManager.getAccType(vdtl.GlCoaCode);
                    vdtl.AmountDr = "0";
                    vdtl.AmountCr =  vmst.ControlAmt.Replace(",", "");
                    vdtl.AUTHO_USER = "CS";
                    vdtl.Status = vmst.Status;
                    vdtl.BookName = vmst.BookName;
                    VouchManager.CreateVouchDtlForAutoVoucher(vmst, vdtl, command);
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
    public static void UpdatePurchaseReturn(PVReturn rtn, DataTable dt, VouchMst vmst, string SupplierGlCode)
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

            command.CommandText = @"UPDATE [PurReturnMst]
   SET [ReturnDate] = '" + rtn.ReturnDate + "',[Remarks] ='" + rtn.Remarks + "' ,[ModifiedBy] ='" + rtn.LogonBy + "' ,[ModifiedDate] ='" + Globals._localTime.ToString() + "',[TotalAmount] ='" + rtn.TotalAmount + "',[Pay_Amount] ='" + rtn.Pay_Amount + "' WHERE ID='" + rtn.ID + "' ";
        
            command.CommandText = @"UPDATE [SupplierPayment]
   SET  [PayAmt] ='" + rtn.Pay_Amount + "' ,[Bank_id] ='" + rtn.BankName + "' ,[ChequeNo] ='" + rtn.ChequeNo + "' ,[ChequeDate] ='" + rtn.ChequeDate + "' ,[update_by] ='" + rtn.LogonBy + "' ,[update_date] ='" + Globals._localTime.ToString() + "' ,[Chk_Status] ='" + rtn.Chk_Status + "'  WHERE [purchase_id] ='" + rtn.ID + "' AND Payment_Type='PR' ";
            command.ExecuteNonQuery();


            command.CommandText = @"DELETE FROM [PurReturnDl] WHERE PurReturnMstID='" + rtn.ID + "'";
            command.ExecuteNonQuery();

            foreach (DataRow dr in dt.Rows)
            {
                if (!string.IsNullOrEmpty(rtn.ID) && dr["ID"].ToString() != "")
                {
                    command.CommandText = @"INSERT INTO [PurReturnDl]
                    ([PurReturnMstID],[ItemID],[UnitPrice],[Quantity],[Total],[CreatedBy],[CreatedDate])  
                    VALUES ('" + rtn.ID + "','" + dr["item_desc"].ToString() + "','" + dr["item_rate"].ToString() + "','" + dr["qnty"].ToString() + "','" + Convert.ToDouble(dr["item_rate"]) * Convert.ToDouble(dr["qnty"]) + "','" + rtn.LogonBy + "','" + Globals._localTime.ToString() + "')";
                    command.ExecuteNonQuery();
                }
            }

            //***************************  Jurnal Voucher ********************************//           

            command.CommandText = VouchManager.SaveVoucherMst(vmst, 2);
            command.ExecuteNonQuery();

            command.CommandText = @"delete from gl_trans_dtl where vch_sys_no=convert(numeric,'" + vmst.VchSysNo +
                                     "')";
            command.ExecuteNonQuery();

            VouchDtl vdtl;
            for (int j = 0; j < 3; j++)
            {
                if (j == 0)
                {
                    //DataRow            

                    vdtl = new VouchDtl();
                    vdtl.VchSysNo = vmst.VchSysNo;
                    vdtl.ValueDate = rtn.ReturnDate;
                    vdtl.LineNo = "1";
                    vdtl.GlCoaCode = "1-" + SupplierGlCode;
                    vdtl.Particulars = rtn.SupplierName;
                    vdtl.AccType = VouchManager.getAccType("1-" + SupplierGlCode);
                    vdtl.AmountDr = vmst.ControlAmt.Replace(",", "");
                    vdtl.AmountCr = "0";
                    vdtl.Status = vmst.Status;
                    vdtl.BookName = vmst.BookName;
                    VouchManager.CreateVouchDtlForAutoVoucher(vmst, vdtl, command);
                }
                else if (j == 1)
                {
                    vdtl = new VouchDtl();
                    vdtl.VchSysNo = vmst.VchSysNo;
                    vdtl.ValueDate = rtn.ReturnDate;
                    vdtl.LineNo = "2";
                    vdtl.GlCoaCode = dtFixCode.Rows[0]["PurchaseCode"].ToString(); //**** Purchase Code *******//
                    vdtl.AccType = VouchManager.getAccType(vdtl.GlCoaCode); //**** Purchase Code *******//
                    vdtl.Particulars = "Item Purchas";
                    vdtl.AmountDr = "0";
                    vdtl.AmountCr = vmst.ControlAmt.Replace(",", "");
                    vdtl.Status = vmst.Status;
                    vdtl.BookName = vmst.BookName;
                    VouchManager.CreateVouchDtlForAutoVoucher(vmst, vdtl, command);
                }
                else if (j == 2)
                {
                    vdtl = new VouchDtl();
                    vdtl.VchSysNo = vmst.VchSysNo;
                    vdtl.ValueDate = rtn.ReturnDate;
                    vdtl.LineNo = "3";
                    vdtl.GlCoaCode = dtFixCode.Rows[0]["ClosingStock"].ToString(); ;
                    vdtl.Particulars = "Closing Stock";
                    vdtl.AccType = VouchManager.getAccType(vdtl.GlCoaCode);
                    vdtl.AmountDr = "0";
                    vdtl.AmountCr = vmst.ControlAmt.Replace(",", "");
                    vdtl.AUTHO_USER = "CS";
                    vdtl.Status = vmst.Status;
                    vdtl.BookName = vmst.BookName;
                    VouchManager.CreateVouchDtlForAutoVoucher(vmst, vdtl, command);
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

    public static DataTable GetShowPurchaseReturnItems()
    {
        String connectionString = DataManager.OraConnString();
        string query = @"SELECT t1.[ID]
      ,t2.GRN 
      ,t1.[Return_No]
      ,CONVERT(NVARCHAR, t1.[ReturnDate],103) AS [ReturnDate]
      ,t1.[Remarks]   
  FROM [PurReturnMst] t1 inner join ItemPurchaseMst t2 on t2.ID=t1.GRN";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurchaseDtl");
        return dt;
    }

    public static DataTable ItemsDetails(string PurReturnMstID)
    {
        String connectionString = DataManager.OraConnString();
        string query = @"SELECT t1.[ID]  
	  ,'' AS item_code    
      ,t1.[ItemID] AS item_desc
      ,t1.[UnitPrice] AS item_rate
      ,'' AS msr_unit_code
      ,t1.[Quantity] AS qnty
      ,t1.[Total]     
      ,t3.BrandName
      ,t2.Name AS[des_name]
      ,isnull(prvQty.Quantity,0)-isnull(t1.Quantity,0) AS[PvQty]
  FROM [PurReturnDl] t1 inner join PurReturnMst tt1 on tt1.ID=t1.PurReturnMstID left join Item t2 on t2.ID=t1.ItemID left join Brand t3 on t3.ID=t2.Brand
  left join (select t2.ID,t1.ItemID,t1.Quantity from ItemPurchaseDtl t1 inner join ItemPurchaseMst t2 on t2.ID=t1.ItemPurchaseMstID) prvQty on prvQty.ID=tt1.GRN and prvQty.ItemID=t1.ItemID where t1.[PurReturnMstID]='" + PurReturnMstID + "'  ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "PurReturnDl");
        return dt;
    }

    public static void DeleteItemsReturn(PVReturn rtn)
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

            string Query = @"select t1.VCH_SYS_NO  from [GL_TRANS_MST] t1 where SERIAL_NO='" + rtn.Return_No + "'";
            DataTable dt = DataManager.ExecuteQuery(connectionString, Query, "ItemPurchaseMst");

            command.CommandText = @"DELETE FROM [GL_TRANS_MST]  WHERE SERIAL_NO='" + rtn.Return_No + "'";
            command.ExecuteNonQuery();

            foreach (DataRow dr in dt.Rows)
            {
                command.CommandText = @"DELETE FROM [GL_TRANS_DTL]  WHERE VCH_SYS_NO='" + dr["VCH_SYS_NO"].ToString() + "'";
                command.ExecuteNonQuery();
            }

            command.CommandText = @"DELETE FROM [PurReturnDl] WHERE PurReturnMstID='" + rtn.ID + "'";
            command.ExecuteNonQuery();

            command.CommandText = @"DELETE FROM [PurReturnMst] WHERE  ID='" + rtn.ID + "'";
            command.ExecuteNonQuery();

            command.CommandText = @"DELETE FROM [SupplierPayment] WHERE  purchase_id='" + rtn.ID + "' AND Payment_Type='PR' ";
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
}