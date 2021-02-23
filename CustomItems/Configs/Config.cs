// <copyright file="Config.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CustomItems.Configs
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using Exiled.API.Features;
    using Exiled.API.Interfaces;
    using Exiled.CustomItems.API.Features;
    using Exiled.Loader;

    /// <summary>
    /// The plugin's config class.
    /// </summary>
    public class Config : IConfig
    {
        /// <summary>
        /// The list of <see cref="CustomItem"/>s and their spawn chances for each Subclass.
        /// </summary>
        public Dictionary<string, List<Tuple<CustomItem, float>>> SubclassItems = new Dictionary<string, List<Tuple<CustomItem, float>>>();

        /// <summary>
        /// The item configs reference object.
        /// </summary>
        public ItemConfigs ItemConfigs;

        /// <inheritdoc/>
        [Description("Whether or not this plugin is enabled.")]
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether if debug mode is enabled.
        /// </summary>
        [Description("Whether or not debug messages should be displayed in the server console.")]
        public bool Debug { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating what subclasses should get what items, and their spawn chances.
        /// </summary>
        [Description("A list of each item and the subclasses that can spawn with it, and the % chance of them receiving it. **This is only used if Advanced Subclassing is installed!**")]
        public Dictionary<string, Dictionary<string, float>> SubclassList { get; set; } = new Dictionary<string, Dictionary<string, float>>
        {
            {
                "ExampleSubclass", new Dictionary<string, float> { { "SR-119", 100 }, { "SG-119", 50 } }
            },
        };

        /// <summary>
        /// Gets or sets a value indicating the custom item config folder path.
        /// </summary>
        [Description("The folder location where CustomItems will look for and store it's item configurations.")]
        public string CustomItemFolder { get; set; } = Path.Combine(Paths.Configs, "CustomItems");

        /// <summary>
        /// Gets or sets a value indicating the custom item config file path used for the server.
        /// </summary>
        [Description("The name of the config file this server will use for custom item configurations.")]
        public string ConfigFileName { get; set; } = "global.yml";

        /// <summary>
        /// Gets or sets a value indicating whether TranqGun is enabled.
        /// </summary>
        [Description("The following configs determine whether or not that item is enabled on this server. If the item is disabled, it's configs are still loaded, however the item will not spawn on the map, cannot be used for subclasses, or given via commands.")]
        public bool TranqGun { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether MediGun is enabled.
        /// </summary>
        public bool MediGun { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether Lethal Injections are enabled.
        /// </summary>
        public bool LethalInjection { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether EMP Grenades are enabled.
        /// </summary>
        public bool EmpGrenade { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether Implosion Grenades are enabled.
        /// </summary>
        public bool ImplosionGrenade { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether SCP-127 is enabled.
        /// </summary>
        public bool Scp127 { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether Sniper Rifles are enabled.
        /// </summary>
        public bool SniperRifle { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether Grenade Launchers are enabled.
        /// </summary>
        public bool GrenadeLauncher { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether Shotguns are enabled.
        /// </summary>
        public bool Shotgun { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether Lucky Coins are enabled.
        /// </summary>
        public bool LuckyCoin { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether SCP-1499 is enabled.
        /// </summary>
        public bool Scp1499 { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether Rocks are enabled.
        /// </summary>
        public bool Rock { get; set; } = true;

        /// <summary>
        /// Loads item configs.
        /// </summary>
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

        /// <summary>
        /// Parses the subclass list from the Plugin config.
        /// </summary>
        public void ParseSubclassList()
        {
            SubclassItems.Clear();
            foreach (KeyValuePair<string, Dictionary<string, float>> list in SubclassList)
            {
                List<Tuple<CustomItem, float>> customItems = new List<Tuple<CustomItem, float>>();
                foreach (KeyValuePair<string, float> itemChance in list.Value)
                {
                    CustomItem item = null;
                    foreach (CustomItem cItem in Exiled.CustomItems.CustomItems.Instance.ItemManagers)
                        if (cItem.Name == itemChance.Key)
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
