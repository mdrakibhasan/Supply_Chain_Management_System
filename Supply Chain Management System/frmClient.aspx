<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="frmClient.aspx.cs" Inherits="frmClient" Title="Client Information" Theme="Themes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
<div id="frmMainDiv" style="background-color:White; width:100%;">
<div style="vertical-align:top;">
<table  id="pageFooterWrapper">
  <tr>  
        <td style="width:5%;"></td>
        <td align="center">
        <asp:Button ID="Delete" runat="server" ToolTip="Delete" onclick="btnDelete_Click"
            
                onclientclick="javascript:return window.confirm('are u really want to delete these data')" Text="Delete" 
        Height="30px" Width="120px" BorderStyle="Outset"  />
        </td>
        <td style="width:20px;"></td>
        <td align="center" >
        <asp:Button ID="btnSave" runat="server" OnClientClick="this.disabled=true;" UseSubmitBehavior="false" ToolTip="Save Supplier Record" 
                onclick="btnSave_Click" Text="Save" 
        Height="30px" Width="120px" BorderStyle="Outset"  />
        </td>             
        <td style="width:20px;"></td>
        <td align="center" >
        <asp:Button ID="Clear" runat="server"  ToolTip="Clear" onclick="btnClear_Click" Text="Clear" 
        Height="30px" Width="120px" BorderStyle="Outset"  />
        </td>
        <td style="width:5%;">&nbsp;</td>
   </tr>
   </table>
</div>
<table style="width:100%;">
<tr>
<td style="width:1%;" align="center"> </td>
<td style="width:98%;" align="center"> 
<br />
<fieldset style="vertical-align: top; border: solid 1px #8BB381; text-align:left;"><legend style="color: maroon;"><b>Customer Information </b></legend>
<table style="width:100%;">
<tr>
<td style="width:14%;" align="left">
<asp:Label ID="lblCid" runat="server" Font-Size="8pt">Customer Code</asp:Label>
</td>
<td style="width:25%;" align="left"> 
<asp:TextBox SkinId="tbPlain"  ID="txtClientId" runat="server"  Width="80%" 
        Font-Size="8pt" MaxLength="4" CssClass="tbc" Enabled="false"></asp:TextBox>
</td>
<td style=" width:8%;" align="left" >
    <asp:CheckBox ID="CheckBox1" Checked="true" runat="server" Text="Active?" />
    </td>
<td align="left">
    <asp:CheckBox ID="CheckBox2"  runat="server" 
        Text="Common Customer?" />
    </td>
    <td> 
<asp:TextBox SkinID="tbPlain" ID="txtGlCoa" style="text-align:left;" 
        runat="server"  Width="80%" 
        Font-Size="9pt"  CssClass="tbc"></asp:TextBox>
                            </td>
    
</tr>
<tr>
<td style="width:14%;" align="left">
<asp:Label ID="lblCaddress" runat="server" Font-Size="8pt">Client Name</asp:Label>
</td>
<td style="width:25%;" align="left"> 
<asp:TextBox SkinId="tbPlain"  ID="txtClientName" runat="server" Width="80%" 
        Font-Size="8pt" MaxLength="200"></asp:TextBox>
</td>
<td style=" width:8%;" > 
<asp:Label ID="lbLId" runat="server" Font-Size="8pt" Visible="False"></asp:Label>
<asp:Label ID="lblGlCoa" runat="server" Font-Size="8pt" Visible="False"></asp:Label>
    </td>
<td style="width:13%;" align="left">
<asp:Label ID="lblCname" runat="server" Font-Size="8pt">National ID</asp:Label>
</td>
<td  style="width:25%;" align="left"> 
<asp:TextBox SkinId="tbPlain"  ID="txtNationalId" runat="server" Width="79.5%" 
        Font-Size="8pt" MaxLength="50" CssClass="tbl"></asp:TextBox></td>
</tr>
<tr>
<td style="width:14%;" align="left">
<asp:Label ID="Label15" runat="server" Font-Size="8pt">Address 1</asp:Label>
</td>
<td style="width:25%;" align="left"> 
<asp:TextBox SkinId="tbPlain"  ID="txtAddress1" runat="server" Width="98%" 
        Font-Size="8pt" MaxLength="200"></asp:TextBox>
</td>
<td style=" width:8%;" > 
    &nbsp;</td>
<td style="width:13%;" align="left">
<asp:Label ID="Label16" runat="server" Font-Size="8pt">Address 2</asp:Label>
</td>
<td  style="width:25%;" align="left"> 
<asp:TextBox SkinId="tbPlain"  ID="txtAddress2" runat="server" Width="98%" 
        Font-Size="8pt" MaxLength="200"></asp:TextBox>
    </td>
</tr>
<tr>
<td style="width:14%;" align="left">
<asp:Label ID="Label11" runat="server" Font-Size="8pt">Mobile</asp:Label>
</td>
<td style="width:25%;" align="left"> 
<asp:TextBox SkinId="tbPlain"  ID="txtMobile" runat="server" Width="80%" 
        Font-Size="8pt" MaxLength="20" CssClass="tbc"></asp:TextBox></td>
<td style=" width:8%;" >&nbsp;</td>
<td style="width:13%;" align="left">
<asp:Label ID="Label10" runat="server" Font-Size="8pt">Phone</asp:Label>
</td>
<td style="width:25%;" align="left" > 
<asp:TextBox SkinId="tbPlain"  ID="txtPhone" runat="server"  Width="79.5%" 
        Font-Size="8pt" MaxLength="20" CssClass="tbl"></asp:TextBox></td>
</tr>
<tr>
<td style="width:14%;" align="left">
<asp:Label ID="Label12" runat="server" Font-Size="8pt">Fax</asp:Label>
</td>
<td style="width:25%;" align="left"> 
<asp:TextBox SkinId="tbPlain"  ID="txtFax" runat="server"  Width="80%" 
        Font-Size="8pt" MaxLength="20" CssClass="tbl"></asp:TextBox></td>
<td style=" width:8%;" ></td>
<td style="width:13%;" align="left">
<asp:Label ID="Label13" runat="server" Font-Size="8pt">Email</asp:Label>
</td>
<td style="width:25%;" align="left" > 
<asp:TextBox SkinId="tbPlain"  ID="txtEmail" runat="server" Width="79.5%" 
        Font-Size="8pt" MaxLength="20" CssClass="tbc"></asp:TextBox></td>
</tr>
<tr>
<td style="width:14%;" align="left">
<asp:Label ID="lblSpId0" runat="server" Font-Size="8pt">Postal Code</asp:Label>
</td>
<td style="width:25%;" align="left"> 
<asp:TextBox SkinId="tbPlain"  ID="txtPostalCode" runat="server" Width="80%" 
        Font-Size="8pt" MaxLength="20"></asp:TextBox>
</td>
<td style=" width:8%;" >&nbsp;</td>
<td style="width:13%;" align="left">
<asp:Label ID="lblSupAddr8" runat="server" Font-Size="9pt">Country</asp:Label>
</td>
<td style="width:25%;" align="left" > 
    <asp:DropDownList ID="ddlCountry" runat="server" Height="26px" Width="83.5%">
    </asp:DropDownList>
    </td>
</tr>
</table>
</fieldset>
<br />
<asp:GridView CssClass="mGrid" PagerStyle-CssClass="pgr"  AlternatingRowStyle-CssClass="alt" 
ID="dgClient" runat="server" AutoGenerateColumns="False" Width="100%" 
        AllowPaging="True" BackColor="White" BorderWidth="1px" BorderStyle="Solid"
        CellPadding="2" BorderColor="LightGray" Font-Size="9pt" 
        AllowSorting="True" PageSize="25" 
        onselectedindexchanged="dgClient_SelectedIndexChanged" ForeColor="#333333" 
        onpageindexchanging="dgClient_PageIndexChanging" 
        onrowdatabound="dgClient_RowDataBound"  >
  <HeaderStyle Font-Size="9pt"  Font-Bold="True" HorizontalAlign="center" BackColor="Silver"/>
  <FooterStyle BackColor="Silver" Font-Bold="True" ForeColor="White" />
  <Columns>
  <asp:CommandField ShowSelectButton="True"  ItemStyle-Width="40px" 
          ItemStyle-HorizontalAlign="Center" ItemStyle-ForeColor="Blue">
<ItemStyle HorizontalAlign="Center" ForeColor="Blue" Width="40px"></ItemStyle>
      </asp:CommandField>
  <asp:BoundField  HeaderText="Customer Code" DataField="Code" 
          ItemStyle-Width="80px" ItemStyle-HorizontalAlign="Center">
<ItemStyle HorizontalAlign="Center" Width="80px"></ItemStyle>
      </asp:BoundField>
  <asp:BoundField  HeaderText="Name" DataField="ContactName" ItemStyle-Width="200px" 
          ItemStyle-HorizontalAlign="Left">
<ItemStyle HorizontalAlign="Left" Width="200px"></ItemStyle>
      </asp:BoundField>
  <asp:BoundField  HeaderText="Address" DataField="Address1" ItemStyle-Width="200px" 
          ItemStyle-HorizontalAlign="Left">  
<ItemStyle HorizontalAlign="Left" Width="200px"></ItemStyle>
      </asp:BoundField>
  <asp:BoundField  HeaderText="Mobile" DataField="Mobile" ItemStyle-Width="150px" 
          ItemStyle-HorizontalAlign="Left">
<ItemStyle HorizontalAlign="Left" Width="150px"></ItemStyle>
      </asp:BoundField>
      <asp:BoundField DataField="ID" HeaderText="ID"  ItemStyle-Width="150px" 
          ItemStyle-HorizontalAlign="Left">
<ItemStyle HorizontalAlign="Left" Width="150px"></ItemStyle>
      </asp:BoundField>
  </Columns>
                        <RowStyle BackColor="white" />
                        <EditRowStyle BackColor="" />
                        <PagerStyle  HorizontalAlign="Center" />
                        <AlternatingRowStyle BackColor="" />
  </asp:GridView>
</td>
<td style="width:1%;" align="center"> </td>
</tr>
</table>
</div>

</asp:Content>

