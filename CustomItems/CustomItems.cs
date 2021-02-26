// -----------------------------------------------------------------------
// <copyright file="CustomItems.cs" company="Galaxy199 and iopietro">
// Copyright (c) Galaxy199 and iopietro. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;
    using HarmonyLib;
    using Server = Exiled.Events.Handlers.Server;

    /// <inheritdoc />
    public class CustomItems : Plugin<Config>
    {
        private static readonly CustomItems InstanceValue = new CustomItems();

        private Harmony harmonyInstance;

        private CustomItems()
        {
        }

        /// <summary>
        /// Gets the Plugin instance.
        /// </summary>
        public static CustomItems Instance => InstanceValue;

        /// <inheritdoc/>
        public override Version RequiredExiledVersion { get; } = new Version(2, 3, 0);

        /// <summary>
        /// Gets the EventHandlers class.
        /// </summary>
        public ServerHandler EventHandlers { get; private set; }

        /// <summary>
        /// Gets the Internal list of item managers.
        /// </summary>
        internal List<CustomItem> ItemManagers { get; } = new List<CustomItem>();

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            EventHandlers = new ServerHandler();

            Config.Load();

            Log.Debug("Checking for Subclassing...", Config.Debug);

            try
            {
                CheckAndPatchSubclassing();
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

            harmonyInstance?.UnpatchAll();

            EventHandlers = null;

            base.OnDisabled();
        }

        private void CheckAndPatchSubclassing()
        {
            if (Exiled.Loader.Loader.Plugins.All(pugin => pugin.Name != "Subclass"))
                return;

            Instance.harmonyInstance = new Harmony($"com.customitems.{DateTime.UtcNow.Ticks}");
            Instance.harmonyInstance.PatchAll();
        }
    }
}