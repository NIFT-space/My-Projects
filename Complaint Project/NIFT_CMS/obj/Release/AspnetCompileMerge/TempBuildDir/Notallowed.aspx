<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Notallowed.aspx.cs" Inherits="NIFT_CMS.Notallowed" %>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="layout">
        <%--<div class="bigText boldText centerText" style="color:white;">NIFT E-PAY Complaint Management System</div>
        <br class="blankline" />--%>
        <div style="padding:20px;"><p class="bigText boldText redColor">System Message</p>
        <p class="bigText ">Sorry you do not have enough permission.</p>
        <p class="bigText ">to access this page.</p>
        <p class="bigText ">Contact NIFT@111 112 2222 for further details.</p>
        <p class="bigText ">Thank You.</p>
            <asp:Button ID="btn_login" runat="server" CssClass="btn_ticket" Text="Re-Login" OnClick="btn_login_Click" />
            </div>
    </div>
</asp:Content>
