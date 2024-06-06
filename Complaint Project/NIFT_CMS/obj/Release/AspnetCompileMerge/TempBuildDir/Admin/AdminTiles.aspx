<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="AdminTiles.aspx.cs" Inherits="NIFT_CMS.AdminTiles" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        .admin_layout{
            background: #f9f9f9;
        }
    </style>
    <div class="main_layout">
        <div class="Head_div">
            <asp:Label runat="server" ID="lbl_user" CssClass="banklogoclass"></asp:Label>
            <asp:Button ID="sign_out" Text="SIGNOUT" CssClass="btn_upload" runat="server" OnClick="sign_out_Click" />
               </div>
        <h3 style="color:white; text-align: center; margin-top: 10px;">COMPLAINT MANAGEMENT SYSTEM</h3>
        
        </div>
    <div class="admin_layout">
            <ul class="gridbox_responsive">          
                <li class="column_responsive">
                    <a href="ManageBank">
                        <img src="../assets/images/bank-4-32.png" alt="" class="shakeImg" />
                        <p>Manage</p>
                        <p>Bank</p>
                    </a>
                </li>            
                 <%--<li class="column_responsive">
                    <a href="ManageBranch">
                        <img src="../assets/images/shop-32.png" alt="" class="shakeImg" />
                        <p>Manage</p>
                        <p>Branch</p>
                    </a>
                </li>   --%>            
                <li class="column_responsive">
                    <a href="EditRole">
                        <img src="../assets/images/role-32.png" alt="" class="shakeImg" />
                        <p>Add</p>
                        <p>Roles</p>
                    </a>
                </li>
                <li class="column_responsive">
                    <a href="ManageRoles">
                        <img src="../assets/images/role-32.png" alt="" class="shakeImg" />
                        <p>Manage</p>
                        <p>Roles</p>
                    </a>
                </li>               
                <li class="column_responsive">
                    <a href="Createuser">
                        <img src="../assets/images/user-3-32.png" alt="" class="shakeImg" />
                        <p>Add</p>
                        <p>Users</p>
                    </a>
                </li>
                <li class="column_responsive">
                    <a href="ManageUser">
                        <img src="../assets/images/user-3-32.png" alt="" class="shakeImg" />
                        <p>Manage</p>
                        <p>Users</p>
                    </a>
                </li>
                <li class="column_responsive">
                    <a href="ManageWorkCode">
                        <img src="../assets/images/user-3-32.png" alt="" class="shakeImg" />
                        <p>Manage</p>
                        <p>Work Code</p>
                    </a>
                </li>
                <li class="column_responsive">
                    <a href="ManageMerchant">
                        <img src="../assets/images/user-3-32.png" alt="" class="shakeImg" />
                        <p>Manage</p>
                        <p>Merchant</p>
                    </a>
                </li>
            </ul>
        </div>
</asp:Content>
