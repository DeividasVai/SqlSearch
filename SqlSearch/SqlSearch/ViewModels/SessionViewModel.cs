using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Caliburn.Micro;
using SqlSearch.Components;
using SqlSearch.Components.FileManaging;
using SqlSearch.Views.Session;

namespace SqlSearch.ViewModels
{
    [Export(typeof(SessionViewModel))]
    public class SessionViewModel : PropertyChangedBase
    {
        public BindableCollection<ConnectionInformation> ConnectionList
        {
            get => _connectionList;
            set
            {
                if (_connectionList == value) return;
                _connectionList = value;
                NotifyOfPropertyChange(() => ConnectionList);
            }
        }
        public ConnectionInformation SelectedConnection
        {
            get => _selectedConnection;
            set
            {
                if (_selectedConnection == value) return;
                _selectedConnection = value;
                NotifyOfPropertyChange(() => SelectedConnection);
            }
        }
        public SqlConnector Connection
        {
            get => _connection;
            set
            {
                if (_connection == value) return;
                _connection = value;
                NotifyOfPropertyChange(() => Connection);
            }
        }
        public string ConnectionTest
        {
            get => _connectionTest;
            set
            {
                if (_connectionTest == value) return;
                _connectionTest = value;
                NotifyOfPropertyChange(() => ConnectionTest);
            }
        }
        public ProjectSession ProjectSession
        {
            get => _projectSession;
            set
            {
                if (_projectSession == value) return;
                _projectSession = value;
                NotifyOfPropertyChange(() => ProjectSession);
            }
        }
        public FileManager FileManager
        {
            get => _fileManager;
            set
            {
                if (_fileManager == value) return;
                _fileManager = value;
                NotifyOfPropertyChange(() => FileManager);
            }
        }
        public bool IsConnecting
        {
            get => _isConnecting;
            set
            {
                if (_isConnecting == value) return;
                _isConnecting = value;
                NotifyOfPropertyChange(() => IsConnecting);
            }
        }

        private string _connectionTest;
        private ProjectSession _projectSession;
        private ConnectionInformation _selectedConnection;
        private BindableCollection<ConnectionInformation> _connectionList;
        private SqlConnector _connection;
        private FileManager _fileManager;
        private bool _isConnecting;

        [ImportingConstructor]
        public SessionViewModel()
        {
            ProjectSession = new ProjectSession();
            BindableCollection<ConnectionInformation> conInf = new BindableCollection<ConnectionInformation>
            {
                new ConnectionInformation() {SqlServer = "localhost\\MSSQLEXPRESS", Database = "CLIENT_SQL IMPORT FIX", IsSelected = false, IntegratedSecurity = true},
                new ConnectionInformation() {SqlServer = "Test2", Database = "Test2.Test", IsSelected = false, IntegratedSecurity = true},
                new ConnectionInformation() {SqlServer = "Test3", Database = "Test3.Test", IsSelected = false, IntegratedSecurity = false}
            };
            ConnectionList = conInf;
            SelectedConnection = new ConnectionInformation();
            ConnectionTest = "Provide connection information";
        }

        public void ChangeActiveConnection(ConnectionInformation conInfo)
        {
            SelectedConnection = conInfo;
            foreach (ConnectionInformation information in ConnectionList.Where(item=>item != conInfo))
            {
                information.IsSelected = false;
            }
        }

        public void OpenExistingProject()
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                var tempPath = folderDialog.SelectedPath;
                Console.Out.WriteLine(tempPath);
                // read settings, get connection information
            }
        }
        
        public async void TestConnection()
        {
            IsConnecting = true;
            Task<bool> status = OpenConnection();
            await status;
            ConnectionTest = status.Result ? "Connection available" : "Connection unavailable";
            IsConnecting = false;
            CloseConnection();
        }
        
        public async Task<bool> OpenConnection(bool isTest = true)
        {
            var status = ProjectSession.OpenConnection(SelectedConnection);
            await status;
            if (isTest)
            {
                return status.Result;
            }

            if (status.Result)
            {
                ProjectSession.ConnectionInformation = SelectedConnection;
                Console.Out.WriteLine("Connection established");
            }
            else
            {
                Console.Out.WriteLine("Connection terminated or unavailable");
            }
            return status.Result;

            // jump to projectView
        }

        public void CloseConnection()
        {
            ProjectSession.CloseConnection();
            ProjectSession.ConnectionInformation = null;
        }
    }
}
