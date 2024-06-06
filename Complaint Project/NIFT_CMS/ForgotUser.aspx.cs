using IBCS.Data;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;

namespace NIFT_CMS
{
    public partial class ForgotUser : System.Web.UI.Page
    {
        public static string Orig_Pass;
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

                lblmsg.Text = "";
                signin.HRef = "Loginpage";

            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }

        protected void btn_Proceed_Click(object sender, EventArgs e)
        {
            try
            {
                Int64 t_ = Convert.ToInt64(Session["userCount"]);
                if (t_ < 5)
                {
                    if (mobile_.Text.Length <= 30)
                    {
                        /////////////////CHECK KEYWORDS of TEXTBOXES//////////////////////
                        Regex regdetail = new Regex("^[A-Za-z0-9'`_()@.]*$");
                        bool chk1 = regdetail.IsMatch(mobile_.Text);
                        if (chk1 == false)
                        {
                            //lblmsg.Text = "Please provide correct details";
                            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','Please Provide Correct Details')", true);
                            return;
                        }
                    }
                    else
                    {
                        //lblmsg.Text = "Please provide correct details";
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','Please Provide Correct Details')", true);
                        return;
                    }
                    String cmd = "";
                    String exists = "";
                    String Usern = "";
                    String fullname = "";
                    cDataAccess cdata = new cDataAccess();

                    if (mobile_.Text != "")
                    {
                        cmd = "select EmailAddress,UserName,FirstName + ' ' + LastName as Fullname from users where EmailAddress=@Unum";
                        SqlCommand sqc = new SqlCommand(cmd, cdata.GetConnection());
                        sqc.Parameters.AddWithValue("@Unum", mobile_.Text.Trim());
                        DataSet sDS = new DataSet();
                        SqlDataAdapter dadEorder = new SqlDataAdapter(sqc);
                        sDS = new DataSet();
                        dadEorder.Fill(sDS);
                        foreach (DataRow dr in sDS.Tables[0].Rows)
                        {
                            exists = Convert.ToString(dr["EmailAddress"]);
                            Usern = Convert.ToString(dr["UserName"]);
                            fullname = Convert.ToString(dr["Fullname"]);
                        }
                        if (exists != null && exists != "")
                        {
                            //////////EMAIL CODE FOR USERNAME/////////////////////
                            //////////////////////////////////////////////////
                            SmtpClient smtpClient = new SmtpClient("mail.nift.pk", 25);

                            smtpClient.Credentials = new System.Net.NetworkCredential("no-reply@niftepay.pk", "Abc123=+");
                            //smtpClient.UseDefaultCredentials = true; // uncomment if you don't want to use the network credentials
                            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                            smtpClient.EnableSsl = true;
                            MailMessage mail = new MailMessage();

                            //Setting From , To and CC
                            mail.From = new MailAddress("no-reply@niftepay.pk", "NIFT - ePAY");
                            mail.To.Add(new MailAddress(exists));
                            mail.Subject = "Auto Email Notification - Complaint Management";

                            string mesage = "Dear " + fullname + "," + System.Environment.NewLine;
                            mesage += "Your NIFT-ePay USER recovery is successful.";
                            mesage += "your current Username is : " + Usern + System.Environment.NewLine + System.Environment.NewLine + System.Environment.NewLine;
                            mesage += "Regards" + System.Environment.NewLine;
                            mesage += "Team NIFT ePay";

                            mail.Body = mesage;
                            smtpClient.Send(mail);

                            ////////////////////////////////////////////////////////////////
                            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','Email Sent For User Recovery')", true);
                            //lblmsg.Text = "Email Sent For User Recovery";
                        }
                        else
                        {
                            //lblmsg.Text = "EmailAddress Does Not Exist";
                            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','EmailAddress Does Not Exist')", true);
                        }
                    }
                    else
                    {
                        //lblmsg.Text = "Please Provide Required Details";
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','Please Provide Required Details')", true);
                    }
                    Session["userCount"] = Convert.ToInt64(Session["userCount"]) + 1;
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert",
                         "infomsg('" + InfoType.info + "','','User Recovery Limit Reached. Please try again later!')", true);
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }
    }
}