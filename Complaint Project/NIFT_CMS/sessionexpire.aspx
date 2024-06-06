<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="sessionexpire.aspx.cs" Inherits="NIFT_CMS.sessionexpire" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="main_layout">
        <h3 style="color:white; text-align: center; margin-top: 10px;">COMPLAINT MANAGEMENT SYSTEM</h3>
            <div style="padding:50px;background: #f9f9f9;"><p class="bigText boldText redColor">System Message</p>
            <p class="bigText ">Your session is expired due to security reasons.</p>
            <p class="bigText ">Please Login again by clicking the button below.</p>
            <p class="bigText ">Or Contact NIFT@111 112 2222 for further details.</p>
            <p class="bigText ">Thank You.</p>
        <asp:Button ID="btn_login" runat="server" CssClass="btn_ticket" Text="Re-Login" OnClick="btn_login_Click" /></div>
        </div>
</asp:Content>
