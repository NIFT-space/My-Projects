using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using IBCS.Data;

public partial class sessionexpire : System.Web.UI.Page
{
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
        }
        catch (Exception ex)
        {
            LogWriter.WriteToLog(ex);
        }
        try
        {
            Session.Abandon();
            Session.RemoveAll();

        }
        catch (Exception ex)
        {
            LogWriter.WriteToLog(ex);
        }


    }
}
