using ChequePro.DAL;
using ChequePro.Models;
using ChequePro.Pages.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ChequePro.Pages.Cheques
{
    public class ChequeSummaryModel : PageModel
    {
        public string FormUrl { get; set; }
        private readonly ILogger<ChequeSummaryModel> _logger;
        public List<ChequeSummary> lstCheques { get; set; }

        public ChequeSummaryModel(ILogger<ChequeSummaryModel> logger)
        {
            _logger = logger;
            FormUrl = "/Home/ChequeSummary";
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
                    lstCheques = new List<ChequeSummary>();
                    Response.Redirect("/Index");
                }
                else
                {
                    DBFactoryChequePro db = new DBFactoryChequePro();
                    lstCheques = db.get_chequeSummary(Convert.ToInt64(AccountNo), Convert.ToInt32(BankID), Convert.ToInt32(BranchID));
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log(FormUrl + " :: error Exception: " + ex);
                lstCheques = new List<ChequeSummary>();
            }
        }
    }
}
