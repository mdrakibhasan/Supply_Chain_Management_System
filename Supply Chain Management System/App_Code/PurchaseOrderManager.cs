using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OldColor;
using System.Data.SqlClient;
using System.Data;
using autouniv;

/// <summary>
/// Summary description for PurchaseOrderManager
/// </summary>
public class PurchaseOrderManager
{
	public PurchaseOrderManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public static PurchaseOrderInfo GetPurchaseOrderMst(string ID)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = @"SELECT [ID]
      ,[PO]
      ,CONVERT(NVARCHAR,[PODate],103)PODate
      ,[SupplierID]
      ,[TermsOfDelivery]
      ,[TermsOfPayment]
      ,CONVERT(NVARCHAR,[ExpDelDate],103)ExpDelDate
      ,[OrderStatus]
      ,[CreatedBy]
      ,[CreatedDate]
      ,[ModifiedBy]
      ,[ModifiedDate]
  FROM [ItemPurOrderMst] where ID='" + ID + "' and DeleteBy is null";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurOrderMst");
        sqlCon.Close();
        if (dt.Rows.Count == 0)
        {
            return null;
        }
        return new PurchaseOrderInfo(dt.Rows[0]);
    }

    public static void SavePurchaseOrder(PurchaseOrderInfo pomst, DataTable dt)
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

            command.CommandText = @"INSERT INTO [ItemPurOrderMst]
           ([PO],[PODate],[SupplierID],[TermsOfDelivery],[TermsOfPayment],[ExpDelDate],[OrderStatus],[CreatedBy],[CreatedDate])
     VALUES
           ('" + pomst.PO + "',convert(date,'" + pomst.PODate + "',103),'" + pomst.SupplierID + "','" + pomst.TermsOfDelivery.Replace("'", "") + "','" + pomst.TermsOfPayment.Replace("'", "") + "',convert(date,'" + pomst.ExpDelDate + "',103),'" + pomst.OrderStatus + "','" + pomst.LoginBy + "','" + Globals._localTime.ToString() + "')";
            command.ExecuteNonQuery();

            command.CommandText = @"SELECT top(1) [ID]  FROM [ItemPurOrderMst] order by ID desc";
            string OrderMstID = command.ExecuteScalar().ToString();

            foreach (DataRow dr in dt.Rows)
            {
                if (dr["ID"].ToString() != "")
                {
                    command.CommandText = @"INSERT INTO [ItemPurOrderDtl]
           ([ItemOrderMstID],[ItemID],[UnitPrice],[Quantity],[Total],[CreatedBy],[CreatedDate],[MsrUnitCode])
     VALUES
           ('" + OrderMstID + "','" + dr["ID"].ToString() + "','" + dr["item_rate"].ToString() + "','" + dr["qnty"].ToString() + "','" + Convert.ToDouble(dr["item_rate"].ToString()) * Convert.ToDouble(dr["qnty"].ToString()) + "','" + pomst.LoginBy + "','" + Globals._localTime.ToString() + "','" + dr["msr_unit_code"].ToString() + "')";
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

    public static void UpdatePurchaseOrder(PurchaseOrderInfo pomst, DataTable dt)
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

            command.CommandText = @"UPDATE [ItemPurOrderMst]
   SET [PODate] =convert(date,'" + pomst.PODate + "',103) ,[SupplierID] ='" + pomst.SupplierID + "',[TermsOfDelivery] ='" + pomst.TermsOfDelivery.Replace("'", "") + "' ,[TermsOfPayment] = '" + pomst.TermsOfPayment.Replace("'", "") + "',[ExpDelDate] =convert(date,'" + pomst.ExpDelDate + "',103) ,[OrderStatus] ='" + pomst.OrderStatus + "',[ModifiedBy] ='" + pomst.LoginBy + "' ,[ModifiedDate] ='" + Globals._localTime.ToString() + "' WHERE ID='" + pomst.ID + "'";
            command.ExecuteNonQuery();

            command.CommandText = @"delete from ItemPurOrderDtl where ItemOrderMstID='" + pomst.ID + "'";
            command.ExecuteNonQuery();

            foreach (DataRow dr in dt.Rows)
            {
                if (dr["ID"].ToString() != "")
                {
                    command.CommandText = @"INSERT INTO [ItemPurOrderDtl]
           ([ItemOrderMstID],[ItemID],[UnitPrice],[Quantity],[Total],[CreatedBy],[CreatedDate],[MsrUnitCode])
     VALUES
           ('" + pomst.ID + "','" + dr["ID"].ToString() + "','" + dr["item_rate"].ToString() + "','" + dr["qnty"].ToString() + "','" + Convert.ToDouble(dr["item_rate"].ToString()) * Convert.ToDouble(dr["qnty"].ToString()) + "','" + pomst.LoginBy + "','" + Globals._localTime.ToString() + "','" + dr["msr_unit_code"].ToString() + "')";
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

    public static void DeletePurchaseOrder(PurchaseOrderInfo pomst)
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

            command.CommandText = @"UPDATE [ItemPurOrderMst]   SET [DeleteBy] = '" + pomst.LoginBy + "',[DeleteDate] ='" + Globals._localTime.ToString() + "' WHERE ID='" + pomst.ID + "'";
            command.ExecuteNonQuery();

            command.CommandText = @" UPDATE [ItemPurOrderDtl]  SET [DeleteBy] = '" + pomst.LoginBy + "',[DeleteDate] ='" + Globals._localTime.ToString() + "' WHERE  ItemOrderMstID='" + pomst.ID + "'";
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

    public static DataTable GetShowPurchaseOrder()
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = @"SELECT t1.ID
      ,t1.[PO]
      ,CONVERT(NVARCHAR,t1.[PODate],103) AS PODate
      ,CONVERT(NVARCHAR,t2.ContactName,103) AS  Supplier_Name    
      ,t1.[ExpDelDate]
      ,case when t1.[OrderStatus]='P' then 'Pending' 
       when t1.[OrderStatus]='C' then 'Completed' 
       when t1.[OrderStatus]='CA' then 'Cancel' else '' end AS [Status]
  FROM [ItemPurOrderMst] t1 inner join Supplier t2 on t2.ID=t1.[SupplierID] where t1.DeleteBy is null";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurOrderMst");
        return dt;       
    }

    public static DataTable GetPurchaseOrderItemsDetails(string ID)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query =
            @"SELECT t1.[ItemID] AS ID,t2.StyleNo AS item_code,t2.Name AS item_desc,CONVERT(nvarchar,t1.[UnitPrice]) AS item_rate,CONVERT(nvarchar,t1.[Quantity]) AS qnty,CONVERT(nvarchar,t1.[Total]) AS [Total],CONVERT(nvarchar,t1.[MsrUnitCode]) AS msr_unit_code,t3.Name AS UMO,'0' AS Additional,t4.BrandName
  FROM ItemPurOrderDtl t1 inner join Item t2 on t2.ID=t1.ItemID inner join UOM t3 on t3.ID=t1.MsrUnitCode left join Brand t4 on t4.ID=t2.Brand where t1.DeleteBy is null and t1.ItemOrderMstID='" + ID + "' union all select '','','','','','','','','','' ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurOrderDtl");
        return dt;    
    }

    public static DataTable GetShowOrder(string OrderId)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = @"SELECT * FROM [ItemPurOrderMst] t1 inner join Supplier t2 on t2.ID=t1.SupplierID where UPPER(t1.[PO]+' - '+t2.ContactName+' - '+CONVERT(NVARCHAR,t1.[PODate],103))='" + OrderId + "' and t1.DeleteBy is null ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurOrderDtl");
        return dt; 
    }

    public static DataTable GetItemTranshistory(string ItemID,string FromDate ,string ToDate)
    {
        try
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection con = new SqlConnection(DataManager.OraConnString());




            SqlCommand sqlComm = new SqlCommand("SP_ItemTranDital", con);
            //da.SelectCommand = new SqlCommand("SP_ItemTranDital", con);
            //da.SelectCommand.CommandType = CommandType.StoredProcedure;

            if (!string.IsNullOrEmpty(FromDate))
            {
                sqlComm.Parameters.AddWithValue("@StartDate", DateTime.ParseExact(FromDate, "dd/MM/yyyy", null));
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@StartDate", null);
            }

            if (!string.IsNullOrEmpty(ToDate))
            {
                sqlComm.Parameters.AddWithValue("@Enddate", DateTime.ParseExact(ToDate, "dd/MM/yyyy", null));
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@Enddate", null);
            }

            if (!string.IsNullOrEmpty(ItemID))
            {
                sqlComm.Parameters.AddWithValue("@ItemID", ItemID);
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@ItemID", 1);
            }


            sqlComm.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter();
            DataSet ds = new DataSet();
            da.SelectCommand = sqlComm;
            da.Fill(ds, "SP_ItemTranDital");
            DataTable dtStdDtl = ds.Tables["SP_ItemTranDital"];
            return dtStdDtl;

            //DataSet ds =  new DataSet();
            //da.Fill(ds, "SP_ItemTranDital");
            //ds.Tables[0].TableName = "SP_ItemTranDital";
            //return ds.Tables[0];
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
       
        
    }

    public static DataTable GetPurchaseOrderHistory(string PoNo, string SupplierID, string FromDate, string ToDate)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);

        string Parameter = "";
        if (PoNo != "")
        {
            Parameter = Parameter + " and t1.[PO]='" + PoNo + "'";
        }
        else
        {
            if (SupplierID != "")
            {
                Parameter = Parameter + " and t2.ID=" + SupplierID + "";
            }

            if (FromDate != "" && ToDate != "")
            {
                Parameter = Parameter + " and Convert(date,t1.[PODate],103) between Convert(date,'" + FromDate + "',103) AND Convert(date,'" + ToDate + "',103) ";
            }
        }
        string query = @"SELECT t1.ID
      ,t1.[PO]
      ,CONVERT(NVARCHAR,t1.[PODate],103) AS PODate
      ,CONVERT(NVARCHAR,t2.ContactName,103) AS  Supplier_Name    
      ,t1.[ExpDelDate]
      ,case when t1.[OrderStatus]='P' then 'Pending' 
       when t1.[OrderStatus]='C' then 'Completed' 
       when t1.[OrderStatus]='CA' then 'Cancel' else '' end AS [Status]
  FROM [ItemPurOrderMst] t1 inner join Supplier t2 on t2.ID=t1.[SupplierID] where t1.DeleteBy is null " + Parameter + " order By t1.ID desc";


        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurOrderMst");
        return dt;
    }
}