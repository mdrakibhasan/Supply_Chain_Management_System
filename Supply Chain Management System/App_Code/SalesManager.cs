using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using autouniv;

/// <summary>
/// Summary description for SalesManager
/// </summary>
public class SalesManager
{
	public SalesManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public static DataTable GetShowItemsInformation(string ID)
    {
        String connectionString = DataManager.OraConnString();
        string query = @"SELECT t1.[ID]
      ,t1.[Code]
      ,t1.[Name]             
      ,isnull(t2.Rate,0) AS Tax
      ,t1.[DiscountAmount] 
      ,t1.[UnitPrice] AS SPrice
      ,'1' AS Qty    
      ,t1.ClosingStock
      ,convert(decimal(18,2),((ISNULL(t1.UnitPrice,0)*1)-(ISNULL(t1.UnitPrice,0)*(t1.DiscountAmount/100)))) AS Total  FROM [Item] t1 left join TaxCategory t2 on t2.ID=t1.TaxCategoryID and t2.Active='1' where t1.[Active]='1' and t1.Code='" + ID + "' and t1.ClosingStock>0 ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Item");
        return dt;
    }

    public static Sales GetShowSalesInfo(string ID)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = @"SELECT t1.[ID],t1.[CompanyId],t1.[InvoiceNo],convert(decimal(18,2),t1.[SubTotal]) AS SubTotal,convert(decimal(18,2),t1.[TaxAmount]) AS TaxAmount,convert(decimal(18,2),t1.[DiscountAmount]) AS DiscountAmount,convert(decimal(18,2),t1.[GrandTotal]) AS GrandTotal,convert(decimal(18,2),t1.[CashReceived]) AS CashReceived,t1.[CashRefund],convert(nvarchar,t1.[OrderDate],103) AS OrderDate,t1.[PaymentMethodID],t2.ChequeNo AS [PaymentMethodNumber],t2.Bank_id AS [BankId],convert(nvarchar,t2.[ChequeDate],103) AS ChequeDate,convert(decimal(18,2),t1.[ChequeAmount]) AS ChequeAmount,t1.[CustomerID],t1.[OrderStatusID],t1.[CreatedBy],t1.[CreatedDate] ,t1.[ModifiedBy],t1.[ModifiedDate],(ISNULL(t1.SubTotal,0)-ISNULL(t1.CashReceived,0)) AS Due,t1.[DeliveryStatus],convert(nvarchar,t1.[DeliveryDate],103) AS DeliveryDate,t1.[Remark],t2.Chk_Status FROM [Order] t1 left join CustomerPaymentReceive t2 on t2.Customer_id=t1.ID where t1.ID='" + ID + "'";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Order");
        if (dt.Rows.Count == 0)
        {
            return null;
        }
        return new Sales(dt.Rows[0]);
    }

    public static void SaveSalesInfo(Sales aSales, DataTable dt)
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

            command.CommandText = @"INSERT INTO [Order]
           ([InvoiceNo],[SubTotal],[TaxAmount],[DiscountAmount],[GrandTotal],[CashReceived],[CashRefund],[OrderDate],[CustomerID],[Due],[DeliveryStatus],[DeliveryDate],[Remark],[CreatedBy],[CreatedDate])
     VALUES
           ('" + aSales.Invoice + "','" + aSales.Total + "','" + aSales.Tax + "','" + aSales.Disount + "','" + aSales.GTotal + "','" + aSales.CReceive + "','0',convert(datetime, nullif( '" + aSales.Date + "',''), 103),'" + aSales.Customer + "','" + aSales.Due + "','" + aSales.DvStatus + "',convert(datetime, nullif( '" + aSales.DvDate + "',''), 103),'" + aSales.Remarks + "','" + aSales.LoginBy + "','" + Globals._localTime.ToString() + "')";
            command.ExecuteNonQuery();

            command.CommandText = @"SELECT top(1) [ID]  FROM [Order] order by ID desc";
            string OrderMstID = command.ExecuteScalar().ToString();


            //*************************** Customer Payment ******************//
            decimal totPay = 0;
            if (aSales.PMethod == "C" && Convert.ToDouble(aSales.CReceive) > 0)
            {
                command.CommandText = @"INSERT INTO [CustomerPaymentReceive]
           ([Date],[Customer_id],[Invoice],[PayAmt],[PayMethod],[Bank_id],[ChequeNo],[ChequeDate],[entry_by],[entry_date],Payment_Type)
     VALUES
           (CONVERT(DATE,'" + aSales.Date + "',103),'" + aSales.Customer + "','" + OrderMstID + "','" + aSales.CReceive + "','" + aSales.PMethod + "','" + aSales.BankId + "','" + aSales.PMNumber + "',CONVERT(DATE,'" + aSales.ChequeDate + "',103),'" + aSales.LoginBy + "','" + Globals._localTime.ToString() + "','IV')";
                command.ExecuteNonQuery();
            }

            if (aSales.PMethod == "Q")
            {
                if (Convert.ToDecimal(aSales.CReceive) == 0) { totPay = Convert.ToDecimal(aSales.GTotal); } else { totPay = Convert.ToDecimal(aSales.CReceive); }
                command.CommandText = @"INSERT INTO [CustomerPaymentReceive]
           ([Date],[Customer_id],[Invoice],[PayAmt],[PayMethod],[Bank_id],[ChequeNo],[ChequeDate],[entry_by],[entry_date],Chk_Status,Payment_Type)
     VALUES
           (CONVERT(DATE,'" + aSales.Date + "',103),'" + aSales.Customer + "','" + OrderMstID + "','" + totPay + "','" + aSales.PMethod + "','" + aSales.BankId + "','" + aSales.PMNumber + "',CONVERT(DATE,'" + aSales.ChequeDate + "',103),'" + aSales.LoginBy + "','" + Globals._localTime.ToString() + "','" + aSales.Chk_Status + "','IV')";
                command.ExecuteNonQuery();

            }
            if (Convert.ToDecimal(aSales.CReceive) > 0 && aSales.PMethod == "CR")
            {
                command.CommandText = @"INSERT INTO [CustomerPaymentReceive]
           ([Date],[Customer_id],[Invoice],[PayAmt],[PayMethod],[Bank_id],[ChequeNo],[ChequeDate],[entry_by],[entry_date],Payment_Type)
     VALUES
           (CONVERT(DATE,'" + aSales.Date + "',103),'" + aSales.Customer + "','" + OrderMstID + "','" + aSales.CReceive + "','" + aSales.PMethod + "','" + aSales.BankId + "','" + aSales.PMNumber + "',CONVERT(DATE,'" + aSales.ChequeDate + "',103),'" + aSales.LoginBy + "','" + Globals._localTime.ToString() + "','IV')";
                command.ExecuteNonQuery();
            }
            //***************************  ********************************// 
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["Code"].ToString() != "")
                {
                    command.CommandText = @"INSERT INTO [OrderDetail]
           ([OrderID],[ItemID],[UnitPrice]  ,[TaxRate] ,[DiscountAmount] ,[SalePrice] ,[Quantity] ,[TotalPrice] ,[CreatedBy],[CreatedDate])
     VALUES
           ('" + OrderMstID + "','" + dr["ID"].ToString() + "','" + dr["SPrice"].ToString().Replace(",", "") + "','" + dr["Tax"].ToString().Replace(",", "") + "','" + dr["DiscountAmount"].ToString().Replace(",", "") + "','" + dr["SPrice"].ToString().Replace(",", "") + "','" + dr["Qty"].ToString().Replace(",", "") + "','" + dr["Total"].ToString().Replace(",", "") + "','" + aSales.LoginBy + "','" + Globals._localTime.ToString() + "')";
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

    public static void UpdateSalesInfo(Sales aSales, DataTable dt)
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

            command.CommandText = @"update [Order] set
           [SubTotal]='" + aSales.Total + "',[TaxAmount]='" + aSales.Tax + "',[DiscountAmount]='" + aSales.Disount + "',[GrandTotal]='" + aSales.GTotal + "',[CashReceived]='" + aSales.CReceive + "',[CashRefund]='0',[OrderDate]=convert(date,'" + aSales.Date + "',103),[CustomerID]='" + aSales.Customer + "',[Due]='" + aSales.Due + "',[DeliveryStatus]='" + aSales.DvStatus + "',[DeliveryDate]=convert(date,'" + aSales.DvDate + "',103),[Remark]='" + aSales.Remarks + "',[ModifiedBy]='" + aSales.LoginBy + "',[ModifiedDate]='" + Globals._localTime.ToString() + "' where ID='" + aSales.ID + "'";    
            command.ExecuteNonQuery();

            command.CommandText = @"delete from [OrderDetail] where OrderID='" + aSales.ID + "'";
            command.ExecuteNonQuery();
           
            command.CommandText = @"UPDATE [CustomerPaymentReceive]
   SET [Date] =CONVERT(DATE,'" + aSales.Date + "',103),[PayAmt] ='" + aSales.Total + "' ,[ChequeNo] ='" + aSales.PMNumber + "' ,[ChequeDate] = CONVERT(DATE,'" + aSales.ChequeDate + "',103),[update_by] ='" + aSales.LoginBy + "',[update_date] ='" + Globals._localTime.ToString() + "' WHERE Invoice='" + aSales.ID + "' ";
            command.ExecuteNonQuery();

            foreach (DataRow dr in dt.Rows)
            {
                if (dr["Code"].ToString() != "")
                {
                    command.CommandText = @"INSERT INTO [OrderDetail]
           ([OrderID] ,[ItemID] ,[UnitPrice] ,[TaxRate] ,[DiscountAmount] ,[SalePrice] ,[Quantity] ,[TotalPrice] ,[CreatedBy] ,[CreatedDate])
     VALUES
           ('" + aSales.ID + "','" + dr["ID"].ToString() + "','" + dr["SPrice"].ToString().Replace(",", "") + "','" + dr["Tax"].ToString().Replace(",", "") + "','" + dr["DiscountAmount"].ToString().Replace(",", "") + "','" + dr["SPrice"].ToString().Replace(",", "") + "','" + dr["Qty"].ToString().Replace(",", "") + "','" + dr["Total"].ToString().Replace(",", "") + "','" + aSales.LoginBy + "','" + Globals._localTime.ToString() + "')";
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

    public static DataTable GetShowSalesDetails()
    {
        String connectionString = DataManager.OraConnString();
        string query = @"select t1.ID,t1.InvoiceNo,t2.ContactName as [CustomerName],CONVERT(nvarchar,t1.OrderDate,103)OrderDate,DeliveryStatus AS [Status],convert(decimal(18,2),t1.CashReceived) AS [CashReceived]  from [Order] t1 left join Customer t2 on t2.ID=t1.CustomerID order by t1.ID desc ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Item");
        return dt;
    }

    public static DataTable GetSalesDetails(string OrderMstId)
    {
        String connectionString = DataManager.OraConnString();
        string query = @"select t2.ID,t2.Code,t2.Name,convert(decimal(18,2),t1.TaxRate) AS Tax,convert(decimal(18,2),t1.DiscountAmount) AS DiscountAmount,convert(decimal(18,2),t1.UnitPrice) AS SPrice,convert(decimal(18,2),t1.Quantity)  AS Qty,t2.ClosingStock,convert(decimal(18,2),TotalPrice) AS Total,t3.BrandName from OrderDetail t1 left join Item t2 on t2.ID=t1.ItemID left join Brand t3 on t3.ID=t2.Brand where t1.OrderID='" + OrderMstId + "'";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "OrderDetail");
        return dt;
    }

    public static void DeleteSalesVoucher(Sales aSales)
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

            string Query = @"select t1.VCH_SYS_NO  from [GL_TRANS_MST] t1 where SERIAL_NO='" + aSales.Invoice + "'";
            DataTable dt = DataManager.ExecuteQuery(connectionString, Query, "ItemPurchaseMst");

            command.CommandText = @"DELETE FROM [GL_TRANS_MST]  WHERE SERIAL_NO='" + aSales.Invoice + "'";
            command.ExecuteNonQuery();

            foreach (DataRow dr in dt.Rows)
            {
                command.CommandText = @"DELETE FROM [GL_TRANS_DTL]  WHERE VCH_SYS_NO='" + dr["VCH_SYS_NO"].ToString() + "'";
                command.ExecuteNonQuery();
            }

            command.CommandText = @"DELETE FROM [Order] WHERE  ID='" + aSales.ID + "'";
            command.ExecuteNonQuery();

            command.CommandText = @"DELETE FROM [OrderDetail] WHERE OrderID='" + aSales.ID + "'";
            command.ExecuteNonQuery();

            command.CommandText = @"DELETE FROM [CustomerPaymentReceive] WHERE Invoice='" + aSales.ID + "' AND Payment_Type ='IV'";
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