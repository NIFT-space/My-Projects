﻿using System;
using System.Web;
using System.Drawing;
using System.IO;
using System.Web.SessionState;

namespace NIFT_CMS
{
    /// <summary>
    /// Summary description for Handler1
    /// </summary>
    public class Handler1 : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            Bitmap bmpOut = new Bitmap(200, 50);
            Graphics g = Graphics.FromImage(bmpOut);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.FillRectangle(Brushes.Black, 0, 0, 200, 50);
            g.DrawString(context.Session["Captcha"].ToString(), new Font("Verdana", 18), new SolidBrush(Color.White), 0, 0);
            MemoryStream ms = new MemoryStream();
            bmpOut.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            byte[] bmpBytes = ms.GetBuffer();
            bmpOut.Dispose();
            ms.Close();
            context.Response.BinaryWrite(bmpBytes);
            context.Response.End();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}