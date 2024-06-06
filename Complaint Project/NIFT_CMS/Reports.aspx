<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Reports.aspx.cs" Inherits="NIFT_CMS.Reports" MaintainScrollPositionOnPostback="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        .from-secarea {
            margin-left: 10px;
            width: 50%;
            padding-top: 36px;
        }
        select#ContentPlaceHolder1_ddl_status {
            width: 100%;
            padding: 8px;
            border: 2px solid #d7182a;
            border-radius: 5px;
        }
    </style>
    <div class="main_layout">
        <%--<asp:Button ID="Btn_back" CssClass="btn_ticket" runat="server" Text="BACK" 
                    UseSubmitBehavior="False" CausesValidation="False" OnClick="Btn_back_Click"/>--%>
        <div class="Head_div">
            <div class="column Main">
            <ul><li class="dropdown">
                <a href="#">MENU ▾</a>
            <ul class="dropdown-menu">
                <%--<li class="lia"><a id="link1" runat="server">Initiate Complaint</a></li>
                <li class="lia"><a id="link2" runat="server">In-Progress</a></li>
                <li class="lia"><a id="link3" runat="server">Pending Complaints</a></li>--%>
                <li class="lia"><a id="link6" runat="server">Open Complaints</a></li>
                <li class="lia"><a id="link4" runat="server">Closed Complaints</a></li>
                <%--<li class="lia"><a id="link5" runat="server">Reports</a></li>--%>
                <li class="lia" id="lr_7" runat="server" visible="false"><a id="link7" runat="server">Operations Dashboard</a></li>
            </ul></li></ul>
                </div>
            <asp:Label runat="server" ID="lbl_user" CssClass="banklogoclass"></asp:Label>
            <asp:Button ID="sign_out" UseSubmitBehavior="False" Text="SIGNOUT" CssClass="btn_upload" runat="server" OnClick="sign_out_Click" />
               </div>
            <h3 style="color:white; text-align: center; margin-top: 10px;margin-left: 100px;">COMPLAINT MANAGEMENT SYSTEM</h3>
        <div class="layout">
            <h3 class="hd-top" runat="server">Report Generation</h3>
                    <div class="dateform-to">
                    <div class="from-area">
                    <asp:Label ID="From" runat="server" Text="From Date"></asp:Label>
                    <asp:TextBox ID="txtFromDate" runat="server" placeholder="Date/Month/Year" TextMode="Date">
                    </asp:TextBox>
                    </div>
                    
                    <div class="from-area">
                    <asp:Label ID="To" runat="server" Text="To Date"></asp:Label>
                    <asp:TextBox ID="txtToDate" runat="server" placeholder="Date/Month/Year" TextMode="Date">
                    </asp:TextBox>
                    </div>

                    <div class="from-area">
                    <asp:Label ID="Status" runat="server" Text="Status"></asp:Label>
                    <asp:DropDownList runat="server" ID="ddl_status" OnSelectedIndexChanged="ddl_status_SelectedIndexChanged">
                        <asp:ListItem Value="10" Text ="Please Select" Selected="True"></asp:ListItem>
                        <asp:ListItem Value="1" Text="OPEN"></asp:ListItem>
                        <asp:ListItem Value="0" Text="CLOSED"></asp:ListItem>
                        <asp:ListItem Value="2" Text="IN-PROGRESS"></asp:ListItem>
                        <asp:ListItem Value="3" Text="PENDING"></asp:ListItem>
                    </asp:DropDownList>
                    </div>

                    <div class="from-secarea">
                    <%--<asp:Label ID="Label1" runat="server" Text="Type"></asp:Label>
                    <asp:DropDownList ID="Comp_Type" runat="server">
                        <asp:ListItem>Please Select</asp:ListItem>
                    </asp:DropDownList>--%>
                    
        <asp:Button ID="Btn_Search" runat="server" Text="Search" CssClass="btn_ticket"  UseSubmitBehavior="False" OnClick="Btn_Search_Click" />
        <asp:Button ID="Btn_ShowAll" runat="server" Text="Show All" CssClass="btn_ticket"  UseSubmitBehavior="False" OnClick="Btn_ShowAll_Click" />
                        <asp:Button ID="Btn_GenAll" runat="server" Text="Generate All" CssClass="btn_ticket" Visible="true"  UseSubmitBehavior="False" OnClick="Btn_GenAll_Click" />
                        <asp:Label ID="lblmsg" runat="server" ForeColor="Red" Font-Names="Verdana" Text="Please Select Search Filters" ></asp:Label>
                    </div>
                        </div>
        
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

                <asp:BoundField DataField="Assignee" HeaderText="Assignee" />

                <asp:CommandField ShowSelectButton="True" SelectText="Generate"> 
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
