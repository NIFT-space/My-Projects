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
    public partial class mobile : System.Web.UI.Page
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

                //if (Session["UserID"] == null)
                //{
                //    Response.Clear();
                //    Response.Redirect("sessionexpire.aspx", true);
                //}

                //bool isAllowed = false;
                //string sUserId = Convert.ToString(Session["UserID"]).Trim();

                //isAllowed = general.isPageAllowed(sUserId, "mobile.aspx");

                //if (isAllowed == false)
                //{
                //    Response.Clear();
                //    Response.Redirect("notallowed.aspx", true);
                //}

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
                Int64 t_ = Convert.ToInt64(Session["mobCount"]);
                if (t_ < 5)
                {
                    if (mobile_.Text.Length <= 20)
                    {
                        /////////////////CHECK KEYWORDS of TEXTBOXES//////////////////////
                        Regex regdetail = new Regex("^[0-9a-zA-Z._&]*$");
                        bool chk1 = regdetail.IsMatch(mobile_.Text);
                        if (chk1 == false)
                        {
                            lblmsg.Text = "Please provide correct details";
                            return;
                        }
                    }
                    else
                    {
                        lblmsg.Text = "Please provide correct details";
                        return;
                    }
                    string cmd = "";
                    string exists = "";
                    string Emailadd = "";
                    string fullname = "";
                    cDataAccess cdata = new cDataAccess();

                    /////////////CODE FOR CHECK MOBILE////////////////
                    ////////////////////////////////////////////////
                    ///
                    //if (mobile_.Text != "")
                    //{
                    //    cmd = "Select contactnumber from users where contactnumber =@Cnum";
                    //    SqlCommand sqc = new SqlCommand(cmd, cdata.GetConnection());
                    //    sqc.Parameters.AddWithValue("@Cnum", mobile_.Text.Trim());
                    //    DataSet sDS = new DataSet();
                    //    SqlDataAdapter dadEorder = new SqlDataAdapter(sqc);
                    //    sDS = new DataSet();
                    //    dadEorder.Fill(sDS);
                    //    foreach (DataRow dr in sDS.Tables[0].Rows)
                    //    {
                    //        exists = Convert.ToString(dr["contactnumber"]);
                    //    }
                    //    if(exists != null && exists != "")
                    //    {
                    //        string hashpassword = CreateRandomPassword();
                    //        cmd = "Update users set Password =@Pass";
                    //        cmd += " Where contactnumber =@Cnum";
                    //    sqc = new SqlCommand(cmd, cdata.GetConnection());
                    //    sqc.Parameters.AddWithValue("@Pass", hashpassword);
                    //    sqc.Parameters.AddWithValue("@Cnum", mobile_.Text.Trim());
                    //    sqc.ExecuteNonQuery();
                    //////////////////////////////////////////////////////////////////////////////////////////

                    if (mobile_.Text != "")
                    {
                        cmd = "Select UserName,EmailAddress,FirstName + ' ' + LastName as Fullname from users where UserName =@Unum";
                        SqlCommand sqc = new SqlCommand(cmd, cdata.GetConnection());
                        sqc.Parameters.AddWithValue("@Unum", mobile_.Text.Trim());
                        DataSet sDS = new DataSet();
                        SqlDataAdapter dadEorder = new SqlDataAdapter(sqc);
                        sDS = new DataSet();
                        dadEorder.Fill(sDS);
                        foreach (DataRow dr in sDS.Tables[0].Rows)
                        {
                            exists = Convert.ToString(dr["UserName"]);
                            Emailadd = Convert.ToString(dr["EmailAddress"]);
                            fullname = Convert.ToString(dr["Fullname"]);
                        }
                        if (exists != null && exists != "")
                        {
                            string hashpassword = CreateRandomPassword();
                            cmd = "Update users set Password =@Pass";
                            cmd += " Where UserName =@Unum";
                            sqc = new SqlCommand(cmd, cdata.GetConnection());
                            sqc.Parameters.AddWithValue("@Pass", hashpassword);
                            sqc.Parameters.AddWithValue("@Unum", mobile_.Text.Trim());
                            sqc.ExecuteNonQuery();

                            //////////EMAIL CODE FOR PASSWORD/////////////////////
                            //////////////////////////////////////////////////////
                            //string NIFTEmail = email_.Text.Trim();

                            SmtpClient smtpClient = new SmtpClient("mail.nift.pk", 25);
                            smtpClient.Credentials = new System.Net.NetworkCredential("no-reply@niftepay.pk", "Abc123=+");
                            //smtpClient.UseDefaultCredentials = true; // uncomment if you don't want to use the network credentials
                            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                            smtpClient.EnableSsl = true;

                            //Setting From , To and CC
                            MailMessage mail = new MailMessage();
                            mail.From = new MailAddress("no-reply@niftepay.pk", "NIFT - ePAY");
                            mail.To.Add(new MailAddress(Emailadd));
                            mail.Subject = "Auto Email Notification - Complaint Management";

                            string mesage = "Dear " + fullname + "," + System.Environment.NewLine;
                            mesage += "Your NIFT-ePay portal account password is changed. Kindly login with the updated password on the portal." + System.Environment.NewLine;
                            mesage += "Your new generated password is: " + Orig_Pass + System.Environment.NewLine + System.Environment.NewLine + System.Environment.NewLine;
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
                            ///////////////////////////////////////////
                            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','Email Sent For New Password')", true);

                            ////////////////////////////////////////////////////////////////
                            //lblmsg.Text = "Email Sent For New Password";
                        }
                        else
                        {
                            //lblmsg.Text = "UserName Does Not Exist";
                            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','UserName Does Not Exist')", true);
                        }
                    }
                    else
                    {
                        //lblmsg.Text = "Please Provide Required Details";
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Alert", "infomsg('" + InfoType.info + "','','Please Provide Required Details')", true);
                    }
                    Session["mobCount"] = Convert.ToInt64(Session["mobCount"]) + 1;
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
        private static string CreateRandomPassword(int length = 20)
        {
            // Create a string of characters, numbers, special characters that allowed in the password  
            string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?_-";
            Random random = new Random();

            // Select one random character at a time from the string  
            // and create an array of chars  
            char[] chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                chars[i] = validChars[random.Next(0, validChars.Length)];
            }
            Orig_Pass = new string(chars);
            EncDec encdec = new EncDec();
            string HASH = encdec.GetHash(Orig_Pass);
            //string enc_chars = encdec.Encrypt(Convert.ToString(Orig_Pass));
            return HASH;
        }
    }
}