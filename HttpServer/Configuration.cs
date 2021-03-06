﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace HttpServer
{
    public class Configuration
    {
        public readonly string RootPath = "C:\\www";
        public readonly int Port = 8080;
        public readonly string DefaultContentType = "application/octet-stream";

        private const string ROOT_NAME = "root";
        private const string PORT_NAME = "port";
        private const string DEFAULT_CONTENT_TYPE = "default-content-type";

        public Configuration(string filename)
        {
            if (String.IsNullOrEmpty(filename))
                return;
            IDictionary<string, string> dict = LoadConfigurationXml(filename);
            if (dict.ContainsKey(ROOT_NAME))
                RootPath = dict[ROOT_NAME];
            if (dict.ContainsKey(PORT_NAME))
                Port = Convert.ToInt32(dict[PORT_NAME]);
            if (dict.ContainsKey(DEFAULT_CONTENT_TYPE))
                DefaultContentType = dict[DEFAULT_CONTENT_TYPE];
        }

        private IDictionary<string, string> LoadConfigurationXml(string filename)
        {
            IDictionary<string, string> dict = new Dictionary<string, string>();
            using (XmlReader reader = new XmlTextReader(filename))
            {
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
}