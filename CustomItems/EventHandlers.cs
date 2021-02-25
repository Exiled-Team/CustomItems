// <copyright file="EventHandlers.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CustomItems
{
    using System.Collections.Generic;
    using CustomItems.API;
    using CustomItems.Items;
    using Exiled.API.Features;

    /// <summary>
    /// Event Handlers.
    /// </summary>
    public class EventHandlers
    {
        private readonly Plugin plugin;

        // This is to prevent making more new item managers when they aren't needed, that could get messy.
        private bool first = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlers"/> class.
        /// </summary>
        /// <param name="plugin">The <see cref="Plugin"/> class.</param>
        public EventHandlers(Plugin plugin) => this.plugin = plugin;

        /// <summary>
        /// WaitingForPlayers Handler.
        /// </summary>
        public void OnWaitingForPlayers()
        {
            if (first)
            {
                if (plugin.Config.Shotgun)
                    new Shotgun(plugin.Config.ItemConfigs.ShotgunCfg.ItemType, plugin.Config.ItemConfigs.ShotgunCfg.SpreadCount * 2, Plugin.Singleton.Config.ItemConfigs.ShotgunCfg.Id).RegisterCustomItem();

                if (plugin.Config.GrenadeLauncher)
                    new GrenadeLauncher(plugin.Config.ItemConfigs.GlCfg.ItemType, plugin.Config.ItemConfigs.GlCfg.ClipSize,  Plugin.Singleton.Config.ItemConfigs.GlCfg.Id).RegisterCustomItem();

                if (plugin.Config.SniperRifle)
                    new SniperRifle(plugin.Config.ItemConfigs.SniperCfg.ItemType, plugin.Config.ItemConfigs.SniperCfg.ClipSize, Plugin.Singleton.Config.ItemConfigs.SniperCfg.Id).RegisterCustomItem();

                if (plugin.Config.Scp127)
                    new Scp127(plugin.Config.ItemConfigs.Scp127Cfg.ItemType, plugin.Config.ItemConfigs.Scp127Cfg.ClipSize, Plugin.Singleton.Config.ItemConfigs.Scp127Cfg.Id).RegisterCustomItem();

                if (plugin.Config.ImplosionGrenade)
                    new ImplosionGrenade(ItemType.GrenadeFrag, Plugin.Singleton.Config.ItemConfigs.ImpCfg.Id).RegisterCustomItem();

                if (plugin.Config.EmpGrenade)
                    new EmpGrenade(ItemType.GrenadeFlash, Plugin.Singleton.Config.ItemConfigs.EmpCfg.Id).RegisterCustomItem();

                if (plugin.Config.LethalInjection)
                    new LethalInjection(ItemType.Adrenaline, Plugin.Singleton.Config.ItemConfigs.LethalCfg.Id).RegisterCustomItem();

                if (plugin.Config.MediGun)
                    new MediGun(plugin.Config.ItemConfigs.MediCfg.ItemType, plugin.Config.ItemConfigs.MediCfg.ClipSize, Plugin.Singleton.Config.ItemConfigs.MediCfg.Id).RegisterCustomItem();

                if (plugin.Config.TranqGun)
                    new TranqGun(plugin.Config.ItemConfigs.TranqCfg.ItemType, plugin.Config.ItemConfigs.TranqCfg.ClipSize, Plugin.Singleton.Config.ItemConfigs.TranqCfg.Id).RegisterCustomItem();

                if (plugin.Config.LuckyCoin)
                    new LuckyCoin(ItemType.Coin, Plugin.Singleton.Config.ItemConfigs.LuckyCfg.Id).RegisterCustomItem();

                if (plugin.Config.Rock)
                    new Items.Rock(ItemType.SCP018, plugin.Config.ItemConfigs.RockCfg.Id).RegisterCustomItem();

                if (plugin.Config.Scp1499)
                    new Scp1499(ItemType.SCP268, Plugin.Singleton.Config.ItemConfigs.Scp1499Cfg.Id).RegisterCustomItem();

                if (plugin.Config.Scp714)
                    new Scp714(ItemType.Coin, Plugin.Singleton.Config.ItemConfigs.Scp714Cfg.Id).RegisterCustomItem();

                plugin.Config.ParseSubclassList();

                first = false;
            }
        }

        /// <summary>
        /// OnReloadingConfigs handler.
        /// </summary>
        public void OnReloadingConfigs()
        {
            plugin.Config.ParseSubclassList();
            plugin.Config.LoadConfigs();
        }

        /// <summary>
        /// OnRoundStart handlers.
        /// </summary>
        public void OnRoundStart()
        {
            foreach (CustomItem item in plugin.ItemManagers)
            {
                if (item.SpawnLocations == null)
                    continue;

                int count = 0;

                foreach (KeyValuePair<SpawnLocation, float> spawn in item.SpawnLocations)
                {
                    Log.Debug($"Attempting to spawn {item.Name} at {spawn.Key}", plugin.Config.Debug);
                    if (plugin.Rng.Next(100) >= spawn.Value || (item.SpawnLimit > 0 && count >= item.SpawnLimit))
                        continue;

                    count++;
                    item.SpawnItem(spawn.Key.TryGetLocation());
                    Log.Debug($"Spawned {item.Name} at {spawn.Key}", plugin.Config.Debug);
                }
            }
        }
    }
}
