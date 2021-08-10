// -----------------------------------------------------------------------
// <copyright file="CustomItems.cs" company="Galaxy119 and iopietro">
// Copyright (c) Galaxy119 and iopietro. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable SA1200
using CustomItems.Patches;

namespace CustomItems
{
    using System;
    using System.Linq;
    using Events;
    using Exiled.API.Features;
    using Exiled.CustomItems.API;
    using HarmonyLib;
    using Subclass;
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

        private PlayerHandler playerHandler;

        private CustomItems()
        {
        }

        /// <summary>
        /// Gets the Plugin instance.
        /// </summary>
        public static CustomItems Instance => InstanceValue;

        /// <inheritdoc/>
        public override Version RequiredExiledVersion { get; } = new Version(2, 12, 0);

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            serverHandler = new ServerHandler();
            playerHandler = new PlayerHandler();

            harmonyInstance = new Harmony($"com.{nameof(CustomItems)}.galaxy-{DateTime.Now.Ticks}");
            harmonyInstance.PatchAll();

            Config.LoadItems();
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
            Events.AddClassEvent.AddClass -= playerHandler.OnAddingSubclass;

            harmonyInstance?.UnpatchAll();

            serverHandler = null;
            playerHandler = null;

            base.OnDisabled();
        }

        private void RegisterItems()
        {
            Instance.Config.ItemConfigs.EmpGrenades?.Register();

            Instance.Config.ItemConfigs.GrenadeLaunchers?.Register();

            Instance.Config.ItemConfigs.ImplosionGrenades?.Register();

            Instance.Config.ItemConfigs.LethalInjections?.Register();

            Instance.Config.ItemConfigs.LuckyCoins?.Register();

            Instance.Config.ItemConfigs.MediGuns?.Register();

            Instance.Config.ItemConfigs.Rocks?.Register();

            Instance.Config.ItemConfigs.Scp127s?.Register();

            Instance.Config.ItemConfigs.Scp1499s?.Register();

            Instance.Config.ItemConfigs.Shotguns?.Register();

            Instance.Config.ItemConfigs.SniperRifle?.Register();

            Instance.Config.ItemConfigs.TranquilizerGun?.Register();

            Instance.Config.ItemConfigs.Scp714s?.Register();

            Instance.Config.ItemConfigs.AntiMemeticPills?.Register();

            Instance.Config.ItemConfigs.DeflectorSheilds?.Register();

            Instance.Config.ItemConfigs.Scp2818s?.Register();

            Instance.Config.ItemConfigs.C4Charges?.Register();

            Instance.Config.ItemConfigs.AutoGuns?.Register();
        }

        private void UnregisterItems()
        {
            Instance.Config.ItemConfigs.EmpGrenades?.Unregister();

            Instance.Config.ItemConfigs.GrenadeLaunchers?.Unregister();

            Instance.Config.ItemConfigs.ImplosionGrenades?.Unregister();

            Instance.Config.ItemConfigs.LethalInjections?.Unregister();

            Instance.Config.ItemConfigs.LuckyCoins?.Unregister();

            Instance.Config.ItemConfigs.MediGuns?.Unregister();

            Instance.Config.ItemConfigs.Rocks?.Unregister();

            Instance.Config.ItemConfigs.Scp127s?.Unregister();

            Instance.Config.ItemConfigs.Scp1499s?.Unregister();

            Instance.Config.ItemConfigs.Shotguns?.Unregister();

            Instance.Config.ItemConfigs.SniperRifle?.Unregister();

            Instance.Config.ItemConfigs.TranquilizerGun?.Register();

            Instance.Config.ItemConfigs.Scp714s?.Unregister();

            Instance.Config.ItemConfigs.AntiMemeticPills?.Unregister();

            Instance.Config.ItemConfigs.DeflectorSheilds?.Unregister();

            Instance.Config.ItemConfigs.Scp2818s?.Unregister();

            Instance.Config.ItemConfigs.C4Charges?.Unregister();

            Instance.Config.ItemConfigs.AutoGuns?.Unregister();
        }

        private void CheckAndPatchSubclassing()
        {
            if (Exiled.Loader.Loader.Plugins.All(pugin => pugin.Name != "Subclass"))
                return;

            Config.ParseSubclassList();
            Instance.harmonyInstance.Patch(HarmonyLib.AccessTools.Method(typeof(TrackingAndMethods), nameof(TrackingAndMethods.AddClass)), postfix: new HarmonyMethod(HarmonyLib.AccessTools.Method(typeof(AddClass), nameof(AddClass.Postfix))));
            Events.AddClassEvent.AddClass += playerHandler.OnAddingSubclass;
        }
    }
}
