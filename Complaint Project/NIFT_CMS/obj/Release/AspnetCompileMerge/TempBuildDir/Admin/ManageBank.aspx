<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="ManageBank.aspx.cs" Inherits="NIFT_CMS.ManageBank" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        .divRow {
            display: flex;
        }
    </style>
    <div class="main_layout">
        <div class="Head_div">
            <div class="column Main">
             </div>
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
        <h3 class="hd-top" runat="server">Manage Banks</h3>
            <div class="divRow">
                <div class="divtd">
                    Bank Code: 
                    <asp:TextBox ID="txtBankCode" TabIndex="1" MaxLength="3" Width="300" CssClass="txtbox" placeholder="Bank Code" runat="server" onkeypress="return onlyNumbers(this);"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtBankCode"   
    ErrorMessage="Please Enter Valid Bank Code" ForeColor="Red" ValidationExpression="^[0-9]*[0-9][0-9]*$">
    </asp:RegularExpressionValidator>
                </div>
                <div class="divtd">
                    Bank Name:
                    <asp:TextBox ID="txtBankName" TabIndex="2" MaxLength="30" Width="300" CssClass="txtbox" placeholder="Bank Name" runat="server" onkeypress="return allowAlphaNumericSpace(event);"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="txtBankName"
    ErrorMessage="Please Enter Valid Bank Name" ForeColor="Red" ValidationExpression="^[0-9a-zA-Z_.\s]*$">
    </asp:RegularExpressionValidator>
                </div>
            </div>
            <br class="blankline" />
        <div style="text-align: center;">
            <asp:Button ID="btn_Submit" UseSubmitBehavior="true" CssClass="btn_ticket" runat="server" Text="Submit" OnClick="btnSubmit_Click" />
            <asp:Button ID="btn_Update" UseSubmitBehavior="true" CssClass="btn_ticket" Visible="false" runat="server" Text="Update" OnClick="btn_Update_Click" />
            <asp:Button ID="btn_Cancel" UseSubmitBehavior="False" CssClass="btn_ticket" runat="server" Text="Cancel" OnClick="btnCancel_Click" />
            </div>
            <asp:GridView ID="dg_bank" runat="server"
                CssClass="GridMain" AutoGenerateColumns="False"
                ShowHeaderWhenEmpty="True"
                OnSelectedIndexChanged="dg_bank_SelectedIndexChanged"
                OnPageIndexChanging="dg_bank_PageIndexChanged"           
                AllowPaging="True" PageSize="10"
                DataKeyNames="InstID,InstName">
                <PagerSettings Mode="NumericFirstLast" PageButtonCount="4" FirstPageText="<<" LastPageText=">>" Position="Bottom" />
                <PagerStyle CssClass="gridviewPager" HorizontalAlign="Center" />
                <Columns>
                    <asp:BoundField DataField="InstID" Visible="true" HeaderText="Bank Code" >
                    </asp:BoundField>
                    <asp:BoundField DataField="InstName" Visible="true" HeaderText="Bank Name" >
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
