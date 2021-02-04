using System;
using System.Collections.Generic;
using CustomItems.API;
using CustomItems.Configs;
using Exiled.API.Features;
using HarmonyLib;
using MapEvents = Exiled.Events.Handlers.Map;
using PlayerEvents = Exiled.Events.Handlers.Player;
using Random = System.Random;
using Scp049Events = Exiled.Events.Handlers.Scp049;
using Scp079Events = Exiled.Events.Handlers.Scp079;
using Scp096Events = Exiled.Events.Handlers.Scp096;
using Scp106Events = Exiled.Events.Handlers.Scp106;
using Scp914Events = Exiled.Events.Handlers.Scp914;
using ServerEvents = Exiled.Events.Handlers.Server;
using WarheadEvents = Exiled.Events.Handlers.Warhead;

namespace CustomItems
{
    public class Plugin : Plugin<Config>
    {
        public override string Author { get; } = "Galaxy119";
        public override string Name { get; } = "CustomItems";
        public override string Prefix { get; } = "CustomItems";
        public override Version Version { get; } = new Version(1, 3, 0);
        public override Version RequiredExiledVersion { get; } = new Version(2, 1, 29);

        public Methods Methods { get; private set; }
        public EventHandlers EventHandlers { get; private set; }
        public List<CustomItem> ItemManagers { get; set; } = new List<CustomItem>();


        public static Plugin Singleton;
        public Harmony HarmonyInstance;
        public Random Rng = new Random();

        public override void OnEnabled()
        {
            Singleton = this;
            EventHandlers = new EventHandlers(this);
            Methods = new Methods(this);
            
            Config.LoadConfigs();

            Log.Debug($"Checking for Subclassing..", Config.Debug);
            try
            {
                Methods.CheckAndPatchSubclassing();
            }
            catch (Exception)
            {
                Log.Debug($"Subclassing not installed.", Config.Debug);
            }

            Exiled.Events.Handlers.Server.RoundStarted += EventHandlers.OnRoundStart;
            Exiled.Events.Handlers.Server.ReloadedConfigs += EventHandlers.OnReloadingConfigs;
            Exiled.Events.Handlers.Server.WaitingForPlayers += EventHandlers.OnWaitingForPlayers;
            
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            foreach (CustomItem item in ItemManagers)
                item.Destroy();
            ItemManagers.Clear();
            
            Exiled.Events.Handlers.Server.RoundStarted -= EventHandlers.OnRoundStart;
            Exiled.Events.Handlers.Server.ReloadedConfigs -= EventHandlers.OnReloadingConfigs;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= EventHandlers.OnWaitingForPlayers;
            
            HarmonyInstance?.UnpatchAll();
            EventHandlers = null;
            Methods = null;

            base.OnDisabled();
        }
    }
}