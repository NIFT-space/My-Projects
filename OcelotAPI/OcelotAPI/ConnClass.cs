using Dapper;
using Microsoft.Data.SqlClient;
using System.Data; // Or your preferred ADO.NET provider
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;

namespace OcelotAPI
{
    public class ConnClass
    {
        IDbConnection connection = new SqlConnection("Data Source=KHIDEV-ARAZA\\JAFF_INSTANCE;Initial Catalog=NIBC;User ID=ibcs2;Password=J@ffofc1;");
        public T GetConn<T>(string sql, DynamicParameters pr)
        {
            var result = connection.Query<T>(sql, pr).FirstOrDefault();
            return result;
        }
    }


    class Getclass
    {
        public void Getbankemail()
        {
            ModelClass mdd = new ModelClass();
            ConnClass cc = new ConnClass();
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@Id", "481");
            mdd = cc.GetConn<ModelClass>("SELECT * FROM bankemail WHERE regId = @Id",dp);
        }
        public void Getusers()
        {
            ModelClass2 mdd2 = new ModelClass2();
            ConnClass cc2 = new ConnClass();
            DynamicParameters dp2 = new DynamicParameters();
            dp2.Add("@Id", "26893");
            mdd2 = cc2.GetConn<ModelClass2>("SELECT * FROM users WHERE userid = @Id", dp2);
        }
    }
    class ModelClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    class ModelClass2
    {
        public int UId { get; set; }
        public string UserName { get; set; }
    }

}
