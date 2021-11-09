<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="SalesVoucher.aspx.cs" Inherits="SalesVoucher" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <script language="javascript" type="text/javascript" >
    function OpenWindow(Url)
    {
        var testwindow = window.open(Url, '', 'width=600px,height=400px,top=230,left=300,scrollbars=1');
    }
    function onListPopulated()
    {
        var completionList1 = $find("AutoCompleteEx1").get_completionList();
        completionList1.style.width = 'auto';
        var completionList2 = $find("AutoCompleteEx2").get_completionList();
        completionList2.style.width = 'auto';
        var completionList3 = $find("AutoCompleteEx3").get_completionList();
        completionList3.style.width = 'auto';
    }
    function setValue(myVal) {
        document.getElementById('<%=txtItemsCode.ClientID%>').value = myVal;
    } 

    function pad(str, max) {
        return str.length < max ? pad("0" + str, max) : str;
    }

    function remLink() {
        if (window.testwindow && window.testwindow.open && !window.testwindow.closed)
            window.testwindow.opener = null;
    }
    function IsEmpty(aTextField) 
    {
        if ((aTextField.value.length == 0) || (aTextField.value == null)) {
            return true;
        }
        else {
                return false;
             }
        }

        function LoadModalDiv() {

            var bcgDiv = document.getElementById("divBackground");
            bcgDiv.style.display = "block";

        }
        function HideModalDiv() {

            var bcgDiv = document.getElementById("divBackground");
            bcgDiv.style.display = "none";

        }

        function changetextbox() {
            var valu = $('#<%=ddlPaymentMethord.ClientID%> option:selected').attr('value');
            if (valu === "C") {

                document.getElementById("<%=ddlBank.ClientID %>").disabled = true;
                document.getElementById("<%=txtChequeNo.ClientID %>").disabled = true;
                document.getElementById("<%=txtChequeDate.ClientID %>").disabled = true;
                document.getElementById("<%=txtChequeAmount.ClientID %>").disabled = true;
            }
            else {

                document.getElementById("<%=ddlBank.ClientID %>").disabled = false;
                document.getElementById("<%=txtChequeNo.ClientID %>").disabled = false;
                document.getElementById("<%=txtChequeDate.ClientID %>").disabled = false;
                document.getElementById("<%=txtChequeAmount.ClientID %>").disabled = false;

            }
        }

        function TotSumation() {
            

            var txtSubTotal = document.getElementById("<%=txtSubTotal.ClientID %>");var txtVat = document.getElementById("<%=txtVat.ClientID %>");          
            var txtDiscount = document.getElementById("<%=txtDiscount.ClientID %>");var txtPayment = document.getElementById("<%=txtPayment.ClientID %>");
            var txtDue = document.getElementById("<%=txtDue.ClientID %>"); 

            var totv = parseFloat(txtSubTotal.value) + ((parseFloat(txtSubTotal.value) * parseFloat(txtVat.value)) / 100);
            var todD = parseFloat(totv) - ((parseFloat(totv) * parseFloat(txtDiscount.value)) / 100);
            txtPayment.value = todD.toFixed(2);
            
        }

        function Due() {

            var txtSubTotal = document.getElementById("<%=txtSubTotal.ClientID %>");
            var txtVat = document.getElementById("<%=txtVat.ClientID %>");
            var txtDiscount = document.getElementById("<%=txtDiscount.ClientID %>");
            var txtPayment = document.getElementById("<%=txtPayment.ClientID %>");
            var txtDue = document.getElementById("<%=txtDue.ClientID %>");

            var totv = parseFloat(txtSubTotal.value) + ((parseFloat(txtSubTotal.value) * parseFloat(txtVat.value)) / 100);
            var todD = parseFloat(totv) - ((parseFloat(totv) * parseFloat(txtDiscount.value)) / 100);          
            if (parseFloat(todD) < parseFloat(txtPayment.value)) {
                alert("Payment Amount Over This Total Charge.....!!!!!!");
                txtTotPayment.value = "0";
                txtTotPayment.focus();
            }
            else {
                var tt = parseFloat(todD) - parseFloat(txtPayment.value)
                txtDue.value = tt.toFixed(2);
            }
        }
       
</script>  
<script language="javascript" type="text/javascript">

    function LoadModalDiv() {

        var bcgDiv = document.getElementById("divBackground");
        bcgDiv.style.display = "block";

    }
    function HideModalDiv() {

        var bcgDiv = document.getElementById("divBackground");
        bcgDiv.style.display = "none";

    }
    
 </script>
<div id="frmMainDiv" style="background-color:White; width:100%;">  
<table  id="pageFooterWrapper">
  <tr>  
        <td align="center">
        <asp:Button ID="btnDelete" runat="server" ToolTip="Delete"
            
                
                
                onclientclick="javascript:return window.confirm('are u really want to delete these data')" Text="Delete" 
        Height="30px" Width="110px" BorderStyle="Outset" onclick="btnDelete_Click"  />
        </td>       
        <td align="center" >
            &nbsp;</td>       
        <td align="center" >
        <asp:Button ID="btnSave" runat="server" OnClientClick="this.disabled=true;" UseSubmitBehavior="false" ToolTip="Save Purchase Record" Text="Save" 
        Height="30px" Width="110px" BorderStyle="Outset" onclick="btnSave_Click"  />
        </td>
        <td align="center" >
        <asp:Button ID="btnNew" runat="server" ToolTip="New"  Text="New" 
        Height="30px" Width="110px" BorderStyle="Outset" onclick="btnNew_Click"  /> 
        </td>
        <td align="center" >
        <asp:Button ID="btnClear" runat="server"  ToolTip="Clear" Text="Clear" 
        Height="30px" Width="110px" BorderStyle="Outset" onclick="btnClear_Click"  />
        </td>
        <td align="center" >
        <asp:Button ID="btnPrint" runat="server" ToolTip="Print PO" Text="Print" 
        Height="30px" Width="110px" BorderStyle="Outset" onclick="btnPrint_Click"  />
        </td>            
        
        <td align="center" >
            <asp:Button ID="btnChallan" runat="server" onclick="btnChallan_Click" 
                Text="Print Challan" />
        </td>   
        
        <td align="center" >
        <asp:Button ID="btnFind" runat="server" ToolTip="Find" Text="Find" 
        Height="30px" Width="110px" BorderStyle="Outset" onclick="btnFind_Click" 
                Visible="False" />
        </td>         
        
   </tr>
   </table>
   <br />

<table style="width:100%;">
<tr>
<td style="width:1%; height: 3px;"></td>
<td style="width:41%; height: 3px;" align="center"> 
               <asp:TextBox ID="txtChequeAmount" runat="server" Enabled="False" 
                   Font-Size="8pt" SkinID="tbPlain" style="text-align:right;" TabIndex="17" 
                   Width="20px" Visible="False"></asp:TextBox>
           <asp:Label ID="lblAmount" runat="server" Visible="False"></asp:Label>
    </td>
<td style="width:62%; height: 3px;">
                <asp:Label ID="lblInvNo" runat="server" Visible="False"></asp:Label>
            </td>
<td style="height: 3px"></td>
</tr>
<tr>
<td style="width:1%;">&nbsp;</td>
<td style="width:41%;" align="center"> 
<fieldset style="vertical-align: top; border: solid 1px #8BB381;text-align:left;"><legend style="color: maroon;"><b>Search Items</b> </legend>
 <asp:UpdatePanel ID="UpSearch" runat="server" UpdateMode="Conditional"><ContentTemplate>
    <table style="width: 100%">
        <tr>
            <td style="width: 124px" align="left">
                <asp:Label ID="Label1" runat="server" Text="Item Code"></asp:Label>
            </td>
            <td align="left" style="width: 346px">
                <asp:TextBox ID="txtItemsCode" runat="server" Width="94%" AutoPostBack="True" 
                    onfocus="this.select();" TabIndex="1"
                    ontextchanged="txtCode_TextChanged" Height="20px"></asp:TextBox>
            </td>
            <td>
            <a href="javascript:OpenWindow('frmItemsDetails.aspx')" style="width:5px">.....</a></td>
        </tr>
    </table>
    </fieldset>
     </ContentTemplate>
    </asp:UpdatePanel>
</td>
<td style="width:62%;">
<fieldset style="vertical-align: top; border: solid 1px #8BB381;"><legend style="color: maroon;"><b>Invoice No</b> </legend>
    <table style="width: 100%">
        <tr>
            <td style="width: 87px">
                <asp:Label ID="Label2" runat="server" Text="Invoice No"></asp:Label>
            </td>
            <td style="width: 232px">
                <asp:TextBox ID="txtInvoiceNo" runat="server" Width="100%" 
                    style="text-align:center;" TabIndex="2" 
                    ontextchanged="txtInvoiceNo_TextChanged" AutoPostBack="True" Height="20px"></asp:TextBox>

                     <ajaxToolkit:AutoCompleteExtender ServicePath="AutoComplete.asmx" runat="server" 
                                     ID="autoComplete1" TargetControlID="txtInvoiceNo"
           ServiceMethod="GetInvoice" MinimumPrefixLength="1" CompletionInterval="1000" EnableCaching="true" 
                                     CompletionSetCount="12"/>
            </td>
            <td align="center" style="width: 30px">
                <asp:Label ID="Label4" runat="server" Text="*" ForeColor="#CC3300"></asp:Label>
            </td>
            <td style="width: 88px">
                <asp:Label ID="Label3" runat="server" Text="Date"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtDate" runat="server" Width="98%" style="text-align:center;" 
                    TabIndex="3" Height="20px"></asp:TextBox>
                <ajaxtoolkit:calendarextender runat="server" ID="Calendarextender1" 
                TargetControlID="txtDate" Format="dd/MM/yyyy"/>
            </td>
        </tr>
    </table>
    </fieldset>
    </td>
<td>
                <asp:TextBox ID="txtItemsID" runat="server" Width="15px" ForeColor="Red"  
                    BorderWidth="0px" style="border-color:none;border:0px; background:transparent;"></asp:TextBox>
            </td>
</tr>
<tr>
<td style="width:1%;">&nbsp;</td>
<td align="center" colspan="2"> 
    <asp:Panel ID="Panel1" runat="server" style="vertical-align: top; border: solid 1px #8BB381;">
    <asp:UpdatePanel ID="UpItemsDetails" runat="server" UpdateMode="Conditional"><ContentTemplate>
        <table style="width:100%;"><tr>
             <td align="center">
                 <table style="width: 100%">                
                     <tr>
                         <td>
                             <asp:GridView ID="dgSV" runat="server" 
                                 AutoGenerateColumns="False" BackColor="White" BorderColor="LightGray" 
                                 BorderStyle="Solid" BorderWidth="1px" CellPadding="2" CssClass="mGrid" 
                                 Font-Size="9pt" onrowdatabound="dgPVMst_RowDataBound" 
                                 onrowdeleting="dgSV_RowDeleting" PageSize="30" 
                                 Width="100%" Caption="Items Details">
                                 <Columns>
                                     <asp:BoundField DataField="ID" HeaderText="ID">
                                     <ItemStyle HorizontalAlign="Center" Width="60px" />
                                     </asp:BoundField>
                                     <asp:TemplateField>
                                         <ItemTemplate>
                                             <asp:ImageButton ID="lbDelete" runat="server" CausesValidation="False" 
                                                 CommandName="Delete" ImageAlign="Middle" ImageUrl="~/img/delete.png" 
                                                 Text="Delete" />
                                         </ItemTemplate>
                                         <ItemStyle Font-Size="8pt" HorizontalAlign="Center" Width="4%" />
                                     </asp:TemplateField>
                                     <asp:BoundField DataField="Code" HeaderText="Code">
                                     <ItemStyle HorizontalAlign="Center" Width="60px" />
                                     </asp:BoundField>
                                     <asp:BoundField DataField="Name" HeaderText="Items">
                                     <ItemStyle Height="20px" HorizontalAlign="Left" Width="250px" />
                                     </asp:BoundField>
                                     <asp:BoundField DataField="Tax" HeaderText="Vat(%)">
                                     <ItemStyle Height="20px" HorizontalAlign="Center" Width="60px" />
                                     </asp:BoundField>                      

                                     <asp:TemplateField HeaderText="Dis(%)">
                                         <ItemTemplate>
                                             <asp:TextBox ID="txtDiscount" runat="server" AutoPostBack="True" ontextchanged="txtDiscount_TextChanged"
                                                 onfocus="this.select();" style="text-align:center;" Text='<%# Eval("DiscountAmount") %>' Width="92%">
                                             </asp:TextBox>
                                         </ItemTemplate>
                                         <ItemStyle Height="20px" Width="60px" />
                                     </asp:TemplateField>

                                     <%--<asp:BoundField DataField="SPrice" HeaderText="Sale Price">
                                     <ItemStyle Height="20px" Width="100px" HorizontalAlign="Right" />
                                     </asp:BoundField>--%>
                                     <asp:TemplateField HeaderText="Sale Price">
                                         <ItemTemplate>
                                             <asp:TextBox ID="txtSalesPrice" runat="server" AutoPostBack="True" 
                                                 onfocus="this.select();" style="text-align:center;" 
                                                 Text='<%# Eval("SPrice") %>' Width="92%" 
                                                 ontextchanged="txtSalesPrice_TextChanged"></asp:TextBox>
                                         </ItemTemplate>
                                         <ItemStyle Height="20px" Width="60px" />
                                     </asp:TemplateField>

                                     <asp:TemplateField HeaderText="Qty">
                                         <ItemTemplate>
                                             <asp:TextBox ID="txtQty" runat="server" AutoPostBack="True" 
                                                 onfocus="this.select();" ontextchanged="txtQty_TextChanged" 
                                                 style="text-align:center;" Text='<%# Eval("Qty") %>' Width="92%"></asp:TextBox>
                                         </ItemTemplate>
                                         <ItemStyle Height="20px" Width="60px" />
                                     </asp:TemplateField>
                                     <asp:BoundField DataField="Total" HeaderText="Total">
                                     <ItemStyle Height="20px" HorizontalAlign="Right" Width="100px" />
                                     </asp:BoundField>
                                    <asp:BoundField DataField="ClosingStock" HeaderText="ClosingStk">
                                     <ItemStyle Height="20px" HorizontalAlign="Right" Width="100px" />
                                     </asp:BoundField>
                                 </Columns>
                                 <RowStyle BackColor="White" />
                                 <SelectedRowStyle Font-Bold="True" />
                                 <PagerStyle BackColor="LightGray" ForeColor="Black" HorizontalAlign="Center" 
                                     CssClass="pgr" />
                                 <AlternatingRowStyle CssClass="alt" />
                                 <HeaderStyle Font-Size="9pt" />
                             </asp:GridView>
                         </td>
                     </tr>
                     <tr>
                         <td>
        <table style="width: 100%">
        <tr>
            <td style="width: 359px" valign="top">
           <fieldset style="vertical-align: top; border: solid 1px #8BB381;"><legend style="color: maroon;"><b>Payment Methord</b> </legend>     
           <asp:UpdatePanel ID="UPPaymentMtd" runat="server" UpdateMode="Conditional">
           <ContentTemplate>  
           <table style="width: 100%">
           <tr>
               <td style="width: 119px; height: 27px;" align="right">
               <asp:Label ID="Label26" runat="server" Font-Size="9pt">Payment Methord</asp:Label></td>
               <td style="width: 8px; height: 27px;"></td>
               <td align="left" style="height: 27px">
               <asp:DropDownList ID="ddlPaymentMethord" runat="server" TabIndex="13"
                       Font-Size="8pt" SkinID="ddlPlain" Width="80%" 
                       Height="26px" 
                       onselectedindexchanged="ddlPaymentMethord_SelectedIndexChanged" 
                       AutoPostBack="True"><asp:ListItem Text="Cash" Value="C"></asp:ListItem><asp:ListItem Value="Q">Cheque</asp:ListItem>
               </asp:DropDownList>
               </td>
           </tr>
               <tr>
                   <td align="right" style="width: 119px; height: 27px;">
                       <asp:Label ID="lblBankName" runat="server" Font-Size="9pt">Bank Name</asp:Label>
                   </td>
                   <td style="width: 8px; height: 27px;">
                       </td>
                   <td align="left" style="height: 27px">
                       <asp:DropDownList ID="ddlBank" runat="server" Font-Size="8pt" 
                           Height="26px" SkinID="ddlPlain" TabIndex="14" Width="80%">
                       </asp:DropDownList>
                   </td>
               </tr>
           <tr>
           <td style="width: 119px; height: 24px;" align="right"><asp:Label ID="lblChequeNo" runat="server" Font-Size="9pt">Cheque No</asp:Label></td>
           <td style="width: 8px; height: 24px;"></td>
           <td align="left" style="height: 24px"><asp:TextBox ID="txtChequeNo" runat="server" 
                   Font-Size="8pt" TabIndex="15"
                   SkinID="tbPlain" Width="77%"></asp:TextBox></td>
           </tr>
           <tr><td style="width: 119px; height: 29px;" align="right"><asp:Label ID="lblChequeDate" runat="server" 
                   Font-Size="9pt">Cheque date</asp:Label></td>
           <td style="width: 8px; height: 29px;"></td>
           <td align="left" style="height: 29px">
               <asp:TextBox ID="txtChequeDate" runat="server" Font-Size="8pt" SkinId="tbPlain" TabIndex="16"
                      Width="77%"></asp:TextBox>
                      <cc:CalendarExtender ID="txtChequeDate_CalendarExtender" runat="server"  Format="dd/MM/yyyy" Enabled="True" TargetControlID="txtChequeDate"></cc:CalendarExtender></td></tr><tr>
               <td style="width: 119px; height: 27px;" align="right">
                   <asp:Label ID="lblChequeStatus" runat="server" Font-Size="9pt">Check 
                          Status</asp:Label>
               </td><td style="width: 8px; height: 27px;">
               </td><td align="left" style="height: 27px">
                   <asp:DropDownList ID="ddlChequeStatus" runat="server" Font-Size="8pt" 
                       Height="26px" 
                       SkinID="ddlPlain" TabIndex="24" Width="80%">
                       <asp:ListItem></asp:ListItem>
                       <asp:ListItem Value="P">Pending</asp:ListItem>
                       <asp:ListItem Value="A">Approved</asp:ListItem>
                       <asp:ListItem Value="B">Bounce</asp:ListItem>
                   </asp:DropDownList>
               </td></tr>
             </table></ContentTemplate></asp:UpdatePanel>
            </fieldset>
            </td>
            <td valign="top">
             <fieldset style="vertical-align: top; border: solid 1px #8BB381;"><legend style="color: maroon;"><b>Payment Info</b> </legend>  
                <table style="width: 100%">
                    <tr>
                        <td style="width: 174px; height: 24px;" align="right">
                            <asp:Label ID="Label35" runat="server" Text="Customer Name" 
                                style="font-weight: 700"></asp:Label>
                        </td>
                        <td style="width: 6px; height: 24px;">
                            </td>
                            
                        <td style="width: 278px; height: 24px;" align="left">
                              <asp:UpdatePanel ID="UPCustomer" runat="server" UpdateMode="Conditional">
                                  <ContentTemplate>
                                      <asp:DropDownList ID="ddlCustomer" runat="server" AutoPostBack="True" 
                                          Font-Size="8pt" Height="26px" onChange="changetextbox();" SkinID="ddlPlain" 
                                          TabIndex="9" Width="102%" 
                                          onselectedindexchanged="ddlCustomer_SelectedIndexChanged">
                                      </asp:DropDownList>
                                      </td>
                                  </ContentTemplate>
                              </asp:UpdatePanel>
                              <td style="width: 78px" align="center">
                                  <asp:LinkButton ID="HyperLink1" runat="server" Font-Bold="True" 
                                      Font-Size="Small" Font-Underline="True" OnClientClick="LoadModalDiv();">New</asp:LinkButton>
                                  <cc:ModalPopupExtender ID="ModalPopupExtender1" runat="server" 
                                      BackgroundCssClass="modalBackground" CancelControlID="btnClientQuit" 
                                      DropShadow="true" PopupControlID="pnlClient" TargetControlID="HyperLink1" />
                              </td>
                              <td align="right" style="width: 161px; height: 24px;">
                                  <asp:Label ID="Label30" runat="server" Text="Sub Total"></asp:Label>
                              </td>
                        <td style="width: 21px; height: 24px;">
                        </td>
                        <td style="height: 24px;" align="right">
                            <asp:TextBox ID="txtSubTotal" runat="server" Enabled="False" 
                                style="text-align:right;" TabIndex="4" Width="100px"></asp:TextBox>
                              </td>
                        </tr>
                    <tr>
                        <td style="width: 174px; height: 24px; font-weight: 700;" align="right">
                            &nbsp;</td>
                        <td style="width: 6px; height: 24px;">
                            </td>
                        <td style="width: 278px; height: 24px;" align="left">
                            <asp:DropDownList ID="ddlDelevery" runat="server" Font-Size="8pt" Height="26px" 
                                onChange="changetextbox();" SkinID="ddlPlain" TabIndex="10" Visible="False" 
                                Width="102%">
                                <asp:ListItem Value="Y">Yes</asp:ListItem>
                                <asp:ListItem Value="N">No</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td style="width: 78px">
                            <asp:TextBox ID="txtMobilenumber" runat="server" Visible="False" Width="98%"></asp:TextBox>
                        </td>
                        <td align="right" style="width: 161px; height: 24px;">
                            <asp:Label ID="Label37" runat="server" Text="VAT (%)"></asp:Label>
                        </td>
                        <td style="width: 21px; height: 24px;">
                            </td>
                        <td align="right" style="height: 24px">
                            <asp:TextBox ID="txtVat" onChange="Due();" runat="server" align="right"  TabIndex="5"
                                style="text-align:right;" Width="100px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 174px; height: 20px;" align="right">
                          <asp:Label ID="lblClientName" runat="server" Font-Size="9pt" 
                                style="font-weight: 700" >Delivery Date </asp:Label>
                            </td>
                        <td style="width: 6px; height: 20px;">
                            </td>
                        <td style="width: 278px; height: 20px;" align="left">
                <asp:TextBox ID="txtDeleveryDate" runat="server" Width="99%" placeholder="dd/MM/yyyy" style="text-align:center;"
                                TabIndex="11"></asp:TextBox>
                       <ajaxtoolkit:calendarextender runat="server" ID="Calendarextender2" 
                TargetControlID="txtDeleveryDate" Format="dd/MM/yyyy"/>
                            </td>
                        <td style="width: 78px">
                            &nbsp;</td>
                        <td align="right" style="width: 161px; height: 20px;">
                            <asp:Label ID="Label36" runat="server" Text="Discount(%)"></asp:Label>
                        </td>
                        <td style="width: 21px; height: 20px;">
                            </td>
                        <td style="height: 20px" align="right">
                            <asp:TextBox ID="txtDiscount" runat="server" style="text-align:right;" onChange="Due();" onfocus="this.select();" TabIndex="6"
                                Width="100px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 174px; height: 15px;" align="right">
                          <asp:Label ID="lblClientName0" runat="server" Font-Size="9pt" 
                                style="font-weight: 700" >Remark&#39;s</asp:Label></td>
                        <td style="width: 6px; height: 15px;">
                            </td>
                        <td style="width: 278px" align="left" rowspan="2" valign="top">
                           <asp:TextBox ID="txtRemarks" runat="server" Width="99%" TextMode="MultiLine" TabIndex="12"></asp:TextBox>
                        </td>
                        <td style="width: 78px">
                        </td>
                        <td align="right" style="width: 161px; height: 15px;">
                            <asp:Label ID="Label33" runat="server" Text="Total Payment"></asp:Label>
                        </td>
                        <td style="width: 21px; height: 15px;">
                            </td>
                        <td align="right" style="height: 15px">
                          <asp:TextBox ID="txtPayment" runat="server" Width="100px" style="text-align:right;"  onChange="Due();" onfocus="this.select();" TabIndex="7"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 174px; height: 16px;" align="right">
                            </td>
                        <td style="width: 6px; height: 16px;">
                            </td>
                        <td style="width: 78px">
                        </td>
                        <td align="right" style="width: 161px; height: 16px;">
                            <asp:Label ID="Label39" runat="server" Text="Due"></asp:Label>
                        </td>
                        <td style="width: 21px; height: 16px;">
                            </td>
                        <td align="right" style="height: 16px">
                            <asp:TextBox ID="txtDue" runat="server" align="right" style="text-align:right;" 
                                TabIndex="8" Width="100px"></asp:TextBox>
                        </td>
                    </tr>
                    </table>
                    </fieldset>  
            </td>            
        </tr>        
    </table>
                         </td>
                     </tr>
                 </table>
             </td></tr>
             </table>  </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>
 </td>
<td>&nbsp;</td>
</tr>
<tr>
<td style="width:1%;">&nbsp;</td>
<td align="left" colspan="2">
    
  <asp:GridView CssClass="mGrid" PagerStyle-CssClass="pgr"  
        AlternatingRowStyle-CssClass="alt" ID="dgSVMst" runat="server" 
        AutoGenerateColumns="False"  BackColor="White" BorderWidth="1px" BorderStyle="Solid"
        CellPadding="2" BorderColor="LightGray" Font-Size="9pt" PageSize="30" 
        Width="100%" AllowPaging="True" onrowdatabound="dgSVMst_RowDataBound" 
        onselectedindexchanged="dgSVMst_SelectedIndexChanged" 
        onpageindexchanging="dgSVMst_PageIndexChanging" >
  <HeaderStyle Font-Size="9pt"  Font-Bold="True" BackColor="LightGray" HorizontalAlign="center"  ForeColor="Black" />
  <Columns>  
  <asp:CommandField ShowSelectButton="True"  ItemStyle-Width="40px" 
          ItemStyle-HorizontalAlign="Center">
<ItemStyle HorizontalAlign="Center" Width="40px"></ItemStyle>
      </asp:CommandField>
  <asp:BoundField  HeaderText="Invoice No"  DataField="InvoiceNo" ItemStyle-Width="100px" 
          ItemStyle-HorizontalAlign="Center">    
<ItemStyle HorizontalAlign="Center" Width="100px"></ItemStyle>
      </asp:BoundField>
  <asp:BoundField  HeaderText="Invoice Date"  DataFormatString="{0:dd/MM/yyyy}" ItemStyle-Width="100px" 
          ItemStyle-HorizontalAlign="Center" DataField="OrderDate">
          <ItemStyle HorizontalAlign="Center" Width="100px"></ItemStyle>
      </asp:BoundField>
     
  <asp:BoundField  HeaderText="Received Amount" 
          ItemStyle-Height="20" ItemStyle-Width="80px" 
          ItemStyle-HorizontalAlign="Right" DataField="CashReceived">
<ItemStyle HorizontalAlign="Right" Height="20px" Width="80px"></ItemStyle>
      </asp:BoundField>
     
      <asp:BoundField  HeaderText="ID" DataField="ID"  ItemStyle-Width="80px" 
          ItemStyle-HorizontalAlign="Left" > 
<ItemStyle HorizontalAlign="Left" Width="80px"></ItemStyle>
      </asp:BoundField>
  </Columns>
                        <RowStyle BackColor="White" />
                        <SelectedRowStyle BackColor="" Font-Bold="True" />
                        <PagerStyle BackColor="LightGray" ForeColor="Black" HorizontalAlign="Center" />
                        <AlternatingRowStyle BackColor="" />
</asp:GridView>

</td>
<td>&nbsp;</td>
</tr>
<tr>
<td style="width:1%;">&nbsp;</td>
<td style="width:41%;" align="center"> 
    &nbsp;</td>
<td style="width:62%;">&nbsp;</td>
<td>&nbsp;</td>
</tr>
<tr>
<td style="width:1%;">&nbsp;</td>
<td align="center" colspan="2">
 <asp:Panel ID="pnlClient" runat="server" CssClass="modalPopup1" Style="padding:15px 15px 15px 15px; display:none; background-color:White; border:1px solid black;" Width="700px">
  <fieldset style="vertical-align: top; border: solid 1px #8BB381;">
                        <legend style="color: maroon;"><b>Save Data</b></legend>     
<table style="width:100%;">
<tr>
<td style="width:15%; height: 5px;" align="left">
    </td>
<td style="width:16%; height: 5px;" align="right"> 
    </td>
<td style=" width:4%; height: 5px;" ></td>
<td style="width:41%; height: 5px;" align="left">
    
</td>
<td style="width:25%; height: 5px;" align="left" > 
    </td>
</tr>
    <tr>
        <td align="left" style="width:15%;">
            &nbsp;</td>
        <td align="right" style="width:16%;">
            <asp:Label ID="Label6" runat="server" Text="Name"></asp:Label>
        </td>
        <td style=" width:4%;">
            &nbsp;</td>
        <td align="left" style="width:41%;">
            <asp:TextBox ID="txtvalue" runat="server" Width="100%"></asp:TextBox>
        </td>
        <td align="left" style="width:25%;">
            &nbsp;</td>
    </tr>
    <tr>
        <td align="left" style="width:15%;">
            &nbsp;</td>
        <td align="right" style="width:16%;">
            <asp:Label ID="Label38" runat="server" Text="Mobile"></asp:Label>
        </td>
        <td style=" width:4%;">
            &nbsp;</td>
        <td align="left" style="width:41%;">
            <asp:TextBox ID="txtMobile" runat="server" Width="100%"></asp:TextBox>
        </td>
        <td align="left" style="width:25%;">
            &nbsp;</td>
    </tr>
    <tr>
        <td align="left" style="width:15%;">
            &nbsp;</td>
        <td align="right" style="width:16%;">
            <asp:Label ID="Label7" runat="server" Text="E-mail"></asp:Label>
        </td>
        <td style=" width:4%;">
            &nbsp;</td>
        <td align="left" style="width:41%;">
            <asp:TextBox ID="txtEmail" runat="server" Width="100%"></asp:TextBox>
        </td>
        <td align="left" style="width:25%;">
            &nbsp;</td>
    </tr>
    <tr>
        <td align="left" style="width:15%;">
            &nbsp;</td>
        <td align="right" style="width:16%;">
            &nbsp;</td>
        <td style=" width:4%;">
            &nbsp;</td>
        <td align="left" style="width:41%;">
            &nbsp;</td>
        <td align="left" style="width:25%;">
            &nbsp;</td>
    </tr>
<tr>
<td style="width:100%;" colspan="5">
<table style="width:100%;">
  <tr>  
  <td style="width:5%;"></td>
  <td align="right" >
       <asp:Button ID="btnClientSave" runat="server" ToolTip="OK" 
           OnClientClick="HideModalDiv();" Text="OK" 
        Height="30px" Width="100px" BorderStyle="Outset"  Font-Size="9pt" onclick="btnClientSave_Click" 
            />
       </td>   
       <td style="width:20px;"></td>
       <td align="left" >
       <asp:Button ID="btnClientQuit" runat="server" ToolTip="Quit Client" Text="Quit" OnClientClick="HideModalDiv();"
        Height="30px" Width="100px" BorderStyle="Outset"  Font-Size="9pt" />
       </td>        
       <td style="width:5%;"></td>       
   </tr>
   </table>
</td>
</tr>
</table>   
</fieldset> 
    </asp:Panel>

</td>
<td>&nbsp;</td>
</tr>
<tr>
<td style="width:1%;">&nbsp;</td>
<td style="width:41%;" align="center"> 
    &nbsp;</td>
<td style="width:62%;">&nbsp;</td>
<td>&nbsp;</td>
</tr>
</table>
</div>
<div id="divBackground" style="position: fixed; z-index: 999; height: 100%; width: 100%; top: 0; left: 0; background-color: Black; filter: alpha(opacity=60); opacity: 0.6; -moz-opacity: 0.8; -webkit-opacity: 0.8; display: none">
</div>
</asp:Content>

