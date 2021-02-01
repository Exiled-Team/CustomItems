using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using CustomItems.API;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using Exiled.Loader;

namespace CustomItems.Configs
{
    public class Config : IConfig
    {
        [Description("Whether or not this plugin is enabled.")]
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        [Description("A list of each item and the subclasses that can spawn with it, and the % chance of them receiving it. **This is only used if Advanced Subclassing is installed!**")]
        public Dictionary<string, Dictionary<string, float>> SubclassList { get; set; } = new Dictionary<string, Dictionary<string, float>>
        {
            {
                "ExampleSubclass", new Dictionary<string, float> {{"SR-119", 100}, {"SG-119", 50}}
            }
        };

        public ItemConfigs ItemConfigs;
        public string CustomItemFolder { get; set; } = Path.Combine(Paths.Configs, "CustomItems");
        public string ConfigFileName { get; set; } = "global.yml";
        public Dictionary<string, List<Tuple<CustomItem, float>>> SubclassItems = new Dictionary<string, List<Tuple<CustomItem, float>>>();
        
        public void LoadConfigs()
        {
            if (!Directory.Exists(CustomItemFolder))
                Directory.CreateDirectory(CustomItemFolder);
            
            string filePath = Path.Combine(CustomItemFolder, ConfigFileName);
            
            if (!File.Exists(filePath))
            {
                ItemConfigs = new ItemConfigs();
                File.WriteAllText(filePath, ConfigManager.Serializer.Serialize(ItemConfigs));
            }
            else
            {
                ItemConfigs =
                    ConfigManager.Deserializer.Deserialize<ItemConfigs>(File.ReadAllText(filePath));
                File.WriteAllText(filePath, ConfigManager.Serializer.Serialize(ItemConfigs));
            }
        }
        
        public void ParseSubclassList()
        {
            SubclassItems.Clear();
            foreach (KeyValuePair<string, Dictionary<string, float>> list in SubclassList)
            {
                List<Tuple<CustomItem, float>> customItems = new List<Tuple<CustomItem, float>>();
                foreach (KeyValuePair<string, float> itemChance in list.Value)
                {
                    CustomItem item = null;
                    foreach (CustomItem cItem in Plugin.Singleton.ItemManagers)
                        if (cItem.ItemName == itemChance.Key)
                            item = cItem;
                    if (item == null)
                    {
                        Log.Warn($"Unable to add {itemChance.Key} to {list.Key}, item not installed.");
                        continue;
                    }
                    
                    customItems.Add(new Tuple<CustomItem, float>(item, itemChance.Value));
                    Log.Debug($"Adding {itemChance.Key} to {list.Key} with {itemChance.Value}% spawn chance.", Debug);
                }
                
                SubclassItems.Add(list.Key, customItems);
                Log.Debug($"{list.Key} has had {customItems.Count} items added to their spawn list.", Debug);
            }
        }
    }
}