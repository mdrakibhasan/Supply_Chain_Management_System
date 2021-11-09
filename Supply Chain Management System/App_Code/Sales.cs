using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for Sales
/// </summary>
public class Sales
{
    public string ID, Invoice, Company, Total, Tax, Disount, GTotal, CReceive, CRefund, Date, PMethod, PMNumber, Customer, OrderStatus, Due, DvStatus, DvDate, Remarks, BankId, ChequeDate, ChequeAmount, Chk_Status;
	public Sales()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public Sales(DataRow dr)
    {
        if (dr["ID"].ToString() != string.Empty){this.ID = dr["ID"].ToString();}
        if (dr["Chk_Status"].ToString() != string.Empty) { this.Chk_Status = dr["Chk_Status"].ToString(); }
        if (dr["CompanyId"].ToString() != string.Empty) { this.Company = dr["CompanyId"].ToString(); }
        if (dr["InvoiceNo"].ToString() != string.Empty) { this.Invoice = dr["InvoiceNo"].ToString(); }
        if (dr["SubTotal"].ToString() != string.Empty) { this.Total = dr["SubTotal"].ToString(); }
        if (dr["TaxAmount"].ToString() != string.Empty) { this.Tax = dr["TaxAmount"].ToString(); }
        if (dr["DiscountAmount"].ToString() != string.Empty) { this.Disount = dr["DiscountAmount"].ToString(); }
        if (dr["GrandTotal"].ToString() != string.Empty) { this.GTotal = dr["GrandTotal"].ToString(); }
        if (dr["CashReceived"].ToString() != string.Empty) { this.CReceive = dr["CashReceived"].ToString(); }
        if (dr["CashRefund"].ToString() != string.Empty) { this.CRefund = dr["CashRefund"].ToString(); }
        if (dr["OrderDate"].ToString() != string.Empty) { this.Date = dr["OrderDate"].ToString(); }
        if (dr["PaymentMethodID"].ToString() != string.Empty) { this.PMethod = dr["PaymentMethodID"].ToString(); }
        if (dr["PaymentMethodNumber"].ToString() != string.Empty) { this.PMNumber = dr["PaymentMethodNumber"].ToString(); }
        if (dr["BankId"].ToString() != string.Empty) { this.BankId = dr["BankId"].ToString(); }
        if (dr["ChequeDate"].ToString() != string.Empty) { this.ChequeDate = dr["ChequeDate"].ToString(); }
        if (dr["ChequeAmount"].ToString() != string.Empty) { this.ChequeAmount = dr["ChequeAmount"].ToString(); }
        if (dr["CustomerID"].ToString() != string.Empty) { this.Customer = dr["CustomerID"].ToString(); }
        if (dr["OrderStatusID"].ToString() != string.Empty) { this.OrderStatus = dr["OrderStatusID"].ToString(); }
        if (dr["Due"].ToString() != string.Empty) { this.Due = dr["Due"].ToString(); }
        if (dr["DeliveryStatus"].ToString() != string.Empty) { this.DvStatus = dr["DeliveryStatus"].ToString(); }
        if (dr["DeliveryDate"].ToString() != string.Empty) { this.DvDate = dr["DeliveryDate"].ToString(); }
        if (dr["Remark"].ToString() != string.Empty) { this.Remarks = dr["Remark"].ToString(); }     
    }

    public string LoginBy { get; set; }
}