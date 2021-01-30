using CustomItems.Components;
using Exiled.API.Features;

namespace CustomItems
{
    public class EventHandlers
    {
        private readonly Plugin plugin;
        public EventHandlers(Plugin plugin) => this.plugin = plugin;

        public void OnWaitingForPlayers()
        {
            if (plugin.SniperRifleComponent == null)
               plugin.SniperRifleComponent = Server.Host.GameObject.AddComponent<SniperRifle>();
            if (plugin.GrenadeLauncherComponent == null)
                plugin.GrenadeLauncherComponent = Server.Host.GameObject.AddComponent<GrenadeLauncher>();
            if (plugin.ShotgunManager == null)
                plugin.ShotgunManager = Server.Host.GameObject.AddComponent<Shotgun>();
        }
    }
}