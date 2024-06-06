using IBCS.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NIFT_CMS
{
    public partial class ManageBranch : System.Web.UI.Page
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

                        isAllowed = general.isPageAllowed(sUserId, "ManageBranch");

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

        protected void BindSQL()
        {
            try
            {
                String sSQL;
                DataTable dtBranch = new DataTable();
                cDataAccess odata = new cDataAccess();


                //code for dropdownlist box

                sSQL = "select * from institution order by instname asc";

                DataTable dtBank = new DataTable();

                SqlDataAdapter adp_bank = odata.GetDataAdapter(sSQL);

                adp_bank.Fill(dtBank);

                DDL1.DataSource = dtBank;
                DDL1.DataTextField = dtBank.Columns["InstName"].ColumnName.ToString();
                DDL1.DataValueField = dtBank.Columns["InstID"].ColumnName.ToString();

                DDL1.DataBind();

                DDL1.Items.Insert(0, "All");

                if (Session["DDL_Selected_Value"] != null)
                {
                    DDL1.SelectedValue = (String)Session["DDL_Selected_Value"];
                }
                //else
                //{
                //    DDL1.SelectedValue = "0";
                //}

                sSQL = " select STUFF( '000', 3 - LEN( a.[InstID] ) + 1, LEN( a.[InstID] ), a.[InstID]) InstID ";
                sSQL += " ,STUFF( '0000', 4 - LEN( [BranchID] ) + 1, LEN( [BranchID] ), [BranchID]) [BranchID] ";
                sSQL += " ,instname";
                sSQL += " ,[Branch_name] ";
                sSQL += " ,CityID,case isopen when 1 then 'Open' when 0 then 'Closed' end status ";
                sSQL += " from branch a inner join institution b on a.InstID=b.InstID";

                if (Session["AppendSQL"] != null)
                {
                    sSQL = sSQL + Session["AppendSQL"];
                }

                sSQL = sSQL + " order by instid,BranchID asc";


                Session["sSQL"] = sSQL;

                SqlDataAdapter adp = odata.GetDataAdapter(sSQL);

                adp.Fill(dtBranch);


                Int64 NoPages = (Int64)dtBranch.Rows.Count / 50;

                if (Convert.ToInt32(Session["pageindex"]) > NoPages)
                {

                    Session["pageindex"] = NoPages - 1;
                }



                if (Convert.ToInt32(dg_branch.PageCount) < Convert.ToInt32(Session["pageindex"]))
                {
                    Session["pageindex"] = dg_branch.PageCount;
                }
                dg_branch.DataSource = dtBranch;
                dg_branch.PageIndex = Convert.ToInt32(Session["pageindex"]);
                dg_branch.DataBind();
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }



        protected void dg_branch_ItemDataBound(Object sender, DataGridItemEventArgs e) //Handles dg_users.ItemDataBound
        {
            try
            {
                TableRow tbl_InstID;
                Label lbl_InstID;
                Label lbl_BranchID;
                Label lbl_PreviousCode;
                Label lbl_Branch_Name;
                Label lbl_Branch_Address;
                Label lbl_CityID;


                switch (e.Item.ItemType)
                {
                    case (ListItemType.Item):
                        {

                            lbl_InstID = (Label)e.Item.FindControl("InstID");
                            lbl_BranchID = (Label)e.Item.FindControl("BranchID");
                            lbl_PreviousCode = (Label)e.Item.FindControl("PreviousCode");
                            lbl_Branch_Name = (Label)e.Item.FindControl("Branch_Name");
                            lbl_Branch_Address = (Label)e.Item.FindControl("Branch_Address");
                            lbl_CityID = (Label)e.Item.FindControl("CityID");

                            string strArguments = "'" + lbl_InstID.Text + "', ";
                            strArguments += "'" + lbl_BranchID.Text + "','" + lbl_CityID.Text + "'";

                            tbl_InstID = (TableRow)e.Item.FindControl("InstID").Parent.Parent;
                            tbl_InstID.Attributes.Add("OnDblClick", "this.bgColor='lightgrey'; ShowDetails(" + strArguments + ");");
                            tbl_InstID.Attributes.Add("onMouseover", "this.bgColor='lightgrey';this.style.cursor='hand'");
                            tbl_InstID.Attributes.Add("onMouseout", "this.bgColor='#FFFFFF';this.style.cursor='default'");



                            if (e.Item.ItemType == ListItemType.Item)
                            {
                                if (e.Item.ItemType == ListItemType.SelectedItem)
                                {
                                    e.Item.Cells[0].BackColor = Color.Yellow;
                                    e.Item.Cells[1].BackColor = Color.Yellow;
                                    e.Item.Cells[2].BackColor = Color.Yellow;
                                    e.Item.Cells[3].BackColor = Color.Yellow;
                                    e.Item.Cells[4].BackColor = Color.Yellow;
                                    e.Item.Cells[5].BackColor = Color.Yellow;
                                }
                            }


                            break;


                        }

                    case (ListItemType.AlternatingItem):
                        {

                            lbl_InstID = (Label)e.Item.FindControl("InstID");
                            lbl_BranchID = (Label)e.Item.FindControl("BranchID");
                            lbl_PreviousCode = (Label)e.Item.FindControl("PreviousCode");
                            lbl_Branch_Name = (Label)e.Item.FindControl("Branch_Name");
                            lbl_Branch_Address = (Label)e.Item.FindControl("Branch_Address");
                            lbl_CityID = (Label)e.Item.FindControl("CityID");

                            string strArguments = "'" + lbl_InstID.Text + "', ";
                            strArguments += "'" + lbl_BranchID.Text + "','" + lbl_CityID.Text + "'";

                            tbl_InstID = (TableRow)e.Item.FindControl("InstID").Parent.Parent;
                            tbl_InstID.Attributes.Add("OnDblClick", "this.bgColor='lightgrey'; ShowDetails(" + strArguments + ");");
                            tbl_InstID.Attributes.Add("onMouseover", "this.bgColor='lightgrey';this.style.cursor='hand'");
                            tbl_InstID.Attributes.Add("onMouseout", "this.bgColor='#FFFFFF';this.style.cursor='default'");

                            if (e.Item.ItemType == ListItemType.AlternatingItem)
                            {
                                if (e.Item.ItemType == ListItemType.SelectedItem)
                                {
                                    e.Item.Cells[0].BackColor = Color.Yellow;
                                    e.Item.Cells[1].BackColor = Color.Yellow;
                                    e.Item.Cells[2].BackColor = Color.Yellow;
                                    e.Item.Cells[3].BackColor = Color.Yellow;
                                    e.Item.Cells[4].BackColor = Color.Yellow;
                                    e.Item.Cells[5].BackColor = Color.Yellow;
                                }
                            }


                            break;


                        }
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }


        protected void dg_branch_PageIndexChanged(Object sender, DataGridPageChangedEventArgs e)
        {
            try
            {
                String sSQL;
                DataTable dtBranch = new DataTable();
                cDataAccess odata = new cDataAccess();

                sSQL = (string)Session["sSQL"];

                SqlDataAdapter adp = odata.GetDataAdapter(sSQL);

                adp.Fill(dtBranch);

                if (dg_branch.PageCount < Convert.ToInt16(Session["pageindex"]))
                {
                    Session["pageindex"] = dg_branch.PageCount;
                }

                dg_branch.PageIndex = Convert.ToInt32(e.NewPageIndex);
                Session["pageindex"] = e.NewPageIndex;
                dg_branch.DataSource = dtBranch;
                dg_branch.DataBind();
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }

        protected void DDL1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string AppendSQL = " and b.InstID='" + DDL1.SelectedValue + "'";

                if ((String)DDL1.SelectedValue == "All")
                {
                    AppendSQL = "";
                }

                Session["DDL_Selected_Value"] = DDL1.SelectedValue;
                Session["AppendSQL"] = AppendSQL;
                Session["pageindex"] = 0;

                BindSQL();
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
                String sSQL = "";
                sSQL = Session["sSQL"].ToString();

                if (string.IsNullOrEmpty(sSQL) != true)
                {

                    String DtTm = "";
                    DtTm = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString();
                    DtTm = DtTm.Replace(" ", "").Replace("/", "").Replace(":", "");

                    Response.Clear();

                    Response.AddHeader("content-disposition", "attachment;filename=BranchList_" + DtTm + ".xls");

                    Response.Charset = "";

                    //Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.AddHeader("Cache-Control", "max-age=0");

                    Response.ContentType = "application/vnd.xls";

                    Response.ContentType = "application/vnd.ms-excel";


                    DataGrid dg = new DataGrid();
                    DataTable dtBranch = new DataTable();
                    cDataAccess odata = new cDataAccess();

                    SqlDataAdapter adp = odata.GetDataAdapter(sSQL);

                    adp.Fill(dtBranch);
                    //set the datagrid datasource to the dataset passed in
                    dg.DataSource = dtBranch;
                    //bind the datagrid
                    dg.DataBind();

                    System.IO.StringWriter stringWrite = new System.IO.StringWriter();

                    System.Web.UI.HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);

                    dg.RenderControl(htmlWrite);

                    Response.Write(stringWrite.ToString());

                    Response.Flush();

                    dg.Dispose();

                    dtBranch.Dispose();

                    Response.End();
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }

        protected void dg_branch_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                String sSQL;
                DataTable dtBranch = new DataTable();
                cDataAccess odata = new cDataAccess();

                sSQL = (string)Session["sSQL"];

                SqlDataAdapter adp = odata.GetDataAdapter(sSQL);

                adp.Fill(dtBranch);

                if (dg_branch.PageCount < Convert.ToInt16(Session["pageindex"]))
                {
                    Session["pageindex"] = dg_branch.PageCount;
                }

                dg_branch.PageIndex = Convert.ToInt32(e.NewPageIndex);
                Session["pageindex"] = e.NewPageIndex;
                dg_branch.DataSource = dtBranch;
                dg_branch.DataBind();
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }

        protected void BindSQL(String BankID, String BranchID, String CityID)
        {
            try
            {
                String sSQL, sCityID = "";
                DataTable dtBranch = new DataTable();
                cDataAccess odata = new cDataAccess();

                //code for dropdownlist box
                sSQL = "select TRIM(STUFF( '000', 3 - LEN( a.[InstID] ) + 1, LEN( a.[InstID] ), a.[InstID])) InstID,instname from institution a order by instid,instname asc";

                DataTable dtBank = new DataTable();

                SqlDataAdapter adp_bank = odata.GetDataAdapter(sSQL);

                adp_bank.Fill(dtBank);

                DropDownList1.DataSource = dtBank;
                DropDownList1.DataTextField = dtBank.Columns["InstID"].ToString();
                DropDownList1.DataValueField = dtBank.Columns["InstID"].ToString();
                DropDownList1.DataBind();

                if (BankID.Trim() != "")
                {
                    DropDownList1.SelectedValue = BankID.ToString().Trim();
                }
                foreach (DataRow DR in dtBank.Rows)
                {
                    if (DropDownList1.SelectedValue == DR["instid"].ToString().Trim())
                    {
                        txtBankName.Text = DR["instname"].ToString().Trim();
                        txtBranchCode.Text = "";
                        txtBranchName.Text = "";
                    }
                }

                if ((BankID.Trim() != "") && (BranchID.Trim() != ""))
                {
                    sSQL = " select STUFF( '000', 3 - LEN( a.[InstID] ) + 1, LEN( a.[InstID] ), a.[InstID]) InstID ";
                    sSQL += " ,STUFF( '0000', 4 - LEN( [BranchID] ) + 1, LEN( [BranchID] ), [BranchID]) [BranchID] ";
                    sSQL += " ,instname";
                    sSQL += " ,[Branch_name],CityID,isOpen ";
                    //sSQL += " ,[branch_address],[Branch_Zone] ";
                    //sSQL += " ,case status when 1 then 'Open' when 0 then 'Closed' end bStatus ";
                    sSQL += "  from branch a inner join institution b on a.InstID=b.InstID";
                    sSQL += " where a.instid= " + BankID.Trim() + " and BranchID = " + BranchID.Trim();
                    sSQL += " and CityID=" + CityID.Trim();


                    if (Session["AppendSQL"] != null)
                    {
                        sSQL = sSQL + Session["AppendSQL"];
                    }

                    sSQL = sSQL + " order by instid,instname,a.branch_name asc";


                    Session["sSQL"] = sSQL;

                    SqlDataAdapter adp = odata.GetDataAdapter(sSQL);

                    adp.Fill(dtBranch);

                    foreach (DataRow DR in dtBranch.Rows)
                    {
                        txtBankName.Text = DR["instname"].ToString();
                        txtBranchCode.Text = DR["BranchID"].ToString();
                        txtBranchName.Text = DR["Branch_name"].ToString();
                        if (DR["isOpen"].ToString().ToLower() == "false")


                            sCityID = DR["CityID"].ToString();

                    }

                }

                //code for dropdownlist box for city
                sSQL = "select cityid, cityname from city order by cityname asc";

                DataTable dtCity = new DataTable();

                SqlDataAdapter adp_City = odata.GetDataAdapter(sSQL);

                adp_City.Fill(dtCity);

                DDL2.DataSource = dtCity;
                DDL2.DataTextField = dtCity.Columns["CityName"].ToString();
                DDL2.DataValueField = dtCity.Columns["CityID"].ToString();
                DDL2.DataBind();

                if (sCityID.Trim() != "")
                {
                    DDL2.SelectedValue = sCityID.ToString().Trim();
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }


        private bool InsertBranch(String BankCode, String BranchCode, String BranchName, String CityID, String isOpen)
        {
            try
            {
                cDataAccess oData = new cDataAccess();
                object obj;
                bool retval = false;

                string sSQL = "";

                if (isBranchExist(BankCode, BranchCode, CityID) == false)
                {
                    sSQL = "insert into Branch(instid,Branchid,Branch_name,cityid,isOpen)values(" + BankCode.Trim();
                    sSQL += ",'" + BranchCode.Trim() + "','" + BranchName.Trim() + "','" + CityID.Trim() + "'," + isOpen.Trim() + ")";
                    obj = oData.RunProc(sSQL);

                    if (Convert.ToString(obj) == "1")
                    {
                        retval = true;
                    }
                }
                else
                {
                    if (SCclose.Checked)
                    {
                        sSQL = "update Branch set branch_name='" + BranchName.Trim() + "', Cityid=" + CityID.Trim() + ", isOpen=" + isOpen.Trim();
                        sSQL += " where instid=" + BankCode.Trim() + " and branchid=" + BranchCode.Trim() + " and CityID=" + CityID.Trim();
                        obj = oData.RunProc(sSQL);
                        if (Convert.ToString(obj) == "1")
                        {
                            retval = true;
                        }
                    }
                    else if (SCopen.Checked)
                    {
                        sSQL = "update Branch set branch_name='" + BranchName.Trim() + "', Cityid=" + CityID.Trim() + ", isOpen=" + isOpen.Trim();
                        sSQL += " where instid=" + BankCode.Trim() + " and branchid=" + BranchCode.Trim() + " and CityID=" + CityID.Trim();
                        obj = oData.RunProc(sSQL);
                        if (Convert.ToString(obj) == "1")
                        {
                            retval = true;
                        }

                        string cmd = "select Subcitycode from SatelliteCity where Maincitycode =" + CityID.Trim();
                        foreach (DataRow dr in oData.GetDataSet(cmd).Tables[0].Rows)
                        {
                            sSQL = "update Branch set branch_name='" + BranchName.Trim() + "', Cityid=" + dr["Subcitycode"].ToString() + ", isOpen=" + isOpen.Trim();
                            sSQL += " where instid=" + BankCode.Trim() + " and branchid=" + BranchCode.Trim() + " and CityID = " + dr["Subcitycode"].ToString();

                            obj = oData.RunProc(sSQL);
                            if (Convert.ToString(obj) == "1")
                            {
                                retval = true;
                            }
                        }
                    }
                }
                return retval;

            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return false;
            }

        }

        private bool isBranchExist(String BankCode, String BranchCode, String CityID)
        {
            try
            {

                String sSQL;
                sSQL = " Select * From branch ";
                sSQL += " where instid=" + BankCode.Trim() + " and branchid=" + BranchCode.Trim() + " and cityid=" + CityID.Trim();

                cDataAccess oData = new cDataAccess();

                DataSet DS = oData.GetDataSet(sSQL);

                if (DS.Tables[0].Rows.Count > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
                return false;
            }
        }


        protected void Button2_Click(object sender, EventArgs e)
        {
            try
            {
                txtBranchCode.Text = "";
                txtBranchName.Text = "";
                DDL2.SelectedItem.Text = "";
                lblmsg.Text = "";
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }
        protected void Submit_Click(object sender, EventArgs e)
        {
            try
            {
                if (DDL2.SelectedItem.Text == "Please Select")
                {
                    lblmsg.Text = "Please Select City!";
                    return;
                }
                String BranchCode = "";
                String BCode = DropDownList1.SelectedItem.Text;
                String BranchName = "";
                String CityID = DDL2.SelectedValue;
                String isOpen = "0";

                Regex regex = new Regex("^[0-9]*[0-9][0-9][1-9]*$");
                bool chk1 = regex.IsMatch(txtBranchCode.Text);
                if (chk1 == true)
                {
                    BranchCode = txtBranchCode.Text;
                }
                else
                {
                    lblmsg.Text = "Please Insert Valid Branch Code";
                    return;
                }

                Regex regex2 = new Regex("^[!@#$%^&*+=?~]*$");
                bool chk2 = regex.IsMatch(txtBranchName.Text);
                if (chk2 == false)
                {
                    BranchName = general.SQLEncode(txtBranchName.Text);
                }
                else
                {
                    lblmsg.Text = "Please Insert Valid Branch Name";
                    return;
                }

                if (bOpen.Checked == true)
                    isOpen = "1";

                if ((BranchCode.Trim() != "") && (BranchName.Trim() != ""))
                {
                    bool isInserted = false;

                    isInserted = InsertBranch(BCode, BranchCode, BranchName, CityID, isOpen);

                    if (isInserted == true)
                    {
                        lblmsg.Text = "Record Inserted Successfully";
                        txtBranchCode.Text = "";
                        txtBranchName.Text = "";
                        DDL2.SelectedItem.Text = "Please Select";
                    }
                    if (isInserted == true)
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
                    if (bOpen.Checked)
                    {
                        bOpen.BorderColor = ColorTranslator.FromHtml("#408000");
                        bOpen.ForeColor = ColorTranslator.FromHtml("#408000");
                        lblchkbx.InnerHtml = "OPEN";
                    }
                    else
                    {
                        bOpen.ForeColor = ColorTranslator.FromHtml("#d7182a");
                        bOpen.BorderColor = ColorTranslator.FromHtml("#d7182a");
                        lblchkbx.InnerHtml = "CLOSED";
                    }
                }
                else
                {
                    lblmsg.Text = "Branch Code and Branch Name can not be empty";
                }
            }
            catch (Exception ex)
            {
                string sMsg = "Record Not Saved";
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','" + sMsg + "')", true);
                LogWriter.WriteToLog(ex);
            }
        }
        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //if (txtBranchCode.Text.Trim() == "")
                //{
                String BankID = DropDownList1.SelectedValue;
                String BranchID = "";
                String CityID = "";
                BindSQL(BankID, BranchID, CityID);
                //}
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }

        protected void dg_branch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                GridViewRow row = dg_branch.SelectedRow;
                string sBankCode = dg_branch.DataKeys[row.RowIndex].Values[0].ToString();
                string sBankName = dg_branch.DataKeys[row.RowIndex].Values[1].ToString();
                string sBranchCode = dg_branch.DataKeys[row.RowIndex].Values[2].ToString();
                string sBranchName = dg_branch.DataKeys[row.RowIndex].Values[3].ToString();
                string city = dg_branch.DataKeys[row.RowIndex].Values[4].ToString();
                DDL2.SelectedValue = city;
                string open = dg_branch.DataKeys[row.RowIndex].Values[5].ToString();
                //DataTable dt = new DataTable();
                //dt = (DataTable)Session["AdminBankTable"];
                //dt.DefaultView.RowFilter = String.Format("InstID = '{0}'", sBankCode);


                if (sBankCode != string.Empty)
                {
                    DropDownList1.SelectedItem.Text = sBankCode;
                }
                if (sBankName != string.Empty)
                {
                    txtBankName.Text = sBankName;
                }
                if (sBranchCode != string.Empty)
                {
                    txtBranchCode.Text = sBranchCode;
                }
                if (sBranchName != string.Empty)
                {
                    txtBranchName.Text = sBranchName;
                }
                if (city != string.Empty)
                {
                    String sSQL;
                    cDataAccess odata = new cDataAccess();
                    sSQL = "select cityname from city where CityID=" + city;

                    DataTable dtCity = new DataTable();

                    SqlDataAdapter adp_City = odata.GetDataAdapter(sSQL);

                    adp_City.Fill(dtCity);
                    foreach (DataRow DR in dtCity.Rows)
                    {
                        DDL2.SelectedItem.Text = DR["cityname"].ToString();
                    }
                }
                if (open != string.Empty)
                {
                    if (open == "Open")
                    {
                        bOpen.Checked = true;
                        lblchkbx.InnerHtml = "<span style='color:green'>OPEN</span>";

                    }
                    else
                    {
                        bOpen.Checked = false;
                        lblchkbx.InnerHtml = "<span style='color:red'>CLOSED</span>";
                    }
                }

            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }
    }
}