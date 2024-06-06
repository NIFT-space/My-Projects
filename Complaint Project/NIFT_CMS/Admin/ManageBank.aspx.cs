using IBCS.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NIFT_CMS
{
    public partial class ManageBank : System.Web.UI.Page
    {
        public SqlConnection oCon = new SqlConnection();
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

                        isAllowed = general.isPageAllowed(sUserId, "ManageBank");

                        if (isAllowed == false)
                        {
                            Response.Clear();
                            Response.Redirect("Notallowed", true);
                        }

                        if (Page.IsPostBack != true)
                        {
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
                sSQL = " select TRIM(STUFF( '000', 3 - LEN( a.[InstID] ) + 1, LEN( a.[InstID] ), ";
                sSQL += " a.[InstID])) InstID,instid bankid,instname from ";
                sSQL += " institution a where instid>0 and instid<999 ";
                sSQL += " order by CreationDateTime desc, instid,instname ";

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
                txtBankCode.Text = "";
                txtBankName.Text = "";
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
                String sBCode = txtBankCode.Text.Trim();
                int iBCode;
                String sBName = general.SQLEncode(txtBankName.Text.Trim());

                if ((sBCode.Trim() != "") && (sBName.Trim() != ""))
                {
                    // Bank Code Validation

                    if (sBCode.Length > 3)
                    {
                        return;
                    }
                    bool isParsCode = Int32.TryParse(sBCode, out iBCode);
                    if (!isParsCode)
                    {
                        return;
                    }
                    Regex rex = new Regex("^[a-zA-Z0-9]+$", RegexOptions.IgnoreCase);
                    Regex rex2 = new Regex(@"^[0-9a-zA-Z_.\s ]+$", RegexOptions.IgnoreCase);
                    Boolean isParsName = rex.IsMatch(sBCode);
                    if (!isParsName)
                    {
                        return;
                    }

                    // Bank Name Validation
                    if (sBName.Length > 30)
                    {
                        return;
                    }
                    Boolean isParsName2 = rex2.IsMatch(sBName);
                    if (!isParsName2)
                    {
                        return;
                    }
                    //
                    bool isInserted = false;
                    isInserted = InsertBank(sBCode, sBName);
                    if (isInserted == true)
                    {
                        string sMsg = "Record Saved Successfully";
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','" + sMsg + "')", true);
                        BindSQL();
                        txtBankCode.Text = "";
                        txtBankName.Text = "";
                    }
                    else
                    {
                        string sMsg = "Bank already exists!";
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','" + sMsg + "')", true);
                        return;
                    }
                }
                else
                {
                    string sMsg = "Please Enter Required Details";
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','" + sMsg + "')", true);
                }
            }
            catch (SqlException exception)
            {
                if (exception.Number == 2601) // Cannot insert duplicate key row in object error
                {
                    string sMsg = "Duplicate Record Not Saved";
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','" + sMsg + "')", true);
                    return;
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
        private bool InsertBank(string BCode, string BName)
        {
            cDataAccess oData = new cDataAccess();
            oCon.ConnectionString = oData.InitializeConnection();
            oCon.Open();
            //object obj;
            string sSQL = "";
            try
            {
                if (Convert.ToString(Session["InstID"]).Trim() != "")
                {
                    sSQL = "insert into institution (instid,instname, CreationDateTime,IsBank)";
                    sSQL += " values";
                    sSQL += " (@iid, @inm, GetDate(), 1)";
                    //sSQL += "('" + BCode.Trim() + "','" + BName.Trim() + "')";
                    
                    SqlCommand sqc = new SqlCommand(sSQL, oCon);
                    sqc.Parameters.AddWithValue("@iid", BCode);
                    sqc.Parameters.AddWithValue("@inm", BName);
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
                    sSQL = "update institution set instname=@inm, CreationDateTime = '"+DateTime.Now+"' where instid="+BCode;

                    SqlCommand sqc = new SqlCommand(sSQL, oCon);
                    sqc.Parameters.AddWithValue("@inm", BName);
                    sqc.ExecuteNonQuery();
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
                txtBankCode.Text = "";
                txtBankName.Text = "";
                btn_Submit.Visible = true;
                btn_Update.Visible = false;
                txtBankCode.Enabled = true;
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
                txtBankCode.Enabled = false;

                GridViewRow row = dg_bank.SelectedRow;
                string sBankCode = dg_bank.DataKeys[row.RowIndex].Values[0].ToString();
                string sBankName = dg_bank.DataKeys[row.RowIndex].Values[1].ToString();
                DataTable dt = new DataTable();
                dt = (DataTable)Session["AdminBankTable"];
                dt.DefaultView.RowFilter = String.Format("InstID = '{0}'", sBankCode);


                if (sBankCode != string.Empty)
                {
                    txtBankCode.Text = sBankCode;
                }
                if (sBankName != string.Empty)
                {
                    txtBankName.Text = sBankName;
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
                string sBankCode = txtBankCode.Text;
                string sBankName = txtBankName.Text;

                if ((sBankCode.Trim() != "") && (sBankName.Trim() != ""))
                {
                    Regex rex = new Regex("^[0-9]+$", RegexOptions.IgnoreCase);
                    Boolean isParsName = rex.IsMatch(sBankCode);
                    if (!isParsName)
                    {
                        string sMsg = "Invalid Bank code";
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','" + sMsg + "')", true);
                        return;
                    }
                    Regex rex2 = new Regex("^[a-zA-Z0-9 ]+$", RegexOptions.IgnoreCase);
                    Boolean isParsName2 = rex2.IsMatch(sBankName);
                    if (!isParsName2)
                    {
                        string sMsg = "Invalid Bank Name";
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','" + sMsg + "')", true);
                        return;
                    }

                    bool isUpdated = false;
                    isUpdated = UpdateBank(sBankCode, sBankName);
                    if (isUpdated == true)
                    {
                        string sMsg = "Record Saved Successfully";
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','" + sMsg + "')", true);
                        BindSQL();
                    }
                    else
                    {
                        string sMsg = "Record Not Saved";
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','" + sMsg + "')", true);
                    }
                }
                else
                {
                    string sMsg = "Kindly Fill Required Fields";
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
                txtBankCode.Enabled = true;
            }
        }
        private bool UpdateBank(string BCode, string BName)
        {
            try
            {
                cDataAccess oData = new cDataAccess();
                oCon.ConnectionString = oData.InitializeConnection();
                oCon.Open();
                //object obj;
                bool retval = false;
                string sSQL = "";
                if (Convert.ToString(Session["InstID"]).Trim() != "")
                {
                    sSQL = " update institution set instname = @inm,";
                    sSQL += " CreationDateTime = getdate(), IsBank = 1";
                    sSQL += " where instid = " + BCode;

                    SqlCommand sqc = new SqlCommand(sSQL, oCon);
                    sqc.Parameters.AddWithValue("@inm", BName);
                    sqc.ExecuteNonQuery();
                    oCon.Close();
                    retval = true;
                }
                //obj = oData.RunProc(sSQL);

                //if (Convert.ToString(obj) == "1")
                //{
                //    retval = true;
                //}
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
    }
}