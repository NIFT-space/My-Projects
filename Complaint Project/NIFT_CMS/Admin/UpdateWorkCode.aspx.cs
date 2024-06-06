using IBCS.Data;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NIFT_CMS
{
    public partial class UpdateWorkCode : System.Web.UI.Page
    {
        public SqlConnection oCon = new SqlConnection();
        public static string Fullname, InstName, Uid;
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

                        isAllowed = general.isPageAllowed(sUserId, "ManageWorkCode");

                        if (isAllowed == false)
                        {
                            Response.Clear();
                            Response.Redirect("Notallowed", true);
                        }
                        if (!IsPostBack)
                        {
                            /////////////DISPLAY USERNAME//////////////
                            lbl_user.Text = GetFullName();
                            BindSQL();
                            ddl_tat.SelectedValue = "0";
                        }

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
                return true;
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
                sSQL = " select workcode,Description,convert(varchar, CreationDateTime,0)as CreationDateTime,convert(varchar,TAT) + ' ' + TAT_type as TAT from workcode ";

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
                if (wc_name.Text.Trim() != "" && _TAT.Text.Trim() != "")
                {
                    string sMsg = "";

                    Regex rex = new Regex("^[ 0-9A-Za-z._]+$", RegexOptions.IgnoreCase);
                    Boolean isParsName4 = rex.IsMatch(wc_name.Text);
                    if (!isParsName4)
                    {
                        sMsg = "Please enter valid work code";
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','" + sMsg + "')", true);
                        return;
                    }

                    int tat = Convert.ToInt32(_TAT.Text);
                    Regex rex2 = new Regex("^[0-9]+$", RegexOptions.IgnoreCase);
                    Boolean isParsName40 = rex2.IsMatch(_TAT.Text);
                    if (ddl_tat.SelectedItem.Value == "0")
                    {
                        if (!isParsName40 || tat > 99)
                        {
                            sMsg = "Please enter turn-around time within 50 days";
                            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','" + sMsg + "')", true);
                            return;
                        }
                    }
                    else
                    {
                        if (!isParsName40 || tat > 800)
                        {
                            sMsg = "Please enter turn-around time within 50 days";
                            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','" + sMsg + "')", true);
                            return;
                        }
                    }

                    bool isavail = true;
                    if (isavail == true)
                    {
                        bool isInserted = false;
                        isInserted = InsertRecord();
                        if (isInserted == true)
                        {
                            sMsg = "Workcode Added Successfully";
                            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','" + sMsg + "')", true);
                            BindSQL();
                            wc_name.Text = "";
                            _TAT.Text = "";
                        }
                        else
                        {
                            sMsg = "Workcode already exists!";
                            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','" + sMsg + "')", true);
                            return;
                        }
                    }
                    else
                    {
                        sMsg = "Duplicate Workcode Not Saved";
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','" + sMsg + "')", true);
                        return;
                    }
                }
                else
                {
                    string sMsg = "Please enter required fields";
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','" + sMsg + "')", true);
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
        private bool InsertRecord()
        {
            cDataAccess oData = new cDataAccess();
            oCon.ConnectionString = oData.InitializeConnection();
            oCon.Open();
            string sSQL = "";
            try
            {
                if (Convert.ToString(Session["userid"]).Trim() != "")
                {
                    sSQL = " Insert into workcode(Description,CreationDateTime,TAT, TAT_type) ";
                    sSQL += " values ";
                    sSQL += " (@desc ,'" + DateTime.Now + "','" + _TAT.Text.Trim() + "', '" + ddl_tat.SelectedItem.Text + "') ";
                    SqlCommand sqc = new SqlCommand(sSQL, oCon);
                    sqc.Parameters.AddWithValue("@desc", wc_name.Text);
                    sqc.ExecuteNonQuery();
                    oCon.Close();
                }
                return true;
            }
            catch (SqlException exception)
            {
                if (exception.Number == 2601) // Cannot insert duplicate key row in object error
                {
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
        protected void dg_bank_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                btn_Submit.Visible = false;
                btn_Update.Visible = true;

                GridViewRow row = dg_bank.SelectedRow;
                string workid = dg_bank.DataKeys[row.RowIndex].Values[0].ToString();
                DataTable dt = new DataTable();
                dt = (DataTable)Session["AdminBankTable"];
                dt.DefaultView.RowFilter = String.Format("Workcode = '{0}'", workid);
                int rowIndex = dt.Rows.IndexOf(dt.Select("Workcode = " + workid)[0]);
                wc_name.Text = Convert.ToString(dt.Rows[rowIndex]["Description"]);
                string t_ = _TAT.Text = Convert.ToString(dt.Rows[rowIndex]["TAT"]);

                int index = _TAT.Text.IndexOf(' ');

                if (index > 0)
                {
                    _TAT.Text = _TAT.Text.Substring(0, index);

                    string n_ = t_.Substring(index);
                    if (n_ == " Days")
                    {
                        ddl_tat.SelectedIndex = 0;
                    }
                    else if (n_ == " Hours")
                    {
                        ddl_tat.SelectedIndex = 1;
                    }
                }

                if (workid != string.Empty)
                {
                    Uid = workid;
                }

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
                if (wc_name.Text.Trim() != "" && Uid.Trim() != "" && _TAT.Text.Trim() != "")
                {
                    Regex rex = new Regex("^[ 0-9A-Za-z._]+$", RegexOptions.IgnoreCase);
                    Boolean isParsName4 = rex.IsMatch(wc_name.Text);

                    int tat = Convert.ToInt32(_TAT.Text);
                    Regex rex2 = new Regex("^[0-9]+$", RegexOptions.IgnoreCase);
                    Boolean isParsName40 = rex2.IsMatch(_TAT.Text);
                    if (ddl_tat.SelectedItem.Value == "0")
                    {
                        if (!isParsName40 || tat > 99)
                        {
                            string sMsg = "Please enter turn-around time within 50 days";
                            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','" + sMsg + "')", true);
                            return;
                        }
                    }
                    else
                    {
                        if (!isParsName40 || tat > 800)
                        {
                            string sMsg = "Please enter turn-around time within 50 days";
                            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','" + sMsg + "')", true);
                            return;
                        }
                    }

                    if (!isParsName4)
                    {
                        string sMsg = "Please enter valid work code";
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','" + sMsg + "')", true);
                        return;
                    }

                    bool isUpdated = true;
                    isUpdated = Updatewc(Uid);
                    if (isUpdated == true)
                    {
                        string sMsg = "WorkCode Updated Successfully";
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','" + sMsg + "')", true);
                        BindSQL();
                        wc_name.Text = "";
                        _TAT.Text = "";
                    }
                    else
                    {
                        string sMsg = "Please enter valid Workcode";
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','" + sMsg + "')", true);
                    }
                }
                else
                {
                    string sMsg = "Please enter required fields";
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
            }
        }
        private bool Updatewc(string userid)
        {
            try
            {
                cDataAccess oData = new cDataAccess();
                object obj;
                bool retval = false;
                string sSQL = "";
                if (Convert.ToString(Session["userid"]).Trim() != "")
                {
                    sSQL = " update workcode set ";
                    sSQL += " Description = '" + wc_name.Text.Trim() + "'";
                    sSQL += " ,CreationDateTime = getdate()";
                    sSQL += " ,TAT = '" + _TAT.Text.Trim();
                    sSQL += "' where workcode = '" + userid.Trim() + "'";
                }
                obj = oData.RunProc(sSQL);

                if (Convert.ToString(obj) == "1")
                {
                    retval = true;
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
            try
            {
                dg_bank.PageIndex = e.NewPageIndex;
                BindSQL();
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }
        protected void dg_bank_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                string confirmValue = Request.Form["confirm_value"];

                if (confirmValue == "YES")
                {
                    Object obj;
                    GridViewRow del = (GridViewRow)dg_bank.Rows[e.RowIndex];
                    string workid = dg_bank.Rows[e.RowIndex].Cells[0].Text;
                    String sSQL;
                    cDataAccess odata = new cDataAccess();

                    sSQL = "delete from workcode where workcode=" + workid;
                    obj = odata.RunProc(sSQL);

                    BindSQL();
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','Record deleted successfully')", true);
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','Unable to delete record!')", true);
                LogWriter.WriteToLog(ex);
                return;
            }
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
    }
}