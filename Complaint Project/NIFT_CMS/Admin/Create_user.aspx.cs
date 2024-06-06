using IBCS.Data;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NIFT_CMS
{
    public partial class Create_user : System.Web.UI.Page
    {
        public SqlConnection oCon = new SqlConnection();
        public int statchnge = 0;
        public static string Orig_Pass, HASH;
        public static string Fullname, InstName;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Response.AddHeader("Expires", System.DateTime.Now.AddDays(-1).ToString());
                Response.AddHeader("Last-Modified", System.DateTime.Now.ToString());
                Response.AddHeader("Cache-Control", "no-cache, must-revalidate");
                Response.Cache.SetNoServerCaching();
                Response.Expires = -1;
                Response.CacheControl = "no-cache";
                Response.Cache.SetExpires(DateTime.Now.AddSeconds(0));
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetValidUntilExpires(false);
                Response.Cache.SetNoStore();
                Response.AppendHeader("Pragma", "no-cache");
                Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);

                if (Session["UserID"] == null)
                {
                    Response.Clear();
                    Response.Redirect("Sessionexpire", true);
                }
                if (Session["IiixA8"] == null)
                {
                    Response.Redirect("Sessionexpire");
                }

                bool isAllowed = false;
                string sUserId = Convert.ToString(Session["UserID"]).Trim();

                isAllowed = general.isPageAllowed(sUserId, "Createuser");

                if (isAllowed == false)
                {
                    Response.Clear();
                    Response.Redirect("Notallowed", true);
                }

                Session["Selected_Inst"] = ddl_inst.SelectedValue;
                //Session["Selected_Branch"] = ddl_branch.SelectedValue;

                if (Page.IsPostBack != true)
                {
                    string sRoleID, sUserID;
                    sRoleID = Request.QueryString["h_roleid"];
                    sUserID = Request.QueryString["h_userid"];

                    Session["sRoleID"] = sRoleID;
                    Session["sUserID"] = sUserID;

                    if (((string.IsNullOrEmpty(sRoleID) != true) || (string.IsNullOrEmpty(sUserID) != true)))
                    {
                        btn_Signup.Text = "UPDATE";
                        BindSQL(sRoleID, sUserID);
                    }
                    else
                    {
                        sRoleID = "0";
                        sUserID = "0";
                        BindSQL(sRoleID, sUserID);
                    }
                }
                /////////////DISPLAY USERNAME//////////////
                lbl_user.Text = GetFullName();
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }

        protected void BindSQL(string RoleID, string UserID)
        {
            try
            {
                String sSQL;
                cDataAccess odata = new cDataAccess();

                //code for Roles dropdownlist box
                sSQL = "select RoleID,Name from [role] order by name asc";
                DataTable dtrole = new DataTable();
                SqlDataAdapter adp_role = odata.GetDataAdapter(sSQL);

                adp_role.Fill(dtrole);

                cl_role.DataSource = dtrole;
                cl_role.DataTextField = dtrole.Columns["Name"].ColumnName.ToString().Trim();
                cl_role.DataValueField = dtrole.Columns["RoleID"].ColumnName.ToString().Trim();
                cl_role.DataBind();

                //code for Institution Name dropdownlist box
                sSQL = "select Trim(InstID)as InstID,InstName from institution where instid>0 order by instname ";
                DataTable dtinst = new DataTable();
                SqlDataAdapter adp_inst = odata.GetDataAdapter(sSQL);
                adp_inst.Fill(dtinst);

                ddl_inst.DataSource = dtinst;
                ddl_inst.DataTextField = dtinst.Columns["InstName"].ColumnName.ToString().Trim();
                ddl_inst.DataValueField = dtinst.Columns["InstID"].ColumnName.ToString().Trim();
                ddl_inst.DataBind();

                if ((RoleID == "0") && (UserID == "0"))
                {
                    ddl_inst.Items.Insert(0, "Select");
                }
                else
                {
                    ddl_inst.Items.Insert(0, "Select");
                    if (String.IsNullOrEmpty(Session["Selected_Inst"].ToString()) != true)
                    {
                        ddl_inst.Items.FindByValue(Session["Selected_Inst"].ToString()).Selected = true;
                    }
                }

                //code for Branch Name dropdownlist box
                //if ((RoleID == "0") && (UserID == "0"))
                //{
                //    ddl_branch.Items.Insert(0, "Select");
                //}
                //else
                //{
                //    sSQL = "select   STUFF( '0000', 4 - LEN( b.BranchID ) + 1, LEN( b.BranchID ), b.BranchID) +'-'+ b.Branch_Name Branch_Name,b.BranchID,b.cityid from [users] a,branch b where a.instid=b.instid and userid=" + UserID + " order by b.BranchID,Branch_Name,b.cityid";
                //    DataTable dtbranch = new DataTable();
                //    SqlDataAdapter adp_branch = odata.GetDataAdapter(sSQL);
                //    adp_branch.Fill(dtbranch);
                //    ddl_branch.DataSource = dtbranch;
                //    ddl_branch.DataTextField = dtbranch.Columns["Branch_Name"].ColumnName.ToString().Trim();
                //    ddl_branch.DataValueField = dtbranch.Columns["BranchID"].ColumnName.ToString().Trim();
                //    ddl_branch.DataBind();
                //}

                //if (Session["Selected_Branch"].ToString() == "")
                //{
                //    ddl_branch.Items.Insert(0, "Select");
                //}
                //else
                //{
                //    ddl_branch.Items.Insert(0, "Select");
                //    ddl_branch.Items.FindByValue(Session["Selected_Branch"].ToString()).Selected = true;
                //}

                if ((RoleID != "0") && (UserID != "0"))
                {
                    sSQL = "select *,case isactive when 1 then 'yes' when 0 then 'no' end active, IsAdmin from [users] where [userid]=" + UserID;
                    DataSet oDS = odata.GetDataSet(sSQL);
                    foreach (DataRow DR in oDS.Tables[0].Rows)
                    {
                        F_name.Text = (string)DR["FirstName"];
                        L_name.Text = (string)DR["LastName"];
                        username.Text = (string)DR["UserName"];
                        Session["uname"] = (string)DR["UserName"];

                        //string ccc = Convert.ToString(DR["InstID"]).Trim();
                        ddl_inst.SelectedValue = Convert.ToString(DR["InstID"]);

                        //ddl_branch.SelectedValue = Convert.ToString(DR["BranchID"]);
                        desig.Text = Convert.ToString(DR["Designation"]);
                        mobile.Text = Convert.ToString(DR["ContactNumber"]);
                        Email.Text = Convert.ToString(DR["EmailAddress"]);
                        ddl_status.SelectedValue = Convert.ToString(DR["isactive"]);
                        int sel_type = (Int32)DR["IsAdmin"];
                        if (sel_type == 2)
                        {
                            r1.Checked = true;
                        }
                        else if (sel_type == 3)
                        {
                            r2.Checked = true;
                        }
                        else if (sel_type == 4)
                        {
                            r3.Checked = true;
                        }
                        else if (sel_type == 1)
                        {
                            r4.Checked = true;
                        }
                        //active.Checked = (bool)DR["isactive"];
                    }

                    sSQL = "select * from [user_role] where [userid]=" + UserID;
                    DataSet DS = odata.GetDataSet(sSQL);
                    foreach (DataRow oDR in DS.Tables[0].Rows)
                    {
                        foreach (ListItem i in cl_role.Items)
                        {
                            if (i.Value == Convert.ToString(oDR["RoleID"]))
                            {
                                i.Selected = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }
        public string GetFullName()
        {
            try
            {
                string cmd;
                cDataAccess cdata = new cDataAccess();
                oCon.ConnectionString = cdata.InitializeConnection();
                oCon.Open();
                cmd = "select Firstname + ' ' + Lastname as FullName, inst.InstName as InstName from users join institution inst on inst.InstID = users.InstID";
                cmd += " Where Userid = @UID";

                SqlCommand sqc = new SqlCommand(cmd, oCon);
                sqc.Parameters.AddWithValue("@UID", Session["UserID"].ToString().Trim());
                DataSet sDS = new DataSet();
                SqlDataAdapter dadEorder = new SqlDataAdapter(sqc);
                dadEorder.Fill(sDS);
                foreach (DataRow DR in sDS.Tables[0].Rows)
                {
                    Fullname = Convert.ToString(DR["FullName"]);
                    InstName = Convert.ToString(DR["InstName"]);
                }

                oCon.Close();
                return Fullname + " - " + InstName;
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return "";
            }
        }
        protected void btn_Signup_Click(object sender, EventArgs e)
        {
            try
            {
                if (F_name.Text.Trim() == "" && L_name.Text.Trim() == "" && desig.Text.Trim() == "" && Email.Text.Trim() == ""
                    && mobile.Text.Trim() == "")
                {
                    //lbl_message.Text = "Please Fill Out Required Details";
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','Please Fill Out Required Details')", true);
                    return;
                }
                if (!r1.Checked && !r2.Checked && !r3.Checked || cl_role.SelectedIndex == -1)
                {
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','Please Fill Out Required Details')", true);
                    return;
                }
                if (username.Text.Length <= 30 && F_name.Text.Length <= 20 && L_name.Text.Length <= 10 &&
                    desig.Text.Length <= 30 && mobile.Text.Length <= 20 && Email.Text.Length <= 30)
                {
                    /////////////////CHECK KEYWORDS of TEXTBOXES//////////////////////
                    Regex regdetail = new Regex("^[0-9a-zA-Z._&]*$");
                    bool chk1 = regdetail.IsMatch(username.Text);
                    if (chk1 == false)
                    {
                        //lbl_message.Text = "Please provide correct details";
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info
                            + "','','Please provide correct details')", true);
                        return;
                    }
                    regdetail = new Regex("^[ A-Za-z'`]*$");
                    chk1 = regdetail.IsMatch(F_name.Text);
                    if (chk1 == false)
                    {
                        //lbl_message.Text = "Please provide correct details";
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info +
                            "','','Please provide correct details')", true);
                        return;
                    }

                    regdetail = new Regex("^[ A-Za-z'`]*$");
                    chk1 = regdetail.IsMatch(L_name.Text);
                    if (chk1 == false)
                    {
                        //lbl_message.Text = "Please provide correct details";
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info +
                            "','','Please provide correct details')", true);
                        return;
                    }

                    regdetail = new Regex("^[ A-Za-z'`]*$");
                    chk1 = regdetail.IsMatch(desig.Text);
                    if (chk1 == false)
                    {
                        //lbl_message.Text = "Please provide correct details";
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info +
                            "','','Please provide correct details')", true);
                        return;
                    }

                    regdetail = new Regex("^[0-9]*$");
                    chk1 = regdetail.IsMatch(mobile.Text);
                    if (chk1 == false)
                    {
                        //lbl_message.Text = "Please provide correct details";
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info +
                            "','','Please provide correct details')", true);
                        return;
                    }

                    regdetail = new Regex("^[A-Za-z0-9'`_()@.]*$");
                    chk1 = regdetail.IsMatch(Email.Text);
                    if (chk1 == false)
                    {
                        //lbl_message.Text = "Please provide correct details";
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info +
                            "','','Please provide correct details')", true);
                        return;
                    }
                    if (!r2.Checked)
                    {
                        if (ddl_inst.SelectedIndex == 0)
                        {
                            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info +
                                "','','Please select bank/institution')", true);
                            return;
                        }
                    }
                }
                else
                {
                    //lbl_message.Text = "Please provide correct details";
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info +
                        "','','Please provide correct details')", true);
                    return;
                }

                ////////////////////////////////////////////////////////
                ///
                int UserID = 0;
                try { UserID = Convert.ToInt32(Session["sUserID"]); }
                catch (Exception) { UserID = 0; }


                if (UserID == 0)
                {
                    if (CheckDuplicateUser() == false)
                    { }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info +
                            "','','UserName Already Exist')", true);
                        return;
                    }

                    if (CheckDuplicateNum() == false)
                    { }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info +
                            "','','Contact Number Already Exist')", true);
                        return;
                    }
                    string cmd = "";
                    string password = CreateRandomPassword();

                    ////ADMIN
                    if (r1.Checked)
                    {
                        cmd = "Insert into users(Username,Password,FirstName,LastName,Designation,EmailAddress,ContactNumber," +
                            "InstID,BranchID,IsActive,CreationDateTime,pass_change,IsAdmin)";
                        cmd += " values";
                        cmd += "(@UID, @pass, @fname, @lname, @desig, @email, @mob, @bknm,'0',1, '" + DateTime.Now + "',0, 2) select @@identity";
                        //cmd += "Values('" + username.Text.Trim() + "', '" + password + "', '" + F_name.Text.Trim() + "', '" + L_name.Text.Trim() + "', '" + desig.Text.Trim() + "', " +
                        //"'" + Email.Text.Trim() + "', '" + mobile.Text.Trim() + "', " + ddl_inst.SelectedValue + ", 1, '" + DateTime.Now + "',0, 0, 1)";
                    }
                    ///Merchant
                    else if (r2.Checked)
                    {
                        cmd = "Insert into users(Username,Password,FirstName,LastName,Designation,EmailAddress,ContactNumber," +
                            "InstID,BranchID,IsActive,CreationDateTime,pass_change,IsAdmin)";
                        cmd += " values";
                        cmd += "(@UID, @pass, @fname, @lname, @desig, @email, @mob, '888','0',1, '" + DateTime.Now + "',0, 3) select @@identity";
                    }
                    ////USER
                    else
                    {
                        cmd = "Insert into users(Username,Password,FirstName,LastName,Designation,EmailAddress,ContactNumber,InstID,BranchID,IsActive," +
                            "CreationDateTime,pass_change,IsAdmin)";
                        cmd += " Values";
                        cmd += "(@UID, @pass, @fname, @lname, @desig, @email, @mob, @bknm,'0', 1, '" + DateTime.Now + "',0, 4) select @@identity";
                        //cmd += "Values('" + username.Text.Trim() + "', '" + password + "', '" + F_name.Text.Trim() + "', '" + L_name.Text.Trim() + "', '" + desig.Text.Trim() + "', " +
                        //    "'" + Email.Text.Trim() + "', '" + mobile.Text.Trim() + "', " + ddl_inst.SelectedValue + ", 1, '" + DateTime.Now + "',0, 0, 0)";
                    }
                    cDataAccess cdata = new cDataAccess();
                    oCon.ConnectionString = cdata.InitializeConnection();
                    oCon.Open();
                    SqlCommand sqc = new SqlCommand(cmd, oCon);
                    sqc.Parameters.AddWithValue("@UID", username.Text.Trim());
                    sqc.Parameters.AddWithValue("@pass", password);
                    sqc.Parameters.AddWithValue("@fname", F_name.Text.Trim());
                    sqc.Parameters.AddWithValue("@lname", L_name.Text.Trim());
                    sqc.Parameters.AddWithValue("@desig", desig.Text.Trim());
                    sqc.Parameters.AddWithValue("@email", Email.Text.Trim());
                    sqc.Parameters.AddWithValue("@mob", mobile.Text.Trim());
                    sqc.Parameters.AddWithValue("@bknm", ddl_inst.SelectedValue);
                    UserID = Convert.ToInt32(sqc.ExecuteScalar());

                    oCon.Close();

                    foreach (ListItem i in cl_role.Items)
                    {
                        if (i.Selected == true)
                        {
                            string roleid = i.Value;
                            string sSQL = " INSERT INTO [User_Role]([UserID],[RoleID])";
                            sSQL += " values(" + UserID + "," + roleid + ")";
                            try
                            {
                                string sResult = Convert.ToString(cdata.RunProc(sSQL));
                            }
                            catch (Exception ex)
                            {
                                LogWriter.WriteToLog(ex);
                            }
                        }
                    }
                }
                else
                {
                    ///UPDATE QUERY
                    ///
                    int isop = 0;
                    if (r1.Checked)
                    {
                        isop = 2;
                    }
                    if (r2.Checked)
                    {
                        isop = 3;
                    }
                    if (r3.Checked)
                    {
                        isop = 4;
                    }

                    statchnge = 1;
                    string cmd = " Update users set Username=@UID," +
                        " FirstName=@fname,LastName=@lname,Designation=@desig,EmailAddress=@email,ContactNumber=@mob,InstID=@bknm" +
                        " ,IsActive=@isact,CreationDateTime='" + DateTime.Now + "', IsAdmin =" + isop +
                        " where userid=" + UserID;
                    cDataAccess cdata = new cDataAccess();
                    oCon.ConnectionString = cdata.InitializeConnection();
                    oCon.Open();
                    SqlCommand sqc = new SqlCommand(cmd, oCon);
                    sqc.Parameters.AddWithValue("@UID", username.Text.Trim());
                    sqc.Parameters.AddWithValue("@fname", F_name.Text.Trim());
                    sqc.Parameters.AddWithValue("@lname", L_name.Text.Trim());
                    sqc.Parameters.AddWithValue("@desig", desig.Text.Trim());
                    sqc.Parameters.AddWithValue("@email", Email.Text.Trim());
                    sqc.Parameters.AddWithValue("@mob", mobile.Text.Trim());
                    sqc.Parameters.AddWithValue("@bknm", ddl_inst.SelectedValue);
                    //sqc.Parameters.AddWithValue("@brnm", ddl_branch.SelectedItem.Text.Trim());
                    sqc.Parameters.AddWithValue("@isact", ddl_status.SelectedValue);
                    string sResult = Convert.ToString(cdata.RunProc(sqc));

                    cmd = "";
                    cmd = " delete from user_role where [userid]=" + UserID;

                    sResult = Convert.ToString(cdata.RunProc(cmd));

                    foreach (ListItem i in cl_role.Items)
                    {
                        if (i.Selected == true)
                        {
                            string roleid = i.Value;
                            string sSQL = " INSERT INTO [User_Role]([UserID],[RoleID])";
                            sSQL += " values(" + UserID + "," + roleid + ")";

                            try
                            {
                                sResult = Convert.ToString(cdata.RunProc(sSQL));
                            }
                            catch (Exception ex)
                            {
                                LogWriter.WriteToLog(ex);
                            }
                        }
                    }
                }

                ////////////EMAIL CODE FOR CMS USER/////////////////
                ////////////////////////////////////////////////////
                SmtpClient smtpClient = new SmtpClient("mail.nift.pk", 25);
                string UserEmail = Email.Text.Trim();

                smtpClient.Credentials = new System.Net.NetworkCredential("no-reply@niftepay.pk", "Abc123=+");
                //smtpClient.UseDefaultCredentials = true; // uncomment if you don't want to use the network credentials
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.EnableSsl = true;
                MailMessage mail = new MailMessage();

                //Setting From , To and CC
                mail.From = new MailAddress("no-reply@niftepay.pk", "NIFT - ePAY");
                mail.To.Add(new MailAddress(UserEmail));
                mail.Subject = "Auto Email Notification - Complaint Management";
                mail.IsBodyHtml = true;
                string mesage = "Dear " + F_name.Text.Trim().ToUpper() + " " + L_name.Text.Trim().ToUpper() + ",";
                mesage += "<br/><br/>Your account is successfully created on NIFT-ePay complaint management system";
                mesage += "<br/>You can login to CMS with the following credentials";
                mesage += "<br/>UserName : " + username.Text.Trim();
                mesage += "<br/>Password : " + Orig_Pass;
                mesage += "<br/><br/>Note: This is one-time generated password and expire after first login. ";
                mesage += "<br/><br/>Regards";
                mesage += "<br/>Dispute Resolution Unit (NIFT ePay)";

                mail.Body = mesage;
                try
                {
                    smtpClient.Send(mail);
                }
                catch
                {

                }
                //smtpClient.Dispose();

                /////////////////////////////////////////////////////////////
                /////////////////////NIFT OP USER EMAIL//////////////////////
                ////////////////////////////////////////////////////////////
                MailMessage newmail = new MailMessage();

                //Setting From , To and CC
                newmail.From = new MailAddress("no-reply@niftepay.pk", "NIFT - ePAY");
                newmail.To.Add(new MailAddress("support@niftepay.pk"));
                newmail.Subject = "Auto Email Notification - Complaint Management";
                newmail.IsBodyHtml = true;
                mesage = "Dear Operations Team,";
                mesage += "<br/><br/>A new user is registered in NIFT-ePay complaint management system with the following details:"
                    + "<br/>UserName: " + username.Text.Trim() +
                    "<br/>FullName: " + F_name.Text.Trim() + " " + L_name.Text.Trim();
                mesage += "<br/><br/>Regards";
                mesage += "<br/>Dispute Resolution Unit (NIFT ePay)";

                newmail.Body = mesage;
                try
                {
                    smtpClient.Send(newmail);
                }
                catch
                {

                }
                ViewState["ReferrerUrl"] = Request.UrlReferrer.ToString();
                Session["StateMsg"] = statchnge;
                Response.Redirect("ManageUser");

            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }

        }

        private static string CreateRandomPassword(int length = 20)
        {
            // Create a string of characters, numbers, special characters that allowed in the password  
            string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?_-";
            Random random = new Random();

            // Select one random character at a time from the string  
            // and create an array of chars  
            char[] chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                chars[i] = validChars[random.Next(0, validChars.Length)];
            }
            Orig_Pass = new string(chars);
            //////////////////////////////////////
            EncDec encdec = new EncDec();
            HASH = encdec.GetHash(Orig_Pass);
            //string enc_chars = encdec.Encrypt(Convert.ToString(Orig_Pass));
            return HASH;
        }
        protected void sign_out_Click(object sender, EventArgs e)
        {
            try
            {
                if (InsertUserLog() == true)
                {
                    Session["UserID"] = null;
                    Session["IiixA8"] = null;

                    Session.Clear();
                    Session.Abandon();
                    Session.RemoveAll();

                    if (Request.Cookies["ASP.NET_SessionId"] != null)
                    {
                        Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                        Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
                    }

                    if (Request.Cookies["AuthToken"] != null)
                    {
                        Response.Cookies["AuthToken"].Value = string.Empty;
                        Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
                    }
                    Response.Redirect("Loginpage");
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }

        public bool InsertUserLog()
        {
            try
            {
                cDataAccess cdata = new cDataAccess();
                int saved_UserLogID = 0;
                string cmd = "Select UserLogID from UserLog where UserID= " + Session["UserID"].ToString() + " and Dtn_Code= '" + Session["dtn"].ToString() + "'";
                foreach (DataRow DR in cdata.GetDataSet(cmd).Tables[0].Rows)
                {
                    saved_UserLogID = Convert.ToInt32(DR["UserLogID"]);
                }
                cmd = "Update UserLog set TimeOut = '" + DateTime.Now + "'";
                cmd += " Where UserLogID = " + saved_UserLogID;
                int i = Convert.ToInt32(cdata.RunProc(cmd));
                if (i == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return false;
            }
        }
        public bool CheckDuplicateUser()
        {
            try
            {
                cDataAccess cdata = new cDataAccess();
                string cmd = "select UserID from Users where UserName ='" + username.Text.Trim() + "'";
                string Userid = cdata.GetDataSet(cmd).Tables[0].Rows[0][0].ToString();
                if (Userid == "")
                { return false; }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool CheckDuplicateNum()
        {
            try
            {
                cDataAccess cdata = new cDataAccess();
                string cmd = "select UserID from Users where ContactNumber ='" + mobile.Text.Trim() + "'";
                string Userid = cdata.GetDataSet(cmd).Tables[0].Rows[0][0].ToString();
                if (Userid == "")
                { return false; }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        protected void Btn_back_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("Adminmain");
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }
    }
}