using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace ChequePro.Models
{
    /// <summary>
    /// Helper functions for logging errors and debug information.
    /// </summary>

    internal enum TraceLevel1
    {
        TraceNone = 0, // no trace
        TraceInfo = 20, // some extra info
        TraceErr = 30, // Error  info		
        TraceDetail = 40 // detailed debugging info
    };

    internal class InfoLogger
    {
        private static StreamWriter tw_rec;
        public static int dlevel;
        private static DateTime CurrFileDateTime_Rec;

        public static void SetTraceLevel(ChequePro.Models.TraceLevel1 Level)
        {
            dlevel = (int)Level;
        }
        static InfoLogger()
        {
            try
            {
                var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                string LogPath = MyConfig.GetValue<string>("AppSettings:LogPath");

                string datetime = System.DateTime.Now.ToString("MM-dd-yyyy");
                string Filename = LogPath + "ChequePro_Info_log" + datetime + ".log";
                tw_rec = File.AppendText(Filename);
                //tw_rec = TextWriter.Synchronized(tw_rec);
                CurrFileDateTime_Rec = System.DateTime.Now;
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(".NET Runtime", "Image Loader Portal :: Exception Error : " + ex, EventLogEntryType.Error, 1000);
            }
        }
        private static void creatFilePointer_rec()
        {
            try
            {
                var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                string LogPath = MyConfig.GetValue<string>("AppSettings:LogPath");

                string datetime = System.DateTime.Now.ToString("MM-dd-yyyy");
                string Filename = LogPath + "ChequePro_Info_log" + datetime + ".log";
                tw_rec = File.AppendText(Filename);
                //tw_rec = TextWriter.Synchronized(tw_rec);
                CurrFileDateTime_Rec = System.DateTime.Now;
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(".NET Runtime", "Image Loader Portal :: Exception Error : " + ex, EventLogEntryType.Error, 1000);
            }
        }
        public static void Log(params string[] message)
        {
            string msg = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + " :|";

            msg = msg + message[0].PadRight(6, ' ') + "|";
            for (int i = 1; i < message.Length; i++)
            {
                msg = msg + message[i].PadRight(50, ' ') + "|";
            }

            EventLog.WriteEntry(".NET Runtime", "Image Loader Portal :: Info : " + msg, EventLogEntryType.Information, 1000);

            if (tw_rec == null)
                creatFilePointer_rec();

            if (DateTime.Now.Day != CurrFileDateTime_Rec.Day)
            {
                string datetime = System.DateTime.Now.ToString("MM-dd-yyyy");
                CurrFileDateTime_Rec = DateTime.Now;

                try
                {
                    tw_rec.Close();
                }
                catch (Exception ex)
                {

                }

                var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                string LogPath = MyConfig.GetValue<string>("AppSettings:LogPath");

                string sFileDT = CurrFileDateTime_Rec.ToString("MM-dd-yyyy");
                string Filename = LogPath + "ChequePro_Info_log" + datetime + ".log";

                tw_rec = File.AppendText(Filename);
                //tw_rec = TextWriter.Synchronized(tw_rec);
            }

            msg = msg.Substring(0, msg.IndexOf('|', msg.Length - 2));

            tw_rec.WriteLine(msg);
            tw_rec.Flush();
            //tw_rec.Close();

            //tw_rec = TextWriter.Synchronized(tw_rec);
            //tw_rec.WriteLine(msg);
            //tw_rec.Flush();
        }
        public static void InsertLineRec()
        {
            if (tw_rec == null) { creatFilePointer_rec(); }
            tw_rec.WriteLine("".PadRight(200, '-'));
            tw_rec.Flush();
        }
    }
    internal class WarningLogger
    {
        private static StreamWriter tw_rec;
        public static int dlevel;
        private static DateTime CurrFileDateTime_Rec;

        public static void SetTraceLevel(ChequePro.Models.TraceLevel1 Level)
        {
            dlevel = (int)Level;
        }
        static WarningLogger()
        {
            try
            {
                var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                string LogPath = MyConfig.GetValue<string>("AppSettings:LogPath");

                string datetime = System.DateTime.Now.ToString("MM-dd-yyyy");
                string Filename = LogPath + "ChequePro_Warning_log" + datetime + ".log";
                tw_rec = File.AppendText(Filename);
                //tw_rec = TextWriter.Synchronized(tw_rec);
                CurrFileDateTime_Rec = System.DateTime.Now;
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(".NET Runtime", "Image Loader Portal :: Exception Error : " + ex, EventLogEntryType.Error, 1000);
            }
        }
        private static void creatFilePointer_rec()
        {
            try
            {
                var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                string LogPath = MyConfig.GetValue<string>("AppSettings:LogPath");

                string datetime = System.DateTime.Now.ToString("MM-dd-yyyy");
                string Filename = LogPath + "ChequePro_Warning_log" + datetime + ".log";
                tw_rec = File.AppendText(Filename);
                //tw_rec = TextWriter.Synchronized(tw_rec);
                CurrFileDateTime_Rec = System.DateTime.Now;
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(".NET Runtime", "Image Loader Portal :: Exception Error : " + ex, EventLogEntryType.Error, 1000);
            }
        }
        public static void Log(params string[] message)
        {
            string msg = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + " :|";

            msg = msg + message[0].PadRight(6, ' ') + "|";
            for (int i = 1; i < message.Length; i++)
            {
                msg = msg + message[i].PadRight(50, ' ') + "|";
            }

            EventLog.WriteEntry(".NET Runtime", "Image Loader Portal :: Warning : " + msg, EventLogEntryType.Warning, 1000);

            if (tw_rec == null)
                creatFilePointer_rec();

            if (DateTime.Now.Day != CurrFileDateTime_Rec.Day)
            {
                string datetime = System.DateTime.Now.ToString("MM-dd-yyyy");
                CurrFileDateTime_Rec = DateTime.Now;

                try
                {
                    tw_rec.Close();
                }
                catch (Exception ex)
                {

                }

                var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                string LogPath = MyConfig.GetValue<string>("AppSettings:LogPath");

                string sFileDT = CurrFileDateTime_Rec.ToString("MM-dd-yyyy");
                string Filename = LogPath + "ChequePro_Warning_log" + datetime + ".log";

                tw_rec = File.AppendText(Filename);
                //tw_rec = TextWriter.Synchronized(tw_rec);
            }

            msg = msg.Substring(0, msg.IndexOf('|', msg.Length - 2));
            tw_rec.WriteLine(msg);
            tw_rec.Flush();
            //tw_rec.Close();

            
            //tw_rec = TextWriter.Synchronized(tw_rec);
            //tw_rec.WriteLine(msg);
            //tw_rec.Flush();
        }
        public static void InsertLineRec()
        {
            if (tw_rec == null) { creatFilePointer_rec(); }
            tw_rec.WriteLine("".PadRight(200, '-'));
            tw_rec.Flush();
        }
    }
    internal class ExceptionLogger
    {
        private static StreamWriter tw_rec;
        public static int dlevel;
        private static DateTime CurrFileDateTime_Rec;

        public static void SetTraceLevel(ChequePro.Models.TraceLevel1 Level)
        {
            dlevel = (int)Level;
        }
        static ExceptionLogger()
        {
            try
            {
                var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                string LogPath = MyConfig.GetValue<string>("AppSettings:LogPath");

                string datetime = System.DateTime.Now.ToString("MM-dd-yyyy");
                string Filename = LogPath + "ChequePro_Exception_log" + datetime + ".log";
                tw_rec = File.AppendText(Filename);
                //tw_rec = TextWriter.Synchronized(tw_rec);
                CurrFileDateTime_Rec = System.DateTime.Now;

            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(".NET Runtime", "Image Loader Portal :: Exception Error : " + ex, EventLogEntryType.Error, 1000);
            }
        }
        private static void creatFilePointer_rec()
        {
            try
            {
                var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                string LogPath = MyConfig.GetValue<string>("AppSettings:LogPath");

                string datetime = System.DateTime.Now.ToString("MM-dd-yyyy");
                string Filename = LogPath + "ChequePro_Exception_log" + datetime + ".log";
                tw_rec = File.AppendText(Filename);
                //tw_rec = TextWriter.Synchronized(tw_rec);
                CurrFileDateTime_Rec = System.DateTime.Now;
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(".NET Runtime", "Image Loader Portal :: Exception Error : " + ex, EventLogEntryType.Error, 1000);
            }
        }
        public static void Log(params string[] message)
        {
            string msg = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + " :|";

            msg = msg + message[0].PadRight(6, ' ') + "|";
            for (int i = 1; i < message.Length; i++)
            {
                msg = msg + message[i].PadRight(50, ' ') + "|";
            }

            EventLog.WriteEntry(".NET Runtime", "Image Loader Portal :: Exception Error : " + msg, EventLogEntryType.Error, 1000);

            if (tw_rec == null)
                creatFilePointer_rec();

            if (DateTime.Now.Day != CurrFileDateTime_Rec.Day)
            {
                string datetime = System.DateTime.Now.ToString("MM-dd-yyyy");
                CurrFileDateTime_Rec = DateTime.Now;

                try
                {
                    tw_rec.Close();
                }
                catch (Exception ex)
                {

                }
                

                var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                string LogPath = MyConfig.GetValue<string>("AppSettings:LogPath");

                string sFileDT = CurrFileDateTime_Rec.ToString("MM-dd-yyyy");
                string Filename = LogPath + "ChequePro_Exception_log" + datetime + ".log";

                tw_rec = File.AppendText(Filename);
                //tw_rec = TextWriter.Synchronized(tw_rec);
            }

            msg = msg.Substring(0, msg.IndexOf('|', msg.Length - 2));
            tw_rec.WriteLine(msg);
            tw_rec.Flush();
            //tw_rec.Close();

            //tw_rec = TextWriter.Synchronized(tw_rec);
            //tw_rec.WriteLine(msg);
            //tw_rec.Flush();
        }
        public static void InsertLineRec()
        {
            if (tw_rec == null) { creatFilePointer_rec(); }
            tw_rec.WriteLine("".PadRight(200, '-'));
            tw_rec.Flush();
        }
    }
}