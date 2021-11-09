<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="frmConsumption.aspx.cs" Inherits="frmConsumption" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">

<div id="frmMainDiv" style="background-color:White; width:100%;">

    a<table  id="pageFooterWrapper">
 <tr>
	<td align="center">
    
	    <asp:Button  ID="BtnDelete" runat="server"  ToolTip="Delete Record"   
            OnClick="BtnDelete_Click"  
            onclientclick="javascript:return window.confirm('are u really want to delete  these data')" Text="Delete" 
        Height="35px" Width="100px" BorderStyle="Outset"  />
	 </td>
	<td align="center">
    
	    &nbsp;</td>
    <td align="center">
        <asp:Button ID="BtnSave" OnClientClick="this.disabled=true;" UseSubmitBehavior="false" runat="server" ToolTip="Save or Update Record" 
            OnClick="BtnSave_Click" Text="Save"  
        Height="35px" Width="100px" BorderStyle="Outset"  /></td>
	
	<td align="center">
        <asp:Button ID="btnNew" runat="server" Text="New"  Height="35px" Width="100px" 
            BorderStyle="Outset" onclick="btnNew_Click" />
     </td>
	<td align="center">
        <asp:Button ID="BtnReset" runat="server" ToolTip="Clear Form" 
            OnClick="BtnReset_Click" Text="Clear" 
        Height="35px" Width="100px" BorderStyle="Outset"  /></td>
    <td align="center">
       <asp:Button ID="btnPrint" runat="server" ToolTip="Print" 
             Text="Print" 
        Height="35px" Width="100px" BorderStyle="Outset" Visible="False"  />
   </td>        
	</tr>		
</table>

    
    <table style="width: 100%">
        <tr>
            <td style="width: 10%; height: 15px;">
                </td>
            <td style="width: 80%; height: 15px;">
                                            <asp:TextBox ID="txtFGQuantity" runat="server" AutoPostBack="True" 
                                                CssClass="bcl" Height="25px" Width="5%" 
                    Visible="False"></asp:TextBox>
                                            <ajaxToolkit:AutoCompleteExtender ID="txtFGQuantity_AutoCompleteExtender" 
                                                runat="server" CompletionInterval="20" CompletionSetCount="12" 
                                                EnableCaching="true" MinimumPrefixLength="1" ServiceMethod="GetFGAndCmnItem" 
                                                ServicePath="AutoComplete.asmx" 
                    TargetControlID="txtFGQuantity" />
                                        </td>
                <td style="width: 10%; height: 15px;">
                </td>
            
        </tr>
        <tr>
            <td style="width: 10%">
                &nbsp;</td>
                <td style="width: 80%">
                    <asp:UpdatePanel ID="UP1" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                             <fieldset style="vertical-align: top; border: solid 1px #8BB381;text-align:left;">
                                <legend style="color: maroon;"><b>Consumption Setup</b> </legend>
                                <table style="width: 100%">
                                    <tr>
                                        <td align="right" style="width: 23%; font-weight: 700; height: 24px;">
                                            Consumption No</td>
                                        <td align="center" style="width: 2%; color: #FF3300; height: 24px;">
                                            </td>
                                        <td style="width: 30%; height: 24px;">
                                            <asp:TextBox ID="txtConsumptionNo" runat="server" AutoPostBack="False" CssClass="tbc" 
                                                Font-Size="8pt" SkinId="tbPlain" TabIndex="0" Width="50%" 
                                                ontextchanged="txtGRNO_TextChanged" ReadOnly="True" Height="18px"></asp:TextBox>
                                            <asp:Label ID="lblFinishGoodItemID" runat="server" Enabled="False" 
                                                Visible="False"></asp:Label>
                                            <asp:Label ID="lblMstID" runat="server" Visible="False"></asp:Label>
                                        </td>
                                        <td align="right" style="width: 15%; height: 24px;">
                                            Declare Date</td>
                                        <td align="center" style="width: 2%; color: #FF3300; height: 24px;">
                                            </td>
                                        <td style="width: 25%; height: 24px;">
                                            <asp:TextBox ID="txtDeclareDate" runat="server" CssClass="bcl" 
                                                placeholder="dd/mm/yyy" style="text-align: center;" Width="50%" 
                                                Height="18px"></asp:TextBox>
                                            <ajaxToolkit:CalendarExtender ID="txtDeclareDate_CalendarExtender" 
                                                runat="server" Format="dd/MM/yyyy" TargetControlID="txtDeclareDate">
                                            </ajaxToolkit:CalendarExtender>
                                        </td>
                                        <td style="width: 30%; height: 24px;">
                                            </td>
                                    </tr>
                                    <tr>
                                        <td align="right" style="width: 23%; height: 31px;">
                                            Finish Good &amp; Common Item</td>
                                        <td align="center" style="width: 2%; color: #FF3300; height: 31px;">
                                            *</td>
                                        <td style="height: 31px;" colspan="4">
                                            <asp:TextBox ID="txtFinishGoodItem" runat="server" AutoPostBack="True" 
                                                CssClass="bcl" ontextchanged="txtFinishGoodItem_TextChanged" 
                                                placeholder="Search By Code and Item Name" Width="83%" Height="18px"></asp:TextBox>
                                            <ajaxToolkit:AutoCompleteExtender ID="txtFinishGoodItem_AutoCompleteExtender" 
                                                runat="server" CompletionInterval="20" CompletionSetCount="12" 
                                                EnableCaching="true" MinimumPrefixLength="1" ServiceMethod="GetFGAndCmnItem" 
                                                ServicePath="AutoComplete.asmx" TargetControlID="txtFinishGoodItem" />
                                        </td>
                                        <td style="width: 30%; height: 31px;">
                                            </td>
                                    </tr>
                                    <tr>
                                        <td align="right" style="width: 23%; height: 27px;">
                                            Status</td>
                                        <td align="center" style="width: 2%; color: #FF3300; height: 27px;">
                                            </td>
                                        <td style="width: 30%; height: 27px;">
                                            <asp:DropDownList ID="ddlStatus" runat="server" Height="22px" Width="50%">
                                                <asp:ListItem Value="1">Active</asp:ListItem>
                                                <asp:ListItem Value="0">InActive</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td align="right" style="width: 15%; height: 27px;">
                                            <asp:Label ID="lblTotalCost" runat="server" Visible="False"></asp:Label>
                                            Total Cost</td>
                                        <td align="center" style="width: 2%; color: #FF3300; height: 27px;">
                                            *</td>
                                        <td style="width: 25%; height: 27px;">
                                            <asp:TextBox ID="txtTotalCost" runat="server" placeholder="0.00" style="text-align: right;"
                                               ReadOnly="True" Width="50%" Height="18px"></asp:TextBox></td>
                                        <td style="width: 30%; height: 27px;">
                                            </td>
                                    </tr>
                    </table>
                                               </fieldset></ContentTemplate>                   
                    </asp:UpdatePanel>
            </td>
            <td style="width: 10%"></td>
        </tr>
        <tr>
        <td style="width: 10%"></td>
        <td style="width: 80%"><asp:GridView ID="dgConsumptionMst" runat="server" AllowPaging="True" 
                     CssClass="mGrid" PageSize="30" 
                    style="text-align:center;" Width="100%" AutoGenerateColumns="False" 
                    onselectedindexchanged="dgConsumptionMst_SelectedIndexChanged" 
                onrowdatabound="dgConsumptionMst_RowDataBound">
                    <Columns>
                        <asp:CommandField ShowSelectButton="True">
                         <ItemStyle HorizontalAlign="Left" Width="3%" />
                        </asp:CommandField>
                        <asp:BoundField DataField="ID" HeaderText="ID" />
                           <asp:BoundField HeaderText="F.G. Item Namae" DataField="ItemNamae">
                        <ItemStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField HeaderText="Declare Date" DataField="DeclareDate" />
                        <asp:BoundField HeaderText="Status" DataField="StatusName" />
                        <asp:BoundField DataField="Status" HeaderText="StatusID" />
                    </Columns>
                </asp:GridView></td>
        <td style="width: 10%"></td>
        </tr>
    </table>

    <asp:Panel ID="PnlDtl" runat="server">
   
    <asp:UpdatePanel ID="UP2" runat="server" UpdateMode="Conditional">
                <ContentTemplate>

    <table style="width: 100%">
   
        <tr>
            <td style="width: 10%">
                &nbsp;</td>
            <td style="width: 80%" align="right">
            
            <fieldset style="vertical-align: top; border: solid 1px #8BB381;text-align:left;">
                                <legend style="color: maroon;"><b>Items Details</b> </legend>
           
                <table style="width: 100%">
                    <tr>
                        <td align="left" style="color: #FF3300" width="25%">
                            <asp:Label ID="Label7" runat="server" style="font-weight: 700; color: #000000;" 
                                Text="Raw Materials &amp; Common Item"></asp:Label>
                            &nbsp;*<asp:Label ID="lblRawMaterialsItemID" runat="server" style="color: #000000" 
                                Visible="False"></asp:Label>
                        </td>
                        <td align="left" style="width: 10%; font-weight: 700; color: #000000;" 
                            width="15%">
                            M.Unit
                        </td>
                        <td align="left" width="10%" style="font-weight: 700">
                            Item Rate</td>
                        <td align="left" style="font-weight: 700" width="10%">
                            <asp:Label ID="Label9" runat="server" style="font-weight: 700" Text="Quantity"></asp:Label>
                            <span style="color: #FF0000">*</span></td>
                        <td align="left" style="font-weight: 700; width: 10%;">
                            Total<asp:Label ID="lblTotal" runat="server" Visible="False"></asp:Label>
                        </td>
                        <td align="left" style="font-weight: 700" width="10%">
                            &nbsp;</td>
                        <td align="left" style="font-weight: 700" width="10%">
                            &nbsp;</td>
                    </tr>
                    <tr>
                        <td align="left" width="25%">
                            <asp:TextBox ID="txtRawMaterialItem" runat="server" AutoPostBack="True" ontextchanged="txtRawMaterialItem_TextChanged" 
                                placeholder=" Search by Raw Material Item Name,Code" Width="95%" 
                                Height="18px"></asp:TextBox>
                            <ajaxToolkit:AutoCompleteExtender ID="txtRawMaterialItem_AutoCompleteExtender" 
                                runat="server" CompletionInterval="20" CompletionSetCount="12" 
                                EnableCaching="true" MinimumPrefixLength="1" ServiceMethod="GetRMAndCmnItem" 
                                ServicePath="AutoComplete.asmx" TargetControlID="txtRawMaterialItem" />
                        </td>
                        <td align="left" width="10%">
                            <asp:DropDownList ID="ddlUniteType" runat="server" Enabled="False" 
                                Height="25px" Width="85%">
                            </asp:DropDownList>
                        </td>
                        <td align="left" width="10%">
                            <asp:TextBox ID="txtUnitePrice" runat="server" Enabled="False" 
                                placeholder="0.00" ReadOnly="True" style="text-align: right;" Width="85%" 
                                Height="18px"></asp:TextBox>
                        </td>
                        <td align="left" width="10%">
                            <asp:TextBox ID="txtQuantity" runat="server" AutoPostBack="True" 
                                ontextchanged="txtQuantity_TextChanged" placeholder="0.00" 
                                style="text-align: right;" Width="85%" Height="18px"></asp:TextBox>
                        </td>
                        <td align="left" style="width: 10%">
                            <asp:TextBox ID="txtTotal" runat="server" AutoPostBack="True" 
                                style="text-align: right;" placeholder="0.00"
                                ReadOnly="True" 
                                Width="85%" Height="18px"></asp:TextBox>
                           
                        </td>
                        <td align="center" width="10%" valign="middle">
                            <asp:LinkButton ID="lbAdd" runat="server" BorderColor="#FF3300" 
                                BorderStyle="Solid" Font-Bold="True" Font-Size="Large" Height="25px" 
                                onclick="btnAdd_Click" Width="90%">Add</asp:LinkButton>
                        </td>
                        <td align="center" width="10%">
                            <asp:LinkButton ID="lbClear" runat="server" BorderColor="#FF3300" 
                                BorderStyle="Solid" Font-Bold="True" Font-Size="Large" Height="25px" 
                                onclick="lbClear_Click" Width="90%">Clear</asp:LinkButton>
                        </td>
                    </tr>
                </table> </fieldset>
            </td>
            <td style="width: 10%">
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width: 10%">
                &nbsp;</td>
            <td style="width: 80%">
                 <asp:GridView ID="dgItemDetails" runat="server" AllowPaging="True" 
                     CssClass="mGrid" PageSize="30" 
                    style="text-align:center;" Width="100%" AutoGenerateColumns="False" 
                     onselectedindexchanged="dgItemDetails_SelectedIndexChanged" 
                     onrowdeleting="dgItemDetails_RowDeleting" 
                     onrowdatabound="dgItemDetails_RowDataBound1">
                    <Columns>
                    <asp:TemplateField>
                                                <ItemTemplate>
                                                    <asp:ImageButton ID="lbDelete" runat="server" CausesValidation="False" 
                                                        CommandName="Delete" ImageAlign="Middle" ImageUrl="~/img/delete.png" 
                                                        Text="Delete" />
                                                </ItemTemplate>
                                                <ItemStyle Font-Size="8pt" HorizontalAlign="Center" Width="3%" />
                                            </asp:TemplateField>
                        <asp:BoundField HeaderText="Item ID" DataField="ItemId">
                        <ItemStyle Font-Size="8pt" HorizontalAlign="Center" Width="10%" />
                         </asp:BoundField>
                        <asp:BoundField HeaderText="Item Name" DataField="ItemName">
                         <ItemStyle Font-Size="8pt" HorizontalAlign="Left" Width="18%" />
                         </asp:BoundField>
                        <asp:BoundField DataField="UnitePrice" HeaderText="Unite Price" >
                         <ItemStyle Font-Size="8pt" HorizontalAlign="Right" Width="6%" />
                         </asp:BoundField>
                        <asp:BoundField HeaderText="Quantity" DataField="Quantity">
                         <ItemStyle Font-Size="8pt" HorizontalAlign="Right" Width="6%" />
                         </asp:BoundField>
                         <asp:BoundField HeaderText="total" DataField="Total">
                         <ItemStyle Font-Size="8pt" HorizontalAlign="Right" Width="6%" />
                         </asp:BoundField>
                    </Columns>
                    <HeaderStyle Font-Bold="True" Height="25px" />
                </asp:GridView>
            </td>
            <td style="width: 10%">
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width: 10%">
                &nbsp;</td>
            <td style="width: 80%">
                &nbsp;</td>
            <td style="width: 10%">
                &nbsp;</td>
        </tr>
       
    </table>
    </ContentTemplate>
                </asp:UpdatePanel>
     </asp:Panel>

</div>

<div id="divBackground" style="position: fixed; z-index: 999; height: 100%; width: 100%;
        top: 0; left:0; background-color: Black; filter: alpha(opacity=60); opacity: 0.6; -moz-opacity: 0.8;-webkit-opacity: 0.8;display:none">
    </div>
</asp:Content>

