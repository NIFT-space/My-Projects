<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Create_user.aspx.cs" Inherits="NIFT_CMS.Create_user" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        .layout {
            overflow-y: scroll;
            height: 515px;
        }
        label{
            font-weight:normal;
        }
    </style>
    <script type="text/javascript">
        $(function () {
            $("[id*=ddl_inst]").select2();
        });
        $(function () {
            $("[id*=ddl_status]").select2();
        });
    </script>
    <div style="margin-top: -46px;">
        <div class="Head_div" >
          <asp:Label runat="server" ID="lbl_user" CssClass="banklogoclass"></asp:Label>
            <asp:Button ID="sign_out" Text="SIGNOUT" CssClass="btn_upload" runat="server" OnClick="sign_out_Click" CausesValidation="False" UseSubmitBehavior="false" />
               </div>
        <h3 style="color:white; text-align: center; margin-top: 10px;">COMPLAINT MANAGEMENT SYSTEM</h3>
     </div>
        <%-- /////////////// --%>
        
        <div class="btn_div">
           <asp:Button ID="Btn_back" CssClass="btn_ticket" runat="server" Text="BACK"
                UseSubmitBehavior="False" CausesValidation="False" OnClick="Btn_back_Click" />
        </div>
        <input type="hidden" name="h_RoleID" />
        <input type="hidden" name="h_UserID" />
    <div class="container"> 
        <%--<div class="row">
                <div class="col-md-12">
                    <h1 style="margin: 0px; padding: 0px"><strong>Welcome to the</strong><br>
                        future of <span style="color: #c03;">NIFT.</span> </h1>

                    <h4><strong>Registration Page</strong></h4>
                    <br />
                    <ul>
                        <li>Servicing 36 Commercial Banks and 7 Microfinance Banks</li>
                        <li>Servicing 11,807 Commercial Bank Branches and 487 Microfinance Bank Branches</li>
                        <li>Covering 357 Cities, Nationwide.</li>
                    </ul>
                </div>

            </div>--%>

        <div class="layout">
        <div class="side_login">
             <div class="col-md-12 signIn_panel">
                 <div class="col-md-12"><h4><strong>User-Registration</strong></h4> <hr style="margin:0px !important"></div>
                        <div class="col-md-12">
                                <p>&nbsp;</p>
                                <div class="form-group">
                                    <asp:label runat="server" class="Heading_" text="Username"></asp:label>
                                    <asp:textbox runat="server" ID="username" class="form-control required" tabindex="1" maxlength="30"></asp:textbox>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="username" Display="Dynamic"
                                            ErrorMessage="Please Enter Valid UserName" ForeColor="Red" ValidationExpression="^[0-9a-zA-Z\s._&]*$"></asp:RegularExpressionValidator>
                                </div>
                            <div class="form-group">
                                    <asp:label runat="server" class="Heading_" text="First Name"></asp:label>
                                    <asp:textbox runat="server" ID="F_name" class="form-control required" tabindex="2" maxlength="20"></asp:textbox>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="F_name" Display="Dynamic"
                                    ErrorMessage="Please Enter Valid First Name" ForeColor="Red" ValidationExpression="^[ A-Za-z'`]*$">  
                                    </asp:RegularExpressionValidator>
                                </div>
                            <div class="form-group">
                                    <asp:label runat="server" class="Heading_" text="Last Name"></asp:label>
                                    <asp:textbox runat="server" ID="L_name" class="form-control required" tabindex="3" maxlength="10"></asp:textbox>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ControlToValidate="L_name" Display="Dynamic"
                                    ErrorMessage="Please Enter Valid Last Name" ForeColor="Red" ValidationExpression="^[ A-Za-z'`]*$">  
                                    </asp:RegularExpressionValidator>
                                </div>
                            <div class="form-group">
                                    <asp:label runat="server" class="Heading_" text="Designation"></asp:label>
                                    <asp:textbox runat="server" ID="desig" class="form-control required" tabindex="4" maxlength="30"></asp:textbox>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server" ControlToValidate="desig" Display="Dynamic"
                                    ErrorMessage="Please Enter Valid Designation" ForeColor="Red" ValidationExpression="^[ A-Za-z'`]*$">
                                </asp:RegularExpressionValidator>
                                </div>
                                <div class="form-group">
                                    <asp:label runat="server" class="Heading_" text="Mobile Number"></asp:label>
                                    <asp:textbox runat="server" ID="mobile" class="form-control required" tabindex="5" maxlength="20"></asp:textbox>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator5" runat="server" ControlToValidate="mobile" Display="Dynamic"
                                        ErrorMessage="Please Enter Valid Phone Number" ForeColor="Red" ValidationExpression="^[0-9+-]*$">  
                                        </asp:RegularExpressionValidator>
                                </div>
                                <div class="form-group">
                                    <asp:label runat="server" class="Heading_" text="Email Address"></asp:label>
                                    <asp:textbox runat="server" ID="Email" class="form-control required" tabindex="6" maxlength="30"></asp:textbox>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator6" runat="server" ControlToValidate="Email" Display="Dynamic"
                                        ErrorMessage="Please Enter Valid Email-Address" ForeColor="Red" 
                                        ValidationExpression="^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$">  
                                        </asp:RegularExpressionValidator>
                                </div>
                            <div class="form-group">
                                <asp:label runat="server" class="Heading_" text="User Type"></asp:label>
                                <br />
                                <asp:RadioButton runat="server" ID="r1" GroupName="rights_rdb" Text="Operation User" />
                                <asp:RadioButton runat="server" ID="r2" GroupName="rights_rdb" Text="Merchant" />
                                <asp:RadioButton runat="server" ID="r3" GroupName="rights_rdb" Text="Bank User" />
                                <asp:RadioButton runat="server" ID="r4" GroupName="rights_rdb" Text="Administrator" />
                            </div>
                            <div class="form-group">
                                    <asp:label runat="server" class="Heading_" text="Bank/Institution Code"></asp:label>
                                <asp:DropDownList ID="ddl_inst" runat="server" class="form-control required"></asp:DropDownList>
                                    <%--<asp:textbox runat="server" ID="Bkname" class="form-control required" tabindex="7" maxlength="3"></asp:textbox>--%>
                                <%--<asp:RegularExpressionValidator ID="RegularExpressionValidator7" runat="server" ControlToValidate="Bkname" Display="Dynamic"
                                    ErrorMessage="Please Enter Valid Bank Code" ForeColor="Red" ValidationExpression="^[0-9]*$">  
                                    </asp:RegularExpressionValidator>--%>
                                </div>
                            <%--<div class="form-group">
                                    <asp:label runat="server" class="Heading_" text="Branch/Institution Code"></asp:label>
                                <asp:DropDownList ID="ddl_branch" runat="server" class="form-control required"></asp:DropDownList>
                                    <%--<asp:textbox runat="server" ID="Brname" class="form-control required" tabindex="8" maxlength="4"></asp:textbox>--%>
                                <%--<asp:RegularExpressionValidator ID="RegularExpressionValidator8" runat="server" ControlToValidate="Brname" Display="Dynamic"
                                    ErrorMessage="Please Enter Valid Branch Code" ForeColor="Red" ValidationExpression="^[0-9]*$">  
                                    </asp:RegularExpressionValidator>
                                </div>--%>
                            <div class="form-group">
                                <asp:label runat="server" class="Heading_" text="Assign Role"></asp:label>
                                <div style="overflow: scroll; height: 150px; width: 320px; border: 1px;" enableviewstate="false">
                            <asp:CheckBoxList ID="cl_role" runat="server" Font-Names="Verdana" Font-Size="Small"
                                Width="241px">
                            </asp:CheckBoxList>
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:label runat="server" class="Heading_" text="Status"></asp:label><br />
                                <asp:DropDownList ID="ddl_status" runat="server" class="form-control required">
                                    <asp:ListItem Value="1">Active</asp:ListItem>
                                    <asp:ListItem Value="0">In-active</asp:ListItem>
                                </asp:DropDownList>
                                <%--<asp:CheckBox ID="active" runat="server" Font-Names="Verdana" Font-Size="Small" Text="Active" />--%>
                            </div>
                            
                            </div>
                 <asp:Label runat="server" ID="lbl_message" CssClass="Message"></asp:Label>
                            <asp:button runat="server" class="btn_signup" text="SUBMIT" tabindex="9" ID="btn_Signup" OnClick="btn_Signup_Click" UseSubmitBehavior="true" />
                    </div>
                 </div>
        </div>
        </div>
</asp:Content>
