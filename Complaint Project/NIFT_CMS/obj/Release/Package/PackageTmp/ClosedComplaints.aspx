<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="ClosedComplaints.aspx.cs" Inherits="NIFT_CMS.ClosedComplaints" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        .btn_div{
            padding: 10px;
        }
    </style>
    <div class="main_layout">
        <%--/////////////HEADER///////--%>
                <div class="Head_div" >
                     <div class="column Main">
                        <ul><li class="dropdown">
                            <a href="#">MENU ▾</a>
                        <ul class="dropdown-menu">
                        <li class="lia"><a id="link1" runat="server">Initiate Complaint</a></li>
                        <%--<li class="lia"><a id="link2" runat="server">In-Progress</a></li>
                        <li class="lia"><a id="link3" runat="server">Pending Complaints</a></li>--%>
                        <li class="lia"><a id="link6" runat="server">Open Complaints</a></li>
                        <li class="lia" id="lr_" runat="server" visible="false"><a id="link5" runat="server">Reports</a></li>
                        <li class="lia" id="lr_7" runat="server" visible="false"><a id="link7" runat="server">Operations Dashboard</a></li>
                        </ul></li></ul>
                    </div>
                    <asp:Label runat="server" ID="lbl_user" CssClass="banklogoclass"></asp:Label>
                    <asp:Button ID="sign_out" UseSubmitBehavior="False" Text="SIGNOUT" CssClass="btn_upload" runat="server" OnClick="sign_out_Click"  />
                </div>
                <h3 style="color:white; text-align: center; margin-top: 10px;margin-left: 100px;">COMPLAINT MANAGEMENT SYSTEM</h3>
        
        <%-- /////////////// --%>

        <div class="layout">
            <h3 class="hd-top" runat="server">Closed Complaints</h3>
        <asp:GridView ID="Gridview1" CssClass="GridMain" runat="server" CellPadding="2"
    HeaderStyle-HorizontalAlign="Center" HorizontalAlign="Center" 
                AutoGenerateColumns="False" AllowPaging="True" ShowHeaderWhenEmpty="True" OnPageIndexChanging="Gridview1_PageIndexChanging" OnSelectedIndexChanged="Gridview1_SelectedIndexChanged" >

            <Columns>
                <asp:BoundField DataField="status" HeaderText="Status" >
                    <HeaderStyle ForeColor="White" />
                </asp:BoundField>
                
                <asp:BoundField DataField="TicketNo" HeaderText="Ticket No" >
                    <HeaderStyle ForeColor="White" />
                </asp:BoundField>

                <asp:BoundField DataField="LastUpdated" HeaderText="Last Updated" />

                <asp:BoundField DataField="ComplaintDate" HeaderText="Date Opened" 
                    DataFormatString="{0:dd/MM/yyyy}" >
                    <HeaderStyle ForeColor="White" />
                </asp:BoundField>
                
                <asp:BoundField DataField="FullName" HeaderText="Customer Name">
                    <HeaderStyle ForeColor="White" />
                </asp:BoundField>
                
                <asp:BoundField DataField="Workcode" HeaderText="Complaint Work Code" >
                    <HeaderStyle ForeColor="White" />
                </asp:BoundField>

                <asp:BoundField DataField="ContactNumber" HeaderText="Contact Number" />

                <asp:CommandField ShowSelectButton="True" SelectText="Details"  > 
		<ItemStyle ForeColor="#d7182a" Font-Bold="True" Font-Underline="True" />
                </asp:CommandField>
                
            </Columns>
            <EmptyDataTemplate>
            <div align="center">No records found.</div>
            </EmptyDataTemplate>
            <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
            <PagerStyle CssClass="gridviewPager" HorizontalAlign="center" />
      </asp:GridView>
        </div>
        </div>
</asp:Content>
