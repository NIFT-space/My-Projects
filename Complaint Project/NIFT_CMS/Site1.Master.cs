using IBCS.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NIFT_CMS
{
    public partial class Site1 : System.Web.UI.MasterPage
    {
        public static string Fullname, InstName;
        protected void Page_Load(object sender, EventArgs e)
        {
            //link1.HRef = "Complaint_form.aspx";
            //link2.HRef = "InprogressComplaints.aspx";
            //link3.HRef = "PendingComplaints.aspx";
            //link4.HRef = "ClosedComplaints.aspx";
            //link5.HRef = "Reports.aspx";
            ///////////////DISPLAY USERNAME//////////////
            //string cmd;
            //lbl_user.Text = GetFullName();
        }
        //protected void sign_out_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (InsertUserLog() == true)
        //        {
        //            Response.Redirect("cms_login.aspx");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogWriter.WriteToLog(ex);
        //    }
        //}
        //public bool InsertUserLog()
        //{
        //    try
        //    {
        //        cDataAccess cdata = new cDataAccess();
        //        int saved_UserLogID = 0;
        //        string cmd = "Select UserLogID from UserLog where UserID= " + Session["UserID"].ToString() + " and Dtn_Code= '" + Session["dtn"].ToString() + "'";
        //        foreach (DataRow DR in cdata.GetDataSet(cmd).Tables[0].Rows)
        //        {
        //            saved_UserLogID = Convert.ToInt32(DR["UserLogID"]);
        //        }
        //        cmd = "Update UserLog set TimeOut = '" + DateTime.Now + "'";
        //        cmd += " Where UserLogID = " + saved_UserLogID;
        //        int i = Convert.ToInt32(cdata.RunProc(cmd));
        //        if (i == 1)
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogWriter.WriteToLog(ex);
        //        return false;
        //    }
        //}
        //public string GetFullName()
        //{
        //    try
        //    {
        //        string cmd;
        //        cDataAccess cdata = new cDataAccess();
        //        cmd = "select Firstname + ' ' + Lastname as FullName, inst.InstName as InstName from users join institution inst on inst.InstID = users.InstID";
        //        cmd += " Where Userid = " + Session["UserID"].ToString().Trim();
        //        foreach (DataRow DR in cdata.GetDataSet(cmd).Tables[0].Rows)
        //        {
        //            Fullname = Convert.ToString(DR["FullName"]);
        //            InstName = Convert.ToString(DR["InstName"]);
        //        }
        //        return Fullname + " - " + InstName;
        //    }
        //    catch (Exception ex)
        //    {
        //        LogWriter.WriteToLog(ex);
        //        return "";
        //    }
        //}
    }
}