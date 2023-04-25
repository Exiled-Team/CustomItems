// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="Joker119">
// Copyright (c) Joker119. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable SA1200

namespace CustomItems;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using Exiled.CustomItems.API.Features;
using Exiled.Loader;

using YamlDotNet.Serialization;

/// <summary>
/// The plugin's config class.
/// </summary>
public class Config : IConfig
{
    /// <summary>
    /// Gets item Config settings.
    /// </summary>
    [YamlIgnore]
    public Configs.Items ItemConfigs { get; private set; } = null!;

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
    public Dictionary<string, Dictionary<string, float>> SubclassList { get; set; } = new()
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
            File.WriteAllText(filePath, Loader.Serializer.Serialize(ItemConfigs));
        }
        else
        {
            ItemConfigs = Loader.Deserializer.Deserialize<Configs.Items>(File.ReadAllText(filePath));
            File.WriteAllText(filePath, Loader.Serializer.Serialize(ItemConfigs));
        }
    }
}