
using System.Data.SqlClient;
using System.Data;
using ChequePro.Models;
using ChequePro.Model;


//using Abstract Factory Design Pattern

namespace ChequePro.DAL
{
    public class DBFactory
    {
        protected string m_szError = "";
        protected string? m_szConn;
        protected SqlConnection? m_conn;

        public string LastError
        {
            get { return m_szError; }
        }
        public void SetConnection(string szToken)
        {
            var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            m_szConn = AES_ENCDEC.DecryptString(MyConfig.GetValue<string>("AppSettings:" + szToken));
        }

        public bool ExecuteNonQuery(string sql)
        {
            try
            {
                using (m_conn = new SqlConnection(m_szConn))
                {
                    if (m_conn.State == ConnectionState.Closed) m_conn.Open();
                    using (SqlCommand cmd = new SqlCommand(sql, m_conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log("ExecuteNonQuery :: " + sql + ": error : " + ex);
                m_szError = ex.Message;
                return false;
            }
            finally { if (m_conn.State == ConnectionState.Open) m_conn.Close(); }
        }

        public bool ExecuteNonQuery(string sql, List<SqlParameter> lstParam)
        {
            try
            {
                using (m_conn = new SqlConnection(m_szConn))
                {
                    if (m_conn.State == ConnectionState.Closed) m_conn.Open();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        foreach (SqlParameter param in lstParam)
                        {
                            cmd.Parameters.AddWithValue(param.ParameterName, param.Value);
                        }

                        cmd.CommandText = sql;
                        cmd.Connection = m_conn;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log("ExecuteNonQuery :: " + sql + ": error : " + ex);
                m_szError = ex.Message;
                return false;
            }
            finally { if (m_conn.State == ConnectionState.Open) m_conn.Close(); }
        }

        public bool ExecuteNonQuery(string sql, List<SqlParameter> lstParam, string conn)
        {
            try
            {
                using (SqlConnection m_con = new SqlConnection(conn))
                {
                    if (m_con.State == ConnectionState.Closed) m_con.Open();

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        foreach (SqlParameter param in lstParam)
                        {
                            cmd.Parameters.AddWithValue(param.ParameterName, param.Value);
                        }

                        cmd.CommandText = sql;
                        cmd.Connection = m_con;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.ExecuteNonQuery();
                    }
                    if (m_con.State == ConnectionState.Open) m_con.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log("ExecuteNonQuery :: " + sql + ": error : " + ex);
                m_szError = ex.Message;
                return false;
            }
            //finally { if (m_con.State == ConnectionState.Open) m_con.Close(); }
        }

        public string ExecuteScalar(string sql)
        {
            string retValue = "";
            try
            {
                using (m_conn = new SqlConnection(m_szConn))
                {
                    if (m_conn.State == ConnectionState.Closed) m_conn.Open();
                    using (SqlCommand cmd = new SqlCommand(sql, m_conn))
                    {
                        retValue = cmd.ExecuteScalar().ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log("ExecuteNonQuery :: " + sql + ": error : " + ex);
                m_szError = ex.Message;
                retValue = "Exception";
            }
            finally { if (m_conn.State == ConnectionState.Open) m_conn.Close(); }
            return retValue;
        }

        public object ExecuteScalar(string sql, List<SqlParameter> lstParam)
        {
            object retValue;
            try
            {
                using (m_conn = new SqlConnection(m_szConn))
                {
                    if (m_conn.State == ConnectionState.Closed) m_conn.Open();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        foreach (SqlParameter param in lstParam)
                        {
                            cmd.Parameters.AddWithValue(param.ParameterName, param.Value);
                        }

                        cmd.CommandText = sql;
                        cmd.Connection = m_conn;
                        cmd.CommandType = CommandType.StoredProcedure;

                        retValue = cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log("ExecuteNonQuery :: " + sql + ": error : " + ex);
                m_szError = ex.Message;
                retValue = "Exception";
            }
            finally { if (m_conn.State == ConnectionState.Open) m_conn.Close(); }
            return retValue;
        }

        public object ExecuteScalar(string sql, List<SqlParameter> lstParam, string conn)
        {
            object retValue;
            try
            {
                using (SqlConnection m_con = new SqlConnection(conn))
                {
                    if (m_con.State == ConnectionState.Closed) m_con.Open();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        foreach (SqlParameter param in lstParam)
                        {
                            cmd.Parameters.AddWithValue(param.ParameterName, param.Value);
                        }

                        cmd.CommandText = sql;
                        cmd.Connection = m_con;
                        cmd.CommandType = CommandType.StoredProcedure;

                        retValue = cmd.ExecuteScalar();
                    }
                    if (m_con.State == ConnectionState.Open) m_con.Close();
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log("ExecuteNonQuery :: " + sql + ": error : " + ex);
                m_szError = ex.Message;
                retValue = "Exception";
            }
            //finally { if (m_con.State == ConnectionState.Open) m_con.Close(); }
            return retValue;
        }

        public DataSet ExecuteQuery(string sql)
        {
            DataSet dtRecord = new DataSet();
            try
            {
                using (SqlDataAdapter adp = new SqlDataAdapter(sql, m_szConn))
                {
                    adp.Fill(dtRecord);
                }
            }
            catch (Exception e)
            {
                ExceptionLogger.Log("ExecuteQuery :: " + sql + ": error : " + e);
                m_szError = e.Message;
            }
            return dtRecord;
        }

        public DataSet ExecuteQuery(string sql, List<SqlParameter> lstParam)
        {
            DataSet dtRecord = new DataSet();
            try
            {
                using (m_conn = new SqlConnection(m_szConn))
                {
                    if (m_conn.State == ConnectionState.Closed) m_conn.Open();

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        foreach (SqlParameter param in lstParam)
                        {
                            cmd.Parameters.AddWithValue(param.ParameterName, param.Value);
                        }

                        cmd.CommandText = sql;
                        cmd.Connection = m_conn;
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                        {
                            adp.Fill(dtRecord);
                        }
                    }

                    if (m_conn.State == ConnectionState.Open) m_conn.Close();
                }
            }
            catch (Exception e)
            {
                ExceptionLogger.Log("ExecuteQuery :: " + sql + ": error : " + e);
                m_szError = e.Message;
            }
            return dtRecord;
        }

        public DataSet ExecuteQuery(string sql, List<SqlParameter> lstParam, string conn)
        {
            DataSet dtRecord = new DataSet();
            try
            {
                using (SqlConnection m_con = new SqlConnection(conn))
                {
                    if (m_con.State == ConnectionState.Closed) m_con.Open();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        foreach (SqlParameter param in lstParam)
                        {
                            cmd.Parameters.AddWithValue(param.ParameterName, param.Value);
                        }

                        cmd.CommandText = sql;
                        cmd.Connection = m_con;
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                        {
                            adp.Fill(dtRecord);
                        }
                    }
                    if (m_con.State == ConnectionState.Open) m_con.Close();
                }
            }
            catch (Exception e)
            {
                ExceptionLogger.Log("ExecuteQuery :: " + sql + ": error : " + e);
                m_szError = e.Message;
            }
            return dtRecord;
        }

        

    }
}