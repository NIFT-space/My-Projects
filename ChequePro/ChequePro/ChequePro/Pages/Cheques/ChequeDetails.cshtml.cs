using ChequePro.DAL;
using ChequePro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ChequePro.Pages.Cheques
{
    public class ChequeDetailsModel : PageModel
    {
        public string FormUrl { get; set; }
        private readonly ILogger<ChequeDetailsModel> _logger;

        public ChequeDetails objChequesDetail { get; set; }
        public ChequeDetailsModel(ILogger<ChequeDetailsModel> logger)
        {
            _logger = logger;
            FormUrl = "/Home/ChequeDetails";
        }

        public void OnGet(long? id)
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
                    objChequesDetail = new ChequeDetails();
                    Response.Redirect("/Index");
                }
                else
                {
                    DBFactoryChequePro db = new DBFactoryChequePro();
                    objChequesDetail = db.get_chequeDetails(id);
                }

            }
            catch (Exception ex)
            {
                ExceptionLogger.Log(FormUrl + " :: error Exception: " + ex);
                objChequesDetail = new ChequeDetails();
            }
        }

        
    }
}
