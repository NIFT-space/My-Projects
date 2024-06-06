using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace Microservices_v1.Connection
{
    public class Conn
    {
        IDbConnection connection = new SqlConnection("Data Source=KHIDEV-ARAZA\\JAFF_INSTANCE;Initial Catalog=NIBC;User ID=ibcs2;Password=J@ffofc1;");
        public T GetConnparam<T>(string sql, DynamicParameters pr)
        {
            var result = connection.Query<T>(sql, pr).FirstOrDefault();
            return result;
        }
        public IEnumerable<T> GetConn<T>(string sql)
        {
            var result = connection.Query<T>(sql);
            return result;
        }
    }
}
