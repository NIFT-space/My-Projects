using IBCS.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClosedXML;

namespace NIFT_CMS
{
    public partial class Reports : System.Web.UI.Page
    {
        public static string Fullname, InstName;
        public static int searchdt = 0;
        public static DataTable dt;
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

                        isAllowed = general.isPageAllowed(sUserId, "Reports");

                        if (isAllowed == false)
                        {
                            Response.Clear();
                            Response.Redirect("Notallowed", true);
                        }

                        if (Session["IsOp"] != null)
                        {
                            lr_7.Visible = true;
                        }

                        //link1.HRef = "Complaint_form.aspx";
                        //link2.HRef = "Inprogress";
                        //link3.HRef = "Pending";
                        link4.HRef = "Closed";
                        //link5.HRef = "Reports.aspx";
                        link6.HRef = "Tickets";
                        link7.HRef = "OpDashboard";
                        /////////////DISPLAY USERNAME//////////////
                        lbl_user.Text = GetFullName();
                        lblmsg.Text = "";

                        if (!IsPostBack)
                        {
                            BindData();
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
            catch (Exception)
            {
                return "";
            }
        }
        protected void BindData()
        {
            try
            {
                string cmd = "";
                ///////FOR NIFT/////////
                //if (Session["InstID"].ToString() == "999")
                //{
                    cmd = "select st.Status_Description as Status,TicketNo,convert(varchar, ComplaintDate, 106) as ComplaintDate," +
                        "FullName, Nature, Wk.Description as Workcode," +
                        "convert(varchar,TATDuration) + ' Days' as TAT_Duration,convert(varchar, DATEDIFF(day, ComplaintDate, GETDATE()))+ ' Days' as Complaint_Duration" +
                        ",Case when TATDuration - DATEDIFF(day, ComplaintDate, GETDATE()) > 0" +
                        " then convert(varchar, (TATDuration-DATEDIFF(day, ComplaintDate, GETDATE()))) +' Days'" +
                        " when TATDuration-DATEDIFF(day, ComplaintDate, GETDATE()) <= 0" +
                        " then convert(varchar,0) +' Days' end as Remaining_Time" +
                        " ,Case when tk.AssigneeID = 999 then 'NIFT OP'" +
                        " when tk.AssigneeID in (select MerchID from Merchant) then(select FirstName + LastName as Fname from Merchant where merchid = tk.AssigneeID)" +
                        " else (select FirstName + LastName as Fname2 from users where instid = tk.AssigneeID) end as Assignee" +
                        ",ContactNumber,EmailAddress,TranSTAN,TranRefNo,TranDate,AccountNumber,Details,Tk_Comment as Closed_Comments from Tickets as tk" +
                        " join Complaint_Status st on st.Status_Code = tk.status" +
                        " join WorkCode Wk on Wk.WorkCode = tk.WorkCode" +
                        " order by tk.Status desc, tk.ComplaintDate desc";
                    fillgridviewrows(cmd);
                
                //}
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
            string ddl = ddl_status.SelectedItem.Text;
            string cmd = "";
            if ((t_fr == "" || t_t == "") && ddl == "Please Select")
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
                //////////////////////////////////////////////////////
                //if (Session["InstID"].ToString() == "999")
                //{
                    cmd = "select * from (";
                    cmd += "select st.Status_Description as Status,TicketNo,convert(varchar, ComplaintDate, 106) as ComplaintDate," +
                        "FullName, Nature, Wk.Description as Workcode," +
                        "convert(varchar,TATDuration) + ' Days' as TAT_Duration,convert(varchar, DATEDIFF(day, ComplaintDate, GETDATE()))+ ' Days' as Complaint_Duration" +
                        ",Case when TATDuration - DATEDIFF(day, ComplaintDate, GETDATE()) > 0" +
                        " then convert(varchar, (TATDuration-DATEDIFF(day, ComplaintDate, GETDATE()))) +' Days'" +
                        " when TATDuration-DATEDIFF(day, ComplaintDate, GETDATE()) <= 0" +
                        " then convert(varchar,0) +' Days' end as Remaining_Time" +
                        " ,Case when tk.AssigneeID = 999 then 'NIFT OP'" +
                        " when tk.AssigneeID in (select MerchID from Merchant) then(select FirstName + LastName as Fname from Merchant where merchid = tk.AssigneeID)" +
                        " else (select FirstName + LastName as Fname2 from users where instid = tk.AssigneeID) end as Assignee" +
                        ",ContactNumber,EmailAddress,TranSTAN,TranRefNo,TranDate,AccountNumber,Details,Tk_Comment as Closed_Comments from Tickets as tk" +
                        " join Complaint_Status st on st.Status_Code = tk.status" +
                        " join WorkCode Wk on Wk.WorkCode = tk.WorkCode) D where ";
                //}
                //else
                //{
                //    cmd = "select * from (";
                //    cmd += "select st.Status_Description as Status,TicketNo,convert(varchar, ComplaintDate, 106) as ComplaintDate," +
                //        "FullName, Nature, Wk.Description as Workcode," +
                //        "convert(varchar,TATDuration) + ' Days' as TAT_Duration,convert(varchar, DATEDIFF(day, ComplaintDate, GETDATE()))+ ' Days' as Complaint_Duration" +
                //        ",Case when TATDuration - DATEDIFF(day, ComplaintDate, GETDATE()) > 0" +
                //        " then convert(varchar, (TATDuration-DATEDIFF(day, ComplaintDate, GETDATE()))) +' Days'" +
                //        " when TATDuration-DATEDIFF(day, ComplaintDate, GETDATE()) <= 0" +
                //        " then convert(varchar,0) +' Days' end as Remaining_Time" +
                //        " ,Case when tk.AssigneeID = 999 then 'NIFT OP'" +
                //        " when tk.AssigneeID in (select MerchID from Merchant) then(select FirstName + LastName as Fname from Merchant where merchid = tk.AssigneeID)" +
                //        " else (select FirstName + LastName as Fname2 from users where instid = tk.AssigneeID) end as Assignee" +
                //        " ,ContactNumber,EmailAddress,TranSTAN,TranRefNo,TranDate,AccountNumber,Details,Tk_Comment as Closed_Comments from Tickets as tk" +
                //        " join Complaint_Status st on st.Status_Code = tk.status" +
                //        " join WorkCode Wk on Wk.WorkCode = tk.WorkCode";
                //    cmd += " Where tk.UserID='" + Session["UserID"].ToString().Trim() + "') D where ";
                //}
            }
            if (txtFromDate.Text != "" || txtFromDate.Text != "")
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

                cmd += " convert(date,ComplaintDate) between @txtFdt and @txtTdt";
                
            }
            if (txtFromDate.Text != "" && txtFromDate.Text != "" && ddl_status.SelectedItem.Text != "Please Select")
            {
                cmd += " and ";
            }

            if (ddl_status.SelectedItem.Text != "Please Select")
            {
                cmd += " D.Status = '" + ddl_status.SelectedItem.Text + "'";
            }
            cmd += " order by Status desc, ComplaintDate desc";

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
                dt = sDS.Tables[0];
            }
        }
        public void fillgridviewrows(string cmd)
        {
            try
            {
                cDataAccess cdata = new cDataAccess();
                dt = cdata.GetDataSet(cmd).Tables[0];
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
                if ((txtFromDate.Text == "" || txtToDate.Text == "") && ddl_status.SelectedValue.ToString() == "10")
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
                    lblmsg.Text = "";
                    BindData_Searchdt();
                    Btn_GenAll.Enabled = true;
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
                lblmsg.Text = "";
                txtFromDate.Text = "";
                txtToDate.Text = "";
                //DropDownList10.SelectedValue = null;
                Gridview1.PageIndex = 0;
                BindData();
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
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
        protected void Gridview1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string cmd = "";
                string t = Gridview1.SelectedRow.Cells[1].Text;
                if (t != "")
                {
                    Session["ticketID"] = t;

                    cmd = "select st.Status_Description as Status,TicketNo,convert(varchar, ComplaintDate, 106) as ComplaintDate," +
                        "FullName, Nature, Wk.Description as Workcode,"+
                        "convert(varchar,TATDuration) + ' Days' as TAT_Duration,convert(varchar, DATEDIFF(day, ComplaintDate, GETDATE()))+ ' Days' as Complaint_Duration"+
                        ",Case when TATDuration - DATEDIFF(day, ComplaintDate, GETDATE()) > 0"+
                        " then convert(varchar, (TATDuration-DATEDIFF(day, ComplaintDate, GETDATE()))) +' Days'"+
                        " when TATDuration-DATEDIFF(day, ComplaintDate, GETDATE()) <= 0"+
                        " then convert(varchar,0) +' Days' end as Remaining_Time"+
                        " ,Case when tk.AssigneeID = 999 then 'NIFT OP'" +
                        " when tk.AssigneeID in (select MerchID from Merchant) then(select FirstName + LastName as Fname from Merchant where merchid = tk.AssigneeID)" +
                        " else (select FirstName + LastName as Fname2 from users where instid = tk.AssigneeID) end as Assignee" +
                        ",ContactNumber,EmailAddress,TranSTAN,TranRefNo,TranDate,AccountNumber,Details,Tk_Comment as Closed_Comments from Tickets as tk" +
                        " join Complaint_Status st on st.Status_Code = tk.status" +
                        " join WorkCode Wk on Wk.WorkCode = tk.WorkCode where TicketNo=" + Session["ticketID"] +
                        " order by tk.Status desc, tk.ComplaintDate desc";
                    fillgridviewrows(cmd);

                    //cDataAccess cdata = new cDataAccess();
                    //string comp_date = cdata.GetDataSet(cmd).Tables[0].Rows[0][2].ToString();
                    //string comp_dur = "" , rem_days="";
                    //DateTime dt1 = Convert.ToDateTime(comp_date);
                    //DateTime dt2 = DateTime.Now;
                    //int duration = dt2.Subtract(dt1).Days;
                    //comp_dur = Convert.ToString(duration) + " Days";
                    //int TATtime = Convert.ToInt32(cdata.GetDataSet(cmd).Tables[0].Rows[0][8].ToString().Substring(0,2));
                    //int remainingdt = TATtime - duration;
                    //if (remainingdt <= 0)
                    //{
                    //    rem_days = "0 Days";
                    //}
                    //else
                    //{
                    //    rem_days = Convert.ToString(remainingdt) + " Days";
                    //}
                    //DataTable dtt2 = new DataTable();
                    //dtt2.Columns.Add("Complaint_Duration", typeof(string));
                    //dtt2.Columns.Add("Remaining_Days", typeof(string));
                    //////////////add new row after adding columns
                    //DataRow dr = dtt2.NewRow();
                    //dr["Complaint_Duration"] = comp_dur;
                    //dr["Remaining_Days"] = rem_days;
                    //dtt2.Rows.Add(dr.ItemArray);
                    ///////////////////////////////////////////
                    //string Path = Server.MapPath("~/");
                    ClosedXML.Excel.XLWorkbook wbook = new ClosedXML.Excel.XLWorkbook();
                    wbook.Worksheets.Add(dt, "CMS Report");
                    //wbook.Worksheet(1).Cell(1, 5).InsertTable(dtt2);
                    // Prepare the response
                    HttpResponse httpResponse = Response;
                    httpResponse.Clear();
                    httpResponse.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    //Provide you file name here
                    httpResponse.AddHeader("content-disposition", "attachment;filename=\"CMSReport_"+t+".xlsx\"");

                    // Flush the workbook to the Response.OutputStream
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        wbook.SaveAs(memoryStream);
                        memoryStream.WriteTo(httpResponse.OutputStream);
                        memoryStream.Close();
                    }

                    httpResponse.End();

                    //////////////////////////////////
                    //FileInfo FI = new FileInfo(Path);
                    //StringWriter stringWriter = new StringWriter();
                    //HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWriter);
                    //DataGrid DataGrd = new DataGrid();
                    ///////////IF DOWNLOAD ALL CLICKED////////
                    ///
                    //DataGrd.DataSource = dt;
                    //DataGrd.DataBind();
                    //////////ELSE////////
                    ///
                    
                    //DataGrd.DataSource = dt;
                    //DataGrd.DataBind();

                    //DataGrd.RenderControl(htmlWrite);
                    //string directory = Path + "Newreport001.xlsx";// GetDirectory(Path);
                    //if (!File.Exists(directory))
                    //{
                    //    File.Create(directory);
                    //}
                    //Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    //Response.ContentType = "application/excel";
                    //Response.AddHeader("Content-Disposition", "attachment;filename=" + "Newreport001.xlsx");
                    
                    //Response.Write(stringWriter.ToString());
                    //Response.End();
                    ////System.IO.StreamWriter vw = new System.IO.StreamWriter(directory, false);
                    //stringWriter.ToString().Normalize();
                    ////vw.Write(stringWriter.ToString());
                    ////vw.Flush();
                    ////vw.Close();
                    ////WriteAttachment(FI.Name, "application/vnd.ms-excel", stringWriter.ToString());

                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }

        protected void ddl_status_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                BindData_Searchdt();
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }

        protected void Btn_GenAll_Click(object sender, EventArgs e)
        {
            try
            {
                if (dt.Rows.Count < 1)
                {
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','Please Select Search Filters')", true);
                    return;
                }
                //if ((txtFromDate.Text == "" && txtToDate.Text == "") || ddl_status.SelectedValue == "10")
                //{
                //    DataTable dt2 = new DataTable();
                //    Gridview1.DataSource = dt2;
                //    Gridview1.DataBind();
                //    //lblmsg.Text = "Please Select Search Filters";
                //    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','Please Select Search Filters')", true);
                //    return;
                //}
                else
                {
                    //////////REPORT ON SEARCH BASIS/////
                    ClosedXML.Excel.XLWorkbook wbook = new ClosedXML.Excel.XLWorkbook();
                    wbook.Worksheets.Add(dt, "CMS Report");
                    // Prepare the response
                    HttpResponse httpResponse = Response;
                    httpResponse.Clear();
                    httpResponse.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    //Provide you file name here
                    string getdate = Convert.ToString(DateTime.Now.ToString("ddMMyyyy"));
                    httpResponse.AddHeader("content-disposition", "attachment;filename=\"CMSReport_"+getdate+".xlsx\"");

                    // Flush the workbook to the Response.OutputStream
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        wbook.SaveAs(memoryStream);
                        memoryStream.WriteTo(httpResponse.OutputStream);
                        memoryStream.Close();
                    }

                    httpResponse.End();
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }
    }
}