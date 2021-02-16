// <copyright file="EventHandlers.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CustomItems
{
    using CustomItems.Items;
    using Exiled.CustomItems.API;

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
                if (plugin.Config.SniperRifle)
                {
                    SniperRifle sniper = new SniperRifle(plugin.Config.ItemConfigs.SniperCfg.ItemType, plugin.Config.ItemConfigs.SniperCfg.ClipSize, plugin.Config.ItemConfigs.SniperCfg.Id);
                    if (sniper.RegisterCustomItem())
                        plugin.ItemManagers.Add(sniper);
                }

                if (plugin.Config.Shotgun)
                {
                    Shotgun shotgun = new Shotgun(plugin.Config.ItemConfigs.ShotgunCfg.ItemType, plugin.Config.ItemConfigs.ShotgunCfg.SpreadCount * 2, Plugin.Singleton.Config.ItemConfigs.ShotgunCfg.Id);
                    if (shotgun.RegisterCustomItem())
                        plugin.ItemManagers.Add(shotgun);
                }

                if (plugin.Config.GrenadeLauncher)
                {
                    GrenadeLauncher launcher = new GrenadeLauncher(plugin.Config.ItemConfigs.GlCfg.ItemType, plugin.Config.ItemConfigs.GlCfg.ClipSize, Plugin.Singleton.Config.ItemConfigs.GlCfg.Id);
                    if (launcher.RegisterCustomItem())
                        plugin.ItemManagers.Add(launcher);
                }

                if (plugin.Config.Scp127)
                {
                    Scp127 scp127 = new Scp127(plugin.Config.ItemConfigs.Scp127Cfg.ItemType, plugin.Config.ItemConfigs.Scp127Cfg.ClipSize, Plugin.Singleton.Config.ItemConfigs.Scp127Cfg.Id);
                    if (scp127.RegisterCustomItem())
                        plugin.ItemManagers.Add(scp127);
                }

                if (plugin.Config.ImplosionGrenade)
                {
                    ImplosionGrenade implosionGrenade = new ImplosionGrenade(ItemType.GrenadeFrag, Plugin.Singleton.Config.ItemConfigs.ImpCfg.Id);
                    if (implosionGrenade.RegisterCustomItem())
                        plugin.ItemManagers.Add(implosionGrenade);
                }

                if (plugin.Config.EmpGrenade)
                {
                    EmpGrenade empGrenade = new EmpGrenade(ItemType.GrenadeFlash, Plugin.Singleton.Config.ItemConfigs.EmpCfg.Id);
                    if (empGrenade.RegisterCustomItem())
                        plugin.ItemManagers.Add(empGrenade);
                }

                if (plugin.Config.LethalInjection)
                {
                    LethalInjection lethalInjection = new LethalInjection(ItemType.Adrenaline, Plugin.Singleton.Config.ItemConfigs.LethalCfg.Id);
                    if (lethalInjection.RegisterCustomItem())
                        plugin.ItemManagers.Add(lethalInjection);
                }

                if (plugin.Config.MediGun)
                {
                    MediGun mediGun = new MediGun(plugin.Config.ItemConfigs.MediCfg.ItemType, plugin.Config.ItemConfigs.MediCfg.ClipSize, Plugin.Singleton.Config.ItemConfigs.MediCfg.Id);
                    if (mediGun.RegisterCustomItem())
                        plugin.ItemManagers.Add(mediGun);
                }

                if (plugin.Config.TranqGun)
                {
                    TranqGun tranqGun = new TranqGun(plugin.Config.ItemConfigs.TranqCfg.ItemType, plugin.Config.ItemConfigs.TranqCfg.ClipSize, Plugin.Singleton.Config.ItemConfigs.TranqCfg.Id);
                    if (tranqGun.RegisterCustomItem())
                        plugin.ItemManagers.Add(tranqGun);
                }

                if (plugin.Config.LuckyCoin)
                {
                    LuckyCoin luckyCoin = new LuckyCoin(ItemType.Coin, Plugin.Singleton.Config.ItemConfigs.LuckyCfg.Id);
                    if (luckyCoin.RegisterCustomItem())
                        plugin.ItemManagers.Add(luckyCoin);
                }

                if (plugin.Config.Rock)
                {
                    Items.Rock rock = new Items.Rock(ItemType.SCP018, plugin.Config.ItemConfigs.RockCfg.Id);
                    if (rock.RegisterCustomItem())
                        plugin.ItemManagers.Add(rock);
                }

                if (plugin.Config.Scp1499)
                {
                    Scp1499 scp1499 = new Scp1499(ItemType.SCP268, Plugin.Singleton.Config.ItemConfigs.Scp1499Cfg.Id);
                    if (scp1499.RegisterCustomItem())
                        plugin.ItemManagers.Add(scp1499);
                }

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
    }
}
