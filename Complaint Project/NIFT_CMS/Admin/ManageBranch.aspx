<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="ManageBranch.aspx.cs" Inherits="NIFT_CMS.ManageBranch" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="main_layout">
        <div class="Head_div">
            <asp:Label runat="server" ID="lbl_user" CssClass="banklogoclass"></asp:Label>
            <asp:Button ID="sign_out" Text="SIGNOUT" CssClass="btn_upload" runat="server" OnClick="sign_out_Click" />
               </div>
        <h3 style="color:white; text-align: center; margin-top: 10px;">COMPLAINT MANAGEMENT SYSTEM</h3>
        <br />
        <div class="btn_div">
           <asp:Button ID="Btn_back" CssClass="btn_ticket" runat="server" Text="BACK"
                UseSubmitBehavior="False" CausesValidation="False" OnClick="Btn_back_Click" />
        </div>

     <div class="admin_layout">
         <h3 class="hd-top" runat="server">Manage Branches</h3>
        <div class="divRow">
                <div class="divtd">
                    Bank Code:
                        <asp:DropDownList ID="DropDownList1" runat="server" Width="275px" AutoPostBack="True" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged" >
                        </asp:DropDownList>
                    </div>
                <div class="divtd">
                    Branch Code:
                        <asp:TextBox ID="txtBranchCode" runat="server" CssClass="StyleSheet.css" EnableViewState="False"
                            Height="20px" MaxLength="4" Width="275px"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ControlToValidate="txtBranchCode"   
                                ErrorMessage="Please Enter Valid Branch Code" ForeColor="Red" ValidationExpression="^[0-9]*[1-9][0-9]*$">  
                                </asp:RegularExpressionValidator>
                    </div>
         </div>
         <div class="divRow">
                <div class="divtd">
                    Bank Name:
                        <asp:TextBox ID="txtBankName" runat="server" Height="41px" MaxLength="50" TextMode="MultiLine"
                            Width="275px" ReadOnly="True"></asp:TextBox>
                    </div>
                <div class="divtd">
                    Branch Name:
                        <asp:TextBox ID="txtBranchName" runat="server" Height="41px" MaxLength="50" TextMode="MultiLine"
                            Width="275px"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server" ControlToValidate="txtBranchName"   
                                ErrorMessage="Please Enter Valid Branch Name" ForeColor="Red" ValidationExpression="^[ 0-9a-zA-Z-,._&()/]*$">  
                                </asp:RegularExpressionValidator>
                    </div>
         </div>

         <div class="divfrst" style="display:flex;">
                <div class="divFtd" style="padding:5px;margin-left:103px;">
                    City:
                        <asp:DropDownList ID="DDL2" runat="server" Width="275px">
                        </asp:DropDownList>
                    <br /><br />Sub-City Status Change:
                    <asp:RadioButton ID="SCopen" GroupName="subct" runat="server" Text="YES" /><asp:RadioButton ID="SCclose" GroupName="subct" runat="server" Text="NO" />
                    <%--<asp:CheckBox ID="Subcity" runat="server" Font-Names="Microsoft Sans Serif" Font-Overline="False" Font-Size="12pt" onclick="twoFunction(this)" />
                         <label runat="server" id="Label2" for="Subcity"></label>--%>
                    </div>
                <div class="divFtd" style="padding:7px;margin-left:164px;">
                    Status:
                        <asp:CheckBox ID="bOpen" runat="server" Font-Names="Microsoft Sans Serif" Font-Overline="False" Font-Size="12pt" onclick="myFunction(this)" />
                         <label runat="server" id="lblchkbx" for="bOpen"></label>
                    
                </div>
                </div>
           
            <br class="blankline" />
            <div class="divRow">
                <div class="divtd">
                    <asp:Button ID="Submit" runat="server"  CssClass="button-form" Text="Submit" Width="96px" OnClick="Submit_Click" />
                    <asp:Button ID="Button3" runat="server"  CssClass="button-form" Text="Cancel" Width="96px" OnClick="Button2_Click" />
                    <br class="blankline" />
                    <br class="blankline" />
                                <asp:Label ID="lblmsg" CssClass="lblMsg" ForeColor="Red" runat="server" ></asp:Label>
                    <br class="blankline" />
                    <br class="blankline" />
                </div>
            </div>

                <div class="divRow">
                <div class="divtd">

                        <asp:Label ID="Label1" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="Small"
                            Text="Select Bank" Width="106px"></asp:Label>
                        <asp:DropDownList ID="DDL1" runat="server"
                            Width="271px" AutoPostBack="True" OnSelectedIndexChanged="DDL1_SelectedIndexChanged">
                        </asp:DropDownList>
                        <input type="hidden" name="h_InstID" />
                        <input type="hidden" name="h_BranchID" />
                        <input type="hidden" name="h_CityID" />
                        &nbsp;
                    </div></div>
					
					<asp:GridView ID="dg_branch" CssClass="GridMain" runat="server" AllowPaging="True" PageSize="100" AutoGenerateColumns="False" DataKeyNames="InstID,InstName,BranchID,Branch_Name,CityID,status"
                        OnItemDataBound="dg_branch_ItemDataBound" OnPageIndexChanging="dg_branch_PageIndexChanging" OnSelectedIndexChanged="dg_branch_SelectedIndexChanged"  >
                        <FooterStyle BackColor="Gainsboro" Font-Bold="False" Font-Italic="False" Font-Overline="False"
                            Font-Strikeout="False" Font-Underline="False" Font-Names="tahoma" ForeColor="#333333" />
                        <EditRowStyle Font-Bold="False" Font-Italic="False" Font-Names="Verdana" Font-Overline="False"
                            Font-Size="X-Small" Font-Strikeout="False" Font-Underline="False" />
                        <SelectedRowStyle Font-Bold="False" Font-Italic="False" Font-Names="Verdana" Font-Overline="False"
                            Font-Size="X-Small" Font-Strikeout="False" Font-Underline="False" />
                        <PagerStyle CssClass="gridviewPager" HorizontalAlign="Center" />
                        <AlternatingRowStyle Font-Bold="False" Font-Italic="False" Font-Names="Verdana"
                            Font-Overline="False" Font-Size="X-Small" Font-Strikeout="False" Font-Underline="False" />
                        <RowStyle Font-Bold="False" Font-Italic="False" Font-Names="Verdana" Font-Overline="False"
                            Font-Size="X-Small" Font-Strikeout="False" Font-Underline="False" />

                        <HeaderStyle BackColor="Gainsboro" ForeColor="Black" Font-Bold="False" Font-Italic="False" Font-Names="Verdana" Font-Overline="False" Font-Size="X-Small" Font-Strikeout="False" Font-Underline="False"></HeaderStyle>
                        <Columns>
                            <asp:TemplateField HeaderText="Bank Code">
                                <ItemTemplate>
                                    <asp:Label ID="InstID" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "InstID") %>' Width="64px"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Bank Name">
                                <ItemTemplate>
                                    <asp:Label ID="InstName" runat="server" Width="160px" Text='<%# DataBinder.Eval(Container.DataItem, "InstName") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Branch Code">
                                <ItemTemplate>
                                    <asp:Label ID="BranchID" runat="server" Width="67px" Text='<%# DataBinder.Eval(Container.DataItem, "BranchID") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Branch Name">
                                <ItemTemplate>
                                    <asp:Label ID="Branch_Name" runat="server" Width="176px" Text='<%# DataBinder.Eval(Container.DataItem, "Branch_Name") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="City Code">
                                <ItemTemplate>
                                    <asp:Label ID="CityID" runat="server" Width="176px" Text='<%# DataBinder.Eval(Container.DataItem, "CityID") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Status">
                                <ItemTemplate>
                                    <asp:Label ID="Status" runat="server" Width="176px" Text='<%# DataBinder.Eval(Container.DataItem, "status") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:ButtonField Text="Edit" HeaderText="Edit" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" CommandName="Select" />
                        </Columns>
                        <EmptyDataTemplate>
                    <div class="EmptyDataTemplate">No Record Found</div>
                </EmptyDataTemplate>
                    </asp:GridView>
        </div>
    </div>
</asp:Content>
