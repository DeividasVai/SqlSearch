using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using SqlSearch.Components;
using SqlSearch.Views;

namespace SqlSearch.ViewModels
{
    [Export(typeof(MainPageViewModel))]
    public class MainPageViewModel : PropertyChangedBase
    {
        public string WindowTitle
        {
            get => _windowTitle;
            set
            {
                if (value == _windowTitle) return;
                _windowTitle = value;
                NotifyOfPropertyChange(() => WindowTitle);
            }
        }

        public SessionViewModel SessVM
        {
            get => _sessVM;
            set
            {
                _sessVM = value;
                NotifyOfPropertyChange(() => SessVM);
            }
        }

        public string ViewName
        {
            get => _viewName;
            set
            {
                if (_viewName == value) return;
                _viewName = value;
                NotifyOfPropertyChange(() => ViewName);
            }
        }


        
        //public ProjectSession ProjectSession
        //{
        //    get => _projectSession;
        //    set
        //    {
        //        if (_projectSession == value) return;
        //        _projectSession = value;
        //        NotifyOfPropertyChange(() => ProjectSession);
        //    }
        //}
        //private ProjectSession _projectSession;


        private readonly IWindowManager _windowManager;
        private readonly IEventAggregator _eventAggregator;
        private string _windowTitle;
        private SessionViewModel _sessVM;
        private string _viewName;

        [ImportingConstructor]
        public MainPageViewModel(IWindowManager windowManager, IEventAggregator eventAggregator)
        {
            _windowManager = windowManager;

            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            // ------------------- later ^

            WindowTitle = "Sql Search";
            OpenVM();
        }

        public void OpenVM(string viewName = "BlankSessionView")
        {
            if (SessVM == null)
            {
                SessVM = new SessionViewModel(this);
            }
            if(viewName.Equals("BlankSessionView"))
                SessVM.FreshConnectionInformation();
            ViewName = viewName;
        }

        public void CloseConnection()
        {
            SessVM.CloseConnection(false);
            OpenVM();
        }
        
        public void LoadConfigurations()
        {
            SessVM.LoadConfigurations();
        }

        public void CloseFlyout()
        {
            SessVM.CloseFlyout();
        }
    }
}
