<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="UpdateWorkCode.aspx.cs" Inherits="NIFT_CMS.UpdateWorkCode" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        .admin_layout{
            background: #f9f9f9;
        }
        #ddl_tat{
                width: 100px;
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
            <h3 class="hd-top" runat="server">Complaint Work Code</h3>
            <div class="divRow">
                <div class="divtd">
                    Work Code Description: 
                    <asp:TextBox ID="wc_name" TabIndex="0" MaxLength="300" Width="300" CssClass="txtbox" runat="server"></asp:TextBox>
                    Turn-Around Time: 
                    <asp:TextBox ID="_TAT" runat="server" MaxLength="10" TextMode="Number" TabIndex="1"></asp:TextBox>
                    <asp:DropDownList ID="ddl_tat" runat="server" TabIndex="2">
                        <asp:ListItem Value="0" Text="Days"></asp:ListItem>
                        <asp:ListItem Value="1" Text="Hours"></asp:ListItem>
                    </asp:DropDownList>
                    </div>
            </div>
        <div>
            <asp:Button ID="btn_Submit" UseSubmitBehavior="true" CssClass="btn_ticket" TabIndex="3" runat="server" Text="Add" OnClick="btnSubmit_Click" />
            <asp:Button ID="btn_Update" UseSubmitBehavior="true" CssClass="btn_ticket" TabIndex="3" Visible="false" runat="server" Text="Update" OnClick="btn_Update_Click" />
        </div>

            <asp:GridView ID="dg_bank" runat="server"
                CssClass="GridMain" AutoGenerateColumns="False"
                ShowHeaderWhenEmpty="True"
                OnSelectedIndexChanged="dg_bank_SelectedIndexChanged"
                OnPageIndexChanging="dg_bank_PageIndexChanged" OnRowDeleting="dg_bank_RowDeleting"          
                AllowPaging="True" PageSize="10" DataKeyNames="workcode">
                <PagerSettings Mode="NumericFirstLast" PageButtonCount="4" FirstPageText="<<" LastPageText=">>" Position="Bottom" />
                <PagerStyle CssClass="gridviewPager" HorizontalAlign="Center" />
                <Columns>
                    <asp:BoundField DataField="WorkCode" HeaderText="Code" >
                    </asp:BoundField>
                    <asp:BoundField DataField="Description" HeaderText="Complaint Code Desription" >
                    </asp:BoundField>
                    <asp:BoundField DataField="CreationDateTime" HeaderText="Updated Time" >
                    </asp:BoundField>
                    <asp:BoundField DataField="TAT" HeaderText="Turnaround Time" >
                    </asp:BoundField>
                    
                    <asp:ButtonField Text="Edit" HeaderText="Edit" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" CommandName="Select" >
                    <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                    </asp:ButtonField>
                    <asp:TemplateField>
                        <ItemTemplate>
                           <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False"
                               CommandName="Delete" Text="Delete" OnClientClick="Confirm()"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <div class="EmptyDataTemplate">No Record Found</div>
                </EmptyDataTemplate>
            </asp:GridView>
            <br class="blankline" />
        </div>
</asp:Content>
