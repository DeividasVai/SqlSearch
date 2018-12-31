using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSearch.Components
{
    public class SqlConnector
    {
        public SqlConnection Connection { get; set; }
        public bool OpenConnection(ConnectionInformation conInfo)
        {
            try
            {
                Connection = new SqlConnection(conInfo.ConnectionString);
                Connection.Open();
                return true;
            }
            catch (SqlException e)
            {
                Console.Out.WriteLine(e);
                return false;
            }
        }

        public void CloseConnection()
        {
            Connection.Close();
            Console.Out.WriteLine("Connection closed");
        }
    }
}
