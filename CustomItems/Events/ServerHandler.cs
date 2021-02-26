// -----------------------------------------------------------------------
// <copyright file="ServerHandler.cs" company="Galaxy199 and iopietro">
// Copyright (c) Galaxy199 and iopietro. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems
{
    using Exiled.CustomItems.API;
    using Items;
    using static CustomItems;

    /// <summary>
    /// Event Handlers.
    /// </summary>
    public class ServerHandler
    {
        // This is to prevent making more new item managers when they aren't needed, that could get messy.
        private bool first = true;

        /// <summary>
        /// WaitingForPlayers Handler.
        /// </summary>
        public void OnWaitingForPlayers()
        {
            if (first)
            {
                /*if (Instance.Config.SniperRifle)
                {
                    SniperRifle sniper = new SniperRifle(Instance.Config.ItemConfigs.SniperCfg.ItemType, Instance.Config.ItemConfigs.SniperCfg.ClipSize, Instance.Config.ItemConfigs.SniperCfg.Id);
                    if (sniper.TryRegister())
                        Instance.ItemManagers.Add(sniper);
                }

                if (Instance.Config.Shotgun)
                {
                    Shotgun shotgun = new Shotgun(Instance.Config.ItemConfigs.ShotgunCfg.ItemType, Instance.Config.ItemConfigs.ShotgunCfg.SpreadCount * 2, Instance.Config.ItemConfigs.ShotgunCfg.Id);
                    if (shotgun.TryRegister())
                        Instance.ItemManagers.Add(shotgun);
                }

                if (Instance.Config.GrenadeLauncher)
                {
                    GrenadeLauncher launcher = new GrenadeLauncher(Instance.Config.ItemConfigs.GlCfg.ItemType, Instance.Config.ItemConfigs.GlCfg.ClipSize, Instance.Config.ItemConfigs.GlCfg.Id);
                    if (launcher.TryRegister())
                        Instance.ItemManagers.Add(launcher);
                }

                if (Instance.Config.Scp127)
                {
                    Scp127 scp127 = new Scp127(Instance.Config.ItemConfigs.Scp127Cfg.ItemType, Instance.Config.ItemConfigs.Scp127Cfg.ClipSize, Instance.Config.ItemConfigs.Scp127Cfg.Id);
                    if (scp127.TryRegister())
                        Instance.ItemManagers.Add(scp127);
                }

                if (Instance.Config.ImplosionGrenade)
                {
                    ImplosionGrenade implosionGrenade = new ImplosionGrenade(ItemType.GrenadeFrag, Instance.Config.ItemConfigs.ImpCfg.Id);
                    if (implosionGrenade.TryRegister())
                        Instance.ItemManagers.Add(implosionGrenade);
                }*/

                if (Instance.Config.EmpGrenades.Count > 0)
                    Instance.Config.EmpGrenades.Register();

                /*if (Instance.Config.LethalInjection)
                {
                    LethalInjection lethalInjection = new LethalInjection(ItemType.Adrenaline, Instance.Config.ItemConfigs.LethalCfg.Id);
                    if (lethalInjection.TryRegister())
                        Instance.ItemManagers.Add(lethalInjection);
                }

                if (Instance.Config.MediGun)
                {
                    MediGun mediGun = new MediGun(Instance.Config.ItemConfigs.MediCfg.ItemType, Instance.Config.ItemConfigs.MediCfg.ClipSize, Instance.Config.ItemConfigs.MediCfg.Id);
                    if (mediGun.TryRegister())
                        Instance.ItemManagers.Add(mediGun);
                }

                if (Instance.Config.TranqGun)
                {
                    TranqGun tranqGun = new TranqGun(Instance.Config.ItemConfigs.TranqCfg.ItemType, Instance.Config.ItemConfigs.TranqCfg.ClipSize, Instance.Config.ItemConfigs.TranqCfg.Id);
                    if (tranqGun.TryRegister())
                        Instance.ItemManagers.Add(tranqGun);
                }

                if (Instance.Config.LuckyCoin)
                {
                    LuckyCoin luckyCoin = new LuckyCoin(ItemType.Coin, Instance.Config.ItemConfigs.LuckyCfg.Id);
                    if (luckyCoin.TryRegister())
                        Instance.ItemManagers.Add(luckyCoin);
                }

                if (Instance.Config.Rock)
                {
                    Items.Rock rock = new Items.Rock(ItemType.SCP018, Instance.Config.ItemConfigs.RockCfg.Id);
                    if (rock.TryRegister())
                        Instance.ItemManagers.Add(rock);
                }

                if (Instance.Config.Scp1499)
                {
                    Scp1499 scp1499 = new Scp1499(ItemType.SCP268, Instance.Config.ItemConfigs.Scp1499Cfg.Id);
                    if (scp1499.TryRegister())
                        Instance.ItemManagers.Add(scp1499);
                }*/

                Instance.Config.ParseSubclassList();

                first = false;
            }
        }

        /// <summary>
        /// OnReloadingConfigs handler.
        /// </summary>
        public void OnReloadingConfigs()
        {
            Instance.Config.ParseSubclassList();
            Instance.Config.Load();
        }
    }
}
