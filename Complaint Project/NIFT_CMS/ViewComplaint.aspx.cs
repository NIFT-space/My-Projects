using IBCS.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NIFT_CMS
{
    public partial class ViewComplaint : System.Web.UI.Page
    {
        public static string Fullname, InstName;
        public static string Status, txt_Assigned, txt_Value, MerchEmail = "", Bank_Name = "", Bank_ID = "";
        public static int changed = 0, commid = 0, Sendmail = 0, chkrr = 0;
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

                        isAllowed = general.isPageAllowed(sUserId, "ViewComplaint");

                        if (isAllowed == false)
                        {
                            Response.Clear();
                            Response.Redirect("Notallowed", true);
                        }

                        if (!IsPostBack)
                        {
                            if (Session["MerchantID"] != null)
                            {
                                link1.Visible = false;
                                Btn_Close.Visible = false;
                            }
                            else if (Session["IsAdmin"] != null)
                            {
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
                            link4.HRef = "Closed";
                            link5.HRef = "Reports";
                            link6.HRef = "Tickets";
                            lbl_user.Text = GetFullName();
                            txtName.Text = Session["FullName"].ToString();
                            GetData();
                            Display_Assignee();

                            if (Session["InstID"].ToString() == "999")
                            {
                                
                                if (Session["BranchID"].ToString() == "9999")
                                {
                                    Btn_Open.Visible = true;
                                    AssigneeID.Visible = false;
                                    ddl_assignee.Visible = true;
                                    
                                    ddl_assignee.Items.Add(new ListItem(Bank_Name, Bank_ID));
                                    ddl_assignee.Items.Add(new ListItem("NIFT Ops", "999"));
                                    try
                                    { ddl_assignee.Items.FindByValue(txt_Value).Selected = true; }
                                    catch (Exception ex)
                                    {
                                        ddl_assignee.SelectedIndex = -1;
                                        LogWriter.WriteToLog(ex);
                                    }
                                }
                                else
                                {
                                    ddl_merchants.Visible = true;
                                    AssigneeID.Visible = false;
                                    ddl_assignee.Visible = false;
                                    //ddl_merchants.Items.Add(new ListItem("mechant1", "1"));
                                    //ddl_merchants.Items.Add(new ListItem("merchant2", "2"));
                                    ddl_merchants.Items.Add(new ListItem("NIFT Ops", "999"));
                                    ddl_merchants.Items.FindByValue(txt_Value).Selected = true;
                                }
                                
                                btn_up.Visible = true;
                                hid_.Visible = true;
                                lr_.Visible = true;
                            }
                            else if (Session["MerchantID"] != null)
                            {
                                Btn_Open.Visible = false;
                            }
                            else
                            {
                                Btn_Open.Visible = true;
                            }
                        }
                        if (Status != null)
                        {
                            if (Status.Trim() == "CLOSED")
                            {
                                txtComment.Enabled = false;
                                btn_Submit.Enabled = false;
                                FileUpload1.Enabled = false;
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
            int saved_UserLogID = 0;
            string cmd = "Select UserLogID from UserLog where UserID=@UID and Dtn_Code=@dtn";
            SqlConnection gc = cdata.GetConnection();
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
            cdata.CloseConnection(gc);
            return true;

        }
        private void show()
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Popup", "Show();", true);
        }
        protected void Btn_Close_Click(object sender, EventArgs e)
        {
            try
            {
                string cmd, status = "";
                cmd = " select status from Tickets where Ticketno = @tkid";
                SqlConnection gc = cdata.GetConnection();
                SqlCommand sqc = new SqlCommand(cmd, gc);
                sqc.Parameters.AddWithValue("@tkid", Session["ticketID"]);
                DataSet sDS = new DataSet();
                SqlDataAdapter dadEorder = new SqlDataAdapter(sqc);
                dadEorder.Fill(sDS);
                if (sDS.Tables[0].Rows.Count > 0)
                {
                    status = sDS.Tables[0].Rows[0][0].ToString();
                }

                if (status.Trim() == "0")
                {
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert",
                        "infomsg('" + InfoType.info + "','','Complaint is already closed')", true);
                }
                else
                {
                    show();
                }
                cdata.CloseConnection(gc);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "',''," +
                    "'Unable to close complaint please try again!')", true);
                LogWriter.WriteToLog(ex);
            }
        }

        protected void Btn_Open_Click(object sender, EventArgs e)
        {
            try
            {
                string confirmValue = Request.Form["confirm_value"];
                string cmd, status = "";

                if (confirmValue == "YES")
                {
                    cmd = " select status from Tickets where Ticketno = @tkid";
                    SqlConnection gc = cdata.GetConnection();
                    SqlCommand sqc = new SqlCommand(cmd, gc);
                    sqc.Parameters.AddWithValue("@tkid", Session["ticketID"]);
                    DataSet sDS = new DataSet();
                    SqlDataAdapter dadEorder = new SqlDataAdapter(sqc);
                    dadEorder.Fill(sDS);
                    if (sDS.Tables[0].Rows.Count > 0)
                    {
                        status = sDS.Tables[0].Rows[0][0].ToString();
                    }

                    if (status.Trim() == "0")
                    {
                        cmd = "Update Tickets set status=4, LastUpdated=GETDATE() where Ticketno=@tkid";
                        sqc = new SqlCommand(cmd, gc);
                        sqc.Parameters.AddWithValue("@tkid", Session["ticketID"]);
                        sqc.ExecuteNonQuery();
                        cdata.CloseConnection(gc);
                        Session["Msg"] = 1;
                        Response.Redirect("Tickets");
                    }
                    else
                    {
                        cdata.CloseConnection(gc);
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert",
                            "infomsg('" + InfoType.info + "','','Complaint is already open')", true);
                    }
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "',''," +
                    "'Unable to open complaint please try again!')", true);
                LogWriter.WriteToLog(ex);
            }
        }

        public string GetFullName()
        {
            try
            {
                string cmd;
                SqlConnection gc = cdata.GetConnection();
                cmd = "select Firstname + ' ' + Lastname as FullName, inst.InstName as InstName from users " +
                    "join institution inst on inst.InstID = users.InstID";
                cmd += " Where Userid = @UID";
                DataSet sDS = new DataSet();
                SqlCommand sqc = new SqlCommand(cmd, gc);
                sqc.Parameters.AddWithValue("@UID", Session["UserID"].ToString().Trim());
                SqlDataAdapter dadEorder = new SqlDataAdapter(sqc);
                dadEorder.Fill(sDS);
                foreach (DataRow DR in sDS.Tables[0].Rows)
                {
                    Session["FullName"] = Fullname = Convert.ToString(DR["FullName"]);
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

        /////////START COMMENT PROCESS///////////////
        /////////////////////////////////////////////
        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            try
            {
                Int64 t_ = Convert.ToInt64(Session["compCount"]);
                if (t_ < 5)
                {

                    if (Session["stu"].ToString() == "CLOSED")
                    {
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert",
                            "infomsg('" + InfoType.info + "','','Ticket is closed!')", true);
                        return;
                    }
                    if (txtComment.Text == "")
                    {
                        ScriptManager.RegisterStartupScript(Page, this.GetType(), "ScrollPage", "window.parent.scroll(0,0);", true);
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert",
                            "infomsg('" + InfoType.info + "','','Please fill out the comment box')", true);
                        return;
                    }
                    Regex regdetail = new Regex("^[ 0-9a-zA-Z-_()`'.,?&/]*$");
                    bool chk1 = regdetail.IsMatch(txtComment.Text);
                    if (chk1 == false)
                    {
                        ScriptManager.RegisterStartupScript(Page, this.GetType(), "ScrollPage", "window.parent.scroll(0,0);", true);
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert",
                            "infomsg('" + InfoType.info + "','','Invalid characters in comment text!')", true);
                        return;
                    }

                    string cmd, cmd2;
                    int comm_ID;
                    SqlConnection gc = cdata.GetConnection();
                    SqlCommand sqc;

                    cmd = "insert into Comments(TicketNo,Commentdt,FullName,Description,InstID,BranchID)" +
                                " Values(@tkid,GETDATE(),@Fname,@comm,@instid,@brid); select @@IDENTITY";

                    sqc = new SqlCommand(cmd, gc);
                    sqc.Parameters.AddWithValue("@tkid", Session["ticketID"]);
                    sqc.Parameters.AddWithValue("@Fname", Session["FullName"]);
                    sqc.Parameters.AddWithValue("@comm", txtComment.Text);
                    sqc.Parameters.AddWithValue("@instid", Session["InstID"]);
                    sqc.Parameters.AddWithValue("@brid", Session["BranchID"]);
                    comm_ID = Convert.ToInt32(sqc.ExecuteScalar());

                    cmd2 = "insert into Comments_History(TicketNo,Commentdt,FullName,Description,InstID,BranchID)" +
                            "Values(@tkid,GETDATE(),@Fname,@comm,@instid,@brid)";
                    sqc = new SqlCommand(cmd2, gc);
                    sqc.Parameters.AddWithValue("@tkid", Session["ticketID"]);
                    sqc.Parameters.AddWithValue("@Fname", Session["FullName"]);
                    sqc.Parameters.AddWithValue("@comm", txtComment.Text);
                    sqc.Parameters.AddWithValue("@instid", Session["InstID"]);
                    sqc.Parameters.AddWithValue("@brid", Session["BranchID"]);
                    sqc.ExecuteNonQuery();
                    cdata.CloseConnection(gc);

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
                                    cmd = "Delete from Comments where CommentID =" + comm_ID;
                                    gc = cdata.GetConnection();
                                    sqc = new SqlCommand(cmd, gc);
                                    sqc.ExecuteNonQuery();
                                    cdata.CloseConnection(gc);
                                }
                                catch (Exception ex2)
                                {
                                    LogWriter.WriteToLog(ex2);
                                }
                                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert",
                                    "infomsg('" + InfoType.info + "','','Only 1 File Allowed')", true);
                                return;
                            }

                            foreach (HttpPostedFile postedFile in FileUpload1.PostedFiles)
                            {
                                if (IsAllowedUpload(postedFile, comm_ID) == 1)
                                {
                                    try
                                    {
                                        //string savdpath = System.AppDomain.CurrentDomain.BaseDirectory + postedFile.FileName;
                                        string savdpath = Server.MapPath("~/UploadImg/" + postedFile.FileName);
                                        //string savdpath = @"D:\\Publish_CMS\\UploadImg\\" + postedFile.FileName;

                                        //string savefilename = Path.GetFileName(savdpath);
                                        FileInfo file = new FileInfo(savdpath);
                                        FileStream fs = file.OpenRead();
                                        //FileStream fs = new FileStream(Session["P2tttxxH"].ToString() , FileMode.Open, FileAccess.Read);

                                        BinaryReader br = new BinaryReader(fs);
                                        if (chkrr == 0)
                                        {
                                            filename1 = Path.GetFileName(postedFile.FileName);

                                            contentType1 = postedFile.ContentType;
                                            //string EvdPath = Path.GetFullPath(postedFile.FileName);
                                            fsize1 = postedFile.ContentLength;
                                            bytes1 = br.ReadBytes(Convert.ToInt32(fs.Length));

                                            //string result = System.Text.Encoding.UTF8.GetString(bytes1);
                                            //LogWriter.WriteToLog("filename"+filename1);
                                            //LogWriter.WriteToLog("size1"+Convert.ToString(fsize1));
                                            //LogWriter.WriteToLog("content"+contentType1);
                                            //LogWriter.WriteToLog("bytes"+ result);
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
                                        //fs.Close();
                                        //fs.Dispose();
                                        //file.Delete();
                                    }
                                    catch (Exception ex)
                                    {
                                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert",
                                            "infomsg('" + InfoType.info + "','','Invalid file: " + postedFile.FileName + "')", true);
                                        LogWriter.WriteToLog(ex.Message);
                                    }
                                }
                                //////////INCASE OF INVALID FILE///////////
                                else
                                {
                                    try
                                    {
                                        cmd = "Delete from Comments where CommentID =" + comm_ID;
                                        gc = cdata.GetConnection();
                                        sqc = new SqlCommand(cmd, gc);
                                        sqc.ExecuteNonQuery();
                                        cdata.CloseConnection(gc);
                                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert",
                                            "infomsg('" + InfoType.info + "','','Invalid File : " + postedFile.FileName + "')", true);
                                        return;
                                    }
                                    catch (Exception ex3)
                                    {
                                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert",
                                            "infomsg('" + InfoType.info + "','','Invalid File : " + postedFile.FileName + "')", true);
                                        LogWriter.WriteToLog(ex3);
                                        return;
                                    }
                                }
                            }

                            ////////////////INSERT FILE DATA IN DATABASE/////////////////
                            /////////////////////////////////////////////////////////////

                            gc = cdata.GetConnection();
                            string query = "Insert into EvidenceCheck(TicketNo,EvdName1,EvdSize1,EvdType1,EvdData1,EvdName2,EvdSize2,EvdType2," +
                                "EvdData2,EvdName3,EvdSize3,EvdType3,EvdData3";
                            query += ",EvdDescription,EvdCreationDate,InstID, BranchID,EvdPoster,CommentID) values";
                            query += " (@TickID, @Name1, @FileSize1, @Type1, @Data1, @Name2, @FileSize2, @Type2, @Data2, @Name3, @FileSize3, " +
                                "@Type3, @Data3";
                            query += " , 'N/A' , @Date," + Session["InstID"] + "," + Session["BranchID"] + ",@EvdPoster,@comid)";
                            //" ('" + Convert.ToInt32(Session["ticketID"]) + "', '" + filename1 + "', '" + Convert.ToString(fsize1) + "'," +
                            //" '" + contentType1 + "', '" + bytes2 + "', '" + filename2 + "', '" + Convert.ToString(fsize2) + "', '"
                            //+ contentType2 + "'" +
                            //", '" + bytes2 + "', '" + filename3 + "', '" + Convert.ToString(fsize3) + "', '" + contentType3 + "', '"
                            //+ bytes3 + "'" +
                            //" , 'N/A' , '" + DateTime.Now + "'," + Session["InstID"] + "," + Session["BranchID"] + ",'" + Fullname +
                            //"','" + comm_ID + "')";

                            //LogWriter.WriteToLog(query);

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
                            sql.Parameters.AddWithValue("@comid", comm_ID);
                            //bytes1 = new byte[10]; bytes2 = new byte[10]; bytes3 = new byte[10];
                            //using (SqlCommand cmd1 = new SqlCommand())
                            //{
                            //    cmd1.CommandTimeout = 120;
                            //    SqlParameter[] parameters =
                            //    {
                            //        new SqlParameter("@TickID", SqlDbType.Int) { Value = Convert.ToInt32(Session["ticketID"]) },
                            //        new SqlParameter("@Name1", SqlDbType.VarChar, 150) { Value = filename1 },
                            //        new SqlParameter("@FileSize1", SqlDbType.VarChar, 50) { Value = Convert.ToString(fsize1)},
                            //        new SqlParameter("@Type1", SqlDbType.VarChar, 100) { Value = contentType1},
                            //        new SqlParameter("@Data1", SqlDbType.VarBinary) { Value = bytes1 },
                            //        new SqlParameter("@Name2", SqlDbType.VarChar, 150) { Value = filename2 },
                            //        new SqlParameter("@FileSize2", SqlDbType.VarChar, 50) { Value = Convert.ToString(fsize2)},
                            //        new SqlParameter("@Type2", SqlDbType.VarChar, 100) { Value = contentType2},
                            //        new SqlParameter("@Data2", SqlDbType.VarBinary) { Value = bytes2 },
                            //        new SqlParameter("@Name3", SqlDbType.VarChar, 150) { Value = filename3 },
                            //        new SqlParameter("@FileSize3", SqlDbType.VarChar, 50) { Value = Convert.ToString(fsize3)},
                            //        new SqlParameter("@Type3", SqlDbType.VarChar, 100) { Value = contentType3},
                            //        new SqlParameter("@Data3", SqlDbType.VarBinary) { Value = bytes3 },
                            //        new SqlParameter("@Date", SqlDbType.DateTime) { Value = DateTime.Now},
                            //        new SqlParameter("@EvdPoster", SqlDbType.VarChar, 50) { Value = Fullname},
                            //        new SqlParameter("@comid", SqlDbType.Int) { Value = comm_ID }
                            //    };
                            //    cmd1.CommandText = query;
                            //    cmd1.Parameters.AddRange(parameters);
                            //    try
                            //    {
                            //        cdata.RunProc(cmd1);
                            //    }
                            //    catch (Exception ex)
                            //    {
                            //        LogWriter.WriteToLog(ex.Message);
                            //    }
                            //}
                            string query2 = "Insert into EvidenceCheck_Logs(TicketNo,EvdName1,EvdSize1,EvdType1,EvdData1,EvdName2,EvdSize2," +
                                "EvdType2,EvdData2,EvdName3,EvdSize3,EvdType3,EvdData3" +
                                        ",EvdDescription,EvdCreationDate,InstID, BranchID,EvdPoster,CommentID) values" +
                                     " (@Tick2ID, @N1, @FSize1, @T1, @Dt1, @N2, @FSize2, @T2, @Dt2, @N3, @FSize3, @T3, @Dt3" +
                                     " , 'N/A' , @Dte," + Session["InstID"] + "," + Session["BranchID"] + ",@EPoster,@cmid)";

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
                            sql2.Parameters.AddWithValue("@cmid", comm_ID);

                            try
                            {
                                sql.ExecuteNonQuery();
                                sql2.ExecuteNonQuery();
                                chkrr = 0;
                            }
                            catch (Exception ex)
                            {
                                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "',''," +
                                    "'Invalid File Format')", true);
                                LogWriter.WriteToLog(ex);
                            }
                            cdata.CloseConnection(gc);
                        }
                    }
                    catch (Exception ex)
                    {
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "',''," +
                            "'Invalid File Format')", true);
                        LogWriter.WriteToLog(ex);
                    }


                    ///////COMMENT BY MERCHANT////////////
                    /////////////////////////////////////
                    if (Session["MerchantID"] != null)
                    {
                        cmd = String.Empty;
                        cmd = "Update Tickets set status=3, LastUpdated=GETDATE(),Tk_Comment='From Merchant:" + Session["MerchantID"] +
                            "' where Ticketno=@tkid";

                        gc = cdata.GetConnection();
                        sqc = new SqlCommand(cmd, gc);
                        sqc.Parameters.AddWithValue("@tkid", Session["ticketID"]);
                        sqc.ExecuteNonQuery();
                        cdata.CloseConnection(gc);
                    }
                    else if (Session["InstID"].ToString() == "999")
                    {
                        cmd = "";
                        sqc = new SqlCommand(cmd, gc);
                        cmd = "Update Tickets set LastUpdated=GETDATE(),Tk_Comment='From NIFT:" + Session["InstID"] +
                            "' where Ticketno=@tkid";
                        gc = cdata.GetConnection();
                        sqc = new SqlCommand(cmd, gc);
                        sqc.Parameters.AddWithValue("@tkid", Session["ticketID"]);
                        sqc.ExecuteNonQuery();
                        cdata.CloseConnection(gc);
                    }
                    else
                    {
                        cmd = "";
                        cmd = "Update Tickets set LastUpdated=GETDATE(),Tk_Comment='From Bank:" + Session["FullName"].ToString() +
                            "' where Ticketno=@tkid";

                        gc = cdata.GetConnection();
                        sqc = new SqlCommand(cmd, gc);
                        sqc.Parameters.AddWithValue("@tkid", Session["ticketID"]);
                        sqc.ExecuteNonQuery();
                        cdata.CloseConnection(gc);
                    }
                    GetData();
                    fillData();
                    Evd_Display();
                    fill_admin_Data();
                    ScriptManager.RegisterStartupScript(Page, this.GetType(), "ScrollPage", "window.parent.scroll(0,0);", true);
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "',''," +
                        "'Complaint Updated Successfully')", true);
                    txtComment.Text = "";

                    SmtpClient smtpClient = new SmtpClient("mail.nift.pk", 25);
                    smtpClient.Credentials = new System.Net.NetworkCredential("no-reply@niftepay.pk", "Abc123=+");
                    //smtpClient.UseDefaultCredentials = true; // uncomment if you don't want to use the network credentials
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.EnableSsl = true;
                    if (Session["InstID"].ToString() != "999")
                    {
                        //////////////EMAIL CODE FOR NIFT OP/////////////////////
                        ////////////////////////////////////////////////////////
                        //MailMessage mail = new MailMessage();
                        ////Setting From , To and CC
                        //mail.From = new MailAddress("no-reply@niftepay.pk", "NIFT - ePAY");
                        //mail.To.Add(new MailAddress("support@niftepay.pk"));
                        //mail.Subject = "Auto Email Notification - Complaint Management";
                        //mail.IsBodyHtml = true;
                        //string message = "Dear Operations Team,";
                        //message += "<br/><br/>Complaint/Query " + Session["ticketID"] + " has been updated by " + txtName.Text.Trim() + " at "
                        //    + DateTime.Now + " against. " + work_code.Text + ".";
                        //message += "<br/><br/>Regards";
                        //message += "<br/>Complaint Management Unit (NIFT ePay)";
                        //mail.Body = message;
                        //try
                        //{
                        //    smtpClient.Send(mail);
                        //}
                        //catch
                        //{

                        //}
                        //////////////////////////////////////////////////////


                        ////////////EMAIL CODE FOR NIFT Assigned USER/////////////////////
                        ////////////////////////////////////////////////////////
                        MailMessage mail1 = new MailMessage();
                        //Setting From , To and CC
                        mail1.From = new MailAddress("no-reply@niftepay.pk", "NIFT - ePAY");
                        mail1.To.Add(new MailAddress(Session["Op_email"].ToString()));
                        mail1.Subject = "Auto Email Notification - Complaint Management";
                        mail1.IsBodyHtml = true;
                        string mesage = "Dear NIFT Operations,";
                        mesage += "<br/><br/>Complaint  " + Session["ticketID"] + " has been updated by " + txtName.Text.Trim() + ", "+ Session["Inst_nm"].ToString() + " at "
                            + DateTime.Now + " against. " + work_code.Text + ".";
                        mesage += "<br/><br/>Regards";
                        mesage += "<br/>Complaint Management Unit (NIFT ePay)";
                        mail1.Body = mesage;
                        try
                        {
                            smtpClient.Send(mail1);
                        }
                        catch
                        {

                        }
                        ///////////////////////////////////////////////////////
                        //}
                        Session["compCount"] = Convert.ToInt64(Session["compCount"]) + 1;
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert",
                         "infomsg('" + InfoType.info + "','','Comment Limit Reached. Please try again later!')", true);
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex.Message);
            }
        }
        ////////////END/////////////////
        ////////////////////////////////

        public void GetData()
        {
            try
            {
                string cmd = "Select cs.Status_Description as Status,Status_Code,convert(varchar, ComplaintDate, 106) as ComplaintDate," +
                    "convert(varchar, LastUpdated,0) as LastUpdated,FullName" +
                    ",EmailAddress,ContactNumber, CNIC, Nature,TATDuration,Wk.Description as Workcode,tk.InstID as iID," +
                    "inst.InstName as InstID, AccountNumber" +
                    " ,TranSTAN,TranRefNo,convert(varchar, TranDate, 106) as TranDate,Amount,TATDuration,inst.InstName as InitiatorID,tk.AssigneeID,Details from Tickets tk " +
                    "join WorkCode Wk on Wk.WorkCode = tk.WorkCode " +
                    " join Institution inst on inst.InstID=tk.InstID join Complaint_Status cs on cs.Status_Code = tk.Status " +
                    "where Ticketno =" + Session["ticketID"];

                foreach (DataRow dr in cdata.GetDataSet(cmd).Tables[0].Rows)
                {
                    if (Session["MerchantID"] != null)
                    {
                        if (Convert.ToInt32(dr["Status_Code"]) == 2)
                        {
                            Session["stu"] = c_status.Text = "OPEN";
                        }
                        else if (Convert.ToInt32(dr["Status_Code"]) == 3)
                        {
                            Session["stu"] = c_status.Text = "CLOSED";
                        }
                        else if (Convert.ToInt32(dr["Status_Code"]) == 4)
                        {
                            Session["stu"] = c_status.Text = "OPEN";
                        }
                        else
                        {
                            Session["stu"] = c_status.Text = "CLOSED";
                        }
                    }
                    else if (Session["IsOp"] != null)
                    {
                        Session["stu"] = Status = c_status.Text = dr["Status"].ToString();
                    }
                    else
                    {
                        if (Convert.ToInt32(dr["Status_Code"]) == 1)
                        {
                            Session["stu"] = c_status.Text = "OPEN";
                        }
                        else if (Convert.ToInt32(dr["Status_Code"]) == 2)
                        {
                            Session["stu"] = c_status.Text = "IN-PROGRESS";
                        }
                        else if (Convert.ToInt32(dr["Status_Code"]) == 3)
                        {
                            Session["stu"] = c_status.Text = "IN-PROGRESS";
                        }
                        else if (Convert.ToInt32(dr["Status_Code"]) == 4)
                        {
                            Session["stu"] = c_status.Text = "OPEN";
                        }
                        else
                        {
                            Session["stu"] = c_status.Text = "CLOSED";
                        }
                    }
                    comp_date.Text = dr["ComplaintDate"].ToString();
                    LastUpdated.Text = dr["LastUpdated"].ToString();
                    comp_name.Text = dr["FullName"].ToString();
                    emailname.Text = dr["EmailAddress"].ToString();
                    contact_num.Text = dr["ContactNumber"].ToString();
                    cnic.Text = dr["CNIC"].ToString();
                    nature.Text = dr["Nature"].ToString();
                    work_code.Text = dr["Workcode"].ToString();
                    c_desc.Text = dr["Details"].ToString();
                    instid.Text = dr["InstID"].ToString().PadLeft(3, '0');
                    //BranchID.Text = dr["BranchID"].ToString().PadLeft(4, '0');
                    Acc_num.Text = dr["AccountNumber"].ToString();
                    TranSTAN.Text = dr["TranSTAN"].ToString();
                    TranRefNo.Text = dr["TranRefNo"].ToString();
                    TranDate.Text = dr["TranDate"].ToString();
                    Amt_.Text = dr["Amount"].ToString();
                    //TAT.Text = dr["TATDuration"].ToString();
                    InitiatorID.Text = dr["InitiatorID"].ToString();
                    txt_Value = dr["AssigneeID"].ToString();
                    txt_Assigned = AssigneeID.Text = GetAssigneeName(txt_Value, dr["iID"].ToString());

                    DateTime dt1 = Convert.ToDateTime(comp_date.Text);
                    DateTime dt2 = DateTime.Now;
                    int duration = dt2.Subtract(dt1).Days;
                    comp_dur.Text = Convert.ToString(duration) + " Days";
                    int TATtime = Convert.ToInt32(dr["TATDuration"]);
                    int remainingdt = TATtime - duration;
                    if (remainingdt <= 0)
                    {
                        rem_days.Text = "0 Days";
                    }
                    else
                    {
                        rem_days.Text = Convert.ToString(remainingdt) + " Days";
                    }
                }
                Evd_Display();
                fillData();
                fill_admin_Data();
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }
        //FillData method for filling Repeater Control with Data
        private void fillData()
        {
            try
            {
                cDataAccess cdata = new cDataAccess();
                string cmd = "";
                if (Session["InstID"].ToString() == "999")
                {
                    if (Session["BranchID"].ToString() == "99901")
                    {
                        cmd = "Select cmt.CommentID,cmt.Commentdt,cmt.FullName,Description," +
                            "CASE when EvdID is NULL then '-' else EvdID end as EvdID,";
                        cmd += "CASE WHEN EvdName1 is NULL THEN 'N/A' ELSE EvdName1 END AS EvdName1,";
                        cmd += "CASE WHEN EvdName2 is NULL THEN 'N/A' ELSE EvdName2 END AS EvdName2,";
                        cmd += "CASE WHEN EvdName3 is NULL THEN 'N/A' ELSE EvdName3 END AS EvdName3,";
                        cmd += "CASE WHEN EvdPoster is NULL THEN 'N/A' ELSE EvdPoster END AS EvdPoster";
                        cmd += " from Comments cmt left join EvidenceCheck evd on evd.CommentID = cmt.CommentID";
                        cmd += " where cmt.ticketno = " + Session["ticketID"] ;
                        cmd += " order by cmt.Commentdt desc, evd.EvdCreationDate desc";
                    }
                    else if (Session["BranchID"].ToString() == "99902")
                    {
                        cmd = "Select cmt.CommentID,cmt.Commentdt,cmt.FullName,Description," +
                            "CASE when EvdID is NULL then '-' else EvdID end as EvdID,";
                        cmd += "CASE WHEN EvdName1 is NULL THEN 'N/A' ELSE EvdName1 END AS EvdName1,";
                        cmd += "CASE WHEN EvdName2 is NULL THEN 'N/A' ELSE EvdName2 END AS EvdName2,";
                        cmd += "CASE WHEN EvdName3 is NULL THEN 'N/A' ELSE EvdName3 END AS EvdName3,";
                        cmd += "CASE WHEN EvdPoster is NULL THEN 'N/A' ELSE EvdPoster END AS EvdPoster";
                        cmd += " from Comments cmt left join EvidenceCheck evd on evd.CommentID = cmt.CommentID";
                        cmd += " where cmt.ticketno = " + Session["ticketID"] ;
                        cmd += " order by cmt.Commentdt desc, evd.EvdCreationDate desc";
                    }
                    else if (Session["BranchID"].ToString() == "99903")
                    {
                        cmd = "Select cmt.CommentID,cmt.Commentdt,cmt.FullName,Description," +
                            "CASE when EvdID is NULL then '-' else EvdID end as EvdID,";
                        cmd += "CASE WHEN EvdName1 is NULL THEN 'N/A' ELSE EvdName1 END AS EvdName1,";
                        cmd += "CASE WHEN EvdName2 is NULL THEN 'N/A' ELSE EvdName2 END AS EvdName2,";
                        cmd += "CASE WHEN EvdName3 is NULL THEN 'N/A' ELSE EvdName3 END AS EvdName3,";
                        cmd += "CASE WHEN EvdPoster is NULL THEN 'N/A' ELSE EvdPoster END AS EvdPoster";
                        cmd += " from Comments cmt left join EvidenceCheck evd on evd.CommentID = cmt.CommentID";
                        cmd += " where cmt.ticketno = " + Session["ticketID"] ;
                        cmd += " order by cmt.Commentdt desc, evd.EvdCreationDate desc";
                    }
                    else if (Session["BranchID"].ToString() == "99904")
                    {
                        cmd = "Select cmt.CommentID,cmt.Commentdt,cmt.FullName,Description," +
                            "CASE when EvdID is NULL then '-' else EvdID end as EvdID,";
                        cmd += "CASE WHEN EvdName1 is NULL THEN 'N/A' ELSE EvdName1 END AS EvdName1,";
                        cmd += "CASE WHEN EvdName2 is NULL THEN 'N/A' ELSE EvdName2 END AS EvdName2,";
                        cmd += "CASE WHEN EvdName3 is NULL THEN 'N/A' ELSE EvdName3 END AS EvdName3,";
                        cmd += "CASE WHEN EvdPoster is NULL THEN 'N/A' ELSE EvdPoster END AS EvdPoster";
                        cmd += " from Comments cmt left join EvidenceCheck evd on evd.CommentID = cmt.CommentID";
                        cmd += " where cmt.ticketno = " + Session["ticketID"] ;
                        cmd += " order by cmt.Commentdt desc, evd.EvdCreationDate desc";
                    }
                    else if (Session["BranchID"].ToString() == "99905")
                    {
                        cmd = "Select cmt.CommentID,cmt.Commentdt,cmt.FullName,Description," +
                            "CASE when EvdID is NULL then '-' else EvdID end as EvdID,";
                        cmd += "CASE WHEN EvdName1 is NULL THEN 'N/A' ELSE EvdName1 END AS EvdName1,";
                        cmd += "CASE WHEN EvdName2 is NULL THEN 'N/A' ELSE EvdName2 END AS EvdName2,";
                        cmd += "CASE WHEN EvdName3 is NULL THEN 'N/A' ELSE EvdName3 END AS EvdName3,";
                        cmd += "CASE WHEN EvdPoster is NULL THEN 'N/A' ELSE EvdPoster END AS EvdPoster";
                        cmd += " from Comments cmt left join EvidenceCheck evd on evd.CommentID = cmt.CommentID";
                        cmd += " where cmt.ticketno = " + Session["ticketID"] ;
                        cmd += " order by cmt.Commentdt desc, evd.EvdCreationDate desc";
                    }
                    else
                    {
                        //cmd = "Select cm.CommentID,cm.Commentdt,cm.FullName,cm.Description,evd.EvdID,CASE WHEN EvdName = NULL THEN 'Nill'
                        //ELSE EvdName END AS EvdName";
                        //cmd += " from Comments cm";
                        //cmd += " left join Evidence evd on evd.commentid = cm.CommentID";
                        //cmd += " where cm.ticketno=" + Session["ticketID"] + " order by Commentdt desc";

                        //cmd = "Select CommentID,Commentdt,FullName,Description from Comments where ticketno=" + Session["ticketID"] +
                        //" order by Commentdt desc";
                        cmd = "Select cmt.CommentID,cmt.Commentdt,cmt.FullName,Description," +
                            "CASE when EvdID is NULL then '-' else EvdID end as EvdID,";
                        cmd += "CASE WHEN EvdName1 is NULL THEN 'N/A' ELSE EvdName1 END AS EvdName1,";
                        cmd += "CASE WHEN EvdName2 is NULL THEN 'N/A' ELSE EvdName2 END AS EvdName2,";
                        cmd += "CASE WHEN EvdName3 is NULL THEN 'N/A' ELSE EvdName3 END AS EvdName3,";
                        cmd += "CASE WHEN EvdPoster is NULL THEN 'N/A' ELSE EvdPoster END AS EvdPoster";
                        cmd += " from Comments cmt left join EvidenceCheck evd on evd.CommentID = cmt.CommentID";
                        cmd += " where cmt.ticketno = " + Session["ticketID"] + "";
                        cmd += " order by cmt.Commentdt desc, evd.EvdCreationDate desc";
                    }
                }
                else if (Session["MerchantID"] == null)
                {
                    //cmd = "Select CommentID,Commentdt,FullName,Description from Comments where ticketno=" + Session["ticketID"] + 
                    //    " and InstID in (999," + Session["InstID"] + ") order by Commentdt desc";
                    cmd = "Select cmt.CommentID,cmt.Commentdt,cmt.FullName,Description," +
                        "CASE when EvdID is NULL then '-' else EvdID end as EvdID,";
                    cmd += "CASE WHEN EvdName1 is NULL THEN 'N/A' ELSE EvdName1 END AS EvdName1,";
                    cmd += "CASE WHEN EvdName2 is NULL THEN 'N/A' ELSE EvdName2 END AS EvdName2,";
                    cmd += "CASE WHEN EvdName3 is NULL THEN 'N/A' ELSE EvdName3 END AS EvdName3,";
                    cmd += "CASE WHEN EvdPoster is NULL THEN 'N/A' ELSE EvdPoster END AS EvdPoster";
                    cmd += " from Comments cmt left join EvidenceCheck evd on evd.CommentID = cmt.CommentID";
                    cmd += " where cmt.ticketno = " + Session["ticketID"] + " and cmt.InstID in (999," + Session["InstID"] + ")";
                    cmd += " order by cmt.Commentdt desc, evd.EvdCreationDate desc";
                }
                else
                {
                    //cmd = "Select CommentID,Commentdt,FullName,Description from Comments where ticketno=" + Session["ticketID"] +
                    //" order by Commentdt desc";
                    cmd = "Select cmt.CommentID,cmt.Commentdt,cmt.FullName,Description," +
                        "CASE when EvdID is NULL then '-' else EvdID end as EvdID,";
                    cmd += "CASE WHEN EvdName1 is NULL THEN 'N/A' ELSE EvdName1 END AS EvdName1,";
                    cmd += "CASE WHEN EvdName2 is NULL THEN 'N/A' ELSE EvdName2 END AS EvdName2,";
                    cmd += "CASE WHEN EvdName3 is NULL THEN 'N/A' ELSE EvdName3 END AS EvdName3,";
                    cmd += "CASE WHEN EvdPoster is NULL THEN 'N/A' ELSE EvdPoster END AS EvdPoster";
                    cmd += " from Comments cmt left join EvidenceCheck evd on evd.CommentID = cmt.CommentID";
                    cmd += " where cmt.ticketno = " + Session["ticketID"] + "";
                    cmd += " order by cmt.Commentdt desc, evd.EvdCreationDate desc";
                }

                PagedDataSource pds = new PagedDataSource();
                DataView dv = new DataView(cdata.GetDataSet(cmd).Tables[0]);
                //foreach(DataRow dr in cdata.GetDataSet(cmd).Tables[0].Rows)
                //{ commid = Convert.ToInt32(dr["CommentID"]); }

                pds.DataSource = dv;
                pds.AllowPaging = true;
                pds.PageSize = 5;
                pds.CurrentPageIndex = PageNumber;
                if (pds.PageCount > 1)
                {
                    rptPaging.Visible = true;
                    ArrayList arraylist = new ArrayList();
                    for (int i = 0; i < pds.PageCount; i++)
                        arraylist.Add((i + 1).ToString());
                    rptPaging.DataSource = arraylist;
                    rptPaging.DataBind();
                }
                else
                {
                    rptPaging.Visible = false;
                }
                Repeater1.DataSource = pds;
                Repeater1.DataBind();

                ////////////////////////////////////////////////////


            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }
        //FillData method for filling Repeater Control with Evidence Data
        private void fill_admin_Data()
        {
            try
            {
                cDataAccess cdata = new cDataAccess();
                string cmd = "";
                if (Session["InstID"].ToString() == "999")
                {
                    cmd = "Select CASE when EvdID is NULL then '-' else EvdID end as EvdID,";
                    cmd += "CASE WHEN EvdName1 is NULL THEN 'N/A' ELSE EvdName1 END AS EvdName1,";
                    cmd += "CASE WHEN EvdName2 is NULL THEN 'N/A' ELSE EvdName2 END AS EvdName2,";
                    cmd += "CASE WHEN EvdName3 is NULL THEN 'N/A' ELSE EvdName3 END AS EvdName3,";
                    cmd += "CASE WHEN EvdPoster is NULL THEN 'N/A' ELSE EvdPoster END AS EvdPoster";
                    cmd += " from EvidenceCheck evd";
                    cmd += " where evd.ticketno = " + Session["ticketID"] + "";
                    cmd += " order by evd.EvdCreationDate desc";

                    //cdata.GetConnection();
                    Admin_Repeater.DataSource = cdata.GetDataSet(cmd).Tables[0];
                    Admin_Repeater.DataBind();
                }
                
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }

        public int PageNumber
        {
            get
            {
                if (ViewState["PageNumber"] != null)
                    return Convert.ToInt32(ViewState["PageNumber"]);
                else return 0;
            }
            set
            {
                ViewState["PageNumber"] = value;
            }
        }

        //public int PageNumberevd
        //{
        //    get
        //    {
        //        if (ViewState["PageNumberevd"] != null)
        //            return Convert.ToInt32(ViewState["PageNumberevd"]);
        //        else return 0;
        //    }
        //    set
        //    {
        //        ViewState["PageNumberevd"] = value;
        //    }
        //}

        protected void Btn_back_Click(object sender, EventArgs e)
        {
            try
            {
                Session["Msg"] = null;
                Response.Redirect("Tickets");
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }

        protected void rptPaging_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            try
            {
                PageNumber = Convert.ToInt32(e.CommandArgument) - 1;
                fillData();
                //Evd_Display();
                //removedel();
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }
        protected void rptPaging_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            try
            {
                var lnkPage = (LinkButton)e.Item.FindControl("btnPage");
                int pn = 1 + PageNumber;
                if (lnkPage.CommandArgument != pn.ToString()) return;
                lnkPage.Enabled = false;
                lnkPage.BackColor = Color.FromName("#d7182a");
                lnkPage.ForeColor = Color.White;
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }

        //////////////////////////////////////////
        /////////////CLOSE COMPLAINT//////////////
        ///////////////PROCESS///////////////////
        protected void btn_closecomm_Click(object sender, EventArgs e)
        {
            try
            {
                //lbl_popupmsg.Text = "";
                if (txt_close_comment.Text != "")
                {
                    if (txt_close_comment.Text.Length <= 1000)
                    {
                        Regex regdetail = new Regex("^[ 0-9a-zA-Z-_()`'.,?&/]*$");
                        bool chk1 = regdetail.IsMatch(txt_close_comment.Text);
                        if (chk1 == false)
                        {
                            //lbl_popupmsg.Text = "Please provide correct details";
                            ScriptManager.RegisterStartupScript(Page, this.GetType(), "ScrollPage", "window.parent.scroll(0,0);", true);
                            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "',''," +
                                "'Please provide correct details')", true);
                            return;
                        }
                        else
                        {
                            string cmd = "";
                            SqlConnection gc = cdata.GetConnection();
                            SqlCommand sqc = new SqlCommand(cmd, gc);
                            cmd = "Update Tickets set status=0, LastUpdated=GETDATE(),Tk_Comment='" + txt_close_comment.Text.Trim() +
                                "' where Ticketno=@tkid";

                            sqc = new SqlCommand(cmd, gc);
                            sqc.Parameters.AddWithValue("@tkid", Session["ticketID"]);
                            sqc.ExecuteNonQuery();
                            cdata.CloseConnection(gc);

                            ////////////EMAIL CODE FOR USER/////////////////
                            ////////////////////////////////////////////////////
                            SmtpClient smtpClient = new SmtpClient("mail.nift.pk", 25);
                            smtpClient.Credentials = new System.Net.NetworkCredential("no-reply@niftepay.pk", "Abc123=+");
                            //smtpClient.UseDefaultCredentials = true; // uncomment if you don't want to use the network credentials
                            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                            smtpClient.EnableSsl = true;
                            MailMessage mail = new MailMessage();
                            //Setting From , To and CC
                            mail.From = new MailAddress("no-reply@niftepay.pk", "NIFT - ePAY");
                            mail.To.Add(new MailAddress(emailname.Text));
                            mail.Subject = "Auto Email Notification - Complaint Closure";
                            mail.IsBodyHtml = true;
                            string mesage = "Dear " + comp_name.Text + ",";
                            mesage += "<br/><br/>Your complaint registered under complaint no. " + Session["ticketID"] +
                                " has been closed by NIFT Complaint Management Unit at " + DateTime.Now + ".";
                            mesage += "<br/><br/>Regards";
                            mesage += "<br/>Complaint Management Unit (NIFT ePay)";

                            mail.Body = mesage;
                            try
                            {
                                smtpClient.Send(mail);
                            }
                            catch
                            {

                            }
                            /////////////////////////////////////////////
                            ////////////EMAIL CODE FOR BANK/////////////////
                            ////////////////////////////////////////////////////
                            smtpClient.Credentials = new System.Net.NetworkCredential("no-reply@niftepay.pk", "Abc123=+");
                            //smtpClient.UseDefaultCredentials = true; // uncomment if you don't want to use the network credentials
                            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                            smtpClient.EnableSsl = true;
                            MailMessage mail2 = new MailMessage();
                            //Setting From , To and CC
                            mail2.From = new MailAddress("no-reply@niftepay.pk", "NIFT - ePAY");
                            mail2.To.Add(new MailAddress(Session["EmailAddress"].ToString()));
                            mail2.Subject = "Auto Email Notification - Complaint Closure";
                            mail2.IsBodyHtml = true;
                            string mesage2 = "<br/><br/>Your complaint registered under complaint no. " + Session["ticketID"] +
                                " has been closed by NIFT Complaint Management Unit at " + DateTime.Now + ".";
                            mesage2 += "<br/><br/>Regards";
                            mesage2 += "<br/>Complaint Management Unit (NIFT ePay)";

                            mail2.Body = mesage2;
                            try
                            {
                                smtpClient.Send(mail2);
                            }
                            catch
                            {

                            }
                            /////////////////////////////////////////////
                            ///
                            //////////////EMAIL CODE FOR NIFT Complaint Team/////////////////
                            //////////////////////////////////////////////////////

                            smtpClient.Credentials = new System.Net.NetworkCredential("no-reply@niftepay.pk", "Abc123=+");
                            //smtpClient.UseDefaultCredentials = true; // uncomment if you don't want to use the network credentials
                            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                            smtpClient.EnableSsl = true;
                            MailMessage mail3 = new MailMessage();

                            //Setting From , To and CC
                            mail3.From = new MailAddress("no-reply@niftepay.pk", "NIFT - ePAY");
                            mail3.To.Add(new MailAddress("complaint@niftepay.pk"));
                            mail3.Subject = "Auto Email Notification - Complaint Closure";

                            string niftmesage = "Dear Team," + System.Environment.NewLine;
                            niftmesage += "A Complaint registered under complaint no. " + Session["ticketID"] +
                                 " has been successfully marked closed by NIFT Complaint Management Unit at " + DateTime.Now + System.Environment.NewLine;
                            niftmesage += "Regards" + System.Environment.NewLine;
                            niftmesage += "Complaint Management Unit (NIFT ePay)";

                            mail2.Body = niftmesage;
                            try
                            {
                                smtpClient.Send(mail3);
                            }
                            catch
                            {

                            }
                            /////////////////////////////////////////
                            Session["Msg"] = 1;
                            Response.Redirect("Tickets");
                        }
                    }
                    else
                    {
                        //lbl_popupmsg.Text = "Please enter 1000 characters only!";
                        ScriptManager.RegisterStartupScript(Page, this.GetType(), "ScrollPage", "window.parent.scroll(0,0);", true);
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "',''," +
                            "'Please enter 1000 characters only!')", true);
                        return;
                    }
                }
                else
                {
                    //TopLabel1.Text = "Please provide required detail";
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "',''," +
                        "'Please provide required detail')", true);
                    return;
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }
        private void hide()
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Popup", "Hide();", true);
        }

        protected void btn_closepopup_Click(object sender, EventArgs e)
        {
            try
            {
                hide();
                txt_close_comment.Text = "";
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }

        //protected void grid_evd_PageIndexChanging(object sender, GridViewPageEventArgs e)
        //{
        //    try
        //    {
        //        grid_evd.PageIndex = e.NewPageIndex;
        //        Evd_Display();
        //    }
        //    catch (Exception ex)
        //    {
        //        LogWriter.WriteToLog(ex);
        //    }
        //}
        protected void Admin_Repeater_ItemDataBound(object source, RepeaterItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    string sSQL = string.Empty;
                    DataTable dt = new DataTable();
                    RepeaterItem item = e.Item;
                    string ev1 = (item.FindControl("EvdName1") as Label).Text;
                    string ev2 = (item.FindControl("EvdName2") as Label).Text;
                    string ev3 = (item.FindControl("EvdName3") as Label).Text;
                    if (ev1 == "" || ev1 == "N/A")
                    {
                        e.Item.FindControl("hid_1").Visible = false;
                    }
                    if (ev2 == "" || ev2 == "N/A")
                    {
                        e.Item.FindControl("hid_2").Visible = false;
                    }
                    if (ev3 == "" || ev3 == "N/A")
                    {
                        e.Item.FindControl("hid_3").Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "',''," +
                    "'Unable to download the file please try again')", true);
                LogWriter.WriteToLog(ex);
            }
        }
        protected void Admin_Repeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            try
            {
                string filename = string.Empty;
                string contentType = string.Empty;
                byte[] MyData = { };
                long fileSize = 0;
                string sSQL = string.Empty;
                cDataAccess oData = new cDataAccess();
                DataTable dt = new DataTable();
                string evdnme = "";
                SqlConnection gc = oData.GetConnection();

                if (e.CommandArgument == null || e.CommandArgument.ToString() == "")
                {
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "',''," +
                        "'File not available')", true);
                    return;
                }

                try
                {
                    if (e.CommandName == "down1")
                    {
                        sSQL = "Select EvdID from Evidencecheck where EvdID=" + e.CommandArgument;
                        if (oData.GetDataSet(sSQL).Tables[0].Rows.Count > 0)
                        {
                            sSQL = "";
                            sSQL = "select TicketNo,InstID,BranchID,EvdID,EvdName1,EvdType1,EvdData1" +
                                " from Evidencecheck where EvdID = " + e.CommandArgument;
                            DataSet DS = oData.GetDataSet(sSQL);

                            foreach (DataRow oDR in DS.Tables[0].Rows)
                            {
                                evdnme = (string)oDR["EvdName1"];
                                if (evdnme != "" && evdnme != null)
                                {
                                    MyData = (byte[])oDR["EvdData1"];
                                    fileSize = (long)MyData.Length;
                                    contentType = oDR["EvdType1"].ToString();
                                    filename = oDR["EvdName1"].ToString();

                                    Down_file(contentType, filename, fileSize, MyData);
                                }
                            }
                        }
                    }
                    if (e.CommandName == "del1")
                    {
                        sSQL = "Update Evidencecheck set EvdName1= null,EvdType1 =null,EvdData1=null,EvdSize1 = null" +
                            " where EvdID = " + e.CommandArgument;
                        oData.RunProc(sSQL);
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "',''," +
                            "'File Deleted Successfully')", true);
                    }

                    //////////////////////////
                    if (e.CommandName == "down2")
                    {
                        sSQL = "Select EvdID from Evidencecheck where EvdID=" + e.CommandArgument;
                        if (oData.GetDataSet(sSQL).Tables[0].Rows.Count > 0)
                        {
                            sSQL = "";
                            sSQL = "select TicketNo,InstID,BranchID,EvdID,EvdName2,EvdType2,EvdData2" +
                                " from Evidencecheck where EvdID = " + e.CommandArgument;
                            DataSet DS = oData.GetDataSet(sSQL);

                            foreach (DataRow oDR in DS.Tables[0].Rows)
                            {
                                evdnme = (string)oDR["EvdName2"];
                                if (evdnme != "" && evdnme != null)
                                {
                                    MyData = (byte[])oDR["EvdData2"];
                                    fileSize = (long)MyData.Length;
                                    contentType = oDR["EvdType2"].ToString();
                                    filename = oDR["EvdName2"].ToString();

                                    Down_file(contentType, filename, fileSize, MyData);
                                }
                            }
                        }
                    }
                    if (e.CommandName == "del2")
                    {
                        sSQL = "Update Evidencecheck set EvdName2= null,EvdType2 =null,EvdData2=null,EvdSize2 = null" +
                            " where EvdID = " + e.CommandArgument;
                        oData.RunProc(sSQL);
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "',''," +
                            "'File Deleted Successfully')", true);
                    }

                    //////////////////////////
                    if (e.CommandName == "down3")
                    {
                        sSQL = "Select EvdID from Evidencecheck where EvdID=" + e.CommandArgument;
                        if (oData.GetDataSet(sSQL).Tables[0].Rows.Count > 0)
                        {
                            sSQL = "";
                            sSQL = "select TicketNo,InstID,BranchID,EvdID,EvdName3,EvdType3,EvdData3" +
                                " from Evidencecheck where EvdID = " + e.CommandArgument;
                            DataSet DS = oData.GetDataSet(sSQL);

                            foreach (DataRow oDR in DS.Tables[0].Rows)
                            {
                                evdnme = (string)oDR["EvdName3"];
                                if (evdnme != "" && evdnme != null)
                                {
                                    MyData = (byte[])oDR["EvdData3"];
                                    fileSize = (long)MyData.Length;
                                    contentType = oDR["EvdType3"].ToString();
                                    filename = oDR["EvdName3"].ToString();

                                    Down_file(contentType, filename, fileSize, MyData);
                                }
                            }
                        }
                    }
                    if (e.CommandName == "del3")
                    {
                        sSQL = "Update Evidencecheck set EvdName3= null,EvdType3 =null,EvdData3=null,EvdSize3 = null" +
                            " where EvdID = " + e.CommandArgument;
                        oData.RunProc(sSQL);
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "',''," +
                            "'File Deleted Successfully')", true);
                    }
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "',''," +
                        "'Unable to delete the file pleae try again')", true);
                    LogWriter.WriteToLog(ex);
                }

                Evd_Display();
                fillData();
                fill_admin_Data();
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }
        protected void grid_evd_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            
        }
        /////////DOWNLOAD FILE FOR ADMIN/////////
        ///
        //protected void grid_evd_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        string filename = string.Empty;
        //        string contentType = string.Empty;
        //        byte[] MyData = { };
        //        long fileSize = 0;
        //        string evdid = grid_evd.SelectedRow.Cells[0].Text;
        //        string evdnme = grid_evd.SelectedRow.Cells[1].Text;
        //        SqlConnection gc = cdata.GetConnection();

        //        try
        //        {
        //            string sSQL = string.Empty;
        //            cDataAccess oData = new cDataAccess();
        //            DataTable dt = new DataTable();
        //            sSQL = "select TicketNo,InstID,BranchID,EvdID,EvdName,EvdType,EvdData from Evidence where EvdID = " + evdid;
        //            using (DataSet DS = oData.GetDataSet(sSQL))
        //            {
        //                foreach (DataRow oDR in DS.Tables[0].Rows)
        //                {
        //                    MyData = (byte[])oDR["EvdData"];
        //                    fileSize = (long)MyData.Length;
        //                    contentType = oDR["EvdType"].ToString();
        //                    filename = oDR["EvdName"].ToString();
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            //lblMsg.Text = "";
        //            LogWriter.WriteToLog(ex);
        //        }
        //        //lblMsg.Text = "";
        //        if (evdnme != string.Empty)
        //        {
        //            Down_file(contentType, filename, fileSize, MyData);
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        LogWriter.WriteToLog(ex);
        //    }
        //}
        //protected void grid_evd_RowDeleting(object sender, GridViewDeleteEventArgs e)
        //{
        //    try
        //    {
        //        string confirmValue = Request.Form["confirm_value"];

        //        if (confirmValue == "YES")
        //        {
        //            Object obj;
        //            GridViewRow del = (GridViewRow)grid_evd.Rows[e.RowIndex];
        //            string evdid = grid_evd.Rows[e.RowIndex].Cells[0].Text;
        //            String sSQL;
        //            cDataAccess odata = new cDataAccess();

        //            sSQL = "delete from Evidence where EvdID=" + evdid;

        //            obj = odata.RunProc(sSQL);

        //            GetData();
        //            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','',
        //            'Evidence deleted successfully')", true);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogWriter.WriteToLog(ex);
        //    }
        //}



        //protected void evdpgrpt_ItemCommand(object source, RepeaterCommandEventArgs e)
        //{
        //    try
        //    {
        //        PageNumberevd = Convert.ToInt32(e.CommandArgument) - 1;
        //        Evd_Display();
        //        fillData();
        //    }
        //    catch (Exception ex)
        //    {
        //        LogWriter.WriteToLog(ex);
        //    }
        //}

        //protected void evdpgrpt_ItemDataBound(object sender, RepeaterItemEventArgs e)
        //{
        //    try
        //    {
        //        var lnkPage = (LinkButton)e.Item.FindControl("btnPageevd");
        //        int pn = 1 + PageNumberevd;
        //        if (lnkPage.CommandArgument != pn.ToString()) return;
        //        lnkPage.Enabled = false;
        //        lnkPage.BackColor = Color.FromName("#d7182a");
        //        lnkPage.ForeColor = Color.White;
        //    }
        //    catch (Exception ex)
        //    {
        //        LogWriter.WriteToLog(ex);
        //    }
        //}

        //protected void btn_Upload_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (Session["stu"].ToString() == "CLOSED")
        //        {
        //            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "',''," +
        //                "'You cannot update! Ticket is closed!')", true);
        //            return;
        //        }

        //        ///////ONLY 3 Files are allowed//////////////
        //        ////////////////////////////////////////////
        //        //if(FileUpload1.PostedFiles.Count >3)
        //        //{
        //        //    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','',
        //        //    'Only 3 Files Allowed')", true);
        //        //    return;
        //        //}
        //        //foreach (HttpPostedFile postedFile in FileUpload1.PostedFiles)
        //        //{
        //        //    if (IsAllowedUpload(postedFile) == 0)
        //        //    {
        //        //        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','',
        //        //        'Invalid File: '"+postedFile.FileName+")", true);
        //        //        return;
        //        //    }
        //        //}

        //        //    if (FileUpload2.HasFile)
        //        //{
        //        //    if (IsAllowedUpload(FileUpload2.PostedFile) == 0)
        //        //    {
        //        //        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','',
        //        //        'Invalid File')", true);
        //        //        return;
        //        //    }
        //        //}
        //        //if (FileUpload3.HasFile)
        //        //{
        //        //    if (IsAllowedUpload(FileUpload3.PostedFile) == 0)
        //        //    {
        //        //        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','',
        //        //        'Invalid File')", true);
        //        //        return;
        //        //    }
        //        //}
        //        //if (FileUpload4.HasFile)
        //        //{
        //        //    if (IsAllowedUpload(FileUpload4.PostedFile) == 0)
        //        //    {
        //        //        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','',
        //        //        'Invalid File')", true);
        //        //        return;
        //        //    }
        //        //}
        //        //Evd_Display();
        //        fillData();

        //        SmtpClient smtpClient = new SmtpClient("mail.nift.pk", 25);
        //        smtpClient.Credentials = new System.Net.NetworkCredential("no-reply@niftepay.pk", "Abc123=+");
        //        //smtpClient.UseDefaultCredentials = true; // uncomment if you don't want to use the network credentials
        //        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
        //        smtpClient.EnableSsl = true;
        //        if (Session["InstID"].ToString() != "999")
        //        {
        //            ////////////EMAIL CODE FOR NIFT OP/////////////////////
        //            //////////////////////////////////////////////////////
        //            MailMessage mail = new MailMessage();
        //            //Setting From , To and CC
        //            mail.From = new MailAddress("no-reply@niftepay.pk", "NIFT - ePAY");
        //            mail.To.Add(new MailAddress("support@niftepay.pk"));
        //            mail.Subject = "Auto Email Notification - Complaint Management";
        //            mail.IsBodyHtml = true;
        //            string message = "Dear Operations Team,";
        //            message += "<br/><br/>Complaint/Query " + Session["ticketID"] + " has been updated by " + txtName.Text.Trim() +
        //                " at " + DateTime.Now + " against. " + work_code.Text + ".";
        //            message += "<br/><br/>Regards";
        //            message += "<br/>Complaint Management Unit (NIFT ePay)";
        //            mail.Body = message;
        //            try
        //            {
        //                smtpClient.Send(mail);
        //            }
        //            catch
        //            {

        //            }
        //            //////////////////////////////////////////////////////                    
        //        }

        //        ////////////EMAIL CODE FOR NIFT USER/////////////////////
        //        ////////////////////////////////////////////////////////
        //        MailMessage mail1 = new MailMessage();
        //        //Setting From , To and CC
        //        mail1.From = new MailAddress("no-reply@niftepay.pk", "NIFT - ePAY");
        //        mail1.To.Add(new MailAddress(emailname.Text));
        //        mail1.Subject = "Auto Email Notification - Complaint Management";
        //        mail1.IsBodyHtml = true;
        //        string mesage = "Dear " + comp_name.Text + ",";
        //        mesage += "<br/><br/>Your Complaint/Query " + Session["ticketID"] + " has been updated by " + txtName.Text.Trim() +
        //            " at " + DateTime.Now + " against. " + work_code.Text + ".";
        //        mesage += "<br/><br/>Regards";
        //        mesage += "<br/>Complaint Management Unit (NIFT ePay)";
        //        mail1.Body = mesage;
        //        try
        //        {
        //            smtpClient.Send(mail1);
        //        }
        //        catch
        //        {

        //        }
        //        ///////////////////////////////////////////////////////
        //        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "',''," +
        //            "'File updated successfully')", true);
        //    }
        //    catch (Exception ex)
        //    {
        //        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "',''," +
        //            "'Unable to upload the file please try again!')", true);
        //        LogWriter.WriteToLog(ex);
        //    }
        //}
        protected void Repeater1_ItemDataBound(object source, RepeaterItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    string sSQL = string.Empty;
                    DataTable dt = new DataTable();
                    RepeaterItem item = e.Item;
                    string ev1 = (item.FindControl("EvdName1") as Label).Text;
                    string ev2 = (item.FindControl("EvdName2") as Label).Text;
                    string ev3 = (item.FindControl("EvdName3") as Label).Text;
                    if (ev1 == "" || ev1 == "N/A")
                    {
                        e.Item.FindControl("hid_1").Visible = false;
                    }
                    if (ev2 == "" || ev2 == "N/A")
                    {
                        e.Item.FindControl("hid_2").Visible = false;
                    }
                    if (ev3 == "" || ev3 == "N/A")
                    {
                        e.Item.FindControl("hid_3").Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "',''," +
                    "'Unable to download the file please try again')", true);
                LogWriter.WriteToLog(ex);
            }
        }

        protected void Repeater1_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            try
            {
                string filename = string.Empty;
                string contentType = string.Empty;
                byte[] MyData = { };
                long fileSize = 0;
                string sSQL = string.Empty;
                cDataAccess oData = new cDataAccess();
                DataTable dt = new DataTable();
                string evdnme = "";
                SqlConnection gc = oData.GetConnection();

                if (e.CommandArgument == null || e.CommandArgument.ToString() == "")
                {
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "',''," +
                        "'File not available')", true);
                    return;
                }

                try
                {
                    if (e.CommandName == "download1")
                    {
                        sSQL = "Select EvdID from Evidencecheck where EvdID=" + e.CommandArgument;
                        if (oData.GetDataSet(sSQL).Tables[0].Rows.Count > 0)
                        {
                            sSQL = "";
                            sSQL = "select TicketNo,InstID,BranchID,EvdID,EvdName1,EvdType1,EvdData1" +
                                " from Evidencecheck where EvdID = " + e.CommandArgument;
                            DataSet DS = oData.GetDataSet(sSQL);

                            foreach (DataRow oDR in DS.Tables[0].Rows)
                            {
                                evdnme = (string)oDR["EvdName1"];
                                if (evdnme != "" && evdnme != null)
                                {
                                    MyData = (byte[])oDR["EvdData1"];
                                    fileSize = (long)MyData.Length;
                                    contentType = oDR["EvdType1"].ToString();
                                    filename = oDR["EvdName1"].ToString();

                                    Down_file(contentType, filename, fileSize, MyData);
                                }
                            }
                        }
                    }

                    //////////////////////////
                    if (e.CommandName == "download2")
                    {
                        sSQL = "Select EvdID from Evidencecheck where EvdID=" + e.CommandArgument;
                        if (oData.GetDataSet(sSQL).Tables[0].Rows.Count > 0)
                        {
                            sSQL = "";
                            sSQL = "select TicketNo,InstID,BranchID,EvdID,EvdName2,EvdType2,EvdData2" +
                                " from Evidencecheck where EvdID = " + e.CommandArgument;
                            DataSet DS = oData.GetDataSet(sSQL);

                            foreach (DataRow oDR in DS.Tables[0].Rows)
                            {
                                evdnme = (string)oDR["EvdName2"];
                                if (evdnme != "" && evdnme != null)
                                {
                                    MyData = (byte[])oDR["EvdData2"];
                                    fileSize = (long)MyData.Length;
                                    contentType = oDR["EvdType2"].ToString();
                                    filename = oDR["EvdName2"].ToString();

                                    Down_file(contentType, filename, fileSize, MyData);
                                }
                            }
                        }
                    }

                    //////////////////////////
                    if (e.CommandName == "download3")
                    {
                        sSQL = "Select EvdID from Evidencecheck where EvdID=" + e.CommandArgument;
                        if (oData.GetDataSet(sSQL).Tables[0].Rows.Count > 0)
                        {
                            sSQL = "";
                            sSQL = "select TicketNo,InstID,BranchID,EvdID,EvdName3,EvdType3,EvdData3" +
                                " from Evidencecheck where EvdID = " + e.CommandArgument;
                            DataSet DS = oData.GetDataSet(sSQL);

                            foreach (DataRow oDR in DS.Tables[0].Rows)
                            {
                                evdnme = (string)oDR["EvdName3"];
                                if (evdnme != "" && evdnme != null)
                                {
                                    MyData = (byte[])oDR["EvdData3"];
                                    fileSize = (long)MyData.Length;
                                    contentType = oDR["EvdType3"].ToString();
                                    filename = oDR["EvdName3"].ToString();

                                    Down_file(contentType, filename, fileSize, MyData);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "',''," +
                        "'Unable to download the file please')", true);
                    LogWriter.WriteToLog(ex);
                }



                ////////FOR DELETING FILE////////
                ///
                //if (e.CommandName == "delete")
                //{
                //    if (Session["InstID"].ToString() != "999")
                //    {
                //       ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','',
                //       'You don't have permission to delete evidence')", true);
                //        return;
                //    }

                //    string confirmValue = Request.Form["confirm_value"];

                //    if (confirmValue == "YES")
                //    {
                //        try
                //        {
                //            String sSQL;
                //            cDataAccess odata = new cDataAccess();
                //            sSQL = "Select EvdID from Evidence where EvdID=" + e.CommandArgument;
                //            if (odata.GetDataSet(sSQL).Tables[0].Rows.Count > 0)
                //            {
                //                sSQL = "";
                //                sSQL = "delete from Evidence where EvdID=" + e.CommandArgument;
                //                odata.RunProc(sSQL);

                //                GetData();
                //                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info +
                //                "','','Evidence deleted successfully')", true);
                //            }
                //            else
                //            {
                //                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info +
                //                "','','No Evidence Attached')", true);
                //            }
                //        }
                //        catch (Exception)
                //        {
                //            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info +
                //            "','','No Evidence Attached')", true);
                //        }
                //    }
                //}

                Evd_Display();
                fillData();
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }

        protected void Down_file(string contentType, string filename, long fileSize, byte[] MyData)
        {
            Response.Expires = 0;
            Response.Buffer = true;
            Response.Clear();
            Response.ClearContent();
            Response.Charset = string.Empty;
            Response.ClearHeaders();
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.AddHeader("Cache-Control", "no-cache, must-revalidate, post-check=0, pre-check=0");
            Response.AddHeader("Pragma", "no-cache");
            //Response.AddHeader("Content-Description", "File Download");
            //Response.AddHeader("Content-Type", "application/force-download");
            //Response.AddHeader("Content-Transfer-Encoding", "binary\n");
            Response.ContentType = contentType;
            Response.Cache.SetCacheability(System.Web.HttpCacheability.Public);
            Response.AddHeader("Content-Disposition", "attachment; filename=" + filename);
            Response.AddHeader("Content-Length", fileSize.ToString());
            Response.BinaryWrite(MyData);
            Response.OutputStream.Flush();
            Response.OutputStream.Close();
            Response.End();
        }

        //protected void evdRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        //{
        //    try
        //    {
        //        if (e.CommandArgument != "")
        //        {
        //            if (e.CommandName == "download")
        //            {
        //                string filename = string.Empty;
        //                string contentType = string.Empty;
        //                byte[] MyData = { };
        //                long fileSize = 0;
        //                //string evdid = grid_evd.SelectedRow.Cells[0].Text;
        //                string evdnme = "";
        //                SqlConnection gc = cdata.GetConnection();

        //                try
        //                {
        //                    string sSQL = string.Empty;
        //                    cDataAccess oData = new cDataAccess();
        //                    DataTable dt = new DataTable();
        //                    sSQL = "select TicketNo,InstID,BranchID,EvdID,EvdName,EvdType,EvdData from Evidence where EvdID = "
        //                    + e.CommandArgument;
        //                    using (DataSet DS = oData.GetDataSet(sSQL))
        //                    {
        //                        foreach (DataRow oDR in DS.Tables[0].Rows)
        //                        {
        //                            evdnme = (string)oDR["EvdName"];
        //                            MyData = (byte[])oDR["EvdData"];
        //                            fileSize = (long)MyData.Length;
        //                            contentType = oDR["EvdType"].ToString();
        //                            filename = oDR["EvdName"].ToString();
        //                        }
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    //lblMsg.Text = "";
        //                    LogWriter.WriteToLog(ex);
        //                }
        //                try
        //                {
        //                    //lblMsg.Text = "";
        //                    if (evdnme != string.Empty)
        //                    {
        //                        Response.Expires = 0;
        //                        Response.Buffer = true;
        //                        Response.Clear();
        //                        Response.ClearContent();
        //                        Response.Charset = string.Empty;
        //                        Response.ClearHeaders();
        //                        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //                        Response.AddHeader("Cache-Control", "no-cache, must-revalidate, post-check=0, pre-check=0");
        //                        Response.AddHeader("Pragma", "no-cache");
        //                        //Response.AddHeader("Content-Description", "File Download");
        //                        //Response.AddHeader("Content-Type", "application/force-download");
        //                        //Response.AddHeader("Content-Transfer-Encoding", "binary\n");
        //                        Response.ContentType = contentType;
        //                        Response.Cache.SetCacheability(System.Web.HttpCacheability.Public);
        //                        Response.AddHeader("Content-Disposition", "attachment; filename=" + filename);
        //                        Response.AddHeader("Content-Length", fileSize.ToString());
        //                        Response.BinaryWrite(MyData);
        //                        Response.OutputStream.Flush();
        //                        Response.OutputStream.Close();
        //                        Response.End();
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info +
        //                    "','','File not downloaded')", true);
        //                    LogWriter.WriteToLog(ex);
        //                }
        //            }

        //            ////////FOR DELETING FILE////////
        //            ///
        //            if (e.CommandName == "delete")
        //            {
        //                string confirmValue = Request.Form["confirm_value"];

        //                if (confirmValue == "YES")
        //                {
        //                    try
        //                    {
        //                        String sSQL;
        //                        cDataAccess odata = new cDataAccess();

        //                        sSQL = "delete from Evidence where EvdID=" + e.CommandArgument;
        //                        odata.RunProc(sSQL);

        //                        GetData();
        //                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info +
        //                        "','','Evidence deleted successfully')", true);
        //                    }
        //                    catch (Exception)
        //                    {
        //                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info +
        //                        "','','Evidence deleted successfully')", true);
        //                    }
        //                }
        //            }
        //        }
        //        Evd_Display();
        //        fillData();
        //    }
        //    catch (Exception ex)
        //    {
        //        LogWriter.WriteToLog(ex);
        //    }
        //}

        /////////CHECK ALLOWED EVIDENCE DOCUMENTS/////////////
        protected int IsAllowedUpload(HttpPostedFile fu, int commentid)
        {
            int AllowUpload = 0;
            try
            {
                String strFileName = fu.FileName;
                if (!(fu == null))
                {
                    try
                    {
                        //Session["P2tttxxH"] = Server.MapPath("~/UploadImg/" + fu.FileName);
                        fu.SaveAs(Server.MapPath("~/UploadImg/" + fu.FileName));
                        //LogWriter.WriteToLog(Server.MapPath("~/UploadImg/" + fu.FileName));

                        //fu.SaveAs(@"D:\\Publish\\UploadImg\\" + fu.FileName);
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
                            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "',''," +
                                "'File size exceeds limit')", true);
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

                        /////GET HEADER INFORMATION OF UPLOADED FILE
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
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "',''," +
                            "'Invalid file')", true);
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
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "',''," +
                    "'Invalid file')", true);
                return AllowUpload;
            }
        }



        protected void btn_up_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["stu"].ToString() == "CLOSED")
                {
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert",
                        "infomsg('" + InfoType.info + "','','Ticket is closed!')", true);
                    return;
                }

                SqlConnection gc = cdata.GetConnection();
                //if (FileUpload1.HasFile)
                //{
                //    hpf = FileUpload1.PostedFile;
                //    int uploadID = Upload(Convert.ToInt32(Session["ticketID"]), FileUpload1.PostedFile);
                //    if (uploadID == 1)
                //    {
                //        Sendmail = 1;
                //        Evd_Display();
                //    }
                //    else
                //    {
                //        //lbl_Message.Text = "Invalid file!";
                //        ScriptManager.RegisterStartupScript(Page, this.GetType(), "ScrollPage", "window.parent.scroll(0,0);", true);
                //        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','',
                //        'Invalid file')", true);
                //        return;
                //    }
                //}
                if (changed == 1)
                {
                    string cmd = "";
                    //string m = Session["MerchantID"].ToString();

                    if (ddl_assignee.SelectedItem.Value == Bank_ID)
                    {
                        cmd = "Update Tickets set AssigneeID=@txtassigned, LastUpdated=GETDATE(), status=2 " +
                            "where Ticketno=" + Session["ticketID"];
                    }
                    else if (ddl_assignee.SelectedItem.Text.Substring(0, 4) == "NIFT")
                    {
                        cmd = "Update Tickets set AssigneeID=@txtassigned, LastUpdated=GETDATE(), status=1, BranchID= " + ddl_sub_assignee.SelectedItem.Value +
                            " where Ticketno=" + Session["ticketID"];
                    }
                    else
                    {
                        cmd = "Update Tickets set AssigneeID=@txtassigned , LastUpdated=GETDATE(), status=2 " +
                            "where Ticketno=" + Session["ticketID"];
                    }
                    SqlCommand sqc = new SqlCommand(cmd, gc);
                    sqc.Parameters.AddWithValue("@txtassigned", txt_Assigned);
                    sqc.ExecuteNonQuery();
                    cdata.CloseConnection(gc);
                    Sendmail = 1;
                }
                else
                {
                    //TopLabel1.Text = "Ticket Updated Successfully";
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "',''," +
                        "'Ticket Updated Successfully')", true);
                    Session["Msg"] = 1;
                    //Response.Redirect("Tickets");
                    Sendmail = 0;
                }
                if (Sendmail == 1)
                {
                    //string UserEmail = "", MerchEmail = "";
                    SmtpClient smtpClient = new SmtpClient("mail.nift.pk", 25);
                    smtpClient.Credentials = new System.Net.NetworkCredential("no-reply@niftepay.pk", "Abc123=+");
                    //smtpClient.UseDefaultCredentials = true; // uncomment if you don't want to use the network credentials
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.EnableSsl = true;

                    
                    

                    if (MerchEmail != "")
                    {
                        ////////////EMAIL CODE FOR MERCHANT/////////////////////
                        ////////////////////////////////////////////////////
                        MailMessage useremail = new MailMessage();
                        //Setting From , To and CC
                        useremail.From = new MailAddress("no-reply@niftepay.pk", "NIFT - ePAY");
                        useremail.To.Add(new MailAddress(MerchEmail));
                        useremail.Subject = "Auto Email Notification - Complaint Management";
                        useremail.IsBodyHtml = true;
                        string mesage2 = "Dear " + Session["MerchFullname"].ToString() + ",";
                        mesage2 += "<br/><br/>A Complaint/Query has been assigned to you under complaint no. " + Session["ticketID"] + "" +
                            " at " + comp_date.Text + " against " + work_code.Text + ".";
                        mesage2 += "<br/>Please respond to the complaint within 5 working days.";
                        mesage2 += "<br/><br/>Regards";
                        mesage2 += "<br/>Complaint Management Unit (NIFT ePay)";

                        useremail.Body = mesage2;
                        try
                        {
                            smtpClient.Send(useremail);
                        }
                        catch
                        {

                        }
                        //    ///////////////////////////////////////////////////////
                        //}
                    }
                    else
                    {
                        /////////////////EMAIL CODE FOR NIFT OP USER/////////////////
                        //////////////////////////////////////////////////////

                        smtpClient.Credentials = new System.Net.NetworkCredential("no-reply@niftepay.pk", "Abc123=+");
                        //smtpClient.UseDefaultCredentials = true; // uncomment if you don't want to use the network credentials
                        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtpClient.EnableSsl = true;
                        MailMessage mail3 = new MailMessage();

                        //Setting From , To and CC
                        mail3.From = new MailAddress("no-reply@niftepay.pk", "NIFT - ePAY");
                        mail3.To.Add(new MailAddress(Session["Op_email"].ToString()));
                        mail3.Subject = "Auto Email Notification - Complaint Creation";

                        string niftmesage2 = "Dear NIFT Operations," + System.Environment.NewLine;
                        niftmesage2 += "A Complaint has been assignd to you under complaint no. " + Session["ticketID"] + " from " +
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
                    }
                    //TopLabel1.Text = "Ticket Updated Successfully";
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert",
                        "infomsg('" + InfoType.info + "','','Ticket Updated Successfully')", true);
                    Session["Msg"] = 1;
                    //Response.Redirect("Tickets");
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert",
                    "infomsg('" + InfoType.info + "','','Ticket Updated Successfully')", true);
                LogWriter.WriteToLog(ex);
            }
        }

        protected void ddl_assignee_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                changed = 1;
                txt_Assigned = ddl_assignee.SelectedItem.Value;
                MerchEmail = GetMerchEmail(txt_Assigned);

                if (ddl_assignee.SelectedItem.Value == "999")
                {
                    if (Session["InstID"].ToString() == "999")
                    {
                        if (Session["BranchID"].ToString() == "9999")
                        {
                            ddl_sub_assignee.Visible = true;
                            ddl_sub_assignee.Enabled = true;
                            ddl_sub_assignee.Attributes.Add("size", "5");

                            string cmd = " select top 5 NIFT_assignee_ID,assignee_name from nift_assignee where IsActive=1 ";
                            SqlConnection gc = cdata.GetConnection();
                            SqlDataAdapter adpt = new SqlDataAdapter(cmd, gc);
                            DataTable dt = new DataTable();
                            adpt.Fill(dt);
                            ddl_sub_assignee.DataSource = dt;
                            ddl_sub_assignee.DataBind();
                            ddl_sub_assignee.DataTextField = "assignee_name";
                            ddl_sub_assignee.DataValueField = "NIFT_assignee_ID";
                            ddl_sub_assignee.DataBind();
                            cdata.CloseConnection(gc);

                            //ddl_sub_assignee.Items.Add("ALL");
                            //ddl_sub_assignee.Items.Add(new ListItem("Taimoor shahzad", "99901"));
                            //ddl_sub_assignee.Items.Add(new ListItem("Ahsan Ali", "99902"));
                            //ddl_sub_assignee.Items.Add(new ListItem("Ikram", "99903"));
                            //ddl_sub_assignee.Items.Add(new ListItem("EY backoffice User", "99904"));
                            //ddl_sub_assignee.Items.Add(new ListItem("Sheraz", "99905"));
                        }
                    }
                }
                else
                {
                    ddl_sub_assignee.Items.Clear();
                    ddl_sub_assignee.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }

        //protected void Repeater1_ItemDataBound(object sender, RepeaterItemEventArgs e)
        //{
        //    if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        //    {
        //        GridView gvwProducts = (GridView)e.Item.FindControl("comm_evd");
        //        cDataAccess cdata = new cDataAccess();
        //        string cmd = "select EvdID,EvdName from Evidence where CommentID=" + commid;
        //        DataTable dt = cdata.GetDataSet(cmd).Tables[0];
        //        gvwProducts.DataSource = dt;
        //        gvwProducts.DataBind();
        //    }
        //}

        /////////DISPLAY EVIDENCE DOCS///////////
        public void Evd_Display()
        {
            cDataAccess cdata = new cDataAccess();
            string cmd = "";
            if (Session["InstID"].ToString() == "999")
            {
                cmd = "select evd.EvdID,evd.EvdPoster,evd.EvdName1,evd.EvdName2,evd.EvdName3 from EvidenceCheck evd, " +
                    "Comments cmt where evd.TicketNo=" + Session["ticketID"] + " and evd.CommentID=cmt.CommentID " +
                    "order by EvdCreationDate desc ";
            }
            else if (Session["MerchantID"] == null)
            {
                cmd = "select evd.EvdID,evd.EvdPoster,evd.EvdName1,evd.EvdName2,evd.EvdName3 from EvidenceCheck evd , Comments cmt " +
                    "where evd.TicketNo=" + Session["ticketID"] +
                    " and evd.InstID in (999," + Session["InstID"] + ") and evd.CommentID=cmt.CommentID " +
                    "order by EvdCreationDate desc ";
            }
            else
            {
                cmd = "select evd.EvdID,evd.EvdPoster,evd.EvdName1,evd.EvdName2,evd.EvdName3 from EvidenceCheck evd, Comments cmt" +
                    " where evd.TicketNo=" + Session["ticketID"] + " and evd.CommentID=cmt.CommentID " +
                    "order by EvdCreationDate desc ";
            }

            //PagedDataSource pds = new PagedDataSource();
            //DataView dv = new DataView(cdata.GetDataSet(cmd).Tables[0]);
            ////foreach(DataRow dr in cdata.GetDataSet(cmd).Tables[0].Rows)
            ////{ commid = Convert.ToInt32(dr["CommentID"]); }

            //pds.DataSource = dv;
            //pds.AllowPaging = true;
            //pds.PageSize = 5;
            //pds.CurrentPageIndex = PageNumberevd;
            //if (pds.PageCount > 1)
            //{
            //    evdpgrpt.Visible = true;
            //    ArrayList arraylist = new ArrayList();
            //    for (int i = 0; i < pds.PageCount; i++)
            //        arraylist.Add((i + 1).ToString());
            //    evdpgrpt.DataSource = arraylist;
            //    evdpgrpt.DataBind();
            //}
            //else
            //{
            //    evdpgrpt.Visible = false;
            //}
            //evdRepeater.DataSource = pds;
            //evdRepeater.DataBind();

            ////grid_evd.DataSource = cdata.GetDataSet(cmd);
            ////grid_evd.DataBind();
        }
        public void Display_Assignee()
        {
            string cmd = "select MerchID,FirstName +' '+LastName as Fullname from merchant where IsActive=1";
            SqlConnection gc = cdata.GetConnection();
            SqlDataAdapter adpt = new SqlDataAdapter(cmd, gc);
            DataTable dt = new DataTable();
            adpt.Fill(dt);
            ddl_assignee.DataSource = dt;
            ddl_assignee.DataBind();
            ddl_assignee.DataTextField = "Fullname";
            ddl_assignee.DataValueField = "MerchID";
            ddl_assignee.DataBind();

            ddl_merchants.DataSource = dt;
            ddl_merchants.DataBind();
            ddl_merchants.DataTextField = "Fullname";
            ddl_merchants.DataValueField = "MerchID";
            ddl_merchants.DataBind();

            cdata.CloseConnection(gc);
        }
        public string GetAssigneeName(string val, string instid)
        {
            cDataAccess cdata = new cDataAccess();
            string cmd = "";
            if (instid != null)
            {
                cmd = "select InstName from Institution where InstID = " + instid;

                if (cdata.GetDataSet(cmd).Tables[0].Rows.Count > 0)
                {
                    Bank_ID = instid;
                    Bank_Name = cdata.GetDataSet(cmd).Tables[0].Rows[0][0].ToString();
                }
            }
            else
            {
                Response.Redirect("Sessionexpire");
            }

            if (val != null)
            {
                if (val == "999")
                {
                    txt_Assigned = "NIFT";
                    return txt_Assigned;
                }
                else
                {
                    cmd = "select FirstName +' ' + LastName as Fullname from Merchant where MerchID = " + val;
                    if (cdata.GetDataSet(cmd).Tables[0].Rows.Count > 0)
                    {
                        txt_Assigned = cdata.GetDataSet(cmd).Tables[0].Rows[0][0].ToString();
                    }
                    else
                    {
                        cmd = "select InstName from Institution where InstID = " + val;
                        if (cdata.GetDataSet(cmd).Tables[0].Rows.Count > 0)
                        {
                            txt_Assigned = cdata.GetDataSet(cmd).Tables[0].Rows[0][0].ToString();
                        }
                    }
                    return txt_Assigned;
                }
            }
            else
            {
                Response.Redirect("Sessionexpire");
            }
            return "";
        }
        public string GetMerchEmail(string val)
        {
            string merchemail = "";
            string cmd = "select EmailAddress ,mc.FirstName+' '+mc.LastName as fullname from users us " +
                "join Merchant mc on mc.UserID = us.UserID where mc.MerchID =" + val;
            cDataAccess cdata = new cDataAccess();
            if (cdata.GetDataSet(cmd).Tables[0].Rows.Count > 0)
            {
                merchemail = cdata.GetDataSet(cmd).Tables[0].Rows[0][0].ToString();
                Session["MerchFullname"] = cdata.GetDataSet(cmd).Tables[0].Rows[0][1].ToString();
                return merchemail;
            }
            else
            {
                return "";
            }
        }
        //private void removedel()
        //{   
        //    /////////FOR NIFT/////////
        //    if (Session["InstID"].ToString() == "999")
        //    {
        //        foreach (RepeaterItem a in Repeater1.Items)
        //        {
        //            if (a.ItemIndex >= 0)
        //            {
        //                LinkButton lkb = a.FindControl("lbdlt") as LinkButton;
        //                lkb.Visible = true;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        foreach (RepeaterItem a in Repeater1.Items)
        //        {
        //            if (a.ItemIndex >= 0)
        //            {
        //                LinkButton lkb = a.FindControl("lbdlt") as LinkButton;
        //                lkb.Visible = false;
        //            }
        //        }
        //    }
        //}
    }
}