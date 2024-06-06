using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
namespace NIFT_CMS
{
    public class cDataAccess
    {
        //**************************************
        //* Purpose: Accessing SQL database 
        //*Methods:
        //*GetDataSet
        //*RunProc
        //*GetDataReader
        //*GetDataView
        //*Created By
        //* *************************************  

        public string InitializeConnection()
        {
            string connectionstring = "";
            string constr = ConfigurationManager.AppSettings["connectionstring"];
            EncDec encdec = new EncDec();
            connectionstring = encdec.decrypt(constr);
            //connectionstring = constr;
            return connectionstring;
        }

        public SqlConnection GetConnection()
        {
            SqlConnection oCon = new SqlConnection();
            string connectionstring = "";
            string constr = ConfigurationManager.AppSettings["connectionstring"];
            EncDec encdec = new EncDec();
            connectionstring = encdec.decrypt(constr);
            //connectionstring = constr;
            oCon.ConnectionString = connectionstring;
            oCon.Open();
            return oCon;
        }


        public void CloseConnection(SqlConnection oCon)
        {
            oCon.Close();

        }
        public SqlDataAdapter GetDataAdapter(String sSQL)
        {
            //********************************
            //* Purpose: Returns Dataset for one or multi datatables 
            //* Input parameters:
            //*strConnect----Connection string
            //*ProcName() ---StoredProcedures name in array
            //*DataTable()---DataTable name in array
            //* Returns :
            //*DataSet Object contains data
            //**************************************************
            SqlConnection conn = GetConnection();
            SqlDataAdapter oDA;
            try
            {

                oDA = new SqlDataAdapter(sSQL, conn);
                CloseConnection(conn);
                return oDA;
                oDA.Dispose();

            }
            catch (SqlException objError)
            {
                //write error to the windows event log                  

                CloseConnection(conn);
                throw;
            }

        }
        public DataSet GetDataSet(string ProcName)
        {
            //********************************
            //* Purpose: Returns Dataset for one or multi datatables 
            //* Input parameters:
            //*strConnect----Connection string
            //*ProcName() ---StoredProcedures name in array
            //*DataTable()---DataTable name in array
            //* Returns :
            //*DataSet Object contains data
            //**************************************************
            DataSet dstEorder;
            SqlConnection conn = GetConnection();
            SqlDataAdapter dadEorder;
            try
            {
                //int intCnt = ProcName.GetUpperBound(0);
                dstEorder = new DataSet();
                // conn = GetConnection();
                dadEorder = new SqlDataAdapter(ProcName, conn);
                dadEorder.Fill(dstEorder);
                CloseConnection(conn);
                return dstEorder;
                //    dadEorder.Dispose();  

            }
            catch (SqlException objError)
            {
                //write error to the windows event log                  

                CloseConnection(conn);
                throw;
            }


        }
        public SqlDataReader GetDataReader(string ProcName)
        {
            //**************************************
            //* Purpose: Getting DataReader for the given Procedure
            //* Input parameters:
            //*strConnect----Connection string
            //*ProcName ---StoredProcedures name
            //* Returns :
            //*DataReader contains data
            //* ************************************

            string strCommandText = ProcName;
            SqlDataReader objDataReader;
            //create a new Connection object using the connection string
            SqlConnection objConnect = GetConnection();
            //create a new Command using the CommandText and Connection object
            SqlCommand objCommand = new SqlCommand(strCommandText, objConnect);
            try
            {
                //open the connection and execute the command
                //objConnect.Open();
                //objDataAdapter.SelectCommand = objCommand
                objDataReader = objCommand.ExecuteReader(CommandBehavior.CloseConnection);
                return objDataReader;
                objDataReader.Dispose();
                objCommand.Dispose();
            }
            catch (SqlException objError)
            {
                //write error to the windows event log
                CloseConnection(objConnect);
                throw;
            }
            finally
            {
                objConnect.Close();
            }
        }
        public object RunProc(string ProcName)
        {
            //****************************************
            //* Purpose: Executing  Stored Procedures where UPDATE, INSERT
            //*and DELETE statements are expected but does not 
            //*work for select statement is expected. 
            //* Input parameters: 
            //*strConnect----Connection string 
            //*ProcName ---StoredProcedures name 
            //* Returns :   
            //*nothing  
            //* ***************************************

            string strCommandText = ProcName;


            //create a new Connection object using the connection string
            SqlConnection objConnect = GetConnection();
            //create a new Command using the CommandText and Connection object
            SqlCommand objCommand = new SqlCommand(strCommandText, objConnect);
            try
            {
                //objConnect.Open();
                return objCommand.ExecuteNonQuery();


            }
            catch (SqlException objError)
            {
                //write error to the windows event log
                //WriteToEventLog(objError);
                CloseConnection(objConnect);
                throw;
            }
            finally
            {
                objConnect.Close();
            }

        }

        public Int64 RunProc(string ProcName, string TableName)
        {
            //****************************************
            //* Purpose: Executing  Stored Procedures where UPDATE, INSERT
            //*and DELETE statements are expected but does not 
            //*work for select statement is expected. 
            //* Input parameters: 
            //*strConnect----Connection string 
            //*ProcName ---StoredProcedures name 
            //* Returns :   
            //*nothing  
            //* ***************************************

            string strCommandText = ProcName;
            string sSQL;
            Int64 retVal = -1;


            //create a new Connection object using the connection string
            SqlConnection objConnect = GetConnection();
            //create a new Command using the CommandText and Connection object
            SqlCommand objCommand = new SqlCommand(strCommandText, objConnect);
            try
            {
                //objConnect.Open();

                objCommand.ExecuteNonQuery();
                sSQL = "SELECT IDENT_CURRENT('" + TableName + "')";
                SqlCommand ident = new SqlCommand(sSQL, objConnect);
                SqlDataReader ident_result = ident.ExecuteReader();

                if (ident_result.Read())
                {
                    retVal = (Int64)ident_result.GetDecimal(0);
                }

                ident_result.Close();
                objConnect.Dispose();
                CloseConnection(objConnect);
                return retVal;


            }
            catch (SqlException objError)
            {
                //write error to the windows event log
                //WriteToEventLog(objError);
                CloseConnection(objConnect);
                throw;
            }
            finally
            {
                objConnect.Close();
            }
        }

        public object RunProc(SqlCommand objCommand)
        {
            //****************************************
            //* Purpose: Executing  Stored Procedures where UPDATE, INSERT
            //*and DELETE statements are expected but does not 
            //*work for select statement is expected. 
            //* Input parameters: 
            //*strConnect----Connection string 
            //*ProcName ---StoredProcedures name 
            //* Returns :   
            //*nothing  
            //* ***************************************

            // string strCommandText = ProcName;


            //create a new Connection object using the connection string
            SqlConnection objConnect = GetConnection();
            //create a new Command using the CommandText and Connection object
            //SqlCommand objCommand = new SqlCommand(strCommandText, objConnect);

            objCommand.Connection = objConnect;
            try
            {
                //objConnect.Open();
                return objCommand.ExecuteNonQuery();


            }
            catch (SqlException objError)
            {
                //write error to the windows event log
                //WriteToEventLog(objError);
                CloseConnection(objConnect);
                throw;
            }
            finally
            {
                objConnect.Close();
            }

        }

    }

}

