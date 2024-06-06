using ChequePro.DAL;
using ChequePro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ChequePro.Pages.Home
{
    public class DashboardModel : PageModel
    {
        public string FormUrl { get; set; }
        private readonly ILogger<DashboardModel> _logger;

        public DashboardModel(ILogger<DashboardModel> logger)
        {
            _logger = logger;
            FormUrl = "/Home/Dashboard";
        }
        public void OnGet()
        {
            try
            {
                string AuthID = HttpContext.Session.GetString(IndexModel.SessionKeyName_AuthID);
                string TokenKey = HttpContext.Session.GetString(IndexModel.SessionKeyName_TokenKey);
                string BankID = HttpContext.Session.GetString(IndexModel.SessionKeyName_BankID);
                string BranchID = HttpContext.Session.GetString(IndexModel.SessionKeyName_BranchID);
                string AccountNo = HttpContext.Session.GetString(IndexModel.SessionKeyName_AccountNo);

                if (string.IsNullOrEmpty(AuthID))
                {
                    Response.Redirect("/Index");
                }
                else
                {
                    //DBFactoryChequePro db = new DBFactoryChequePro();
                    //lstCheques = db.get_chequeSummary(Convert.ToInt64(AccountNo), Convert.ToInt32(BankID), Convert.ToInt32(BranchID));
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log(FormUrl + " :: error Exception: " + ex);
            }
        }
    }
}
