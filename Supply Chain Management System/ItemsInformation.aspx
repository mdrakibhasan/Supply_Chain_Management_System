<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ItemsInformation.aspx.cs" Inherits="ItemsInformation" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxtoolkit" %>
<%@ Import Namespace="System.Data" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">

<script language="javascript" type="text/javascript" >
    function SetImage() {
        document.getElementById('<% =lbImgUpload.ClientID %>').click();
    }

    function OpStockCal() {
        aler(txtUnitPrice.value);
        var txtUnitPrice = document.getElementById("<%=txtUnitPrice.ClientID %>");
        var txtOpeningStock = document.getElementById("<%=txtOpeningStock.ClientID %>");
        var txtOpeningAmount = document.getElementById("<%=txtOpeningAmount.ClientID %>");
        txtOpeningAmount.value = (txtUnitPrice.value * txtOpeningStock.value);
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

    function LoadModalDiv() {

        var bcgDiv = document.getElementById("divBackground");
        bcgDiv.style.display = "block";

    }
    function HideModalDiv() {

        var bcgDiv = document.getElementById("divBackground");
        bcgDiv.style.display = "none";

    }

    function UncheckOthers(objchkbox) {
        //Get the parent control of checkbox which is the checkbox list
        var objchkList = objchkbox.parentNode.parentNode.parentNode;
        //Get the checkbox controls in checkboxlist
        var chkboxControls = objchkList.getElementsByTagName("input");
        //Loop through each check box controls
        for (var i = 0; i < chkboxControls.length; i++) {
            //Check the current checkbox is not the one user selected
            if (chkboxControls[i] != objchkbox && objchkbox.checked) {
                //Uncheck all other checkboxes
                chkboxControls[i].checked = false;
            }
        }
    }

    onblur = function () {
        setTimeout('self.focus()', 100);
    }
</script>
<div id="frmMainDiv" style="background-color:White; width:100%;">
<table  id="pageFooterWrapper">
 <tr>
	<td align="center">
    
	    <asp:Button  ID="BtnDelete" runat="server"  ToolTip="Delete Record"   
            OnClick="BtnDelete_Click"  
            
            
            
            onclientclick="javascript:return window.confirm('are u really want to delete  these data')" Text="Delete" 
        Height="35px" Width="100px" BorderStyle="Outset"  />
	 </td>
	<td align="center">
    
	<asp:Button  ID="BtnFind" runat="server"  ToolTip="Find"  
            OnClick="BtnFind_Click"  Text="Find" 
        Height="30px" Width="100px" BorderStyle="Outset"  />
	</td>
    <td align="center">
        &nbsp;</td>
	
	<td align="center">
        <asp:Button ID="BtnSave" runat="server" OnClientClick="this.disabled=true;" UseSubmitBehavior="false" ToolTip="Save or Update Record" 
            OnClick="BtnSave_Click" Text="Save"  
        Height="35px" Width="100px" BorderStyle="Outset"  /></td>
	<td align="center">
        <asp:Button ID="BtnReset" runat="server" ToolTip="Clear Form" 
            OnClick="BtnReset_Click" Text="Clear" 
        Height="35px" Width="100px" BorderStyle="Outset"  /></td>
    <td align="center">
       <asp:Button ID="btnPrint" runat="server" ToolTip="Print" Text="Print" 
        Height="35px" Width="100px" BorderStyle="Outset" Visible="False"  />
   </td>        
	</tr>		
</table>

    
    <table style="width: 100%">
        <tr>
            <td style="width: 26px">
                &nbsp;</td>
            <td style="width: 1166px">
                &nbsp;</td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width: 26px">
                &nbsp;</td>
            <td style="width: 1166px">     
                <%-- </ContentTemplate>
         </asp:UpdatePanel>--%>
         <fieldset style="vertical-align: top; border: solid 1px #8BB381;"><legend style="color: maroon;"><b> Items Information </b></legend>
              <table style="width:100%;">
                     <tr>
            <td align="left" >
                Items Type</td>
            <td >&nbsp;</td>
            <td colspan="4">
                <asp:UpdatePanel ID="UPItemType" runat="server" UpdateMode="Conditional"><ContentTemplate>
                <asp:RadioButtonList ID="rdbItemType" runat="server" BorderStyle="Solid" 
                    RepeatDirection="Horizontal" AutoPostBack="True" 
                        onselectedindexchanged="rdbItemType_SelectedIndexChanged">
                    <asp:ListItem Selected="True" Value="1">Row Material Items</asp:ListItem>
                    <asp:ListItem Value="2">Finished Goods Items</asp:ListItem>
                    <asp:ListItem Value="3">Common </asp:ListItem>
                    <%--<asp:ListItem Value="4">Damaged Items</asp:ListItem>--%>
                </asp:RadioButtonList>
                </ContentTemplate>
                </asp:UpdatePanel>
            </td>
            
                         <td>
                <asp:DropDownList ID="ddlCurrency" runat="server" Width="100%" Height="26px" Visible="False">
                    <asp:ListItem>BDT</asp:ListItem>
            <asp:ListItem>Pesso</asp:ListItem>
                    <asp:ListItem>USD</asp:ListItem>
                    <asp:ListItem>EUR</asp:ListItem>
                </asp:DropDownList>
                         </td>
                         <td align="left">
                             &nbsp;</td>
            
        </tr>
                     <tr>
            <td align="left" style="width: 101px">
                <asp:Label ID="Label3" runat="server" Text="Code"></asp:Label>
            </td>
            <td style="width: 13px">&nbsp;</td>
            <td style="width: 250px">
                <asp:Label ID="lblID" runat="server" Visible="False"></asp:Label>
                <asp:UpdatePanel ID="UPCode" runat="server" UpdateMode="Conditional"><ContentTemplate>
                <asp:TextBox ID="txtStyleNo" runat="server" Width="96%"></asp:TextBox>
                </ContentTemplate>
                </asp:UpdatePanel>
            </td>
            <td style="width: 57px">
                &nbsp;</td>
            <td align="left" style="width: 129px">
                Security Code</td>
            <td >
                <asp:TextBox ID="txtSecurityCode" runat="server" Width="96%"></asp:TextBox>
            </td>
            
                         <td>
                <asp:CheckBox ID="CheckBox1" runat="server" Text="Active?" />
                         </td>
                         <td align="left">
                            <asp:FileUpload ID="imgUpload" style="display:none" runat="server" Visible="true" Size="15" Height="20px"  onchange="javascript:SetImage();" Font-Size="8pt"/>
<asp:Button ID="lbImgUpload" runat="server" Text="Upload" Font-Size="8pt" Width="50px" Height="20px" onclick="lbImgUpload_Click" style="display:none;"></asp:Button>
                         </td>
            
        </tr>
        <tr>
            <td align="left" style="width: 101px">
                <asp:Label ID="Label4" runat="server" Text="Name"></asp:Label>
            </td>
            <td style="width: 13px">&nbsp;</td>
            <td colspan="4">
                <asp:TextBox ID="txtName" runat="server" Width="99%"></asp:TextBox>           
            </td>
            <td align="center" colspan="2" rowspan="6">
    <asp:Image ID="imgEmp" runat="server" Width="145px" BorderStyle="Solid" 
                    BackColor="#EFF3FB" BorderWidth="1px" Visible="False" />  
            </td>
        </tr>
        <tr>
            <td align="left" style="width: 101px">
                <asp:Label ID="Label21" runat="server" Text="Short Name"></asp:Label>
            </td>
            <td style="width: 13px">&nbsp;</td>
            <td style="width: 250px">
                <asp:TextBox ID="txtStName" runat="server" Width="90%" placeholder="Short Name"></asp:TextBox>
            </td>
            <td style="width: 57px">
                &nbsp;</td>
            <td align="left" style="width: 129px">
                &nbsp;</td>
            <td style="width: 167px">
                <asp:TextBox ID="txtCode" runat="server" Width="90%" Visible="False"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td align="left" style="width: 101px">
                <asp:Label ID="Label20" runat="server" Text="Brand"></asp:Label>
            </td>
            <td style="width: 13px">&nbsp;</td>
            <td style="width: 250px">
                <asp:DropDownList ID="ddlBrand" runat="server" Height="26px" Width="94%">
                </asp:DropDownList>
            </td>
            <td style="width: 57px">
                <asp:LinkButton ID="Hyper" runat="server" Font-Bold="True" 
                    Font-Size="Small" Font-Underline="True" OnClientClick="LoadModalDiv();">New</asp:LinkButton>
                    <ajaxToolkit:ModalPopupExtender ID="ModalPopupExtender1" runat="server" 
            BackgroundCssClass="modalBackground" CancelControlID="btnClientQuit" 
            DropShadow="true" PopupControlID="pnlClient" TargetControlID="Hyper"></ajaxToolkit:ModalPopupExtender>

               
            </td>
            <td align="left" style="width: 129px">
                <asp:Label ID="Label5" runat="server" Text="UOM"></asp:Label>
            </td>
            <td style="width: 167px">
                <asp:DropDownList ID="ddlUmo" runat="server" Width="100%" Height="26px">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td align="left" style="width: 101px">
                <asp:Label ID="Label11" runat="server" Text="Unit Price"></asp:Label>
            </td>
            <td style="width: 13px">&nbsp;</td>
            <td style="width: 250px">
                <asp:TextBox ID="txtUnitPrice" runat="server" Width="90%" 
                    style="text-align:right;"  onkeypress="return isNumber(event)" 
                    onfocus="this.select();"></asp:TextBox>
            </td>
            <td style="width: 57px">
                &nbsp;</td>
            <td align="left" style="width: 129px">
                 Sale Price</td>
            <td style="width: 167px">
                <asp:TextBox ID="txtBranchSalesPrice" runat="server" Width="95%" 
                    style="text-align:right;"  onkeypress="return isNumber(event)" 
                    onfocus="this.select();"></asp:TextBox>
            </td>
        </tr>
        <tr style="display:none">
            <td align="left" style="width: 101px">
                <asp:Label ID="Label6" runat="server" Text="Opening Stock"  Visible="False"></asp:Label>
            </td>
            <td style="width: 13px"></td>
            <td style="width: 250px" align="left">  
                 <asp:UpdatePanel ID="UPOPStock" runat="server" UpdateMode="Conditional">    
                 <ContentTemplate>
                <asp:TextBox ID="txtOpeningStock" runat="server" Width="90%"  onkeypress="return isNumber(event)"
                     onfocus="this.select();"
                    style="text-align:right;" AutoPostBack="True" 
                    ontextchanged="txtOpeningStock_TextChanged" Visible="False"></asp:TextBox>
                  </ContentTemplate></asp:UpdatePanel>              
            </td>
            <td style="width: 57px" align="left">  
                 &nbsp;</td>
            <td align="left" style="width: 129px">
                <asp:Label ID="Label12" runat="server" Text="Amount" Visible="False"></asp:Label>
            </td>
            <td style="width: 167px">    
            <asp:UpdatePanel ID="UPOPAmount" runat="server" UpdateMode="Conditional">    
                 <ContentTemplate>       
                <asp:TextBox ID="txtOpeningAmount" runat="server" Width="95%" 
                    style="text-align:right;" Visible="False"></asp:TextBox>  
                    </ContentTemplate></asp:UpdatePanel>                  
            </td>
        </tr>
        <tr style="display:none">
            <td style="height: 22px; width: 101px;" align="left">
                <asp:Label ID="Label7" runat="server" Text="Closing Stock"></asp:Label>
            </td>
            <td style="width: 13px"></td>
            <td style="height: 22px; width: 250px;" align="left">
                <asp:TextBox ID="txtClosingStock" runat="server" Width="90%"  onkeypress="return isNumber(event)"
                    style="text-align:right;" Enabled="False"></asp:TextBox>
            </td>
            <td style="height: 22px; width: 57px;" align="left">
                &nbsp;</td>
            <td style="height: 22px; width: 129px;" align="left">
                <asp:Label ID="Label13" runat="server" Text="Amount"></asp:Label>
            </td>
            <td style="height: 22px; width: 167px;">
                <asp:TextBox ID="txtClosingAmount" runat="server" Width="95%" 
                    style="text-align:right;" Enabled="False"></asp:TextBox>
            </td>
        </tr>
        <tr style="display:none">
            <td style="height: 22px; width: 101px;" align="left">
                <asp:Label ID="Label8" runat="server" Text="Catagory"></asp:Label>
            </td>
            <td style="width: 13px">&nbsp;</td>
            <td style="height: 22px; width: 250px;" align="left">      
            <asp:UpdatePanel ID="UPanel1Cat" runat="server" UpdateMode="Conditional">    
                 <ContentTemplate>      
                <asp:DropDownList ID="ddlCatagory" runat="server" AutoPostBack="True" 
                    Height="26px" onselectedindexchanged="ddlCatagory_SelectedIndexChanged" 
                    Width="94%">
                </asp:DropDownList>
                 </ContentTemplate></asp:UpdatePanel>
            </td>
            <td style="height: 22px; width: 57px;" align="left">      
                <asp:LinkButton ID="HyperLink2" runat="server" Font-Bold="True" 
                    Font-Size="Small" Font-Underline="True" OnClientClick="LoadModalDiv();">New</asp:LinkButton>
                <ajaxToolkit:ModalPopupExtender ID="ModalPopupExtender2" runat="server" 
                    BackgroundCssClass="modalBackground" CancelControlID="btnClientQuitCategory" 
                    DropShadow="true" PopupControlID="pnlClient1" TargetControlID="HyperLink2" />
                
               
            </td>
            <td style="height: 22px; width: 129px;" align="left">
                <asp:Label ID="Label15" runat="server" Text="Sub Catagory"></asp:Label>
            </td>
            <td style="height: 22px; width: 167px;">
            <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">    
                 <ContentTemplate>
                <asp:DropDownList ID="ddlSubCatagory" runat="server" Height="26px" Width="100%">
                </asp:DropDownList>
                </ContentTemplate></asp:UpdatePanel>
            </td>
            <td style="height: 22px; width: 154px;" align="right">
                
&nbsp;
                <asp:LinkButton ID="HyperLink3" runat="server" Font-Bold="True" 
                    Font-Size="Small" Font-Underline="True" OnClientClick="LoadModalDiv();">New</asp:LinkButton>
                <ajaxToolkit:ModalPopupExtender ID="ModalPopupExtender4" runat="server" 
                    BackgroundCssClass="modalBackground" CancelControlID="btnClientQuitSubCategory" 
                    DropShadow="true" PopupControlID="pnlClient2" TargetControlID="HyperLink3">
                </ajaxToolkit:ModalPopupExtender>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Label ID="Label16" runat="server" Text="Tax Category"></asp:Label>
            </td>
            <td style="height: 22px">
                <asp:DropDownList ID="ddlTextCatagory" runat="server" Height="26px" 
                    Width="100px">
                </asp:DropDownList>
            </td>
        </tr>
        <tr style="display:none">
            <td style="width: 101px;" align="left">
                <asp:Label ID="Label19" runat="server" Text="Description"></asp:Label>
            </td>
            <td style="width: 13px"></td>
            <td align="left" colspan="4">            
                <asp:TextBox ID="txtDescription" runat="server" Width="99%"></asp:TextBox>
            </td>
            <td style="width: 154px;" align="right">
                <asp:CheckBox ID="CheckBox2" runat="server" 
                    Text="Discounted?" AutoPostBack="True" 
                    oncheckedchanged="CheckBox2_CheckedChanged" />
            </td>
            <td>
                <asp:TextBox ID="txtDiscountAmount" runat="server" Width="100px" style="text-align:right;"></asp:TextBox>
                <asp:Label ID="Label17" runat="server" Text="%" Font-Bold="True" 
                    ForeColor="#CC3300"></asp:Label>
            </td>
        </tr>
        <tr style="display:none;">
            <td align="left" colspan="6">
                <table style="width: 100%">
                    <tr>
                        <td style="width: 48%;text-align: left;">
                             <fieldset style="vertical-align: top; border: solid 1px #8BB381; text-align:left;">
                                                        <legend style="color: maroon;"><b>Color</b></legend>
                                                        <asp:Panel ID="Pandel1"  style="text-align:left;" runat="server" Height="150px" ScrollBars="Horizontal">
                                                            
                                                             <asp:RadioButtonList ID="chkColor" runat="server">
                                                            </asp:RadioButtonList>
                                                        </asp:Panel>
                                                    </fieldset>
                        </td>
                        <td style="width: 4%">
                            &nbsp;</td>
                        <td style="width: 48%">
                             <fieldset style="vertical-align: top; border: solid 1px #8BB381; text-align:left;">
                                                        <legend style="color: maroon;"><b>Size</b></legend>
                                                        <asp:Panel ID="Panel2" style="text-align:left;"  runat="server" Height="150px" ScrollBars="Horizontal">
                                                            <asp:RadioButtonList ID="chkSize" runat="server">
                                                            </asp:RadioButtonList>
                                                            
                                                        </asp:Panel>
                                                    </fieldset>
                        </td>
                    </tr>
                </table>
            </td>
            <td style="width: 154px;" align="right">
                &nbsp;</td>
            <td>
                &nbsp;</td>
        </tr>
                </table></fieldset>
               <%-- </ContentTemplate>
         </asp:UpdatePanel>--%>
            </td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width: 26px">
                &nbsp;</td>
            <td style="width: 1166px">
                <fieldset style="vertical-align: top; border: solid 1px #8BB381;"><legend style="color: maroon;"><b> Search Option</b></legend>
                <table style="width: 100%;">
                        <tr>
                            <td style="width: 19%; text-align: center">&nbsp;</td>
                            <td>

                                        <asp:RadioButtonList ID="chkItemsType" runat="server" RepeatDirection="Horizontal" 
                                            BorderColor="#999966" BorderStyle="None">
                                            <asp:ListItem Value="1" onclick="UncheckOthers(this);">Row Material</asp:ListItem>
                                            <asp:ListItem Value="2" onclick="UncheckOthers(this);">Finished Goods</asp:ListItem>
                                            <asp:ListItem Value="3" onclick="UncheckOthers(this);">Common </asp:ListItem>
                                            <asp:ListItem Value="4" onclick="UncheckOthers(this);">All</asp:ListItem>
                                        </asp:RadioButtonList>
                                    </td>
                            <td style="width: 15%; text-align: center">
                                &nbsp;</td>
                            <td style="width: 20%;text-align: center">
                                &nbsp;</td>
                        </tr>
                        <tr>
                            <td style="width: 19%; text-align: center">Search By Code/Name/Style No. :</td>
                            <td>
                                        <asp:TextBox ID="txtItemsSearch" runat="server" Width="98%" AutoPostBack="True" 
                                            ontextchanged="txtItemsSearch_TextChanged"></asp:TextBox>
                                <ajaxtoolkit:AutoCompleteExtender ID="txtItemsSearch_AutoCompleteExtender"
                                                        runat="server" CompletionInterval="20" CompletionSetCount="30"
                                                        EnableCaching="true" MinimumPrefixLength="1"
                                                        ServiceMethod="GetItemList" ServicePath="~/AutoComplete.asmx"
                                                        TargetControlID="txtItemsSearch">
                                                    </ajaxtoolkit:AutoCompleteExtender>
                                    </td>
                            <td style="width: 15%; text-align: center">
                                <asp:LinkButton ID="lbSearch" runat="server" Font-Bold="True" Font-Size="12pt" OnClick="lbSearch_Click" Width="90%" BorderStyle="Solid">Search</asp:LinkButton>
                            </td>
                            <td style="width: 20%;text-align: center">
                                <table style="width: 100%;">
                                    <tr>
                                        <td style="width: 50%;text-align: center;">
                                            <asp:LinkButton ID="lbClear" runat="server" Font-Bold="True" Font-Size="12pt" OnClick="lbClear_Click" Width="90%" BorderStyle="Solid">Clear</asp:LinkButton>
                                        </td>
                                        <td style="width: 50%;">
                                            <asp:Label ID="lblItemIDSearch" runat="server" Visible="False"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        </table>
                        </fieldset>
            </td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width: 26px">
                &nbsp;</td>
            <td style="width: 1166px">
                <asp:GridView ID="dgHistory" runat="server" AllowPaging="True" CssClass="mGrid" 
                    onpageindexchanging="dgHistory_PageIndexChanging" 
                    onrowdatabound="dgHistory_RowDataBound" 
                    onselectedindexchanged="dgHistory_SelectedIndexChanged" PageSize="50" 
                    style="text-align:center;" Width="100%" Font-Size="Small" 
                    AutoGenerateColumns="False">
                    <Columns>
                        <asp:CommandField ShowSelectButton="True">
                             <ItemStyle Width="4%" HorizontalAlign="Center"></ItemStyle>
                        </asp:CommandField>
                        <asp:BoundField HeaderText="ID" DataField="ID">
                        </asp:BoundField>
                        <asp:BoundField DataField="StyleNo" HeaderText="Code">
                        <ItemStyle HorizontalAlign="Center" Width="8%" />
                        </asp:BoundField>
                        <asp:BoundField HeaderText="Name" DataField="Name">
                          <ItemStyle Width="14%" HorizontalAlign="Left"></ItemStyle>
                        </asp:BoundField>
                        <asp:BoundField HeaderText="Brand Name" DataField="Brand Name">
                          <ItemStyle Width="12%" HorizontalAlign="Left"></ItemStyle>
                        </asp:BoundField>
                        <asp:BoundField HeaderText="Catagory" DataField="Catagory">
                          <ItemStyle Width="12%" HorizontalAlign="Left"></ItemStyle>
                        </asp:BoundField>
                        <asp:BoundField HeaderText="Sub Catagory" DataField="Sub Catagory">
                          <ItemStyle Width="12%" HorizontalAlign="Left"></ItemStyle>
                        </asp:BoundField>
                        <asp:BoundField HeaderText="Unit Price" DataField="Unit Price">
                          <ItemStyle Width="10%" HorizontalAlign="Right"></ItemStyle>
                        </asp:BoundField>
                        <asp:BoundField HeaderText="Closing Stock" DataField="Closing Stock">
                          <ItemStyle Width="10%" HorizontalAlign="Right"></ItemStyle>
                        </asp:BoundField>
                    </Columns>
                    <HeaderStyle Font-Bold="True" Height="25px" />
                </asp:GridView>
            </td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width: 26px">
                &nbsp;</td>
            <td style="width: 1166px">
                <asp:Panel ID="pnlClient" runat="server" CssClass="modalPopup1" Style="padding:15px 15px 15px 15px; Display:none; background-color:White; border:1px solid black;" Width="700px">
  <fieldset style="vertical-align: top; border: solid 1px #8BB381;">
                        <legend style="color: maroon;"><b>Save Data</b></legend>     
<table style="width:100%;">
<tr>
<td style="width:15%;" align="left">
    &nbsp;</td>
<td style="width:16%;" align="right"> 
    &nbsp;</td>
<td style=" width:4%;" >&nbsp;</td>
<td style="width:41%;" align="left">
    &nbsp;</td>
<td style="width:25%;" align="left" > 
    &nbsp;</td>
</tr>
<tr>
<td style="width:15%;" align="left">
    &nbsp;</td>
<td style="width:16%;" align="right"> 
    <asp:Label ID="Label2" runat="server" Text="Name"></asp:Label>
    </td>
<td style=" width:4%;" >&nbsp;</td>
<td style="width:41%;" align="left">
    <asp:TextBox ID="txtBrandName" runat="server" Width="100%"></asp:TextBox>
    
</td>
<td style="width:25%;" align="left" > 
    &nbsp;</td>
</tr>
    <tr>
        <td align="left" style="width:15%;">
            &nbsp;</td>
        <td align="right" style="width:16%;">
            <asp:Label ID="Label38" runat="server" Text="Description"></asp:Label>
        </td>
        <td style=" width:4%;">
            &nbsp;</td>
        <td align="left" style="width:41%;">
            <asp:TextBox ID="txtBrandDescription" runat="server" Width="100%"></asp:TextBox>
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
    <tr>
        <td style="width:5%;">
            &nbsp;</td>
        <td align="right">
            &nbsp;</td>
        <td style="width:20px;">
            &nbsp;</td>
        <td align="left">
            &nbsp;</td>
        <td style="width:5%;">
            &nbsp;</td>
    </tr>
   </table>
</td>
</tr>
</table>   
</fieldset> 
    </asp:Panel></td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width: 26px">
                &nbsp;</td>
            <td style="width: 1166px">
                &nbsp;</td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width: 26px; height: 17px;">
                </td>
            <td style="width: 1166px; height: 17px;">
                <asp:Panel ID="pnlClient1" runat="server" CssClass="modalPopup1" 
                    Style="padding:15px 15px 15px 15px; Display:none; background-color:White; border:1px solid black;" 
                    Width="700px">
  <fieldset style="vertical-align: top; border: solid 1px #8BB381;">
                        <legend style="color: maroon;"><b>Save Data</b></legend>     
<table style="width:100%;">
<tr>
<td style="width:15%;" align="left">
    &nbsp;</td>
<td style="width:16%;" align="right"> 
    &nbsp;</td>
<td style=" width:4%;" >&nbsp;</td>
<td style="width:41%;" align="left">
    &nbsp;</td>
<td style="width:25%;" align="left" > 
    &nbsp;</td>
</tr>
    <tr>
        <td align="left" style="width:15%;">
            &nbsp;</td>
        <td align="right" style="width:16%;">
            Code</td>
        <td style=" width:4%;">
            &nbsp;</td>
        <td align="left" style="width:41%;">
            <asp:TextBox ID="txtCategoryCode" runat="server" ReadOnly="True" Width="100%"></asp:TextBox>
        </td>
        <td align="left" style="width:25%;">
            &nbsp;</td>
    </tr>
<tr>
<td style="width:15%;" align="left">
    &nbsp;</td>
<td style="width:16%;" align="right"> 
    <asp:Label ID="Label39" runat="server" Text="Category Name"></asp:Label>
    </td>
<td style=" width:4%;" >&nbsp;</td>
<td style="width:41%;" align="left">
    <asp:TextBox ID="txtCategoryName" runat="server" Width="100%"></asp:TextBox>
    
</td>
<td style="width:25%;" align="left" > 
    &nbsp;</td>
</tr>
    <tr>
        <td align="left" style="width:15%;">
            &nbsp;</td>
        <td align="right" style="width:16%;">
            <asp:Label ID="Label40" runat="server" Text="Description"></asp:Label>
        </td>
        <td style=" width:4%;">
            &nbsp;</td>
        <td align="left" style="width:41%;">
            <asp:TextBox ID="txtCatageryDescription" runat="server" Width="100%"></asp:TextBox>
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
       <asp:Button ID="btnClientCategorySave0" runat="server" ToolTip="OK" 
           OnClientClick="HideModalDiv();" Text="OK" 
        Height="30px" Width="100px" BorderStyle="Outset"  Font-Size="9pt" onclick="btnClientCategorySave0_Click" 
            />
       </td>   
       <td style="width:20px;"></td>
       <td align="left" >
       <asp:Button ID="btnClientQuitCategory" runat="server" ToolTip="Quit Client" Text="Quit" OnClientClick="HideModalDiv();"
        Height="30px" Width="100px" BorderStyle="Outset"  Font-Size="9pt" />
       </td>        
       <td style="width:5%;"></td>       
   </tr>
    <tr>
        <td style="width:5%;">
            &nbsp;</td>
        <td align="right">
            &nbsp;</td>
        <td style="width:20px;">
            &nbsp;</td>
        <td align="left">
            &nbsp;</td>
        <td style="width:5%;">
            &nbsp;</td>
    </tr>
   </table>
</td>
</tr>
</table>   
</fieldset> 
    </asp:Panel></td>
            <td style="height: 17px">
                </td>
        </tr>
        <tr>
            <td style="width: 26px">
                &nbsp;</td>
            <td style="width: 1166px">
                <asp:Panel ID="pnlClient2" runat="server" CssClass="modalPopup1" 
                    Style="padding:15px 15px 15px 15px;  Display:none; background-color:White; border:1px solid black;" 
                    Width="700px">
  <fieldset style="vertical-align: top; border: solid 1px #8BB381;">
                        <legend style="color: maroon;"><b>Save Data</b></legend>     
<table style="width:100%;">
<tr>
<td style="width:15%;" align="left">
    &nbsp;</td>
<td style="width:16%;" align="right"> 
    &nbsp;</td>
<td style=" width:4%;" >&nbsp;</td>
<td style="width:41%;" align="left">
    &nbsp;</td>
<td style="width:25%;" align="left" > 
    &nbsp;</td>
</tr>
    <tr>
        <td align="left" style="width:15%;">
            &nbsp;</td>
        <td align="right" style="width:16%;">
            Code</td>
        <td style=" width:4%;">
            &nbsp;</td>
        <td align="left" style="width:41%;">
            <asp:TextBox ID="txtSubCategoryCode" runat="server" ReadOnly="True" 
                Width="100%"></asp:TextBox>
        </td>
        <td align="left" style="width:25%;">
            &nbsp;</td>
    </tr>
<tr>
<td style="width:15%;" align="left">
    &nbsp;</td>
<td style="width:16%;" align="right"> 
    Sub Category
    </td>
<td style=" width:4%;" >&nbsp;</td>
<td style="width:41%;" align="left">
    <asp:TextBox ID="txtSubcategory" runat="server" Width="100%"></asp:TextBox>
    
</td>
<td style="width:25%;" align="left" > 
    &nbsp;</td>
</tr>
    <tr>
        <td align="left" style="width:15%;">
            &nbsp;</td>
        <td align="right" style="width:16%;">
            <asp:Label ID="Label43" runat="server" Text="Category Name"></asp:Label>
        </td>
        <td style=" width:4%;">
            &nbsp;</td>
        <td align="left" style="width:41%;">
            <asp:DropDownList ID="ddlCategoryName" runat="server" Width="103%">
            </asp:DropDownList>
        </td>
        <td align="left" style="width:25%;">
            &nbsp;</td>
    </tr>
    <tr>
        <td align="left" style="width:15%;">
            &nbsp;</td>
        <td align="right" style="width:16%;">
            <asp:Label ID="Label44" runat="server" Text="Description"></asp:Label>
        </td>
        <td style=" width:4%;">
            &nbsp;</td>
        <td align="left" style="width:41%;">
            <asp:TextBox ID="txtSubcategoryDescription" runat="server" Width="100%"></asp:TextBox>
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
       <asp:Button ID="btnSubcategory" runat="server" ToolTip="OK" 
           OnClientClick="HideModalDiv();" Text="OK" 
        Height="30px" Width="100px" BorderStyle="Outset"  Font-Size="9pt" onclick="btnSubcategory_Click" 
            />
       </td>   
       <td style="width:20px;"></td>
       <td align="left" >
       <asp:Button ID="btnClientQuitSubCategory" runat="server" ToolTip="Quit Client" Text="Quit" OnClientClick="HideModalDiv();"
        Height="30px" Width="100px" BorderStyle="Outset"  Font-Size="9pt" 
                />
       </td>        
       <td style="width:5%;"></td>       
   </tr>
    <tr>
        <td style="width:5%;">
            &nbsp;</td>
        <td align="right">
            &nbsp;</td>
        <td style="width:20px;">
            &nbsp;</td>
        <td align="left">
            &nbsp;</td>
        <td style="width:5%;">
            &nbsp;</td>
    </tr>
   </table>
</td>
</tr>
</table>   
</fieldset> 
    </asp:Panel></td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width: 26px">
                &nbsp;</td>
            <td style="width: 1166px">
                &nbsp;</td>
            <td>
                &nbsp;</td>
        </tr>
    </table>
    <div id="divBackground" style="position: fixed; z-index: 999; height: 100%; width: 100%;
        top: 0; left:0; background-color: Black; filter: alpha(opacity=60); opacity: 0.6; -moz-opacity: 0.8;-webkit-opacity: 0.8;display:none">
    </div>
    
    </div>
</asp:Content>

