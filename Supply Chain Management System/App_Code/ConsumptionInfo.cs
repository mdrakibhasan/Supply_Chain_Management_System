using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ConsumptionInfo
/// </summary>
public class ConsumptionInfo
{
	public ConsumptionInfo()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public string DeclareDate { get; set; }

    public string FGItemIdMst { get; set; }

    public string StatusMst { get; set; }

    public string LogineBy { get; set; }

    public string TotalCost { get; set; }

    public string FGQuantity { get; set; }

    public int ConsumptionNo { get; set; }

    public string UnitPriceDtl { get; set; }
}