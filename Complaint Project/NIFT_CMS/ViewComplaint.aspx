<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="ViewComplaint.aspx.cs" Inherits="NIFT_CMS.ViewComplaint" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        .layouta {
            margin-top: 20px;
            padding-right: 70px;
            padding-left: 70px;
        }
        .gridtablearea {
            background: #f9f9f9;
        }
        h3.hd-top {
            padding-top: 10px;
        }
        .txtName2 {
            max-width: 100%;
            width: 100%;
            padding: 4px;
            border: 1px solid #d7182a;
            /*text-transform: uppercase;*/
        }
        .column.bg-alt {
            padding-left: 50px;
            padding-top: 10px;
            overflow-y: scroll;
            height: 550px;
        }
        .container {
            height: 572px;
        }
        .btnpg {
            color: #d7182a;
            border: 1px solid #d7182a;
            padding: 5px;
        }
        .ddl_merchants {
            width: 100% !important;
            padding: 6px !important;
        }
    </style>

    
<script type="text/javascript">
    $(function () {
        $("[id*=ddl_merchants]").select2();
    });
</script>
    
    <script type = "text/javascript">
        function Confirm() {
            var confirm_value = document.createElement("INPUT");

            confirm_value.type = "hidden";
            confirm_value.name = "confirm_value";

            if (confirm("Please Confirm!")) {
                confirm_value.value = "YES";
            }
            else {
                confirm_value.value = "NO";
            }

            document.forms[0].appendChild(confirm_value);
        }

        function Show() {
            document.getElementById('popupBox').style.display='block';
            document.getElementById('popupBackground').style.display = 'block';
            document.body.style.overflow = 'hidden';
        }

        function Hide() {
            document.getElementById('popupBox').style.display = 'none';
            document.getElementById('popupBackground').style.display = 'none';
        }
        
    </script>
    <script type="text/javascript">
    window.onload = function () {
    var div = document.getElementById("dvScroll");
    var div_position = document.getElementById("div_position");
    var position = parseInt('<%=Request.Form["div_position"] %>');
    if (isNaN(position)) {
        position = 0;
    }
    div.scrollTop = position;
    div.onscroll = function () {
        div_position.value = div.scrollTop;
        };
    };
    </script>
    <div class="main_layout">
            <%--/////////////HEADER///////--%>
                <div class="Head_div">
                    <div class="column Main">
                   
                    <ul><li class="dropdown">
                        <a href="#">MENU ▾</a>
                    <ul class="dropdown-menu">
                        <li class="lia"><a id="link1" runat="server">Initiate Complaint</a></li>
                        <%--<li class="lia"><a id="link2" runat="server">In-Progress</a></li>
                        <li class="lia"><a id="link3" runat="server">Pending Complaints</a></li>--%>
                        <li class="lia"><a id="link6" runat="server">Open Complaints</a></li>
                        <li class="lia"><a id="link4" runat="server">Closed Complaints</a></li>
                        <li class="lia" id="lr_" runat="server" visible="false"><a id="link5" runat="server">Reports</a></li>
                        <li class="lia" id="lr_7" runat="server" visible="false"><a id="link7" runat="server">Operations Dashboard</a></li>
                    </ul></li></ul>
                        </div>
                            <asp:Label runat="server" ID="lbl_user" CssClass="banklogoclass"></asp:Label>
                    <asp:Button ID="sign_out" Text="SIGNOUT" CssClass="btn_upload" runat="server" OnClick="sign_out_Click" UseSubmitBehavior="false" CausesValidation="False" />
               </div>
                    <h3 style="color:white; text-align: center; margin-top: 10px;margin-left: 100px;">COMPLAINT MANAGEMENT SYSTEM</h3>
                
            <%-- /////////////// --%>
                
                <div class="btn_div">
                    <asp:Button ID="Btn_back" CssClass="btn_ticket" runat="server" Text="BACK" 
                    UseSubmitBehavior="False" CausesValidation="False" OnClick="Btn_back_Click" />
                <asp:Button ID="Btn_Close" CssClass="btn_ticket" runat="server" Text="CLOSE COMPLAINT" OnClick="Btn_Close_Click"
                    UseSubmitBehavior="False" />
                <asp:Button ID="Btn_Open" CssClass="btn_ticket" runat="server" Text="RE-OPEN COMPLAINT" OnClientClick="Confirm()"
                    UseSubmitBehavior="False" OnClick="Btn_Open_Click" Visible="false" />
                <asp:Label ID="TopLabel1" class="Message" runat="server" ForeColor="Red" ></asp:Label>
                    </div>
                <div id="popupBox" class="white_content">
                        <h3 style=" text-align: center; margin-top: 5px;">ADD COMMENTS<span class="star-valid">*</span></h3>
                        <asp:TextBox ID="txt_close_comment" runat="server" Height="50%" TextMode="MultiLine" Width="80%" MaxLength="1000" ></asp:TextBox>
                        <br />
                    <div class="btns">
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="txt_close_comment" Display="Dynamic"
                                ErrorMessage="Please Enter Valid Details." ForeColor="Red" ValidationExpression="^[ 0-9a-zA-Z-_()`'.,?&/]{1,1000}$">
                        </asp:RegularExpressionValidator>
                        <br />
                        <asp:Button ID="btn_closepopup" Text="CANCEL" CssClass="btn_ticket" runat="server" CausesValidation="false" OnClick="btn_closepopup_Click" />
                        <asp:Button ID="btn_closecomm" Text="SUBMIT" CssClass="btn_ticket" runat="server" UseSubmitBehavior="false" OnClick="btn_closecomm_Click" />
                        <br />
                        <%--<asp:Label runat="server" ID="lbl_popupmsg" ForeColor="Red" display="Dynamic"></asp:Label>--%>
                    </div>
                </div>
                <div id="popupBackground" class="black_overlay"></div>
        <div class="layouta">
                 <div class="gridtablearea table60lida table-report-12">
                     <h3 class="hd-top" runat="server">Complaint Details</h3>
                    <div class="column bg-alt" id="dvScroll">
            <table cellpadding="7" >
            <tr>
            <td class="tidi" runat="server">Complaint Status</td>
            <td class="tidi"><asp:Label id="c_status" runat="server"></asp:Label></td>
            </tr>

            <tr>
            <td class="tidi" runat="server">Complaint Date</td>
            <td class="tidi"><asp:Label id="comp_date" runat="server"></asp:Label></td>
            </tr>
                            
            <tr>
            <td class="tidi" runat="server">Last Updated</td>            
            <td class="tidi"><asp:Label id="LastUpdated" runat="server"></asp:Label></td>
            </tr>

            <tr>
            <td class="tidi" runat="server">Customer Name: </td>
            <td class="tidi"><asp:Label id="comp_name" runat="server"></asp:Label></td>
            </tr>

            <tr>
            <td class="tidi" runat="server">Customer Email Address</td>
            <td class="tidi"><asp:Label id="emailname" runat="server"></asp:Label></td>
            </tr>

            <tr>
            <td class="tidi" runat="server">Customer Contact Number</td>
            <td class="tidi"><asp:Label id="contact_num" runat="server"></asp:Label></td>
            </tr>
                
            <tr>
            <td class="tidi" runat="server">Customer CNIC</td>            
            <td class="tidi"><asp:Label id="cnic" runat="server"></asp:Label></td>
            </tr>

            <tr>
            <td class="tidi" runat="server">Nature Of Complaint</td>
            <td class="tidi"><asp:Label id="nature" runat="server"></asp:Label></td>
            </tr>

            <tr>
            <td class="tidi" runat="server">Work Code</td>
            <td class="tidi"><asp:Label id="work_code" runat="server"></asp:Label></td>
            </tr>

            <tr>
            <td class="tidi" runat="server">Complaint Description</td>            
            <td class="tidi"><asp:Label id="c_desc" runat="server"></asp:Label>
            </td>
            </tr>

            <tr>
            <td class="tidi" runat="server">Bank Name</td>            
            <td class="tidi"><asp:Label id="instid" runat="server"></asp:Label></td>
            </tr>

            <%--<tr>
            <td class="tidi" runat="server">Branch Name</td>            
            <td class="tidi"><asp:Label id="BranchID" runat="server"></asp:Label></td>
            </tr>--%>

            <tr>
            <td class="tidi" runat="server">Account Number</td>            
            <td class="tidi"><asp:Label id="Acc_num" runat="server"></asp:Label></td>
            </tr>

            <tr>
            <td class="tidi" runat="server">Tran STAN</td>            
            <td class="tidi"><asp:Label id="TranSTAN" runat="server"></asp:Label></td>
            </tr>

            <tr>
            <td class="tidi" runat="server">Tran Reference Number</td>            
            <td class="tidi"><asp:Label id="TranRefNo" runat="server"></asp:Label></td>
            </tr>

            <tr>
            <td class="tidi" runat="server">Tran Date</td>            
            <td class="tidi"><asp:Label id="TranDate" runat="server"></asp:Label></td>
            </tr>

            <tr>
            <td class="tidi" runat="server">Amount</td>            
            <td class="tidi"><asp:Label id="Amt_" runat="server"></asp:Label></td>
            </tr>

            <tr>
            <td class="tidi" runat="server">Complaint Duration</td>            
            <td class="tidi"><asp:Label id="comp_dur" runat="server"></asp:Label></td>
            </tr>
                
            <tr>
            <td class="tidi" runat="server">Remaining Days</td>            
            <td class="tidi"><asp:Label id="rem_days" runat="server"></asp:Label></td>
            </tr>

            <tr>
            <td class="tidi" runat="server">Initiator</td>            
            <td class="tidi"><asp:Label id="InitiatorID" runat="server"></asp:Label></td>
            </tr>

            <tr runat="server" id="div_assignee">
            <td class="tidi" runat="server">Assignee</td>
            <td class="tidi"><asp:Label id="AssigneeID" runat="server"></asp:Label>
            <asp:DropDownList runat="server" ID="ddl_assignee" Visible="false" AutoPostBack="True" CausesValidation="True" 
                OnSelectedIndexChanged="ddl_assignee_SelectedIndexChanged"  size="5" ></asp:DropDownList>
                <asp:DropDownList runat="server" ID="ddl_sub_assignee" Visible="false" CausesValidation="True" size="5"></asp:DropDownList>
                <asp:DropDownList runat="server" CssClass="ddl_merchants" ID="ddl_merchants" Visible="false" CausesValidation="True"></asp:DropDownList>
            </td>
            </tr>

            <tr id="hid_" runat="server" visible="false">
            <td class="tidi" runat="server">Complainant Evidence</td>
            <%--<td class="tidi">
                <asp:GridView CssClass="EvdGrid" BorderWidth="0px" runat="server" Width="100%" ID="grid_evd" AutoGenerateColumns="False" GridLines="Horizontal" 
                    OnSelectedIndexChanged="grid_evd_SelectedIndexChanged" ShowHeader="False" OnRowDeleting="grid_evd_RowDeleting" OnRowDataBound="grid_evd_RowDataBound">
                    <PagerStyle CssClass="gridviewPager" HorizontalAlign="Center" />
                <Columns>
                    <asp:BoundField DataField="EvdID" HeaderText="ID" HeaderStyle-CssClass = "hideGridColumn" ItemStyle-CssClass="hideGridColumn" >
                    <HeaderStyle CssClass="hideGridColumn"></HeaderStyle>
                    <ItemStyle CssClass="hideGridColumn"></ItemStyle>
                    </asp:BoundField>
                    <asp:BoundField DataField="EvdName1" HeaderStyle-CssClass = "Evdcol" ItemStyle-CssClass="Evdcol">
                    <HeaderStyle CssClass="Evdcol"></HeaderStyle>
                    <ItemStyle CssClass="Evdcol"></ItemStyle>
                    </asp:BoundField>

                   

                    <asp:ButtonField Text="Download" CommandName="Select" />

                    <asp:TemplateField>
                        <ItemTemplate>
                           <asp:LinkButton ID="LinkButton1" Visible="false" runat="server" CausesValidation="False"
                               CommandName="Delete" Text="Delete" OnClientClick="Confirm()"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                </asp:GridView>
            </td>
            </tr>

                <tr>
                    <td>
                        </td>
                    </tr>--%>
                    <td class="tidi">
                        <asp:Repeater ID="Admin_Repeater" runat="server" OnItemCommand="Admin_Repeater_ItemCommand" OnItemDataBound="Admin_Repeater_ItemDataBound" >
                        <ItemTemplate>
                            <table>
                                <tr runat="server" id="hid_1">
                                    <td style="width:60%; padding-top: 10px;">
                                    <b><asp:Label ID="EvdName1" runat="server" Text='<%#Eval("EvdName1") %>'></asp:Label></b>
                                    </td>
                                    <td style="width:25%; border:0px solid">
                                    <asp:LinkButton ID="LinkButton5" runat="server" CommandArgument='<%#Eval("EvdID") %>' Text="Download" CommandName="down1"></asp:LinkButton>
                                    </td>
                                    <td style="width:25%; border:0px solid">
                                    <asp:LinkButton ID="lbdown" runat="server" CommandArgument='<%#Eval("EvdID") %>' Text="Delete" CommandName="del1"></asp:LinkButton>
                                    </td>
                                </tr>

                                <tr runat="server" id="hid_2">
                                    <td style="width:60%; padding-top: 10px;">
                                    <b><asp:Label ID="EvdName2" runat="server" Text='<%#Eval("EvdName2") %>'></asp:Label></b>
                                    </td>
                                    <td style="width:25%; border:0px solid">
                                    <asp:LinkButton ID="LinkButton4" runat="server" CommandArgument='<%#Eval("EvdID") %>' Text="Download" CommandName="down2"></asp:LinkButton>
                                    </td>
                                    <td style="width:25%; border:0px solid">
                                    <asp:LinkButton ID="LinkButton2" runat="server" CommandArgument='<%#Eval("EvdID") %>' Text="Delete" CommandName="del2"></asp:LinkButton>
                                    </td>
                                </tr>
                            

                                <tr runat="server" id="hid_3">
                                    <td style="width:60%; padding-top: 10px;">
                                    <b><asp:Label ID="EvdName3" runat="server" Text='<%#Eval("EvdName3") %>'></asp:Label></b>
                                    </td>
                                    <td style="width:25%; border:0px solid">
                                    <asp:LinkButton ID="LinkButton1" runat="server" CommandArgument='<%#Eval("EvdID") %>' Text="Download" CommandName="down3"></asp:LinkButton>
                                    </td>
                                    <td style="width:25%; border:0px solid">
                                    <asp:LinkButton ID="LinkButton3" runat="server" CommandArgument='<%#Eval("EvdID") %>' Text="Delete" CommandName="del3"></asp:LinkButton>
                                    </td>
                                </tr>
                            </table>
                          </ItemTemplate>
                        </asp:Repeater>
                    </td>
                </tr>
            
            <%--<tr runat="server" id="div_doc_upload" visible="false">
            <td id="doc_upload" class="tidi" runat="server">Document Upload<span class="star-valid">*</span></td>
            <td class="tidi">
            <asp:FileUpload ID="FileUpload1" CssClass="fileupload" runat="server" />
                <asp:RegularExpressionValidator ID="RegExValFileUploadFileType" runat="server"
                        ControlToValidate="FileUpload1"
                        ErrorMessage="Note: Only DOCX,JPG,PNG,PDF,XLSX files are allowed to be uploaded/File Limit is 5MB" ForeColor="Red"
                        ValidationExpression="([a-zA-Z0-9 \s()_\\.\-:])+(.png|.PNG|.JPG|.jpg|.pdf|.doc|.docx|.xlsx)$"></asp:RegularExpressionValidator></td>
            </tr>--%>
            </table>
                    <asp:Button runat="server" UseSubmitBehavior="False" CssClass="btn_ticket" ID="btn_up" Text="Update" OnClick="btn_up_Click" Visible="False" />
            
                <%--<div class="comment-area">
                    <h4 id="h1" runat="server">EVIDENCE:</h4>
                    <asp:Repeater ID="evdRepeater" runat="server" OnItemCommand="evdRepeater_ItemCommand">
                    <ItemTemplate>
                        <div class="rep_div">
                            <b><asp:Label runat="server" Text="   Posted By:" ></asp:Label></b>
                            <asp:Label ID="lbl_user" runat="server" Text='<%#Eval("EvdPoster") %>' ></asp:Label>
                            <br />
                                <b><asp:Label runat="server" Text="Attachment:" ></asp:Label></b>
                                <asp:Label runat="server" Text='<%#Eval("EvdName") %>'></asp:Label>
                                <br />
                                <asp:LinkButton  runat="server" CommandArgument='<%#Eval("EvdID") %>' Text="Download" CommandName="download"></asp:LinkButton>
                                
                                <asp:LinkButton ID="lbdlt" runat="server" CommandArgument='<%#Eval("EvdID") %>' Text=" - Delete"
                                    CommandName="delete" OnClientClick="Confirm()"></asp:LinkButton>
                            </div>
                            <br />
                      </ItemTemplate>
                        </asp:Repeater>
                    
                        <div style="margin-left:260px;margin-bottom:10px">
                            <asp:Repeater ID="evdpgrpt" runat="server" OnItemCommand="evdpgrpt_ItemCommand" OnItemDataBound="evdpgrpt_ItemDataBound">
                            <ItemTemplate>
                            <asp:LinkButton ID="btnPageevd" CssClass="btnpg" CommandName="Page" CommandArgument="<%# Container.DataItem %>"
                                runat="server" CausesValidation="false"><%# Container.DataItem %>
                            </asp:LinkButton>
                            </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </div>--%>
                    <%--<table class="tblcmnt" style="width: 100%;">
                    <tr>
                    <td id="Td1" class="tidi" runat="server">Document Upload<span class="star-valid">*</span></td>
                    <td class="tidi">

                    <%--<asp:FileUpload ID="FileUpload2" CssClass="fileupload" runat="server" />
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="FileUpload2"
                                ErrorMessage="Invalid File" ForeColor="Red"
                                ValidationExpression="([a-zA-Z0-9 \s()_\\.\-:])+(.png|.PNG|.JPG|.jpg|.pdf|.doc|.docx|.xlsx)$"></asp:RegularExpressionValidator>
                        <br />
                        <asp:FileUpload ID="FileUpload3" CssClass="fileupload" runat="server" />
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ControlToValidate="FileUpload3"
                                ErrorMessage="Invalid File" ForeColor="Red"
                                ValidationExpression="([a-zA-Z0-9 \s()_\\.\-:])+(.png|.PNG|.JPG|.jpg|.pdf|.doc|.docx|.xlsx)$"></asp:RegularExpressionValidator>
                        <br />
                        <asp:FileUpload ID="FileUpload4" CssClass="fileupload" runat="server" />
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server" ControlToValidate="FileUpload4"
                                ErrorMessage="Invalid File" ForeColor="Red"
                                ValidationExpression="([a-zA-Z0-9 \s()_\\.\-:])+(.png|.PNG|.JPG|.jpg|.pdf|.doc|.docx|.xlsx)$"></asp:RegularExpressionValidator>
                    </td>
                    </tr>
                    <tr>
                    <td>
                    <asp:Button CssClass="btn_ticket" ID="btn_Upload" runat="server" 
                            Text="Upload Evidence" UseSubmitBehavior="False" OnClick="btn_Upload_Click" />
                        <%--<asp:Label ID="lbl_Message" class="Message" runat="server" ForeColor="Red" ></asp:Label>
                    </td>
                    </tr>
                    </table>--%>

                <div class="comment-area" >
                <h4 id="h4" runat="server">COMMENTS:</h4>
                    <asp:Repeater ID="Repeater1" runat="server" OnItemCommand="Repeater1_ItemCommand" OnItemDataBound="Repeater1_ItemDataBound" >
                    <ItemTemplate>
                        <div class="rep_div"><b><asp:Label runat="server" Text="   User:" ></asp:Label></b>
                            <asp:Label ID="lbl_user" runat="server" Text='<%#Eval("Fullname") %>' ></asp:Label>
                            
                            <b><asp:Label runat="server" Text=" - " ></asp:Label></b>
                            <asp:Label ID="lbl_ComDate" runat="server" Text='<%#Eval("Commentdt") %>' ></asp:Label>
                            <br />
                            <b><asp:Label runat="server" Text="Comment:" ></asp:Label></b>
                            <asp:Label ID="lbl_ComTxt" runat="server" Text='<%#Eval("Description") %>' ></asp:Label>

                            <div id="hid" runat="server">
                            <b><asp:Label runat="server" Text="Evidence Posted By:" ></asp:Label></b>
                            <asp:Label ID="Label1" runat="server" Text='<%#Eval("EvdPoster") %>' ></asp:Label>
                            
                            <div runat="server" id="hid_1">
                            <b>Attachment : <asp:Label ID="EvdName1" runat="server" Text='<%#Eval("EvdName1") %>'></asp:Label></b>
                            <asp:LinkButton ID="lbdown" runat="server" CommandArgument='<%#Eval("EvdID") %>' Text="Download" CommandName="download1"></asp:LinkButton>
                            </div>
                            
                            <div runat="server" id="hid_2">
                            <b>Attachment : <asp:Label ID="EvdName2" runat="server" Text='<%#Eval("EvdName2") %>'></asp:Label></b>
                            <asp:LinkButton ID="LinkButton2" runat="server" CommandArgument='<%#Eval("EvdID") %>' Text="Download" CommandName="download2"></asp:LinkButton>
                                </div>
                            
                            <div runat="server" id="hid_3">
                            <b>Attachment : <asp:Label ID="EvdName3" runat="server" Text='<%#Eval("EvdName3") %>'></asp:Label></b>
                            <asp:LinkButton ID="LinkButton3" runat="server" CommandArgument='<%#Eval("EvdID") %>' Text="Download" CommandName="download3"></asp:LinkButton>
                                    </div>
                            <%--<asp:LinkButton ID="lbdlt" runat="server" CommandArgument='<%#Eval("EvdID") %>' Text=" - Delete"
                              CommandName="delete" OnClientClick="Confirm()"></asp:LinkButton>--%>
                            </div>
                         </div>
                         <br />
                        <%--<asp:GridView CssClass="EvdGrid" runat="server" ID="comm_evd" AutoGenerateColumns="False" GridLines="Horizontal" ShowHeader="False">
                <Columns>
                    <asp:BoundField DataField="EvdID" HeaderText="ID" HeaderStyle-CssClass = "hideGridColumn" ItemStyle-CssClass="hideGridColumn" />
                    <asp:BoundField DataField="EvdName" />
                    <asp:ButtonField Text="Download" CommandName="Select" />
                </Columns>
                </asp:GridView>--%>

                      </ItemTemplate>
                        </asp:Repeater>
                    
                    <div style="margin-left:260px;margin-bottom:10px">
                        <asp:Repeater ID="rptPaging" runat="server" OnItemCommand="rptPaging_ItemCommand" OnItemDataBound="rptPaging_ItemDataBound">
                        <ItemTemplate>
                        <asp:LinkButton ID="btnPage" CssClass="btnpg" CommandName="Page" CommandArgument="<%# Container.DataItem %>"
                            runat="server" CausesValidation="false"><%# Container.DataItem %>
                        </asp:LinkButton>
                        </ItemTemplate>
                        </asp:Repeater>
                    </div>
                    </div>
                
                <table class="tblcmnt" style="width: 100%;">
                    <tr>
                    <td id="cmntnme" class="tidi" runat="server">Name</td>
                    <td> <asp:TextBox ID="txtName" CssClass="txtName2" runat="server" Enabled="False" ></asp:TextBox></td>
                    </tr>
                    
                    <tr>
                    <td id="comments" class="tidi" runat="server">Comment</td>
                    <td> <asp:TextBox ID="txtComment" CssClass="txtName2" runat="server" TextMode="MultiLine" ></asp:TextBox>
                         <asp:RegularExpressionValidator ID="RegularExpressionValidator8" runat="server" ControlToValidate="txtComment" Display="Dynamic"
                                      ErrorMessage="Please Enter Valid Details." ForeColor="Red" ValidationExpression="^[ 0-9a-zA-Z-_()`'.,?&/]*$">  
                                </asp:RegularExpressionValidator>
                    </td> 
                    </tr>

                    <tr>
                    <td id="Td2" class="tidi" runat="server">Document Upload<span class="star-valid">*</span></td>
                    <td class="tidi">
                        <label style="color:red;font-weight:300;">DOCX,JPG,PNG,PDF,XLSX formats allowed/File Limit is 5MB</label>
                        <asp:FileUpload ID="FileUpload1" CssClass="fileupload" runat="server" />
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="FileUpload1"
                                ErrorMessage="Invalid File" ForeColor="Red"
                                ValidationExpression="([a-zA-Z0-9 \s()_\\.\-:])+(.png|.PNG|.JPG|.jpg|.pdf|.doc|.docx|.xlsx)$"></asp:RegularExpressionValidator>
                    <br />
                    </td>
                    </tr>

                    <tr>
                    <td>
                    <asp:Button CssClass="btn_ticket" ID="btn_Submit" runat="server" 
                            Text="Post Comment" UseSubmitBehavior="False" OnClick="btn_Submit_Click"/>
                    </td>
                    </tr>
                    
                    </table>
                 </div>
                <input type="hidden" id="div_position" name="div_position" />
                    </div>
    </div>
        </div>
</asp:Content>