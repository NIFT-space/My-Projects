<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="OpTilePage.aspx.cs" Inherits="NIFT_CMS.OpTilePage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        .admin_layout{
            background: #f9f9f9;
        }
    </style>
    <div class="main_layout">
        <div class="Head_div" >
             <div class="column Main">
            <ul><li class="dropdown">
                <a href="#" >MENU ▾</a>
            <ul class="dropdown-menu">
                <%--<li class="lia"><a id="link2" runat="server">In-Progress</a></li>
                <li class="lia"><a id="link3" runat="server">Pending Complaint</a></li>--%>
                <li class="lia"><a id="link4" runat="server">Closed Complaint</a></li>
                <li class="lia"><a id="link6" runat="server">Open Complaints</a></li>
                <li class="lia" id="lr_" runat="server" visible="false"><a id="link5" runat="server">Reports</a></li>
            </ul></li></ul>
                </div>
            <asp:Label runat="server" ID="lbl_user" CssClass="banklogoclass"></asp:Label>
            <asp:Button ID="Button1" Text="SIGNOUT" CssClass="btn_upload" runat="server" OnClick="sign_out_Click" UseSubmitBehavior="False" />
                
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
                <li class="column_responsive">
                    <a href="ManageMerchant">
                        <img src="../assets/images/user-3-32.png" alt="" class="shakeImg" />
                        <p>Manage</p>
                        <p>Merchant</p>
                    </a>
                </li>
                <li class="column_responsive">
                    <a href="ManageWorkCode">
                        <img src="../assets/images/user-3-32.png" alt="" class="shakeImg" />
                        <p>Manage</p>
                        <p>Work Code</p>
                    </a>
                </li>
            </ul>
        </div>
</asp:Content>
