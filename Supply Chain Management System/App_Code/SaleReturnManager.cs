using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using autouniv;

/// <summary>
/// Summary description for SaleReturnManager
/// </summary>
public class SaleReturnManager
{
	public SaleReturnManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public static System.Data.DataTable GetShowSLMasterInfo(string IV)
    {
        String connectionString = DataManager.OraConnString();
        string query = @"SELECT t1.[ID] ,t1.InvoiceNo ,t1.CustomerID,t2.ContactName 
      FROM [Order] t1 inner join Customer t2 on t2.ID=t1.CustomerID WHERE t1.InvoiceNo='" + IV + "'";
//        string query = @"SELECT t1.[ID] ,t1.InvoiceNo ,t1.CustomerID,t2.ContactName ,t2.Gl_CoaCode
//      FROM [Order] t1 inner join Customer t2 on t2.ID=t1.CustomerID WHERE t1.InvoiceNo='" + IV + "'";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurchaseDtl");
        return dt;
    }

    public static DataTable GetSalesReturnItems(string ID)
    {
        String connectionString = DataManager.OraConnString();
        string query = @" SELECT  t1.[ItemID],t2.Name AS Items_Name FROM OrderDetail t1 inner join Item t2 on t2.ID=t1.ItemID where t1.OrderID='" + ID + "'";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurchaseDtl");
        return dt;
    }

    public static DataTable GetIVItems(string ItemsID, string IV)
    {
        String connectionString = DataManager.OraConnString();
        string query = @"SELECT t1.[ID]    
      ,'' AS item_code   
      ,t1.[ItemID]  AS item_desc   
      ,CONVERT(DECIMAL(18,2),t1.[UnitPrice])  AS item_rate
      ,'' AS qnty     
      ,''  AS msr_unit_code 
,     Quantity
  FROM OrderDetail t1 where t1.OrderID='" + IV + "' and  t1.[ItemID]='" + ItemsID + "' ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurchaseDtl");
        return dt;
    }

    public static SaleReturn getShowRetirnItems(string p)
    {
        String connectionString = DataManager.OraConnString();
        string query = @"SELECT t1.[ID],t1.InvoiceNo,CONVERT(nvarchar,t1.[ReturnDate],103) AS ReturnDate,t1.[Remarks],t1.[Return_No],t1.TotalAmount,CASE WHEN t2.PayMethod IS NULL THEN 'C' ELSE t2.PayMethod END AS [PaymentMethod],t2.Bank_id AS[BankName],t2.[ChequeNo],CONVERT(NVARCHAR,t2.[ChequeDate],103) as ChequeDate ,t2.Chk_Status,ISNULL(t2.PayAmt,0) AS Pay_Amount FROM [OrderReturn] t1 left join CustomerPaymentReceive t2 on t2.Invoice=t1.ID and t2.Payment_Type='IV' Where t1.[ID]='" + p + "'";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "OtMaster");
        if (dt.Rows.Count == 0)
        {
            return null;
        }
        return new SaleReturn(dt.Rows[0]);
    }

    public static void SaveInvoiceReturn(SaleReturn rtn, DataTable dt)
    {
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transaction;
        try
        {
            connection.Open();

            transaction = connection.BeginTransaction();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transaction;

            command.CommandText = @"INSERT INTO [OrderReturn]
           ([InvoiceNo],[ReturnDate],[Remarks],[CreatedBy],[CreatedDate],[Return_No],[TotalAmount],[Pay_Amount])
     VALUES
           ('" + rtn.GRN + "',convert(date,'" + rtn.ReturnDate + "',103),'" + rtn.Remarks + "','" + rtn.LogonBy + "','" + Globals._localTime.ToString() + "','" + rtn.Return_No + "','" + rtn.TotalAmount + "','" + rtn.Pay_Amount + "')";
            command.ExecuteNonQuery();

            command.CommandText = @"SELECT top(1) [ID]  FROM [OrderReturn] order by ID desc";
            string PurchaseMstID = command.ExecuteScalar().ToString();

            foreach (DataRow dr in dt.Rows)
            {
                if (dr["item_desc"].ToString() != "")
                {
                    command.CommandText = @"INSERT INTO [OrderReturnDetail]
                    ([OrderReturnMstID],[ItemID],[UnitPrice],[Quantity],[Total],[CreatedBy],[CreatedDate])  
                    VALUES ('" + PurchaseMstID + "','" + dr["item_desc"].ToString() + "','" + dr["item_rate"].ToString() + "','" + dr["qnty"].ToString() + "','" + Convert.ToDouble(dr["item_rate"]) * Convert.ToDouble(dr["qnty"]) + "','" + rtn.LogonBy + "','" + Globals._localTime.ToString() + "')";
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

    public static void UpdateSalesReturn(SaleReturn rtn, DataTable dt)
    {
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transaction;
        try
        {
            connection.Open();

            transaction = connection.BeginTransaction();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transaction;

            command.CommandText = @"UPDATE [OrderReturn]
   SET [ReturnDate] = convert(date,'" + rtn.ReturnDate + "',103),[Remarks] ='" + rtn.Remarks + "' ,[ModifiedBy] ='" + rtn.LogonBy + "' ,[ModifiedDate] ='" + Globals._localTime.ToString() + "',[TotalAmount] ='" + rtn.TotalAmount + "',[Pay_Amount] ='" + rtn.Pay_Amount + "' WHERE ID='" + rtn.ID + "' ";
            command.ExecuteNonQuery();

            command.CommandText = @"UPDATE [CustomerPaymentReceive]
   SET [Date] =convert(date,'" + rtn.ReturnDate + "',103) ,[Customer_id] = '" + rtn.SupplierID + "',[PayAmt] ='" + rtn.Pay_Amount + "' ,[PayMethod] ='" + rtn.PaymentMethod + "' ,[Bank_id] ='" + rtn.BankName + "' ,[ChequeNo] ='" + rtn.ChequeNo + "' ,[ChequeDate] =convert(date,'" + rtn.ChequeDate + "',103),[Chk_Status] ='" + rtn.Chk_Status + "',[update_by] ='" + rtn.LogonBy + "' ,[update_date] = '" + Globals._localTime.ToString() + "'  WHERE Invoice='" + rtn.ID + "' ";
            command.ExecuteNonQuery();

            command.CommandText = @"DELETE FROM [OrderReturnDetail] WHERE OrderReturnMstID='" + rtn.ID + "'";
            command.ExecuteNonQuery();

            foreach (DataRow dr in dt.Rows)
            {
                if (dr["item_desc"].ToString() != "")
                {
                    command.CommandText = @"INSERT INTO [OrderReturnDetail]
                    ([OrderReturnMstID],[ItemID],[UnitPrice],[Quantity],[Total],[CreatedBy],[CreatedDate])  
                    VALUES ('" + rtn.ID + "','" + dr["item_desc"].ToString() + "','" + dr["item_rate"].ToString() + "','" + dr["qnty"].ToString() + "','" + Convert.ToDouble(dr["item_rate"]) * Convert.ToDouble(dr["qnty"]) + "','" + rtn.LogonBy + "','" + Globals._localTime.ToString() + "')";
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

    public static DataTable GetShowSalesReturnItems()
    {
        String connectionString = DataManager.OraConnString();
        string query = @"SELECT t1.[ID]
      ,t2.InvoiceNo AS GRN 
      ,t1.[Return_No]
      ,CONVERT(NVARCHAR, t1.[ReturnDate],103) AS [ReturnDate]
      ,t1.[Remarks]   
  FROM dbo.OrderReturn t1 inner join [Order] t2 on t2.ID=t1.InvoiceNo";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "OrderReturn");
        return dt;
    }

    public static DataTable ItemsDetails(string p)
    {
        String connectionString = DataManager.OraConnString();
        string query = @" SELECT t1.[ID]  
	  ,'' AS item_code    
      ,t1.[ItemID] AS item_desc
      ,t1.[UnitPrice] AS item_rate
      ,'' AS msr_unit_code
      ,t1.[Quantity] AS qnty
      ,t1.[Total]      
      ,t3.BrandName
      ,t2.Name AS[des_name]
  FROM OrderReturnDetail t1 left join Item t2 on t2.ID=t1.ItemID left join Brand t3 on t3.ID=t2.Brand where t1.OrderReturnMstID='" + p + "'";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "OrderReturn");
        return dt;
    }

    public static void DeleteItemsReturn(SaleReturn rtn)
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

            command.CommandText = @"DELETE FROM [OrderReturn] WHERE ID='" + rtn.ID + "'";
            command.ExecuteNonQuery();

            command.CommandText = @"DELETE FROM [OrderReturnDetail]  WHERE OrderReturnMstID='"+rtn.ID+"'";
            command.ExecuteNonQuery();

            command.CommandText = @"DELETE FROM [CustomerPaymentReceive] WHERE Invoice='" + rtn.ID + "' and Payment_Type='IR'";
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