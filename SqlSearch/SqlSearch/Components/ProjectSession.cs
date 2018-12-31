using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSearch.Components
{
    public class ProjectSession
    {
        protected SqlConnector Connector { get; set; }
        protected ConnectionInformation ConnectionInformation { get; set; }

        public ProjectSession()
        {
            Connector = new SqlConnector();
            ConnectionInformation = new ConnectionInformation();
        }

        #region Sql connection managing

        public void OpenConnection()
        {
            Connector.OpenConnection(ConnectionInformation);
        }

        public void CloseConnection()
        {
            Connector.CloseConnection();
        }

        #endregion
    }
}
