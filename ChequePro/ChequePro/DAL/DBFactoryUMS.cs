using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography.Xml;
//using ServiceStack;
//using ServiceStack.Configuration;
using System.Security.Policy;
using System.Net;
//using static ServiceStack.Svg;
using System.Transactions;
using static System.Net.WebRequestMethods;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using System.Security.AccessControl;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Components.Web;
using ChequePro.Models;

namespace ChequePro.DAL
{
    public class DBFactoryUMS : DBFactory
    {
        public int ModuleID { get; set; }
        public DBFactoryUMS()
        {
            var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            ModuleID = Convert.ToInt32(MyConfig.GetValue<string>("AppSettings:Module"));
            SetConnection("UMS");
        }


        public bool validate_Password(LoginUser param)
        {
            bool ret = false;
            try
            {
                //InfoLogger.Log("validate_Password :: " + param.UserID);

                List<SqlParameter> sqlParam = new List<SqlParameter>();
                sqlParam.Add(new SqlParameter("@UserID", param.UserID));

                DataSet ds = ExecuteQuery("sp_Get_UserPassword", sqlParam);
                string Password = "";
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow item in ds.Tables[0].Rows)
                    {
                        Password = item["Password"] == DBNull.Value ? "" : item["Password"].ToString();
                    }
                }

                if (!string.IsNullOrEmpty(Password) && Password == param.Password && !string.IsNullOrEmpty(param.Password))
                {
                    ret = true;
                }

                //Logger.Log("get_Cities ExecuteQuery sp_get_City :: " + obj.Count);
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log("validate_Password Error Exception :: " + ex);
            }
            return ret;
        }

        public bool validate_AuthToken(LoginUser param)
        {
            bool ret = false;
            try
            {
                //InfoLogger.Log("validate_Password :: " + param.UserID);

                List<SqlParameter> sqlParam = new List<SqlParameter>();
                sqlParam.Add(new SqlParameter("@UserID", param.UserID));

                DataSet ds = ExecuteQuery("sp_Get_UserPassword", sqlParam);
                string Password = "";
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow item in ds.Tables[0].Rows)
                    {
                        Password = item["Password"] == DBNull.Value ? "" : item["Password"].ToString();
                    }
                }

                if (!string.IsNullOrEmpty(Password) && Password == param.Password && !string.IsNullOrEmpty(param.Password))
                {
                    ret = true;
                }

                //Logger.Log("get_Cities ExecuteQuery sp_get_City :: " + obj.Count);
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log("validate_AuthToken Error Exception :: " + ex);
            }
            return ret;
        }
        

    }
}