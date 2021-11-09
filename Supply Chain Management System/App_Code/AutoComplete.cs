using System;
using System.Collections.Generic;
using System.Web.Services;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using autouniv;


[WebService(Namespace = "http://shofthousebd.com/webservices")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.Web.Script.Services.ScriptService]

public class AutoComplete : WebService
{
    public AutoComplete()
    {

    }
    //********************* ItemPurOrderMst PO No ********//
    [WebMethod]
    public string[] GetPONoForSearch(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetGetPONoForSearch(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable GetGetPONoForSearch(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = "SELECT t1.[PO] FROM [ItemPurOrderMst] t1  where upper(t1.[PO]) like upper('%" + strName + "%') ";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }

    //********************* ItemPurOrderMst PO No ********//
    [WebMethod]
    public string[] GEtStyleNo(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetGEtStyleNo(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable GetGEtStyleNo(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = "SELECT Code FROM Item t1  where ItemsType!=2 and upper(Code) like upper('%" + strName + "%') ";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }
    //*********************************************
    [WebMethod(EnableSession = true)]
    public string[] GetSupplierForSearch(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GeGetSupplierForSearch(prefixText, Session["user"].ToString());
        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            string str = dt.Rows[i][0].ToString();
            items.Add(str);
        }
        return items.ToArray();
    }

    public DataTable GeGetSupplierForSearch(string strName, string User)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = "";
        DataTable dt = null;
        Users usr = UsersManager.getUser(User.ToUpper());
        if (usr != null)
        {

            query =
                @"select Code+'-'+ContactName+'-'+Mobile from supplier where DeleteBy is null and   UPPER(Code+'-'+ContactName+'-'+Mobile) like upper('%" +
                strName + "%')  ";
            dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        }
        return dt;
    }

    //*********************************************
    [WebMethod(EnableSession = true)]
    public string[] GetInvoice(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GeGetInvoice(prefixText);
        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            string str = dt.Rows[i][0].ToString();
            items.Add(str);
        }
        return items.ToArray();
    }

    public DataTable GeGetInvoice(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = "";
        DataTable dt = null;        
       

            query =
                @"Select InvoiceNo from [order] where id not in (Select InvoiceNo from OrderReturn) and  UPPER(InvoiceNo) like upper('%" +
                strName + "%')  ";
            dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        
        return dt;
    }


    //**************************** Get Gl_Goa_Code *************///

    [WebMethod]
    public string[] GetSearchGlCoa(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetGlCoa(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable GetGlCoa(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = @"SELECT [SEG_COA_CODE]+' - '+[SEG_COA_DESC] FROM [GL_SEG_COA] where ROOTLEAF='L' and upper([SEG_COA_CODE]+' - '+[SEG_COA_DESC]) like upper('%" + strName + "%') ";
        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }
    //**************************** GEt GetGRCItem Item*************
    [WebMethod]
    public string[] GetGRCItem(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetGetGRCItem(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }

    public DataTable GetGetGRCItem(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = @"Select ItemName from View_ItemSearch where Upper(ItemName) like UPPER('%" + strName + "%') and ItemsType in(1,2,3)";
        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }
    //********************* ********//
    //****************************  GetDamagedItem Item*************
    [WebMethod]
    public string[] GetDamagedItem(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetGetDamagedItem(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }

    public DataTable GetGetDamagedItem(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = @"Select ItemName from View_ItemSearch where Upper(ItemName) like UPPER('%" + strName + "%') and ItemsType in(4)";
        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }
  //************************************ Get Item Code ********************************
    [WebMethod]
    public string[] GetGDandCmnItemCode(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GDandCmnItemCode(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable GDandCmnItemCode(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);

        string query = "Select Code from View_ItemSearch where Upper(Code) like UPPER('%" + strName + "%') and ItemsType in(2,3)";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }
    //************************************ Get Brand ********************************
         [WebMethod]
    public string[] GetBrandName(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GrandName(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
         public DataTable GrandName(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = "select BrandName from Brand where upper(BrandName) like UPPER('%" + strName + "%') ";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }
    //********************* ItemPurOrderMst List ********//
    [WebMethod]
    public string[] GetShowPONo(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetAllPOno(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable GetAllPOno(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = "SELECT  [PUrOrderNo] FROM [View_PUrOrderNO] where UPPER([PUrOrderNo]) like UPPER ('%" + strName + "%') ";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }
    //*********************************
    //**************************** GEt All Item*************
    [WebMethod]
    public string[] GetAllItem(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetGetAllItem(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }

    public DataTable GetGetAllItem(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = @"Select ItemName from View_ItemSearch where Upper(ItemName) like UPPER('%" + strName + "%') and ItemsType is not null";
        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }
    //********************* ********//
    //********************* Return GRN Number ********//
    [WebMethod]
    public string[] GetGRN(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetGetGRNSup(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable GetGetGRNSup(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = "SELECT [GRN] FROM [ItemPurchaseMst] WHERE UPPER([GRN]) like upper('%" + strName + "%') Order By GRN DESC ";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }
    //**************************** GEt Finish Good Item*************
    [WebMethod]
    public string[] GetFGAndCmnItem(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetGetFGAndCmnItem(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable GetGetFGAndCmnItem(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = @"Select ItemName from View_ItemSearch where Upper(ItemName) like UPPER('%" + strName + "%') and ItemsType in(2,3)";
        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }

    //**************************** GEt Raw Material Item*************
    [WebMethod]
    public string[] GetRMAndCmnItem(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetGetRMAndCmnItem(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }

    public DataTable GetGetRMAndCmnItem(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = @"Select ItemName from View_ItemSearch where Upper(ItemName) like UPPER('%" + strName + "%') and ItemsType in(1,3)";
        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }
    //********************* Return GRN Number ********//
    [WebMethod]
    public string[] GetProductNO(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetProductPN(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable GetProductPN(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = "SELECT [PN] FROM [ProductionMst] WHERE UPPER([PN]) like upper('%" + strName + "%') Order By PN DESC ";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }
    //*********

    [WebMethod]
    public string[] GetTransferItem(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetGetTransferItemList(prefixText);
        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            string str = dt.Rows[i][0].ToString();
            items.Add(str);
        }
        return items.ToArray();
    }

    public DataTable GetGetTransferItemList(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = @"SELECT     t1.Name + CASE WHEN (t1.StyleNo IS NULL OR
                      t1.StyleNo = '') THEN '' ELSE ' - ' + t1.StyleNo END + '-' + CONVERT(nvarchar, t1.UnitPrice) + CASE WHEN t3.BrandName IS NULL THEN '' ELSE ' - ' + t3.BrandName END AS ItemName, t1.ItemsType, 
                      t1.Code, t1.ClosingStock
FROM         dbo.Item AS t1 LEFT OUTER JOIN
                      dbo.UOM AS t2 ON t2.ID = t1.UOMID LEFT OUTER JOIN
                      dbo.Brand AS t3 ON t3.ID = t1.Brand
WHERE     (t1.Active = 1) and  Upper( t1.Name + CASE WHEN (t1.StyleNo IS NULL OR
                      t1.StyleNo = '') THEN '' ELSE ' - ' + t1.StyleNo END + '-' + CONVERT(nvarchar, t1.UnitPrice) + CASE WHEN t3.BrandName IS NULL THEN '' ELSE ' - ' + t3.BrandName END) like UPPER('%" + strName + "%') and t1.ItemsType in(2,3) and  t1.ClosingStock>0";
        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");

        return dt;
    }

    //*********************************** All Common And Finnish Good Iten  *************************
    [WebMethod]
    public string[] GetFGAndCMNItems(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetFGAndCMNItemList(prefixText);
        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            string str = dt.Rows[i][0].ToString();
            items.Add(str);
        }
        return items.ToArray();
    }

    public DataTable GetFGAndCMNItemList(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = @"Select ItemName from View_ItemSearch where Upper(ItemName) like UPPER('%" + strName + "%') and ItemsType in(2,3)";
          DataTable  dt = DataManager.ExecuteQuery(strConn, query, "autoname");
      
        return dt;
    }

    //********************* Items List Barcode ********//
    [WebMethod(EnableSession = true)]
    public string[] GetItemListBarcode(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetAllItemsBarcode(prefixText, Session["user"].ToString());
        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            string str = dt.Rows[i][0].ToString();
            items.Add(str);
        }
        return items.ToArray();
    }

    public DataTable GetAllItemsBarcode(string strName, string User)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = "";
        DataTable dt = null;
        Users usr = UsersManager.getUser(User.ToUpper());
        if (usr != null)
        {

            query = "Select ItemName from View_ItemSearch where Upper(ItemName) like UPPER('%" + strName + "%') and ItemsType in(1,3)";
                //@"select ISNULL(t1.Code,'')+ ' - '+ISNULL(t1.Name,'')+' - '+ISNULL(t2.BrandName,'')+' - '+convert(nvarchar,t1.UnitPrice) from Item t1 inner join Brand t2 on t1.Brand=t2.ID where upper(ISNULL(t1.Code,'')+ ' - '+ISNULL(t1.Name,'')+' - '+ISNULL(t2.BrandName,'')+' - '+convert(nvarchar,t1.UnitPrice)) like upper('%" +
                //strName + "%') AND t1.DeleteBy IS NULL and t1.ItemsType in(1,3) ";
            dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        }
        return dt;
    }
      [WebMethod(EnableSession = true)]
    public string[] GetItemList(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetAllItems(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable GetAllItems(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = "select Code+ ' - '+Name from Item where upper(Code+ ' - '+Name) like upper('%" + strName + "%') ";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }

    //********************* ItemPurOrderMst List ********//
    [WebMethod]
    public string[] GetShowItems(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetItemsList(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable GetItemsList(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = "SELECT t.Code+' - '+t.Name+' - '+Isnull(t1.BrandName,'')+' - '+Isnull(t2.Name,'') FROM [Item] t left join Brand t1 on t.Brand=t1.ID left join Category t2 on t2.ID=t.CategoryID where  t.ClosingStock>0 and upper(t.Code+' - '+t.Name+' - '+Isnull(t1.BrandName,'')+' - '+Isnull(t2.Name,'')) like upper('%" + strName + "%') ";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }
    //*********************************

    //********************************* Gl COA CODE *********************//
    [WebMethod(EnableSession = true)]
    public string[] GetCompletionList(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        if (Session["user"].ToString() != null)
        {
            DataTable dt = GetItems(prefixText);

            List<string> items = new List<string>(count);
            for (int i = 0; i < dt.Rows.Count; i++)
            {

                string str = dt.Rows[i][0].ToString();

                items.Add(str);
            }

            return items.ToArray();
        }
        else
            return null;
    }
    public DataTable GetItems(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = "select coa_desc from gl_coa where upper(coa_desc) like upper('%" + strName + "%') ";
        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    } 
}
