// <copyright file="Plugin.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CustomItems
{
    using System;
    using System.Collections.Generic;
    using CustomItems.Configs;
    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;
    using HarmonyLib;
    using Scp914Events = Exiled.Events.Handlers.Scp914;
    using Server = Exiled.Events.Handlers.Server;

    /// <inheritdoc />
    public class Plugin : Plugin<Config>
    {
        /// <summary>
        /// The static Plugin reference.
        /// </summary>
        public static Plugin Singleton;

        /// <summary>
        /// The Harmony instance.
        /// </summary>
        public Harmony HarmonyInstance;

        /// <summary>
        /// The Random object.
        /// </summary>
        public Random Rng = new Random();

        /// <inheritdoc/>
        public override string Author { get; } = "Galaxy119";

        /// <inheritdoc/>
        public override string Name { get; } = "CustomItems";

        /// <inheritdoc/>
        public override string Prefix { get; } = "CustomItems";

        /// <inheritdoc/>
        public override Version RequiredExiledVersion { get; } = new Version(2, 3, 0);

        /// <summary>
        /// Gets the Methods class.
        /// </summary>
        public Methods Methods { get; private set; }

        /// <summary>
        /// Gets the EventHandlers class.
        /// </summary>
        public EventHandlers EventHandlers { get; private set; }

        /// <summary>
        /// Gets the Internal list of item managers.
        /// </summary>
        internal List<CustomItem> ItemManagers { get; } = new List<CustomItem>();

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            Singleton = this;
            EventHandlers = new EventHandlers(this);
            Methods = new Methods(this);

            Config.LoadConfigs();

            Log.Debug("Checking for Subclassing..", Config.Debug);
            try
            {
                Methods.CheckAndPatchSubclassing();
            }
            catch (Exception)
            {
                Log.Debug("Subclassing not installed.", Config.Debug);
            }

            Server.ReloadedConfigs += EventHandlers.OnReloadingConfigs;
            Server.WaitingForPlayers += EventHandlers.OnWaitingForPlayers;

            base.OnEnabled();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            foreach (CustomItem item in ItemManagers)
                item.TryUnregister();

            Server.ReloadedConfigs -= EventHandlers.OnReloadingConfigs;
            Server.WaitingForPlayers -= EventHandlers.OnWaitingForPlayers;

            HarmonyInstance?.UnpatchAll();
            EventHandlers = null;
            Methods = null;

            base.OnDisabled();
        }
    }
}