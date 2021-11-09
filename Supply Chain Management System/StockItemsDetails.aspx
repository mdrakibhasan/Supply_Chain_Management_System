<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="StockItemsDetails.aspx.cs" Inherits="StockItemsDetails" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <script language="javascript" type="text/javascript">
    function onListPopulated() {
        var completionList1 = $find("AutoCompleteEx1").get_completionList();
        completionList1.style.width = 'auto';
    }
    //    function OpenWindow(Url) {
    //        var testwindow = window.open(Url, '', 'width=800px,height=620px,top=150,left=300,scrollbars=1');

    function OpenWindow(Url) {
        var popUpObj;
        //        var testwindow = window.open(Url, '', 'width=500px,height=420,top=200,left=500,scrollbars=1');
        //        testwindow.blur();

        popUpObj = window.open(Url, "ModalPopUp", "toolbar=no," + "scrollbars=no," + "location=no," + "statusbar=no," + "menubar=no," + "resizable=0," + "width=800px," +

    "height=620px," + "left = 300," + "top=150");
        popUpObj.focus();
        LoadModalDiv();
    }

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
    <table style="width: 100%">
        
        <tr>
            <td colspan="3">
                &nbsp;</td>
        </tr>
        
        <tr>
            <td colspan="3" align="center">
<ajaxtoolkit:TabContainer ID="tabVch" runat="server" Width="99%" 
         Font-Size="8pt" >
 <ajaxtoolkit:TabPanel ID="tpVchDtl" runat="server" HeaderText="Items Details">
     <HeaderTemplate>
         Items Wise Stock
     </HeaderTemplate>
       <ContentTemplate>
           <div>
            <table style="width:100%;">
                <tr>
            <td style="width:20%; height: 17px;"></td>
            <td style="height: 17px"></td>
            <td style="width:20%; height: 17px;"></td>
        </tr>
        <tr>
            <td style="width:20%;">&nbsp;</td>
            <td>
            &nbsp;</td>
            <td style="width:20%;">&nbsp;</td>
        </tr>
                <tr>
                    <td style="width:20%;">
                    </td>
                    <td>
                        <fieldset style="vertical-align: top; border: solid 1px #8BB381;">
                            <legend style="color: maroon;"><b>Search Option</b> </legend>
                            <table style="width:100%;">
                            <tr>
                            <td style="width:5%;">
                            </td>
                            <td style="width:5%;"></td>
                            <td style="width:80%;" align="left">
                                <asp:RadioButtonList ID="rdbItemType" runat="server" BorderStyle="None" 
                                    RepeatDirection="Horizontal" Font-Size="XX-Small" Width="98%" 
                                    AutoPostBack="True" onselectedindexchanged="rdbItemType_SelectedIndexChanged">
                                    <asp:ListItem Selected="True" Value="0">All</asp:ListItem>
                                    <asp:ListItem Value="1">Row Material Items</asp:ListItem>
                                    <asp:ListItem Value="2">Finished Good Items</asp:ListItem>
                                    <asp:ListItem Value="3">Common </asp:ListItem>
                                    <%--<asp:ListItem Value="4">Damaged Items</asp:ListItem>--%>
                                </asp:RadioButtonList>
                                </td>
                            <td style="width:10%;"></td>
                            </tr>
                            <tr>
                            <td colspan="4">
                               
                                <div style="width:18;height:20px; float: left;" align="right">
                                </div>
                                      <div style="width:20%;height:20px; float: left;" align="right">
                                          Branch Name</div>                          
                                <div style="width:5%;height:20px; float: left;">
                                </div>
                                
                                <div style="width:55%;height:20px; float: left;">
                                    <asp:DropDownList ID="ddlItemStockType" runat="server" AutoPostBack="True" 
                                        Width="100%" 
                                        onselectedindexchanged="ddlItemStockType_SelectedIndexChanged1">
                                    </asp:DropDownList>
                                </div>
                                
                                <div style="width:100%;height:10px; float: left;">
                                </div>
                                <div align="right" style="width:20%; height:20px;float: left;">
                                    <asp:Label ID="Label1" runat="server" Text="Search Items"></asp:Label>
                                </div>
                                <div style="width:5%;height:20px; float: left;">
                                </div>
                                <div style="width:55%;height:20px; float: left;">
                                    <asp:TextBox ID="txtName" runat="server" AutoPostBack="True" Height="26px" 
                                        placeholder="Search ** Items Code OR Item's Name OR Quantity" 
                                        style="width: 100%; text-indent: 15px; display: inline-block; border: 1px solid #ccc; border-radius: 4px; box-sizing: border-box; background: transparent !important;" 
                                        Width="100%" ontextchanged="txtName_TextChanged"></asp:TextBox>
                                    <ajaxToolkit:AutoCompleteExtender ID="autoComplete1" runat="server" 
                                        CompletionSetCount="12" DelimiterCharacters="" Enabled="True" 
                                        MinimumPrefixLength="1" ServiceMethod="GetItemList" 
                                        ServicePath="AutoComplete.asmx" TargetControlID="txtName">
                                    </ajaxToolkit:AutoCompleteExtender>
                                </div>
                                <div style="width:100%;height:10px; float: left;">
                                </div>
                                </td>
                            </tr>
                            </table>
                        </fieldset>
                    </td>
                    <td style="width:20%;">
                    </td>
                </tr>
        <tr>
            <td style="width:20%;"></td>
            <td align="center">

          
             <div style="width:35%;height:20px; float: left;" >
                 <asp:RadioButtonList ID="rbReportType" runat="server" BorderStyle="Double" 
                     RepeatDirection="Horizontal">
                     <asp:ListItem Selected="True" Value="P">Pdf</asp:ListItem>
                     <asp:ListItem Value="E">Excel</asp:ListItem>
                 </asp:RadioButtonList>
                </div>
                   <div style="width:17%;height:20px; float: left;" >
                       <asp:Button ID="btnPrint" runat="server" OnClick="btnPrint_Click" Text="Print" 
                           Width="90px" />
                </div>
             <div style="width:17%;height:20px; float: left;" >
                     &nbsp;&nbsp;&nbsp;&nbsp;
                </div>
             <div style="width:15%;height:20px; float: left;" >
                     <asp:Button ID="btnClear" runat="server" Text="Clear" Width="90px" 
                         onclick="btnClear_Click" />
                </div>
             <div style="width:35%;height:20px; float: left;" ></div>
             <div style="width:100%;height:5px; float: left;" ></div>
            </td>
            <td style="width:20%;"></td>
        </tr>
        <tr>
            <td align="center" colspan="3">
                &nbsp;</td>
        </tr>
                <tr>
                    <td align="center" colspan="3">
                        <asp:GridView ID="dgItems" runat="server" AutoGenerateColumns="False" 
                            CssClass="mGrid" OnPageIndexChanging="dgItems_PageIndexChanging" 
                            OnRowDataBound="dgItems_RowDataBound" 
                            OnSelectedIndexChanged="dgItems_SelectedIndexChanged" PageSize="30" Width="95%">
                            <Columns>
                                <asp:CommandField HeaderText="Image" SelectText="Image" ShowSelectButton="True">
                                <ItemStyle HorizontalAlign="Center" Width="50px" />
                                </asp:CommandField>
                                <asp:BoundField DataField="Items" HeaderText="Items Code &amp; Name">
                                <ItemStyle HorizontalAlign="Left" Width="200px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Catagory" HeaderText="Catagory">
                                <ItemStyle HorizontalAlign="Left" Width="100px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="SubCat" HeaderText="Sub Catagory">
                                <ItemStyle HorizontalAlign="Left" Width="100px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="UnitPrice" HeaderText="Unit Price">
                                <ItemStyle HorizontalAlign="Center" Width="100px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="OpeningStock" HeaderText="OP. Stock">
                                <ItemStyle HorizontalAlign="Center" Width="100px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="OpeningAmount" HeaderText="OP. Amount">
                                <ItemStyle HorizontalAlign="Right" Width="100px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="ClosingStock" HeaderText="Closing Stock">
                                <ItemStyle HorizontalAlign="Center" Width="100px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="ClosingAmount" HeaderText="Closing Amount">
                                <ItemStyle HorizontalAlign="Right" Width="100px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="ID" HeaderText="ID" />
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
        <tr>
            <td style="width:20%;">&nbsp;</td>
            <td>&nbsp;</td>
            <td style="width:20%;">&nbsp;</td>
        </tr>
            </table>
           </div>
     </ContentTemplate>
       </ajaxtoolkit:TabPanel>
    
</ajaxtoolkit:TabContainer>
            </td>
        </tr>
        <tr>
            <td style="width:20%;">&nbsp;</td>
            <td>&nbsp;</td>
            <td style="width:20%;">&nbsp;</td>
        </tr>
        <tr>
            <td style="width:20%;">&nbsp;</td>
            <td>&nbsp;</td>
            <td style="width:20%;">&nbsp;</td>
        </tr>
    </table>
    </div>
     <div id="divBackground" style="position: fixed; z-index: 999; height: 100%; width: 100%;
        top: 0; left:0; background-color: Black; filter: alpha(opacity=60); opacity: 0.6; -moz-opacity: 0.8;-webkit-opacity: 0.8;display:none">
    </div>
</asp:Content>

