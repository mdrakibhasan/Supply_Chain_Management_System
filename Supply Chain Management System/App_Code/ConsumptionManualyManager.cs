using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using autouniv;
using System.Data.SqlClient;
using System.Data;
using OldColor;

/// <summary>
/// Summary description for ConsumptionManualyManager
/// </summary>
public class ConsumptionManualyManager
{
	public ConsumptionManualyManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public static void SaveConsumptionManualy(ConsumptionManualyInfo aConsumptionManualyInfo, DataTable dtitemDtl, VouchMst aVouchMst)
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

            command.CommandText = @"INSERT INTO [ConsumptionManualyMst]
           ([ConsumptionDate]
           ,[Remarks]
           ,[AddBy]
           ,[AddDate])
     VALUES
           (Convert(date,'" + aConsumptionManualyInfo.ConsumtionDate + "',103),'" + aConsumptionManualyInfo.Remarks + "','" + aConsumptionManualyInfo.LogineBy + "','" + Globals._localTime.ToString() + "')";
            command.ExecuteNonQuery();

            //****************************************
            command.CommandText = @"SELECT top(1) [ID]  FROM [ConsumptionManualyMst] order by ID desc";
            string MstID = command.ExecuteScalar().ToString();

            //***************************  ********************************//           

            foreach (DataRow dr in dtitemDtl.Rows)
            {
                if (dr["item_code"].ToString() != "" && dr["ItemID"].ToString() != "")
                {
                    command.CommandText = @"INSERT INTO [ConsumptionManualyDtl]
           ([MstID]
           ,[ItemID]
           ,[UniteType]
           ,[UnitePrice]
           ,[Quantity]
           ,[AddBy]
           ,[AddDate],[TotalCost])
     VALUES
           ('" + MstID + "','" + dr["ItemID"].ToString() + "','" + dr["msr_unit_code"].ToString() + "','" + dr["item_rate"].ToString() + "','" + dr["qnty"].ToString() + "','" + aConsumptionManualyInfo.LogineBy + "','" + Globals._localTime.ToString() + "','" + Convert.ToDouble(dr["item_rate"].ToString()) * Convert.ToDouble(dr["qnty"].ToString()) + "')";
                    command.ExecuteNonQuery();                


                }
               
            }
            //********************* Sales Total *********//

            command.CommandText = "SP_PV_UnitPrice_All";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@MstID", Convert.ToInt32(MstID));
            command.Parameters.AddWithValue("@option", 2);
            decimal ProductionPrice = Convert.ToDecimal(command.ExecuteScalar());

            aVouchMst.ControlAmt = ProductionPrice.ToString().Replace("'", "");
            aVouchMst.SerialNo = MstID;
            command.CommandText = VouchManager.SaveVoucherMst(aVouchMst, 1);
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();
            VouchDtl vdtl;
            vdtl = new VouchDtl();
            vdtl.VchSysNo = aVouchMst.VchSysNo;
            vdtl.ValueDate = aConsumptionManualyInfo.ConsumtionDate;
            vdtl.LineNo = "3";
            vdtl.GlCoaCode = dtFixCode.Rows[0]["ClosingStock"].ToString(); ;
            vdtl.Particulars = "Closing Stock";
            vdtl.AccType = VouchManager.getAccType(vdtl.GlCoaCode);
            vdtl.AmountDr = "0";
            vdtl.AmountCr = aVouchMst.ControlAmt.Replace(",", "");
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


    public static void UpdateConsumptionManualy(ConsumptionManualyInfo aConsumptionManualyInfo, DataTable dtitemDtl, DataTable OldStock)
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

            command.CommandText = @"UPDATE [ConsumptionManualyMst]
   SET [ConsumptionDate] =Convert(date,'" + aConsumptionManualyInfo.ConsumtionDate + "',103) ,[Remarks] ='" + aConsumptionManualyInfo.Remarks + "'  ,[UpdateBy] ='" + aConsumptionManualyInfo.LogineBy + "'   ,[UpdateDate] = '" + Globals._localTime.ToString() + "'  where ID='" + aConsumptionManualyInfo.MstID + "' ";
            command.ExecuteNonQuery();

            //****************************************
            command.CommandText = @"DELETE FROM [ConsumptionManualyDtl]   WHERE MstID='"+aConsumptionManualyInfo.MstID+"'";
            command.ExecuteNonQuery();

            foreach (DataRow dr in dtitemDtl.Rows)
            {
                if (dr["item_code"].ToString() != "" && dr["ItemID"].ToString() != "")
                {
                    command.CommandText = @"INSERT INTO [ConsumptionManualyDtl]
           ([MstID]
           ,[ItemID]
           ,[UniteType]
           ,[UnitePrice]
           ,[Quantity]
           ,[AddBy]
           ,[AddDate],[TotalCost])
     VALUES
           ('" + aConsumptionManualyInfo.MstID + "','" + dr["ItemID"].ToString() + "','" + dr["msr_unit_code"].ToString() + "','" + dr["item_rate"].ToString() + "','" + dr["qnty"].ToString() + "','" + aConsumptionManualyInfo.LogineBy + "','" + Globals._localTime.ToString() + "','" + Convert.ToDouble(dr["item_rate"].ToString()) * Convert.ToDouble(dr["qnty"].ToString()) + "')";
                    command.ExecuteNonQuery();
                
                
                }
            }
            //************************************************
            //********************* Sales Total *********//

            command.CommandText = "SP_PV_UnitPrice_All";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@MstID", Convert.ToInt32(aConsumptionManualyInfo.MstID));
            command.Parameters.AddWithValue("@option", 2);
            decimal ProductionPrice = Convert.ToDecimal(command.ExecuteScalar());

           

            
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

    public static string GetMstAutoID()
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);

        string query = @"Select Top(1) ID from ConsumptionManualyMst order by ID desc";

        object GetMstID = DataManager.ExecuteScalar(connectionString, query);

        string ConsumtionNo = (Convert.ToDecimal(GetMstID)+1).ToString();
        return ConsumtionNo;
       
    }

    public static DataTable GetConsumtionMAnualyMstInfo(string MstID)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string per = "";
        if(!string.IsNullOrEmpty(MstID))
        {
            per=" And ID='"+MstID+"' ";
        }
        string query = @"SELECT [ID]
      ,convert(nvarchar,[ConsumptionDate],103) as ConsumptionDate
      ,[Remarks]     
  FROM [ConsumptionManualyMst] where DeleteBy is null "+per+" order by ID Desc";

        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ConsumptionManualyMst");
        return dt;
    }
     
    public static DataTable GetItemDtls(string MstID)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);

        string query = @"SELECT t2.ID as DtlsID ,t2.Quantity as qnty,t2.Quantity as Oldqnty,t3.ID as ItemID, t3.StyleNo as item_code,t3.Name as item_desc,t4.ID as msr_unit_code,t3.UnitPrice as item_rate, t5.BrandName as BrandName 
  FROM [ConsumptionManualyMst] t1
  inner join ConsumptionManualyDtl t2  on t1.ID=t2.MstID  left join Item t3 on t2.ItemID=t3.ID left join UOM t4 on t4.ID=t2.UniteType left join Brand t5 on t5.ID=t3.Brand where t1.DeleteBy is null and t2.DeleteBy is null and t1.ID='" + MstID + "' ";

        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ConsumptionManualyMst");
        return dt;
    }

    public static object GetClosingStock(string ItemID)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = @"select ClosingStock from Item where  ID='" + ItemID + "'";

        object dt = DataManager.ExecuteScalar(connectionString, query);
        return dt;
    }
   
    public static void DeleteConsumptionManualyInfo(ConsumptionManualyInfo aConsumptionManualyInfo, string MstID)
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

            command.CommandText = @"UPDATE [ConsumptionManualyMst] SET [DeleteBy]='" + aConsumptionManualyInfo.LogineBy + "',[DeleteDate]='" + Globals._localTime.ToString() + "' where [ID]='" + MstID + "'";

            command.ExecuteNonQuery();

            command.CommandText = @"UPDATE [ConsumptionManualyDtl] SET [DeleteBy]='" + aConsumptionManualyInfo.LogineBy + "',[DeleteDate]='" + Globals._localTime.ToString() + "' where [MstID]='" + MstID + "'";

            command.ExecuteNonQuery();


            transection.Commit();
            connection.Close();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public static DataTable GetConsumptionManualyInfo(string MstID)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);

        string query = @"SELECT t2.ID as DtlsID,Convert(nvarchar,t1.ConsumptionDate,103) as ConsumptionDate ,t2.Quantity as qnty,t3.ID as ItemID,t4.Name as UOM, t3.StyleNo as item_code,t3.Name as item_desc,t4.ID as msr_unit_code,t3.UnitPrice as item_rate, t5.BrandName as BrandName 
  FROM [ConsumptionManualyMst] t1
  inner join ConsumptionManualyDtl t2  on t1.ID=t2.MstID  left join Item t3 on t2.ItemID=t3.ID left join UOM t4 on t4.ID=t2.UniteType left join Brand t5 on t5.ID=t3.Brand where t1.DeleteBy is null and t2.DeleteBy is null   and t1.ID='" + MstID + "' ";

        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ConsumptionManualyMst");
        return dt;
    }
}