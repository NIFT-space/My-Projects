<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="NewPassword.aspx.cs" Inherits="NIFT_CMS.NewPassword" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        .signIn_panel {
            top: 120px;
        }
        .container {
            height: 572px;
        }
    </style>
    <div class="container">
        <div class="layout">
            <div class="side_login">
            <h3 style="color:white; text-align: center;  margin-top: -68px; width: 450px;">COMPLAINT MANAGEMENT SYSTEM</h3>
                <div class="col-md-12 signIn_panel">
        
                    <div class="">
                        <div class="col-md-12"><h4><strong>Change Password</strong></h4> <hr style="margin:0px !important"></div>
                    </div>
                    <div class="col-md-12">
                            <p class="para1">Password must be of 12-15 characters including simple characters,numbers,1 upper case letter and special characters</p>
                                <div class="form-group">
                                    <asp:label runat="server" class="Heading_" text="New Password"></asp:label>
                                    <asp:textbox runat="server" ID="password0" name="password0" class="form-control required" type="password" tabindex="1" maxlength="15"></asp:textbox>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="password0" Display="Dynamic"
                                            ErrorMessage="Please Enter Valid Password" ForeColor="Red" ValidationExpression="(?=^.{12,15}$)(?=.*\d)(?=.*[a-zA-Z])(?=.*[!@#$%^&*()_+}{:;'?/>.<,])(?!.*\s).*$">
                                        </asp:RegularExpressionValidator>
                                </div>

                                <div class="form-group">
                                    <asp:label runat="server" class="Heading_" text="ReType Password"></asp:label>
                                    <asp:textbox runat="server" ID="password1" name="password1" class="form-control required" type="password" tabindex="2" maxlength="15"></asp:textbox>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="password1" Display="Dynamic"
                                            ErrorMessage="Please Enter Valid Password" ForeColor="Red" ValidationExpression="(?=^.{12,15}$)(?=.*\d)(?=.*[a-zA-Z])(?=.*[!@#$%^&*()_+}{:;'?/>.<,])(?!.*\s).*$">
                                        </asp:RegularExpressionValidator>
                                </div>
                        </div>
                            <asp:button runat="server" UseSubmitBehavior="true" class="btn_signup" text="PROCEED" tabindex="3" ID="Btn_Change" OnClick="Btn_Change_Click" />
                 <asp:Label ID="alert_" runat="server" ForeColor="Red" Font-Size="Medium"></asp:Label>
            </div>
            </div>
        </div>
        </div>
</asp:Content>
