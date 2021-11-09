<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Home.aspx.cs" Inherits="Home" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <span style="font-size: x-small">
<link rel="stylesheet" type="text/css" href="css/bootstrap.min.css">
     <link rel="stylesheet" type="text/css" href="css/style.css">
    <script language="javascript" type="text/javascript" >    

    function LoadModalDiv() {

        var bcgDiv = document.getElementById("divBackground");
        bcgDiv.style.display = "block";

    }
    function HideModalDiv() {

        var bcgDiv = document.getElementById("divBackground");
        bcgDiv.style.display = "none";

    }

</script>


    </span>


<div id="frmMainDiv" style="background-color:White; width:99%; overflow:visible;">
<table style="width:100%;">
<tr>
<td style="width:100%; padding-left:10px;" align="center">
<asp:Panel ID="pnlTask" runat="server" Visible="false" Width="100%" 
        style="font-size: x-small">
            <table style="width:100%;">

            <tr>
                    <td align="left" style="width:15%; vertical-align:middle;">
                        &nbsp;</td>
                    <td style="width:2%;" rowspan="2">
                        <img src="img/box_bottom_hori.gif" alt="" width="1px" height="400px" />
                    </td>
                    <td align="center" colspan="2" style="vertical-align:top;" valign="middle">
                        <div class="row" style="padding-top: 20px; font-size: x-small;">
                            <div class="col-xs-6 col-md-2">
                                <a class="mnu-box"  href="PurchaseVoucher.aspx?mno=1.2" target="_blank">
                               <%--<img src="images/designation.png" />--%>
                                  Qty:<asp:Label ID="lblpurQty"
                                    runat="server" Text="0" ForeColor="#CC3300"></asp:Label>
                                    <br />
                                    Amount:<asp:Label ID="lblpurAmn"
                                    runat="server" Text="0" ForeColor="#CC3300"></asp:Label>                              
                                <br />
                                <h3>
                                    Purchase</h3>
                                </a>
                            </div>
                            <div class="col-xs-6 col-md-2">
                                <a class="mnu-box" href="PurchaseReturn.aspx?mno=1.3" target="_blank">
                               
                                 Qty:<asp:Label ID="lblpurReQty"
                                    runat="server" Text="0" ForeColor="#CC3300"></asp:Label>
                                    <br />
                                    Amount:<asp:Label ID="lblpurReAmn"
                                    runat="server" Text="0" ForeColor="#CC3300"></asp:Label>
                                                                   <br />
                                <h3>
                                    Purchase Return</h3>
                                </a>
                            </div>
                            <div class="col-xs-6 col-md-2">
                                <a  class="mnu-box" target="_blank" href="frmConsumptionManually.aspx?mno=0.0">
                                   Qty:<asp:Label ID="lblCnQty"
                                    runat="server" Text="0" ForeColor="#CC3300"></asp:Label>
                                    <br />
                                    Amount:<asp:Label ID="lblCnAmn"
                                    runat="server" Text="0" ForeColor="#CC3300"></asp:Label>                             
                                 <br />
                                <h3>
                                    Consumption</h3>
                                </a>
                            </div>
                            <div class="col-xs-6 col-md-2">
                                <a class="mnu-box" href="rptProductin.aspx?mno=0.1" target="_blank">
                                 Qty:<asp:Label ID="lblDmQty"
                                    runat="server" Text="0" ForeColor="#CC3300"></asp:Label>
                                    <br />
                                    Amount:<asp:Label ID="lblDmAmn"
                                    runat="server" Text="0" ForeColor="#CC3300"></asp:Label>                               
                                <br />
                                <h3>
                                    Production Received</h3>
                                </a>
                            </div>
                            <div class="col-xs-6 col-md-2">
                                <a class="mnu-box" href="SalesVoucher.aspx?mno=1.6" target="_blank">
                                Qty:<asp:Label ID="lblsalesqty"
                                    runat="server" Text="0" ForeColor="#CC3300"></asp:Label>
                                    <br />
                                    Amount:<asp:Label ID="lblsaleamn"
                                    runat="server" Text="0" ForeColor="#CC3300"></asp:Label>

                                <br />
                                <h3>
                                    Sales </h3>
                                </a>
                            </div>
                            <div class="col-xs-6 col-md-2">
                                <a class="mnu-box" href="StockItemsDetails.aspx?mno=1.4" target="_blank">
                                Qty:<asp:Label ID="lblStQty"
                                    runat="server" Text="0" ForeColor="#CC3300"></asp:Label>
                                    <br />
                                    Amount:<asp:Label ID="lblStAmn"
                                    runat="server" Text="0" ForeColor="#CC3300"></asp:Label>

                                <br />
                                <h3>
                                    Closing Stock</h3>
                                </a>
                            </div>
                            
                        </div>
                    </td>
                </tr>
            <tr>
            <td style="width:15%; vertical-align:middle;" align="left">            
                <asp:LinkButton ID="lbChangePass" runat="server" Font-Bold="True" OnClientClick="LoadModalDiv();"
                    Font-Size="8pt" ForeColor="#993300" 
                    Text="Change Password"></asp:LinkButton>
                <ajaxToolkit:ModalPopupExtender ID="ModalPopupExtenderLogin" runat="server" 
                    BackgroundCssClass="modalBackground" DropShadow="true" 
                    PopupControlID="pnlChangePass" TargetControlID="lbChangePass" />

                     
                </td>

  <td style="width:25%; vertical-align:top;" align="center" valign="middle">         
      <br />
      <br />
      <br />
      <br />
      <div style=" width:70%; background-color: #0076AE">    
<asp:Timer ID="Timer1" runat="server" Interval="1000" ontick="Timer1_Tick">
                </asp:Timer>
        <asp:UpdatePanel ID="UpdatePanel1"
            runat="server">
            <ContentTemplate>
                   <asp:Label ID="lblTime" runat="server"
                    ForeColor="White" Font-Bold="True" Font-Size="X-Large" ></asp:Label>
                <br />
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="tIMER1" EventName="Tick"></asp:AsyncPostBackTrigger>
            </Triggers>
        </asp:UpdatePanel>
        <br />
    

    </div>
    <br />    
      <br />
      <img src="img/supply.jpg" 
          style="height: 180px" width="90%" />
</td>
                <td style="padding: 10px;" valign="top">
                    <asp:Panel ID="Panel1" runat="server" BackColor="#faebd7">
                       <fieldset style="vertical-align: top; border: solid 1px #8BB381;">
                                 <legend style="color: maroon; font-weight: 700;">Item Status</legend>
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 20%">
                                    &nbsp;</td>
                                <td style="width: 55%">
                                    
                                   
                                </td>
                                <td style="width: 15%; margin-left: 40px;">
                                    &nbsp;</td>
                            </tr>
                            <tr>
                                <td style="width: 20%; font-size: small;">
                                    Search StyleNo</td>
                                <td style="width: 55%">
                                <asp:TextBox ID="txtSearchItemITS" runat="server" 
                                        Height="22px" 
                                       Width="96%"></asp:TextBox>

                                        <ajaxToolkit:AutoCompleteExtender ServicePath="AutoComplete.asmx" runat="server" ID="autoComplete1" TargetControlID="txtSearchItemITS"
                ServiceMethod="GEtStyleNo" MinimumPrefixLength="1" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="12"/>
                                </td>
                                <td style="width: 15%; margin-left: 40px;">
                                    <asp:Button ID="btnPrint" runat="server" BackColor="#CACACE" 
                                        BorderStyle="Inset" onclick="btnPrint_Click" Text="Find" />
                                </td>
                            </tr>
                            <tr>
                                <td align="right" colspan="3">
                                    <asp:Panel ID="Panel2" runat="server">
                                        <table style="width: 100%">
                                            <tr>
                                                <td style="width: 20%">
                                                    &nbsp;</td>
                                                <td style="width: 25%">
                                                    &nbsp;</td>
                                                <td style="width: 25%">
                                                    &nbsp;</td>
                                                <td style="width: 25%">
                                                    &nbsp;</td>
                                            </tr>
                                            <tr>
                                                <td style="width: 20%; font-weight: 700; height: 13px; font-size: small;" 
                                                    align="right">
                                                    <span>Name:&nbsp;&nbsp;&nbsp;&nbsp; </span>
                                                </td>
                                                <td align="left" colspan="2" style="height: 13px">
                                                    <asp:Label ID="lblItemName" runat="server" style="font-size: small"></asp:Label>
                                                    </span>
                                                </td>
                                                <td style="width: 25%; height: 13px;">
                                                    </td>
                                            </tr>
                                            <tr style="font-size: x-small">
                                                <td style="width: 20%; font-weight: 700; font-size: small;" align="right">
                                                    Unit Price:&nbsp;&nbsp;&nbsp;&nbsp;
                                                </td>
                                                <td align="left" style="width: 25%; font-size: small;">
                                                    <asp:Label ID="lblUnitPrice" runat="server"></asp:Label>
                                                </td>
                                                <td style="width: 25%; font-weight: 700; font-size: small;" align="right">
                                                    UOM:&nbsp;&nbsp;&nbsp;
                                                </td>
                                                <td align="left" style="width: 25%; font-size: small;">
                                                    <asp:Label ID="lblUOM" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr style="font-size: x-small">
                                                <td align="right" 
                                                    style="width: 20%; font-weight: 700; height: 12px; font-size: small;">
                                                    Style NO:&nbsp;&nbsp;&nbsp;
                                                </td>
                                                <td align="left" style="width: 25%; height: 12px; font-size: small;">
                                                    <asp:Label ID="lblStyleNO" runat="server"></asp:Label>
                                                </td>
                                                <td align="right" 
                                                    style="width: 25%; font-weight: 700; height: 12px; font-size: small;">
                                                    Supplier:&nbsp;&nbsp;</td>
                                                <td align="left" style="width: 25%; height: 12px; font-size: small;">
                                                    <asp:Label ID="lblSupplier" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="width: 20%; font-weight: 700; height: 4px;" align="right">
                                                    &nbsp;&nbsp;&nbsp;&nbsp;
                                                </td>
                                                <td style="width: 25%; height: 4px;" align="left">
                                                    &nbsp;</td>
                                                <td style="width: 25%; font-weight: 700; height: 4px;" align="right">
                                                </td>
                                                <td style="width: 25%; height: 4px;" align="left">
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="width: 20%" align="right">
                                                    &nbsp;</td>
                                                <td style="width: 25%">
                                                    &nbsp;</td>
                                                <td style="width: 25%" align="right">
                                                    &nbsp;</td>
                                                <td style="width: 25%" align="left">
                                                    &nbsp;</td>
                                            </tr>
                                            <tr>
                                                <td colspan="4">
                                                    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
                                                        BackColor="Cornsilk" Width="100%" Font-Size="Small">
                                                        <Columns>
                                                            <asp:BoundField DataField="Description" HeaderText="Description" 
                                                                HeaderStyle-HorizontalAlign="Center">
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="Date" HeaderText="Date">
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle HorizontalAlign="Center" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="PurQty" HeaderText="Purchase">
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle HorizontalAlign="Center" />
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle HorizontalAlign="Center" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="CnQty" HeaderText="Consumtion" />
                                                            <asp:BoundField DataField="DmQty" HeaderText="Damages" />
                                                            <asp:BoundField DataField="PRQty" HeaderText="Pur.Return" />
                                                            <asp:BoundField DataField="Stock" HeaderText="Stock" />
                                                        </Columns>
                                                        <PagerStyle HorizontalAlign="Center" />
                                                    </asp:GridView>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="width: 20%">
                                                    &nbsp;</td>
                                                <td style="width: 25%">
                                                    &nbsp;</td>
                                                <td style="width: 25%">
                                                    &nbsp;</td>
                                                <td style="width: 25%">
                                                    &nbsp;</td>
                                            </tr>
                                            <tr>
                                                <td style="width: 20%">
                                                    &nbsp;</td>
                                                <td style="width: 25%">
                                                    &nbsp;</td>
                                                <td style="width: 25%">
                                                    &nbsp;</td>
                                                <td align="right" style="width: 25%">
                                                    <asp:Button ID="Button1" runat="server" Font-Size="10pt" Height="28px" 
                                                        Text="Print" onclick="Button1_Click" />
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" colspan="3">
                                
                                    &nbsp;&nbsp;</td>
                            </tr>
                            <tr>
                                <td align="right" colspan="3">
                                    &nbsp;</td>
                            </tr>
                        </table></fieldset>
                    </asp:Panel>
                </td>
</tr>
                
</table>
</asp:Panel>

<%-- </ContentTemplate>
 </asp:UpdatePanel>--%>
</td>
</tr>
<tr>
<td style="width:100%; padding-left:10px;" align="center">
 <asp:Panel ID="pnlChangePass" runat="server" CssClass="modalPopup" 
                            Style=" display:none; background-color: White; font-size: x-small;" 
                            Width="275px" Height="227px">
                            <fieldset style=" text-align:left; vertical-align: top; border: solid 1px #8BB381;line-height:1.5em;"><legend style="color: maroon;"><b>Change Password</b></legend>
                            <table style="width:250px; font-size:8pt;">
                                <tr>
                                    <td style="width:150px;">
                                        User ID</td>
                                    <td style="width:100px;">
                                        <asp:TextBox ID="txtCpUserName" runat="server" Enabled="false" Font-Size="9" 
                                            MaxLength="18" SkinID="tbGray" Width="100px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="width:150px;">
                                        Current Password</td>
                                    <td style="width:100px;">
                                        <asp:TextBox ID="txtCpCurPass" runat="server" Font-Size="9" MaxLength="18" 
                                            SkinID="tbGray" TextMode="Password" Width="100px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="width:150px;">
                                        New Password</td>
                                    <td style="width:100px;">
                                        <asp:TextBox ID="txtCpNewPass" runat="server" Font-Size="9" MaxLength="18" 
                                            SkinID="tbGray" TextMode="Password" Width="100px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="width:150px;">
                                        Confirm Password</td>
                                    <td style="width:100px;">
                                        <asp:TextBox ID="txtCpConfPass" runat="server" Font-Size="9" MaxLength="18" 
                                            SkinID="tbGray" TextMode="Password" Width="100px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2" style="height:10px;">
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right" style="width:150px;">
                                        <asp:Button ID="lbCancel" runat="server" BorderStyle="Outset" BorderWidth="1px" OnClientClick="HideModalDiv();"
                                            Font-Size="8pt" Height="25px" Text="Cancel" 
                                            Width="100px" />
                                    </td>
                                    <td align="center" style="width:100px;">
                                        <asp:Button ID="lbChangePassword" runat="server" BorderStyle="Outset" 
                                            BorderWidth="1px" Font-Size="8pt" Height="25px" 
                                            OnClick="lbChangePassword_click" Text="Change" Width="100px" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <asp:Label ID="lblTranStatus" runat="server" Font-Size="8pt" Text="" 
                                            Visible="false"></asp:Label>
                                    </td>
                                </tr>
                            </table></fieldset>
                        </asp:Panel>
    
</td>
</tr>
</table>
</div>      
<div id="divBackground" style="position: fixed; z-index: 999; height: 100%; width: 100%;
        top: 0; left:0; background-color: Black; filter: alpha(opacity=60); opacity: 0.6; -moz-opacity: 0.8;-webkit-opacity: 0.8;display:none">
    </div>
</asp:Content>

