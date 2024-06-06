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
    public partial class ticket_form : System.Web.UI.Page
    {
        public static string Fullname, InstName, merchid;
        public static int searchdt = 0;
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

                        isAllowed = general.isPageAllowed(sUserId, "Tickets");

                        if (isAllowed == false)
                        {
                            Response.Clear();
                            Response.Redirect("Notallowed", true);
                        }

                        if (Request.UrlReferrer != null)
                        {
                            string previousPageUrl = Request.UrlReferrer.AbsoluteUri;
                            string previousPageName = System.IO.Path.GetFileName(Request.UrlReferrer.AbsolutePath);
                            if (previousPageName == "ViewComplaint")
                            {
                                if (Session["Msg"] != null)
                                {
                                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','Ticket Updated Successfully')", true);
                                }
                            }
                        }

                        CheckMerchant(Session["InstID"].ToString(), Session["BranchID"].ToString());
                        if (Session["MerchantID"] != null)
                        {
                            link1.Visible = false;
                            //Button3.Visible = false;
                        }
                        else if (Session["IsAdmin"] != null)
                        {
                            divInst_.Visible = true;
                            link1.Visible = false;
                            lr_.Visible = true;
                            //Button3.Visible = false;
                        }
                        else if (Session["IsOp"] != null)
                        {
                            link1.Visible = false;
                            divInst_.Visible = true;
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
                        link4.HRef = "Closed";
                        link5.HRef = "Reports";

                        /////////////DISPLAY USERNAME//////////////
                        lbl_user.Text = GetFullName();
                        //lblmsg.Text = "";
                        if (!IsPostBack)
                        {
                            if (Session["newtk"] != null)
                            {
                                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','Ticket Submitted Successfully')", true);
                                Session["newtk"] = null;
                            }
                            BindData();
                            BindBranchDD();
                            Comp_Type.Items.Add("Complaint");
                            Comp_Type.Items.Add("Query");
                        }
                    }
                }
                else
                {
                    Response.Clear();
                    Response.Redirect("Sessionexpire", true);
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }

        protected void BindData()
        {
            try
            {
                string cmd = "";
                ///////FOR NIFT/////////
                if (Session["InstID"].ToString() == "999")
                {
                    if (Session["BranchID"].ToString() == "99901")
                    {
                        cmd = "select st.Status_Description as Status, '' as NIFTAssignee,TicketNo,convert(varchar, ComplaintDate, 106) as ComplaintDate,";
                        cmd += " FullName, inst.InstName, Nature, Wk.Description as Workcode, convert(varchar, LastUpdated,0) as LastUpdated, EmailAddress, ContactNumber from Tickets as tk";
                        cmd += " join Complaint_Status st on st.Status_Code=tk.status";
                        cmd += " join WorkCode Wk on Wk.WorkCode = tk.WorkCode join Institution inst on inst.InstID=tk.InstID ";
                        cmd += " Where tk.status in (1,2,3,4) and tk.BranchID = 99901 ";
                        cmd += " order by tk.LastUpdated desc,tk.Status desc, tk.ComplaintDate desc";
                        fillgridviewrows(cmd);
                    }
                    else if (Session["BranchID"].ToString() == "99902")
                    {
                        cmd = "select st.Status_Description as Status, '' as NIFTAssignee,TicketNo,convert(varchar, ComplaintDate, 106) as ComplaintDate,";
                        cmd += " FullName, inst.InstName, Nature, Wk.Description as Workcode, convert(varchar, LastUpdated,0) as LastUpdated, EmailAddress, ContactNumber from Tickets as tk";
                        cmd += " join Complaint_Status st on st.Status_Code=tk.status";
                        cmd += " join WorkCode Wk on Wk.WorkCode = tk.WorkCode join Institution inst on inst.InstID=tk.InstID ";
                        cmd += " Where tk.status in (1,2,3,4) and tk.BranchID = 99902 ";
                        cmd += " order by tk.LastUpdated desc,tk.Status desc, tk.ComplaintDate desc";
                        fillgridviewrows(cmd);
                    }
                    else if (Session["BranchID"].ToString() == "99903")
                    {
                        cmd = "select st.Status_Description as Status, '' as NIFTAssignee,TicketNo,convert(varchar, ComplaintDate, 106) as ComplaintDate,";
                        cmd += " FullName, inst.InstName, Nature, Wk.Description as Workcode, convert(varchar, LastUpdated,0) as LastUpdated, EmailAddress, ContactNumber from Tickets as tk";
                        cmd += " join Complaint_Status st on st.Status_Code=tk.status";
                        cmd += " join WorkCode Wk on Wk.WorkCode = tk.WorkCode join Institution inst on inst.InstID=tk.InstID ";
                        cmd += " Where tk.status in (1,2,3,4) and tk.BranchID = 99903 ";
                        cmd += " order by tk.LastUpdated desc,tk.Status desc, tk.ComplaintDate desc";
                        fillgridviewrows(cmd);
                    }
                    else if (Session["BranchID"].ToString() == "99904")
                    {
                        cmd = "select st.Status_Description as Status, '' as NIFTAssignee,TicketNo,convert(varchar, ComplaintDate, 106) as ComplaintDate,";
                        cmd += " FullName, inst.InstName, Nature, Wk.Description as Workcode, convert(varchar, LastUpdated,0) as LastUpdated, EmailAddress, ContactNumber from Tickets as tk";
                        cmd += " join Complaint_Status st on st.Status_Code=tk.status";
                        cmd += " join WorkCode Wk on Wk.WorkCode = tk.WorkCode join Institution inst on inst.InstID=tk.InstID ";
                        cmd += " Where tk.status in (1,2,3,4) and tk.BranchID = 99904 ";
                        cmd += " order by tk.LastUpdated desc,tk.Status desc, tk.ComplaintDate desc";
                        fillgridviewrows(cmd);
                    }
                    else if (Session["BranchID"].ToString() == "99905")
                    {
                        cmd = "select st.Status_Description as Status,'' as NIFTAssignee,TicketNo,convert(varchar, ComplaintDate, 106) as ComplaintDate,";
                        cmd += " FullName, inst.InstName, Nature, Wk.Description as Workcode, convert(varchar, LastUpdated,0) as LastUpdated, EmailAddress, ContactNumber from Tickets as tk";
                        cmd += " join Complaint_Status st on st.Status_Code=tk.status";
                        cmd += " join WorkCode Wk on Wk.WorkCode = tk.WorkCode join Institution inst on inst.InstID=tk.InstID ";
                        cmd += " Where tk.status in (1,2,3,4) and tk.BranchID = 99905 ";
                        cmd += " order by tk.LastUpdated desc,tk.Status desc, tk.ComplaintDate desc";
                        fillgridviewrows(cmd);
                    }
                    else
                    {
                        cmd = "select st.Status_Description as Status, na.assignee_name as NIFTAssignee,TicketNo,convert(varchar, ComplaintDate, 106) as ComplaintDate,";
                        cmd += " FullName, inst.InstName, Nature, Wk.Description as Workcode, convert(varchar, LastUpdated,0) as LastUpdated, EmailAddress, ContactNumber from Tickets as tk";
                        cmd += " join Complaint_Status st on st.Status_Code=tk.status";
                        cmd += " join WorkCode Wk on Wk.WorkCode = tk.WorkCode join Institution inst on inst.InstID=tk.InstID ";
                        cmd += " join nift_assignee na on na.NIFT_assignee_ID = tk.BranchID";
                        cmd += " Where tk.status in (1,2,3,4) ";
                        cmd += " order by tk.LastUpdated desc,tk.Status desc, tk.ComplaintDate desc";
                        fillgridviewrows(cmd);

                        //Button3.Visible = false;
                        //Button3.BackColor = Color.IndianRed;
                        //Button3.ForeColor = Color.White;
                    }
                }
                ///////FOR MERCHANT/////////
                else if (Session["MerchantID"] != null)
                {
                    cmd = "select case Status when '4' then 'OPEN' when '3' then 'CLOSED' when '2' then 'OPEN' when '0' then 'CLOSED' when '1' then 'OPEN' end as Status,";
                    cmd += " '' as NIFTAssignee, TicketNo,convert(varchar, ComplaintDate, 106) as ComplaintDate,";
                    cmd += " FullName , merc.FirstName+' '+merc.LastName as InstName, Nature, Wk.Description as Workcode, convert(varchar, LastUpdated,0) as LastUpdated, EmailAddress, ContactNumber from Tickets as tk";
                    cmd += " join Merchant merc on merc.MerchID=tk.AssigneeID";
                    cmd += " join WorkCode Wk on Wk.WorkCode = tk.WorkCode";
                    cmd += " Where AssigneeID=" + merchid;
                    //cmd += " and tk.status=1 ";
                    cmd += " order by tk.LastUpdated desc,tk.Status desc, tk.ComplaintDate desc";
                    fillgridviewrows(cmd);

                    //Button3.Visible = false;
                    //Button3.BackColor = Color.IndianRed;
                    //Button3.ForeColor = Color.White;
                }
                ///////FOR USER/////////
                else
                {
                    cmd = "select case Status when '4' then 'OPEN' when '3' then 'IN-PROGRESS' when '2' then 'IN-PROGRESS' when '0' then 'CLOSED' when '1' then 'OPEN' end as Status,";
                    cmd += " '' as NIFTAssignee, TicketNo,convert(varchar, ComplaintDate, 106) as ComplaintDate,";
                    cmd += " FullName, inst.InstName, Nature, Wk.Description as Workcode, convert(varchar, LastUpdated,0) as LastUpdated, EmailAddress, ContactNumber from Tickets as tk";
                    cmd += " join Complaint_Status st on st.Status_Code=tk.status";
                    cmd += " join WorkCode Wk on Wk.WorkCode = tk.WorkCode join Institution inst on inst.InstID=tk.InstID";
                    cmd += " Where tk.UserID='" + Session["UserID"].ToString().Trim() + "'";
                    cmd += " and tk.status in (1,2,3,4) ";
                    cmd += " order by tk.LastUpdated desc,tk.Status desc, tk.ComplaintDate desc";

                    fillgridviewrows(cmd);
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }
        protected void BindData_Searchdt()
        {
            cDataAccess cdata = new cDataAccess();
            string t_fr = txtFromDate.Text.Trim();
            string t_t = txtToDate.Text.Trim();
            string cmd = "";
            if (t_fr != "" || t_t != "")
            {
                /////////////////CHECK KEYWORDS of TEXTBOXES//////////////////////
                string reg = @"\d{4}(\/|\-)((10|11|12)|0[1-9]{1})(\/|\-)(([0-2][1-9])|([1-2][0-9])|(3[01]{1}))";
                Regex regdetail = new Regex(reg);
                bool chk1 = regdetail.IsMatch(txtFromDate.Text);
                bool chk2 = regdetail.IsMatch(txtToDate.Text);
                if (chk1 == false || chk2 == false)
                {
                    //lblmsg.Text = "Please select correct date";
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','Please select correct date')", true);
                    return;
                }

                //ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','Please select required date')", true);
                //return;
            }


            //////////////////////////////////////////////////////
            if (Session["InstID"] != null)
            {

                if (Session["InstID"].ToString() == "999")
                {

                    if (Session["BranchID"].ToString() == "99901")
                    {
                        cmd = "select * from (";
                        cmd += "select st.Status_Description as Status, '' as NIFTAssignee,TicketNo,convert(varchar, ComplaintDate, 106) as ComplaintDate,";
                        cmd += " FullName, inst.InstID, inst.InstName, Nature, Wk.Description as Workcode, convert(varchar, LastUpdated,0) as LastUpdated, EmailAddress, ContactNumber from Tickets as tk";
                        cmd += " join Complaint_Status st on st.Status_Code=tk.status";
                        cmd += " join WorkCode Wk on Wk.WorkCode = tk.WorkCode join Institution inst on inst.InstID=tk.InstID where tk.status in (1,2,3,4) and tk.BranchID = 99901 ) D where ";
                    }
                    else if (Session["BranchID"].ToString() == "99902")
                    {
                        cmd = "select * from (";
                        cmd += "select st.Status_Description as Status, '' as NIFTAssignee,TicketNo,convert(varchar, ComplaintDate, 106) as ComplaintDate,";
                        cmd += " FullName, inst.InstID, inst.InstName, Nature, Wk.Description as Workcode, convert(varchar, LastUpdated,0) as LastUpdated, EmailAddress, ContactNumber from Tickets as tk";
                        cmd += " join Complaint_Status st on st.Status_Code=tk.status";
                        cmd += " join WorkCode Wk on Wk.WorkCode = tk.WorkCode join Institution inst on inst.InstID=tk.InstID where tk.status in (1,2,3,4) and tk.BranchID = 99902 ) D where ";
                    }
                    else if (Session["BranchID"].ToString() == "99903")
                    {
                        cmd = "select * from (";
                        cmd += "select st.Status_Description as Status, '' as NIFTAssignee,TicketNo,convert(varchar, ComplaintDate, 106) as ComplaintDate,";
                        cmd += " FullName, inst.InstID, inst.InstName, Nature, Wk.Description as Workcode, convert(varchar, LastUpdated,0) as LastUpdated, EmailAddress, ContactNumber from Tickets as tk";
                        cmd += " join Complaint_Status st on st.Status_Code=tk.status";
                        cmd += " join WorkCode Wk on Wk.WorkCode = tk.WorkCode join Institution inst on inst.InstID=tk.InstID where tk.status in (1,2,3,4) and tk.BranchID = 99903 ) D where ";
                    }
                    else if (Session["BranchID"].ToString() == "99904")
                    {
                        cmd = "select * from (";
                        cmd += "select st.Status_Description as Status, '' as NIFTAssignee,TicketNo,convert(varchar, ComplaintDate, 106) as ComplaintDate,";
                        cmd += " FullName, inst.InstID, inst.InstName, Nature, Wk.Description as Workcode, convert(varchar, LastUpdated,0) as LastUpdated, EmailAddress, ContactNumber from Tickets as tk";
                        cmd += " join Complaint_Status st on st.Status_Code=tk.status";
                        cmd += " join WorkCode Wk on Wk.WorkCode = tk.WorkCode join Institution inst on inst.InstID=tk.InstID where tk.status in (1,2,3,4) and tk.BranchID = 99904 ) D where ";
                    }
                    else if (Session["BranchID"].ToString() == "99905")
                    {
                        cmd = "select * from (";
                        cmd += "select st.Status_Description as Status, '' as NIFTAssignee,TicketNo,convert(varchar, ComplaintDate, 106) as ComplaintDate,";
                        cmd += " FullName, inst.InstID, inst.InstName, Nature, Wk.Description as Workcode, convert(varchar, LastUpdated,0) as LastUpdated, EmailAddress, ContactNumber from Tickets as tk";
                        cmd += " join Complaint_Status st on st.Status_Code=tk.status";
                        cmd += " join WorkCode Wk on Wk.WorkCode = tk.WorkCode join Institution inst on inst.InstID=tk.InstID where tk.status in (1,2,3,4) and tk.BranchID = 99905 ) D where ";
                    }
                    else
                    {
                        cmd = "select * from (";
                        cmd += "select st.Status_Description as Status, na.assignee_name as NIFTAssignee,TicketNo,convert(varchar, ComplaintDate, 106) as ComplaintDate,";
                        cmd += " FullName, inst.InstID, inst.InstName, Nature, Wk.Description as Workcode, convert(varchar, LastUpdated,0) as LastUpdated, EmailAddress, ContactNumber from Tickets as tk";
                        cmd += " join Complaint_Status st on st.Status_Code=tk.status";
                        cmd += " join WorkCode Wk on Wk.WorkCode = tk.WorkCode join Institution inst on inst.InstID=tk.InstID";
                        cmd += " join nift_assignee na on na.NIFT_assignee_ID = tk.BranchID";
                        cmd += " where tk.status in (1,2,3,4)) D where ";
                    }
                }
                else
                {
                    cmd = "select * from (";
                    cmd += "select st.Status_Description as Status, '' as NIFTAssignee ,TicketNo,convert(varchar, ComplaintDate, 106) as ComplaintDate,";
                    cmd += " FullName, inst.InstID, inst.InstName, Nature, Wk.Description as Workcode, convert(varchar, LastUpdated,0) as LastUpdated, EmailAddress, ContactNumber from Tickets as tk";
                    cmd += " join Complaint_Status st on st.Status_Code=tk.status";
                    cmd += " join WorkCode Wk on Wk.WorkCode = tk.WorkCode join Institution inst on inst.InstID=tk.InstID";
                    cmd += " Where  tk.status  in (1,2,3,4) and tk.UserID='" + Session["UserID"].ToString().Trim() + "' ) D where ";
                }

                if (txtFromDate.Text != "" && txtFromDate.Text != "")
                {
                    cmd += " convert(date,ComplaintDate) between @txtFdt and @txtTdt";
                }
                if (txtFromDate.Text != "" && txtFromDate.Text != "" && Comp_Type.SelectedItem.Text != "Please Select")
                {
                    cmd += " and ";
                }
                if (Comp_Type.Text != "" && Comp_Type.SelectedItem.Text != "Please Select")
                {
                    cmd += "Nature ='" + Comp_Type.SelectedItem.Text + "'";
                }
                if (Comp_Type.Text != "" && Comp_Type.SelectedItem.Text != "Please Select" && ddl_Inst.SelectedItem.Text != "" && ddl_Inst.SelectedItem.Text != "Select Institute")
                {
                    cmd += " and ";
                }
                if (ddl_Inst.SelectedItem.Text != "" && ddl_Inst.SelectedItem.Text != "Select Institute")
                {
                    cmd += " D.InstID =" + ddl_Inst.SelectedValue.Trim();
                }
                cmd += " order by LastUpdated desc,Status desc, ComplaintDate desc";

                SqlCommand sqc = new SqlCommand(cmd, cdata.GetConnection());
                sqc.Parameters.AddWithValue("@txtFdt", txtFromDate.Text);
                sqc.Parameters.AddWithValue("@txtTdt", txtToDate.Text);
                DataSet sDS = new DataSet();
                SqlDataAdapter dadEorder = new SqlDataAdapter(sqc);
                dadEorder.Fill(sDS);
                if (sDS.Tables[0].Rows.Count <= 0)
                {
                    DataTable dt2 = new DataTable();
                    Gridview1.DataSource = dt2;
                    Gridview1.DataBind();
                }
                else
                {
                    Gridview1.DataSource = sDS;
                    Gridview1.DataBind();
                    Gridview1.PageIndex = 0;
                }
            }
        }

        //protected void Button3_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        Response.Redirect("Complaint_form.aspx");
        //    }
        //    catch (Exception ex)
        //    {
        //        LogWriter.WriteToLog(ex);
        //    }
        //}

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
                string cmd = "Select UserLogID from UserLog where UserID=@UID and Dtn_Code=@dtn";
                SqlCommand sqc = new SqlCommand(cmd, cdata.GetConnection());
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
                if (searchdt == 1)
                { BindData_Searchdt(); }
                else
                { BindData(); }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }

        protected void Btn_Search_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtFromDate.Text == "" && txtToDate.Text == "" && Comp_Type.SelectedItem.Text == "Please Select" && ddl_Inst.SelectedItem.Text == "Select Institute")
                {
                    //DataTable dt2 = new DataTable();
                    //Gridview1.DataSource = dt2;
                    //Gridview1.DataBind();
                    //lblmsg.Text = "Please Select Search Filters";
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','Please Select Search Filters')", true);
                    return;
                }
                else if (txtFromDate.Text == "" && txtToDate.Text != "")
                {
                    DataTable dt2 = new DataTable();
                    Gridview1.DataSource = dt2;
                    Gridview1.DataBind();
                    //lblmsg.Text = "Please Select Search Filters";
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','Please Select Search Filters')", true);
                    return;
                }
                else
                {
                    searchdt = 1;
                    //lblmsg.Text = "";
                    BindData_Searchdt();
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }

        protected void Btn_ShowAll_Click(object sender, EventArgs e)
        {
            try
            {
                searchdt = 0;
                //lblmsg.Text = "";
                txtFromDate.Text = "";
                txtToDate.Text = "";
                Comp_Type.SelectedValue = null;
                ddl_Inst.SelectedValue = null;
                //DropDownList10.SelectedValue = null;
                Gridview1.PageIndex = 0;
                BindData();

                //DataTable dt = new DataTable();
                //string cmd;
                /////////FOR NIFT/////////
                //if (Session["InstID"].ToString() == "999")
                //{
                //    cmd = "select st.Status_Description as Status,TicketNo,convert(varchar, ComplaintDate, 106) as ComplaintDate,";
                //    cmd += " FullName, Nature, Wk.Description as Workcode, convert(varchar, LastUpdated,113) as LastUpdated, EmailAddress, ContactNumber from Tickets as tk";
                //    cmd += " join Complaint_Status st on st.Status_Code=tk.status";
                //    cmd += " join WorkCode Wk on Wk.WorkCode = tk.WorkCode";
                //    cmd += " order by tk.Status desc, tk.LastUpdated desc,tk.ComplaintDate desc";
                //    fillgridviewrows(cmd);

                //    Button3.Visible = false;
                //    Button3.BackColor = Color.IndianRed;
                //    Button3.ForeColor = Color.White;
                //}
                //else
                //{
                //    cmd = "select st.Status_Description as Status,TicketNo,convert(varchar, ComplaintDate, 106) as ComplaintDate,";
                //    cmd += " FullName, Nature, Wk.Description as Workcode, convert(varchar, LastUpdated,113) as LastUpdated, EmailAddress, ContactNumber from Tickets as tk";
                //    cmd += " join Complaint_Status st on st.Status_Code=tk.status";
                //    cmd += " join WorkCode Wk on Wk.WorkCode = tk.WorkCode";
                //    cmd += " Where tk.UserID='" + Session["UserID"].ToString().Trim() + "'";
                //    cmd += " order by tk.Status desc, tk.LastUpdated desc,tk.ComplaintDate desc";

                //    fillgridviewrows(cmd);
                //}
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
                string t = Gridview1.SelectedRow.Cells[2].Text;
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

        public string GetFullName()
        {
            try
            {
                string cmd;
                cDataAccess cdata = new cDataAccess();
                cmd = "select Firstname + ' ' + Lastname as FullName, inst.InstName as InstName from users join institution inst on inst.InstID = users.InstID";
                cmd += " Where Userid = @UID";

                SqlCommand sqc = new SqlCommand(cmd, cdata.GetConnection());
                sqc.Parameters.AddWithValue("@UID", Session["UserID"].ToString().Trim());
                DataSet sDS = new DataSet();
                SqlDataAdapter dadEorder = new SqlDataAdapter(sqc);
                dadEorder.Fill(sDS);
                foreach (DataRow DR in sDS.Tables[0].Rows)
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

        protected void Gridview1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            int i = Gridview1.Rows.Count;
            foreach (GridViewRow item in Gridview1.Rows)
            {
                string id = (item.Cells[2].FindControl("NIFTAssignee") as Label).Text;
                if (string.IsNullOrEmpty(id))
                {
                    Gridview1.Columns[1].Visible = false;
                }
            }
        }

        public void CheckMerchant(string Instid, string BRID)
        {
            try
            {
                cDataAccess cdata = new cDataAccess();
                string cmd = "select MerchID,FirstName+' '+ LastName as fullname from Merchant where InstID = @instid and BranchID =@Brid";
                SqlCommand sqc = new SqlCommand(cmd, cdata.GetConnection());
                sqc.Parameters.AddWithValue("@instid", Instid.Trim());
                sqc.Parameters.AddWithValue("@Brid", BRID.Trim());
                DataSet sDS = new DataSet();
                SqlDataAdapter dadEorder = new SqlDataAdapter(sqc);
                dadEorder.Fill(sDS);
                if (sDS.Tables[0].Rows.Count > 0)
                {
                    Session["MerchantID"] = merchid = sDS.Tables[0].Rows[0][0].ToString();
                    Session["MerchName"] = sDS.Tables[0].Rows[0][1].ToString();
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);

            }
        }
        private void BindBranchDD()
        {
            try
            {
                if (ddl_Inst.SelectedIndex == -1)
                {
                }
                else
                {
                    String sSQL = "";
                    //Branch

                    sSQL += " SELECT instID, InstName from Institution";
                    sSQL += " ORDER by InstName asc";


                    DataTable dtBranch = new DataTable();
                    cDataAccess brdata = new cDataAccess();
                    SqlDataAdapter adp_branch = brdata.GetDataAdapter(sSQL);

                    adp_branch.Fill(dtBranch);

                    ddl_Inst.DataSource = dtBranch;
                    ddl_Inst.DataTextField = dtBranch.Columns["InstName"].ColumnName.ToString();
                    ddl_Inst.DataValueField = dtBranch.Columns["instID"].ColumnName.ToString();
                    ddl_Inst.DataBind();
                    ddl_Inst.Items.Insert(0, new ListItem("Select Institute", "0"));
                    ddl_Inst.SelectedIndex = 0;
                    //ddl_Inst.Items.FindByValue("0").Selected = true;
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }

        //public void BindBranches(string Instid, string BRID)
        //{
        //    try
        //    {
        //        cDataAccess cdata = new cDataAccess();
        //        string cmd = "select MerchID,FirstName+' '+ LastName as fullname from Merchant where InstID = @instid and BranchID =@Brid";
        //        SqlCommand sqc = new SqlCommand(cmd, cdata.GetConnection());
        //        sqc.Parameters.AddWithValue("@instid", Instid.Trim());
        //        sqc.Parameters.AddWithValue("@Brid", BRID.Trim());
        //        DataSet sDS = new DataSet();
        //        SqlDataAdapter dadEorder = new SqlDataAdapter(sqc);
        //        dadEorder.Fill(sDS);
        //        if (sDS.Tables[0].Rows.Count > 0)
        //        {
        //            Session["MerchantID"] = merchid = sDS.Tables[0].Rows[0][0].ToString();
        //            Session["MerchName"] = sDS.Tables[0].Rows[0][1].ToString();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogWriter.WriteToLog(ex);

        //    }
        //}
    }
}