<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="frmProduction.aspx.cs" Inherits="frmProduction" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <script src='<%= ResolveUrl("~/Scripts/valideDate.js") %>' type="text/javascript"></script>
<script language="javascript" type="text/javascript" >
    function OpenWindow(Url) {
        var testwindow = window.open(Url, '', 'width=600px,height=400px,top=100,left=300,scrollbars=1');
    }

    function setValueItem(item, iname, msr, rate) {
        $('input:text[id$=txtItemCode]').val(item);
        $('input:text[id$=txtQnty]').focus();
    }

    function remLink() {
        if (window.testwindow && window.testwindow.open && !window.testwindow.closed)
            window.testwindow.opener = null;
    }
    function IsEmpty(aTextField) {
        if ((aTextField.value.length == 0) || (aTextField.value == null)) {
            return true;
        }
        else {
            return false;
        }
    }
    function onListPopulated() {
        var completionList = $find("AutoCompleteEx").get_completionList();
        completionList.style.width = 'auto';
    }

   
   

    function Remarks() {
        
        var txtRemarks = document.getElementById("<%=txtRemarks.ClientID %>");
        txtRemarks.focus();
    }

    function setDecimal(abc) {
        var dt = document.getElementById(abc).value;
        if (dt.length > 0) {
            document.getElementById(abc).value = parseFloat(dt).toFixed(2);
        }
    }
    function isNumber(evt) {
        evt = (evt) ? evt : window.event;
        var charCode = (evt.which) ? evt.which : evt.keyCode;
        if (charCode != 46 && charCode > 31
            && (charCode < 48 || charCode > 57)) {
            return false;
        }
        return true;
    }
    	
</script>    
    <script language="javascript" type="text/javascript" >
        function isNumber(evt) {
            evt = (evt) ? evt : window.event;
            var charCode = (evt.which) ? evt.which : evt.keyCode;
            if (charCode != 46 && charCode > 31
            && (charCode < 48 || charCode > 57)) {
                return false;
            }
            return true;
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
                    <asp:Button ID="btnDelete" runat="server" ToolTip="Delete" onclick="Delete_Click"
            
                onclientclick="javascript:return window.confirm('are u really want to delete these data')" Text="Delete" 
        Height="35px" Width="110px" BorderStyle="Outset"  />
                </td>
                <td >
            &nbsp;</td>
                <td align="center" >
                    <asp:Button ID="btnSave" OnClientClick="this.disabled=true;" UseSubmitBehavior="false" runat="server" ToolTip="Save Purchase Record" 
                onclick="btnSave_Click" Text="Save" 
        Height="35px" Width="110px" BorderStyle="Outset"  />
                </td>
                <td align="center" >
                    <asp:Button ID="btnNew" runat="server" ToolTip="New" onclick="btnNew_Click"  Text="New" 
        Height="35px" Width="110px" BorderStyle="Outset"  />
                </td>
                <td align="center" >
                    <asp:Button ID="btnClear" runat="server"  ToolTip="Clear" onclick="Clear_Click" Text="Clear" 
        Height="35px" Width="110px" BorderStyle="Outset"  />
                </td>
                <td align="center" >
                    <asp:Button ID="btnPrint" runat="server" ToolTip="Print PO" Text="Print" 
        Height="35px" Width="110px" BorderStyle="Outset" onclick="btnPrint_Click" 
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
                                <legend style="color: maroon;"><b>Production Setup</b> </legend>
                                <table id="Table1" style="width:100%"  cellpadding="2">
                                    <tr >
                                        <td style="width: 15%; height: 20px;" align="left">
                                            <asp:Label ID="lblPurNo" runat="server" Font-Size="9pt">Production No</asp:Label>
                                        </td>
                                        <td style="width: 29%; height: 20px;" align="left">
                                            <asp:TextBox SkinId="tbPlain" ID="txtProductionNo" runat="server" Width="60%"  
                                                CssClass="tbc" TabIndex="0"
            AutoPostBack="False"  Font-Size="8pt"></asp:TextBox>
                                        </td>
                                        <td style=" width:7%;">
                                            <asp:TextBox ID="txtID" runat="server" AutoPostBack="False" CssClass="tbc" 
            Font-Size="8pt" SkinId="tbPlain" Width="60%" Visible="False"></asp:TextBox>
                                        </td>
                                        <td style="width: 16%; height: 20px;" align="left">
                                            <asp:Label ID="lblPurDate" runat="server" Font-Size="9pt">Receive Date</asp:Label>
                                        </td>
                                        <td style="width: 25%; height: 20px;" align="left">
                                            <asp:TextBox SkinId="tbPlain" ID="txtReceiveDate" runat="server" Width="70%"  
            placeholder="dd/MM/yyyy" TabIndex="1"
            AutoPostBack="False"  Font-Size="8pt"></asp:TextBox>
                                            <ajaxtoolkit:calendarextender runat="server" ID="txtReceiveDate_CalendarExtender" 
            TargetControlID="txtReceiveDate" Format="dd/MM/yyyy"/>
                                        </td>
                                    </tr>
                                    <tr >
                                        <td style="width: 15%; " align="left">
                                            <asp:Label ID="lblRemarks" runat="server" Font-Size="9pt">Remark&#39;s</asp:Label>
                                        </td>
                                        <td style="width: 29%; " align="left">
                                            <asp:TextBox ID="txtRemarks" runat="server" AutoPostBack="true" CssClass="tbc" 
                                                Font-Size="8pt" SkinId="tbPlain" style="text-align:left;" TabIndex="9" 
                                                Width="97%"></asp:TextBox>
                                        </td>
                                        <td style=" width:7%;">
                                            <asp:Label ID="lblOrNo" runat="server" Font-Size="9pt" Visible="False"></asp:Label>
                                        </td>
                                        <td style="width: 16%; " align="left">
                                            <asp:Label ID="Label40" runat="server" Text="Batch No"></asp:Label>
                                        </td>
                                        <td style="width: 25%; " align="left">
                                            <asp:TextBox ID="txtBatchNo" runat="server" AutoPostBack="False" 
                                                Font-Size="8pt" SkinId="tbPlain" TabIndex="1" 
                                                Width="70%"></asp:TextBox>
                                            
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
                                    <asp:GridView ID="dgProMst" runat="server" AllowPaging="True" 
                        AllowSorting="True" AlternatingRowStyle-CssClass="alt" 
                        AutoGenerateColumns="False" BackColor="White" BorderColor="LightGray" 
                        BorderStyle="Solid" BorderWidth="1px" CellPadding="2" CssClass="mGrid" 
                        Font-Size="9pt" onpageindexchanging="dgProMst_PageIndexChanging" 
                        onrowdatabound="dgProMst_RowDataBound" 
                        onselectedindexchanged="dgProMst_SelectedIndexChanged" 
                        PagerStyle-CssClass="pgr" PageSize="30" Width="100%">
                                        <Columns>
                                            <asp:CommandField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="40px" 
                                ShowSelectButton="True">
                                            <ItemStyle HorizontalAlign="Center" Width="5%" />
                                            </asp:CommandField>
                                            <asp:BoundField DataField="ID" HeaderText="ID" ItemStyle-Height="20" 
                                                ItemStyle-Width="100px">
                                            <ItemStyle Height="20px" Width="5%" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="PN" HeaderText="Produc No" 
                                ItemStyle-HorizontalAlign="Center" ItemStyle-Width="180px">
                                            <ItemStyle HorizontalAlign="Center" Width="15%" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="ReceiveDate" 
                                HeaderText="Receive  Date" ItemStyle-HorizontalAlign="Center" 
                                ItemStyle-Width="90px">
                                            <ItemStyle HorizontalAlign="Center" Width="10%" />
                                            </asp:BoundField>
                                            <asp:BoundField HeaderText="Batch" DataField="BatchNo">
                                                    <ItemStyle HorizontalAlign="Center" Width="10%" />
                                            </asp:BoundField>
                                            <asp:BoundField HeaderText="Remark" DataField="Remarks">
                                                    <ItemStyle HorizontalAlign="Left" Width="20%" />
                                            </asp:BoundField>
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
<%--<div runat="server" id="PVDetails">--%>
                    <asp:Panel ID="pnlVch" runat="server" Width="100%">
                        <div style="font-size: 8pt;" align="center">
                            <asp:UpdatePanel ID="PVIesms_UP" runat="server"  UpdateMode="Conditional">
                                <ContentTemplate>
                                    <ajaxtoolkit:TabContainer ID="tabVch" runat="server" Width="99%" ActiveTabIndex="0" Font-Size="8pt">
                                        <ajaxtoolkit:TabPanel ID="tpVchDtl" runat="server" HeaderText="Items Details">
                                            <ContentTemplate>
                                                <table style="width:100%;">
                                                    <tr>
                                                        <td colspan="2" align="center">
                                                            <asp:GridView ID="dgProductionDtl" runat="server" AutoGenerateColumns="False" 
                                                                BorderColor="LightGray" CssClass="mGrid" Font-Size="9pt"  
    OnRowDataBound="dgProDtl_RowDataBound" OnRowDeleting="dgProDtl_RowDeleting" Width="100%">
                                                                <AlternatingRowStyle CssClass="alt" />
                                                                <Columns>
                                                                    <asp:TemplateField>
                                                                        <ItemTemplate>
                                                                            <asp:ImageButton ID="lbDelete" runat="server" CausesValidation="False" CommandName="Delete" ImageAlign="Middle" ImageUrl="~/img/delete.png" Text="Delete" />
                                                                        </ItemTemplate>
                                                                        <ItemStyle Font-Size="8pt" HorizontalAlign="Center" Width="4%" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Code">
                                                                        <ItemTemplate>
                                                                            <asp:TextBox ID="txtItemCode" runat="server" AutoPostBack="true" 
                                                                                Enabled="False" Font-Size="8pt" MaxLength="15" 
                SkinId="tbPlain" Text='<%#Eval("item_code")%>' Width="80%" onFocus="this.select()" ReadOnly="True"></asp:TextBox>
                                                                        </ItemTemplate>
                                                                        <FooterStyle HorizontalAlign="Center" />
                                                                        <ItemStyle HorizontalAlign="Center" Width="8%" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Description">
                                                                        <ItemTemplate>
                                                                            <asp:TextBox ID="txtItemDesc" runat="server" autocomplete="off" AutoPostBack="true" Font-Size="8pt" OnTextChanged="txtItemDesc_TextChanged" 
                SkinId="tbPlain" Text='<%#Eval("item_desc")%>' Width="95%" onFocus="this.select()">
                </asp:TextBox>
                                                                            <ajaxToolkit:AutoCompleteExtender ServicePath="AutoComplete.asmx" 
                                                                                runat="server" ID="autoComplete2" TargetControlID="txtItemDesc"
                ServiceMethod="GetFGAndCMNItems" MinimumPrefixLength="1" CompletionInterval="100" EnableCaching="true" CompletionSetCount="12"/>
                                                                        </ItemTemplate>
                                                                        <ItemStyle HorizontalAlign="Left" Width="20%" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="M.Unit">
                                                                        <ItemTemplate>
                                                                            <asp:DropDownList ID="ddlMeasure" runat="server" AutoPostBack="true" 
                                                                                DataSource="<%#PopulateMeasure()%>" DataTextField="Name" DataValueField="ID" Font-Size="8pt" 
                SelectedValue='<%#Eval("msr_unit_code")%>' SkinId="ddlPlain" Width="95%" Height="26px" 
                                                                                onselectedindexchanged="ddlMeasure_SelectedIndexChanged">
                                                                            </asp:DropDownList>
                                                                        </ItemTemplate>
                                                                        <ItemStyle HorizontalAlign="Center" Width="8%" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Item Rate">
                                                                        <ItemTemplate>
                                                                            <asp:TextBox ID="txtItemRate" runat="server" AutoPostBack="True" CssClass="tbr" 
                        Font-Size="8pt" SkinId="tbPlain" Text='<%#Eval("item_rate")%>' Width="90%" 
                        onFocus="this.select()" ReadOnly="True"></asp:TextBox>
                                                                        </ItemTemplate>
                                                                        <ItemStyle HorizontalAlign="Right" Width="8%" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Quantity">
                                                                        <ItemTemplate>
                                                                            <asp:TextBox ID="txtQnty" runat="server" AutoPostBack="true" CssClass="tbc" Font-Size="8pt" OnTextChanged="txtQnty_TextChanged" SkinId="tbPlain" 
                Text='<%#Eval("qnty")%>' Width="90%" onkeypress="return isNumber(event)" onFocus="this.select()">
                </asp:TextBox>
                                                                        </ItemTemplate>
                                                                        <ItemStyle HorizontalAlign="Center" Width="8%" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Expire Date">
                                                                        <ItemTemplate>
                                                                            
                  
                                                                            <asp:TextBox ID="txtExpireDate" runat="server" CssClass="tbc" Font-Size="8pt" 
                                                                                OnTextChanged="txtExpireDate_TextChanged" SkinId="tbPlain" 
                Text='<%#Eval("ExpireDate")%>'  Width="90%" AutoPostBack="True"></asp:TextBox>
                                                                           <ajaxToolkit:CalendarExtender runat="server" ID="CalanderExpire" Format="dd/MM/yyyy" TargetControlID="txtExpireDate"></ajaxToolkit:CalendarExtender> 
                                                                            
                                                                        </ItemTemplate>
                                                                        <FooterStyle HorizontalAlign="Center" />
                                                                        <ItemStyle HorizontalAlign="Center" Width="9%" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Total">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblAddTotal" runat="server" Font-Size="8pt" Width="95%"></asp:Label>
                                                                        </ItemTemplate>
                                                                        <FooterStyle HorizontalAlign="Center" />
                                                                        <ItemStyle HorizontalAlign="Right" Width="8%" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="ID">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblid" runat="server" Font-Size="8pt" Width="95%" Text='<%#Eval("ID")%>'>
            </asp:Label>
                                                                        </ItemTemplate>
                                                                        <FooterStyle HorizontalAlign="Center" />
                                                                        <ItemStyle HorizontalAlign="Right" Width="10%" />
                                                                    </asp:TemplateField>
                                                                     <asp:TemplateField HeaderText="DtlID">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblFGItemID" runat="server" Font-Size="8pt" Width="95%" Text='<%#Eval("ItemID")%>'>
            </asp:Label>
                                                                        </ItemTemplate>
                                                                        <FooterStyle HorizontalAlign="Center" />
                                                                        <ItemStyle HorizontalAlign="Right" Width="10%" />
                                                                    </asp:TemplateField>
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
                                        </ajaxtoolkit:TabPanel>
                                    </ajaxtoolkit:TabContainer>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </asp:Panel>
     <%--</div>--%>
                </td>
                <td style="width:1%;">
                    &nbsp;</td>
            </tr>
            <tr>
                <td style="width:1%;">
                    &nbsp;</td>
                <td style="width:98%; " align="center">
                  
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

