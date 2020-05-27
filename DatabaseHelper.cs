using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using PTC;

namespace Captivate.Helpers
{
    public class DatabaseHelper
    {
        // create object fields 
        private SqlConnection conn;

        private SqlDataAdapter adapter;

        private SqlCommand doit;

        public DatabaseHelper()
        {
            conn = new SqlConnection()
            {
                // creates our connection string variable
                ConnectionString = ConfigurationManager.AppSettings["ConnectionString"]
            };
        }

        public int ExecuteNonQuery(string query, List<SqlParameter> actions)
        {
            try
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                using (doit = new SqlCommand()) //implements IDisposable
                {
                    doit.Connection = conn; // connects to designated db
                    doit.CommandText = query; // specifies the query that we're runnning 
                    doit.CommandType = CommandType.Text; //look up enum, specifies the type of query 
                    if (actions != null && actions.Count() != 0)
                        doit.Parameters.AddRange(actions.ToArray());
                    return doit.ExecuteNonQuery(); // executes our  query and returns the # of rows affected
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        public DataSet GetDataSet(string query)
        {
            try
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                var dataSet = new DataSet();

                using (doit = new SqlCommand())
                {
                    doit.Connection = conn;
                    doit.CommandText = query;
                    doit.CommandType = CommandType.Text;
                    using (adapter = new SqlDataAdapter())
                    {
                        adapter.SelectCommand = doit;
                        adapter.Fill(dataSet);
                        return dataSet;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }
    }
}