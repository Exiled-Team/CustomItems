using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CustomItems.Components;
using Exiled.API.Features;
using Exiled.API.Interfaces;

namespace CustomItems
{
    public class Config : IConfig
    {
        [Description("Whether or not this plugin is enabled.")]
        public bool IsEnabled { get; set; } = true;

        [Description("The damage multiplier to apply to Sniper Rifles.")]
        public float SniperDmgMult { get; set; } = 1.5f;

        public bool Debug { get; set; } = false;

        [Description("A list of each item and the subclasses that can spawn with it, and the % chance of them receiving it. **This is only used if Advanced Subclassing is installed!**")]
        public Dictionary<string, List<string>> SubclassList { get; set; } = new Dictionary<string, List<string>>
        {
            {
                "ExampleSubclass", new List<string>{"ExampleItem1-100", "ExampleItem2-50"}
            }
        };
        
        public int ShotgunSpreadCount { get; set; } = 5;
        public float ShotgunAimCone { get; set; } = 5;
        public float ShotgunHeadDamage { get; set; } = 12.5f;
        public float ShotgunArmDamage { get; set; } = 6.75f;
        public float ShotgunLegDamage { get; set; } = 6.75f;
        public float ShotgunBodyDamage { get; set; } = 13.5f;

        public Dictionary<string, List<Tuple<CustomItem, float>>> SubclassItems = new Dictionary<string, List<Tuple<CustomItem, float>>>();
        public void ParseSubclassList()
        {
            foreach (KeyValuePair<string, List<string>> list in SubclassList)
            {
                List<Tuple<CustomItem, float>> customItems = new List<Tuple<CustomItem, float>>();
                foreach (string itemName in list.Value)
                {
                    string[] array = itemName.Split('-');
                    string name = array[0];
                    if (!float.TryParse(array[1], out float chance))
                    {
                        Log.Warn($"Unable to parse spawn chance for {name}.");

                        continue;
                    }

                    CustomItem item = null;
                    foreach (CustomItem cItem in Plugin.Singleton.ItemManagers)
                        if (cItem.ItemName == name)
                            item = cItem;
                    if (item == null)
                    {
                        Log.Warn($"Unable to add {name} to {list.Key}, item not installed.");
                        continue;
                    }
                    
                    customItems.Add(new Tuple<CustomItem, float>(item, chance));
                    Log.Debug($"Adding {name} to {list.Key} with {chance}% spawn chance.", Debug);
                }
                
                SubclassItems.Add(list.Key, customItems);
                Log.Debug($"{list.Key} has had {customItems.Count} items added to their spawn list.", Debug);
            }
        }
    }
}