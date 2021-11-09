<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="frmBercode.aspx.cs" Inherits="frmBercode" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <div id="frmMainDiv" style="background-color:White; width:100%;"> 
    <table style="width: 100%">
        <tr>
            <td style="width:15%;">
                &nbsp;</td>
            <td style="width:71%;">
                &nbsp;</td>
            <td style="width:20%;">
                &nbsp;</td>
        </tr>      
        <tr>
            <td style="width:15%;">
                &nbsp;</td>
            <td style="width:71%;">
                &nbsp;</td>
            <td style="width:20%;">
                &nbsp;</td>
        </tr>      
        <tr>
            <td style="text-align: center" colspan="3">
            <fieldset style="vertical-align: top; border: solid 1px #8BB381; text-align:left;"><legend style="color: maroon;">
                <b>Bercode Generate</b> </legend>
            <asp:UpdatePanel ID="PVIesms_UP" runat="server"  UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:GridView ID="dgPODetailsDtl" runat="server" AutoGenerateColumns="False" 
                        BorderColor="LightGray" CssClass="mGrid" Font-Size="9pt" 
                        OnRowDataBound="dgPurDtl_RowDataBound" OnRowDeleting="dgPurDtl_RowDeleting" 
                        Width="100%" Visible="False">
                        <AlternatingRowStyle CssClass="alt" />
                        <Columns>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:ImageButton ID="lbDelete" runat="server" CausesValidation="False" 
                                        CommandName="Delete" ImageAlign="Middle" ImageUrl="~/img/delete.png" 
                                        Text="Delete" />
                                </ItemTemplate>
                                <ItemStyle Font-Size="8pt" HorizontalAlign="Center" Width="4%" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Item Code">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtItemCode" runat="server" AutoPostBack="true" 
                                        Font-Size="8pt" MaxLength="15" onFocus="this.select()" SkinId="tbPlain" 
                                        Text='<%#Eval("StyleNo")%>' Width="93%"></asp:TextBox>
                                </ItemTemplate>
                                <FooterStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" Width="12%" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Description">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtItemDesc" runat="server" autocomplete="off" placeholder="Search Items"
                                        AutoPostBack="true" Font-Size="8pt" onFocus="this.select()" 
                                        OnTextChanged="txtItemDesc_TextChanged" SkinId="tbPlain" 
                                        Text='<%#Eval("item_desc")%>' Width="97%">
                                    </asp:TextBox>
                                    <ajaxToolkit:AutoCompleteExtender ID="autoComplete1" runat="server"  CompletionInterval="20"
                                        CompletionSetCount="12" EnableCaching="true" 
                                        MinimumPrefixLength="1" ServiceMethod="GetAllItem" 
                                        ServicePath="AutoComplete.asmx" TargetControlID="txtItemDesc" />                                     
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Left" Width="30%" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="StockQunatity">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtStkQty" runat="server" AutoPostBack="False" Enabled="false"
                                        CssClass="tbr" Font-Size="8pt" onFocus="this.select()" SkinId="tbPlain" 
                                        Text='<%#Eval("StkQty")%>' Width="93%">
                                    </asp:TextBox>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Right" Width="10%" />
                            </asp:TemplateField>
                            
                            <%-- <asp:TemplateField HeaderText="Color">
                                <ItemTemplate>
                                    <asp:DropDownList ID="ddlColor" runat="server" AutoPostBack="true" DataSource="<%#Color()%>" DataTextField="ColorName" DataValueField="ColorID" Font-Size="8pt" 
                     SelectedValue='<%#Eval("Color")%>' SkinId="ddlPlain" Width="95%" Height="26px">
                </asp:DropDownList>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Right" Width="10%" />
                            </asp:TemplateField>--%>
                            
                             <%--<asp:TemplateField HeaderText="Size">
                                <ItemTemplate>
                                   <asp:DropDownList ID="ddlSize" runat="server" AutoPostBack="true" DataSource="<%#Size()%>" DataTextField="SizeName" DataValueField="SizeID" Font-Size="8pt" 
                     SelectedValue='<%#Eval("Size")%>' SkinId="ddlPlain" Width="95%" Height="26px">
                </asp:DropDownList>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Right" Width="10%" />
                            </asp:TemplateField>--%>

                            <asp:TemplateField HeaderText="Item Rate">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtItemRate" runat="server" AutoPostBack="False"  Enabled="false"
                                        CssClass="tbr" Font-Size="8pt" onFocus="this.select()" SkinId="tbPlain" 
                                        Text='<%#Eval("BranchSalePice")%>' Width="93%">
                                    </asp:TextBox>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Right" Width="10%" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Quantity">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtQnty" runat="server" AutoPostBack="true" CssClass="tbc" 
                                        Font-Size="8pt" onFocus="this.select()" OnTextChanged="txtQnty_TextChanged" 
                                        SkinId="tbPlain" Text='<%#Eval("qnty")%>' Width="93%"></asp:TextBox>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" Width="10%" />
                            </asp:TemplateField>

                           <%-- <asp:TemplateField HeaderText="Tax">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtTax" runat="server" AutoPostBack="true" CssClass="tbc" 
                                        Font-Size="8pt" onFocus="this.select()" 
                                        SkinId="tbPlain" Text='<%#Eval("Tax")%>' Width="93%"></asp:TextBox>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" Width="10%" />
                            </asp:TemplateField>--%>

                        </Columns>
                        <HeaderStyle Font-Bold="True" Font-Size="9pt" ForeColor="White" />
                        <PagerStyle CssClass="pgr" ForeColor="White" HorizontalAlign="Center" />
                        <RowStyle BackColor="White" />
                    </asp:GridView>
                </ContentTemplate>
        </asp:UpdatePanel></fieldset>
            </td>
        </tr>      
        <tr>
            <td style="width:15%;">
                &nbsp;</td>
            <td style="width:71%;" align="center">
                <asp:Button ID="btnPrint" runat="server" onclick="btnPrint_Click" Text="Print" 
                    Width="100px" />
&nbsp;
                <asp:Button ID="btnClear" runat="server" Text="Clear" Width="100px" 
                    onclick="btnClear_Click" />
            &nbsp;
               
            </td>
            <td style="width:20%;">
                &nbsp;</td>
        </tr>      
        <tr>
            <td colspan="3">
                &nbsp;</td>
        </tr>      
        <tr>
            <td style="width:15%;">
                &nbsp;</td>
            <td style="width:71%;" align="center">
                &nbsp;</td>
            <td style="width:20%;">
                &nbsp;</td>
        </tr>      
    </table>
    </div>
</asp:Content>

