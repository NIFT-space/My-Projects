using IBCS.Data;
using System;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Web;
using System.Web.UI;

namespace NIFT_CMS
{
    public partial class NewPassword : System.Web.UI.Page
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

                        isAllowed = general.isPageAllowed(sUserId, "ChangePassword");

                        if (isAllowed == false)
                        {
                            Response.Clear();
                            Response.Redirect("Notallowed", true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }
        }

        protected void Btn_Change_Click(object sender, EventArgs e)
        {
            try
            {
                Int64 t_ = Convert.ToInt64(Session["passCount"]);
                if (t_ < 5)
                {
                    if (password0.Text.Length <= 15 && password1.Text.Length <= 15)
                    {
                        /////////////////CHECK KEYWORDS of TEXTBOXES//////////////////////
                    }
                    else
                    {
                        //alert_.Text = "Please provide correct details";
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','Please provide correct details')", true);
                        return;
                    }
                    //////////////////////////////////////////////////////////////////

                    if (password0.Text.Trim() == password1.Text.Trim())
                    {
                        if (Session["UserID"].ToString().Trim() != "")
                        {
                            string password = password1.Text.Trim();
                            if (password != Session["Old_Pass"].ToString())
                            {
                                EncDec enc = new EncDec();
                                string hashpass = enc.GetHash(password);
                                //string enc_pass = enc.Encrypt(password);
                                cDataAccess cdata = new cDataAccess();
                                string cmd = "update users set Password = @enc, pass_change=1 Where UserID =@UID";
                                SqlCommand sqc = new SqlCommand(cmd, cdata.GetConnection());
                                sqc.Parameters.AddWithValue("@enc", hashpass);
                                sqc.Parameters.AddWithValue("@UID", Session["UserID"].ToString().Trim());
                                sqc.ExecuteNonQuery();

                                //////////////////EMAIL FOR PASSWORD CHANGE///////////////////
                                ///
                                SmtpClient smtpClient = new SmtpClient("mail.nift.pk", 25);
                                string UserEmail = Session["EmailAddress"].ToString();

                                smtpClient.Credentials = new System.Net.NetworkCredential("no-reply@niftepay.pk", "Abc123=+");
                                //smtpClient.UseDefaultCredentials = true; // uncomment if you don't want to use the network credentials
                                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                                smtpClient.EnableSsl = true;
                                MailMessage mail = new MailMessage();

                                //Setting From , To and CC
                                mail.From = new MailAddress("no-reply@niftepay.pk", "NIFT - ePAY");
                                mail.To.Add(new MailAddress(UserEmail));
                                mail.Subject = "Auto Email Notification - Complaint Management";

                                string mesage = "Dear " + Session["Fullname"].ToString().ToUpper() + "," + System.Environment.NewLine;
                                mesage += "Your NIFT-ePay portal account password is changed. Kindly login with the updated password on the portal." + System.Environment.NewLine;
                                mesage += "Regards" + System.Environment.NewLine;
                                mesage += "Team NIFT ePay";

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
                                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','New Password is updated')", true);
                                Response.Redirect("Tickets");
                                //////////////////////////////////////////////////////////////
                            }
                            else
                            {
                                //alert_.Text = "New Password Cannot Be Same As Old Password!";
                                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','New Password Cannot Be Same As Old Password')", true);
                            }
                        }
                    }
                    else
                    {
                        //alert_.Text = "Both Passwords Did Not Matched!";
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','Both Passwords Did Not Matched')", true);
                    }
                    Session["passCount"] = Convert.ToInt64(Session["passCount"]) + 1;
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert",
                         "infomsg('" + InfoType.info + "','','Password Recovery Limit Reached. Please try again later!')", true);
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToLog(ex);
            }

        }
    }
}