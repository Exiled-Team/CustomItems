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

        [Description("The damage multiplier to apply to Sniper Rifles.")]
        public float SniperDmgMult { get; set; } = 1.5f;

        public bool Debug { get; set; } = false;

        [Description("A list of each item and the subclasses that can spawn with it, and the % chance of them receiving it. **This is only used if Advanced Subclassing is installed!**")]
        public Dictionary<string, Dictionary<string, float>> SubclassList { get; set; } = new Dictionary<string, Dictionary<string, float>>
        {
            {
                "ExampleSubclass", new Dictionary<string, float> {{"SR-119", 100}, {"SG-119", 50}}
            }
        };

        public WeaponConfigs WeaponConfigs;

        public void LoadConfigs()
        {
            if (!Directory.Exists(CustomWeaponFolder))
                Directory.CreateDirectory(CustomWeaponFolder);
            
            string filePath = Path.Combine(CustomWeaponFolder, ConfigFileName);
            
            if (!File.Exists(filePath))
            {
                WeaponConfigs = new WeaponConfigs();
                File.WriteAllText(filePath, ConfigManager.Serializer.Serialize(WeaponConfigs));
            }
            else
            {
                WeaponConfigs =
                    ConfigManager.Deserializer.Deserialize<WeaponConfigs>(File.ReadAllText(filePath));
                File.WriteAllText(filePath, ConfigManager.Serializer.Serialize(WeaponConfigs));
            }
        }
        

        public string CustomWeaponFolder { get; set; } = Path.Combine(Paths.Configs, "CustomWeapons");
        public string ConfigFileName { get; set; } = "global.yml";
        public int ShotgunSpreadCount { get; set; } = 5;
        public float ShotgunAimCone { get; set; } = 5;
        public float ShotgunHeadDamage { get; set; } = 12.5f;
        public float ShotgunArmDamage { get; set; } = 6.75f;
        public float ShotgunLegDamage { get; set; } = 6.75f;
        public float ShotgunBodyDamage { get; set; } = 13.5f;
        public float Scp127RegenerationDelay { get; set; } = 10f;
        public int Scp127RegenerationAmount { get; set; } = 2;
        public int EmpDuration { get; set; } = 20;
        public int MedigunZombieHealthRequired { get; set; } = 200;

        public Dictionary<string, List<Tuple<CustomItem, float>>> SubclassItems = new Dictionary<string, List<Tuple<CustomItem, float>>>();
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
        
        /*        public Dictionary<string, List<Tuple<string, string>>> CustomWeaponConfigs { get; set; } = new Dictionary<string, List<Tuple<string, string>>>
        {
            {
                "SR-119", new List<Tuple<string, string>>()
                {
                    new Tuple<string, string>("DamageMultiplier", "7.5")
                }
            },
            {
                "SG-119", new List<Tuple<string, string>>()
                {
                    new Tuple<string, string>("SpreadCount", "12"),
                    new Tuple<string, string>("Aimcone", "5"),
                    new Tuple<string, string>("BaseDamage", "13.5")
                }
            },
            {
                "SCP-127", new List<Tuple<string, string>>()
                {
                    new Tuple<string, string>("RegenDelay", "10"),
                    new Tuple<string, string>("RegenAmount", "2")
                }
            },
            {
                "EM-119", new List<Tuple<string, string>>()
                {
                    new Tuple<string, string>("Duration", "20")
                }
            }
        };*/
    }
}