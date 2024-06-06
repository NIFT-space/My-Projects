<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Complaint_form.aspx.cs" Inherits="NIFT_CMS.Complaint_form" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        .layout {
            padding: 5px;
        }
        .layouta {
            margin-top: 20px;
            padding-right: 70px;
            padding-left: 70px;
        }
        .gridtablearea2nd {
            background: #f9f9f9;
        }
        .column.bg-alt {
            padding-left: 50px;
            padding-top: 10px;
            overflow-y: scroll;
            height: 550px;
        }
        h3.hd-top {
            padding-top: 10px;
        }
        .select2-container--default .select2-selection--single {
            border: 1px solid #d7182a !important;
        }
    </style>
     <script type="text/javascript">
         window.addDash = function addDash(a) {
             var b = /(\D+)/g,
                 npa = '',
                 nxw = '',
                 last1 = '';
             a.value = a.value.replace(b, '');
             npa = a.value.substr(0, 5);
             nxw = a.value.substr(5, 7);
             last1 = a.value.substr(12, 1);
             a.value = npa + '-' + nxw + '-' + last1;
         }
        
         $(function () {
             $("[id*=ddl_wcode]").select2();
         });
     </script>
<%--    t1.Visible = true;
                    t2.Visible = true;
                    t3.Visible = true;
                    t4.Visible = true;
                    t5.Visible = true;
                    t6.Visible = true;
                    p_details.InnerText = "Complaint Description";--%>
   <%-- <script src="https://code.jquery.com/ui/1.13.2/jquery-ui.min.js" 
        integrity="sha256-lSjKY0/srUM9BE3dPm+c4fBo1dky2v27Gdjm2uoZaL0="
  crossorigin="anonymous"></script>
    <script>
        function rd1click() {
            document.getElementById("t1").style.display = "none";
            document.getElementById("t2").style.display = "none";
            document.getElementById("t3").style.display = "none";
            document.getElementById("t4").style.display = "none";
            document.getElementById("t5").style.display = "none";
            document.getElementById("t6").style.display = "none";
            document.getElementById("p_details").style.InnerText = "Complaint Description";
        }
        
    </script>--%>
    <%--<script type="text/javascript">
        
        function keyUP(txt) {
            if (txt.length == 5) {
                var $next = $(this).next('#cnic2');
                if ($next.length)
                    $(this).next('#cnic3').focus();
                else
                    $(this).blur();
            }
        }
   --%>
    <div class="main_layout">
        <%--/////////////HEADER///////--%>
        <div class="Head_div" >
            <div class="column Main">
                <ul><li class="dropdown">
                <a href="#">MENU ▾</a>
                <ul class="dropdown-menu">
                <%--<li class="lia"><a id="link2" runat="server">In-Progress</a></li>
                <li class="lia"><a id="link3" runat="server">Pending Complaints</a></li>--%>
                <li class="lia"><a id="link6" runat="server">Open Complaints</a></li>
                <li class="lia"><a id="link4" runat="server">Closed Complaints</a></li>
                <li class="lia" id="lr_" runat="server" visible="false"><a id="link5" runat="server">Reports</a></li>
                <li class="lia" id="lr_7" runat="server" visible="false"><a id="link7" runat="server">Operations Dashboard</a></li>
                </ul></li></ul>
            </div>
            <asp:Label runat="server" ID="lbl_user" CssClass="banklogoclass"></asp:Label>
            <asp:Button ID="sign_out" Text="SIGNOUT" CssClass="btn_upload" runat="server" OnClick="sign_out_Click" CausesValidation="False" UseSubmitBehavior="False" />
        </div>
        <h3 style="color:white; text-align: center; margin-top: 10px;margin-left: 100px;">COMPLAINT MANAGEMENT SYSTEM</h3>
        
        <%-- /////////////// --%>

        <div class="btn_div">
            <asp:Button ID="Btn_back" CssClass="btn_ticket" runat="server" Text="BACK"
                 UseSubmitBehavior="False" CausesValidation="False" OnClick="Btn_back_Click" />
        </div>
        <div class="layouta">
            <div class="gridtablearea2nd">
            <asp:Label ID="title1" runat="server"></asp:Label>

                <h3 class="hd-top" runat="server">Complaint Submission Form</h3>
 <div class="column bg-alt">
    <asp:label ID="mandatory" class="sub-title-mandatory" runat="server">*Mandatory fields</asp:label>
    <br />

    <div class="gridtablearea2nd table60lida">
    <table cellpadding="7">
    <tr>
    <td id="nature" class="tidi" runat="server">Nature: <span class="star-valid">*</span></td>
    <td class="tidi">
        <asp:RadioButton ID="RadioButton1" CssClass="RadioButton1" runat="server" Text="Complaint" GroupName="nature" OnCheckedChanged="RadioButton1_CheckedChanged" AutoPostBack="True"  />
        <asp:RadioButton ID="RadioButton2" CssClass="RadioButton1" runat="server" Text="Query" GroupName="nature" OnCheckedChanged="RadioButton2_CheckedChanged" AutoPostBack="True" />
    </tr>
    
    <tr  runat="server" id="t1">
    <td id="work_code" class="tidi" runat="server">Complaint Work Code:<span class="star-valid">*</span></td>
    <td class="tidi"><asp:DropDownList ID="ddl_wcode" class="ddl_wcode" runat="server">
    </asp:DropDownList>
    </td>
    </tr>

    <tr>
    <td id="lbl_name" class="tidi" runat="server">Customer Name: <span class="star-valid">*</span></td>
    <td class="tidi">
        <asp:TextBox ID="c_name" runat="server" MaxLength="50" autocomplete="off" ></asp:TextBox>
        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="c_name" Display="Dynamic"
           ErrorMessage="Please Enter Valid Name" ForeColor="Red" ValidationExpression="^[ A-Za-z'`]*$"></asp:RegularExpressionValidator>
    </td>
    </tr>

        <tr>
    <td class="tidi" runat="server">Customer Email-Address: <span class="star-valid">*</span></td>
    <td class="tidi">
        <asp:TextBox ID="email_" runat="server" MaxLength="30" autocomplete="off" ></asp:TextBox>
        <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="email_" Display="Dynamic"
              ErrorMessage="Please Enter Valid Email-Address" ForeColor="Red" 
              ValidationExpression="^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$">  
        </asp:RegularExpressionValidator>
    </td>
    </tr>

        <tr>
    <td class="tidi" runat="server">Customer Contact Number: <span class="star-valid">*</span></td>
    <td class="tidi">
        <asp:TextBox ID="mobile_" runat="server" MaxLength="11" autocomplete="off" onkeypress="return onlyNumbers(this);" ></asp:TextBox>
        <%--<asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ControlToValidate="mobile_" Display="Dynamic"
              ErrorMessage="Please Enter Valid Contact Number" ForeColor="Red" ValidationExpression="^0*(300[1-8][0-9]{6}|3009[0-8][0-9]{5}|30099[0-8][0-9]{4}|300999[0-8][0-9]{3}|3009999[0-8][0-9]{2}|30099999[0-8][0-9]|300999999[0-9]|30[1-9][0-9]{7}|3[1-9][0-9]{8})$">  
        </asp:RegularExpressionValidator>--%>
    </td>
    </tr>
        
    <tr>
    <td id="lbl_cnic" class="tidi" runat="server">Customer CNIC<span class="star-valid">*</span></td>
    <td class="tidi">
    <asp:TextBox ID="cnic_" runat="server" onkeyup="addDash(this)" MaxLength="15" autocomplete="off" ></asp:TextBox>
        <span class="blue-valid"></span>
        <asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server" ControlToValidate="cnic_" Display="Dynamic"
              ErrorMessage="Please Enter Valid CNIC Without Dash" ForeColor="Red" ValidationExpression="^[0-9-]*$">
        </asp:RegularExpressionValidator>
        <%--<asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server" ControlToValidate="cnic_" Display="Dynamic"
              ErrorMessage="Please Enter Valid CNIC" ForeColor="Red" ValidationExpression="\d{5}[-|]\d{7}[-|]\d">  
        </asp:RegularExpressionValidator>--%>
    </td>
    </tr>

    <tr runat="server" id="t2">
    <td id="acc_num" class="tidi" runat="server">Customer Account Number<span class="star-valid">*</span></td>
    <td class="tidi">
    <asp:TextBox ID="acc_no" runat="server" MaxLength="24" autocomplete="off" placeholder="Please enter Account Number" ></asp:TextBox><span runat="server" id="a1" class="blue-valid"></span>
        <asp:RegularExpressionValidator ID="RegularExpressionValidator5" runat="server" ControlToValidate="acc_no" Display="Dynamic"
              ErrorMessage="Please Enter Valid Account Number Without Dash" ForeColor="Red" ValidationExpression="^[0-9]*$"></asp:RegularExpressionValidator>
        <%--<asp:RegularExpressionValidator ID="RegularExpressionValidator5" runat="server" ControlToValidate="acc_no" Display="Dynamic"
              ErrorMessage="Please Enter Valid Account Number" ForeColor="Red" ValidationExpression="^[A-Z0-9]{24}"></asp:RegularExpressionValidator>--%>
    </td>
    </tr>

    <tr runat="server" id="t3">
    <td id="lbltran_date" class="tidi" runat="server">Transaction Date<span class="star-valid">*</span></td>
    <td class="tidi">
    <asp:TextBox ID="txtFromDate" runat="server" placeholder="Date/Month/Year" TextMode="Date"></asp:TextBox>
    </td>
    </tr>

    <tr runat="server" id="t4">
    <td id="lblstan" class="tidi" runat="server">Transaction STAN<span class="blue-valid"> (Optional)</span></td>
    <td class="tidi"><asp:TextBox ID="stan_" runat="server" MaxLength="15" ></asp:TextBox>
        <asp:RegularExpressionValidator ID="RegularExpressionValidator6" runat="server" ControlToValidate="stan_" Display="Dynamic"
              ErrorMessage="Please Enter Valid STAN" ForeColor="Red" ValidationExpression="^[0-9a-zA-Z]*$">  
        </asp:RegularExpressionValidator>
    </td>
    </tr>
     
    <tr runat="server" id="t5">
    <td id="title" class="tidi" runat="server">Transaction Ref No.<span runat="server" class="blue-valid"> (Auth Code)</span></td>
    <td class="tidi"><asp:TextBox ID="TRNrefno" runat="server" MaxLength="24" placeholder="Please enter Transaction Ref Number" ></asp:TextBox><span runat="server" id="a2" class="blue-valid"></span>
        <asp:RegularExpressionValidator ID="RegularExpressionValidator7" runat="server" ControlToValidate="TRNrefno" Display="Dynamic"
              ErrorMessage="Please Enter Valid Ref No." ForeColor="Red" ValidationExpression="^[0-9a-zA-Z]*$">  
        </asp:RegularExpressionValidator>
    </td>
    </tr>

    <tr runat="server" id="Tr1">
    <td id="Td1" class="tidi" runat="server">Amount<span class="star-valid">*</span></td>
    <td class="tidi"><asp:TextBox ID="Amount_" runat="server" MaxLength="16" type="number"></asp:TextBox>
        <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ControlToValidate="Amount_" Display="Dynamic"
              ErrorMessage="Please Enter Valid Amount" ForeColor="Red" ValidationExpression="^[0-9.]*$">  
        </asp:RegularExpressionValidator>
    </td>
    </tr>

    <tr>
    <td ><p id="p_details" runat="server" style="font-size: 14px;display: inline;"></p><span class="star-valid">*</span></td>
    <td class="tidi">
    <asp:TextBox ID="details_" CssClass="New_Details" runat="server" TextMode="MultiLine" Rows="6" MaxLength="2000" ></asp:TextBox>
        <asp:RegularExpressionValidator ID="RegularExpressionValidator8" runat="server" ControlToValidate="details_" Display="Dynamic"
              ErrorMessage="Please Enter Valid Details." ForeColor="Red" ValidationExpression="^[ 0-9a-zA-Z\n-_()`'.,?&/]*$">  
        </asp:RegularExpressionValidator>
    </td>
    </tr>

    <tr runat="server" id="t6">
    <td id="doc_upload" class="tidi" runat="server">Document Upload<span class="star-valid"></span></td>
    <td class="tidi">
        <label style="color:red;font-weight:300">DOCX,JPG,PNG,PDF,XLSX formats allowed/File Limit is 5MB</label>
        <asp:FileUpload ID="FileUpload1" CssClass="fileupload" runat="server" />
            <asp:RegularExpressionValidator ID="RegularExpressionValidator90" runat="server"
                        ControlToValidate="FileUpload1"
                        ErrorMessage="Invalid File!" ForeColor="Red"
                        ValidationExpression="([a-zA-Z0-9 \s()_\\.\-:])+(.png|.PNG|.JPG|.jpg|.pdf|.doc|.docx|.xlsx)$"></asp:RegularExpressionValidator>
        <%--<br />
        <asp:FileUpload ID="FileUpload2" CssClass="fileupload" runat="server" />
            <asp:RegularExpressionValidator ID="RegularExpressionValidator9" runat="server"
                        ControlToValidate="FileUpload2"
                        ErrorMessage="Incorrect File" ForeColor="Red"
                        ValidationExpression="([a-zA-Z0-9 \s()_\\.\-:])+(.png|.PNG|.JPG|.jpg|.pdf|.doc|.docx)$"></asp:RegularExpressionValidator>
        <br />
        <asp:FileUpload ID="FileUpload3" CssClass="fileupload" runat="server" />
            <asp:RegularExpressionValidator ID="RegularExpressionValidator10" runat="server"
                        ControlToValidate="FileUpload3"
                        ErrorMessage="Incorrect File" ForeColor="Red"
                        ValidationExpression="([a-zA-Z0-9 \s()_\\.\-:])+(.png|.PNG|.JPG|.jpg|.pdf|.doc|.docx)$"></asp:RegularExpressionValidator>--%>
    </td>
    </tr>

    <tr>
    <td>
    <asp:Button ID="btn_Submit" UseSubmitBehavior="true" Text="SUBMIT" CssClass="btn_MainSubmit"  runat="server" OnClick="btn_Submit_Click" />
    </td>
    </tr>
        
    </table>
        </div>

        </div>
            </div>
        </div>
    </div>
    
</asp:Content>
