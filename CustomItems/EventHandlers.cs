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
            if (plugin.Config.SniperEnabled)
            {
                if (plugin.SniperRifleComponent == null)
                    plugin.SniperRifleComponent = Server.Host.GameObject.AddComponent<SniperRifle>();
                Log.Debug($"{plugin.SniperRifleComponent == null}", plugin.Config.Debug);
            }

            if (plugin.Config.GrenadeLauncherEnabled)
            {
                if (plugin.GrenadeLauncherComponent == null)
                    plugin.GrenadeLauncherComponent = Server.Host.GameObject.AddComponent<GrenadeLauncher>();
                Log.Debug($"{plugin.GrenadeLauncherComponent == null}", plugin.Config.Debug);
            }
        }
    }
}