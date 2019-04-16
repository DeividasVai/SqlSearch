﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using SqlSearch.Components;
using SqlSearch.Components.FileManaging;
using SqlSearch.Views.Session;
using DataFormats = System.Windows.Forms.DataFormats;
using DragEventArgs = System.Windows.Forms.DragEventArgs;
using Timer = System.Threading.Timer;

namespace SqlSearch.ViewModels
{
    public enum VisibilityStrings
    {
        Collapsed, Hidden, Visible
    }

    [Export(typeof(SessionViewModel))]
    public class SessionViewModel : PropertyChangedBase
    {
        public List<ConnectionInformation> ConnectionList
        {
            get
            {
                IEnumerable<ConnectionInformation> sorted = _connectionList;
                return sorted.OrderByDescending(con => con.LastUsed).ToList();
            }
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
        public MainPageViewModel MainPageViewModel
        {
            get => _mainPageViewModel;
            set
            {
                if (_mainPageViewModel == value) return;
                _mainPageViewModel = value;
                NotifyOfPropertyChange(() => MainPageViewModel);
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
        public string FlyoutContent
        {
            get => _flyoutContent;
            set
            {
                if (_flyoutContent == value) return;
                _flyoutContent = value;
                NotifyOfPropertyChange(() => FlyoutContent);
            }
        }
        public bool IsTestingConnection
        {
            get => _isTestingIsTestingConnection;
            set
            {
                if (_isTestingIsTestingConnection == value) return;
                _isTestingIsTestingConnection = value;
                NotifyOfPropertyChange(() => IsTestingConnection);
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
        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                if (_isConnected == value) return;
                _isConnected = value;
                NotifyOfPropertyChange(() => IsConnected);
            }
        }
        public bool ShowFlyout
        {
            get => _showFlyout;
            set
            {
                if (_showFlyout == value) return;
                _showFlyout = value;
                NotifyOfPropertyChange(() => ShowFlyout);
            }
        }
        public bool IsLoadingObjects
        {
            get => _isLoadingObjects;
            set
            {
                if (_isLoadingObjects == value) return;
                _isLoadingObjects = value;
                NotifyOfPropertyChange(() => IsLoadingObjects);
            }
        }

        private ProjectSession _projectSession;
        private ConnectionInformation _selectedConnection;
        private List<ConnectionInformation> _connectionList;
        private SqlConnector _connection;
        private FileManager _fileManager;
        private MainPageViewModel _mainPageViewModel;
        private string _connectionTest;
        private string _flyoutContent;
        private bool _isTestingIsTestingConnection;
        private bool _showFlyout;
        private bool _isConnecting;
        private bool _isConnected;
        private bool _isLoadingObjects;
        private System.Timers.Timer _aTimer;
        private string _text;

        [ImportingConstructor]
        public SessionViewModel(MainPageViewModel mainPageViewModel)
        {
            MainPageViewModel = mainPageViewModel;
            Initialization();
        }

        public void Initialization()
        {
            ProjectSession = new ProjectSession();
            FileManager = new FileManager();
            IsConnected = false;
            IsLoadingObjects = false;
            FreshConnectionInformation();
            HideProgress();
            ConnectionTest = "Provide connection information";
            // ----------------------------------------------- EXAMPLES 
            //ConnectionList = new List<ConnectionInformation>
            //{
            //    new ConnectionInformation() {SqlServer = "localhost\\MSSQLEXPRESS", Database = "CLIENT_SQL IMPORT FIX", IsSelected = false, IntegratedSecurity = true, LastUsed = new DateTime(2004, 05, 01)},
            //    new ConnectionInformation() {SqlServer = "Test2", Database = "Test2.Test", IsSelected = false, IntegratedSecurity = true, LastUsed = new DateTime(2010, 10, 30)},
            //    new ConnectionInformation() {SqlServer = "Test3", Database = "Test3.Test", IsSelected = false, IntegratedSecurity = false, LastUsed = new DateTime(2012, 12, 31)}
            //};
        }

        public void FreshConnectionInformation()
        {
            SelectedConnection = new ConnectionInformation();
            LoadConfigurations();
        }

        public void LoadConfigurations()
        {
            ConnectionList = FileManager.GetSavedConfigurations();
        }
        
        public void ChangeActiveConnection(ConnectionInformation conInfo)
        {
            SelectedConnection = conInfo;
            foreach (ConnectionInformation information in ConnectionList.Where(item=>item != conInfo))
            {
                information.IsSelected = false;
            }
        }
        
        public async void TestConnection()
        {
            IsConnecting = true;
            Task<bool> status = OpenConnection();
            await status;
            ConnectionTest = status.Result ? "Connection available" : "Connection unavailable";
            IsConnecting = false;
            if (!status.Result)
                return;
            CloseConnection();
        }
        
        public async Task<bool> OpenConnection(bool isTest = true)
        {
            if (string.IsNullOrEmpty(SelectedConnection.Database) || string.IsNullOrEmpty(SelectedConnection.SqlServer)) return false;
            ShowProgress();
            var status = ProjectSession.OpenConnection(SelectedConnection);
            await status;
            HideProgress();
            if (isTest)
            {
                return status.Result;
            }

            if (status.Result)
            {
                ProjectSession.ConnectionInformation = SelectedConnection;
                bool saved = FileManager.SaveConfiguration(SelectedConnection);
                IsConnected = true;

                OpenFlyout(saved ? "Configuration saved and loaded succesfuly" : "Configuration loaded successfuly");
                LoadConfigurations();
                MainPageViewModel.OpenVM("SessionView");
            }
            else
            {
                Console.Out.WriteLine("Connection terminated or unavailable");
            }
            return status.Result;
        }

        public void CloseConnection(bool isTest = true)
        {
            ProjectSession.CloseConnection();
            ProjectSession.ConnectionInformation = null;
            IsConnected = false;
            if (isTest) return;
            OpenFlyout("Connection closed");
        }

        public async void SearchByCriteria()
        {
            IsLoadingObjects = true;
            ActivateTimer();
        }

        public void OpenFlyout(string content)
        {
            FlyoutContent = content;
            ShowFlyout = true;
        }

        public void CloseFlyout()
        {
            ShowFlyout = false;
        }

        public string Text
        {
            get => _text;
            set
            {
                if (value == _text) return;
                _text = value;
                NotifyOfPropertyChange();
            }
        }
        
        private void ShowProgress()
        {
            IsTestingConnection = true;
        }

        private void HideProgress()
        {
            IsTestingConnection = false;
        }

        private void ActivateTimer()
        {
            _aTimer = new System.Timers.Timer {Interval = 2000};

            // Hook up the Elapsed event for the timer. 
            _aTimer.Elapsed += OnTimedEvent;

            // Have the timer fire repeated events (true is the default)
            //_aTimer.AutoReset = true;

            // Start the timer
            _aTimer.Enabled = true;
        }

        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            IsLoadingObjects = false;
            _aTimer.Elapsed -= OnTimedEvent;
            _aTimer.Enabled = false;
            _aTimer = null;
        }
    }
}


