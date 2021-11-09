<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="rptProductin.aspx.cs" Inherits="rptProductin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <div id=id="frmMainDiv" style="background-color:White; width:100%;">
 <table  id="pageFooterWrapper">
            <tr>
               
                <td >
            &nbsp;</td>
              <td align="center" >
                </td>
                <td align="center" >
                    <asp:Button ID="btnPrint" runat="server" ToolTip="Print PO" Text="Print" 
        Height="30px" Width="110px" BorderStyle="Outset" onclick="btnPrint_Click"  />
                </td>
              
                <td align="center" >
                    <asp:Button ID="btnClear" runat="server"  ToolTip="Clear" onclick="Clear_Click" Text="Clear" 
        Height="30px" Width="110px" BorderStyle="Outset"  />
                </td>
               
                <td align="center" >
                    &nbsp;</td>
            </tr>
        </table>
    <table style="width: 100%">
        <tr>
            <td style="width: 10%">
                &nbsp;</td>
            <td style="width: 70%">
                &nbsp;</td>
            <td style="width:20%">
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width: 10%">
                &nbsp;</td>
            <td style="width: 70%">
            <fieldset style="border: solid 1px #8BB381;"><legend>Production Report</legend>
                <table style="width: 100%">
                    <tr>
                        <td style="width: 20%" align="right">
                            &nbsp;</td>
                        <td style="width: 5%">
                            &nbsp;</td>
                        <td style="width: 70%">
                            <asp:DropDownList ID="ddlReportType" runat="server" Width="99%" 
                                onselectedindexchanged="ddlReportType_SelectedIndexChanged" 
                                AutoPostBack="True">
                                <asp:ListItem Value="All">All Production Report</asp:ListItem>
                                <asp:ListItem Value="UP">Unit Price</asp:ListItem>
                                <asp:ListItem Value="ED">Expire Date</asp:ListItem>
                                <asp:ListItem Value="CD">Create Date</asp:ListItem>
                                <asp:ListItem Value="RD">Receive Date</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td style="width: 5%; font-weight: 700; color: #CC0000;">
                            *</td>
                    </tr>
                    <tr>
                        <td style="width: 20%" align="right">
                            <asp:Label ID="lblUnitPriceFrom" runat="server" Text=" Unite Price"></asp:Label>
                        </td>
                        <td style="width: 5%">
                            &nbsp;</td>
                        <td style="width: 70%">
                            <table style="width: 100%">
                                <tr>
                                    <td style="width: 35%">
                                        <asp:TextBox ID="txtUnitePriceFrom" runat="server" Width="90%"></asp:TextBox>
                                    </td>
                                    <td align="right" style="width: 23%">
                                        <asp:Label ID="lblUnitePriceTo" runat="server" Text="To"></asp:Label>
                                    </td>
                                    <td style="width: 7%">
                                        &nbsp;</td>
                                    <td style="width: 35%">
                                        <asp:TextBox ID="txtUnitePriceTo" runat="server" Width="90%"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td style="width: 5%">
                            &nbsp;</td>
                    </tr>
                     <tr>
                        <td style="width: 20%" align="right">
                            <asp:Label ID="lblFromDate" runat="server" Text="Receive Date From"></asp:Label>
                        </td>
                        <td style="width: 5%">
                            &nbsp;</td>
                        <td style="width: 70%">
                            <table style="width: 100%">
                                <tr>
                                    <td style="width: 35%">
                                        <asp:TextBox ID="txtfromdate" runat="server" Width="90%"></asp:TextBox>

                                        <ajaxToolkit:CalendarExtender ID="Calander1" runat="server" Format="dd/MM/yyyy" TargetControlID="txtfromdate"></ajaxToolkit:CalendarExtender>

                                    </td>
                                    <td align="right" style="width: 23%">
                                        <asp:Label ID="lblToDate" runat="server" Text="To Date"></asp:Label>
                                    </td>
                                    <td style="width: 7%">
                                        &nbsp;</td>
                                    <td style="width: 35%">
                                        <asp:TextBox ID="txtToDate" runat="server" Width="90%"></asp:TextBox>
                                         <ajaxToolkit:CalendarExtender ID="CalendarExtender1" runat="server" Format="dd/MM/yyyy" TargetControlID="txtToDate"></ajaxToolkit:CalendarExtender>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td style="width: 5%">
                            &nbsp;</td>
                    </tr>
                    <tr>
                        <td style="width: 20%" align="right">
                                        Production Code</td>
                        <td style="width: 5%">
                            &nbsp;</td>
                        <td style="width: 70%">
                            <table style="width: 100%">
                                <tr>
                                    <td style="width: 35%">
                                        <asp:TextBox ID="txtProductionCode" runat="server" Width="90%"></asp:TextBox>

                                        
                                        <ajaxToolkit:AutoCompleteExtender ServicePath="AutoComplete.asmx" 
                                                                                runat="server" ID="AutoCompleteExtender1" TargetControlID="txtProductionCode"
                ServiceMethod="GetGDandCmnItemCode" MinimumPrefixLength="1" CompletionInterval="100" EnableCaching="true" CompletionSetCount="12"></ajaxToolkit:AutoCompleteExtender>

                                    </td>
                                    <td style="width: 23%" align="right">
                                        Batch No</td>
                                    <td style="width: 7%">
                                        &nbsp;</td>
                                    <td style="width: 35%">
                                        <asp:TextBox ID="txtBatchNo" runat="server" Width="90%"></asp:TextBox>
                                    </td>
                                    
                                </tr>
                            </table>
                        </td>
                        <td style="width: 5%">
                            &nbsp;</td>
                    </tr>
                    <tr>
                        <td style="width: 20%; height: 19px;" align="right">
                                        &nbsp;</td>
                        <td style="width: 5%; height: 19px;">
                            </td>
                        <td style="width: 70%; height: 19px;">
                            <table style="width: 100%">
                                <tr>
                                    <td style="width: 35%">
                                        &nbsp;</td>
                                    <td align="right" style="width: 23%">
                                        &nbsp;</td>
                                    <td style="width: 7%">
                                        &nbsp;</td>
                                    <td style="width: 35%">
                                        <asp:TextBox ID="txtBrandName" runat="server" Width="90%" Visible="False"></asp:TextBox>
                                        <ajaxToolkit:AutoCompleteExtender ServicePath="AutoComplete.asmx" 
                                                                                runat="server" ID="autoComplete2" TargetControlID="txtBrandName"
                ServiceMethod="GetBrandName" MinimumPrefixLength="1" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="12"></ajaxToolkit:AutoCompleteExtender>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td style="width: 5%; height: 19px;">
                            </td>
                    </tr>
                    </table></fieldset>
            </td>
            <td style="width:20%">
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width: 10%">
                &nbsp;</td>
            <td style="width: 70%">
                
              
            </td>
            <td style="width:20%">
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width: 10%">
                &nbsp;</td>
            <td style="width: 70%">
                &nbsp;</td>
            <td style="width:20%">
                &nbsp;</td>
        </tr>
    </table>
    </div>
</asp:Content>

