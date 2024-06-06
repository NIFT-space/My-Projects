<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="ForgotUser.aspx.cs" Inherits="NIFT_CMS.ForgotUser" %>

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
            <div class="side_login"><h3 style="color:white; text-align: center; margin-top: -68px; width: 420px;">COMPLAINT MANAGEMENT SYSTEM</h3>
                <div class="col-md-12 signIn_panel">

                    <div class="">
                        <div class="col-md-12"><h4><strong>Forgot User</strong></h4> <hr style="margin:0px !important"></div>
                    </div>
                    
                        <div class="col-md-12">
                                <p>&nbsp;</p>
                                <div class="form-group">
                                    <asp:label runat="server" class="Heading_" text="EmailAddress" Font-Size="16px"></asp:label>
                                    <asp:textbox runat="server" ID="mobile_" class="form-control required" tabindex="1" maxlength="20"></asp:textbox>
                                    <a id="signin" runat="server">SIGN-IN</a>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator6" runat="server" ControlToValidate="mobile_" Display="Dynamic"
                                        ErrorMessage="Please Enter Valid Email-Address" ForeColor="Red" 
                                        ValidationExpression="^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$">  
                                        </asp:RegularExpressionValidator>
                                </div>
                            </div>
                    <asp:button runat="server" UseSubmitBehavior="true" class="btn_signup" text="PROCEED" tabindex="2" ID="btn_Proceed" OnClick="btn_Proceed_Click" />
                     <asp:Label ID="lblmsg" runat="server" ForeColor="Red" Font-Names="Verdana"></asp:Label>
                    </div>
            </div>
        </div>
        </div>
</asp:Content>

