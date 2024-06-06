<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="EditRole.aspx.cs" Inherits="NIFT_CMS.EditRole" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        .divRow {
            display: flex;
        }
    </style>
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
        <h3 class="hd-top" runat="server">Edit Roles</h3>
        <div class="divRow">
                <div class="divtd">
                        <strong>Role Name </strong>
                        <asp:TextBox ID="tRole" runat="server" CssClass="txtbox" Width="223px" Font-Names="Verdana" Font-Size="X-Small"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server" ControlToValidate="tRole"   
                                ErrorMessage="Please Enter Valid Role Name" ForeColor="Red" ValidationExpression="^[ 0-9a-zA-Z-._&()/]*$">
                                </asp:RegularExpressionValidator>
                </div>
                <div class="divtd" style="display: table-cell; vertical-align: middle; text-align: center; margin-right: auto;">
                        <strong>Role Type</strong>
                        <asp:RadioButton ID="r1" runat="server" GroupName="roletype" Text="Adminstrator" Font-Names="Verdana" Font-Size="X-Small" />
                    <asp:RadioButton CssClass="r2" ID="r3" runat="server" GroupName="roletype" Text="Operation" Font-Names="Verdana" Font-Size="X-Small" />
                        <asp:RadioButton CssClass="r2" ID="r2" runat="server" GroupName="roletype" Text="User" Font-Names="Verdana" Font-Size="X-Small" />
                </div>
        </div>
        <div class="divRow">
                <div class="divtd">
                    <strong>Role Description</strong> &nbsp;
                        <asp:TextBox ID="tRoleDesc" runat="server" TextMode="MultiLine" CssClass="txtbox" Width="224px" Font-Names="Verdana" Font-Size="X-Small"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="tRoleDesc"
                                ErrorMessage="Please Enter Valid Role Description" ForeColor="Red" ValidationExpression="^[ 0-9a-zA-Z-._&()/]*$">  
                                </asp:RegularExpressionValidator>
                </div>
        </div>
        <asp:GridView ID="grrole" runat="server" CssClass="GridMain" AllowPaging="True" PageSize="200" AutoGenerateColumns="False">   
             <Columns>

                 <asp:TemplateField Visible="False" HeaderText="PageID">
                     <ItemTemplate>
                         <asp:Label ID="PageID" runat="server" text='<%# DataBinder.Eval(Container.DataItem, "PageID") %>' Width="32px"></asp:Label>
                     </ItemTemplate>
                 </asp:TemplateField>

                 <asp:TemplateField HeaderText="PageName">
                     <ItemTemplate>
                         <asp:Label ID="PageName" runat="server" text='<%# DataBinder.Eval(Container.DataItem, "title") %>' Width="128px"></asp:Label>
                     </ItemTemplate>
                     <HeaderStyle Width="200px" />
                     <ItemStyle Width="200px" />
                 </asp:TemplateField>

                 <asp:TemplateField HeaderText="URL">
                     <ItemTemplate>
                         <asp:Label ID="URL" runat="server" text='<%# DataBinder.Eval(Container.DataItem, "URL") %>' Width="135px"></asp:Label>
                     </ItemTemplate>
                     <HeaderStyle Width="250px" />
                     <ItemStyle Width="250px" />
                 </asp:TemplateField>

                 <asp:TemplateField HeaderText="View/Download">
                     <ItemTemplate>
                         <asp:CheckBox ID="cView" runat="server" Width="62px" />
                     </ItemTemplate>
                     <HeaderStyle Width="250px" />
                     <ItemStyle Width="250px" />
                 </asp:TemplateField>
             </Columns>
             <PagerStyle Font-Size="X-Small" Font-Bold="True" HorizontalAlign="Center" ForeColor="Blue" BackColor="White"
                 Font-Italic="False" Font-Overline="False" Font-Strikeout="False" Font-Underline="False" ></PagerStyle>
         </asp:GridView>
                
        <br class="blankline" />
           <asp:Button ID="Button1" UseSubmitBehavior="true" runat="server" CssClass="btn_ticket" OnClick="Button1_Click"
                Text="SUBMIT" Width="76px" />
            <asp:Button ID="Button2" runat="server" UseSubmitBehavior="False" CssClass="btn_ticket" Text="CANCEL" Width="79px" OnClick="Button2_Click" />
        </div>
</asp:Content>
