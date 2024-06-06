<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="ticket_form.aspx.cs" Inherits="NIFT_CMS.ticket_form" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        
        .ddl_common {
            width: 95%;
            padding: 7px;
            border: 2px solid #d7182a;
            border-radius: 5px;
            font: 500 13px 'Montserrat-Regular';
        }
        span#ContentPlaceHolder1_lbl_Inst {
            display: block;
            font-size: 13px;
            font-weight: 600;
            font-family: Montserrat-Regular;
            padding: 8px 6px;
        }
        div#ContentPlaceHolder1_divInst_ {
            margin-left: 0px;
        }
    </style>
    <div class="main_layout">
            <div class="Head_div" >
             <div class="column Main">
            <ul><li class="dropdown">
                <a href="#" >MENU ▾</a>
            <ul class="dropdown-menu">
                <li class="lia"><a id="link1" runat="server">Initiate Complaint</a></li>
                <%--<li class="lia"><a id="link2" runat="server">In-Progress</a></li>
                <li class="lia"><a id="link3" runat="server">Pending Complaint</a></li>--%>
                <li class="lia"><a id="link4" runat="server">Closed Complaint</a></li>
                <li class="lia" id="lr_" runat="server" visible="false"><a id="link5" runat="server">Reports</a></li>
                <li class="lia" id="lr_7" runat="server" visible="false"><a id="link7" runat="server">Operations Dashboard</a></li>
            </ul></li></ul>
                </div>
            <asp:Label runat="server" ID="lbl_user" CssClass="banklogoclass"></asp:Label>
            <asp:Button ID="sign_out" Text="SIGNOUT" CssClass="btn_upload" runat="server" OnClick="sign_out_Click" UseSubmitBehavior="False" />
                
               </div>
            <h3 style="color:white; text-align: center; margin-top: 10px;margin-left: 100px;">COMPLAINT MANAGEMENT SYSTEM</h3>

        <div class="layout">
            <h3 class="hd-top" runat="server">All Complaints</h3>
                    <div class="dateform-to">
                        <%--<asp:Button ID="Button3" runat="server" Text="NEW COMPLAINT" CssClass="btn_ticket" OnClick="Button3_Click" />--%>
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

                    <div class="from-newarea">
                    <asp:Label ID="Label1" runat="server" Text="Type"></asp:Label>
                    <asp:DropDownList ID="Comp_Type" CssClass="ddl_common" runat="server">
                        <asp:ListItem>Please Select</asp:ListItem></asp:DropDownList>
                    </div>

                    <div id="divInst_" runat="server" class="from-newarea" visible="false">
                    <asp:Label ID="lbl_Inst" runat="server" Text="Institution"></asp:Label>
                    <asp:DropDownList ID="ddl_Inst" CssClass="ddl_common" runat="server">
                    <asp:ListItem>Please Select</asp:ListItem>
                    </asp:DropDownList>
                    </div>

                    <div style="margin-top: 30px;">
                    <asp:Button ID="Btn_Search" runat="server" Text="Search" CssClass="btn_ticket" UseSubmitBehavior="False" OnClick="Btn_Search_Click" />
                    <asp:Button ID="Btn_ShowAll" runat="server" Text="Show All" CssClass="btn_ticket" UseSubmitBehavior="False" OnClick="Btn_ShowAll_Click" />
                        <%--<asp:Label ID="lblmsg" runat="server" ForeColor="Red" Font-Names="Verdana" Text="Please Select Search Filters" ></asp:Label>--%>
                    </div>   
                  </div>
        <asp:GridView ID="Gridview1" CssClass="GridMain" runat="server" CellPadding="2" HeaderStyle-HorizontalAlign="Center" HorizontalAlign="Center" 
                AutoGenerateColumns="False" AllowPaging="True" ShowHeaderWhenEmpty="True" OnPageIndexChanging="Gridview1_PageIndexChanging"
             OnRowDataBound="Gridview1_RowDataBound" OnSelectedIndexChanged="Gridview1_SelectedIndexChanged" >
            <PagerSettings Mode="NumericFirstLast" PageButtonCount="4" FirstPageText="<<" LastPageText=">>" Position="Bottom" />
                    <PagerStyle CssClass="gridviewPager" HorizontalAlign="Center" />
            <Columns>
                <asp:BoundField DataField="status" HeaderText="Status" >
                    <HeaderStyle ForeColor="White" />
                </asp:BoundField>
                
                <asp:TemplateField HeaderText="NIFT Assignee">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="NIFTAssignee" Text='<%# Eval("NIFTAssignee") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                
                <%--<asp:BoundField DataField="NIFTAssignee" HeaderText="NIFT Assignee" >
                    <HeaderStyle ForeColor="White" />
                </asp:BoundField>--%>

                <asp:BoundField DataField="TicketNo" HeaderText="Ticket No" >
                    <HeaderStyle ForeColor="White" />
                </asp:BoundField>

                <asp:BoundField DataField="Nature" HeaderText="Ticket Type" />

                <asp:BoundField DataField="LastUpdated" HeaderText="Last Updated" />

                <asp:BoundField DataField="ComplaintDate" HeaderText="Date Opened" 
                    DataFormatString="{0:dd/MM/yyyy}" >
                    <HeaderStyle ForeColor="White" />
                </asp:BoundField>
                
                <asp:BoundField DataField="FullName" HeaderText="Customer Name">
                    <HeaderStyle ForeColor="White" />
                </asp:BoundField>

                <asp:BoundField DataField="InstName" HeaderText="InstName">
                    <HeaderStyle ForeColor="White" />
                </asp:BoundField>
                
                <asp:BoundField DataField="Workcode" HeaderText="Complaint Work Code" >
                    <HeaderStyle ForeColor="White" />
                </asp:BoundField>

                <asp:BoundField DataField="ContactNumber" HeaderText="Contact Number" />

                <asp:CommandField ShowSelectButton="True" SelectText="Details" > 
		<ItemStyle ForeColor="#d7182a" Font-Bold="True" Font-Underline="True" />
                </asp:CommandField>
                
            </Columns>
            <EmptyDataTemplate>
            <div align="center">No records found.</div>
            </EmptyDataTemplate>
            <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
      </asp:GridView>
   </div>
        </div>
    </asp:Content>