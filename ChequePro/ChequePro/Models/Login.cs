namespace ChequePro.Models
{
    public class Login
    {
        public string LoginID { get; set; }
        public string Password { get; set; }
        public string CaptchaCode { get; set; }
    }

    public class LoginUser
    {
        public string UserID { get; set; }
        public string Password { get; set; }
    }
    public class FormAccessUser
    {
        public string UserID { get; set; }
        public string FormUrl { get; set; }
    }
    public class UsersFormOptions
    {
        public int OptionID { get; set; }
        public string MenuDisplayText { get; set; }
        public string URL { get; set; }
        public int OrderBy { get; set; }
        public int MenuGroupID { get; set; }
    }

    public class UsersFormOptionsGroup
    {
        public string MenuGroup { get; set; }
        public int MenuGroupID { get; set; }
    }
    public class UsersFOGroup_List
    {
        public List<UsersFormOptionsGroup> lst_UsersFormOptionsGroup { get; set; }
        public List<UsersFormOptions> lst_UsersFormOptions { get; set; }
    }
}
