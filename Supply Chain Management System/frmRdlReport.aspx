<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="frmRdlReport.aspx.cs" Inherits="frmRdlReport" %>

<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">

    <div>
      <div id="frmMainDiv" style="background-color:White; width:100%;">
    &nbsp;
    <table style="width: 100%">
        <tr>
            <td style="width: 5%"></td>
             <td style="width: 80%">
 <ajaxToolkit:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0" 
            Width="100%" style="font-weight: 700">
                        
                        <ajaxToolkit:TabPanel ID="TabPanel1" runat="server" HeaderText="TabPanel2">
                         <HeaderTemplate>
                               Report
                            </HeaderTemplate>
                         <ContentTemplate>

                         <table style="width: 100%">
                    <tr>
                        <td style="border-style: none; padding-top:10px; width: 28%" valign="top">
                            &nbsp;</td>
                       
                        <td style="width: 77%">
                        
                            &nbsp;</td>
                    </tr>
                    <tr>
                        <td style="width: 35%" valign="top">
                             <fieldset style="vertical-align: top; border: solid 1px #8BB381;text-align:left;">
                                 <legend style="color: maroon; font-weight: 700;">Select Report Type</legend>
                            <asp:RadioButtonList ID="rdbReportType" runat="server" 
                                onselectedindexchanged="rdbReportType_SelectedIndexChanged" Width="100%" 
                                AutoPostBack="True">
                                <asp:ListItem Value="pur" Selected="True">Purchase </asp:ListItem>
                                <asp:ListItem  Value="PReR">Purchase Return </asp:ListItem>
                                <asp:ListItem  Value="Cn">Consumption</asp:ListItem>                                
                                <%--<asp:ListItem Value="ITS">Item Status</asp:ListItem>--%>                                
                                <asp:ListItem  Value="Stk">Stock Report</asp:ListItem>
                                <asp:ListItem  Value="SL">Sales Report</asp:ListItem>
                                <asp:ListItem  Value="SLD">Sales Details Report</asp:ListItem>
                                 <asp:ListItem  Value="PL">Profit/ Loss (P&L)</asp:ListItem>
                            </asp:RadioButtonList>
                  </fieldset>
                        </td>
                       
                        <td style="width: 65%">
                        
                            <table style="width: 100%">
                               
                                <tr>
                                    <td align="left" valign="top" style="margin-left: 80px">
                                        <asp:Panel ID="pnlPurchase" runat="server" BorderStyle="None">
                                      <fieldset style="vertical-align: top; border: solid 1px #8BB381;text-align:left;"><legend style="color: maroon;">
                                            <asp:Label ID="lblPaneltext1" runat="server" style="font-weight: 700">Report Paremeter</asp:Label></legend>
                                            <table style="width: 100%">
                                                <tr>
                                                    <td align="right" style="width: 15%; height: 27px;">
                                                        </td>
                                                    <td style="width: 5%; height: 27px;">
                                                    </td>
                                                    <td style="width: 25%; height: 27px;">
                                                        &nbsp;</td>
                                                    <td align="right" style="width: 15%; height: 27px;" colspan="2">
                                                        <asp:Label ID="lblItemIDPurchase" runat="server" Visible="False"></asp:Label>
                                                    </td>
                                                    <td style="width: 25%; height: 27px;">
                                                    </td>
                                                </tr>
                                                
                                                <tr>
                                                    <td align="right" style="width: 15%; font-size: small; height: 27px;">
                                                        <asp:Label ID="lblSearchItemPur" runat="server" Text="  Search Item"></asp:Label></td>
                                                    <td style="width: 5%; height: 27px; font-weight: 700;" align="center">
                                                        &nbsp;</td>
                                                    <td colspan="4" style="height: 27px">
                                                        <asp:TextBox ID="txtSearchItemPur" runat="server" Width="96%" placeHolder="Search Items..."
                                                            ontextchanged="txtSearchItemPur_TextChanged" AutoPostBack="True"></asp:TextBox>

                                                        <ajaxToolkit:AutoCompleteExtender ID="autoComplete1" runat="server"  CompletionInterval="20"
                                        CompletionSetCount="12" 
                                        MinimumPrefixLength="1" ServiceMethod="GetItemList" 
                                        ServicePath="AutoComplete.asmx" TargetControlID="txtSearchItemPur" DelimiterCharacters="" 
                                                            Enabled="True" /> 
                                        <asp:Label ID="lblItemsIDPur" runat="server" style="display:none;"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="right" style="width: 15%; font-size: small; height: 27px;">
                                                         <asp:Label ID="lblSupplier" runat="server" Text=" Supplier"></asp:Label></td>
                                                    <td style="width: 5%; height: 27px; font-weight: 700;" align="center">
                                                        &nbsp;</td>
                                                    <td colspan="4" style="height: 27px">
                                                        <asp:DropDownList ID="ddlSupplierpur" runat="server" Height="26px" Width="98%">
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="right" style="width: 15%; font-size: small; height: 27px;">
                                                        <asp:Label ID="lblFromDatePur" runat="server" Text="Date From"></asp:Label>
                                                        &nbsp;</td>
                                                    <td style="width: 5%; height: 27px; font-weight: 700;" align="center">
                                                        &nbsp;</td>
                                                    <td style="width: 25%; height: 27px;">
                                                        <asp:TextBox ID="txtDatefromPur" placeHolder="dd/MM/yyyy" runat="server" Width="90%"></asp:TextBox>
                                                        <ajaxToolkit:CalendarExtender ID="ExtranderCalander" runat="server" 
                                                            format="dd/MM/yyyy" TargetControlID="txtDatefromPur" Enabled="True"></ajaxToolkit:CalendarExtender>
                                                    </td>
                                                    <td style="width: 15%; height: 27px;" align="right">
                                                       <asp:Label ID="lblDateTo" runat="server" Text=" Date To"></asp:Label></td>
                                                    <td style="width: 5%; height: 27px; font-weight: 700;" align="center">
                                                        &nbsp;</td>
                                                    <td style="width: 25%; height: 27px;">
                                                        <asp:TextBox ID="txtDateToPur" placeHolder="dd/MM/yyyy" runat="server" 
                                                            Width="89%"></asp:TextBox>
                                                         <ajaxToolkit:CalendarExtender ID="CalendarExtender1" runat="server" 
                                                            format="dd/MM/yyyy" TargetControlID="txtDateToPur" Enabled="True"></ajaxToolkit:CalendarExtender>
                                                   
                                                    </td>
                                                </tr>
                                            </table></fieldset>
                                        </asp:Panel>
                                        <asp:Panel ID="pnlOrderReceived" runat="server">
                                      <fieldset style="vertical-align: top; border: solid 1px #8BB381;text-align:left;"><legend style="color: maroon;">
                                            <asp:Label ID="lblpnlOrderrecived" runat="server" style="font-weight: 700">Report Paremeter</asp:Label></legend>
                                            <table style="width: 100%">
                                                <tr>
                                                    <td align="right" style="width: 16%; height: 22px;">
                                                        </td>
                                                    <td style="width: 5%; height: 22px;">
                                                    </td>
                                                    <td style="width: 25%; height: 22px;">
                                                        &nbsp;</td>
                                                    <td align="right" style="height: 22px;" colspan="2">
                                                        &nbsp;</td>
                                                    <td style="width: 25%; height: 22px;">
                                                        <asp:Label ID="lblItemIDORC" runat="server" Visible="False"></asp:Label>
                                                    </td>
                                                </tr>
                                                
                                                <tr>
                                                    <td align="right" style="width: 16%; font-size: small; height: 39px;">
                                                        <asp:Label ID="Label3" runat="server" Text="  Search Item"></asp:Label></td>
                                                    <td style="width: 5%; height: 39px; font-weight: 700;" align="center">
                                                        &nbsp;</td>
                                                    <td colspan="4" style="height: 39px">
                                                        <asp:TextBox ID="txtSearchItemORC" runat="server" Width="96%" placeHolder="Search Items..."
                                                            ontextchanged="txtSearchItemORC_TextChanged" AutoPostBack="True"></asp:TextBox>

                                                        <ajaxToolkit:AutoCompleteExtender ID="AutoCompleteExtender2" runat="server"  CompletionInterval="20"
                                        CompletionSetCount="12" 
                                        MinimumPrefixLength="1" ServiceMethod="GetItemList" 
                                        ServicePath="AutoComplete.asmx" TargetControlID="txtSearchItemORC" DelimiterCharacters="" 
                                                            Enabled="True" /> 
                                        <asp:Label ID="Label4" runat="server" style="display:none;"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="right" style="width: 16%; font-size: small; height: 27px;">
                                                        <asp:Label ID="lblFromDateOrC" runat="server" Text="Date From"></asp:Label>
                                                    </td>
                                                    <td style="width: 5%; height: 27px; font-weight: 700;" align="center">
                                                        &nbsp;</td>
                                                    <td style="width: 25%; height: 27px;">
                                                        <asp:TextBox ID="txtFromDateORC" placeHolder="dd/MM/yyyy" runat="server" 
                                                            Width="90%"></asp:TextBox>
                                                        <ajaxToolkit:CalendarExtender ID="CalendarExtender6" runat="server" 
                                                            format="dd/MM/yyyy" TargetControlID="txtFromDateORC" Enabled="True"></ajaxToolkit:CalendarExtender>
                                                    </td>
                                                    <td style="width: 15%; height: 27px;" align="right">
                                                       <asp:Label ID="lblToDateORC" runat="server" Text=" Date To"></asp:Label></td>
                                                    <td style="width: 5%; height: 27px; font-weight: 700;" align="center">
                                                        &nbsp;</td>
                                                    <td style="width: 25%; height: 27px;">
                                                        <asp:TextBox ID="txtToDateORC" placeHolder="dd/MM/yyyy" runat="server" 
                                                            Width="89%"></asp:TextBox>
                                                         <ajaxToolkit:CalendarExtender ID="CalendarExtender7" runat="server" 
                                                            format="dd/MM/yyyy" TargetControlID="txtToDateORC" Enabled="True"></ajaxToolkit:CalendarExtender>
                                                   
                                                    </td>
                                                </tr>
                                            </table></fieldset>
                                        </asp:Panel>
                                        
                                        <asp:Panel ID="pnlStock" runat="server">
                                        <fieldset style="vertical-align: top; border: solid 1px #8BB381;text-align:left;"><legend style="color: maroon;">Report Paremeter</legend>
                                            <table style="width: 100%">
                                                <tr>
                                                    <td align="right" style="width: 16%; height: 22px;">
                                                    </td>
                                                    <td style="width: 5%; height: 22px;">
                                                    </td>
                                                    <td style="width: 25%; height: 22px;">
                                                        &nbsp;</td>
                                                    <td align="right" colspan="2" style="height: 22px;">
                                                        &nbsp;</td>
                                                    <td style="width: 25%; height: 22px;">
                                                        <asp:Label ID="lblItemIDStk" runat="server" Visible="False"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="right" style="width: 16%; font-size: small;">
                                                        <asp:Label ID="Label5" runat="server" Text="  Search Item"></asp:Label>
                                                    </td>
                                                    <td align="center" style="width: 5%; font-weight: 700;">
                                                        &nbsp;</td>
                                                    <td colspan="4">
                                                        <asp:TextBox ID="txtSearchItemStk" runat="server" AutoPostBack="True" 
                                                            OnTextChanged="txtSearchItemStk_TextChanged" placeHolder="Search Items..." 
                                                            Width="86%" Height="22px"></asp:TextBox>
                                                        <ajaxToolkit:AutoCompleteExtender ID="txtSearchItemStk_AutoCompleteExtender" 
                                                            runat="server" CompletionInterval="20" CompletionSetCount="12" 
                                                            DelimiterCharacters="" Enabled="True" MinimumPrefixLength="1" 
                                                            ServiceMethod="GetItemList" ServicePath="AutoComplete.asmx" 
                                                            TargetControlID="txtSearchItemStk">
                                                        </ajaxToolkit:AutoCompleteExtender>
                                                        <asp:Label ID="Label6" runat="server" style="display:none;"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="right" style="width: 16%; font-size: small; height: 27px;">
                                                        <asp:Label ID="Label1" runat="server" Text="Catagory"></asp:Label>
                                                    </td>
                                                    <td align="center" style="width: 5%; height: 27px; font-weight: 700;">
                                                        &nbsp;</td>
                                                    <td colspan="4" style="height: 27px;">
                                                        <asp:DropDownList ID="ddlCatagory" runat="server" AutoPostBack="True" 
                                                            Font-Size="8pt" Height="25px" 
                                                            onselectedindexchanged="ddlCatagory_SelectedIndexChanged" SkinId="tbPlain" 
                                                            Width="87%">
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="right" style="width: 16%; font-size: small; height: 27px;">
                                                        <asp:Label ID="Label2" runat="server" Text="Sub Catagory"></asp:Label>
                                                    </td>
                                                    <td align="center" style="width: 5%; height: 27px; font-weight: 700;">
                                                        &nbsp;</td>
                                                    <td colspan="4" style="height: 27px;">
                                                        <asp:DropDownList ID="ddlSubCatagory" runat="server" Font-Size="8pt" 
                                                            Height="25px" SkinId="tbPlain" Width="87%">
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="right" style="width: 16%; font-size: small; height: 27px;">
                                                        &nbsp;</td>
                                                    <td align="center" style="width: 5%; height: 27px; font-weight: 700;">
                                                        &nbsp;</td>
                                                    <td style="width: 25%; height: 27px;">
                                                        &nbsp;</td>
                                                    <td align="right" style="width: 15%; height: 27px;">
                                                        &nbsp;</td>
                                                    <td align="center" style="width: 5%; height: 27px; font-weight: 700;">
                                                        &nbsp;</td>
                                                    <td style="width: 25%; height: 27px;">
                                                        &nbsp;</td>
                                                </tr>
                                            </table>  </fieldset>
                                        </asp:Panel>
                                      

                                        <asp:Panel ID="pnlITS" runat="server">
                                            <fieldset style="vertical-align: top; border: solid 1px #8BB381;text-align:left;">
                                                <legend style="color: maroon;">Report Paremeter</legend>
                                                <table style="width: 100%">
                                                    <tr>
                                                        <td align="right" style="width: 16%; height: 22px;">
                                                        </td>
                                                        <td style="width: 5%; height: 22px;">
                                                        </td>
                                                        <td style="width: 25%; height: 22px;">
                                                            &nbsp;</td>
                                                        <td align="right" colspan="2" style="height: 22px;">
                                                            &nbsp;</td>
                                                        <td style="width: 25%; height: 22px;">
                                                            <asp:Label ID="lblItemIDItemStatus" runat="server" Visible="False"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" style="width: 16%; font-size: small;">
                                                            <asp:Label ID="Label7" runat="server" Text=" Search Item"></asp:Label>
                                                        </td>
                                                        <td align="center" style="width: 5%; font-weight: 700;">
                                                            &nbsp;</td>
                                                        <td colspan="4">
                                                            <asp:TextBox ID="txtSearchItemITS" runat="server" AutoPostBack="True" 
                                                                Height="22px" OnTextChanged="txtSearchItemITS_TextChanged" 
                                                                placeHolder="Search By Items StyleNo, Code OR Name..." Width="86%"></asp:TextBox>
                                                            <ajaxToolkit:AutoCompleteExtender ID="txtSearchItemStk_AutoCompleteExtender0" 
                                                                runat="server" CompletionInterval="20" CompletionSetCount="12" 
                                                                DelimiterCharacters="" Enabled="True" MinimumPrefixLength="1" 
                                                                ServiceMethod="GetRMAndCmnItem" ServicePath="AutoComplete.asmx" 
                                                                TargetControlID="txtSearchItemITS">
                                                            </ajaxToolkit:AutoCompleteExtender>
                                                            <asp:Label ID="Label8" runat="server" style="display:none;"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" style="width: 16%; font-size: small; height: 27px;">
                                                            <asp:Label ID="Label9" runat="server" Text="Date From"></asp:Label>
                                                        </td>
                                                        <td align="center" style="width: 5%; height: 27px; font-weight: 700;">
                                                            &nbsp;</td>
                                                        <td style="height: 27px; width: 25%;">
                                                            <asp:TextBox ID="txtFromDateITS" runat="server" placeHolder="dd/MM/yyyy" 
                                                                Width="90%"></asp:TextBox>
                                                            <ajaxToolkit:CalendarExtender ID="CalendarExtender2" runat="server" 
                                                                Enabled="True" Format="dd/MM/yyyy" TargetControlID="txtFromDateITS">
                                                            </ajaxToolkit:CalendarExtender>
                                                        </td>
                                                        <td align="right" style="width: 15%; height: 27px;">
                                                            <asp:Label ID="Label10" runat="server" Text=" Date To"></asp:Label>
                                                        </td>
                                                        <td align="center" style="width: 5%; height: 27px; font-weight: 700;">
                                                            &nbsp;</td>
                                                        <td style="width: 25%; height: 27px;">
                                                            <asp:TextBox ID="txtToDateITS" runat="server" placeHolder="dd/MM/yyyy" 
                                                                Width="89%"></asp:TextBox>
                                                            <ajaxToolkit:CalendarExtender ID="CalendarExtender3" runat="server" 
                                                                Enabled="True" Format="dd/MM/yyyy" TargetControlID="txtToDateITS">
                                                            </ajaxToolkit:CalendarExtender>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" style="width: 16%; font-size: small; height: 27px;">
                                                            &nbsp;</td>
                                                        <td align="center" style="width: 5%; height: 27px; font-weight: 700;">
                                                            &nbsp;</td>
                                                        <td style="height: 27px; width: 25%;">
                                                            &nbsp;</td>
                                                        <td align="right" style="width: 15%; height: 27px;">
                                                            &nbsp;</td>
                                                        <td align="center" style="width: 5%; height: 27px; font-weight: 700;">
                                                            &nbsp;</td>
                                                        <td style="width: 25%; height: 27px;">
                                                            &nbsp;</td>
                                                    </tr>
                                                </table>
                                            </fieldset>
                                        </asp:Panel>
                                      

                                        <br />
                                    </td>
                                </tr>
                               
                                <tr>
                                    <td align="left">
                                        <table style="width: 100%">
                                            <tr>
                                                <td style="width: 35%" align="right">
                                                    

                                                    <asp:Button ID="btnPrint" runat="server" BorderStyle="Outset" BorderWidth="1px" 
                                                        Height="35px"  TabIndex="904" Text="Preview" 
                                                        ToolTip="Clear" Width="120px" onclick="btnPrint_Click" />
                                                    

                                                </td>
                                                <td style="width: 40%" align="center">
                                                    <asp:Button ID="btnInventoryClear" runat="server" BorderStyle="Outset" BorderWidth="1px" 
                                                        Height="35px" onclick="btnInventoryClear_Click" TabIndex="904" Text="Clear" 
                                                        ToolTip="Clear" Width="120px" />
                                                </td>
                                                <td style="width: 25%">
                                                    &nbsp;</td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                               
                                <tr>
                                    <td align="left" valign="top">
                                        &nbsp;</td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                        
                        </ajaxToolkit:TabContainer>
             </td>
              <td style="width: 5%"></td>
        </tr>
        
 
        
        <tr>
            <td style="width: 5%">&nbsp;</td>
             <td style="width: 80%" align="center">
                  <asp:Panel ID="Panel1" runat="server">
                      <rsweb:ReportViewer ID="rptOthersreportviewer" runat="server" Width="90%">
                      </rsweb:ReportViewer>
                   
                
                
                 </asp:Panel></td>
              <td style="width: 5%">&nbsp;</td>
        </tr>
        
 
        
        <tr>
            <td style="width: 5%">&nbsp;</td>
             <td style="width: 80%">
                 &nbsp;</td>
              <td style="width: 5%">&nbsp;</td>
        </tr>
        
 
        
    </table>
    
    </div>
    </div>
</asp:Content>

