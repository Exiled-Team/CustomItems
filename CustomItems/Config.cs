// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="Galaxy199 and iopietro">
// Copyright (c) Galaxy199 and iopietro. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

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
    using Items;

    /// <summary>
    /// The plugin's config class.
    /// </summary>
    public class Config : IConfig
    {
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
        /// Gets the list of emp greanades.
        /// </summary>
        [Description("The list of EMP grenades.")]
        public List<EmpGrenade> EmpGrenades { get; private set; } = new List<EmpGrenade>()
        {
            new EmpGrenade() { Type = ItemType.GrenadeFlash },
        };

        /// <summary>
        /// Gets the list of grenade launchers.
        /// </summary>
        [Description("The list of grenade launchers.")]
        public List<GrenadeLauncher> GrenadeLaunchers { get; private set; } = new List<GrenadeLauncher>()
        {
            new GrenadeLauncher() { Type = ItemType.GunLogicer },
        };

        /// <summary>
        /// Gets the list of implosion grenades.
        /// </summary>
        [Description("The list of implosion grenades.")]
        public List<ImplosionGrenade> ImplosionGrenades { get; private set; } = new List<ImplosionGrenade>()
        {
            new ImplosionGrenade() { Type = ItemType.GrenadeFrag },
        };

        /// <summary>
        /// Gets the list of lethal injections.
        /// </summary>
        [Description("The list of lethal injections.")]
        public List<LethalInjection> LethalInjections { get; private set; } = new List<LethalInjection>()
        {
            new LethalInjection() { Type = ItemType.Adrenaline },
        };

        /// <summary>
        /// Gets the list of lucky coins.
        /// </summary>
        [Description("The list of lucky coins.")]
        public List<LuckyCoin> LuckyCoins { get; private set; } = new List<LuckyCoin>()
        {
            new LuckyCoin() { Type = ItemType.Coin },
        };

        /// <summary>
        /// Gets the list of mediGuns.
        /// </summary>
        [Description("The list of mediGuns.")]
        public List<MediGun> MediGuns { get; private set; } = new List<MediGun>()
        {
            new MediGun() { Type = ItemType.GunProject90 },
        };

        /// <summary>
        /// Gets the list of Rocks.
        /// </summary>
        [Description("The list of Rocks.")]
        public List<Rock> Rocks { get; private set; } = new List<Rock>()
        {
            new Rock() { Type = ItemType.SCP018 },
        };

        /// <summary>
        /// Gets the list of Scp127s.
        /// </summary>
        [Description("The list of Scp127s.")]
        public List<Scp127> Scp127s { get; private set; } = new List<Scp127>()
        {
            new Scp127() { Type = ItemType.GunCOM15 },
        };

        /// <summary>
        /// Gets the list of Scp1499s.
        /// </summary>
        [Description("The list of Scp1499s.")]
        public List<Scp1499> Scp1499s { get; private set; } = new List<Scp1499>()
        {
            new Scp1499() { Type = ItemType.SCP268 },
        };

        /// <summary>
        /// Gets the list of shotguns.
        /// </summary>
        [Description("The list of shotguns.")]
        public List<Shotgun> Shotguns { get; private set; } = new List<Shotgun>()
        {
            new Shotgun() { Type = ItemType.GunMP7 },
        };

        /// <summary>
        /// Gets the list of sniper rifles.
        /// </summary>
        [Description("The list of sniper rifles.")]
        public List<SniperRifle> SniperRifle { get; private set; } = new List<SniperRifle>()
        {
            new SniperRifle() { Type = ItemType.GunE11SR },
        };

        /// <summary>
        /// Gets the list of tranquilizer guns.
        /// </summary>
        [Description("The list of tranquilizer guns.")]
        public List<TranquilizerGun> TranquilizerGun { get; private set; } = new List<TranquilizerGun>()
        {
            new TranquilizerGun() { Type = ItemType.GunUSP },
        };

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
