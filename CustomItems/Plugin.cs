using System;
using CustomItems.Components;
using Exiled.API.Features;
using HarmonyLib;
using MapEvents = Exiled.Events.Handlers.Map;
using Object = UnityEngine.Object;
using PlayerEvents = Exiled.Events.Handlers.Player;
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
        public override Version Version { get; } = new Version(1, 0, 0);
        public override Version RequiredExiledVersion { get; } = new Version(2, 1, 28);

        public Methods Methods { get; private set; }
        public EventHandlers EventHandlers { get; private set; }
        public SniperRifle SniperRifleComponent { get; set; }
        public GrenadeLauncher GrenadeLauncherComponent { get; set; }

        public static Plugin Singleton;
        public Harmony HarmonyInstance;

        public override void OnEnabled()
        {
            Singleton = this;
            EventHandlers = new EventHandlers(this);
            Methods = new Methods(this);

            HarmonyInstance = new Harmony($"com.galaxy.customItems-{DateTime.UtcNow.Ticks}");
            HarmonyInstance.PatchAll();

            Exiled.Events.Handlers.Server.WaitingForPlayers += EventHandlers.OnWaitingForPlayers;
            
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            if (SniperRifleComponent != null)
                Object.Destroy(SniperRifleComponent);
            
            HarmonyInstance.UnpatchAll();
            EventHandlers = null;
            Methods = null;

            base.OnDisabled();
        }
    }
}