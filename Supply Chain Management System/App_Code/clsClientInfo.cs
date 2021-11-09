using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;


public class clsClientInfo
{
    public string Code, CustomerName, NationalId, Address1, Address2, Phone, Mobile, Fax, Email, Status, ID, PostalCode, Country, Active, GlCoa, CommonCus;

    public clsClientInfo()
    {
        
    }
    public clsClientInfo(DataRow dr)
    {
        if (dr["ID"].ToString() != string.Empty) { this.ID = dr["ID"].ToString();}
        if (dr["Code"].ToString() != string.Empty) { this.Code = dr["Code"].ToString(); }
        if (dr["ContactName"].ToString() != string.Empty) { this.CustomerName = dr["ContactName"].ToString(); }
        if (dr["Address1"].ToString() != string.Empty) { this.Address1 = dr["Address1"].ToString(); }
        if (dr["Address2"].ToString() != string.Empty) { this.Address2 = dr["Address2"].ToString(); }
        if (dr["Email"].ToString() != string.Empty) { this.Email = dr["Email"].ToString(); }
        if (dr["Mobile"].ToString() != string.Empty) { this.Mobile = dr["Mobile"].ToString(); }
        if (dr["Phone"].ToString() != string.Empty) { this.Phone = dr["Phone"].ToString(); }
        if (dr["City"].ToString() != string.Empty) { this.NationalId = dr["City"].ToString(); }
        if (dr["PostalCode"].ToString() != string.Empty) { this.PostalCode = dr["PostalCode"].ToString(); }
        if (dr["Country"].ToString() != string.Empty) { this.Country = dr["Country"].ToString(); }
        if (dr["Active"].ToString() != string.Empty) { this.Active = dr["Active"].ToString(); }
        if (dr["Gl_CoaCode"].ToString() != string.Empty) { this.GlCoa = dr["Gl_CoaCode"].ToString(); }
        if (dr["Fax"].ToString() != string.Empty) { this.Fax = dr["Fax"].ToString(); }
        if (dr["CommonCus"].ToString() != string.Empty) { this.CommonCus = dr["CommonCus"].ToString(); }
    }

    public string LoginBy { get; set; }

    public string GlCoaDesc { get; set; }
}
