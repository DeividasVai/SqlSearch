using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace SqlSearch.Components
{
    public class ConnectionInformation : PropertyChangedBase
    {
        public string Database { get; set; }
        public string SqlServer { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IntegratedSecurity { get; set; }
        public string ConnectionString => IntegratedSecurity
            ? $"Server={SqlServer};Database={Database};Integrated Security=True;"
            : $"Server={SqlServer};Database={Database};User Id={Username};Password={Password};";

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                NotifyOfPropertyChange(() => IsSelected);
            }
        }

        private bool _isSelected;
    }
}
