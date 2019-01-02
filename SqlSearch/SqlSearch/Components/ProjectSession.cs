using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Caliburn.Micro;
using Cursor = System.Windows.Input.Cursor;

namespace SqlSearch.Components
{
    public class ProjectSession : PropertyChangedBase
    {
        public SqlConnector Connection
        {
            get => _sqlConnector;
            set
            {
                _sqlConnector = value;
                NotifyOfPropertyChange(() => Connection);
            }
        }

        public ConnectionInformation ConnectionInformation { get; set; }

        private SqlConnector _sqlConnector;

        public ProjectSession()
        {
            Connection = new SqlConnector();
            ConnectionInformation = new ConnectionInformation();
        }

        #region Sql connection managing

        public Task<bool> OpenConnection(ConnectionInformation conInfo)
        {
            ConnectionInformation = conInfo;
            var status = Connection.OpenConnection(ConnectionInformation);
            return status;
        }

        public void CloseConnection()
        {
            Connection.CloseConnection();
        }

        #endregion
    }
}
