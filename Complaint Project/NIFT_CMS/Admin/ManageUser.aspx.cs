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
    public partial class ManageUser : System.Web.UI.Page
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
                if (Session["IiixA8"] == null)
                {
                    Response.Redirect("Sessionexpire");
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

                        isAllowed = general.isPageAllowed(sUserId, "ManageUser");

                        if (isAllowed == false)
                        {
                            Response.Clear();
                            Response.Redirect("Notallowed", true);
                        }
                        if (Page.IsPostBack != true)
                        {
                            if (Session["StateMsg"] != null)
                            {
                                if (Session["StateMsg"].ToString() == "1")
                                {
                                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + 
                                        "','','User Updated Successfully')", true);
                                }
                                else
                                {
                                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info +
                                        "','','User Created Successfully')", true);
                                }
                                Session["StateMsg"] = null;
                            }
                            string referrerUrl = (string)ViewState["ReferrerUrl"];
                            BindSQL();
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
        protected void Btn_back_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("Adminmain");
            }
            catch (Exception ex)
            {
            }
        }
        protected void Btn_Cr_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("Createuser");
            }
            catch (Exception ex)
            {
            }
        }
        private void BindSQL()
        {
            try
            {
                dg_user.DataSource = GetFiles();
                // dg_user.DataMember = "userid";
                dg_user.DataBind();
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }
        protected DataSet GetFiles()
        {
            String sSQL = " select a.userid,a.username,a.firstname+' '+a.lastname fullname,";
            sSQL += " case IsAdmin when 1 then 'Administrator' when 2  then 'Operations' when 3 then 'Merchant' when 4 then 'Bank user' end as usertype, ";
            sSQL += " case isactive ";
            sSQL += " when 1 then 'Yes' ";
            sSQL += " when 0 then 'No' ";
            sSQL += " end isactive";
            sSQL += " ,c.instname bankname ";
            sSQL += " from [users] a ";
            sSQL += " inner join institution c on  a.instid=c.instid ";
            sSQL += " order by a.Isactive desc, a.CreationDateTime desc, fullname asc ";

            cDataAccess oData = new cDataAccess();
            DataSet oDS = oData.GetDataSet(sSQL);

            Session["sSQL"] = sSQL;

            return oDS;

        }

        protected void dg_user_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                string sSQL;
                DataTable dtUser = new DataTable();
                cDataAccess odata = new cDataAccess();

                sSQL = (string)Session["sSQL"];

                SqlDataAdapter adp = odata.GetDataAdapter(sSQL);

                adp.Fill(dtUser);

                if (dg_user.PageCount < Convert.ToInt16(Session["pageindex"]))
                {
                    Session["pageindex"] = dg_user.PageCount;
                }

                dg_user.PageIndex = Convert.ToInt32(e.NewPageIndex);
                Session["pageindex"] = e.NewPageIndex;
                dg_user.DataSource = dtUser;
                dg_user.DataBind();
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }

        protected void dg_user_Sorting(object sender, GridViewSortEventArgs e)
        {
            try
            {
                if (ViewState["SortBy"] == null)
                {
                    ViewState["SortBy"] = "ASC";
                }
                else if (ViewState["SortBy"].ToString().Equals("ASC"))
                {
                    ViewState["SortBy"] = "DESC";
                }
                else
                {
                    ViewState["SortBy"] = "ASC";
                }
                dg_user.Columns[0].HeaderText = "";
                dg_user.Columns[1].HeaderText = "User Name ";
                dg_user.Columns[2].HeaderText = "Bank Name";
                dg_user.Columns[3].HeaderText = "Privileges";
                //dg_user.Columns[4].HeaderText = "Branch Name";
                dg_user.Columns[4].HeaderText = "Active";

                for (int i = 0; i < dg_user.Columns.Count; i++)
                {
                    if (dg_user.Columns[i].SortExpression.Trim().Equals(e.SortExpression.Trim()))
                    {

                        if (ViewState["SortBy"].ToString().Trim().Equals("ASC"))
                        {
                            dg_user.Columns[i].HeaderText = "Department";
                            dg_user.Columns[i].HeaderText = dg_user.Columns[i].HeaderText + "(ASC)";
                        }
                        else
                        {
                            dg_user.Columns[i].HeaderText = "Department";
                            dg_user.Columns[i].HeaderText = dg_user.Columns[i].HeaderText + "(DESC)";
                        }
                    }
                }

                DataView dv = GetFiles().Tables[0].DefaultView;
                dv.Sort = e.SortExpression + " " + ViewState["SortBy"];
                dg_user.DataSource = dv;
                dg_user.DataBind();
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }

        protected void dg_user_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string strValue2 = ((Label)dg_user.SelectedRow.Cells[1].FindControl("userid")).Text;
                Response.Redirect("~/createuser?&h_userid=" + strValue2);
            }
            catch (Exception ex)
            {
            }
        }
        protected void search_btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (search_bar.Text != "")
                {
                    Regex regdetail = new Regex("^[ A-Za-z'`]*$");
                    bool chk1 = regdetail.IsMatch(search_bar.Text);
                    if (chk1 == false)
                    {
                        //lbl_message.Text = "Please provide correct details";
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info
                            + "','','Please Enter Valid User Name')", true);
                        return;
                    }


                    String sSQL = " select a.userid,a.username,a.firstname+' '+a.lastname fullname,";
                    sSQL += " case isactive ";
                    sSQL += " when 1 then 'Yes' ";
                    sSQL += " when 0 then 'No' ";
                    sSQL += " end isactive";
                    sSQL += " ,c.instname bankname  ";
                    sSQL += " from [users] a ";
                    sSQL += " inner join institution c on  a.instid=c.instid ";
                    sSQL += " where a.FirstName+a.LastName like '%" + search_bar.Text + "%' order by a.Isactive desc, a.CreationDateTime desc, fullname asc ";

                    cDataAccess oData = new cDataAccess();
                    DataSet oDS = oData.GetDataSet(sSQL);
                    dg_user.DataSource = oDS;
                    dg_user.DataBind();
                }
                else
                {
                    string sMsg = "Please enter valid search!";
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','" + sMsg + "')", true);
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }

    }
}