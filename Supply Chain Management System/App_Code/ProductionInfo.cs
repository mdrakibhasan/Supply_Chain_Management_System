using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for ProductionInfo
/// </summary>
public class ProductionInfo
{
	public ProductionInfo()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public ProductionInfo(DataRow dr)
    {
        if (dr["ID"].ToString() != string.Empty) { this.ID = dr["ID"].ToString(); }
        if (dr["PN"].ToString() != string.Empty) { this.GoodsReceiveNo = dr["PN"].ToString(); }
        if (dr["ReceiveDate"].ToString() != string.Empty) { this.ReceiveDate = dr["ReceiveDate"].ToString(); }
        if (dr["Remarks"].ToString() != string.Empty) { this.Remarks = dr["Remarks"].ToString(); }
        if (dr["BatchNo"].ToString() != string.Empty) { this.BatchNo = dr["BatchNo"].ToString(); }
        
     
    }

    public string ID { get; set; }

    public string GoodsReceiveNo { get; set; }

    public string Remarks { get; set; }

    public string ShiftmentID { get; set; }

    public string LoginBy { get; set; }

    public string ReceiveDate { get; set; }

    public string BatchNo { get; set; }
}