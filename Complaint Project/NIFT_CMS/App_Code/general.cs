using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using System.IO;
using System.Xml.Serialization;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
using NIFT_CMS;

/// <summary>
/// Summary description for general
/// </summary>
public class general
{
	public general()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public static bool IsValidEmailAddress(string sEmail)
    {
        if (sEmail == null)
        {
            return false;
        }
        else
        {
            return Regex.IsMatch(sEmail, @"^[-a-zA-Z0-9][-.a-zA-Z0-9]*@[-.a-zA-Z0-9]+(\.[-.a-zA-Z0-9]+)*\.(com|edu|info|gov|int|mil|net|org|biz|name|museum|coop|aero|pro|[a-zA-Z]{2})$",RegexOptions.IgnorePatternWhitespace);
        }
    }

    public static Int64 PageData(string PageName)
    {

        string sSQL = "select * from [page] where [url]='" + PageName.Trim() + "'";

        Int64 retval = -1;

        cDataAccess obj = new cDataAccess();

        //SqlDataReader oDR = obj.GetDataReader(sSQL);

        DataSet DS = obj.GetDataSet(sSQL); 

        foreach (DataRow oDR in DS.Tables[0].Rows)
        {
            retval = (Int64)oDR["PageID"];

            if (retval >= 0)
            {
                return (retval);
            }

        }
        return (retval);

    }


    //public static void UserLog(string UserLogID, string PageID)
    //{
    //    char[] delimiterChars = { '/', ',' };
     
    //    cDataAccess oData = new cDataAccess();
    //    try
    //    {

    //        string sSQL = "INSERT INTO [userlog_page]([UserLogID],[pageid],[timein])";

    //        sSQL += "VALUES(";
    //        sSQL += UserLogID + ",";
    //        sSQL += PageID + ",GetDate()";
    //        //sSQL += DateTime.Now() + "'";
    //        sSQL += ")";

    //        oData.RunProc(sSQL);
    //    }
    //    catch (Exception)
    //    {
    //        string sSQL = "INSERT INTO [userlog_page2]([UserLogID],[pageid],[timein])";

    //        sSQL += "VALUES(";
    //        sSQL += UserLogID + ",";
    //        sSQL += PageID + ",GetDate()";
    //        //sSQL += DateTime.Now() + "'";
    //        sSQL += ")";

    //        oData.RunProc(sSQL);      
        
        
    //    }


    //}

    //public static bool isPageAllowed(string UserID, string PageID)
    //{
    //    try
    //    {
    //        cDataAccess oData = new cDataAccess();
    //        string sSQL = "select b.* from [role] a,[role_page] b,[user_role] c,[users] d where a.roleid=b.roleid and a.roleid=c.roleid and b.roleid=c.roleid and d.userid=c.userid and  d.userid="+ UserID + " and b.pageid=" + PageID;
            
    //        cDataAccess obj = new cDataAccess();
    //        DataSet oDs = obj.GetDataSet(sSQL);

    //        if (oDs.Tables[0].Rows.Count > 0)
    //        {
    //            bool IsPageAllow = true;
    //            return IsPageAllow;
    //        }
    //        else
    //        {
    //            bool IsPageAllow = false;
    //            return IsPageAllow;
    //        }
    //    }
    //    catch (SqlException ex)
    //    {

    //        bool IsPageAllow = false;
    //        return IsPageAllow;
    //    }
    //}

    public static bool isPageAllowed(string UserID, string PageName)
    {
        //string sSQL = "select [url] from [users] a,user_role b,[role] c,[roletype] d , role_page e,page f where a.userid=b.userid and b.RoleID=c.RoleID and c.roletypeid=d.roletypeid and  a.UserID='" + UserID.Trim() + "' and a.isactive=1 and e.roleid=c.roleid and e.pageid=f.pageid and lower(ltrim(rtrim(f.URL))) like '" + PageName.Trim() + "%'";

        string sSQL = "Select p.URL as URL from users a ";
        sSQL += "join User_Role b on b.UserID = a.UserID ";
        sSQL += "join Role_Page c on c.RoleID = b.RoleID ";
        sSQL += "join Page p on p.PageID = c.PageID ";
        sSQL += "where a.UserID = '" + UserID.Trim() + "' and lower(ltrim(rtrim(p.title))) like '" + PageName.Trim() + "%'";

        bool retval = false;
        cDataAccess obj = new cDataAccess();
        DataSet oDS = obj.GetDataSet(sSQL);

        if (oDS.Tables[0].Rows.Count > 0)
        {
            retval = true;
        }
        else
        {
            retval = false;
        }
        return retval;
    }



    public static void GenerateXMLFile(string sFileName,string UserID)
{

    cDataAccess oData = new cDataAccess();

    //Create XML
    string sSQL = "select e.* from [role] a,[role_page] b,[user_role] c,[users] d,page e where a.roleid=b.roleid and a.roleid=c.roleid and b.roleid=c.roleid and d.userid=c.userid and  d.userid=18 and b.pageid=e.pageid ";
    SqlDataReader reader1 = oData.GetDataReader(sSQL);

    Encoding enc = Encoding.UTF8;
    

    XmlTextWriter objXMLTW = new XmlTextWriter(sFileName, enc);
    try
    {          
        objXMLTW.WriteStartDocument();//xml document open
        //'Top level (Parent element)
        //root node open
        objXMLTW.WriteStartElement("siteMap");
        //first Node of the Menu open
        objXMLTW.WriteStartElement("siteMapNode");
        //Title attribute set
        objXMLTW.WriteAttributeString("title", "Home");
        objXMLTW.WriteAttributeString("description", 
                 "This is home");//Description attribute set
        objXMLTW.WriteAttributeString("url", 
                 "http://www.home.com");//URL attribute set
        //Loop and create nodes
        while (reader1.Read())
        {
            Int64  MasterID = reader1.GetInt64(0);
            objXMLTW.WriteStartElement("siteMapNode");
            objXMLTW.WriteAttributeString("title", 
                            reader1.GetString(1));
            objXMLTW.WriteAttributeString("description", 
                                  Convert.ToString(reader1["Description"]));
            objXMLTW.WriteAttributeString("url", 
                          reader1.GetString(2));
            
            objXMLTW.WriteEndElement();//Close the siteMapNode
        }
        objXMLTW.WriteEndElement();//Close the first siteMapNode
        objXMLTW.WriteEndDocument();//xml document closed
    }
    finally
    {
        objXMLTW.Flush();
        objXMLTW.Close();
    }
}

    public static String GetReportID(string RepName)
    {

        string retval = "";
        string sSQL = "select * from report where repname='" + RepName + "' and repversion=(select ";
        sSQL += " max(repversion) from report where repname='" + RepName +"')";

        cDataAccess obj = new cDataAccess();
        //SqlDataReader oDR = obj.GetDataReader(sSQL);
        DataSet DS = obj.GetDataSet(sSQL);

        foreach (DataRow oDR in DS.Tables[0].Rows)
        {
            retval = Convert.ToString(oDR["RepID"]);

            if (retval != "")
            {
                return (retval);
            }

        }
        return (retval);

    }

    public static String GetCity(string UserID)
    {

        string retval = "";
        string sSQL = " select distinct e.cityid CityID from [users] a,user_role b,[role] c,[roletype] d ,";
        sSQL += " role_city e where  a.userid=b.userid and  b.RoleID=c.RoleID and  ";
        sSQL += " c.roletypeid=d.roletypeid and   b.roleid=e.roleid and c.roleid=e.roleid and  ";
        sSQL += " a.UserID=" + UserID;

        cDataAccess obj = new cDataAccess();
        //SqlDataReader oDR = obj.GetDataReader(sSQL);
        DataSet DS = obj.GetDataSet(sSQL);
        if (DS.Tables[0].Rows.Count > 0)
        {
            foreach (DataRow oDR in DS.Tables[0].Rows)
            {
                if (retval.Trim() != "")
                {
                    retval = retval + "," + Convert.ToString(oDR["CityID"]).Trim();
                }
                if (Convert.ToString(oDR["CityID"]).Trim() == "0")
                {
                    retval = "0";

                }
                if(retval.Trim() =="")
                {
                    retval = Convert.ToString(oDR["CityID"]).Trim();
                }
            }
        }
        else
        {
            retval = "0";
        }
        return (retval);
    }

    public static String GetCityCount(string UserID)
    {
        string retval = "";
        string sSQL = "";
        if (!isBranchUser(UserID))
        {   
            sSQL = " select count(e.cityid) cityno from [users] a,user_role b,[role] c,[roletype] d ,";
            sSQL += " role_city e where  a.userid=b.userid and  b.RoleID=c.RoleID and  ";
            sSQL += " c.roletypeid=d.roletypeid and   b.roleid=e.roleid and c.roleid=e.roleid and  ";
            sSQL += " a.UserID=" + UserID;
        }
        else
        {
            sSQL = " select count(e.cityid) cityno from [users] a,user_role b,[role] c,[roletype] d ,";
            sSQL += " role_city e where  a.userid=b.userid and  b.RoleID=c.RoleID and  ";
            sSQL += " c.roletypeid=d.roletypeid and   b.roleid=e.roleid and c.roleid=e.roleid and  ";
            sSQL += " a.UserID=" + UserID;
        }
        cDataAccess obj = new cDataAccess();
   
        DataSet DS = obj.GetDataSet(sSQL);

        foreach (DataRow oDR in DS.Tables[0].Rows)
        {
            retval = Convert.ToString(oDR["cityno"]).Trim();
        }

        return (retval);
    }


    public static String GetInstitute(string UserID)
    {

        string retval = "";
        bool zeroflag = false;
        string sSQL = "select distinct e.Instid InstID from [users] a,user_role b,[role] c,[roletype] d ,";
        sSQL += " role_inst_branch e where  a.userid=b.userid and  b.RoleID=c.RoleID and  ";
        sSQL += " c.roletypeid=d.roletypeid and   b.roleid=e.roleid and c.roleid=e.roleid and  ";
        sSQL += " a.UserID=" + UserID ;

        cDataAccess obj = new cDataAccess();
        //SqlDataReader oDR = obj.GetDataReader(sSQL);
        DataSet DS = obj.GetDataSet(sSQL);  

        foreach (DataRow oDR in DS.Tables[0].Rows)
        {
            if (retval.Trim()!="")
            {
            retval = retval + "," +  Convert.ToString(oDR["InstID"]).Trim();
            }
            if (Convert.ToString(oDR["InstID"]).Trim() == "0")
            {
                retval = "0";
                zeroflag = true;
            }

            else
            {
                retval = Convert.ToString(oDR["InstID"]).Trim();
            }
        }

        if ((retval.Trim() == "0")||(zeroflag==true))
        {
            sSQL = "select distinct Instid InstID from institution";

            //SqlDataReader oDR2 = obj.GetDataReader(sSQL);
            DataSet DS2 = obj.GetDataSet(sSQL);

            foreach (DataRow oDR2 in DS2.Tables[0].Rows)
            {
                if (retval.Trim() != "")
                {
                    retval = retval + "," + Convert.ToString(oDR2["InstID"]).Trim();
                }
                else
                {
                    retval = Convert.ToString(oDR2["InstID"]).Trim();
                }
            }

        }
        return (retval);

    }

    public static String GetInstituteBranch(string UserID)
    {
        string sSQL = "";
        if (isBranchUser(UserID) == false)
        {
            string BankID = "";
            string BranchID = "";

            sSQL = "select e.branchid,e.instid ";
            sSQL += " from [users] a,user_role b,[role] c,[roletype] d , role_inst_branch e ";
            sSQL += " where  a.userid=b.userid and  b.RoleID=c.RoleID and  c.roletypeid=d.roletypeid ";
            sSQL += " and   b.roleid=e.roleid and c.roleid=e.roleid and  a.UserID=" + UserID;

            //sSQL=" select e.branchid,e.instid,f.branch_name,f.cityid  from [users] a,user_role b,[role] c,[roletype] d , ";
            //sSQL += " role_inst_branch e,branch f  where  a.userid=b.userid and  b.RoleID=c.RoleID and   ";
            //sSQL += " c.roletypeid=d.roletypeid  and   b.roleid=e.roleid and c.roleid=e.roleid and e.branchid=f.branchid and e.instid=f.instid ";
            //sSQL += "  and  a.UserID=" + UserID;


            cDataAccess obj = new cDataAccess();
            //SqlDataReader oDR = obj.GetDataReader(sSQL);
            DataSet DS = obj.GetDataSet(sSQL);

            foreach (DataRow oDR in DS.Tables[0].Rows)
            {
                if (BranchID.Trim() != "")
                {
                    BranchID = BranchID + "," + Convert.ToString(oDR["BranchID"]).Trim();
                }
                else
                {
                    BankID = Convert.ToString(oDR["InstID"]).Trim();
                    BranchID = Convert.ToString(oDR["BranchID"]).Trim();
                }
            }

            if (BankID.Trim() == "0")
            {
                sSQL = "select InstID,Branchid,branch_name,cityid from branch";

            }
            if ((BankID.Trim() != "0") && (BranchID.Trim() == "0"))
            {
                sSQL = "select InstID,Branchid ,branch_name,cityid from branch where InstID=" + BankID.Trim();
            }
            if ((BankID.Trim() != "0") && (BranchID.Trim() != "0"))
            {
                //sSQL = "select e.branchid,e.instid ";
                //sSQL += " from [users] a,user_role b,[role] c,[roletype] d , role_inst_branch e ";
                //sSQL += " where  a.userid=b.userid and  b.RoleID=c.RoleID and  c.roletypeid=d.roletypeid ";
                //sSQL += " and   b.roleid=e.roleid and c.roleid=e.roleid and  a.UserID=" + UserID;

                sSQL = " select e.branchid,e.instid,f.branch_name,f.cityid  from [users] a,user_role b,[role] c,[roletype] d , ";
                sSQL += " role_inst_branch e,branch f  where  a.userid=b.userid and  b.RoleID=c.RoleID and   ";
                sSQL += " c.roletypeid=d.roletypeid  and   b.roleid=e.roleid and c.roleid=e.roleid and e.branchid=f.branchid and e.instid=f.instid ";
                sSQL += "  and  a.UserID=" + UserID;


            }
        }
        else
        {
            
            sSQL=" select a.branchid,a.instid,a.locality cityid,b.branch_name from [users] a,branch b ";
            sSQL += " where a.userid=" + UserID + " and a.branchid=b.branchid and a.instid=b.instid and a.locality=b.cityid ";

        }
        return (sSQL);

    }


    public static String GetInstituteBranch(string UserID,string CityID)
    {
        string sSQL = "";
        if (isBranchUser(UserID) == false)
        {
            string BankID = "";
            string BranchID = "";

            sSQL = "select e.branchid,e.instid ";
            sSQL += " from [users] a,user_role b,[role] c,[roletype] d , role_inst_branch e ";
            sSQL += " where  a.userid=b.userid and  b.RoleID=c.RoleID and  c.roletypeid=d.roletypeid ";
            sSQL += " and   b.roleid=e.roleid and c.roleid=e.roleid and  a.UserID=" + UserID;
            
        

            cDataAccess obj = new cDataAccess();
            //SqlDataReader oDR = obj.GetDataReader(sSQL);
            DataSet DS = obj.GetDataSet(sSQL);

            foreach (DataRow oDR in DS.Tables[0].Rows)
            {
                if (BranchID.Trim() != "")
                {
                    BranchID = BranchID + "," + Convert.ToString(oDR["BranchID"]).Trim();
                }
                else
                {
                    BankID = Convert.ToString(oDR["InstID"]).Trim();
                    BranchID = Convert.ToString(oDR["BranchID"]).Trim();
                }
            }

            if (BankID.Trim() == "0")
            {
                sSQL = "select InstID,Branchid,branch_name,cityid from branch where cityid="+ CityID ;

            }
            if ((BankID.Trim() != "0") && (BranchID.Trim() == "0"))
            {
                sSQL = "select InstID,Branchid ,branch_name,cityid from branch where InstID=" + BankID.Trim() + " and CityID=" + CityID;
            }
            if ((BankID.Trim() != "0") && (BranchID.Trim() != "0"))
            {
                //sSQL = "select e.branchid,e.instid ";
                //sSQL += " from [users] a,user_role b,[role] c,[roletype] d , role_inst_branch e ";
                //sSQL += " where  a.userid=b.userid and  b.RoleID=c.RoleID and  c.roletypeid=d.roletypeid ";
                //sSQL += " and   b.roleid=e.roleid and c.roleid=e.roleid and  a.UserID=" + UserID;

                sSQL = " select e.branchid,e.instid,f.branch_name,f.cityid  from [users] a,user_role b,[role] c,[roletype] d , ";
                sSQL += " role_inst_branch e,branch f  where  a.userid=b.userid and  b.RoleID=c.RoleID and   ";
                sSQL += " c.roletypeid=d.roletypeid  and   b.roleid=e.roleid and c.roleid=e.roleid and e.branchid=f.branchid and e.instid=f.instid ";
                sSQL += " and  a.UserID=" + UserID + " and f.CityID=" + CityID;


            }
        }
        else
        {

            sSQL = " select a.branchid,a.instid,a.locality cityid,b.branch_name from [users] a,branch b ";
            sSQL += " where a.userid=" + UserID + " and a.branchid=b.branchid and a.locality=b.cityid and a.instid=b.instid and b.CityID=" + CityID;

        }
        return (sSQL);

    }

    public static String GetInstituteBranch(string UserID, string CityID,string InstID)
    {
        string sSQL = "";
        if (isBranchUser(UserID) == false)
        {
            string BankID = "";
            string BranchID = "";

            sSQL = "select e.branchid,e.instid ";
            sSQL += " from [users] a,user_role b,[role] c,[roletype] d , role_inst_branch e ";
            sSQL += " where  a.userid=b.userid and  b.RoleID=c.RoleID and  c.roletypeid=d.roletypeid ";
            sSQL += "  and   b.roleid=e.roleid and c.roleid=e.roleid and  a.UserID=" + UserID;



            cDataAccess obj = new cDataAccess();
            //SqlDataReader oDR = obj.GetDataReader(sSQL);
            DataSet DS = obj.GetDataSet(sSQL);

            foreach (DataRow oDR in DS.Tables[0].Rows)
            {
                if (BranchID.Trim() != "")
                {
                    BranchID = BranchID + "," + Convert.ToString(oDR["BranchID"]).Trim();
                }
                else
                {
                    BankID = Convert.ToString(oDR["InstID"]).Trim();
                    BranchID = Convert.ToString(oDR["BranchID"]).Trim();
                }
            }

            if (BankID.Trim() == "0")
            {
                sSQL = "select InstID,Branchid,branch_name,cityid from branch where cityid=" + CityID + " and instid=" + InstID + " and isopen=1 order by branch_name asc";

            }
            if ((BankID.Trim() != "0") && (BranchID.Trim() == "0"))
            {
                sSQL = "select InstID,Branchid ,branch_name,cityid from branch where InstID=" + BankID.Trim() + " and CityID=" + CityID + " and isopen=1  order by branch_name asc";
            }
            if ((BankID.Trim() != "0") && (BranchID.Trim() != "0"))
            {
                //sSQL = "select e.branchid,e.instid ";
                //sSQL += " from [users] a,user_role b,[role] c,[roletype] d , role_inst_branch e ";
                //sSQL += " where  a.userid=b.userid and  b.RoleID=c.RoleID and  c.roletypeid=d.roletypeid ";
                //sSQL += " and   b.roleid=e.roleid and c.roleid=e.roleid and  a.UserID=" + UserID;

                sSQL = " select e.branchid,e.instid,f.branch_name,f.cityid  from [users] a,user_role b,[role] c,[roletype] d , ";
                sSQL += " role_inst_branch e,branch f  where  a.userid=b.userid and  b.RoleID=c.RoleID and   ";
                sSQL += " c.roletypeid=d.roletypeid  and isopen=1  and   b.roleid=e.roleid and c.roleid=e.roleid and e.branchid=f.branchid and e.instid=f.instid ";
                sSQL += " and  a.UserID=" + UserID + " and f.CityID=" + CityID + " order by branch_name asc";


            }
        }
        else
        {

            sSQL = " select a.branchid,a.instid,a.locality cityid,b.branch_name from [users] a,branch b ";
            sSQL += " where a.userid=" + UserID + " and b.isopen=1 and a.branchid=b.branchid and a.locality=b.cityid and a.instid=b.instid and b.CityID=" + CityID + " and b.instid=" + InstID + " order by branch_name asc";

        }
        return (sSQL);

    }

    public static String GetInstituteHub(string UserID, string CityID, string InstID)
    {
        string sSQL = "";
        if (isBranchUser(UserID) == false)
        {
             string Hub = "";

            sSQL = " select instid,hubcode from role_image_inst_hub a,[role] b,[user_role] c ";
            sSQL += " where a.roleid=b.roleid and b.roleid=c.roleid and c.userid=" + UserID;            

            cDataAccess obj = new cDataAccess();
 
            DataSet DS = obj.GetDataSet(sSQL);

            foreach (DataRow oDR in DS.Tables[0].Rows)
            {
                if (InstID.Trim() != "")
                {
                    if (Hub=="")
                        Hub =Convert.ToString(oDR["HubCode"]).Trim();
                    else
                        Hub = Hub + "," + Convert.ToString(oDR["HubCode"]).Trim();
                }
                else
                {
                    InstID = Convert.ToString(oDR["InstID"]).Trim();
                    Hub = Convert.ToString(oDR["HubCode"]).Trim();
                }
            }

            if (InstID.Trim() == "0")
            {
                sSQL = "select distinct InstID,hubcode,hubname from hub where isactive=1 order by hubname asc";

            }
            if ((InstID.Trim() != "0") && (Hub.Trim() == "0"))
            {
                sSQL = "select distinct InstID,hubcode,hubname from hub where  instid="+ InstID + " and isactive=1 order by hubname asc";
            } 
            if ((InstID.Trim() != "0") && (Hub.Trim() != "0"))
            {              
                sSQL = " select distinct a.hubcode,a.hubname, ";
                sSQL += " (select instname from institution i ";
                sSQL += " where i.instid=a.instid) instname  ";
                sSQL += " from hub a,role_image_inst_hub b,[role] c,users d,user_role e ";
                sSQL += " where ";
                sSQL += " a.hubcode=b.hubcode ";
                sSQL += " and  ";
                sSQL += " a.instid=b.instid ";
                sSQL += " and ";
                sSQL += " e.roleid=c.roleid ";
                sSQL += " and ";
                sSQL += " e.userid=d.userid ";
                sSQL += " and ";
                sSQL += " b.roleid=c.roleid ";
                sSQL += " and ";
                sSQL += " b.roleid=e.roleid ";
                sSQL += " and ";
                sSQL += " d.userid=" + UserID;
                sSQL += " and ";
                sSQL += " e.userid=" + UserID;
                //sSQL += " and a.cityid=" + CityID;
                sSQL += " and a.instid=" + InstID;

            }
        }
        else
        {

            sSQL = " select distinct a.hubcode,a.hubname, ";
            sSQL += " (select instname from institution i ";
            sSQL += " where i.instid=a.instid) instname  ";
            sSQL += " from hub a,role_image_inst_hub b,[role] c,users d,user_role e ";
            sSQL += " where ";
            sSQL += " a.hubcode=b.hubcode ";
            sSQL += " and  ";
            sSQL += " a.instid=b.instid ";
            sSQL += " and ";
            sSQL += " e.roleid=c.roleid ";
            sSQL += " and ";
            sSQL += " e.userid=d.userid ";
            sSQL += " and ";
            sSQL += " b.roleid=c.roleid ";
            sSQL += " and ";
            sSQL += " b.roleid=e.roleid ";
            sSQL += " and ";
            sSQL += " d.userid=" + UserID;
            sSQL += " and ";
            sSQL += " e.userid=" + UserID;
            sSQL += " and ";
            //sSQL += " a.cityid=" + CityID;
            sSQL += " and a.instid=" + InstID;


        }
        return (sSQL);

    }






    public static String GetInstituteBranchURL(string UserID, string URL)
    {
        string sSQL = "";
        if (isBranchUser(UserID) == false)
        {
            string BankID = "";
            string BranchID = "";

            sSQL = "select e.branchid,e.instid ";
            sSQL += " from [users] a,user_role b,[role] c,[roletype] d , role_inst_branch e ";
            sSQL += " where  a.userid=b.userid and  b.RoleID=c.RoleID and  c.roletypeid=d.roletypeid ";
            sSQL += " and   b.roleid=e.roleid and c.roleid=e.roleid and  a.UserID=" + UserID;

            //NEW CODE STARTS HERE
            //sSQL = "select e.branchid,e.instid,f.branch_name,f.cityid  from [users] a, ";
            //sSQL += " user_role b,[role] c,[roletype] d ,  role_inst_branch e,branch f,role_page g,page h ";
            //sSQL += " where  a.userid=b.userid ";
            //sSQL += " and c.roleid=g.roleid ";
            //sSQL += " and  b.RoleID=c.RoleID and ";
            //sSQL += " c.roletypeid=d.roletypeid  and   b.roleid=e.roleid and ";
            //sSQL += " c.roleid=e.roleid and e.branchid=f.branchid and ";
            //sSQL += " e.instid=f.instid   and  a.UserID=" + UserID;
            //sSQL += " and b.roleid=g.roleid";
            //sSQL += " and g.pageid=h.pageid";
            //sSQL += " and url like '%" + URL + "%'";



            cDataAccess obj = new cDataAccess();
            //SqlDataReader oDR = obj.GetDataReader(sSQL);
            DataSet DS = obj.GetDataSet(sSQL);

            foreach (DataRow oDR in DS.Tables[0].Rows)
            {
                if (BranchID.Trim() != "")
                {
                    BranchID = BranchID + "," + Convert.ToString(oDR["BranchID"]).Trim();
                }
                else
                {
                    BankID = Convert.ToString(oDR["InstID"]).Trim();
                    BranchID = Convert.ToString(oDR["BranchID"]).Trim();
                }
            }

            if (BankID.Trim() == "0")
            {
                sSQL = "select InstID,Branchid,branch_name,cityid from branch";

            }
            if ((BankID.Trim() != "0") && (BranchID.Trim() == "0"))
            {
                sSQL = "select InstID,Branchid ,branch_name,cityid from branch where InstID=" + BankID.Trim();
            }
            if ((BankID.Trim() != "0") && (BranchID.Trim() != "0"))
            {

                //sSQL = " select e.branchid,e.instid,f.branch_name,f.cityid  from [users] a,user_role b,[role] c,[roletype] d , ";
                //sSQL += " role_inst_branch e,branch f  where  a.userid=b.userid and  b.RoleID=c.RoleID and   ";
                //sSQL += " c.roletypeid=d.roletypeid  and   b.roleid=e.roleid and c.roleid=e.roleid and e.branchid=f.branchid and e.instid=f.instid ";
                //sSQL += "  and  a.UserID=" + UserID;
                sSQL = "select e.branchid,e.instid,f.branch_name,f.cityid  from [users] a, ";
                sSQL += " user_role b,[role] c,[roletype] d ,  role_inst_branch e,branch f,role_page g,page h ";
                sSQL += " where  a.userid=b.userid ";
                sSQL += " and c.roleid=g.roleid ";
                sSQL += " and  b.RoleID=c.RoleID and ";
                sSQL += " c.roletypeid=d.roletypeid  and   b.roleid=e.roleid and ";
                sSQL += " c.roleid=e.roleid and e.branchid=f.branchid and ";
                sSQL += " e.instid=f.instid   and  a.UserID=" + UserID;
                sSQL += " and b.roleid=g.roleid";
                sSQL += " and g.pageid=h.pageid";
                sSQL += " and url like '%" + URL + "%'";


            }
        }
        else
        {

            sSQL = " select a.branchid,a.instid,a.locality cityid,b.branch_name from [users] a,branch b ";
            sSQL += " where a.userid=" + UserID + " and a.branchid=b.branchid and a.locality=b.cityid and a.instid=b.instid ";

        }
        return (sSQL);

    }



    public static String SQLEncode(String sSQL)
      {
          String Result="";
        if (string.IsNullOrEmpty(sSQL.Trim())!=false)
            return Result;
        else
            return sSQL.Replace("'", "''");
        
      }

   public static bool LoginUser(string UserID, string Password)
    {

        try
        {

            cDataAccess oData = new cDataAccess();

            string sSQL = "select * from [users] a where ((username='" + UserID.Trim()

           + "') and (password='" + Password.Trim() + "') and a.isactive=1)";



            cDataAccess obj = new cDataAccess();
            DataSet oDs = obj.GetDataSet(sSQL);

            if (oDs.Tables[0].Rows.Count > 0)
            {
                bool IsValidUser = true;
                return IsValidUser;
            }
            else
            {
                bool IsValidUser = false;
                return IsValidUser;
            }
        }
        catch (SqlException)
        {

            bool IsValidUser = false;
            return IsValidUser;
        }

    }

    public static String GetFromIntlDate(String DDMMYYYY)
    {
        if (isIntlDate(DDMMYYYY, "-") == false)
        {
            char[] delimiterChars = { '/', '-' };
            string DD = "", MM = "", YYYY = "";
            string[] DateArray = DDMMYYYY.Split(delimiterChars);
            DD = DateArray[0];
            MM = DateArray[1];
            YYYY = DateArray[2];
            return (YYYY + "-" + MM + "-" + DD + " 00:00:00");
        }
        else
        {
            return (DDMMYYYY + " 00:00:00");
        }
    }
    public static String GetToIntlDate(String DDMMYYYY)
    {
        if (isIntlDate(DDMMYYYY, "-") == false)
        {
            char[] delimiterChars = { '/', '-' };
            string DD = "", MM = "", YYYY = "";
            string[] DateArray = DDMMYYYY.Split(delimiterChars);
            DD = DateArray[0];
            MM = DateArray[1];
            YYYY = DateArray[2];
            return (YYYY + "-" + MM + "-" + DD + " 23:59:00");
        }
        else
        {
            return (DDMMYYYY + " 23:59:00");
        }
    }

    public static String GetFromIntlDate()
    {
        string FromDate="";

        FromDate = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day +" 00:00:00";

        return FromDate;
    }

    public static String GetToIntlDate()
    {
        string ToDate="";

        ToDate = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " 23:59:00";              

        return ToDate;
    }
    public static String GetOrigDate(String YYYYMMDD)
    {
        char[] delimiterChars = { '-',' ',':'};
        
        string DD = "", MM = "", YYYY = "";

        string[] DateArray = YYYYMMDD.Split(delimiterChars);
        YYYY = DateArray[0];
        MM = DateArray[1];
        DD = DateArray[2];

        if (MM.Length == 1)
            MM = "0" + MM.ToString();

        if (DD.Length == 1)
            DD = "0" + DD.ToString(); 
 
        return (DD + "-"+ MM + "-" + YYYY);

    }
    public static Boolean  isIntlDate(String DateString,String Delimiter)
    {
      Int32 DelimiterPlace =-1;
      DelimiterPlace = DateString.IndexOf(Delimiter);
      if (DelimiterPlace > 2)
      {
          return true;
      }
      else
      {
          return false;
      }
        

    }

    public static bool isBranchUser(string UserID)
    {

        try
        {

            cDataAccess oData = new cDataAccess();
            Boolean isUser=false;


            string sSQL = "select isBranchUser from [users] a where (a.UserID=" + UserID.Trim()+ ")";

            cDataAccess obj = new cDataAccess();
            DataSet Ds = obj.GetDataSet(sSQL);
            
            foreach (DataRow oDR in Ds.Tables[0].Rows)
            {
                isUser = Convert.ToBoolean(oDR["isBranchUser"]);
            }
            return isUser; 
        }
        catch (SqlException)
        {

            bool isUser = false;
            return isUser;
        }

    }

  }
