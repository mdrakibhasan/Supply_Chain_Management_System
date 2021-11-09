﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmItemsDetails.aspx.cs" Inherits="frmItemsDetails" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Select Items</title>
<script language="javascript" type="text/javascript">
    function SubmitToParent(item) {
        window.opener.setValue(item);
        window.opener.document.forms[0].submit();
        window.close();
        return false;
    }
    function OnClose() 
    {
        if (window.opener != null && !window.opener.closed)
        {
                    window.opener.HideModalDiv();
        }

    }
    function OnOpen()
    {
        window.opener.LoadModalDiv();
    }
    window.onbeforeunload = OnClose;
    window.onload = OnOpen;

    function text_changed() {
       // alert("HMM");
        var txtSearch = document.getElementById("<%=txtSearch.ClientID %>");
        //alert(txtSearch.value);
        $.ajax({
            type: "POST",
            async: false,
            cache: false,
            contentType: "application/json; charset=utf-8",
            url: "AutoComplete.asmx/getAllItems",
            data: '{"pat": " "}',
            dataType: "json",
            success: function (val) {
                var pdata = JSON.parse(val.d);               
            },
            error: function (xhr, status, error) {
                alert('error: ' + xhr.statusText);
            }
        });
    }

</script>
    <style type="text/css">
        .style1
        {
            width: 100%;
        }
        .style2
        {
            width: 176px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
<div>

    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>

</div>
    <div>
    <fieldset style="vertical-align: top; border: solid 1px #8BB381;text-align:left;"><legend style="color: maroon;"><b>Search Items</b> </legend>
        <table class="style1">
            <tr>
                <td class="style2">
                    <asp:Label ID="Label1" runat="server" Text="Search By Code/Name"></asp:Label>
                </td>
                <td>
                    <%-- onkeyup="javascript:text_changed();"--%>
                    <asp:TextBox ID="txtSearch" runat="server"  placeholder="Search Item Code OR Name"
                                 onfocus="this.select();"  Width="50%" 
                        ontextchanged="txtSearch_TextChanged" AutoPostBack="True"></asp:TextBox>
              <ajaxToolkit:AutoCompleteExtender ServicePath="AutoComplete.asmx" runat="server" 
                                     ID="autoComplete" TargetControlID="txtSearch"
           ServiceMethod="GetShowItems" MinimumPrefixLength="1" CompletionInterval="1000" EnableCaching="true" 
                                     CompletionSetCount="12"/>
                    <asp:Button ID="btnClear" runat="server" onclick="btnClear_Click" 
                        Text="Clear" />
                </td>
            </tr>
        </table></fieldset>
    </div>
    <div>
    <br />
        <asp:GridView ID="dgItems" runat="server" AllowPaging="True" 
            AllowSorting="True" AlternatingRowStyle-CssClass="alt" 
            AutoGenerateColumns="False" BackColor="White" BorderColor="LightGray" 
            BorderStyle="Solid" BorderWidth="1px" CellPadding="2" CssClass="mGrid" 
            Font-Size="8pt" onpageindexchanging="dgItems_PageIndexChanging" 
            onrowdatabound="dgItems_RowDataBound" 
            onselectedindexchanged="dgItems_SelectedIndexChanged" PagerStyle-CssClass="pgr" 
            PageSize="30">
            <HeaderStyle BackColor="LightGray" Font-Bold="True" Font-Names="Arial" 
                Font-Size="9" ForeColor="Black" HorizontalAlign="center" />
            <Columns>
                <asp:CommandField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="40px" 
                    ShowSelectButton="True">
<ItemStyle HorizontalAlign="Center" Width="40px"></ItemStyle>
                </asp:CommandField>
                <asp:BoundField DataField="ID" HeaderText="ID" 
                    ItemStyle-HorizontalAlign="Center" ItemStyle-Width="100px" />
                <asp:BoundField DataField="Code" HeaderText="Code" 
                    ItemStyle-HorizontalAlign="Center" ItemStyle-Width="100px" />
                <asp:BoundField DataField="Name" HeaderText="Item Name" ItemStyle-Height="20" 
                    ItemStyle-HorizontalAlign="Left" ItemStyle-Width="300px" />
                <asp:BoundField DataField="UnitPrice" HeaderText="Unit Price" 
                    ItemStyle-Height="20" ItemStyle-HorizontalAlign="Left" 
                    ItemStyle-Width="100px" />
                <asp:BoundField DataField="ClosingStock" HeaderText="C.Stock" 
                    ItemStyle-Height="20" ItemStyle-HorizontalAlign="Left" 
                    ItemStyle-Width="100px" />
                <asp:BoundField DataField="ClosingAmount" HeaderText="C.Amount" />
            </Columns>
            <RowStyle BackColor="White" />
            <SelectedRowStyle BackColor="" Font-Bold="True" />
            <PagerStyle BackColor="LightGray" ForeColor="Black" HorizontalAlign="Center" />
            <AlternatingRowStyle BackColor="" />
        </asp:GridView>
    
    </div>
    </form>
</body>
</html>
