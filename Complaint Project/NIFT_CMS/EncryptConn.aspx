<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="EncryptConn.aspx.cs" Inherits="NIFT_CMS.EncryptConn" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label ID="Label1" runat="server" Text="Enter Your Connection String:"></asp:Label>
    <asp:TextBox ID="connstr" runat="server" Width="750px"></asp:TextBox>
    <br />
    <asp:Button ID="btn_Encr" runat="server" Text="ENCRYPT" OnClick="btn_Encr_Click" />
    <asp:Button ID="Button1" runat="server" Text="DECRYPT" OnClick="Button1_Click" />
    <br />
    <br />
    <asp:Label ID="Label2" runat="server" Text="Encrypted Connection Sting:"></asp:Label>
    &nbsp;
    <asp:TextBox ID="encrconnstr" runat="server" CssClass="auto-style1" Height="93px" TextMode="MultiLine" Width="759px"></asp:TextBox>
</asp:Content>

