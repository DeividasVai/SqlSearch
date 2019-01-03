using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Caliburn.Micro;

namespace SqlSearch.Components.FileManaging
{
    public class FileManager
    {
        private List<ConnectionInformation> _successfulConnectionsList;
        private const string ConfigurationsFile = "SavedConfigurations.xml";
        private string _currentPath;

        private const string SuccessfulConnectionsTag = "Successful_connections";
        private const string ConnectionInformationTag = "Connection_Information";
        private const string DatabaseTag = "Database";
        private const string SqlServerTag = "Sql_Server";
        private const string IntegratedSecurityTag = "Integrated_Security";
        private const string LastUsedTag = "Last_Used";

        public List<ConnectionInformation> GetSavedConfigurations()
        {
            if(!ConfigurationsExist())
                CreateConfigurations();
            return _successfulConnectionsList = ReadAllConfigs(XDocument.Load(Path.Combine(_currentPath, ConfigurationsFile))) ?? new List<ConnectionInformation>();
        }

        public bool SaveConfiguration(ConnectionInformation conInfo)
        {
            if (!ConfigurationsExist())
                CreateConfigurations();
            var path = Path.Combine(_currentPath, ConfigurationsFile);
            XDocument xdoc = XDocument.Load(path);

            if (_successfulConnectionsList?.Find(con => con.Database.Equals(conInfo.Database) &&
                                                       con.SqlServer.Equals(conInfo.SqlServer)) != null)
            {
                xdoc.Descendants(ConnectionInformationTag).FirstOrDefault(o => o.Element(DatabaseTag).Value.Equals(conInfo.Database) &&
                                                                      o.Element(SqlServerTag).Value.Equals(conInfo.SqlServer)).Element(LastUsedTag).Value = DateTime.Now.ToString();                                                                             
                xdoc.Save(path);
                return false;
            }
            XElement newConnectionInfo = new XElement(ConnectionInformationTag,
                new XElement(DatabaseTag, conInfo.Database),
                new XElement(SqlServerTag, conInfo.SqlServer),
                new XElement(IntegratedSecurityTag, conInfo.IntegratedSecurity.ToString()),
                new XElement(LastUsedTag, DateTime.Now.ToString()));
            xdoc.Element(SuccessfulConnectionsTag).Add(newConnectionInfo);
            xdoc.Save(path);
            return true;
        }

        private bool ConfigurationsExist()
        {
            _currentPath = Directory.GetCurrentDirectory();
            Console.Out.WriteLine($"{_currentPath} - Testing purpose");
            return File.Exists(Path.Combine(_currentPath, ConfigurationsFile));
        }

        private void CreateConfigurations()
        {
            Console.Out.WriteLine($"{_currentPath} - Testing purpose");
            XDocument xdoc = new XDocument();
            XElement root = new XElement(SuccessfulConnectionsTag);
            xdoc.Add(root);
            xdoc.Save(Path.Combine(_currentPath, ConfigurationsFile));
        }

        private List<ConnectionInformation> ReadAllConfigs(XDocument xdoc)
        {
            List<ConnectionInformation> tempList = new List<ConnectionInformation>();

            foreach (XElement element in xdoc.Descendants(ConnectionInformationTag))
            {
                tempList.Add(new ConnectionInformation
                {
                    Database = element.Element(DatabaseTag).Value,
                    SqlServer = element.Element(SqlServerTag).Value,
                    IntegratedSecurity = Boolean.Parse(element.Element(IntegratedSecurityTag).Value),
                    LastUsed = DateTime.Parse(element.Element(LastUsedTag).Value)
                });    
            }
            return tempList;
        }
    }
}
