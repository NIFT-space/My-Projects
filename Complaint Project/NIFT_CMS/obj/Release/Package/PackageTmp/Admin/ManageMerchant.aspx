<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="ManageMerchant.aspx.cs" Inherits="NIFT_CMS.ManageMerchant" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        .divRow {
            display: flex;
        }
        .divtd {
            display: inline-grid;
        }
    </style>
    <script type="text/javascript">
    $(function () {
        $("[id*=ddl_users]").select2();
    });
    </script>

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
        <h3 class="hd-top" runat="server">Manage Merchant</h3>
            <div class="divRow">
                 <div class="divtd">
                    Merchant ID: 
                    <asp:TextBox ID="MccID_" TabIndex="0" MaxLength="10" Width="100" CssClass="txtbox" placeholder="Merchant ID" runat="server"></asp:TextBox>
                    <%--<asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="f_name"   
                                ErrorMessage="Please Enter Valid Bank Code" ForeColor="Red" ValidationExpression="^[0-9]*[0-9][0-9]*$">  
                                </asp:RegularExpressionValidator>--%>
                    </div>
                <div class="divtd" style="margin-left: 20px;">
                    First Name: 
                    <asp:TextBox ID="f_name" TabIndex="1" MaxLength="30" Width="300" CssClass="txtbox" placeholder="First Name" runat="server"></asp:TextBox>
                    <%--<asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="f_name"   
                                ErrorMessage="Please Enter Valid Bank Code" ForeColor="Red" ValidationExpression="^[0-9]*[0-9][0-9]*$">  
                                </asp:RegularExpressionValidator>--%>
                    </div>
                <div class="divtd" style="margin-left: 20px;">
                    Last Name:
                    <asp:TextBox ID="l_name" TabIndex="2" MaxLength="30" Width="300" CssClass="txtbox" placeholder="Last Name" runat="server"></asp:TextBox>
                    <%--<asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="l_name"   
                                ErrorMessage="Please Enter Valid Bank Name" ForeColor="Red" ValidationExpression="^[0-9a-zA-Z_.\s]*$">  
                                </asp:RegularExpressionValidator>--%>
                    </div>
            </div>
            <div class="divRow">
                <div class="divtd">
                    UserID: 
                    <asp:DropDownList runat="server" ID="ddl_users" TabIndex="3" class="form-control required" Width="300px">
                    </asp:DropDownList>
                    <%--<asp:TextBox ID="txtuserid" TabIndex="3" MaxLength="3" Width="300" CssClass="txtbox" placeholder="User ID" runat="server" onkeypress="return onlyNumbers(this);"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtuserid"   
                                ErrorMessage="Please Enter Valid User ID" ForeColor="Red" ValidationExpression="^[0-9]*[0-9][0-9]*$">  
                                </asp:RegularExpressionValidator>--%>
                    </div>
                <div class="" style="margin-left: 50px;">
                    Status:
                    <br />
                    <asp:CheckBox ID="chkbox_active" runat="server" Text=" Active"/>
                    </div>
            </div>
        <div style="margin-top:10px">
            <asp:Button ID="btn_Submit" UseSubmitBehavior="true" CssClass="btn_ticket" TabIndex="5" runat="server" Text="Submit" OnClick="btnSubmit_Click" />
            <asp:Button ID="btn_Update" UseSubmitBehavior="true" CssClass="btn_ticket" TabIndex="5" Visible="false" runat="server" Text="Update" OnClick="btn_Update_Click" />
            <asp:Button ID="btn_Cancel" UseSubmitBehavior="False" CssClass="btn_ticket" TabIndex="6" runat="server" Text="Cancel" OnClick="btnCancel_Click" />
            </div>

            <asp:GridView ID="dg_bank" runat="server"
                CssClass="GridMain" AutoGenerateColumns="False"
                ShowHeaderWhenEmpty="True"
                OnSelectedIndexChanged="dg_bank_SelectedIndexChanged"
                OnPageIndexChanging="dg_bank_PageIndexChanged"           
                AllowPaging="True" PageSize="20"
                DataKeyNames="MerchID,MccID,UserID,Username,FirstName,LastName,isactive">
                <PagerSettings Mode="NumericFirstLast" PageButtonCount="4" FirstPageText="<<" LastPageText=">>" Position="Bottom" />
                <PagerStyle CssClass="gridviewPager" HorizontalAlign="Center" />
                <Columns>
                    <asp:BoundField DataField="MerchID" Visible="false" >
                    </asp:BoundField>
                    <asp:BoundField DataField="MccID" Visible="true" HeaderText="Merchant Code" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Center" >
                    </asp:BoundField>
                    <asp:BoundField DataField="uid" Visible="true" HeaderText="UserID" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Center" >
                    </asp:BoundField>
                    <asp:BoundField DataField="FirstName" Visible="true" HeaderText="First Name" ItemStyle-HorizontalAlign="Center" >
                    </asp:BoundField>
                    <asp:BoundField DataField="LastName" Visible="true" HeaderText="Last Name" ItemStyle-HorizontalAlign="Center" >
                    </asp:BoundField>
                    <asp:BoundField DataField="isactive" Visible="true" HeaderText="Status" ItemStyle-HorizontalAlign="Center" >
                    </asp:BoundField>

                    <asp:ButtonField Text="Edit" HeaderText="Edit" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" CommandName="Select" >
                    <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                    </asp:ButtonField>
                </Columns>
                <EmptyDataTemplate>
                    <div class="EmptyDataTemplate">No Record Found</div>
                </EmptyDataTemplate>
            </asp:GridView>
            <br class="blankline" />
        </div>
</asp:Content>
