using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using autouniv;
using System.Data;

/// <summary>
/// Summary description for clsBercodeHistoryManager
/// </summary>
public class clsBercodeHistoryManager
{
    SqlConnection connection = new SqlConnection(DataManager.OraConnString());
    SqlTransaction transection;
	public clsBercodeHistoryManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public void SaveBercodeHistoryInfo(List<clsBercodeHistory> ItemPrintList)
    {
        try
        {
            connection.Open();
            transection = connection.BeginTransaction();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transection;

            foreach (clsBercodeHistory aBercodeHistoryInfo in ItemPrintList)
            {
                command.CommandText = @"INSERT INTO [BarcodeGenerateHistory]
                       ([ItemID]
                        ,[ColorID]
                        ,[SizeID]
                        ,[ItemRate]
                        ,[Quantity]
                        ,[AddBy]
                        ,[AddDate])
                        VALUES
                    ('" + aBercodeHistoryInfo.ItemId + "','" + aBercodeHistoryInfo.ColorId + "','" + aBercodeHistoryInfo.SizeId + "','" + aBercodeHistoryInfo.ItemRate + "','" + aBercodeHistoryInfo.Quantity + "','" + aBercodeHistoryInfo.LoginBy + "','" + Globals._localTime.ToString() + "')";
                command.ExecuteNonQuery();
            }
            transection.Commit();
        }
        catch (Exception ex)
        {
            transection.Rollback();
            throw new Exception(ex.Message);
        }
        finally
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }
    }
}