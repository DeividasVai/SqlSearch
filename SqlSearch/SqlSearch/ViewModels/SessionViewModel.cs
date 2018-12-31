using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using SqlSearch.Components;

namespace SqlSearch.ViewModels
{
    [Export(typeof(MainPageViewModel))]
    public class SessionViewModel : Conductor<object>
    {
        public BindableCollection<ConnectionInformation> ConnectionList
        {
            get => _connectionList;
            set
            {
                _connectionList = value;
                NotifyOfPropertyChange(() => ConnectionList);
            }
        }

        public ConnectionInformation SelectedConnection
        {
            get => _selectedConnection;
            set
            {
                _selectedConnection = value;
                NotifyOfPropertyChange(() => SelectedConnection);
            }
        }

        private ConnectionInformation _selectedConnection;
        private BindableCollection<ConnectionInformation> _connectionList;

        [ImportingConstructor]
        public SessionViewModel()
        {
            BindableCollection<ConnectionInformation> conInf = new BindableCollection<ConnectionInformation>();
            conInf.Add(new ConnectionInformation() { SqlServer = "Test1", Database = "Test1.Test", IsSelected = false });
            conInf.Add(new ConnectionInformation() { SqlServer = "Test2", Database = "Test2.Test", IsSelected = false });
            conInf.Add(new ConnectionInformation() { SqlServer = "Test3", Database = "Test3.Test", IsSelected = false });
            ConnectionList = conInf;
        }

        public void ChangeActiveConnection(ConnectionInformation conInfo)
        {
            SelectedConnection = conInfo;
            foreach (ConnectionInformation information in ConnectionList.Where(item=>item != conInfo))
            {
                information.IsSelected = false;
            }
            //ConnectionList.Refresh();
        }
    }
}
