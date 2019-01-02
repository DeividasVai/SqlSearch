using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SqlSearch.Components
{
    public class SqlConnector
    {
        public SqlConnection Connection { get; set; }
        public async Task<bool> OpenConnection(ConnectionInformation conInfo)
        {
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            try
            {
                Connection = new SqlConnection(conInfo.ConnectionString);
                await Connection.OpenAsync(token);
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
