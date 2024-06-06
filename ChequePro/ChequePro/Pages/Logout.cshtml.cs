using ChequePro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ChequePro.Pages
{
    public class LogoutModel : PageModel
    {
        public void OnGet()
        {
            try
            {
                InfoLogger.Log("Logout UserID: " + HttpContext.Session.GetString(IndexModel.SessionKeyName_AuthID) + " : Successfully.");
                HttpContext.Session.Remove(IndexModel.SessionKeyName_AuthID);
                InfoLogger.Log("Logout UserID: " + HttpContext.Session.GetString(IndexModel.SessionKeyName_TokenKey) + " : Successfully.");
                HttpContext.Session.Remove(IndexModel.SessionKeyName_TokenKey);
                InfoLogger.Log("Logout UserID: " + HttpContext.Session.GetString(IndexModel.SessionKeyName_AccountNo) + " : Successfully.");
                HttpContext.Session.Remove(IndexModel.SessionKeyName_AccountNo);
                InfoLogger.Log("Logout UserID: " + HttpContext.Session.GetString(IndexModel.SessionKeyName_BankID) + " : Successfully.");
                HttpContext.Session.Remove(IndexModel.SessionKeyName_BankID);
                InfoLogger.Log("Logout UserID: " + HttpContext.Session.GetString(IndexModel.SessionKeyName_BranchID) + " : Successfully.");
                HttpContext.Session.Remove(IndexModel.SessionKeyName_BranchID);
                HttpContext.Session.Clear();
                Response.Redirect("/Index");
            }
            catch (Exception ex)
            {
                WarningLogger.Log("Logout Page OnGet :: Error Exception : " + ex);
                Response.Redirect("/Index");
            }
        }
    }
}
