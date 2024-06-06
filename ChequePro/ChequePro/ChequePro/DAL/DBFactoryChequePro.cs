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
using ChequePro.Model;
using System.Security.Principal;

namespace ChequePro.DAL
{
    public class DBFactoryChequePro : DBFactory
    {
        public DBFactoryChequePro()
        {
            SetConnection("ChequePro");
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

        public User_Login get_Keys(string UserID)
        {
            User_Login obj = new User_Login();
            try
            {
                //InfoLogger.Log("get_Cities :: ");

                List<SqlParameter> sqlParam = new List<SqlParameter>();
                sqlParam.Add(new SqlParameter("@UserID", UserID));

                DataSet ds = ExecuteQuery("sp_Get_UserToken", sqlParam);

                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow item in ds.Tables[0].Rows)
                    {
                        obj.H_AuthToken = item["AuthToken"] == DBNull.Value ? "" : item["AuthToken"].ToString();
                        obj.H_AuthSecretKey = item["AuthSecretKey"] == DBNull.Value ? "" : item["AuthSecretKey"].ToString();
                        obj.BankID = Convert.ToInt32(item["BankID"] == DBNull.Value ? "0" : item["BankID"]);
                        obj.BranchID = Convert.ToInt32(item["BranchID"] == DBNull.Value ? "0" : item["BranchID"]);
                        obj.AccountID = Convert.ToInt64(item["AccountID"] == DBNull.Value ? "0" : item["AccountID"]);
                    }
                }

            }
            catch (Exception ex)
            {
                ExceptionLogger.Log("get_Keys Error Exception :: " + ex);
            }
            return obj;
        }

        public bool validate_AuthToken(string H_AuthToken, string H_AuthSecretKey)
        {
            bool ret = false;
            try
            {
                //InfoLogger.Log("validate_Password :: " + param.UserID);

                List<SqlParameter> sqlParam = new List<SqlParameter>();
                sqlParam.Add(new SqlParameter("@H_AuthToken", H_AuthToken));
                sqlParam.Add(new SqlParameter("@H_AuthSecretKey", H_AuthSecretKey));

                int cnt = Convert.ToInt32(ExecuteScalar("sp_Get_UserAuthToken", sqlParam));
                
                if (cnt > 0)
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

        public int upd_AuthBankUser_SessionToken(string H_AuthToken, string H_AuthSecretKey, int bankID, int branchID, long accountID, string sessionKey, int AuthID)
        {
            int ret = 0;
            try
            {
                //InfoLogger.Log("validate_Password :: " + param.UserID);

                List<SqlParameter> sqlParam = new List<SqlParameter>();
                sqlParam.Add(new SqlParameter("@H_AuthToken", H_AuthToken));
                sqlParam.Add(new SqlParameter("@H_AuthSecretKey", H_AuthSecretKey));
                sqlParam.Add(new SqlParameter("@BankID", bankID));
                sqlParam.Add(new SqlParameter("@BranchID", branchID));
                sqlParam.Add(new SqlParameter("@AccountID", accountID));
                sqlParam.Add(new SqlParameter("@SessionKey", sessionKey));
                sqlParam.Add(new SqlParameter("@AuthID", AuthID));

                ret = Convert.ToInt32(ExecuteScalar("sp_Get_verify_User_Upd_Token", sqlParam));


                //Logger.Log("get_Cities ExecuteQuery sp_get_City :: " + obj.Count);
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log("upd_AuthBankUser_SessionToken Error Exception :: " + ex);
            }
            return ret;
        }

        public AuthDetails  (string H_AuthToken, string H_AuthSecretKey, int bankID)
        {
            AuthDetails dt = new AuthDetails();
            try
            {
                //InfoLogger.Log("validate_Password :: " + param.UserID);

                List<SqlParameter> sqlParam = new List<SqlParameter>();
                sqlParam.Add(new SqlParameter("@H_AuthToken", H_AuthToken));
                sqlParam.Add(new SqlParameter("@H_AuthSecretKey", H_AuthSecretKey));
                sqlParam.Add(new SqlParameter("@BankID", bankID));

                DataSet ds = new DataSet();
                ds = ExecuteQuery("sp_Get_AuthID_Details", sqlParam);

                if (ds.Tables.Count > 0) 
                {
                    dt.AuthID = Convert.ToInt32(ds.Tables[0].Rows[0]["AuthTokenID"] == DBNull.Value ? "0" : ds.Tables[0].Rows[0]["AuthTokenID"]);
                    dt.IsFirstLogin = Convert.ToInt32(ds.Tables[0].Rows[0]["IsFirstLogin"] == DBNull.Value ? "0" : ds.Tables[0].Rows[0]["IsFirstLogin"]);
                    dt.AuthTokenExpireOn = Convert.ToDateTime(ds.Tables[0].Rows[0]["AuthTokenExpireOn"] == DBNull.Value ? "0" : ds.Tables[0].Rows[0]["AuthTokenExpireOn"]);
                }

                //Logger.Log("get_Cities ExecuteQuery sp_get_City :: " + obj.Count);
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log("get_AuthBankUser_SessionToken Error Exception :: " + ex);
            }
            return dt;
        }

        public List<ChequeSummary> get_chequeSummary(long accountNo, int bankID, int branchcode)
        {
            List<ChequeSummary> dt = new List<ChequeSummary>();
            try
            {
                //InfoLogger.Log("validate_Password :: " + param.UserID);

                List<SqlParameter> sqlParam = new List<SqlParameter>();
                sqlParam.Add(new SqlParameter("@account", accountNo));
                sqlParam.Add(new SqlParameter("@bankcode", bankID));
                sqlParam.Add(new SqlParameter("@branchcode", branchcode));

                DataSet ds = new DataSet();
                ds = ExecuteQuery("SP_ChequeRecordList", sqlParam);

                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        ChequeSummary cs = new ChequeSummary();
                        cs.hostid = Convert.ToInt32(dr["hostid"] == DBNull.Value ? "0" : dr["hostid"]);
                        cs.ChequeNo = dr["ChequeNo"] == DBNull.Value ? "" : dr["ChequeNo"].ToString();
                        cs.AccountNo = dr["AccountNo"] == DBNull.Value ? "" : dr["AccountNo"].ToString();
                        cs.Amount = dr["Amount"] == DBNull.Value ? "" : dr["Amount"].ToString();
                        cs.Processdate = dr["Processdate"] == DBNull.Value ? "" : dr["Processdate"].ToString();
                        cs.receiverbank = dr["receiverbank"] == DBNull.Value ? "" : dr["receiverbank"].ToString();
                        cs.receiverbranch = dr["receiverbranch"] == DBNull.Value ? "" : dr["receiverbranch"].ToString();
                        cs.senderbank = dr["senderbank"] == DBNull.Value ? "" : dr["senderbank"].ToString();
                        cs.senderbranch = dr["senderbranch"] == DBNull.Value ? "" : dr["senderbranch"].ToString();
                        cs.reasonid = dr["reasonid"] == DBNull.Value ? "" : dr["reasonid"].ToString();
                        cs.reason = dr["reason"] == DBNull.Value ? "" : dr["reason"].ToString();
                        cs.ReceiverBankBranchName = dr["ReceiverBankBranchName"] == DBNull.Value ? "" : dr["ReceiverBankBranchName"].ToString();
                        dt.Add(cs);
                    }
                }

                //Logger.Log("get_Cities ExecuteQuery sp_get_City :: " + obj.Count);
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log("get_chequeSummary Error Exception :: " + ex);
            }
            return dt;
        }

        public ChequeDetails get_chequeDetails(long? hostID)
        {
            ChequeDetails dt = new ChequeDetails();
            try
            {
                //InfoLogger.Log("validate_Password :: " + param.UserID);

                List<SqlParameter> sqlParam = new List<SqlParameter>();
                sqlParam.Add(new SqlParameter("@hostid", hostID));
                
                DataSet ds = new DataSet();
                ds = ExecuteQuery("SP_Cheque_Details", sqlParam);

                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        dt.hostid = Convert.ToInt32(dr["hostid"] == DBNull.Value ? "0" : dr["hostid"]);
                        dt.ChequeNo = dr["ChequeNo"] == DBNull.Value ? "" : dr["ChequeNo"].ToString();
                        dt.AccountNo = dr["AccountNo"] == DBNull.Value ? "" : dr["AccountNo"].ToString();
                        dt.SequenceNo = dr["SequenceNo"] == DBNull.Value ? "" : dr["SequenceNo"].ToString();
                        dt.Amount = dr["Amount"] == DBNull.Value ? "" : dr["Amount"].ToString();
                        dt.cycleno = dr["cycleno"] == DBNull.Value ? "" : dr["cycleno"].ToString();
                        dt.CityID = dr["CityID"] == DBNull.Value ? "" : dr["CityID"].ToString();
                        dt.Processdate = dr["Processdate"] == DBNull.Value ? "" : dr["Processdate"].ToString();
                        dt.receiverbank = dr["receiverbank"] == DBNull.Value ? "" : dr["receiverbank"].ToString();
                        dt.receiverbranch = dr["receiverbranch"] == DBNull.Value ? "" : dr["receiverbranch"].ToString();
                        dt.reasonid = dr["reasonid"] == DBNull.Value ? "" : dr["reasonid"].ToString();
                        dt.reason = dr["reason"] == DBNull.Value ? "" : dr["reason"].ToString();
                        dt.isDeffer = dr["isDeffer"] == DBNull.Value ? "" : dr["isDeffer"].ToString();
                        dt.isauth = dr["isauth"] == DBNull.Value ? "" : dr["isauth"].ToString();
                        dt.Undersize_Image = dr["Undersize_Image"] == DBNull.Value ? "" : dr["Undersize_Image"].ToString();
                        dt.FoldedDocCorners = dr["FoldedDocCorners"] == DBNull.Value ? "" : dr["FoldedDocCorners"].ToString();
                        dt.FoldedDocEdges = dr["FoldedDocEdges"] == DBNull.Value ? "" : dr["FoldedDocEdges"].ToString();
                        dt.Framing_Error = dr["Framing_Error"] == DBNull.Value ? "" : dr["Framing_Error"].ToString();
                        dt.DocSkew = dr["DocSkew"] == DBNull.Value ? "" : dr["DocSkew"].ToString();
                        dt.Oversize_Image = dr["Oversize_Image"] == DBNull.Value ? "" : dr["Oversize_Image"].ToString();
                        dt.Piggy_Back = dr["Piggy_Back"] == DBNull.Value ? "" : dr["Piggy_Back"].ToString();
                        dt.Image_Too_Light = dr["Image_Too_Light"] == DBNull.Value ? "" : dr["Image_Too_Light"].ToString();
                        dt.Image_Too_Dark = dr["Image_Too_Dark"] == DBNull.Value ? "" : dr["Image_Too_Dark"].ToString();
                        dt.Horizontal_Streaks = dr["Horizontal_Streaks"] == DBNull.Value ? "" : dr["Horizontal_Streaks"].ToString();
                        dt.BelowMinImgsize = dr["BelowMinImgsize"] == DBNull.Value ? "" : dr["BelowMinImgsize"].ToString();
                        dt.AboveMaxImgsize = dr["AboveMaxImgsize"] == DBNull.Value ? "" : dr["AboveMaxImgsize"].ToString();
                        dt.Spot_Noise = dr["Spot_Noise"] == DBNull.Value ? "" : dr["Spot_Noise"].ToString();
                        dt.FrontRearMismatch = dr["FrontRearMismatch"] == DBNull.Value ? "" : dr["FrontRearMismatch"].ToString();
                        dt.Carbon_Strip = dr["Carbon_Strip"] == DBNull.Value ? "" : dr["Carbon_Strip"].ToString();
                        dt.Out_of_Focus = dr["Out_of_Focus"] == DBNull.Value ? "" : dr["Out_of_Focus"].ToString();
                        dt.IQATag = dr["IQATag"] == DBNull.Value ? "" : dr["IQATag"].ToString();
                        dt.barcodeMatch = dr["barcodeMatch"] == DBNull.Value ? "" : dr["barcodeMatch"].ToString();
                        dt.UVStr = dr["UVStr"] == DBNull.Value ? "" : dr["UVStr"].ToString();
                        dt.Duplicate = dr["Duplicate"] == DBNull.Value ? "" : dr["Duplicate"].ToString();
                        dt.MICR_Present = dr["MICR_Present"] == DBNull.Value ? "" : dr["MICR_Present"].ToString();
                        dt.Average_Amount = dr["Average_Amount"] == DBNull.Value ? "" : dr["Average_Amount"].ToString();
                        dt.STD_Non_STD = dr["STD_Non_STD"] == DBNull.Value ? "" : dr["STD_Non_STD"].ToString();
                        dt.Water_Mark = dr["Water_Mark"] == DBNull.Value ? "" : dr["Water_Mark"].ToString();
                        dt.isFraud = dr["isFraud"] == DBNull.Value ? "" : dr["isFraud"].ToString();
                        dt.FrontImage = (byte[]) (dr["FrontImage"] == DBNull.Value ? "" : dr["FrontImage"]);
                        dt.BackImage = (byte[]) (dr["BackImage"] == DBNull.Value ? "" : dr["BackImage"]);
                        dt.UVImage = (byte[]) (dr["UVImage"] == DBNull.Value ? "" : dr["UVImage"]);
                        dt.ReceiverBankName = dr["ReceiverBankName"] == DBNull.Value ? "" : dr["ReceiverBankName"].ToString();
                        dt.ReceiverBranchName = dr["ReceiverBranchName"] == DBNull.Value ? "" : dr["ReceiverBranchName"].ToString();
                        dt.TransCode = dr["TransCode"] == DBNull.Value ? "" : dr["TransCode"].ToString();
                        dt.UVPercent = dr["UVPercent"] == DBNull.Value ? "" : dr["UVPercent"].ToString();
                        dt.UVTemplateID = dr["UVTemplateID"] == DBNull.Value ? "" : dr["UVTemplateID"].ToString();
                    }
                }

                //Logger.Log("get_Cities ExecuteQuery sp_get_City :: " + obj.Count);
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log("get_chequeSummary Error Exception :: " + ex);
            }
            return dt;
        }
    }
}