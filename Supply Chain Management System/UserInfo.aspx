<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="UserInfo.aspx.cs" Inherits="UserInfo" Title="User Administration"  Theme="Themes"%>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">  
    <div id="frmMainDiv" style="width:100%; background-color:White;">

<table id="pageFooterWrapper">
   <tr>
   <td align="center">
       <asp:Button ID="btnDelete" runat="server" ToolTip="Delete" onclick="btnDelete_Click"  
           onclientclick="javascript:return window.confirm('Are u really want to delete these data')" 
           Text="Delete"
            
             Width="130px" Height="35px" BorderStyle="Outset" BorderWidth="1px"/> </td> 
   <td align="center"> 
    <asp:Button ID="btnFind" runat="server" ToolTip="Find" 
           onclick="btnFind_Click"  Text="Find"            
             Width="130px" Height="35px" BorderStyle="Outset" BorderWidth="1px" />        
           </td> 
   <td align="center"> 
       <asp:Button ID="btnSave" runat="server" ToolTip="Save" onclick="btnSave_Click" 
       Text="Save"
            
             Width="130px" Height="35px" BorderStyle="Outset" BorderWidth="1px"  /></td>  
   <td align="center"> 
       <asp:Button ID="btnClear" runat="server" ToolTip="Clear" 
           onclick="btnClear_Click" Text="Clear" 
            
             Width="130px" Height="35px" BorderStyle="Outset" BorderWidth="1px"/>       
           </td>            
   </tr>
</table>

<table style="width:100%; font-family:Verdana;">
<tr>
<td style="width:1%;">&nbsp;</td>
<td style="width:98%;" align="center">
    &nbsp;</td>
<td style="width:1%;">&nbsp;</td>
</tr>
<tr>
<td style="width:1%;">&nbsp;</td>
<td style="width:98%;" align="center">

    <table style="width: 100%">
        <tr>
            <td style="width: 10%">
                &nbsp;</td>
            <td style="width: 80%">
            <fieldset style="vertical-align: top; border: solid 1px #8BB381;text-align:left;line-height:2.5em;"><legend style="color: maroon;"><b>User Information</b></legend>
                <table style="width: 100%;">
                    <tr>
                        <td style="width: 9%;" align="right">
<asp:Label ID="lblUserIf" runat="server" Font-Size="8pt">User ID</asp:Label>
                        </td>
                        <td style="width: 1%;">
                            &nbsp;</td>
                        <td style="width: 11%;">
                            <asp:TextBox SkinID="tbGray" ID="txtUserId" runat="server"  Width="100%" 
                                Font-Size="8pt" Enabled="true" MaxLength="20"></asp:TextBox>
                        </td>
                        <td style="width: 10%;" align="right">
<asp:Label ID="lblDescription" runat="server" Font-Size="8pt">User Name</asp:Label>
                        </td>
                        <td style="width: 1%;">
                            &nbsp;</td>
                        <td style="width: 25%;" align="left">
                            <asp:TextBox SkinID="tbGray" ID="txtDescription" runat="server" Width="95%" 
        Font-Size="8" MaxLength="50" TabIndex="1"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td style="width: 9%;" align="right">
<asp:Label ID="Label3" runat="server" Font-Size="8pt">Password</asp:Label>
                        </td>
                        <td style="width: 1%;">
                            &nbsp;</td>
                        <td style="width: 11%;">
                            <asp:TextBox SkinID="tbGray" ID="txtPassword" runat="server"  Width="100%" 
                                Font-Size="8" Enabled="true" MaxLength="15" TextMode="Password"></asp:TextBox></td>
                        <td style="width: 10%;" align="right">
<asp:Label ID="Label2" runat="server" Font-Size="8pt">User Group</asp:Label>
                        </td>
                        <td style="width: 1%;">
                            &nbsp;</td>
                        <td style="width: 25%;" align="left">
                            <asp:DropDownList SkinID="ddlPlain" ID="ddlUsrGrp" runat="server"  
                                Font-Size="8" Width="150px" TabIndex="2">
  </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 9%;" align="right">
<asp:Label ID="lblFax" runat="server" Font-Size="8pt">Status</asp:Label>
                        </td>
                        <td style="width: 1%;">
                            &nbsp;</td>
                        <td style="width: 11%;">
                            <asp:DropDownList SkinID="ddlPlain" ID="ddlStatus" runat="server"  
                                Font-Size="8" Width="104%" TabIndex="2">
  <asp:ListItem Text="Enabled" Value="A"></asp:ListItem>
  <asp:ListItem Text="Disabled" Value="U"></asp:ListItem>
  </asp:DropDownList>
                        </td>

                        <td style="width: 10%;" align="right">
                             E-mail</td>
                        <td style="width: 1%;">
                            &nbsp;</td>
                        <td style="width: 25%;" align="left">
                            <asp:TextBox SkinID="tbGray" ID="txtEmail" runat="server" Width="95%" 
        Font-Size="8" MaxLength="50" TabIndex="1"></asp:TextBox>
                    </tr>
                    <tr style="display:none;">
                        <td style="width: 9%;" align="right">
                            Branch Name</td>
                        <td style="width: 1%;">
                            &nbsp;</td>
                        <td style="width: 11%;">
        <asp:DropDownList SkinID="ddlPlain" ID="ddlBranch" runat="server" 
            Font-Size="8pt" Width="104%" Height="18px" TabIndex="13" >
     <asp:ListItem></asp:ListItem>       
     <%--<asp:ListItem Text="Banani" Value="1"></asp:ListItem> 
     <asp:ListItem Text="Dhanmondi" Value="2"></asp:ListItem>
     <asp:ListItem Text="Mirpur" Value="3"></asp:ListItem>--%>
     </asp:DropDownList>
                        </td>
                        </td>
                        <td style="width: 10%;" align="right">
                             <asp:UpdatePanel ID="UP2" runat="server" UpdateMode="Conditional"><ContentTemplate>
<asp:Label ID="Label1" runat="server" Font-Size="8pt">Emplyee ID</asp:Label>
<asp:Label ID="lblEmpID" runat="server" Font-Size="8pt" Visible="False"></asp:Label></ContentTemplate></asp:UpdatePanel>
                        </td>
                        <td style="width: 1%;">
                            &nbsp;</td>
                        <td style="width: 25%;" align="left">
                            <asp:UpdatePanel ID="UP1" runat="server" UpdateMode="Conditional"><ContentTemplate>
                            <asp:TextBox SkinID="tbGray" ID="txtEmpNo" runat="server"  Width="95%" PlaceHolder="Search Employee..."
                                Font-Size="8" Enabled="true" MaxLength="15" AutoPostBack="True" 
                                ontextchanged="txtEmpNo_TextChanged"></asp:TextBox>
         <ajaxToolkit:AutoCompleteExtender ID="txtTransfer_AutoCompleteExtender" runat="server" CompletionInterval="20" 
        CompletionSetCount="30" EnableCaching="true" MinimumPrefixLength="1" ServiceMethod="GetFacultySearch" 
        ServicePath="~/AutoComplete.asmx" TargetControlID="txtEmpNo">
        </ajaxToolkit:AutoCompleteExtender>
        </ContentTemplate></asp:UpdatePanel>
                        </td>
                    </tr>
                    <tr style="display:none;">
                        <td style="width: 9%;" align="right">
                            User Type</td>
                        <td style="width: 1%;">
                            &nbsp;</td>
                        <td align="left" colspan="2">
                            <asp:RadioButtonList ID="rbUserType" runat="server" 
                                RepeatDirection="Horizontal">
                                <asp:ListItem Selected="True" Value="1">Main Office</asp:ListItem>
                                <asp:ListItem Value="2">Branch</asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                        <td style="width: 1%;">
                            &nbsp;</td>
                        <td style="width: 25%;" align="left">
                            &nbsp;</td>
                    </tr>
                </table>  </fieldset>
            </td>
            <td style="width: 10%">
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width: 10%">
                &nbsp;</td>
            <td style="width: 80%">
                <div id="Search" runat="server">
                    <fieldset style="vertical-align: top; border: solid 1px #8BB381;text-align:left;line-height:1.5em;">
                        <legend style="color: maroon;"><b>Search User</b></legend>
                        <table style="width: 100%">
                            <tr>
                                <td align="right" style="width: 15%">
                                    Search :</td>
                                <td style="width: 2%">
                                    &nbsp;</td>
                                <td align="left" style="width: 40%">
                                    <asp:TextBox ID="txtSearch" runat="server" AutoPostBack="True" Font-Size="8" 
                                        MaxLength="150" ontextchanged="txtSearch_TextChanged" 
                                        placeholder="Search By User Name OR ID ." SkinID="tbGray" Width="362px"></asp:TextBox>
                                </td>
                                <ajaxToolkit:AutoCompleteExtender ID="txtSubjectSearch_AutoCompleteExtender" 
                                    runat="server" CompletionInterval="20" CompletionSetCount="30" 
                                    EnableCaching="true" MinimumPrefixLength="1" ServiceMethod="GetUserSearch" 
                                    ServicePath="~/AutoComplete.asmx" TargetControlID="txtSearch">
                                </ajaxToolkit:AutoCompleteExtender>
                                <td style="width: 30%">
                                    &nbsp;</td>
                            </tr>
                        </table>
                    </fieldset>
                </div>
            </td>
            <td style="width: 10%">
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width: 10%">
                &nbsp;</td>
            <td style="width: 80%">
                <asp:GridView runat="server" AutoGenerateColumns="False" CssClass="mGrid" 
                    Width="100%" ID="GridView1" OnRowCommand="GridView1_RowCommand" 
                    OnSelectedIndexChanged="GridView1_SelectedIndexChanged">
                    <Columns>
                        <asp:CommandField ShowSelectButton="True"></asp:CommandField>
                        <asp:BoundField DataField="USER_NAME" HeaderText="UserID"></asp:BoundField>
                        <asp:BoundField DataField="DESCRIPTION" HeaderText="User Name"></asp:BoundField>
                        <asp:BoundField DataField="BranchName" HeaderText="Branch"></asp:BoundField>
                        <asp:BoundField DataField="STATUS" HeaderText="Status"></asp:BoundField>
                        <asp:TemplateField HeaderText="R. Password">
                            <ItemTemplate>
                                <asp:LinkButton ID="lbProgram" runat="server" CommandName="Reset" 
                        Font-Bold="True" Font-Size="12pt" 
                        onclientclick="javascript:return window.confirm('are u really want to Change Password.')" 
                        Text="( Reset )"></asp:LinkButton>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" Width="15%"></ItemStyle>
                        </asp:TemplateField>
                    </Columns>
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
  
</td>
<td style="width:1%;">&nbsp;</td>
</tr>
<tr>
<td style="width:1%;">&nbsp;</td>
<td style="width:98%;" align="center">
    &nbsp;</td>
<td style="width:1%;">&nbsp;</td>
</tr>
<tr>
<td style="width:1%;"></td>
<td style="width:98%;" align="center">
<table style="width:100%; background-color:White;">
<tr>
<td style="width:100%;" align="center">
    &nbsp;</td>
</tr>
</table>
</td>
<td style="width:1%;"></td>
</tr>
</table>
</div>
</asp:Content>

