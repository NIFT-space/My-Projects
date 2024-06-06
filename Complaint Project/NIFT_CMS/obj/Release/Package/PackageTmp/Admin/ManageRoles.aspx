<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="ManageRoles.aspx.cs" Inherits="NIFT_CMS.ManageRoles" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="main_layout">
        <div class="Head_div">
            <asp:Label runat="server" ID="lbl_user" CssClass="banklogoclass"></asp:Label>
            <asp:Button ID="sign_out" UseSubmitBehavior="False" Text="SIGNOUT" CssClass="btn_upload" runat="server" OnClick="sign_out_Click" />
               </div>
        <h3 style="color:white; text-align: center; margin-top: 10px;">COMPLAINT MANAGEMENT SYSTEM</h3>
        
        <div class="btn_div">
           <asp:Button ID="Btn_back" CssClass="btn_ticket" runat="server" Text="BACK"
                UseSubmitBehavior="False" CausesValidation="False" OnClick="Btn_back_Click" />
        </div>
        </div>

    <div class="admin_layout">
        <h3 class="hd-top" runat="server">Manage Roles</h3>
            <input type="hidden" name="h_RoleID" id="h_rolid" runat="server" />
            <input type="hidden" name="h_RoleName" id="h_rolnm" runat="server" />

                        <asp:GridView ID="dg_role" CssClass="GridMain" runat="server" CellPadding="0" AllowSorting="True" AutoGenerateColumns="False"
                            OnSelectedIndexChanged="dg_role_SelectedIndexChanged1" OnPageIndexChanging="dg_role_PageIndexChanging"
                            EnableTheming="True" AllowPaging="True" PageSize="100" BorderColor="Silver">
                            <Columns>
                                <asp:ButtonField Text="Edit" HeaderText="Edit" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" CommandName="Select" />
                                <asp:BoundField DataField="RoleID" HeaderText="RoleID" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" >
                                </asp:BoundField>
                                <asp:BoundField DataField="RoleName" HeaderText="RoleName" HeaderStyle-HorizontalAlign="left" ItemStyle-HorizontalAlign="left" >
                                </asp:BoundField>
                                <asp:BoundField DataField="Roledesc" HeaderText="Roledesc" HeaderStyle-HorizontalAlign="left" ItemStyle-HorizontalAlign="left" >
                                </asp:BoundField>
                            </Columns>
                            <PagerStyle CssClass="gridviewPager" HorizontalAlign="center"  />
                        </asp:GridView>
        </div>
</asp:Content>
