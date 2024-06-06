using IBCS.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NIFT_CMS
{
    public partial class AdminTiles : System.Web.UI.Page
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

                //if (Session["UserID"] == null)
                //{
                 //   Response.Clear();
                 //   Response.Redirect("Sessionexpire", true);
                //}
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

                        isAllowed = general.isPageAllowed(sUserId, "Adminmain");

                        if (isAllowed == false)
                        {
                            Response.Clear();
                            Response.Redirect("Notallowed", true);
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
    }
}