using System;

namespace NIFT_CMS
{
    public partial class EncryptConn : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //connstr.Text = @"Data Source=EPAY-CMS\SQLEXPRESS;Initial Catalog=CMS; User ID=sa;Password=EPAY1234@";
        }

        protected void btn_Encr_Click(object sender, EventArgs e)
        {
            //string connectionstring = ("Data Source = ALIRAZA\\SQLEXPRESS; " + "Initial Catalog = CMS;" + "User id =sa;" + "Password = J@ffofc5;");
            string connectionstring = connstr.Text.Trim();
            EncDec encdec = new EncDec();
            string enc = "";
            //enc = encdec.GetHash(connectionstring);
            enc = encdec.Encrypt(connectionstring);

            encrconnstr.Text = enc;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            string connectionstring = "";
            string constr = encrconnstr.Text;
            EncDec encdec = new EncDec();
            connstr.Text = encdec.decrypt(constr);
        }
    }
}