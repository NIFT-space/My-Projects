<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Forget_Password.aspx.cs" Inherits="NIFT_CMS.NewPassword1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="complain-area layout">
    <div class="container">
        <div class="row">
                <div class="col-md-12">
                    <h1 style="margin: 0px; padding: 0px"><strong>Welcome to the</strong><br>
                        future of <span style="color: #c03;">NIFT.</span> </h1>

                    <h4><strong>Account Recovery:</strong></h4>

                    <ul>
                        <li>Servicing 36 Commercial Banks and 7 Microfinance Banks</li>
                        <li>Servicing 11,807 Commercial Bank Branches and 487 Microfinance Bank Branches</li>
                        <li>Covering 357 Cities, Nationwide.</li>
                    </ul>
                </div>
            </div>

        <div class="side_login">
             <div class="col-md-12 signIn_panel">
                    <div class="">
                        <div class="col-md-12"><h4><strong>Sign-in</strong></h4> <hr style="margin:0px !important"></div>
                    </div> 
                    
                        <div class="col-md-12">
                            

                                <p>&nbsp;</p>
                                <div class="form-group">
                                    <asp:label runat="server" class="Heading_" text="Username or Email Address"></asp:label>
                                    <asp:textbox runat="server" ID="userId" name="userId" class="form-control required" type="text" tabindex="1" placeholder="abc@nift.com" maxlength="50"></asp:textbox>
                                </div>
                            
                            </div>
                            <asp:button runat="server" class="btn_signup" text="SUBMIT" tabindex="2" />
            </div>
            </div>
        </div>
        </div>
    <!-- /container -->
</asp:Content>
