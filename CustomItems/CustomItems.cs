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
        public override Version RequiredExiledVersion { get; } = new Version(2, 3, 0);

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            serverHandler = new ServerHandler();

            RegisterItems();

            Log.Debug("Checking for Subclassing...", Config.IsDebugEnabled);

            try
            {
                CheckAndPatchSubclassing();
            }
            catch (Exception)
            {
                Log.Debug("Subclassing not installed.", Config.IsDebugEnabled);
            }

            Server.ReloadedConfigs += serverHandler.OnReloadingConfigs;

            base.OnEnabled();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            UnregisterItems();

            Server.ReloadedConfigs -= serverHandler.OnReloadingConfigs;

            harmonyInstance?.UnpatchAll();

            serverHandler = null;

            base.OnDisabled();
        }

        private void RegisterItems()
        {
            if (Instance.Config.EmpGrenades != null)
                Instance.Config.EmpGrenades.Register();

            if (Instance.Config.GrenadeLaunchers != null)
                Instance.Config.GrenadeLaunchers.Register();

            if (Instance.Config.ImplosionGrenades != null)
                Instance.Config.ImplosionGrenades.Register();

            if (Instance.Config.LethalInjections != null)
                Instance.Config.LethalInjections.Register();

            if (Instance.Config.LuckyCoins != null)
                Instance.Config.LuckyCoins.Register();

            if (Instance.Config.MediGuns != null)
                Instance.Config.MediGuns.Register();

            if (Instance.Config.Rocks != null)
                Instance.Config.Rocks.Register();

            if (Instance.Config.Scp127s != null)
                Instance.Config.Scp127s.Register();

            if (Instance.Config.Scp1499s != null)
                Instance.Config.Scp1499s.Register();

            if (Instance.Config.Shotguns != null)
                Instance.Config.Shotguns.Register();

            if (Instance.Config.SniperRifle != null)
                Instance.Config.SniperRifle.Register();
        }

        private void UnregisterItems()
        {
            if (Instance.Config.EmpGrenades != null)
                Instance.Config.EmpGrenades.Unregister();

            if (Instance.Config.GrenadeLaunchers != null)
                Instance.Config.GrenadeLaunchers.Unregister();

            if (Instance.Config.ImplosionGrenades != null)
                Instance.Config.ImplosionGrenades.Unregister();

            if (Instance.Config.LethalInjections != null)
                Instance.Config.LethalInjections.Unregister();

            if (Instance.Config.LuckyCoins != null)
                Instance.Config.LuckyCoins.Unregister();

            if (Instance.Config.MediGuns != null)
                Instance.Config.MediGuns.Unregister();

            if (Instance.Config.Rocks != null)
                Instance.Config.Rocks.Unregister();

            if (Instance.Config.Scp127s != null)
                Instance.Config.Scp127s.Unregister();

            if (Instance.Config.Scp1499s != null)
                Instance.Config.Scp1499s.Unregister();

            if (Instance.Config.Shotguns != null)
                Instance.Config.Shotguns.Unregister();

            if (Instance.Config.SniperRifle != null)
                Instance.Config.SniperRifle.Unregister();
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