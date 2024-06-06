<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="cms_login.aspx.cs" Inherits="NIFT_CMS.cms_login" MaintainScrollPositionOnPostback="true" %>
 
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        .signIn_panel {
            margin-top: 50px;
        }
        .container {
            height: 572px;
        }
 </style>
    <div class="container">
        
        <%--<div class="row">
                <div class="col-md-12 laptops">
                    <h1 style="margin: 0px; padding: 0px"><strong>Welcome to the</strong><br>
                        future of <span style="color: #c03;">NIFT.</span> </h1>
                    <h4><strong>Sign up now to:</strong></h4>

                    <ul>
                        <li>Servicing 36 Commercial Banks and 7 Microfinance Banks</li>
                        <li>Servicing 11,807 Commercial Bank Branches and 487 Microfinance Bank Branches</li>
                        <li>Covering 357 Cities, Nationwide.</li>
                    </ul>
                    <img src="images/tick.png" style = "position: relative; left: 9px; top: 14px;max-width:150px">--%>
                <%--</div>

            </div>--%>
        <div class="layout">
        <div class="side_login"><h3 style="color:white; text-align: center; margin-top: -68px; width: 450px;">COMPLAINT MANAGEMENT SYSTEM</h3>
             <div class="col-md-12 signIn_panel">
                 
                    <div class="">
                        <div class="col-md-12"><h4><strong>Sign-in</strong></h4><hr style="margin:0px !important"></div>
                    </div>
                    
                        <div class="col-md-12">
                                <p>&nbsp;</p>
                                <div class="form-group">
                                    <asp:label runat="server" class="Heading_" text="USER NAME"></asp:label>
                                    <asp:textbox runat="server" ID="userId" name="userId" class="form-control required" type="text" tabindex="1" placeholder="" maxlength="20"></asp:textbox>
                                    <a runat="server" id="forgotuser">Forgot your User?</a>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="userId" Display="Dynamic"
                                            ErrorMessage="Please Enter Valid Username" ForeColor="Red" ValidationExpression="^[0-9a-zA-Z\s._&]*$"></asp:RegularExpressionValidator>
                                </div>

                                <div class="form-group">
                                    <asp:label runat="server" class="Heading_" text="PASSWORD"></asp:label>
                                    <asp:textbox runat="server" ID="pass_word" class="form-control required" type="password" tabindex="2" placeholder="" maxlength="25"></asp:textbox>
                                    <a runat="server" id="forgotpass">Forgot your password?</a>
                                </div>
                            <div>

                            
                                <asp:label runat="server" class="Heading_" text="Re-Captcha"></asp:label>
                                <br />
                                <div class="form-group">
                <asp:Image ID="imgCaptcha" runat="server" Height="80" Width="320" />
            </div>

            <div class="form-group">
                <asp:TextBox ID="txtCaptcha" runat="server" CssClass="form-control" placeholder="Enter Captcha Code" TabIndex="3"></asp:TextBox>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="txtCaptcha" Display="Dynamic"
                        ErrorMessage="Please Enter Correct Captcha" ForeColor="Red" ValidationExpression="^[0-9a-zA-Z]*$"></asp:RegularExpressionValidator>
                <asp:Label ID="lblMessage" runat="server"></asp:Label>
            </div>
         </div>
                                
                            <%--<asp:ScriptManager ID="scriptmanager1" runat="server">
                            </asp:ScriptManager>
                            <asp:updatepanel runat="server">
                                <contenttemplate>--%>
                            <%--<div class="form-group" style="text-align: center;font-style: oblique;font-size: 21px;">
                                <asp:textbox runat="server" ID="Fnum" maxlength="2" tabindex="3" style="text-align:center;width:40px; font-size:21px;font-style: oblique;" Enabled="False"></asp:textbox> + 
                                <asp:textbox runat="server" ID="Lnum" maxlength="2" tabindex="4" style="text-align:center;width:40px;font-size:21px;font-style: oblique;" Enabled="False"></asp:textbox> =
                                 
                                <asp:textbox runat="server" ID="Fnl" maxlength="2" tabindex="5" style="text-align:center;width:40px;font-size:21px;font-style: oblique;"
                                        AutoPostBack="True" OnTextChanged="Fnl_TextChanged" onkeypress="return onlyNumbers(this);"></asp:textbox>--%>

                                <%--</contenttemplate>
                            </asp:updatepanel>--%>
                            
                        <%--<div class="checkbox">
                                    <label>
                                        <input type="checkbox" name="rememberme" id="rememberme" value="1"> Keep me signed in. <a href="#" data-toggle="modal">Learn more</a>
                                    </label>
                                </div>--%>
                            </div>
                            <asp:button runat="server" ID="Sign_IN" class="btn_Submit" text="SIGN IN" tabindex="4" OnClick="Sign_IN_Click" UseSubmitBehavior="true"/>
                            
                    <asp:Label ID="alert_" runat="server" ForeColor="Red" Font-Size="Medium"></asp:Label>
                            
                        
                    </div>
                 </div>
            </div>
        </div>
</asp:Content>
