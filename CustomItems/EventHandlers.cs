using System.Collections.Generic;
using System.Linq;
using CustomItems.Components;
using Exiled.API.Features;
using UnityEngine;

namespace CustomItems
{
    public class EventHandlers
    {
        private readonly Plugin plugin;
        public EventHandlers(Plugin plugin) => this.plugin = plugin;

        public void OnWaitingForPlayers()
        {
            API.RegisterCustomItem(new Shotgun());
            API.RegisterCustomItem(new GrenadeLauncher());
            API.RegisterCustomItem(new SniperRifle());
            
            foreach (CustomItem item in plugin.ItemManagers)
                item.Init();
        }

        public void OnReloadingConfigs()
        {
            plugin.Config.ParseSubclassList();
        }
    }
}