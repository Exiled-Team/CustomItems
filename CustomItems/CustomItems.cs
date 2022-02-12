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
            Config.ItemConfigs.Scp127s.Register();
            Config.ItemConfigs.Scp714s.Register();
            Config.ItemConfigs.Scp1499s.Register();
            Config.ItemConfigs.Scp2818s.Register();
            Config.ItemConfigs.AutoGuns.Register();
            Config.ItemConfigs.C4Charges.Register();
            Config.ItemConfigs.DeflectorSheilds.Register();
            Config.ItemConfigs.EmpGrenades.Register();
            Config.ItemConfigs.GrenadeLaunchers.Register();
            Config.ItemConfigs.ImplosionGrenades.Register();
            Config.ItemConfigs.LethalInjections.Register();
            Config.ItemConfigs.LuckyCoins.Register();
            Config.ItemConfigs.MediGuns.Register();
            Config.ItemConfigs.SniperRifle.Register();
            Config.ItemConfigs.TranquilizerGun.Register();
            Config.ItemConfigs.AntiMemeticPills.Register();

            Server.ReloadedConfigs += serverHandler.OnReloadingConfigs;

            base.OnEnabled();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            Config.ItemConfigs.Scp127s.Unregister();
            Config.ItemConfigs.Scp714s.Unregister();
            Config.ItemConfigs.Scp1499s.Unregister();
            Config.ItemConfigs.Scp2818s.Unregister();
            Config.ItemConfigs.AutoGuns.Unregister();
            Config.ItemConfigs.C4Charges.Unregister();
            Config.ItemConfigs.DeflectorSheilds.Unregister();
            Config.ItemConfigs.EmpGrenades.Unregister();
            Config.ItemConfigs.GrenadeLaunchers.Unregister();
            Config.ItemConfigs.ImplosionGrenades.Unregister();
            Config.ItemConfigs.LethalInjections.Unregister();
            Config.ItemConfigs.LuckyCoins.Unregister();
            Config.ItemConfigs.MediGuns.Unregister();
            Config.ItemConfigs.SniperRifle.Unregister();
            Config.ItemConfigs.TranquilizerGun.Unregister();
            Config.ItemConfigs.AntiMemeticPills.Unregister();

            Server.ReloadedConfigs -= serverHandler.OnReloadingConfigs;

            harmonyInstance?.UnpatchAll();

            serverHandler = null;
            Instance = null;

            base.OnDisabled();
        }
    }
}
