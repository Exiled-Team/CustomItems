// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="Galaxy119 and iopietro">
// Copyright (c) Galaxy119 and iopietro. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable SA1200

using CustomItems.Configs;

namespace CustomItems
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
        /// Item Config settings.
        /// </summary>
        public Configs.Items ItemConfigs;

        /// <summary>
        /// The list of <see cref="CustomItem"/>s and their spawn chances for each Subclass.
        /// </summary>
        public Dictionary<string, List<Tuple<CustomItem, float>>> SubclassItems = new Dictionary<string, List<Tuple<CustomItem, float>>>();

        /// <inheritdoc/>
        [Description("Whether or not this plugin is enabled.")]
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether if debug mode is enabled.
        /// </summary>
        [Description("Whether or not debug messages should be displayed in the server console.")]
        public bool IsDebugEnabled { get; set; } = false;

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
        /// Gets or sets a value indicating what folder item configs will be stored in.
        /// </summary>
        public string ItemConfigFolder { get; set; } = Path.Combine(Paths.Configs, "CustomItems");

        /// <summary>
        /// Gets or sets a value indicating what file will be used for item configs.
        /// </summary>
        public string ItemConfigFile { get; set; } = "global.yml";

        /// <summary>
        /// Loads the item configs.
        /// </summary>
        public void LoadItems()
        {
            if (!Directory.Exists(ItemConfigFolder))
                Directory.CreateDirectory(ItemConfigFolder);

            string filePath = Path.Combine(ItemConfigFolder, ItemConfigFile);
            Log.Info($"{filePath}");
            if (!File.Exists(filePath))
            {
                ItemConfigs = new Configs.Items();
                File.WriteAllText(filePath, ConfigManager.Serializer.Serialize(ItemConfigs));
            }
            else
            {
                ItemConfigs = ConfigManager.Deserializer.Deserialize<Configs.Items>(File.ReadAllText(filePath));
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

                    foreach (CustomItem customItem in CustomItem.Registered)
                    {
                        if (customItem.Name == itemChance.Key)
                            item = customItem;
                    }

                    if (item == null)
                    {
                        Log.Warn($"Unable to add {itemChance.Key} to {list.Key}, item not installed.");
                        continue;
                    }

                    customItems.Add(new Tuple<CustomItem, float>(item, itemChance.Value));

                    Log.Debug($"Adding {itemChance.Key} to {list.Key} with {itemChance.Value}% spawn chance.", IsDebugEnabled);
                }

                SubclassItems.Add(list.Key, customItems);

                Log.Debug($"{list.Key} has had {customItems.Count} items added to their spawn list.", IsDebugEnabled);
            }
        }
    }
}
