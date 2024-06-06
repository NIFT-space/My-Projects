namespace Jwttest
{
	public class User
	{
		public string Uid { get; set; }
		public string Pass { get; set; }
		public User(string uid, string pass)
		{
			Uid = uid;
			Pass = pass;
		}
	}
}
