/*
 * Copyright (C) 2017  Nick Chapsas
 * This program is free software: you can redistribute it and/or modify it under
 * the terms of the GNU General Public License as published by the Free Software
 * Foundation, either version 2 of the License, or (at your option) any later
 * version.
 * 
 * L2ACP is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE. See the GNU General Public License for more
 * details.
 * 
 * You should have received a copy of the GNU General Public License along with
 * this program. If not, see <http://www.gnu.org/licenses/>.
 */
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
        private ConcurrentDictionary<int, L2Npc> _npcs;
        private readonly IHostingEnvironment _hostingEnvironment;
        public AssetManager(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;    
        }

        public ConcurrentDictionary<int, L2Item> GetItems()
        {
            return _items;
        }

        public ConcurrentDictionary<int, L2Npc> GetNpcs()
        {
            return _npcs;
        }

        public void Initialize()
        {
            string webRootPath = _hostingEnvironment.WebRootPath;
            InitialiseItems(webRootPath);

            var files = Directory.GetFileSystemEntries(Path.Combine(webRootPath,"xml","npcs"));
            Dictionary<int, L2Npc> npcs = new Dictionary<int, L2Npc>();
            foreach (var file in files)
            {
                var content = File.ReadAllText(file);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(content);
                XmlNodeList xnList = doc.SelectNodes("list/npc"); //.ChildNodes;
                foreach (XmlNode node in xnList)
                {
                    int id = int.Parse(node.Attributes["id"].InnerText);
                    string name = node.Attributes["name"].InnerText;
                    var npc = new L2Npc
                    {
                        NpcId = id,
                        Name = name
                    };
                    npcs.Add(id, npc);
                }
            }
            _npcs = new ConcurrentDictionary<int, L2Npc>(npcs);
        }

        private void InitialiseItems(string webRootPath)
        {
            var files = Directory.GetFileSystemEntries(Path.Combine(webRootPath,"xml","items"));
            Dictionary<int, L2Item> items = new Dictionary<int, L2Item>();
            foreach (var file in files)
            {
                var content = File.ReadAllText(file);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(content);
                XmlNodeList xnList = doc.SelectNodes("list/item"); //.ChildNodes;
                foreach (XmlNode node in xnList)
                {
                    int id = int.Parse(node.Attributes["id"].InnerText);
                    string name = node.Attributes["name"].InnerText;
                    string icon = string.Empty;
                    bool enchantable = false;
                    bool questItem = false;
                    foreach (XmlNode child in node.ChildNodes)
                    {
//enchant_enabled
                        if (child?.Attributes?["name"]?.InnerText == "icon")
                        {
                            icon = child.Attributes["val"].InnerText.Replace("icon.", string.Empty);
                        }
                        if (child?.Attributes?["name"]?.InnerText == "enchant_enabled")
                        {
                            enchantable = child.Attributes["val"].InnerText == "1";
                        }

                        if (child?.Attributes?["name"]?.InnerText == "is_questitem")
                        {
                            questItem = child.Attributes["val"].InnerText == "true";
                        }
                    }
                    var item = new L2Item
                    {
                        ItemId = id,
                        Name = name,
                        Image = icon,
                        Enchantable = enchantable,
                        IsQuestItem = questItem
                    };
                    items.Add(id, item);
                }
            }
            _items = new ConcurrentDictionary<int, L2Item>(items);
        }
    }
}