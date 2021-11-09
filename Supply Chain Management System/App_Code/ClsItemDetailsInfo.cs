using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for ClsItemDetailsInfo
/// </summary>
public class ClsItemDetailsInfo
{
    public string ItemsCode;
    public string ItemsName;
    public string Umo;
    public string UnitPrice;
    public string Currency;
    public string OpeningStock;
    public string OpeningAmount;
    public string ClosingStock;
    public string ClosingAmount;
    public string Catagory;
    public string SubCatagory;
    public string Text;
    public string Discount;
    public Byte[] ItemsImage;
    public bool Active;
    public bool DiscountCheck;
    public string Description;
    public string ItemsType, Brand, ShortName, StyleNo, SecurityCode, BranchSalesPrice;
    public ClsItemDetailsInfo()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public ClsItemDetailsInfo(DataRow dr)
    {

        if (dr["BranchSalesPrice"].ToString() != string.Empty)
        {
            this.BranchSalesPrice = dr["BranchSalesPrice"].ToString();
        }
        if (dr["ItemsType"].ToString() != string.Empty)
        {
            this.ItemsType = dr["ItemsType"].ToString();
        }
        if (dr["StyleNo"].ToString() != string.Empty)
        {
            this.StyleNo = dr["StyleNo"].ToString();
        }
        if (dr["SecurityCode"].ToString() != string.Empty)
        {
            this.SecurityCode = dr["SecurityCode"].ToString();
        }
        if (dr["Code"].ToString() != string.Empty)
        {
            this.ItemsCode = dr["Code"].ToString();
        }
        if (dr["ShortName"].ToString() != string.Empty)
        {
            this.ShortName = dr["ShortName"].ToString();
        }
        if (dr["Brand"].ToString() != string.Empty)
        {
            this.Brand = dr["Brand"].ToString();
        }
        if (dr["Name"].ToString() != string.Empty)
        {
            this.ItemsName = dr["Name"].ToString();
        }
        if (dr["UOMID"].ToString() != string.Empty)
        {
            this.Umo = dr["UOMID"].ToString();
        }
        if (dr["UnitPrice"].ToString() != string.Empty)
        {
            this.UnitPrice = dr["UnitPrice"].ToString();
        }
        if (dr["Currency"].ToString() != string.Empty)
        {
            this.Currency = dr["Currency"].ToString();
        }
        if (dr["OpeningStock"].ToString() != string.Empty)
        {
            this.OpeningStock = dr["OpeningStock"].ToString();
        }
        if (dr["OpeningAmount"].ToString() != string.Empty)
        {
            this.OpeningAmount = dr["OpeningAmount"].ToString();
        }
        if (dr["ClosingStock"].ToString() != string.Empty)
        {
            this.ClosingStock = dr["ClosingStock"].ToString();
        }
        if (dr["ClosingAmount"].ToString() != string.Empty)
        {
            this.ClosingAmount = dr["ClosingAmount"].ToString();
        }
        if (dr["CategoryID"].ToString() != string.Empty)
        {
            this.Catagory = dr["CategoryID"].ToString();
        }
        if (dr["SubCategoryID"].ToString() != string.Empty)
        {
            this.SubCatagory = dr["SubCategoryID"].ToString();
        }
        if (dr["TaxCategoryID"].ToString() != string.Empty)
        {
            this.Text = dr["TaxCategoryID"].ToString();
        }
        if (dr["DiscountAmount"].ToString() != string.Empty)
        {
            this.Discount = dr["DiscountAmount"].ToString();
        }
        if (dr["ItemImage"].ToString() != string.Empty)
        {
            this.ItemsImage = (byte[])dr["ItemImage"];
        }
        if (dr["Active"].ToString() != string.Empty)
        {
            this.Active = Convert.ToBoolean(dr["Active"]);
        }
        if (dr["Discounted"].ToString() != string.Empty)
        {
            this.DiscountCheck = Convert.ToBoolean(dr["Discounted"]);
        }
        if (dr["description"].ToString() != string.Empty)
        {
            this.Description = dr["description"].ToString();
        }       
    }
    public string ItemId { get; set; }

    public string ItemName { get; set; }   

    public string UnitId { get; set; }

    public string Quantity { get; set; }

    public string Price { get; set; }

    public string LoginBy { get; set; }

    public string ID { get; set; }
}


    