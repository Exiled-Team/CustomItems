// -----------------------------------------------------------------------
// <copyright file="CustomItems.cs" company="Galaxy119 and iopietro">
// Copyright (c) Galaxy119 and iopietro. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable SA1200
namespace CustomItems
{
    using System;
    using Events;
    using Exiled.API.Features;
    using Exiled.CustomItems.API;
    using Exiled.CustomItems.API.Features;
    using HarmonyLib;
    using Server = Exiled.Events.Handlers.Server;

    /// <inheritdoc />
    public class CustomItems : Plugin<Config>
    {
        /// <summary>
        /// Random Number Generator.
        /// </summary>
        public Random Rng = new Random();

        private Harmony harmonyInstance;

        private ServerHandler serverHandler;

        /// <summary>
        /// Gets the Plugin instance.
        /// </summary>
        public static CustomItems Instance { get; private set; }

        /// <inheritdoc/>
        public override Version RequiredExiledVersion { get; } = new Version(3, 0, 0);

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            Instance = this;
            serverHandler = new ServerHandler();

            harmonyInstance = new Harmony($"com.{nameof(CustomItems)}.galaxy-{DateTime.Now.Ticks}");
            harmonyInstance.PatchAll();

            Config.LoadItems();
            CustomItem.RegisterItems();

            Server.ReloadedConfigs += serverHandler.OnReloadingConfigs;

            base.OnEnabled();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            CustomItem.RegisterItems();

            Server.ReloadedConfigs -= serverHandler.OnReloadingConfigs;

            harmonyInstance?.UnpatchAll();

            serverHandler = null;
            Instance = null;

            base.OnDisabled();
        }
    }
}
