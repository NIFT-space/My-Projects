<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="ManageUser.aspx.cs" Inherits="NIFT_CMS.ManageUser" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        .auto-style1 {
            float: right;
            margin-right: 15px;
        }
        .search_btn{
            background-color: white;
            color: #d7182a;
            padding: 5px;
            border: 1px solid #d7182a !important;
            font-weight: bold !important;
        }
    </style>
    <div class="main_layout">
        <div class="Head_div">
            <asp:Label runat="server" ID="lbl_user" CssClass="banklogoclass"></asp:Label>
            <asp:Button ID="sign_out" Text="SIGNOUT" CssClass="btn_upload" runat="server" OnClick="sign_out_Click" UseSubmitBehavior="false" />
               </div>
        <h3 style="color:white; text-align: center; margin-top: 10px;">COMPLAINT MANAGEMENT SYSTEM</h3>
        
        <div class="btn_div">
           <asp:Button ID="Btn_back" CssClass="btn_ticket" runat="server" Text="BACK"
                UseSubmitBehavior="False" CausesValidation="False" OnClick="Btn_back_Click" />
            <asp:Button ID="cr_user" CssClass="btn_ticket" runat="server" Text="CREATE USER"
                UseSubmitBehavior="False" CausesValidation="False" OnClick="Btn_Cr_Click" />
        </div>

        <div class="layout">
            <div style="float:left;"><h3 class="hd-top" runat="server">Manage Users</h3></div>
            <div class="auto-style1"><asp:TextBox ID="search_bar" runat="server" Width="300px" height="25px"></asp:TextBox>
                <asp:Button ID="search_btn" runat="server" Text="SEARCH" CssClass="search_btn" OnClick="search_btn_Click" UseSubmitBehavior="false" />
                <br />
                <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="search_bar" Display="Dynamic"
                                    ErrorMessage="Please Enter Valid User Name" ForeColor="Red" ValidationExpression="^[ A-Za-z'`]*$">  
                                    </asp:RegularExpressionValidator>
            </div>

            <br class="blankline" />
            <input type="hidden" name="h_RoleID" />
            <input type="hidden" name="h_UserID" />
               
            <asp:GridView ID="dg_user" CssClass="GridMain" runat="server" CellPadding="0" AllowSorting="True" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true"
                 EnableTheming="False" AllowPaging="True" PageSize="15" OnPageIndexChanging="dg_user_PageIndexChanging" OnSorting="dg_user_Sorting" OnSelectedIndexChanged="dg_user_SelectedIndexChanged">

                        <Columns>
                                <asp:TemplateField Visible="False" HeaderText="userid">
                                    <ItemTemplate>
                                        <asp:Label ID="userid" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "userid") %>' Width="32px"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:ButtonField Text="Edit" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" CommandName="Select" >
                                <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center" ForeColor="#0066FF"></ItemStyle>
                                </asp:ButtonField>

                                <asp:BoundField DataField="userid" SortExpression="userid" ReadOnly="True" HeaderText="User ID">
                                <HeaderStyle ForeColor="White" />
                                </asp:BoundField>

                                <asp:BoundField DataField="username" SortExpression="username" ReadOnly="True" HeaderText="User Name">
                                <HeaderStyle ForeColor="White" />
                                </asp:BoundField>

                                <asp:BoundField DataField="usertype" SortExpression="usertype" ReadOnly="True" HeaderText="User Type">
                                <HeaderStyle ForeColor="White" />
                                </asp:BoundField>

                                <asp:BoundField DataField="BankName" SortExpression="BankName" ReadOnly="True" HeaderText="Bank Name">
                                <HeaderStyle ForeColor="White" />
                                </asp:BoundField>

                                <%--<asp:BoundField DataField="branchname" SortExpression="branchname" ReadOnly="True" HeaderText="Branch Name">
                                <HeaderStyle ForeColor="White" />
                                </asp:BoundField>--%>

                                <asp:BoundField DataField="isactive" SortExpression="isactive" ReadOnly="True" HeaderText="Active">
                                <HeaderStyle ForeColor="White" />
                                </asp:BoundField>

                            </Columns>
                            <EmptyDataTemplate>
                            <div align="center">No records found.</div>
                        </EmptyDataTemplate>
                <PagerStyle CssClass="gridviewPager" HorizontalAlign="center"  />
                         </asp:GridView>
        </div>
        </div>
    <%--<div class="admin_layout">
        <div class="auto-style1"><asp:TextBox ID="search_bar" runat="server" Width="300px" height="25px"></asp:TextBox>
                <asp:Button ID="search_btn" runat="server" Text="SEARCH" CssClass="search_btn" OnClick="search_btn_Click" /></div>

            <br class="blankline" />
            <input type="hidden" name="h_RoleID" />
            <input type="hidden" name="h_UserID" />
               
            <asp:GridView ID="dg_user" CssClass="GridMain" runat="server" CellPadding="0" AllowSorting="True" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true"
                 EnableTheming="False" AllowPaging="True" PageSize="1000" OnSortCommand="dg_user_SortCommand" OnPageIndexChanging="dg_user_PageIndexChanging" OnSorting="dg_user_Sorting" OnSelectedIndexChanged="dg_user_SelectedIndexChanged">

                        <Columns>
                                <asp:TemplateField Visible="False" HeaderText="userid">
                                    <ItemTemplate>
                                        <asp:Label ID="userid" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "userid") %>' Width="32px"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:ButtonField Text="Edit" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" CommandName="Select" >
                                <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center" ForeColor="#0066FF"></ItemStyle>
                                </asp:ButtonField>

                                <asp:BoundField DataField="fullname" SortExpression="fullname" ReadOnly="True" HeaderText="User Name">
                                <HeaderStyle ForeColor="White" />
                                </asp:BoundField>

                                <asp:BoundField DataField="BankName" SortExpression="BankName" ReadOnly="True" HeaderText="Bank Name">
                                <HeaderStyle ForeColor="White" />
                                </asp:BoundField>

                                <asp:BoundField DataField="isbranchuser" SortExpression="isbranchuser" ReadOnly="True" HeaderText="Privileges">
                                <HeaderStyle ForeColor="White" />
                                </asp:BoundField>

                                <asp:BoundField DataField="branchname" SortExpression="branchname" ReadOnly="True" HeaderText="Branch Name">
                                <HeaderStyle ForeColor="White" />
                                </asp:BoundField>

                                <asp:BoundField DataField="isactive" SortExpression="isactive" ReadOnly="True" HeaderText="Active">
                                <HeaderStyle ForeColor="White" />
                                </asp:BoundField>

                            </Columns>
                            <EmptyDataTemplate>
                            <div align="center">No records found.</div>
                        </EmptyDataTemplate>
                <PagerStyle CssClass="gridviewPager" HorizontalAlign="center"  />
                         </asp:GridView>
                   <br class="blankline" />
            <%--<asp:Button ID="Button1" runat="server" CssClass="button-form" OnClick="Button1_Click" Text="Excel Download" />
        </div>--%>
</asp:Content>
