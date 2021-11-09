<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="frmSupplier.aspx.cs" Inherits="frmSupplier" Title="Supplier Setup" Theme="Themes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">

<div style="background-color:White; width:100%; min-height:700px; height:auto !important; height:700px; font-family:Tahoma;">
<div style="vertical-align:top;">
<table  id="pageFooterWrapper">
  <tr>  
        <td style="width:5%;"></td>
        <td align="center">
        <asp:Button ID="Delete" runat="server" ToolTip="Delete" onclick="btnDelete_Click"
            
                
                onclientclick="javascript:return window.confirm('are u really want to delete these data')" Text="Delete" 
        Height="35px" Width="120px" BorderStyle="Outset"  />
        </td>
        <td style="width:20px;"></td>
        <td align="center" >
        <asp:Button ID="btnSave" runat="server" OnClientClick="this.disabled=true;" UseSubmitBehavior="false" ToolTip="Save Supplier Record" 
                onclick="btnSave_Click" Text="Save" 
        Height="35px" Width="120px" BorderStyle="Outset"  />
        </td>             
        <td style="width:20px;"></td>
        <td align="center" >
        <asp:Button ID="Clear" runat="server"  ToolTip="Clear" onclick="btnClear_Click" Text="Clear" 
        Height="35px" Width="120px" BorderStyle="Outset"  />
        </td>
        <td style="width:5%;">&nbsp;</td>
   </tr>
   </table>
</div>
<table style="width:100%;"><tr>
<td style="width:1%;"></td>
<td style="width:98%;" align="center">
<table style="width:100%;">
<tr>
<td style="width:100%;" align="center"> 
<br />
<fieldset style="vertical-align: top; border: solid 1px #8BB381;text-align:left;"><legend style="color: maroon;"><b>Supplier Information </b></legend>
<table style="width:100%;">
<tr>
<td style="width:12%;" align="left">
<asp:Label ID="lblSupCode" runat="server" Font-Size="9pt">Supplier Code</asp:Label>
</td>
<td style="width:20%;" align="left"> 
<asp:TextBox SkinID="tbPlain" ID="txtSupCode" runat="server"  Width="80%" 
        Font-Size="9pt"  CssClass="tbc" Enabled="False"></asp:TextBox>
</td>
<td style="height: 27px; width:5%;" align="left" >
    <asp:CheckBox ID="CheckBox1" Checked="true" runat="server" Text="Active?" />
    </td>
<td style="width:11%;" align="left">
    &nbsp;</td>
<td  style="width:20%;" align="left"> 
<asp:TextBox SkinID="tbPlain" ID="txtGlCoa" style="text-align:left;" 
        runat="server"  Width="80%" 
        Font-Size="9pt"  CssClass="tbc" Visible="False"></asp:TextBox>
                            </td>
</tr>
<tr>
<td style="width:12%;" align="left">
<asp:Label ID="lblSupName0" runat="server" Font-Size="9pt">Company Name</asp:Label>
</td>
<td style="width:20%;" align="left"> 
<asp:TextBox SkinID="tbPlain" ID="txtCompanyName" runat="server" Width="80%" 
        Font-Size="8pt"  CssClass="tbl"></asp:TextBox>
</td>
<td style="height: 27px; width:5%;" >
<asp:Label ID="lbLID" runat="server" Font-Size="9pt" Visible="False"></asp:Label>
    </td>
<td style="width:11%;" align="left">
<asp:Label ID="lblSupAddr" runat="server" Font-Size="9pt" Width="100%">Address 1</asp:Label>
</td>
<td  style="width:20%;" align="left"> 
<asp:TextBox SkinID="tbPlain" ID="txtAddress1" runat="server"  Width="80%" 
        Font-Size="9pt"  CssClass="tbc"></asp:TextBox>
    </td>
</tr>
<tr>
<td style="width:12%;" align="left">
<asp:Label ID="lblSupName" runat="server" Font-Size="9pt">Supplier Name</asp:Label>
</td>
<td style="width:20%;" align="left"> 
<asp:TextBox SkinID="tbPlain" ID="txtSupplierName" runat="server" Width="80%" 
        Font-Size="8pt"  CssClass="tbl"></asp:TextBox>
</td>
<td style="height: 27px; width:5%;" >&nbsp;</td>
<td style="width:11%;" align="left">
<asp:Label ID="lblSupAddr0" runat="server" Font-Size="9pt" Width="100%">Address 2</asp:Label>
</td>
<td  style="width:20%;" align="left"> 
<asp:TextBox SkinID="tbPlain" ID="txtAddress2" runat="server" Width="80%" 
        Font-Size="8pt"></asp:TextBox>
    </td>
</tr>
<tr>
<td style="width:12%;" align="left">
<asp:Label ID="lblSupAddr1" runat="server" Font-Size="9pt">Designation</asp:Label>
</td>
<td style="width:20%;" align="left"> 
<asp:TextBox SkinID="tbPlain"  ID="txtDesignation" runat="server" style="text-align:left;"  Width="80%"  
        Font-Size="9pt" CssClass="tbc"></asp:TextBox>
</td>
<td style="height: 27px; width:5%;" >&nbsp;</td>
<td style="width:11%;" align="left">
<asp:Label ID="lblSupAddr2" runat="server" Font-Size="9pt" Width="100%">City</asp:Label>
</td>
<td style="width:20%;" align="left" > 
<asp:TextBox SkinID="tbPlain" ID="txtCity" runat="server" Width="80%" 
        Font-Size="8pt"  CssClass="tbl"></asp:TextBox> 
</td>
</tr>

<tr>
<td style="width:12%;" align="left">
<asp:Label ID="lblSupAddr3" runat="server" Font-Size="9pt">Mobile</asp:Label>
</td>
<td style="width:20%;" align="left"> 
<asp:TextBox SkinID="tbPlain" ID="txtMobile" runat="server" style="text-align:left;"  Width="80%" 
        Font-Size="9pt"  CssClass="tbc"></asp:TextBox>
</td>
<td style="height: 27px; width:5%;" >&nbsp;</td>
<td style="width:11%;" align="left">
<asp:Label ID="lblSupAddr4" runat="server" Font-Size="9pt" Width="100%">State</asp:Label>
</td>
<td style="width:20%;" align="left" > 
<asp:TextBox SkinID="tbPlain" ID="txtState" runat="server" Width="80%" 
        Font-Size="8pt"  CssClass="tbl"></asp:TextBox> 
</td>
</tr>

<tr>
<td style="width:12%;" align="left">
<asp:Label ID="lblSupAddr5" runat="server" Font-Size="9pt">Phone</asp:Label>
</td>
<td style="width:20%;" align="left"> 
<asp:TextBox SkinID="tbPlain" ID="txtPhone" runat="server" style="text-align:left;"  Width="80%" 
        Font-Size="9pt" CssClass="tbc"></asp:TextBox>
</td>
<td style="height: 27px; width:5%;" >&nbsp;</td>
<td style="width:11%;" align="left">
<asp:Label ID="lblSupAddr6" runat="server" Font-Size="9pt" Width="100%">Postal Code</asp:Label>
</td>
<td style="width:20%;" align="left" > 
<asp:TextBox SkinID="tbPlain" ID="txtPostalCode" runat="server" Width="80%" 
        Font-Size="8pt" CssClass="tbl"></asp:TextBox> 
</td>
</tr>

<tr>
<td style="width:12%;" align="left">
<asp:Label ID="lblSupAddr7" runat="server" Font-Size="9pt">Fax</asp:Label>
</td>
<td style="width:20%;" align="left"> 
<asp:TextBox SkinID="tbPlain" ID="txtFax" runat="server"  style="text-align:left;" Width="80%" 
        Font-Size="9pt" CssClass="tbc"></asp:TextBox>
</td>
<td style="height: 27px; width:5%;" >&nbsp;</td>
<td style="width:11%;" align="left">
<asp:Label ID="lblSupAddr8" runat="server" Font-Size="9pt" Width="100%">Country</asp:Label>
</td>
<td style="width:20%;" align="left" > 
    <asp:DropDownList ID="ddlCountry" runat="server" Height="26px" Width="84%">
    </asp:DropDownList>
</td>
</tr>

<tr>
<td style="width:12%;" align="left">
<asp:Label ID="lblSupAddr9" runat="server" Font-Size="9pt">Email</asp:Label>
</td>
<td style="width:20%;" align="left"> 
<asp:TextBox SkinID="tbPlain" ID="txtEmail" runat="server" style="text-align:left;"  Width="80%" 
        Font-Size="9pt" CssClass="tbc"></asp:TextBox>
</td>
<td style="height: 27px; width:5%;" >&nbsp;</td>
<td style="width:11%;" align="left">
<asp:Label ID="lblSupAddr10" runat="server" Font-Size="9pt" Width="100%">Supplier Group</asp:Label>
</td>
<td style="width:20%;" align="left" > 
    <asp:DropDownList ID="ddlSupplierGroup" runat="server" Height="26px" 
        Width="84%">
    </asp:DropDownList>
</td>
</tr>

</table>
</fieldset>
<br />
<asp:GridView CssClass="mGrid" PagerStyle-CssClass="pgr"  AlternatingRowStyle-CssClass="alt" 
ID="dgSup" runat="server" AutoGenerateColumns="False" Width="100%" 
        AllowPaging="True" BackColor="White" BorderWidth="1px" BorderStyle="Solid"
        CellPadding="2" BorderColor="LightGray" Font-Size="9pt" 
        AllowSorting="True" PageSize="20" 
        onselectedindexchanged="dgSup_SelectedIndexChanged" ForeColor="#333333" 
        onpageindexchanging="dgSup_PageIndexChanging" 
        onrowdatabound="dgSup_RowDataBound"  >
  <HeaderStyle Font-Size="9pt"  Font-Bold="True" HorizontalAlign="center" BackColor="Silver"/>
  <FooterStyle BackColor="Silver" Font-Bold="True" ForeColor="White" />
  <Columns>
  <asp:CommandField ShowSelectButton="True"  ItemStyle-Width="40px" 
          ItemStyle-HorizontalAlign="Center" ItemStyle-ForeColor="Blue" 
          ItemStyle-Height="25px">
<ItemStyle HorizontalAlign="Center" ForeColor="Blue" Height="25px" Width="40px"></ItemStyle>
      </asp:CommandField>
  <asp:BoundField  HeaderText="Supplier Code" DataField="Code" ItemStyle-Width="100px" 
          ItemStyle-HorizontalAlign="Center">
<ItemStyle HorizontalAlign="Center" Width="100px"></ItemStyle>
      </asp:BoundField>
  <asp:BoundField  HeaderText="Name" DataField="ContactName" ItemStyle-Width="200px" 
          ItemStyle-HorizontalAlign="Left">
<ItemStyle HorizontalAlign="Left" Width="200px"></ItemStyle>
      </asp:BoundField>
  <asp:BoundField  HeaderText="Address" DataField="Address1" ItemStyle-Width="200px" 
          ItemStyle-HorizontalAlign="Left">  
<ItemStyle HorizontalAlign="Left" Width="200px"></ItemStyle>
      </asp:BoundField>
  <asp:BoundField  HeaderText="Mobile" DataField="Mobile" ItemStyle-Width="100px" 
          ItemStyle-HorizontalAlign="Left">
<ItemStyle HorizontalAlign="Left" Width="100px"></ItemStyle>
      </asp:BoundField>
   <asp:BoundField  HeaderText="ID" DataField="ID" ItemStyle-Width="100px" 
          ItemStyle-HorizontalAlign="Left">
<ItemStyle HorizontalAlign="Left" Width="100px"></ItemStyle>
      </asp:BoundField>
      <asp:BoundField  HeaderText="Country" DataField="COUNTRY_DESC" ItemStyle-Width="100px" 
          ItemStyle-HorizontalAlign="Left">
<ItemStyle HorizontalAlign="Left" Width="100px"></ItemStyle>
      </asp:BoundField>
  </Columns>
                        <RowStyle BackColor="white" />
                        <EditRowStyle BackColor="" />
                        <PagerStyle  HorizontalAlign="Center" />
                        <AlternatingRowStyle BackColor="" />
  </asp:GridView>
</td>
</tr>
</table>
</td>
<td style="width:1%;"></td>
</tr></table>
</div>
</asp:Content>

