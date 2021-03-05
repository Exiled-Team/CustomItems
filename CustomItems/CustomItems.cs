// -----------------------------------------------------------------------
// <copyright file="CustomItems.cs" company="Galaxy199 and iopietro">
// Copyright (c) Galaxy199 and iopietro. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems
{
    using System;
    using System.Linq;
    using Events;
    using Exiled.API.Features;
    using Exiled.CustomItems.API;
    using HarmonyLib;
    using Server = Exiled.Events.Handlers.Server;

    /// <inheritdoc />
    public class CustomItems : Plugin<Config>
    {
        /// <summary>
        /// Random Number Generator.
        /// </summary>
        public Random Rng = new Random();

        private static readonly CustomItems InstanceValue = new CustomItems();

        private Harmony harmonyInstance;

        private ServerHandler serverHandler;

        private CustomItems()
        {
        }

        /// <summary>
        /// Gets the Plugin instance.
        /// </summary>
        public static CustomItems Instance => InstanceValue;

        /// <inheritdoc/>
        public override Version RequiredExiledVersion { get; } = new Version(2, 4, 1);

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            serverHandler = new ServerHandler();

            Server.ReloadedConfigs += serverHandler.OnReloadingConfigs;

            base.OnEnabled();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            UnregisterItems();

            Server.ReloadedConfigs -= serverHandler.OnReloadingConfigs;

            serverHandler = null;

            base.OnDisabled();
        }

        private void RegisterItems()
        {
            Instance.Config.ItemConfigs.SCP2818?.Register();
        }

        private void UnregisterItems()
        {
            Instance.Config.ItemConfigs.SCP2818?.Unregister();

        }
    }
}