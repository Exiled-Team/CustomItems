// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="Galaxy199 and iopietro">
// Copyright (c) Galaxy199 and iopietro. All rights reserved.
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
        /// Gets or sets a value indicating what folder item configs will be stored in.
        /// </summary>
        public string ItemConfigFolder { get; set; } = Path.Combine(Paths.Configs, "SCPItems");

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
    }
}
