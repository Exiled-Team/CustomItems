// <copyright file="Methods.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CustomItems
{
    using System;
    using System.Linq;
    using Exiled.Loader;
    using HarmonyLib;

    /// <summary>
    /// Methods.
    /// </summary>
    public class Methods
    {
        private readonly Plugin plugin;

        /// <summary>
        /// Initializes a new instance of the <see cref="Methods"/> class.
        /// </summary>
        /// <param name="plugin">The <see cref="Plugin"/> class.</param>
        public Methods(Plugin plugin) => this.plugin = plugin;

        /// <summary>
        /// Checks if subclassing is installed then patches.
        /// </summary>
        internal void CheckAndPatchSubclassing()
        {
            if (Loader.Plugins.All(p => p.Name != "Subclass"))
                return;

            plugin.HarmonyInstance = new Harmony($"com.galaxy.CI=-{DateTime.UtcNow.Ticks}");
            plugin.HarmonyInstance.PatchAll();
        }
    }
}