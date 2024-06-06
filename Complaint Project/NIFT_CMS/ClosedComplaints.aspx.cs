using IBCS.Data;
using System;
using System.Data;
using System.Web;
using System.Web.UI.WebControls;

namespace NIFT_CMS
{
    public partial class ClosedComplaints : System.Web.UI.Page
    {
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

                        isAllowed = general.isPageAllowed(sUserId, "Closed");

                        if (isAllowed == false)
                        {
                            Response.Clear();
                            Response.Redirect("Notallowed", true);
                        }

                        if (Session["MerchantID"] != null)
                        {
                            link1.Visible = false;
                        }
                        else if (Session["IsAdmin"] != null)
                        {
                            lr_.Visible = true;
                            link1.Visible = false;
                        }
                        else if (Session["IsOp"] != null)
                        {
                            link1.Visible = false;
                            lr_.Visible = true;
                            lr_7.Visible = true;
                            link7.HRef = "OpDashboard";
                        }
                        else
                        {
                            link1.HRef = "NewComplaint";
                        }
                        //link2.HRef = "Inprogress";
                        //link3.HRef = "Pending";
                        //link4.HRef = "ClosedComplaints.aspx";
                        link5.HRef = "Reports";
                        link6.HRef = "Tickets";

                        /////////////DISPLAY USERNAME//////////////
                        string cmd;
                        lbl_user.Text = GetFullName();

                        ///////FOR NIFT/////////
                        if (Session["InstID"].ToString() == "999")
                        {
                            if (Session["BranchID"].ToString() == "99901")
                            {
                                cmd = "select st.Status_Description as Status, '' as NIFTAssignee,TicketNo,convert(varchar, ComplaintDate, 106) as ComplaintDate,";
                                cmd += " FullName, Nature, Wk.Description as Workcode, convert(varchar, LastUpdated,113) as LastUpdated, EmailAddress, ContactNumber from Tickets as tk";
                                cmd += " join Complaint_Status st on st.Status_Code=tk.status";
                                cmd += " join WorkCode Wk on Wk.WorkCode = tk.WorkCode Where tk.status=0 and tk.BranchID = 99901";
                                cmd += " order by tk.LastUpdated desc,tk.Status desc, tk.ComplaintDate desc";
                                fillgridviewrows(cmd);
                            }
                            else if (Session["BranchID"].ToString() == "99902")
                            {
                                cmd = "select st.Status_Description as Status, '' as NIFTAssignee,TicketNo,convert(varchar, ComplaintDate, 106) as ComplaintDate,";
                                cmd += " FullName, Nature, Wk.Description as Workcode, convert(varchar, LastUpdated,113) as LastUpdated, EmailAddress, ContactNumber from Tickets as tk";
                                cmd += " join Complaint_Status st on st.Status_Code=tk.status";
                                cmd += " join WorkCode Wk on Wk.WorkCode = tk.WorkCode Where tk.status=0 and tk.BranchID = 99902";
                                cmd += " order by tk.LastUpdated desc,tk.Status desc, tk.ComplaintDate desc";
                                fillgridviewrows(cmd);
                            }
                            else if (Session["BranchID"].ToString() == "99903")
                            {
                                cmd = "select st.Status_Description as Status, '' as NIFTAssignee,TicketNo,convert(varchar, ComplaintDate, 106) as ComplaintDate,";
                                cmd += " FullName, Nature, Wk.Description as Workcode, convert(varchar, LastUpdated,113) as LastUpdated, EmailAddress, ContactNumber from Tickets as tk";
                                cmd += " join Complaint_Status st on st.Status_Code=tk.status";
                                cmd += " join WorkCode Wk on Wk.WorkCode = tk.WorkCode Where tk.status=0 and tk.BranchID = 99903";
                                cmd += " order by tk.LastUpdated desc,tk.Status desc, tk.ComplaintDate desc";
                                fillgridviewrows(cmd);
                            }
                            else if (Session["BranchID"].ToString() == "99904")
                            {
                                cmd = "select st.Status_Description as Status, '' as NIFTAssignee,TicketNo,convert(varchar, ComplaintDate, 106) as ComplaintDate,";
                                cmd += " FullName, Nature, Wk.Description as Workcode, convert(varchar, LastUpdated,113) as LastUpdated, EmailAddress, ContactNumber from Tickets as tk";
                                cmd += " join Complaint_Status st on st.Status_Code=tk.status";
                                cmd += " join WorkCode Wk on Wk.WorkCode = tk.WorkCode Where tk.status=0 and tk.BranchID = 99904";
                                cmd += " order by tk.LastUpdated desc,tk.Status desc, tk.ComplaintDate desc";
                                fillgridviewrows(cmd);
                            }
                            else if (Session["BranchID"].ToString() == "99905")
                            {
                                cmd = "select st.Status_Description as Status, '' as NIFTAssignee,TicketNo,convert(varchar, ComplaintDate, 106) as ComplaintDate,";
                                cmd += " FullName, Nature, Wk.Description as Workcode, convert(varchar, LastUpdated,113) as LastUpdated, EmailAddress, ContactNumber from Tickets as tk";
                                cmd += " join Complaint_Status st on st.Status_Code=tk.status";
                                cmd += " join WorkCode Wk on Wk.WorkCode = tk.WorkCode Where tk.status=0 and tk.BranchID = 99905";
                                cmd += " order by tk.LastUpdated desc,tk.Status desc, tk.ComplaintDate desc";
                                fillgridviewrows(cmd);
                            }
                            else
                            {
                                cmd = "select st.Status_Description as Status, na.assignee_name as NIFTAssignee,TicketNo,convert(varchar, ComplaintDate, 106) as ComplaintDate,";
                                cmd += " FullName, Nature, Wk.Description as Workcode, convert(varchar, LastUpdated,113) as LastUpdated, EmailAddress, ContactNumber from Tickets as tk";
                                cmd += " join Complaint_Status st on st.Status_Code=tk.status";
                                cmd += " join WorkCode Wk on Wk.WorkCode = tk.WorkCode ";
                                cmd += " join nift_assignee na on na.NIFT_assignee_ID = tk.BranchID Where tk.status=0";
                                cmd += " order by tk.LastUpdated desc,tk.Status desc, tk.ComplaintDate desc";
                                fillgridviewrows(cmd);
                            }
                        }
                        ///////FOR MERCHANT/////////
                        else if (Session["MerchantID"] != null)
                        {
                            cmd = "select st.Status_Description as Status,'' as NIFTAssignee ,TicketNo,convert(varchar, ComplaintDate, 106) as ComplaintDate,";
                            cmd += " FullName, Nature, Wk.Description as Workcode, convert(varchar, LastUpdated,113) as LastUpdated, EmailAddress, ContactNumber from Tickets as tk";
                            cmd += " join Complaint_Status st on st.Status_Code=tk.status";
                            cmd += " join WorkCode Wk on Wk.WorkCode = tk.WorkCode";
                            cmd += " Where tk.status=0 and AssigneeID=" + Session["MerchantID"];
                            cmd += " order by tk.LastUpdated desc,tk.Status desc, tk.ComplaintDate desc";
                            fillgridviewrows(cmd);
                        }
                        ///////FOR USER/////////
                        else
                        {
                            cmd = "select st.Status_Description as Status,'' as NIFTAssignee ,TicketNo,convert(varchar, ComplaintDate, 106) as ComplaintDate,";
                            cmd += " FullName, Nature, Wk.Description as Workcode, convert(varchar, LastUpdated,113) as LastUpdated, EmailAddress, ContactNumber from Tickets as tk";
                            cmd += " join Complaint_Status st on st.Status_Code=tk.status";
                            cmd += " join WorkCode Wk on Wk.WorkCode = tk.WorkCode";
                            cmd += " Where tk.status=0 and tk.UserID='" + Session["UserID"].ToString().Trim() + "'";
                            cmd += " order by tk.LastUpdated desc,tk.Status desc, tk.ComplaintDate desc";

                            fillgridviewrows(cmd);
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

        protected void Btn_back_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("Tickets");
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
                cmd = "select Firstname + ' ' + Lastname as FullName, inst.InstName as InstName from users join institution inst on inst.InstID = users.InstID";
                cmd += " Where Userid = " + Session["UserID"].ToString().Trim();
                foreach (DataRow DR in cdata.GetDataSet(cmd).Tables[0].Rows)
                {
                    Fullname = Convert.ToString(DR["FullName"]);
                    InstName = Convert.ToString(DR["InstName"]);
                }
                return Fullname + " - " + InstName;
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return "";
            }
        }
        public void fillgridviewrows(string cmd)
        {
            try
            {
                cDataAccess cdata = new cDataAccess();
                DataTable dt = cdata.GetDataSet(cmd).Tables[0];
                Gridview1.DataSource = dt;
                Gridview1.DataBind();
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }

        protected void Gridview1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                Gridview1.PageIndex = e.NewPageIndex;
                this.DataBind();
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }
        protected void Gridview1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string t = Gridview1.SelectedRow.Cells[1].Text;
                if (t != "")
                {
                    Session["ticketID"] = t;
                    Response.Redirect("ViewComplaint");
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }
    }
}