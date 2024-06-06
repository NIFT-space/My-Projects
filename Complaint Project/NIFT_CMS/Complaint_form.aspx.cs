using IBCS.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;

namespace NIFT_CMS
{
    public partial class Complaint_form : System.Web.UI.Page
    {
        public static string Fullname, InstName;
        public static string Workcode, AssigneeID, Cust_emailID;
        public static int Tick_id, chkrr = 0;
        public static string TATdur;
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

                        isAllowed = general.isPageAllowed(sUserId, "NewComplaint");

                        if (isAllowed == false)
                        {
                            Response.Clear();
                            Response.Redirect("Notallowed", true);
                        }

                        if (Session["MerchantID"] != null)
                        {
                            //link1.Visible = false;
                        }
                        else if (Session["IsAdmin"] != null)
                        {
                            lr_.Visible = true;
                            //link1.Visible = false;
                        }
                        else if (Session["IsOp"] != null)
                        {
                            lr_.Visible = true;
                            lr_7.Visible = true;
                            link7.HRef = "OpDashboard";
                        }
                        else
                        {
                            //link1.HRef = "Complaintform";
                        }
                        //link2.HRef = "Inprogress";
                        //link3.HRef = "Pending";
                        link4.HRef = "Closed";
                        link5.HRef = "Reports";
                        link6.HRef = "Tickets";
                        p_details.InnerText = "Complaint Description";

                        lbl_user.Text = GetFullName();
                        if (!IsPostBack)
                        {
                            GetWorkCode();
                        }
                    }
                }
                //TATdur = DateTime.Now.Subtract(Convert.ToDateTime(txtDate2.Text)).Days;
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }

        protected void RadioButton2_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (RadioButton2.Checked)
                {
                    t1.Visible = false;
                    t2.Visible = false;
                    t3.Visible = false;
                    t4.Visible = false;
                    t5.Visible = false;
                    t6.Visible = false;
                    Tr1.Visible = false;
                    p_details.InnerText = "Query Description";
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }

        protected void RadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (RadioButton1.Checked)
                {
                    t1.Visible = true;
                    t2.Visible = true;
                    t3.Visible = true;
                    t4.Visible = true;
                    t5.Visible = true;
                    t6.Visible = true;
                    Tr1.Visible = true;
                    p_details.InnerText = "Complaint Description";
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
                SqlConnection gc = cdata.GetConnection();
                int saved_UserLogID = 0;
                string cmd = "Select UserLogID from UserLog where UserID=@UID and Dtn_Code=@dtn";
                SqlCommand sqc = new SqlCommand(cmd, gc);
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
                cdata.CloseConnection(gc);
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

        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            try
            {
                Int64 t_ = Convert.ToInt64(Session["newcompCount"]);
                if (t_ < 10)
                {
                    if (c_name.Text.Length < 30 || email_.Text.Length < 30 || mobile_.Text.Length < 11 || cnic_.Text.Length < 15 || acc_no.Text.Length < 24
                    || stan_.Text.Length < 15 || TRNrefno.Text.Length < 24 || details_.Text.Length < 2000)
                    {
                        /////////////////CHECK KEYWORDS of TEXTBOXES//////////////////////
                        Regex regdetail = new Regex("^[ A-Za-z'`]*$");
                        bool chk1 = regdetail.IsMatch(c_name.Text);
                        if (chk1 == false)
                        {
                            //lblmsg.Text = "Please provide correct details";
                            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','Please provide correct details')", true);
                            return;
                        }
                        regdetail = new Regex("^[A-Za-z0-9'`_()@.]*$");
                        chk1 = regdetail.IsMatch(email_.Text);
                        if (chk1 == false)
                        {
                            //lblmsg.Text = "Please provide correct details";
                            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','Please provide correct details')", true);
                            return;
                        }

                        regdetail = new Regex("^[0-9]*$");
                        chk1 = regdetail.IsMatch(mobile_.Text);
                        if (chk1 == false)
                        {
                            //lblmsg.Text = "Please provide correct details";
                            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','Please provide correct details')", true);
                            return;
                        }

                        regdetail = new Regex("^[0-9-]*$");
                        chk1 = regdetail.IsMatch(cnic_.Text);
                        if (chk1 == false)
                        {
                            //lblmsg.Text = "Please provide correct details";
                            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','Please provide Cnic number')", true);
                            return;
                        }
                        if (cnic_.Text.Length != 15)
                        {
                            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','Please provide correct Cnic number')", true);
                            return;
                        }

                        regdetail = new Regex("^[0-9]*$");
                        chk1 = regdetail.IsMatch(acc_no.Text);
                        if (chk1 == false)
                        {
                            //lblmsg.Text = "Please provide correct details";
                            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','Please provide correct details')", true);
                            return;
                        }

                        regdetail = new Regex("^[0-9a-zA-Z]*$");
                        chk1 = regdetail.IsMatch(stan_.Text);
                        if (chk1 == false)
                        {
                            //lblmsg.Text = "Please provide correct details";
                            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','Please provide correct details')", true);
                            return;
                        }

                        regdetail = new Regex("^[ 0-9a-zA-Z\n-_()`'.,?&/]*$");
                        chk1 = regdetail.IsMatch(details_.Text);
                        if (chk1 == false)
                        {
                            //lblmsg.Text = "Please provide correct details";
                            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','Please provide correct details')", true);
                            return;
                        }
                    }
                    else
                    {
                        //lblmsg.Text = "Please provide correct details";
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','Please provide correct details')", true);
                        return;
                    }
                    /////////////////////////////////////////////////
                    ///
                    string cmd = "";
                    SqlConnection gc = cdata.GetConnection();
                    ////////////////////////////////////////////
                    ///
                    /// 
                    /////GET TAT TIME DURATION
                    try
                    {
                        Workcode = Convert.ToString(ddl_wcode.SelectedItem.Value);
                        cmd = "Select TAT,AssigneeID, emailId from WorkCode Where Workcode=@WKcode";
                        SqlCommand sqc = new SqlCommand(cmd, gc);
                        sqc.Parameters.AddWithValue("@WKcode", Workcode.Trim());
                        using (SqlDataReader rdr = sqc.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                TATdur = rdr.GetString(0);
                                AssigneeID = rdr.GetString(2);
                                Cust_emailID = rdr.GetString(3);
                                Session["Op_email"] = rdr.GetString(3);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    //    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "',''," +
                    //"'Unable to create ticket please try again!')", true);
                        TATdur = "30";
                        LogWriter.WriteToLog(ex);
                    }

                    ////////////////////////////////

                    cmd = "";
                    /////////////FOR COMPLAINT////////////////////
                    if (RadioButton1.Checked == true)
                    {
                        if (c_name.Text != "" && email_.Text != "" && mobile_.Text != "" && cnic_.Text != "" && acc_no.Text != "" && txtFromDate.Text != "" && details_.Text != ""
                            && Workcode != "Please Select" && Workcode != "" && Amount_.Text != "")
                        {
                            cmd = "insert into Tickets(UserID,FullName,ComplaintDate,Nature,WorkCode,Details,Status,CNIC,InstID,BranchID,";
                            cmd += "AccountNumber,TranSTAN,TranRefNo,TranDate,Amount,ContactNumber,TATDuration,EmailAddress,LastUpdated,InitiatorID,AssigneeID)";

                            cmd += "values(@UID , @Cname , '" + DateTime.Now + "' , 'Complaint' , @Workcode , @Details , 1";
                            cmd += ",@Cnic , @InstID , @BRID , @Accno , @STAN , @TRN";
                            cmd += " , @txtdt, @amt_ , @mob , @TAT , @email , '" + DateTime.Now + "'";
                            cmd += ",@Initiator, 999); select @@IDENTITY";

                            SqlCommand sqc = new SqlCommand(cmd, gc);
                            sqc.Parameters.AddWithValue("@UID", Session["UserID"]);
                            sqc.Parameters.AddWithValue("@Cname", c_name.Text.Trim());
                            sqc.Parameters.AddWithValue("@Workcode", Workcode);
                            sqc.Parameters.AddWithValue("@Details", details_.Text.Trim());
                            sqc.Parameters.AddWithValue("@Cnic", cnic_.Text.Trim());
                            sqc.Parameters.AddWithValue("@InstID", Session["InstID"].ToString().PadLeft(3, '0'));
                            //sqc.Parameters.AddWithValue("@BRID", Session["BranchID"].ToString());
                            sqc.Parameters.AddWithValue("@BRID", AssigneeID);
                            sqc.Parameters.AddWithValue("@Accno", acc_no.Text.Trim());
                            sqc.Parameters.AddWithValue("@STAN", stan_.Text.Trim());
                            sqc.Parameters.AddWithValue("@TRN", TRNrefno.Text.Trim());
                            sqc.Parameters.AddWithValue("@txtdt", txtFromDate.Text.Trim());
                            sqc.Parameters.AddWithValue("@amt_", Amount_.Text.Trim());
                            sqc.Parameters.AddWithValue("@mob", mobile_.Text.Trim());
                            sqc.Parameters.AddWithValue("@TAT", TATdur.Trim());
                            sqc.Parameters.AddWithValue("@email", email_.Text.Trim());
                            sqc.Parameters.AddWithValue("@Initiator", Session["InstID"].ToString().PadLeft(3, '0') + "-" + Session["BranchID"].ToString().PadLeft(4, '0'));

                            Tick_id = Convert.ToInt32(sqc.ExecuteScalar());
                            //cdata.CloseConnection(gc);
                            //if (FileUpload1.HasFile)
                            //{
                            //    if(Upload(Tick_id,FileUpload1.PostedFile)== 0)
                            //    {
                            //        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','Invalid File')", true);
                            //        return;
                            //    }
                            //}



                            //////FILE UPLOAD WORKING/////////////
                            //////////////////////////////////////

                            try
                            {
                                string filename1 = "", filename2 = "", filename3 = "";
                                string contentType1 = "", contentType2 = "", contentType3 = "";
                                int fsize1 = 0, fsize2 = 0, fsize3 = 0;
                                byte[] bytes1 = new byte[10], bytes2 = new byte[10], bytes3 = new byte[10];

                                if (FileUpload1.HasFile)
                                {
                                    /////ONLY 3 Files are allowed//////////////
                                    //////////////////////////////////////////
                                    if (FileUpload1.PostedFiles.Count > 1)
                                    {
                                        try
                                        {
                                            cmd = "Delete from tickets where ticketno =" + Tick_id;
                                            gc = cdata.GetConnection();
                                            sqc = new SqlCommand(cmd, gc);
                                            sqc.ExecuteNonQuery();
                                            cdata.CloseConnection(gc);
                                        }
                                        catch (Exception ex2)
                                        {
                                            LogWriter.WriteToLog(ex2);
                                        }
                                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','Only one file is allowed to be uploaded')", true);
                                        return;
                                    }

                                    foreach (HttpPostedFile postedFile in FileUpload1.PostedFiles)
                                    {
                                        if (IsAllowedUpload(postedFile) == 1)
                                        {
                                            try
                                            {
                                                string savdpath = Server.MapPath("~/UploadImg/" + postedFile.FileName);
                                                string savefilename = Path.GetFileName(savdpath);
                                                FileInfo file = new FileInfo(savdpath);
                                                FileStream fs = file.OpenRead();
                                                BinaryReader br = new BinaryReader(fs);

                                                if (chkrr == 0)
                                                {
                                                    filename1 = Path.GetFileName(postedFile.FileName);
                                                    contentType1 = postedFile.ContentType;
                                                    //string EvdPath = Path.GetFullPath(postedFile.FileName);
                                                    fsize1 = postedFile.ContentLength;
                                                    bytes1 = br.ReadBytes(Convert.ToInt32(fs.Length));
                                                }
                                                else if (chkrr == 1)
                                                {
                                                    filename2 = Path.GetFileName(postedFile.FileName);
                                                    contentType2 = postedFile.ContentType;
                                                    fsize2 = postedFile.ContentLength;
                                                    bytes2 = br.ReadBytes(Convert.ToInt32(fs.Length));
                                                }
                                                else if (chkrr == 2)
                                                {
                                                    filename3 = Path.GetFileName(postedFile.FileName);
                                                    contentType3 = postedFile.ContentType;
                                                    fsize3 = postedFile.ContentLength;
                                                    bytes3 = br.ReadBytes(Convert.ToInt32(fs.Length));
                                                }
                                                chkrr++;
                                                fs.Close();
                                                fs.Dispose();
                                                file.Delete();
                                            }
                                            catch (Exception ex)
                                            {
                                                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','Invalid file: " + postedFile.FileName + "')", true);
                                                LogWriter.WriteToLog(ex.Message);
                                            }
                                        }
                                        //////////INCASE OF INVALID FILE///////////
                                        else
                                        {
                                            try
                                            {
                                                cmd = "Delete from tickets where ticketno =" + Tick_id;
                                                gc = cdata.GetConnection();
                                                sqc = new SqlCommand(cmd, gc);
                                                sqc.ExecuteNonQuery();
                                                cdata.CloseConnection(gc);
                                                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','Invalid File : " + postedFile.FileName + "')", true);
                                                return;
                                            }
                                            catch (Exception ex3)
                                            {
                                                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','Invalid File : " + postedFile.FileName + "')", true);
                                                LogWriter.WriteToLog(ex3);
                                                return;
                                            }
                                        }
                                    }

                                    ////////////////INSERT FILE DATA IN DATABASE/////////////////
                                    /////////////////////////////////////////////////////////////

                                    gc = cdata.GetConnection();
                                    string query = "Insert into EvidenceCheck(TicketNo,EvdName1,EvdSize1,EvdType1,EvdData1,EvdName2,EvdSize2,EvdType2,EvdData2,EvdName3,EvdSize3,EvdType3,EvdData3" +
                                                ",EvdDescription,EvdCreationDate,InstID, BranchID,EvdPoster,CommentID) values" +
                                             " (@TickID, @Name1, @FileSize1, @Type1, @Data1, @Name2, @FileSize2, @Type2, @Data2, @Name3, @FileSize3, @Type3, @Data3" +
                                             " , 'N/A' , @Date," + Session["InstID"] + "," + Session["BranchID"] + ",@EvdPoster,1)";

                                    SqlCommand sql = new SqlCommand(query, gc);
                                    sql.Parameters.AddWithValue("@TickID", Convert.ToInt32(Session["ticketID"]));
                                    sql.Parameters.AddWithValue("@Name1", filename1);
                                    sql.Parameters.AddWithValue("@FileSize1", Convert.ToString(fsize1));
                                    sql.Parameters.AddWithValue("@Type1", contentType1);
                                    sql.Parameters.AddWithValue("@Data1", bytes1);
                                    sql.Parameters.AddWithValue("@Name2", filename2);
                                    sql.Parameters.AddWithValue("@FileSize2", Convert.ToString(fsize2));
                                    sql.Parameters.AddWithValue("@Type2", contentType2);
                                    sql.Parameters.AddWithValue("@Data2", bytes2);
                                    sql.Parameters.AddWithValue("@Name3", filename3);
                                    sql.Parameters.AddWithValue("@FileSize3", Convert.ToString(fsize3));
                                    sql.Parameters.AddWithValue("@Type3", contentType3);
                                    sql.Parameters.AddWithValue("@Data3", bytes3);
                                    sql.Parameters.AddWithValue("@Date", DateTime.Now);
                                    sql.Parameters.AddWithValue("@EvdPoster", Fullname);

                                    string query2 = "Insert into EvidenceCheck_Logs(TicketNo,EvdName1,EvdSize1,EvdType1,EvdData1,EvdName2,EvdSize2,EvdType2,EvdData2,EvdName3,EvdSize3,EvdType3,EvdData3" +
                                                ",EvdDescription,EvdCreationDate,InstID, BranchID,EvdPoster,CommentID) values" +
                                             " (@Tick2ID, @N1, @FSize1, @T1, @Dt1, @N2, @FSize2, @T2, @Dt2, @N3, @FSize3, @T3, @Dt3" +
                                             " , 'N/A' , @Dte," + Session["InstID"] + "," + Session["BranchID"] + ",@EPoster,1)";

                                    SqlCommand sql2 = new SqlCommand(query2, gc);
                                    sql2.Parameters.AddWithValue("@Tick2ID", Convert.ToInt32(Session["ticketID"]));
                                    sql2.Parameters.AddWithValue("@N1", filename1);
                                    sql2.Parameters.AddWithValue("@FSize1", Convert.ToString(fsize1));
                                    sql2.Parameters.AddWithValue("@T1", contentType1);
                                    sql2.Parameters.AddWithValue("@Dt1", bytes1);
                                    sql2.Parameters.AddWithValue("@N2", filename2);
                                    sql2.Parameters.AddWithValue("@FSize2", Convert.ToString(fsize2));
                                    sql2.Parameters.AddWithValue("@T2", contentType2);
                                    sql2.Parameters.AddWithValue("@Dt2", bytes2);
                                    sql2.Parameters.AddWithValue("@N3", filename3);
                                    sql2.Parameters.AddWithValue("@FSize3", Convert.ToString(fsize3));
                                    sql2.Parameters.AddWithValue("@T3", contentType3);
                                    sql2.Parameters.AddWithValue("@Dt3", bytes3);
                                    sql2.Parameters.AddWithValue("@Dte", DateTime.Now);
                                    sql2.Parameters.AddWithValue("@EPoster", Fullname);
                                    try
                                    {
                                        sql.ExecuteNonQuery();
                                        sql2.ExecuteNonQuery();
                                        chkrr = 0;
                                    }
                                    catch (Exception ex)
                                    {
                                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','Invalid File Format')", true);
                                        LogWriter.WriteToLog(ex);
                                    }
                                    cdata.CloseConnection(gc);
                                }
                            }
                            catch (Exception ex)
                            {
                                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','Invalid File Format')", true);
                                LogWriter.WriteToLog(ex);
                            }

                            //try
                            //{
                            //    if (FileUpload1.HasFile)
                            //    {
                            //        /////ONLY 3 Files are allowed//////////////
                            //        //////////////////////////////////////////
                            //        if (FileUpload1.PostedFiles.Count > 3)
                            //        {
                            //            try
                            //            {
                            //                cmd = "Delete from tickets where ticketno =" + Tick_id;
                            //                gc = cdata.GetConnection();
                            //                sqc = new SqlCommand(cmd, gc);
                            //                sqc.ExecuteNonQuery();
                            //                cdata.CloseConnection(gc);
                            //            }
                            //            catch (Exception)
                            //            {

                            //            }
                            //            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','Only 3 Files Allowed')", true);
                            //            return;
                            //        }
                            //        foreach (HttpPostedFile postedFile in FileUpload1.PostedFiles)
                            //        {
                            //            if (IsAllowedUpload(postedFile, Tick_id) == 0)
                            //            {
                            //                try
                            //                {
                            //                    cmd = "Delete from tickets where ticketno =" + Tick_id;
                            //                    gc = cdata.GetConnection();
                            //                    sqc = new SqlCommand(cmd, gc);
                            //                    sqc.ExecuteNonQuery();
                            //                    cdata.CloseConnection(gc);
                            //                }
                            //                catch (Exception)
                            //                {

                            //                }
                            //                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','Invalid File: '" + postedFile.FileName + ")", true);
                            //                return;
                            //            }
                            //        }
                            //    }
                            //}
                            //catch (Exception ex)
                            //{
                            //    LogWriter.WriteToLog(ex.ToString());
                            //}
                            ////////////////////////////////////

                            string cmd2 = "insert into Tickets_History(UserID,FullName,ComplaintDate,Nature,WorkCode,Details,Status,CNIC,InstID,BranchID,";
                            cmd2 += "AccountNumber,TranSTAN,TranRefNo,TranDate,Amount,ContactNumber,TATDuration,EmailAddress,LastUpdated,InitiatorID,AssigneeID)";

                            cmd2 += "values(@UID , @Cname , '" + DateTime.Now + "' , 'Complaint' , @Workcode , @Details , 1";
                            cmd2 += ", @Cnic , @InstID , @BRID , @Accno , @STAN , @TRN";
                            cmd2 += " , @txtdt , @amt_, @mob , @TAT , @email , '" + DateTime.Now + "'";
                            cmd2 += ",@Initiator, 999)";

                            gc = cdata.GetConnection();

                            sqc = new SqlCommand(cmd2, gc);
                            sqc.Parameters.AddWithValue("@UID", Session["UserID"]);
                            sqc.Parameters.AddWithValue("@Cname", c_name.Text.Trim());
                            sqc.Parameters.AddWithValue("@Workcode", Workcode);
                            sqc.Parameters.AddWithValue("@Details", details_.Text.Trim());
                            sqc.Parameters.AddWithValue("@Cnic", cnic_.Text.Trim());
                            sqc.Parameters.AddWithValue("@InstID", Session["InstID"].ToString().PadLeft(3, '0'));
                            sqc.Parameters.AddWithValue("@BRID", AssigneeID);
                            sqc.Parameters.AddWithValue("@Accno", acc_no.Text.Trim());
                            sqc.Parameters.AddWithValue("@STAN", stan_.Text.Trim());
                            sqc.Parameters.AddWithValue("@TRN", TRNrefno.Text.Trim());
                            sqc.Parameters.AddWithValue("@txtdt", txtFromDate.Text.Trim());
                            sqc.Parameters.AddWithValue("@amt_", Amount_.Text.Trim());
                            sqc.Parameters.AddWithValue("@mob", mobile_.Text.Trim());
                            sqc.Parameters.AddWithValue("@TAT", TATdur.Trim());
                            sqc.Parameters.AddWithValue("@email", email_.Text.Trim());
                            sqc.Parameters.AddWithValue("@Initiator", Session["InstID"].ToString().PadLeft(3, '0') + "-" + Session["BranchID"].ToString().PadLeft(4, '0'));
                            sqc.ExecuteNonQuery();
                            cdata.CloseConnection(gc);
                            ////////////EMAIL CODE FOR CMS USER/////////////////
                            ////////////////////////////////////////////////////
                            SmtpClient smtpClient = new SmtpClient("mail.nift.pk", 25);
                            string UserEmail = email_.Text.Trim();

                            smtpClient.Credentials = new System.Net.NetworkCredential("no-reply@niftepay.pk", "Abc123=+");
                            //smtpClient.UseDefaultCredentials = true; // uncomment if you don't want to use the network credentials
                            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                            smtpClient.EnableSsl = true;
                            MailMessage mail = new MailMessage();

                            //Setting From , To and CC
                            mail.From = new MailAddress("no-reply@niftepay.pk", "NIFT - ePAY");
                            mail.To.Add(new MailAddress(UserEmail));
                            mail.Subject = "Auto Email Notification - Complaint Creation";

                            string mesage = "Dear " + c_name.Text.Trim() + "," + System.Environment.NewLine;
                            mesage += "Your Complaint has been registered under complaint no. " + Tick_id + " and is under review." +
                                "We will revert to you with an update within 10 working days." + System.Environment.NewLine;
                            mesage += "Regards" + System.Environment.NewLine;
                            mesage += "Complaint Management Unit (NIFT ePay)";

                            mail.Body = mesage;
                            try
                            {
                                smtpClient.Send(mail);
                            }
                            catch
                            {

                            }
                            ///////////////////////////////////////////

                            //////////////EMAIL CODE FOR NIFT Complaint Team/////////////////
                            //////////////////////////////////////////////////////

                            smtpClient.Credentials = new System.Net.NetworkCredential("no-reply@niftepay.pk", "Abc123=+");
                            //smtpClient.UseDefaultCredentials = true; // uncomment if you don't want to use the network credentials
                            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                            smtpClient.EnableSsl = true;
                            MailMessage mail2 = new MailMessage();

                            //Setting From , To and CC
                            mail2.From = new MailAddress("no-reply@niftepay.pk", "NIFT - ePAY");
                            mail2.To.Add(new MailAddress("complaint@niftepay.pk"));
                            mail2.Subject = "Auto Email Notification - Complaint Creation";

                            string niftmesage = "Dear Team," + System.Environment.NewLine;
                            niftmesage += "A Complaint has been registered under complaint no. " + Tick_id + " from " + c_name.Text.Trim() + ", " +
                                 Session["Inst_nm"].ToString() + " at " + DateTime.Now + System.Environment.NewLine;
                            niftmesage += "Regards" + System.Environment.NewLine;
                            niftmesage += "Complaint Management Unit (NIFT ePay)";

                            mail2.Body = niftmesage;
                            try
                            {
                                smtpClient.Send(mail2);
                            }
                            catch
                            {

                            }
                            /////////////////////////////////////////
                            /////////////////EMAIL CODE FOR NIFT OP USER/////////////////
                            //////////////////////////////////////////////////////

                            smtpClient.Credentials = new System.Net.NetworkCredential("no-reply@niftepay.pk", "Abc123=+");
                            //smtpClient.UseDefaultCredentials = true; // uncomment if you don't want to use the network credentials
                            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                            smtpClient.EnableSsl = true;
                            MailMessage mail3 = new MailMessage();

                            //Setting From , To and CC
                            mail3.From = new MailAddress("no-reply@niftepay.pk", "NIFT - ePAY");
                            mail3.To.Add(new MailAddress(Cust_emailID));
                            mail3.Subject = "Auto Email Notification - Complaint Creation";

                            string niftmesage2 = "Dear To," + System.Environment.NewLine;
                            niftmesage2 += "A Complaint has been assignd to you under complaint no. " + Tick_id + " from " + c_name.Text.Trim() + ", " +
                                 Session["Inst_nm"].ToString() + " at " + DateTime.Now + System.Environment.NewLine;
                            niftmesage2 += "Regards" + System.Environment.NewLine;
                            niftmesage2 += "Complaint Management Unit (NIFT ePay)";

                            mail3.Body = niftmesage2;
                            try
                            {
                                smtpClient.Send(mail3);
                            }
                            catch
                            {

                            }
                            /////////////////////////////////////////
                            ///
                            //////////////EMAIL CODE FOR Issuer Bank/////////////////
                            //////////////////////////////////////////////////////

                            smtpClient.Credentials = new System.Net.NetworkCredential("no-reply@niftepay.pk", "Abc123=+");
                            //smtpClient.UseDefaultCredentials = true; // uncomment if you don't want to use the network credentials
                            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                            smtpClient.EnableSsl = true;
                            MailMessage mail4 = new MailMessage();

                            //Setting From , To and CC
                            mail4.From = new MailAddress("no-reply@niftepay.pk", "NIFT - ePAY");
                            mail4.To.Add(new MailAddress(Session["EmailAddress"].ToString()));
                            mail4.Subject = "Auto Email Notification - Complaint Creation";

                            string niftmesage3 = "Your Complaint has been registered under complaint no. " + Tick_id + "  lodge by " + c_name.Text.Trim() + " and is under review. It will be resolved within 04 - 05 working days." + System.Environment.NewLine;
                            
                            niftmesage3 += "Regards," +System.Environment.NewLine;
                            niftmesage3 += "Complaint Management Unit(NIFT ePay)";

                            mail4.Body = niftmesage3;
                            try
                            {
                                smtpClient.Send(mail4);
                            }
                            catch
                            {

                            }
                            //////////////////////////////////////////

                            //lblmsg.Text = "Ticket Submitted Successfully";
                            Session["newtk"] = "1";
                            //ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','Ticket Created Successfully')", true);
                            Response.Redirect("Tickets");
                        }
                        else
                        {
                            //lblmsg.Text = "Please Provide Required Details";
                            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','Please Provide Required Details')", true);
                            return;
                        }
                    }
                    /////////////FOR QUERY////////////////////////
                    else if (RadioButton2.Checked == true)
                    {
                        if (c_name.Text != "" && email_.Text != "" && mobile_.Text != "" && cnic_.Text != "" && details_.Text != "")
                        {
                            cmd = "insert into Tickets(UserID,FullName,ComplaintDate,Nature,Details,Status,CNIC,InstID,BranchID";
                            cmd += " ,ContactNumber,TATDuration,EmailAddress,LastUpdated,InitiatorID,AssigneeID,WorkCode)";

                            cmd += "values(@UID , @Cname , '" + DateTime.Now + "' , 'Query' , @Details , 1";
                            cmd += ",@Cnic , @InstID , @BRID , @mob , 30 , @email , '" + DateTime.Now + "'";
                            cmd += ",@Initiator, 999,1)";

                            gc = cdata.GetConnection();
                            SqlCommand sqc = new SqlCommand(cmd, gc);
                            sqc.Parameters.AddWithValue("@UID", Session["UserID"]);
                            sqc.Parameters.AddWithValue("@Cname", c_name.Text.Trim());
                            sqc.Parameters.AddWithValue("@Details", details_.Text.Trim());
                            sqc.Parameters.AddWithValue("@Cnic", cnic_.Text.Trim());
                            sqc.Parameters.AddWithValue("@InstID", Session["InstID"].ToString().PadLeft(3, '0'));
                            sqc.Parameters.AddWithValue("@BRID", Session["BranchID"].ToString());
                            sqc.Parameters.AddWithValue("@mob", mobile_.Text.Trim());
                            //sqc.Parameters.AddWithValue("@TAT", TATdur.Trim());
                            sqc.Parameters.AddWithValue("@email", email_.Text.Trim());
                            sqc.Parameters.AddWithValue("@Initiator", Session["InstID"].ToString().PadLeft(3, '0') + "-" + Session["BranchID"].ToString().PadLeft(4, '0'));

                            Tick_id = Convert.ToInt32(sqc.ExecuteScalar());
                            //cdata.CloseConnection(gc);
                            string cmd2 = "insert into Tickets_History(UserID,FullName,ComplaintDate,Nature,Details,Status,CNIC,InstID,BranchID";
                            cmd2 += " ,ContactNumber,TATDuration,EmailAddress,LastUpdated,InitiatorID,AssigneeID,WorkCode)";

                            cmd2 += "values(@UID , @Cname , '" + DateTime.Now + "' , 'Query' , @Details , 1";
                            cmd2 += ", @Cnic , @InstID , @BRID, @mob , 30 , @email , '" + DateTime.Now + "'";
                            cmd2 += ",@Initiator, 999,1)";

                            sqc = new SqlCommand(cmd2, gc);
                            sqc.Parameters.AddWithValue("@UID", Session["UserID"]);
                            sqc.Parameters.AddWithValue("@Cname", c_name.Text.Trim());
                            sqc.Parameters.AddWithValue("@Details", details_.Text.Trim());
                            sqc.Parameters.AddWithValue("@Cnic", cnic_.Text.Trim());
                            sqc.Parameters.AddWithValue("@InstID", Session["InstID"].ToString().PadLeft(3, '0'));
                            sqc.Parameters.AddWithValue("@BRID", Session["BranchID"].ToString());
                            sqc.Parameters.AddWithValue("@mob", mobile_.Text.Trim());
                            //sqc.Parameters.AddWithValue("@TAT", TATdur.Trim());
                            sqc.Parameters.AddWithValue("@email", email_.Text.Trim());
                            sqc.Parameters.AddWithValue("@Initiator", Session["InstID"].ToString().PadLeft(3, '0') + "-" + Session["BranchID"].ToString().PadLeft(4, '0'));
                            sqc.ExecuteNonQuery();
                            cdata.CloseConnection(gc);

                            ////////////EMAIL CODE FOR CMS USER/////////////////
                            ////////////////////////////////////////////////////
                            SmtpClient smtpClient = new SmtpClient("mail.nift.pk", 25);
                            string UserEmail = email_.Text.Trim();

                            smtpClient.Credentials = new System.Net.NetworkCredential("no-reply@niftepay.pk", "Abc123=+");
                            //smtpClient.UseDefaultCredentials = true; // uncomment if you don't want to use the network credentials
                            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                            smtpClient.EnableSsl = true;
                            MailMessage mail = new MailMessage();

                            //Setting From , To and CC
                            mail.From = new MailAddress("no-reply@niftepay.pk", "NIFT - ePAY");
                            mail.To.Add(new MailAddress(UserEmail));
                            mail.Subject = "Auto Email Notification - Ticket Creation";

                            string mesage = "Dear " + c_name.Text.Trim() + "," + System.Environment.NewLine;
                            mesage += "Your Query has been registered under complaint no. " + Tick_id + " and is under review." +
                                "We will revert to you with an update within 10 working days." + System.Environment.NewLine;
                            mesage += "Regards" + System.Environment.NewLine;
                            mesage += "Complaint Management Unit (NIFT ePay)";

                            mail.Body = mesage;
                            try
                            {
                                smtpClient.Send(mail);
                            }
                            catch
                            {

                            }
                            //smtpClient.Dispose();
                            ///////////////////////////////////////////
                            /////

                            //////////////EMAIL CODE FOR NIFT Customer Team/////////////////
                            //////////////////////////////////////////////////////
                            //string NIFTEmail = email_.Text.Trim();

                            smtpClient.Credentials = new System.Net.NetworkCredential("no-reply@niftepay.pk", "Abc123=+");
                            //smtpClient.UseDefaultCredentials = true; // uncomment if you don't want to use the network credentials
                            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                            smtpClient.EnableSsl = true;
                            MailMessage mail2 = new MailMessage();

                            //Setting From , To and CC
                            mail2.From = new MailAddress("no-reply@niftepay.pk", "NIFT - ePAY");
                            mail2.To.Add(new MailAddress("complaint@niftepay.pk"));
                            mail2.Subject = "Auto Email Notification - Ticket Creation";

                            string niftmesage = "Dear Operations Team," + System.Environment.NewLine;
                            niftmesage += "A Query has been registered under complaint no. " + Tick_id + " from " + c_name.Text.Trim() + ", " +
                                 Session["Inst_nm"].ToString() + " at " + DateTime.Now + System.Environment.NewLine;
                            niftmesage += "Regards" + System.Environment.NewLine;
                            niftmesage += "Complaint Management Unit (NIFT ePay)";

                            mail2.Body = niftmesage;
                            try
                            {
                                smtpClient.Send(mail2);
                            }
                            catch
                            {

                            }
                            ///////////////////////////////////////////
                            //lblmsg.Text = "Ticket Submitted Successfully";
                            Session["newtk"] = "1";
                            //ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','Ticket Created Successfully')", true);
                            Response.Redirect("Tickets");
                        }
                        else
                        {
                            //lblmsg.Text = "Please Provide Required Details";
                            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','Please Provide Required Details')", true);
                            return;
                        }
                    }
                    else
                    {
                        //lblmsg.Text = "Please Select Nature of Complaint";
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','Please Select Nature of Complaint')", true);
                        return;
                    }

                    Session["newcompCount"] = Convert.ToInt64(Session["newcompCount"]) + 1;
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert",
                         "infomsg('" + InfoType.info + "','','Ticket Limit Reached. Please try again later!')", true);
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "',''," +
                                    "'Unable to create ticket please try again!')", true);
                LogWriter.WriteToLog(ex);
            }
        }

        //protected void ddl_wcode_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        Workcode = Convert.ToString(ddl_wcode.SelectedItem.Value);
        //        SqlConnection gc = cdata.GetConnection();
        //        string cmd = "Select TAT from WorkCode Where Workcode=@WKcode";
        //        DataSet sDS = new DataSet();
        //        SqlCommand sqc = new SqlCommand(cmd, gc);
        //        sqc.Parameters.AddWithValue("@WKcode", Workcode.Trim());
        //        SqlDataAdapter dadEorder = new SqlDataAdapter(sqc);
        //        dadEorder.Fill(sDS);
        //        if (sDS.Tables[0].Rows.Count > 0)
        //        {
        //            TATdur = sDS.Tables[0].Rows[0][0].ToString();
        //        }
        //        else
        //        { TATdur = "30"; }
        //        cdata.CloseConnection(gc);
        //    }
        //    catch (Exception ex)
        //    {
        //        LogWriter.WriteToLog(ex);
        //    }
        //}

        public void GetWorkCode()
        {
            try
            {
                cDataAccess cdata = new cDataAccess();
                string cmd = "Select WorkCode, Description from WorkCode";
                ddl_wcode.DataSource = cdata.GetDataSet(cmd).Tables[0];
                ddl_wcode.DataTextField = Convert.ToString(cdata.GetDataSet(cmd).Tables[0].Columns["Description"]);
                ddl_wcode.DataValueField = Convert.ToString(cdata.GetDataSet(cmd).Tables[0].Columns["WorkCode"]);
                ddl_wcode.DataBind();
                ddl_wcode.Items.Insert(0, "Please Select");
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }

        /////////CHECK ALLOWED EVIDENCE DOCUMENTS/////////////
        protected int IsAllowedUpload(HttpPostedFile fu)
        {
            int AllowUpload = 0;
            try
            {
                String strFileName = fu.FileName;

                SqlConnection gc = cdata.GetConnection();
                if (!(fu == null))
                {
                    try
                    {
                        fu.SaveAs(Server.MapPath("~/UploadImg/" + fu.FileName));
                    }
                    catch (Exception ex)
                    {
                        LogWriter.WriteToLog(ex);
                    }

                    Stream checkfs = fu.InputStream;
                    try
                    {
                        int fsize = fu.ContentLength;
                        if (fsize > 5000000)
                        {
                            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','File size exceeds limit')", true);
                            return AllowUpload;
                        }

                        // DICTIONARY OF ALL IMAGE FILE HEADER
                        Dictionary<string, byte[]> imageHeader = new Dictionary<string, byte[]>();
                        imageHeader.Add("JPG", new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 });
                        imageHeader.Add("JPEG", new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 });
                        imageHeader.Add("PNG", new byte[] { 0x89, 0x50, 0x4E, 0x47 });
                        imageHeader.Add("DOC", new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 });
                        //imageHeader.Add("DOCX", new byte[] { 0x50, 0x4B, 0x03, 0x04 });
                        imageHeader.Add("PDF", new byte[] { 0x25, 0x50, 0x44, 0x46 });
                        imageHeader.Add("XLSX", new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00 });
                        imageHeader.Add("DOCX", new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00 });

                        //imageHeader.Add("TIF", new byte[] { 0x49, 0x49, 0x2A, 0x00 });
                        //imageHeader.Add("TIFF", new byte[] { 0x49, 0x49, 0x2A, 0x00 });
                        //imageHeader.Add("GIF", new byte[] { 0x47, 0x49, 0x46, 0x38 });
                        //imageHeader.Add("BMP", new byte[] { 0x42, 0x4D });
                        //imageHeader.Add("ICO", new byte[] { 0x00, 0x00, 0x01, 0x00 });

                        byte[] header;
                        // GET FILE EXTENSION
                        string fileExt;
                        fileExt = fu.FileName.Substring(fu.FileName.LastIndexOf('.') + 1).ToUpper();

                        byte[] tmp = imageHeader[fileExt];
                        header = new byte[tmp.Length];

                        // GET HEADER INFORMATION OF UPLOADED FILE
                        checkfs.Read(header, 0, header.Length);

                        if (CompareArray(tmp, header))
                        {
                            AllowUpload = 1;
                            return AllowUpload;
                        }
                        else
                        {
                            return AllowUpload;
                        }
                        ///////////////////////////////
                    }
                    catch (Exception ex)
                    {
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','Invalid file')", true);
                        return AllowUpload;
                    }
                }
                else
                {
                    return AllowUpload;
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','Invalid file')", true);
                return AllowUpload;
            }
        }


        //protected int Upload(int Tick_id, HttpPostedFile fu)
        //{
        //    try
        //    {
        //        SqlConnection gc = cdata.GetConnection();
        //        if (!(fu == null))
        //        {
        //            //foreach (HttpPostedFile postedFile in FileUpload1.PostedFiles)
        //            //{;
        //            /////////////////////////////////////////////////////////
        //            bool AllowUpload = false;
        //            String strFileName = fu.FileName;
        //            fu.SaveAs(Server.MapPath("~/" + strFileName));


        //            try
        //            {
        //                int fsize = fu.ContentLength;
        //                if (fsize > 5000000)
        //                {
        //                    //lblmsg.Text = "Upload file size exceeds limit!";
        //                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','Upload file size exceeds limit')", true);
        //                    return 0;
        //                }

        //                // DICTIONARY OF ALL IMAGE FILE HEADER
        //                Dictionary<string, byte[]> imageHeader = new Dictionary<string, byte[]>();
        //                imageHeader.Add("JPG", new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 });
        //                imageHeader.Add("JPEG", new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 });
        //                imageHeader.Add("PNG", new byte[] { 0x89, 0x50, 0x4E, 0x47 });
        //                //imageHeader.Add("DOCX", new byte[] { 0x50, 0x4B, 0x03, 0x04 });
        //                imageHeader.Add("PDF", new byte[] { 0x25, 0x50, 0x44, 0x46 });
        //                imageHeader.Add("XLSX", new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00 });
        //                imageHeader.Add("DOCX", new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00 });
        //                //imageHeader.Add("TIF", new byte[] { 0x49, 0x49, 0x2A, 0x00 });
        //                //imageHeader.Add("TIFF", new byte[] { 0x49, 0x49, 0x2A, 0x00 });
        //                //imageHeader.Add("GIF", new byte[] { 0x47, 0x49, 0x46, 0x38 });
        //                //imageHeader.Add("BMP", new byte[] { 0x42, 0x4D });
        //                //imageHeader.Add("ICO", new byte[] { 0x00, 0x00, 0x01, 0x00 });

        //                byte[] header;
        //                // GET FILE EXTENSION
        //                string fileExt;
        //                fileExt = fu.FileName.Substring(fu.FileName.LastIndexOf('.') + 1).ToUpper();

        //                // CUSTOM VALIDATION GOES HERE BASED ON FILE EXTENSION IF ANY
        //                byte[] tmp = imageHeader[fileExt];
        //                header = new byte[tmp.Length];

        //                // GET HEADER INFORMATION OF UPLOADED FILE
        //                Stream fs = fu.InputStream;
        //                fs.Read(header, 0, header.Length);

        //                if (CompareArray(tmp, header))
        //                {
        //                    AllowUpload = true;
        //                }
        //                else
        //                {
        //                    AllowUpload = false;
        //                }
        //                ///////////////////////////////
        //            }
        //            catch (Exception)
        //            {
        //                //lblmsg.Text = "Invalid file!";
        //                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','Invalid file')", true);
        //                return 0;
        //            }


        //            if (AllowUpload == true)
        //            {
        //                string filename = Path.GetFileName(fu.FileName);
        //                //string cont = FileUpload1.PostedFile.ContentType;
        //                string contentType = fu.ContentType;
        //                //string ftype = Path.GetExtension(fu.FileName).ToLower();
        //                string EvdPath = Path.GetFullPath(fu.FileName);
        //                int fsize = fu.ContentLength;
        //                cDataAccess cdata = new cDataAccess();

        //                string savdpath = Server.MapPath("~/" + strFileName);
        //                string savefilename = null;
        //                savefilename = Path.GetFileName(savdpath);
        //                FileInfo file = new FileInfo(savdpath);
        //                FileStream fs = file.OpenRead();

        //                BinaryReader br = new BinaryReader(fs);

        //                byte[] bytes = br.ReadBytes(Convert.ToInt32(fs.Length));

        //                string query = "Insert into Evidence(TicketNo,EvdName,EvdDescription,EvdSize,EvdData,EvdType,EvdPath,EvdCreationDate,InstID, BranchID,EvdPoster) values" +
        //                         " (@TickID, @Name, @Name ,@FileSize, @Data, @Type, @Path , @Date," + Session["InstID"] + "," + Session["BranchID"] + ",@EvdPoster)";

        //                SqlCommand cmd = new SqlCommand(query, gc);
        //                cmd.Parameters.AddWithValue("@TickID", Tick_id);
        //                cmd.Parameters.AddWithValue("@Name", filename);
        //                cmd.Parameters.AddWithValue("@Type", contentType);
        //                cmd.Parameters.AddWithValue("@Data", bytes);
        //                cmd.Parameters.AddWithValue("@FileSize", Convert.ToString(fsize));
        //                cmd.Parameters.AddWithValue("@Date", DateTime.Now);
        //                cmd.Parameters.AddWithValue("@Path", EvdPath);
        //                cmd.Parameters.AddWithValue("@EvdPoster", Fullname);

        //                string query2 = "Insert into EvidenceLog(TicketNo,EvdName,EvdDescription,EvdSize,EvdData,EvdType,EvdPath,EvdCreationDate,InstID, BranchID,EvdPoster) values" +
        //                         " (@TickID, @Name, @Name ,@FileSize, @Data, @Type, @Path , @Date," + Session["InstID"] + "," + Session["BranchID"] + ",@EvdPoster)";

        //                SqlCommand cmd2 = new SqlCommand(query2, gc);
        //                cmd2.Parameters.AddWithValue("@TickID", Convert.ToInt32(Session["ticketID"]));
        //                cmd2.Parameters.AddWithValue("@Name", filename);
        //                cmd2.Parameters.AddWithValue("@Type", contentType);
        //                cmd2.Parameters.AddWithValue("@Data", bytes);
        //                cmd2.Parameters.AddWithValue("@FileSize", Convert.ToString(fsize));
        //                cmd2.Parameters.AddWithValue("@Date", DateTime.Now);
        //                cmd2.Parameters.AddWithValue("@Path", EvdPath);
        //                cmd2.Parameters.AddWithValue("@EvdPoster", Fullname);
        //                try
        //                {
        //                    cmd.ExecuteNonQuery();
        //                    cmd2.ExecuteNonQuery();
        //                    fs.Close();
        //                    fs.Dispose();
        //                    file.Delete();
        //                }
        //                catch (Exception)
        //                {
        //                    fs.Close();
        //                    fs.Dispose();
        //                    file.Delete();
        //                    return 0;
        //                }
        //                cdata.CloseConnection(gc);
        //            }
        //            else
        //            {
        //                return 0;
        //            }
        //            //}
        //            return 1;
        //        }
        //        else
        //        {
        //            return 0;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogWriter.WriteToLog(ex);
        //        return 0;
        //    }
        //}
        private bool CompareArray(byte[] a1, byte[] a2)
        {
            if (a1.Length != a2.Length)
                return false;

            for (int i = 0; i < a1.Length; i++)
            {
                if (a1[i] != a2[i])
                    return false;
            }

            return true;
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

        //protected void cnic__TextChanged(object sender, EventArgs e)
        //{
        //    if(cnic_.Text.Length >= 5 || cnic_.Text.Length < 12)
        //    {
        //        cnic_.Text = cnic_.Text + "-";
        //    }
        //    else if(cnic_.Text.Length == 12)
        //    {
        //        cnic_.Text = cnic_.Text + "-";
        //    }
        //    else if(cnic_.Text.Length == 13)
        //    {
        //        cnic_.Text = cnic_.Text.Substring(0, 5) + "-" + cnic_.Text.Substring(5, 7) + "-" + cnic_.Text.Substring(12, 1);
        //    }
        //}

        public string GetFullName()
        {
            try
            {
                string cmd;
                SqlConnection gc = cdata.GetConnection();
                cmd = "select Firstname + ' ' + Lastname as FullName, inst.InstName as InstName from users join institution inst on inst.InstID = users.InstID";
                cmd += " Where Userid = @UID";
                DataSet sDS = new DataSet();
                SqlCommand sqc = new SqlCommand(cmd, gc);
                sqc.Parameters.AddWithValue("@UID", Session["UserID"].ToString().Trim());
                SqlDataAdapter dadEorder = new SqlDataAdapter(sqc);
                dadEorder.Fill(sDS);
                foreach (DataRow DR in sDS.Tables[0].Rows)
                {
                    Fullname = Convert.ToString(DR["FullName"]);
                    InstName = Convert.ToString(DR["InstName"]);
                }
                cdata.CloseConnection(gc);
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