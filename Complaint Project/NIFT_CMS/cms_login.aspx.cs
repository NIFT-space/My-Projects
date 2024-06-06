using System;
using System.Web;
using System.Data;
using IBCS.Data;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Web.UI;

namespace NIFT_CMS
{
    public partial class cms_login : System.Web.UI.Page
    {
        public static string Orig_Pass;
        public bool isCalled = false;
        public static bool captcha_check;
        public string saved_HASH, Cont_Num;
        cDataAccess cdata = new cDataAccess();
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

                if (!IsPostBack)
                {
                    if (Session["UserID"] != null)
                    {
                        bool isAllowed = false;
                        string sUserId = Convert.ToString(Session["UserID"]).Trim();

                        isAllowed = general.isPageAllowed(sUserId, "Tickets");

                        if (isAllowed == false)
                        {
                            string cmd = "select IsAdmin,Pass_Change from Users where userid=@UID";
                            SqlConnection gc = cdata.GetConnection();
                            SqlCommand sqc = new SqlCommand(cmd, gc);
                            sqc.Parameters.AddWithValue("@UID", sUserId);
                            DataSet sDS = new DataSet();
                            SqlDataAdapter dadEorder = new SqlDataAdapter(sqc);
                            sDS = new DataSet();
                            dadEorder.Fill(sDS);

                            if (sDS.Tables[0].Rows.Count > 0)
                            {
                                int IsAdmin = Convert.ToInt32(sDS.Tables[0].Rows[0][0]);
                                if (IsAdmin == 1)
                                {
                                    cdata.CloseConnection(gc);
                                    Response.Redirect("Adminmain");
                                }
                                else
                                {
                                    Session["UserID"] = null;
                                    cdata.CloseConnection(gc);
                                    Response.Clear();
                                    Response.Redirect("Notallowed", true);
                                }
                            }
                        }
                        else
                        {
                            Response.Redirect("Tickets");
                        }
                    }
                    else
                    {
                        imgCaptcha.ImageUrl = "CreateCaptcha.aspx?New=1";
                        forgotuser.HRef = "ForgotUser";
                        forgotpass.HRef = "ForgotPassword";

                        if (!isCalled)
                        {
                            //SetCaptchaText();
                            isCalled = true; // set global variable to true
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }
        protected void Sign_IN_Click(object sender, EventArgs e)
        {
            try
            {
                /////////////////CHECK KEYWORDS of TEXTBOXES//////////////////////
                Regex regdetail = new Regex("^[0-9a-zA-Z]*$");
                bool chk1 = regdetail.IsMatch(txtCaptcha.Text);
                if (chk1 == false)
                {
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','INCORRECT RECAPTCHA')", true);
                    txtCaptcha.Text = "";
                    //alert_.Text = "INCORRECT RECAPTCHA!";
                    return;
                }

                if (txtCaptcha.Text == Session["CaptchaCode"].ToString())
                {

                    if (userId.Text == "" && pass_word.Text == "")
                    {
                        //alert_.Text = "PLEASE ENTER COMPLETE DETAILS";
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','PLEASE ENTER COMPLETE DETAILS')", true);
                        txtCaptcha.Text = "";
                        return;
                    }
                    ///////////CHECKING LENGTH AND KEYWORDS////////////
                    if (userId.Text.Length <= 20)
                    {
                        regdetail = new Regex("^[0-9a-zA-Z._&]*$");
                        bool chk2 = regdetail.IsMatch(userId.Text);
                        if (chk2 == false)
                        {
                            //alert_.Text = "INVALID CREDENTIALS!";
                            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','INVALID CREDENTIALS')", true);
                            return;
                        }
                    }
                    else
                    {
                        //alert_.Text = "INVALID CREDENTIALS!";
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','INVALID CREDENTIALS')", true);
                        txtCaptcha.Text = "";
                        return;
                    }

                    if (pass_word.Text.Length > 25)
                    {
                        //alert_.Text = "INVALID CREDENTIALS!";
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','INVALID CREDENTIALS')", true);
                        txtCaptcha.Text = "";
                        return;
                    }
                    /////////////////////////////////////////////////////

                    if (CheckUser() == true)
                    {
                        if (CheckPass() == true)
                        {

                            ////////////FORCE PASSWORD CHANGE////////////

                            if (Session["UserID"] != null)
                            {
                                string cmd = "select IsAdmin,IsOp,Pass_Change from Users where userid=@UID";
                                SqlConnection gc = cdata.GetConnection();
                                SqlCommand sqc = new SqlCommand(cmd, gc);
                                sqc.Parameters.AddWithValue("@UID", Session["UserID"]);
                                DataSet sDS = new DataSet();
                                SqlDataAdapter dadEorder = new SqlDataAdapter(sqc);
                                sDS = new DataSet();
                                dadEorder.Fill(sDS);

                                if (sDS.Tables[0].Rows.Count > 0)
                                {
                                    int IsAdmin = Convert.ToInt32(sDS.Tables[0].Rows[0][0]);
                                    int IsOp = Convert.ToInt32(sDS.Tables[0].Rows[0][1]);
                                                                        
                                    int pass_change = Convert.ToInt32(sDS.Tables[0].Rows[0][2]);
                                    if (pass_change == 1)
                                    {
                                        if (InsertUserLog() == true)
                                        {
                                            if (IsAdmin == 1)
                                            {
                                                string guid = Guid.NewGuid().ToString();
                                                Session["AuthToken"] = guid;
                                                // now create a new cookie with this guid value
                                                cdata.CloseConnection(gc);
                                                Response.Cookies.Add(new HttpCookie("AuthToken", guid));
                                                Session["IsAdmin"] = 1;
                                                Session["IiixA8"] = 1;
                                                Response.Redirect("Adminmain");
                                            }
                                            else if (IsAdmin == 2)
                                            {
                                                string guid = Guid.NewGuid().ToString();
                                                Session["AuthToken"] = guid;
                                                // now create a new cookie with this guid value
                                                cdata.CloseConnection(gc);
                                                Response.Cookies.Add(new HttpCookie("AuthToken", guid));
                                                Session["IsOp"] = 1;
                                                Session["00O3Px"] = 1;
                                                Response.Redirect("Tickets");
                                            }
                                            else
                                            {
                                                string guid = Guid.NewGuid().ToString();
                                                Session["AuthToken"] = guid;
                                                // now create a new cookie with this guid value
                                                cdata.CloseConnection(gc);
                                                Response.Cookies.Add(new HttpCookie("AuthToken", guid));
                                                Response.Redirect("Tickets");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (InsertUserLog() == true)
                                        {
                                            string guid = Guid.NewGuid().ToString();
                                            Session["AuthToken"] = guid;
                                            // now create a new cookie with this guid value  
                                            cdata.CloseConnection(gc);
                                            Response.Cookies.Add(new HttpCookie("AuthToken", guid));
                                            Session["Old_Pass"] = pass_word.Text.Trim();
                                            Response.Redirect("ChangePassword");
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            //alert_.Text = "INVALID CREDENTIALS!";
                            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','INVALID CREDENTIALS')", true);
                        }
                    }
                    else
                    {
                        //alert_.Text = "INVALID CREDENTIALS!";
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','INVALID CREDENTIALS')", true);
                    }
                    //}
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','INCORRECT RECAPTCHA!')", true);
                    //alert_.Text = "INCORRECT RECAPTCHA";
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }

        public bool CheckUser()
        {
            try
            {
                SqlConnection gc = cdata.GetConnection();
                string cmd = "Select UserName, userid,EmailAddress,FirstName + ' ' + LastName as Fullname from users where UserName =@Uname";
                SqlCommand sqc = new SqlCommand(cmd, gc);
                sqc.Parameters.AddWithValue("@Uname", userId.Text.Trim());
                DataSet sDS = new DataSet();
                SqlDataAdapter dadEorder = new SqlDataAdapter(sqc);
                sDS = new DataSet();
                dadEorder.Fill(sDS);
                foreach (DataRow DR in sDS.Tables[0].Rows)
                {
                    Cont_Num = DR["UserName"].ToString();
                    Session["UserID"] = DR["userid"].ToString();
                    Session["EmailAddress"] = DR["EmailAddress"].ToString();
                    Session["Fullname"] = DR["Fullname"].ToString();
                }
                cdata.CloseConnection(gc);
                if (Cont_Num != "" && Cont_Num != null)
                {
                    return true;
                }
                else
                {
                    txtCaptcha.Text = "";
                    Session["UserID"] = null;
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return false;
            }
        }

        public bool CheckPass()
        {
            try
            {
                SqlConnection gc = cdata.GetConnection();
                string cmd = "Select Password from users where UserName =@Uname";
                DataSet sDS = new DataSet();
                SqlCommand sqc = new SqlCommand(cmd, gc);
                sqc.Parameters.AddWithValue("@Uname", userId.Text.Trim());
                SqlDataAdapter dadEorder = new SqlDataAdapter(sqc);
                dadEorder.Fill(sDS);
                foreach (DataRow DR in sDS.Tables[0].Rows)
                {
                    saved_HASH = DR["Password"].ToString();
                }
                cdata.CloseConnection(gc);
                EncDec encdec = new EncDec();
                string HASH = encdec.GetHash(pass_word.Text.Trim());
                //string dec_chars = encdec.decrypt(saved_Pass);

                if (saved_HASH == HASH)
                {
                    return true;
                }
                else
                {
                    txtCaptcha.Text = "";
                    Session["UserID"] = null;
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return false;
            }
        }
        public bool InsertUserLog()
        {
            try
            {
                SqlConnection gc = cdata.GetConnection();
                string saved_UserName = "";
                string cmd = "Select UserName,us.InstID as InstID,inst.InstName as Inst_nm,BranchID from users us join Institution inst on inst.InstID = us.InstID where us.UserID=@UID";
                SqlCommand sqc = new SqlCommand(cmd, gc);
                sqc.Parameters.AddWithValue("@UID", Session["UserID"].ToString().Trim());
                DataSet sDS = new DataSet();
                SqlDataAdapter dadEorder = new SqlDataAdapter(sqc);
                dadEorder.Fill(sDS);
                foreach (DataRow DR in sDS.Tables[0].Rows)
                {
                    saved_UserName = DR["UserName"].ToString();
                    Session["InstID"] = DR["InstID"].ToString();
                    Session["Inst_nm"] = DR["Inst_nm"].ToString();
                    Session["BranchID"] = DR["BranchID"].ToString();
                }
                string dtn = Session["BranchID"] + DateTime.Now.ToString("ddMMyyyyHHmm");

                cmd = "Insert into UserLog(UserID, TimeIn, UserName, Dtn_Code)";
                cmd += " Values(@UID , @dt , @Uname , @dtn)";

                sqc = new SqlCommand(cmd, gc);
                sqc.Parameters.AddWithValue("@UID", Session["UserID"]);
                sqc.Parameters.AddWithValue("@dt", DateTime.Now);
                sqc.Parameters.AddWithValue("@Uname", saved_UserName);
                sqc.Parameters.AddWithValue("@dtn", dtn);

                sqc.ExecuteNonQuery();
                cdata.CloseConnection(gc);
                //int i = Convert.ToInt32(cdata.RunProc(cmd));
                //if(i == 1)
                //{
                Session["dtn"] = dtn;
                //Session["UserID"] = userId.Text.Trim();
                return true;
                //}
                //else
                //{
                //return false;
                //}
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return false;
            }
        }
    }
}