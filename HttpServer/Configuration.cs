using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace HttpServer
{
    public class Configuration
    {
        public readonly string RootPath;


        private const string ROOT_NAME = "root";
        private const string ROOT_DEFAULT = "C:\\www";

        public Configuration(string filename)
        {
            IDictionary<string, string> dict = LoadConfigurationXml(filename);
            RootPath = dict.ContainsKey(ROOT_NAME) ? dict[ROOT_NAME] : ROOT_DEFAULT;
        }

        private IDictionary<string, string> LoadConfigurationXml(string filename)
        {
            IDictionary<string, string> dict = new Dictionary<string, string>();
            XmlReader reader = new XmlTextReader(filename);
            string name = null;
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        name = reader.Name;
                        break;
                    case XmlNodeType.Text:
                        if (name != null)
                            dict[name] = reader.Value;
                        break;
                }
            }
            return dict;
        }
    }
}
