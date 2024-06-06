using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace NIFT_CMS
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            RegisterRoutes(RouteTable.Routes);
            PreSendRequestHeaders += Application_PreSendRequestHeaders;
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
        static void RegisterRoutes(RouteCollection routes)
        {
            //routes.MapPageRoute("myfriend1", "csharpcorner-friend-list", "~/FriendList.aspx");
            //Route Name   : myfriend1 
            //Route URL    : csharpcorner-friend-list
            //Physical File: FriendList.aspx

            routes.MapPageRoute("Home", "Home", "~/index.aspx");

            routes.MapPageRoute("Loginpage", "Loginpage", "~/cms_login.aspx");
            routes.MapPageRoute("ForgotUser", "ForgotUser", "~/ForgotUser.aspx");
            routes.MapPageRoute("ForgotPassword", "ForgotPassword", "~/mobile.aspx");
            routes.MapPageRoute("ChangePassword", "ChangePassword", "~/NewPassword.aspx");

            routes.MapPageRoute("OpDashboard", "OpDashboard", "~/Admin/OpTilePage.aspx");

            routes.MapPageRoute("Tickets", "Tickets", "~/ticket_form.aspx");
            routes.MapPageRoute("ViewComplaint", "ViewComplaint", "~/ViewComplaint.aspx");
            routes.MapPageRoute("NewComplaint", "NewComplaint", "~/Complaint_form.aspx");
            routes.MapPageRoute("Closed", "Closed", "~/ClosedComplaints.aspx");
            routes.MapPageRoute("Inprogress", "Inprogress", "~/InprogressComplaints.aspx");
            routes.MapPageRoute("Pending", "Pending", "~/PendingComplaints.aspx");
            routes.MapPageRoute("Reports", "Reports", "~/Reports.aspx");
            routes.MapPageRoute("Sessionexpire", "Sessionexpire", "~/sessionexpire.aspx");
            routes.MapPageRoute("Notallowed", "Notallowed", "~/Notallowed.aspx");

            routes.MapPageRoute("Adminmain", "Adminmain", "~/Admin/AdminTiles.aspx");
            routes.MapPageRoute("Createmerchant", "Createmerchant", "~/Admin/Create_merchant.aspx");
            routes.MapPageRoute("Createuser", "Createuser", "~/Admin/Create_user.aspx");
            routes.MapPageRoute("EditRole", "EditRole", "~/Admin/EditRole.aspx");
            routes.MapPageRoute("ManageBank", "ManageBank", "~/Admin/ManageBank.aspx");
            routes.MapPageRoute("ManageBranch", "ManageBranch", "~/Admin/ManageBranch.aspx");
            routes.MapPageRoute("ManageMerchant", "ManageMerchant", "~/Admin/ManageMerchant.aspx");
            routes.MapPageRoute("ManageRoles", "ManageRoles", "~/Admin/ManageRoles.aspx");
            routes.MapPageRoute("ManageUser", "ManageUser", "~/Admin/ManageUser.aspx");
            routes.MapPageRoute("ManageWorkCode", "ManageWorkCode", "~/Admin/UpdateWorkCode.aspx");
        }
        protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {
            HttpContext.Current.Response.Headers.Remove("Server");
            HttpContext.Current.Response.Headers.Remove("X-AspNetWebPages-Version");
            HttpContext.Current.Response.Headers.Remove("X-AspNet-Version");
            HttpContext.Current.Response.Headers.Remove("X-Powered-By");
            HttpContext.Current.Response.Headers.Remove("X-AspNetMvc-Version");
        }
    }
}