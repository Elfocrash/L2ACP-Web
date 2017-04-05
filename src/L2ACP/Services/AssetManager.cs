using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using L2ACP.Models;
using Microsoft.AspNetCore.Hosting;

namespace L2ACP.Services
{
    public class AssetManager
    {
        private ConcurrentDictionary<int, L2Item> _items;
        private readonly IHostingEnvironment _hostingEnvironment;
        public AssetManager(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;    
        }

        public ConcurrentDictionary<int, L2Item> GetItems()
        {
            return _items;
        }

        public void Initialize()
        {
            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = Directory.GetFileSystemEntries(webRootPath + "\\xml\\items");
            Dictionary<int, L2Item> items = new Dictionary<int, L2Item>();
            foreach (var file in files)
            {
                var content = File.ReadAllText(file);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(content);
                XmlNodeList xnList = doc.SelectNodes("list/item");//.ChildNodes;
                foreach (XmlNode node in xnList)
                {
                    int id = int.Parse(node.Attributes["id"].InnerText);
                    string name = node.Attributes["name"].InnerText;
                    string icon = string.Empty;
                    bool enchantable = false;
                    foreach (XmlNode child in node.ChildNodes)
                    {//enchant_enabled
                        if (child?.Attributes?["name"]?.InnerText == "icon")
                        {
                            icon = child.Attributes["val"].InnerText.Replace("icon.",string.Empty);
                        }
                        if (child?.Attributes?["name"]?.InnerText == "enchant_enabled")
                        {
                            enchantable = child.Attributes["val"].InnerText == "1";
                        }
                    }
                    var item = new L2Item
                    {
                        ItemId = id, Name = name, Image = icon, Enchantable = enchantable
                    };
                    items.Add(id,item);
                }
            }
            _items = new ConcurrentDictionary<int, L2Item>(items);
        }
    }
}