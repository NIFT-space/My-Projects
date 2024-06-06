using IBCS.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NIFT_CMS
{
    public partial class ManageMerchant : System.Web.UI.Page
    {
        public SqlConnection oCon = new SqlConnection();
        public static string Fullname, InstName,Uid;
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
                if (Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
                {
                    if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                    {
                        Response.Clear();
                        Response.Redirect("Sessionexpire", true);
                    }
                    else
                    {
                        bool isAllowed = false;
                        string sUserId = Convert.ToString(Session["UserID"]).Trim();

                        isAllowed = general.isPageAllowed(sUserId, "ManageMerchant");

                        if (isAllowed == false)
                        {
                            Response.Clear();
                            Response.Redirect("Notallowed", true);
                        }
                        if (Page.IsPostBack != true)
                        {
                            Get_merchantlist();
                            BindSQL();
                            ScriptManager.RegisterStartupScript(Page, this.GetType(), "ScrollPage", "window.parent.scroll(0,0);", true);
                        }
                        /////////////DISPLAY USERNAME//////////////
                        lbl_user.Text = GetFullName();
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }
        protected void sign_out_Click(object sender, EventArgs e)
        {
            try
            {
                if (InsertUserLog() == true)
                {
                    Session["UserID"] = null;
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
                oCon.ConnectionString = cdata.InitializeConnection();
                oCon.Open();
                int saved_UserLogID = 0;
                string cmd = "Select UserLogID from UserLog where UserID=@UID and Dtn_Code=@dtn";
                SqlCommand sqc = new SqlCommand(cmd, oCon);
                sqc.Parameters.AddWithValue("@UID", Session["UserID"].ToString());
                sqc.Parameters.AddWithValue("@dtn", Session["dtn"].ToString());
                DataSet sDS = new DataSet();
                SqlDataAdapter dadEorder = new SqlDataAdapter(sqc);
                dadEorder.Fill(sDS);
                foreach (DataRow DR in sDS.Tables[0].Rows)
                {
                    saved_UserLogID = Convert.ToInt32(DR["UserLogID"]);
                }
                cmd = "Update UserLog set TimeOut = @T_out";
                cmd += " Where UserLogID = @ULID";
                sqc.Parameters.AddWithValue("@T_out", DateTime.Now);
                sqc.Parameters.AddWithValue("@ULID", saved_UserLogID);
                sqc.ExecuteNonQuery();
                oCon.Close();
                //int i = Convert.ToInt32(cdata.RunProc(cmd));
                //if (i == 1)
                //{
                return true;
                //}
                //else
                //{
                //    return false;
                //}
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return false;
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
        protected void BindSQL()
        {
            try
            {
                String sSQL;
                DataTable dt = new DataTable();
                cDataAccess odata = new cDataAccess();
                sSQL = " select MerchID, MccID,convert(varchar,UserID)+ ' - ' + username as uid, UserID,Username,FirstName,LastName,case when isactive=1 then 'Active' else 'In-active' end as isactive from " +
                         "Merchant a " +
                         "order by CreationDateTime desc, MerchID desc";

                SqlDataAdapter adp = odata.GetDataAdapter(sSQL);
                adp.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    dg_bank.DataSource = dt;
                    dg_bank.DataBind();
                }
                else
                {
                    dt = new DataTable();
                    dg_bank.DataSource = dt;
                    dg_bank.DataBind();
                }
                Session["AdminBankTable"] = dt;
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }

        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                string sMsg = "";
                if (f_name.Text == "")
                {
                    sMsg = "Please enter required details";
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','" + sMsg + "')", true);
                    return;
                }
                Regex rex2 = new Regex("^[0-9A-Za-z ]+$", RegexOptions.IgnoreCase);
                Boolean isParsName2 = rex2.IsMatch(f_name.Text);
                if (!isParsName2)
                {
                    sMsg = "Invalid First Name";
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','" + sMsg + "')", true);
                    return;
                }

                Boolean isParsName3 = rex2.IsMatch(l_name.Text);
                if (!isParsName3)
                {
                    sMsg = "Invalid Last Name";
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','" + sMsg + "')", true);
                    return;
                }

                //Regex rex3 = new Regex("^[0-9A-Za-z._]+$", RegexOptions.IgnoreCase);
                //Boolean isParsName4 = rex3.IsMatch(txtuserid.Text);
                //if (!isParsName4)
                //{
                //    sMsg = "Record Saved Successfully";
                //    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','" + sMsg + "')", true);
                //    return;
                //}

                bool isavail = true;
                if (isavail == true)
                {
                    bool isInserted = false;
                    bool isexist = false;
                    isexist = CheckUserID(ddl_users.SelectedItem.Value);
                    if(isexist == true)
                    {
                        sMsg = "Merchant already exists!";
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','" + sMsg + "')", true);
                        return;
                    }
                    isInserted = InsertMerch();
                    if (isInserted == true)
                    {
                        sMsg = "Record Saved Successfully";
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','" + sMsg + "')", true);
                        BindSQL();
                        f_name.Text = "";
                        l_name.Text = "";
                        //txtuserid.Text = "";
                        ddl_users.SelectedIndex = 0;
                    }
                    else
                    {
                        sMsg = "Merchant already exists!";
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','" + sMsg + "')", true);
                        return;
                    }
                }
                else
                {
                    sMsg = "Duplicate Record Not Saved";
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','" + sMsg + "')", true);
                    return;
                }
            }
            catch (SqlException exception)
            {
                if (exception.Number == 2601) // Cannot insert duplicate key row in object error
                {
                    string sMsg = "Duplicate Record Not Saved";
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','" + sMsg + "')", true);
                }
                else
                {
                    string sMsg = "Record Not Saved";
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','" + sMsg + "')", true);
                    LogWriter.WriteToLog(exception);
                }
            }
            catch (Exception ex)
            {
                string sMsg = "Record Not Saved";
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','" + sMsg + "')", true);
                LogWriter.WriteToLog(ex);
            }
        }
        private bool CheckUserID(string userid)
        {
            try
            {
                string sSQL = " Select UserID from Merchant where UserID = "+ userid;
                cDataAccess cd = new cDataAccess();
                DataTable dt = new DataTable();
                dt = cd.GetDataSet(sSQL).Tables[0];
                if (dt.Rows.Count > 0)
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
        private bool InsertMerch()
        {
            cDataAccess oData = new cDataAccess();
            oCon.ConnectionString = oData.InitializeConnection();
            oCon.Open();
            //object obj;
            string sSQL = "";
            try
            {
                if (Convert.ToString(Session["userid"]).Trim() != "")
                {
                    sSQL = "insert into Merchant (MccID, FirstName,LastName,UserID, Username,CreationDateTime,IsActive)";
                    sSQL += " values";
                    sSQL += "(@mccid, @fnm, @lnm , @bkcd, @bknm,'" + DateTime.Now+"',1)";
                    //sSQL += "('" + f_name.Text.Trim() + "','" + l_name.Text.Trim() + "','" + BCode.Trim() + "','" + txtuserid.Text.Trim() + "')";
                    SqlCommand sqc = new SqlCommand(sSQL, oCon);
                    sqc.Parameters.AddWithValue("@MccID", MccID_.Text);
                    sqc.Parameters.AddWithValue("@fnm", f_name.Text);
                    sqc.Parameters.AddWithValue("@lnm", l_name.Text);
                    //sqc.Parameters.AddWithValue("@bkcd", txtuserid.Text);
                    sqc.Parameters.AddWithValue("@bkcd", ddl_users.SelectedItem.Value);
                    sqc.Parameters.AddWithValue("@bknm", ddl_users.SelectedItem.Text);
                    //sqc.Parameters.AddWithValue("@txtusid", txtuserid.Text);
                    sqc.ExecuteNonQuery();
                    oCon.Close();
                }
                //obj = oData.RunProc(sSQL);
                return true;
            }
            catch (SqlException exception)
            {
                if (exception.Number == 2601) // Cannot insert duplicate key row in object error
                {
                    //sSQL = "update Merchant set instname='" + BName.Trim() + "' where instid='" + "'" + BCode.Trim() + "'";
                    //obj = oData.RunProc(sSQL);
                    return true;
                }
                else
                {
                    LogWriter.WriteToLog(exception);
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return false;
            }

        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                f_name.Text = "";
                l_name.Text = "";
                //txtuserid.Text = "";
                ddl_users.SelectedItem.Value = "0";
                //bkcode.Text = "";
                btn_Submit.Visible = true;
                btn_Update.Visible = false;
                //bkcode.Enabled = true;
                //txtuserid.Enabled = true;
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }
        protected void dg_bank_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                btn_Submit.Visible = false;
                btn_Update.Visible = true;
                

                GridViewRow row = dg_bank.SelectedRow;
                string merchid = dg_bank.DataKeys[row.RowIndex].Values[0].ToString();
                string mccid = dg_bank.DataKeys[row.RowIndex].Values[1].ToString();
                string userid = dg_bank.DataKeys[row.RowIndex].Values[2].ToString();
                string username = dg_bank.DataKeys[row.RowIndex].Values[3].ToString();
                string fname = dg_bank.DataKeys[row.RowIndex].Values[4].ToString();
                string lname = dg_bank.DataKeys[row.RowIndex].Values[5].ToString();
                string isactive = dg_bank.DataKeys[row.RowIndex].Values[6].ToString();
                DataTable dt = new DataTable();
                dt = (DataTable)Session["AdminBankTable"];
                dt.DefaultView.RowFilter = String.Format("UserID = '{0}'", userid);


                if (userid != string.Empty)
                {
                    //Uid = txtuserid.Text = userid;
                    Uid = userid;
                    MccID_.Text = mccid;
                    f_name.Text = fname;
                    l_name.Text = lname;
                    ddl_users.SelectedValue = userid;
                    //ddl_users.Items.FindByValue(userid).Selected = true;
                    if (isactive == "Active")
                    {
                        chkbox_active.Checked = true;
                    }
                    else
                    {
                        chkbox_active.Checked = false;
                    }
                }
                ddl_users.Enabled = false;
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }
        protected void btn_Update_Click(object sender, EventArgs e)
        {
            try
            {
                if (MccID_.Text !="" && f_name.Text.Trim() != "" && Uid.Trim() != "")
                {
                    Regex rex2 = new Regex("^[0-9A-Za-z ]+$", RegexOptions.IgnoreCase);

                    Boolean isParsName1 = rex2.IsMatch(MccID_.Text);
                    if (!isParsName1)
                    {
                        string sMsg = "Invalid Merchant ID";
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','" + sMsg + "')", true);
                        return;
                    }

                    Boolean isParsName2 = rex2.IsMatch(f_name.Text);
                    if (!isParsName2)
                    {
                        string sMsg = "Invalid First Name";
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','" + sMsg + "')", true);
                        return;
                    }

                    Boolean isParsName3 = rex2.IsMatch(l_name.Text);
                    if (!isParsName3)
                    {
                        string sMsg = "Invalid Last Name";
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','" + sMsg + "')", true);

                        return;
                    }

                    //Regex rex3 = new Regex("^[0-9A-Za-z._]+$", RegexOptions.IgnoreCase);
                    //Boolean isParsName4 = rex3.IsMatch(txtuserid.Text);
                    //if (!isParsName4)
                    //{
                    //    string sMsg = "Record Saved Successfully";
                    //    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','" + sMsg + "')", true);

                    //    return;
                    //}

                    bool isUpdated = true;
                    isUpdated = UpdateBank(Uid);
                    if (isUpdated == true)
                    {
                        string sMsg = "Record Saved Successfully";
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','" + sMsg + "')", true);
                        BindSQL();
                        MccID_.Text = "";
                        f_name.Text = "";
                        l_name.Text = "";
                        ddl_users.SelectedIndex = 0;
                        //txtuserid.Text = "";
                        //bkcode.Text = "";
                    }
                    else
                    {
                        string sMsg = "Please enter required details";
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','" + sMsg + "')", true);
                    }
                }
                else
                {
                    string sMsg = "Please enter required details";
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','" + sMsg + "')", true);
                }

            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
            finally
            {
                btn_Submit.Visible = true;
                btn_Update.Visible = false;
                //bkcode.Enabled = true;
                //txtuserid.Enabled = true;
                ddl_users.Enabled = true;
                
                chkbox_active.Checked = false;
            }
        }
        private bool UpdateBank(string userid)
        {
            try
            {
                int active = 0;
                if(chkbox_active.Checked)
                {
                    active = 1;
                }
                else
                {
                    active = 0;
                }
                cDataAccess oData = new cDataAccess();
                object obj;
                bool retval = false;
                string sSQL = "";
                if (Convert.ToString(Session["userid"]).Trim() != "")
                {
                    sSQL = " update Merchant set ";
                    sSQL += " MccID = '" + MccID_.Text.Trim() + "',";
                    sSQL += " FirstName = '" + f_name.Text.Trim() + "', LastName = '" + l_name.Text.Trim() + "'";
                    sSQL += " ,CreationDateTime = getdate(), IsActive = "+ active;
                    sSQL += " where userid = '" + userid.Trim() + "'";
                }
                try
                {
                    oData.RunProc(sSQL);
                    retval = true;
                }
                catch
                {
                    retval = false;
                }
                return retval;
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return false;
            }
        }

        protected void dg_bank_PageIndexChanged(object sender, GridViewPageEventArgs e)
        {
            dg_bank.PageIndex = e.NewPageIndex;
            BindSQL();
        }
        protected void Btn_back_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["IsOp"] != null)
                {
                    Response.Redirect("OpDashboard");
                }
                else
                {
                    Response.Redirect("Adminmain");
                }
            }
            catch (Exception ex)
            {
                //LogWriter.WriteToLog(ex);
            }
        }
        protected void Get_merchantlist()
        {
            try
            {
                //code for active merchants dropdownlist box
                string sSQL = " select userid ,UserName from Users where IsActive=1 and IsAdmin=3 order by userid desc ";
                DataTable dtuser = new DataTable();
                cDataAccess odata = new cDataAccess();
                SqlDataAdapter adp_inst = odata.GetDataAdapter(sSQL);
                adp_inst.Fill(dtuser);

                ddl_users.DataSource = dtuser;
                ddl_users.DataTextField = dtuser.Columns["UserName"].ColumnName.ToString().Trim();
                ddl_users.DataValueField = dtuser.Columns["userid"].ColumnName.ToString().Trim();
                ddl_users.DataBind();
                ddl_users.Items.Insert(0, "Select");
            }
            catch
            {

            }
        }
    }
}