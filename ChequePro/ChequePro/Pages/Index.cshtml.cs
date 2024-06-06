using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Novell.Directory.Ldap;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ChequePro.Models;
using System.Collections.Generic;
using ChequePro.DAL;
using Microsoft.AspNetCore.Http;

namespace ChequePro.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public const string SessionKeyName_AuthID = "_AuthID";
        public const string SessionKeyName_TokenKey = "_TokenKey";
        public const string SessionKeyName_AccountNo = "_AccountNo";
        public const string SessionKeyName_BankID = "_BankID";
        public const string SessionKeyName_BranchID = "_BranchID";
        static Novell.Directory.Ldap.LdapConnection ADConnect = new Novell.Directory.Ldap.LdapConnection();

        public Login lgn { get; set; }

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            try
            {
                string AuthID = HttpContext.Session.GetString(IndexModel.SessionKeyName_AuthID);
                if (!string.IsNullOrEmpty(AuthID))
                {
                    WarningLogger.Log("Index Login Page  :: Already Login Session Available : " + AuthID);
                    Response.Redirect("/Home/Dashboard");
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log("Index Login Page OnGet :: Error Exception : " + ex);
            }
        }


        public JsonResult OnPostVerifyLogin(string LoginID, string Password)
        {
            try
            {

                LoginUser obj = new LoginUser();

                //// Generate API Key
                //string apiKey = KeyGenerator.GenerateApiKey(32); // Adjust the length as needed
                //Console.WriteLine($"Generated API Key: {apiKey}");

                //// Generate API Key Secret
                //string apiSecret = KeyGenerator.GenerateApiKey(32); // Adjust the length as needed
                //Console.WriteLine($"Generated API Secret: {apiSecret}");


                HashIt hs = new HashIt();

                obj.UserID = LoginID;
                obj.Password = hs.GetHash(Password);

                DBFactoryChequePro db = new DBFactoryChequePro();

                bool val = db.validate_Password(obj);

                if (val)
                {
                    User_Login objUser = new User_Login();
                    objUser = db.get_Keys(LoginID);

                    string H_authToken = hs.GetHash(objUser.H_AuthToken);
                    string H_AuthSecretKey = hs.GetHash(objUser.H_AuthSecretKey);

                    AuthDetails ad = new AuthDetails();
                    ad = db.get_AuthBankUser_SessionToken(H_authToken, H_AuthSecretKey, objUser.BankID);

                    if (ad.AuthID < 0)
                    {
                        WarningLogger.Log("Login UserLogin LoginID: " + LoginID + " : Invalid AuthToken or AuthSecretKey Details Provided.");
                        return new JsonResult("Invalid AuthToken or AuthSecretKey Details Provided");
                    }

                    if (ad.AuthTokenExpireOn < DateTime.Now)
                    {
                        WarningLogger.Log("Login UserLogin LoginID: " + LoginID + " : AuthSecretKey Is Expired.");
                        return new JsonResult("AuthSecretKey Is Expired");
                    }

                    string TokenKey = KeyGenerator.GenerateApiKey(32);
                    int i_ret = db.upd_AuthBankUser_SessionToken(H_authToken, H_AuthSecretKey, objUser.BankID, objUser.BranchID, objUser.AccountID, TokenKey, ad.AuthID);

                    if (i_ret == 1)
                    {
                        HttpContext.Session.SetString(SessionKeyName_AuthID, ad.AuthID.ToString());
                        HttpContext.Session.SetString(SessionKeyName_TokenKey, TokenKey);
                        HttpContext.Session.SetString(SessionKeyName_BankID, objUser.BankID.ToString());
                        HttpContext.Session.SetString(SessionKeyName_BranchID, objUser.BranchID.ToString());
                        HttpContext.Session.SetString(SessionKeyName_AccountNo, objUser.AccountID.ToString());

                        InfoLogger.Log("Login UserLogin LoginID: " + LoginID + " : Validated Successfully.");
                        return new JsonResult("Success");
                    }
                    else if (i_ret == 2)
                    {
                        WarningLogger.Log("Login UserLogin LoginID: " + LoginID + " : Invalid Account Details.");
                        return new JsonResult("Invalid Account Details");
                    }
                    else
                    {
                        WarningLogger.Log("Login UserLogin LoginID: " + LoginID + " : Invalid Details Provided.");
                        return new JsonResult("Invalid Details Provided");
                    }
                }
                else
                {
                    WarningLogger.Log("Login UserLogin LoginID: " + LoginID + " : User Validation Failed.");
                    return new JsonResult("User Validation Failed");
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log("VerifyLogin :: error Exception : " + ex);
                return new JsonResult("Exception: " + ex.Message);
            }
        }
    }
}