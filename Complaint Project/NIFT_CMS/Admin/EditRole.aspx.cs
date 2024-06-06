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
    public partial class EditRole : System.Web.UI.Page
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

                        isAllowed = general.isPageAllowed(sUserId, "EditRole");

                        if (isAllowed == false)
                        {
                            Response.Clear();
                            Response.Redirect("Notallowed", true);
                        }

                        if (!Page.IsPostBack)
                        {
                            string RoleID = "";
                            string RoleName = "";
                            string RoleType = "";

                            //RoleID = Request.Form["h_RoleID"];
                            //RoleName = Request.Form["h_RoleName"];
                            RoleID = Request.QueryString["RoleID"];
                            RoleName = Request.QueryString["RoleNM"];
                            RoleType = Request.Form["r1"] + Request.Form["r2"] + Request.Form["r3"];

                            if (Convert.ToString(RoleID) != null)
                            {
                                //tRole.Enabled = false;
                                //tRoleDesc.Enabled = false;
                                //r1.Enabled = false;
                                //r2.Enabled = false;
                                //r3.Enabled = false;
                            }
                                Session["RoleID"] = RoleID;
                                Session["RoleName"] = RoleName;
                            
                            BindSQL(RoleID);
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
        protected void BindSQL(string RoleID)
        {
            try
            {
                DataTable dtPage = new DataTable();

                cDataAccess odata = new cDataAccess();

                string sSQL = "select * from [page] order by [title] asc ";

                Session["sSQL"] = sSQL;

                SqlDataAdapter adp = odata.GetDataAdapter(sSQL);

                adp.Fill(dtPage);

                grrole.DataSource = dtPage;
                grrole.DataBind();

                if (string.IsNullOrEmpty(RoleID) == false)
                {
                    DataTable dtRole = new DataTable();

                    sSQL = "select * from [role] where roleid=" + RoleID;

                    Session["sSQL"] = sSQL;

                    SqlDataAdapter sAdp = odata.GetDataAdapter(sSQL);

                    sAdp.Fill(dtRole);

                    foreach (DataRow DR in dtRole.Rows)
                    {
                        tRole.Text = (string)DR["Name"];
                        tRoleDesc.Text = (string)DR["Description"];

                        if (Convert.ToString(DR["RoleTypeID"] + "") == "1")
                        {
                            r1.Checked = true;
                        }
                        if (Convert.ToString(DR["RoleTypeID"] + "") == "2")
                        {
                            r2.Checked = true;
                        }
                        if (Convert.ToString(DR["RoleTypeID"] + "") == "3")
                        {
                            r3.Checked = true;
                        }
                    }
                    sSQL = "select * from [role_page] where roleid=" + RoleID;

                    Session["sSQL"] = sSQL;

                    sAdp = odata.GetDataAdapter(sSQL);

                    sAdp.Fill(dtRole);
                    foreach (DataRow DR in dtRole.Rows)
                    {
                        foreach (GridViewRow item in grrole.Rows)
                        {
                            Label pgid = (Label)item.FindControl("PageID");
                            CheckBox chkView = (CheckBox)item.FindControl("cView");

                            if (DR["PageID"].ToString().Trim() == pgid.Text.Trim())
                            {
                                chkView.Checked = true;
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
        protected void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                cDataAccess oData = new cDataAccess();
                oCon.ConnectionString = oData.InitializeConnection();
                oCon.Open();
                String RoleID = "";
                String RoleName = "";
                String RoleType = "";

                if ((r1.Checked == true) || (r2.Checked == true) || (r3.Checked == true))
                {


                    if (r1.Checked == true)
                    {
                        RoleType = "1"; //adminstrator  
                    }
                    if (r2.Checked == true)
                    {
                        RoleType = "2";//user
                    }
                    if (r3.Checked == true)
                    {
                        RoleType = "3";//Operations
                    }

                    if ((Convert.ToString(Session["RoleID"]) != "") && (RoleID.ToString() != "-1"))
                    {
                        RoleID = Convert.ToString(Session["RoleID"] + "");

                        RoleName = Convert.ToString(Session["RoleName"] + "");

                        String sSQL2 = "update [role] set [description]='" + general.SQLEncode(tRoleDesc.Text) + "' , roletypeid=" + RoleType + " where roleid=" + RoleID;

                        oData.RunProc(sSQL2);

                    }


                    if (string.IsNullOrEmpty(RoleID) == true)
                    {
                        RoleID = Convert.ToString(SaveRoleData());
                        Session["RoleID"] = RoleID;
                        Session["RoleName"] = tRole.Text;
                    }
                    else
                    {
                        DeleteRolePage(RoleID);
                    }
                    if (RoleID.ToString() == "-1")
                    {

                        Session["RoleID"] = "";
                        Session["RoleName"] = "";
                    }


                    if (RoleID.ToString() != "-1")
                    {
                        string sSQL;
                        //object obj;

                        foreach (GridViewRow item in grrole.Rows)
                        {

                            Label pgid = (Label)item.FindControl("PageID");

                            Label pgname = (Label)item.FindControl("title");

                            CheckBox chkView = (CheckBox)item.FindControl("cView");

                            if (chkView.Checked == true)
                            {
                                sSQL = "insert into role_page (RoleID,PageID,creationdatetime)";
                                sSQL += "values(@rid,@pid,GETDATE())";
                                
                                SqlCommand sqc = new SqlCommand(sSQL, oCon);
                                sqc.Parameters.AddWithValue("@rid", general.SQLEncode(RoleID));
                                sqc.Parameters.AddWithValue("@pid", general.SQLEncode(pgid.Text));
                                sqc.ExecuteNonQuery();
                                //obj = oData.RunProc(sSQL);
                                //
                            }
                        }
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','Record Updated Successfully')", true);
                        oCon.Close();
                        Response.Redirect("ManageRoles");
                    }
                }
                else
                {
                    string msg = "Please select Role Type.";
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','"+msg+"')", true);
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }
        protected Int64 SaveRoleData()
        {
            try
            {
                cDataAccess oData = new cDataAccess();
                //oCon.ConnectionString = oData.InitializeConnection();
                //oCon.Open();
                string sSQL;
                Int64 IDVal = -1;
                String RoleType = "";

                if (r1.Checked == true)
                {
                    RoleType = "1"; //adminstrator  
                }
                if (r2.Checked == true)
                {
                    RoleType = "2";//user
                }
                if (r3.Checked == true)
                {
                    RoleType = "3";//Operation
                }

                if (isRoleExist(tRole.Text) == false)
                {
                    ////////
                    if (tRole.Text != "")
                    {
                        Regex mob = new Regex("^[ 0-9a-zA-Z-._&()/]*$");
                        bool chk1 = mob.IsMatch(tRole.Text);
                        if (chk1 == false)
                        {
                            string msg = "Please Enter Valid Role Name";
                            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','" + msg + "')", true);
                            return 0;
                        }
                    }

                    if (tRoleDesc.Text != "")
                    {
                        Regex mob = new Regex("^[ 0-9a-zA-Z-._&()/]*$");
                        bool chk1 = mob.IsMatch(tRoleDesc.Text);
                        if (chk1 == false)
                        {
                            string msg = "Please Enter Valid Role Description";
                            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','" + msg + "')", true);
                            return 0;
                        }
                    }


                    /////////
                    sSQL = "insert into [role]([name],[description],[AuditID],[ActionID],[RoleTypeID],[CreationDateTime])";
                    //sSQL += "values (@tRtxt,@tdesc,1,1,@rtpe, '" + DateTime.Now+"')";
                    sSQL += "values ('"+ general.SQLEncode(tRole.Text)+"','"+ general.SQLEncode(tRoleDesc.Text) + "',1,1,'"+ general.SQLEncode(RoleType) + "', '" + DateTime.Now + "')";
                    //SqlCommand sqc = new SqlCommand(sSQL, oCon);
                    //sqc.Parameters.AddWithValue("@tRtxt", general.SQLEncode(tRole.Text));
                    //sqc.Parameters.AddWithValue("@tdesc", general.SQLEncode(tRoleDesc.Text));
                    //sqc.Parameters.AddWithValue("@rtpe", general.SQLEncode(RoleType));
                    IDVal = oData.RunProc(sSQL, "[role]");
                    //oCon.Close();
                    Response.Write("<br>" + sSQL);
                    return IDVal;
                    

                }
                else
                {
                    string msg = "A Role already exists with this role name. Please select other Role Name.";
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','" + msg + "')", true);

                }
                return (IDVal);

            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return 0;
            }
        }
        private bool isRoleExist(string sRoleName)
        {
            try
            {
                string sSQL = "Select * from [role] where [name]='" + general.SQLEncode(sRoleName) + "'";

                cDataAccess obj = new cDataAccess();
                DataSet oDs = obj.GetDataSet(sSQL);

                if (oDs.Tables[0].Rows.Count > 0)
                {
                    bool isRoleExist = true;
                    oDs.Dispose();
                    return isRoleExist;
                }
                else
                {
                    bool isRoleExist = false;
                    oDs.Dispose();
                    return isRoleExist;
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return false;
            }
        }
        private bool isRoleExist(Int64 iRoleID, Int64 iPageID)
        {
            try
            {
                string sSQL = "Select * from [role_page] where [roleid]=" + iRoleID + " and PageID=" + iPageID;

                cDataAccess obj = new cDataAccess();
                DataSet oDs = obj.GetDataSet(sSQL);

                if (oDs.Tables[0].Rows.Count > 0)
                {
                    bool isRoleExist = true;
                    oDs.Dispose();
                    return isRoleExist;
                }
                else
                {
                    bool isRoleExist = false;
                    oDs.Dispose();
                    return isRoleExist;
                }

            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return false;
            }
        }

        protected void DeleteRolePage(string RoleID)
        {
            try
            {
                cDataAccess oData = new cDataAccess();
                string sSQL;

                sSQL = "Delete from [role_page] where roleid=" + general.SQLEncode(RoleID.Trim());

                oData.RunProc(sSQL);
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }
        //protected void DeleteImageData(String RoleID)
        //{
        //    try
        //    {
        //        cDataAccess oData = new cDataAccess();
        //        String sSQL;
        //        object obj;

        //        sSQL = "Delete from [Role_Image_Inst_Branch] where roleid=" + RoleID;

        //        obj = oData.RunProc(sSQL);
        //    }
        //    catch (Exception ex)
        //    {
        //        LogWriter.WriteToLog(ex);
        //    }
        //}

        protected void Button2_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("ManageRoles", true);
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }
    }
}