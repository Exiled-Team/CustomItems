using CustomItems.Components;
using CustomItems.Items;

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
        }

        public void OnReloadingConfigs()
        {
            plugin.Config.ParseSubclassList();
        }
    }
}