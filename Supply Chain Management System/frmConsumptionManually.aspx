<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="frmConsumptionManually.aspx.cs" Inherits="frmConsumptionManually" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">

    <div id="frmMainDiv" style="background-color:White; width:100%;">
        <table  id="pageFooterWrapper">
            <tr>
                <td align="center">
                    <asp:Button ID="btnDelete" runat="server" ToolTip="Delete" 
                        onclientclick="javascript:return window.confirm('are u really want to delete these data')" Text="Delete" 
        Height="30px" Width="110px" BorderStyle="Outset" onclick="btnDelete_Click"  />
                </td>
                <td >
            &nbsp;</td>
                <td align="center" >
                    <asp:Button ID="btnSave" OnClientClick="this.disabled=true;" UseSubmitBehavior="false" runat="server" ToolTip="Save Purchase Record" Text="Save" 
        Height="30px" Width="110px" BorderStyle="Outset" onclick="btnSave_Click"  />
                </td>
                <td align="center" >
                    <asp:Button ID="btnNew" runat="server" ToolTip="New" onclick="btnNew_Click"  Text="New" 
        Height="30px" Width="110px" BorderStyle="Outset"  />
                </td>
                <td align="center" >
                    <asp:Button ID="btnClear" runat="server"  ToolTip="Clear"  Text="Clear" 
        Height="30px" Width="110px" BorderStyle="Outset" onclick="btnClear_Click"  />
                </td>
                <td align="center" >
                    <asp:Button ID="btnPrint" runat="server" ToolTip="Print PO" Text="Print" 
        Height="30px" Width="110px" BorderStyle="Outset" onclick="btnPrint_Click" 
                        Visible="False"  />
                </td>
                <td align="center" >
                    &nbsp;</td>
            </tr>
        </table>
        <table style="width:99%;">
            <tr>
                <td style="width:1%; height: 2px;">
                </td>
                <td style="width:95%; height: 2px;" align="center">
                    <asp:Label ID="lblShiftmentID" runat="server" Visible="False"></asp:Label>
                </td>
                <td style="width:1%; height: 2px;">
                </td>
            </tr>
            <tr>
                <td style="width:1%;">
                </td>
                <td style="width:95%; " align="center">
                    <asp:UpdatePanel ID="PVI_UP" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <fieldset style="vertical-align: top; border: solid 1px #8BB381;text-align:left;">
                                <legend style="color: maroon;"><b>Consumption Manualy</b> </legend>
                                <table id="Table1" style="width:100%"  cellpadding="2">
                                    <tr >
                                        <td style="width: 15%; height: 20px;" align="left">
                                            <asp:Label ID="lblPurNo" runat="server" Font-Size="9pt">Consumption No</asp:Label>
                                        </td>
                                        <td style="width: 29%; height: 20px;" align="left">
                                            <asp:TextBox SkinId="tbPlain" ID="txtConsumptionNo" runat="server" Width="60%"  
                                                CssClass="tbc" TabIndex="0"
            AutoPostBack="False"  Font-Size="8pt" ReadOnly="True"></asp:TextBox>
                                        </td>
                                        <td style=" width:7%;">
                                            <asp:TextBox ID="txtID" runat="server" AutoPostBack="False" CssClass="tbc" 
            Font-Size="8pt" SkinId="tbPlain" Width="60%" Visible="False"></asp:TextBox>
                                            <asp:Label ID="lblmstID" runat="server" Font-Size="9pt" Visible="False"></asp:Label>
                                        </td>
                                        <td style="width: 16%; height: 20px;" align="left">
                                            <asp:Label ID="lblDeclareDate" runat="server" Font-Size="9pt"> Consumption Date</asp:Label>
                                        </td>
                                        <td style="width: 25%; height: 20px;" align="left">
                                            <asp:TextBox SkinId="tbPlain" ID="txtConsumptionDate" runat="server" Width="69%"  
            placeholder="dd/MM/yyyy" TabIndex="1"
            AutoPostBack="False"  Font-Size="8pt"></asp:TextBox>
                                            <ajaxtoolkit:calendarextender runat="server" ID="txtConsumptionDate_CalendarExtender" 
            TargetControlID="txtConsumptionDate" Format="dd/MM/yyyy"/>
                                        </td>
                                    </tr>
                                    <tr >
                                        <td style="width: 15%; " align="left">
                                            <asp:Label ID="lblRemarks" runat="server" Font-Size="9pt">Remark&#39;s</asp:Label>
                                        </td>
                                        <td align="left" colspan="4">
                                            <asp:TextBox ID="txtRemarks" runat="server" AutoPostBack="true" CssClass="tbc" 
                                                Font-Size="8pt" SkinId="tbPlain" style="text-align:left;" TabIndex="9" 
                                                Width="90%"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </fieldset>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
                <td style="width:1%;">
                </td>
            </tr>
            <tr>
                <td style="width:1%;">
                    &nbsp;</td>
                <td style="width:95%; " align="center">
                    <asp:Panel ID="PanelHistory" runat="server">
                        <table style="width: 100%">
                            <tr>
                                <td>
                                    <asp:GridView ID="dgMst" runat="server" AllowPaging="True" 
                        AllowSorting="True" AlternatingRowStyle-CssClass="alt" 
                        AutoGenerateColumns="False" BackColor="White" BorderColor="LightGray" 
                        BorderStyle="Solid" BorderWidth="1px" CellPadding="2" CssClass="mGrid" 
                        Font-Size="9pt" 
                        PagerStyle-CssClass="pgr" PageSize="30" Width="100%" onrowdatabound="dgMst_RowDataBound" 
                                        onselectedindexchanged="dgMst_SelectedIndexChanged">
                                        <Columns>
                                            <asp:CommandField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="50px" 
                                ShowSelectButton="True">
                                            <ItemStyle HorizontalAlign="Center" Width="190px" />
                                            </asp:CommandField>
                                            <asp:BoundField HeaderText="Consumption No" 
                                ItemStyle-HorizontalAlign="Center" ItemStyle-Width="190px" DataField="ID">
                                            <ItemStyle HorizontalAlign="Center" Width="220px" />
                                            </asp:BoundField>
                                            <asp:BoundField 
                                HeaderText="Declare  Date" ItemStyle-HorizontalAlign="Center" 
                                ItemStyle-Width="90px" DataField="ConsumptionDate">
                                            <ItemStyle HorizontalAlign="Center" Width="300px" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="ID" HeaderText="ID" ItemStyle-Height="20" 
                                ItemStyle-Width="100px">
                                            <ItemStyle Height="20px" Width="100px" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Remarks" HeaderText="Remarks" />
                                            
                                        </Columns>
                                        <RowStyle BackColor="White" />
                                        <SelectedRowStyle BackColor="" Font-Bold="True" />
                                        <PagerStyle BackColor="LightGray" ForeColor="Black" HorizontalAlign="Center" />
                                        <AlternatingRowStyle BackColor="" />
                                        <HeaderStyle Font-Size="9pt" />
                                    </asp:GridView>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </td>
                <td style="width:1%;">
                    &nbsp;</td>
            </tr>
            <tr>
                <td style="width:1%;">
                    &nbsp;</td>
                <td style="width:98%; " align="center">
<asp:Panel ID="pnlConMn" runat="server" Width="100%">
                        <div style="font-size: 8pt;" align="center">
                            <asp:UpdatePanel ID="UP2" runat="server"  UpdateMode="Conditional">
                                <ContentTemplate>
                                  
                                                <table style="width:100%;">
                                                    <tr>
                                                        <td colspan="2" align="center">
                                                            <asp:GridView ID="dgConsumptionManualyDtl" runat="server" AutoGenerateColumns="False" 
                                                                BorderColor="LightGray" CssClass="mGrid" Font-Size="9pt" Width="100%" 
                                                                onrowdatabound="dgConsumptionManualyDtl_RowDataBound" onrowdeleting="dgConsumptionManualyDtl_RowDeleting" 
                                                                >
                                                                <AlternatingRowStyle CssClass="alt" />
                                                                <Columns>
                                                                    <asp:TemplateField>
                                                                        <ItemTemplate>
                                                                            <asp:ImageButton ID="lbDelete" runat="server" CausesValidation="False" CommandName="Delete" ImageAlign="Middle" ImageUrl="~/img/delete.png" Text="Delete" />
                                                                        </ItemTemplate>
                                                                        <ItemStyle Font-Size="8pt" HorizontalAlign="Center" Width="6%" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Code">
                                                                        <ItemTemplate>
                                                                            <asp:TextBox ID="txtItemCode" runat="server" AutoPostBack="true" Enabled="False" Font-Size="8pt" MaxLength="15" 
                SkinId="tbPlain" Text='<%#Eval("item_code")%>' Width="80%" onFocus="this.select()">
                </asp:TextBox>
                                                                        </ItemTemplate>
                                                                        <FooterStyle HorizontalAlign="Center" />
                                                                        <ItemStyle HorizontalAlign="Center" Width="10%" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Description">
                                                                        <ItemTemplate>
                                                                            <asp:TextBox ID="txtItemDesc" runat="server" autocomplete="off" AutoPostBack="true" Font-Size="8pt" OnTextChanged="txtItemDesc_TextChanged" 
                SkinId="tbPlain" Text='<%#Eval("item_desc")%>' Width="95%" onFocus="this.select()">
                </asp:TextBox>
                                                                            <ajaxToolkit:AutoCompleteExtender ServicePath="AutoComplete.asmx" 
                                                                                runat="server" ID="autoComplete2" TargetControlID="txtItemDesc"
                ServiceMethod="GetRMAndCmnItem" MinimumPrefixLength="1" CompletionInterval="100" EnableCaching="true" CompletionSetCount="12"/>
                                                                        </ItemTemplate>
                                                                        <ItemStyle HorizontalAlign="Left" Width="25%" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="M.Unit">
                                                                        <ItemTemplate>
                                                                            <asp:DropDownList ID="ddlMeasure" runat="server" AutoPostBack="true" 
                                                                                DataSource="<%#PopulateMeasure()%>" DataTextField="Name" DataValueField="ID" Font-Size="8pt" 
                SelectedValue='<%#Eval("msr_unit_code")%>' SkinId="ddlPlain" Width="95%" Height="26px" 
                                                                                onselectedindexchanged="ddlMeasure_SelectedIndexChanged">
                                                                            </asp:DropDownList>
                                                                        </ItemTemplate>
                                                                        <ItemStyle HorizontalAlign="Center" Width="10%" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Item Rate">
                                                                        <ItemTemplate>
                                                                            <asp:TextBox ID="txtItemRate" runat="server" AutoPostBack="True" CssClass="tbr" 
                        Font-Size="8pt" SkinId="tbPlain" Text='<%#Eval("item_rate")%>' Width="90%" 
                        onFocus="this.select()" ReadOnly="True"></asp:TextBox>
                                                                        </ItemTemplate>
                                                                        <ItemStyle HorizontalAlign="Right" Width="10%" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Quantity">
                                                                        <ItemTemplate>
                                                                            <asp:TextBox ID="txtQnty" runat="server" AutoPostBack="true" CssClass="tbc" Font-Size="8pt" OnTextChanged="txtQnty_TextChanged" SkinId="tbPlain" 
                Text='<%#Eval("qnty")%>' Width="90%" onkeypress="return isNumber(event)" onFocus="this.select()">
                </asp:TextBox>
                                                                        </ItemTemplate>
                                                                        <ItemStyle HorizontalAlign="Center" Width="8%" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Total">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblAddTotal" runat="server" Font-Size="8pt" Width="95%"></asp:Label>
                                                                        </ItemTemplate>
                                                                        <FooterStyle HorizontalAlign="Center" />
                                                                        <ItemStyle HorizontalAlign="Right" Width="8%" />
                                                                    </asp:TemplateField>
                                                                  
                                                                    <asp:BoundField DataField="ItemID" HeaderText="Item ID" />
                                                                  
                                                                </Columns>
                                                                <HeaderStyle Font-Bold="True" Font-Size="9pt" ForeColor="White" />
                                                                <PagerStyle CssClass="pgr" ForeColor="White" HorizontalAlign="Center" />
                                                                <RowStyle BackColor="White" />
                                                            </asp:GridView>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                        </td>
                                                        <td>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td valign="top" >
&nbsp;</td>
                                                        <td valign="top">
&nbsp;</td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                        </td>
                                                        <td>
                                                        </td>
                                                    </tr>
                                                </table>
                                            
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </asp:Panel>
                </td>
                <td style="width:1%;">
                    &nbsp;</td>
            </tr>
            <tr>
                <td style="width:1%;">
                    &nbsp;</td>
                <td style="width:98%; " align="center">
                    <asp:Panel ID="pnlClient" runat="server" CssClass="modalPopup1" Style="padding:15px 15px 15px 15px; display:none; background-color:White; border:1px solid black;" Width="700px">
                        <fieldset style="vertical-align: top; border: solid 1px #8BB381;">
                            <legend style="color: maroon;"><b>Save Data</b></legend>
                            <table style="width:100%;">
                                <tr>
                                    <td style="width:15%;" align="left">
    &nbsp;</td>
                                    <td style="width:16%;" align="right">
                                        <asp:Label ID="Label3" runat="server" Text="Type"></asp:Label>
                                    </td>
                                    <td style=" width:4%;" >
                                        &nbsp;</td>
                                    <td style="width:41%;" align="left">
                                        <asp:DropDownList SkinID="ddlPlain" ID="ddlType" runat="server"  
                    Font-Size="8" Width="100%" TabIndex="2" Height="26px">
                                            <asp:ListItem></asp:ListItem>
                                            <asp:ListItem Value="S">Supplier</asp:ListItem>
                                            <asp:ListItem Value="P">Party</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td style="width:25%;" align="left" >
    &nbsp;</td>
                                </tr>
                                <tr>
                                    <td style="width:15%;" align="left">
    &nbsp;</td>
                                    <td style="width:16%;" align="right">
                                        <asp:Label ID="Label4" runat="server" Text="Name"></asp:Label>
                                    </td>
                                    <td style=" width:4%;" >
                                        &nbsp;</td>
                                    <td style="width:41%;" align="left">
                                        <asp:TextBox ID="txtvalue" runat="server" Width="100%"></asp:TextBox>
                                    </td>
                                    <td style="width:25%;" align="left" >
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
                                        <asp:Label ID="Label39" runat="server" Text="E-mail"></asp:Label>
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
                                                <td style="width:5%;">
                                                </td>
                                                <td align="right" >
                                                    <asp:Button ID="btnClientSave" runat="server" ToolTip="OK" 
           OnClientClick="HideModalDiv();" Text="OK" 
        Height="30px" Width="100px" BorderStyle="Outset"  Font-Size="9pt" 
            />
                                                </td>
                                                <td style="width:20px;">
                                                </td>
                                                <td align="left" >
                                                    <asp:Button ID="btnClientQuit" runat="server" ToolTip="Quit Client" Text="Quit" 
        Height="30px" Width="100px" BorderStyle="Outset"  Font-Size="9pt" />
                                                </td>
                                                <td style="width:5%;">
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </fieldset>
                    </asp:Panel>
                </td>
                <td style="width:1%;">
                    &nbsp;</td>
            </tr>
        </table>
    </div>
    <div id="divBackground" style="position: fixed; z-index: 999; height: 100%; width: 100%;
        top: 0; left:0; background-color: Black; filter: alpha(opacity=60); opacity: 0.6; -moz-opacity: 0.8;-webkit-opacity: 0.8;display:none">
    </div>
</asp:Content>

