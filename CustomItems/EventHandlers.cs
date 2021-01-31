using CustomItems.Items;

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
                API.RegisterCustomItem(new Shotgun(ItemType.GunMP7, plugin.Config.ShotgunSpreadCount * 2, 1));
                API.RegisterCustomItem(new GrenadeLauncher(ItemType.GunLogicer,1,  2));
                API.RegisterCustomItem(new SniperRifle(ItemType.GunE11SR, 1, 3));
                API.RegisterCustomItem(new Scp127(ItemType.GunCOM15, 12, 4));
                API.RegisterCustomItem(new ImplosionGrenade(ItemType.GrenadeFrag, 5));
                API.RegisterCustomItem(new EmpGrenade(ItemType.GrenadeFlash, 6));
                API.RegisterCustomItem(new LethalInjection(ItemType.Adrenaline, 7));
                API.RegisterCustomItem(new MediGun(ItemType.GunProject90, 30, 8));
                plugin.Config.ParseSubclassList();
                
                first = false;
            }
        }

        public void OnReloadingConfigs()
        {
            plugin.Config.ParseSubclassList();
        }
    }
}