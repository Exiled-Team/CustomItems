using System.Collections.Generic;
using System.Linq;
using CustomItems.Items;
using CustomItems.API;
using Exiled.API.Features;
using MEC;
using UnityEngine;

namespace CustomItems
{
    public class EventHandlers
    {
        private readonly Plugin plugin;
        public EventHandlers(Plugin plugin) => this.plugin = plugin;

        //This is to prevent making more new item managers when they aren't needed, that could get messy.
        private bool first = true;

        public void OnWaitingForPlayers()
        {
            if (first)
            {
                if (plugin.Config.Shotgun)
                    new Shotgun(plugin.Config.ItemConfigs.ShotgunCfg.ItemType, plugin.Config.ItemConfigs.ShotgunCfg.SpreadCount * 2, 1).RegisterCustomItem();
                
                if (plugin.Config.GrenadeLauncher)
                    new GrenadeLauncher(plugin.Config.ItemConfigs.GlCfg.ItemType, plugin.Config.ItemConfigs.GlCfg.ClipSize,  2).RegisterCustomItem();
                
                if (plugin.Config.SniperRifle)
                    new SniperRifle(plugin.Config.ItemConfigs.SniperCfg.ItemType, plugin.Config.ItemConfigs.SniperCfg.ClipSize, 3).RegisterCustomItem();
                
                if (plugin.Config.Scp127)
                    new Scp127(plugin.Config.ItemConfigs.Scp127Cfg.ItemType, plugin.Config.ItemConfigs.Scp127Cfg.ClipSize, 4).RegisterCustomItem();
                
                if (plugin.Config.ImplosionGrenade)
                    new ImplosionGrenade(ItemType.GrenadeFrag, 5).RegisterCustomItem();
                
                if (plugin.Config.EmpGrenade)
                    new EmpGrenade(ItemType.GrenadeFlash, 6).RegisterCustomItem();
                
                if (plugin.Config.LethalInjection)
                    new LethalInjection(ItemType.Adrenaline, 7).RegisterCustomItem();
                
                if (plugin.Config.MediGun)
                    new MediGun(plugin.Config.ItemConfigs.MediCfg.ItemType, plugin.Config.ItemConfigs.MediCfg.ClipSize, 8).RegisterCustomItem();

                if (plugin.Config.TranqGun)
                    new TranqGun(plugin.Config.ItemConfigs.TranqCfg.ItemType, plugin.Config.ItemConfigs.TranqCfg.ClipSize, 9).RegisterCustomItem();

                if (plugin.Config.LuckyCoin)
                    new LuckyCoin(ItemType.Coin, 10).RegisterCustomItem();
                
                plugin.Config.ParseSubclassList();
                
                first = false;
            }
        }

        public void OnReloadingConfigs()
        {
            plugin.Config.ParseSubclassList();
            plugin.Config.LoadConfigs();
        }

        public void OnRoundStart()
        {
            foreach (CustomItem item in plugin.ItemManagers)
            {
                if (item.SpawnLocations != null)
                {
                    foreach (KeyValuePair<SpawnLocation, float> spawn in item.SpawnLocations)
                    {
                        Log.Debug($"Attempting to spawn {item.Name} at {spawn.Key}", plugin.Config.Debug);
                        if (plugin.Rng.Next(100) <= spawn.Value)
                        {
                            item.SpawnItem(spawn.Key.TryGetLocation());
                            Log.Debug($"Spawned {item.Name} at {spawn.Key}", plugin.Config.Debug);
                        }
                    }
                }
            }
        }
    }
}