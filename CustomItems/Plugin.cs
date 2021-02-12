// <copyright file="Plugin.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CustomItems
{
    using System;
    using System.Collections.Generic;
    using CustomItems.API;
    using CustomItems.Configs;
    using Exiled.API.Features;
    using HarmonyLib;
    using MapEvents = Exiled.Events.Handlers.Map;
    using PlayerEvents = Exiled.Events.Handlers.Player;
    using Random = System.Random;
    using Scp049Events = Exiled.Events.Handlers.Scp049;
    using Scp079Events = Exiled.Events.Handlers.Scp079;
    using Scp096Events = Exiled.Events.Handlers.Scp096;
    using Scp106Events = Exiled.Events.Handlers.Scp106;
    using Scp914Events = Exiled.Events.Handlers.Scp914;
    using ServerEvents = Exiled.Events.Handlers.Server;
    using WarheadEvents = Exiled.Events.Handlers.Warhead;

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
        public override Version Version { get; } = new Version(1, 8, 1);

        /// <inheritdoc/>
        public override Version RequiredExiledVersion { get; } = new Version(2, 1, 30);

        /// <summary>
        /// Gets the Methods class.
        /// </summary>
        public Methods Methods { get; private set; }

        /// <summary>
        /// Gets the EventHandlers class.
        /// </summary>
        public EventHandlers EventHandlers { get; private set; }

        /// <summary>
        /// Gets the list of current Item Managers.
        /// </summary>
        public List<CustomItem> ItemManagers { get; } = new List<CustomItem>();

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            Singleton = this;
            EventHandlers = new EventHandlers(this);
            Methods = new Methods(this);

            Config.LoadConfigs();

            Log.Debug($"Checking for Subclassing..", Config.Debug);
            try
            {
                Methods.CheckAndPatchSubclassing();
            }
            catch (Exception)
            {
                Log.Debug($"Subclassing not installed.", Config.Debug);
            }

            Exiled.Events.Handlers.Server.RoundStarted += EventHandlers.OnRoundStart;
            Exiled.Events.Handlers.Server.ReloadedConfigs += EventHandlers.OnReloadingConfigs;
            Exiled.Events.Handlers.Server.WaitingForPlayers += EventHandlers.OnWaitingForPlayers;

            base.OnEnabled();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            foreach (CustomItem item in ItemManagers)
                item.Destroy();
            ItemManagers.Clear();

            Exiled.Events.Handlers.Server.RoundStarted -= EventHandlers.OnRoundStart;
            Exiled.Events.Handlers.Server.ReloadedConfigs -= EventHandlers.OnReloadingConfigs;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= EventHandlers.OnWaitingForPlayers;

            HarmonyInstance?.UnpatchAll();
            EventHandlers = null;
            Methods = null;

            base.OnDisabled();
        }
    }
}