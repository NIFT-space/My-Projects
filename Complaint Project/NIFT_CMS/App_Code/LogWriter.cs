using NIFT_CMS;
using System;
using System.IO;
using System.Web;

namespace IBCS.Data
{
    public class LogWriter
    {
        public LogWriter()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public static void WriteToLog(Exception ex)
        {
            try
            {
                if (ex.HResult == -2146233040)
                {

                }
                else if (ex.HResult == -2147467261)
                {

                }
                else
                {

                    string sYear = DateTime.Now.Year.ToString();
                    string sMonth = DateTime.Now.Month.ToString();
                    string sDay = DateTime.Now.Day.ToString();
                    string sHrs = DateTime.Now.Hour.ToString();
                    string sMin = DateTime.Now.Minute.ToString();
                    string sFileName = "Err" + sYear + sMonth + sDay + sHrs + sMin + ".txt";
                    string message = string.Format("Time: {0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
                    message += Environment.NewLine;
                    message += string.Format("Message: {0}", ex.Message);
                    message += Environment.NewLine;
                    message += string.Format("Source: {0}", ex.Source);
                    message += Environment.NewLine;
                    message += string.Format("Stack: {0}", ex.StackTrace);
                    message += Environment.NewLine;


                    string msg = "", src = "", err = "";
                    int issent = 0;
                    int hr = 0;
                    string cmd = string.Empty;
                    cmd = "Insert into CMSLogs (ExceptionCode, Message, Source, ErrorMessage, Updatedatetime , isSent)";
                    cmd += " values";
                    //cmd += " ('" + ex.HResult + "', '', '' , '' , '" + DateTime.Now + "')";

                    hr = ex.HResult;
                    msg = ex.Message;
                    src = ex.Source;
                    int len = ex.StackTrace.Length;
                    int lenmsg = ex.Message.Length;
                    try
                    {
                        if (len >= 500)
                        {
                            //err = ex.StackTrace.PadRight(100);
                            err = ex.StackTrace.Substring(len - 500, 500);
                        }
                        else
                        {
                            err = ex.StackTrace;
                        }
                    }
                    catch
                    {
                        err = ex.StackTrace.ToString();
                    }

                    if (msg.Contains("'"))
                    {
                        msg = ex.Message.Replace("'", "");
                    }
                    try
                    {
                        //if (msg.Length > 1000)
                        //{
                        //    msg = msg.PadRight(1000);
                        //}
                        //else
                        //{
                        //    msg = msg.PadRight(200);
                        //}
                        if (lenmsg >= 500)
                        {
                            msg = ex.Message.Substring(lenmsg - 500, 500);
                        }
                        else
                        {
                            msg = ex.Message;
                        }
                    }
                    catch
                    {
                        msg = "Large Data in Message";
                    }
                    if (msg.Contains("'"))
                    {
                        msg = msg.ToString().Replace("'", "");
                    }
                    if (src.Contains("'"))
                    {
                        src = src.ToString().Replace("'", "");
                    }
                    //if (src.Length > 100)
                    //{
                    //    src = src.PadRight(100);
                    //}
                    if (err.Contains("'"))
                    {
                        err = err.ToString().Replace("'", "");
                    }
                    cmd += " ('" + ex.HResult + "', '" + msg + "', '" + src + "' , '" + err + "' , '" + DateTime.Now + "' , " + issent + ")";

                    cDataAccess cd = new cDataAccess();
                    try
                    {
                        if (hr == -2146233040)
                        {

                        }
                        else if (hr == -2147467261)
                        {

                        }
                        else
                        {
                            cd.RunProc(cmd);
                        }
                    }
                    catch (Exception)
                    {
                        cmd = "";
                        cmd = "Insert into CMSLogs (ExceptionCode, Message, Source, ErrorMessage, Updatedatetime , isSent)";
                        cmd += " values";
                        cmd += " ('" + ex.HResult + "', '" + msg + "', '" + src + "' , 'EMPTY' , '" + DateTime.Now + "' , " + issent + ")";
                        cd.RunProc(cmd);
                    }

                    if (hr == -2146233040)
                    {

                    }
                    else if (hr == -2147467261)
                    {

                    }


                    string filepath = System.Web.HttpContext.Current.Server.MapPath("~/Logs/");

                    if (!Directory.Exists(filepath))
                    {
                        Directory.CreateDirectory(filepath);
                    }
                    filepath = filepath + sFileName;
                    if (!File.Exists(filepath))
                    {
                        File.Create(filepath).Dispose();
                    }
                    using (StreamWriter sw = File.AppendText(filepath))
                    {
                        sw.WriteLine(message);
                        sw.Flush();
                        sw.Close();
                    }
                }
            }
            catch (Exception e)
            {
                e.ToString();

            }
        }

        public static void WriteToLog(string ex)
        {
            try
            {
                string sYear = DateTime.Now.Year.ToString();
                string sMonth = DateTime.Now.Month.ToString();
                string sDay = DateTime.Now.Day.ToString();
                string sHrs = DateTime.Now.Hour.ToString();
                string sMin = DateTime.Now.Minute.ToString();
                string sFileName = "Msg" + sYear + sMonth + sDay + sHrs + sMin + ".txt";
                string message = string.Format("Time: {0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
                message += Environment.NewLine;
                message += string.Format("Message: {0}", ex);
                message += Environment.NewLine;

                
                string filepath = System.Web.HttpContext.Current.Server.MapPath("~/Logs/");

                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }
                filepath = filepath + sFileName;
                if (!File.Exists(filepath))
                {
                    File.Create(filepath).Dispose();
                }
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(message);
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception e)
            {
                e.ToString();

            }
        }
    }
}