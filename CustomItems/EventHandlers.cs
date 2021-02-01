using CustomItems.Items;
using CustomItems.API;

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
                new Shotgun(ItemType.GunMP7, plugin.Config.WeaponConfigs.ShotgunCfg.SpreadCount * 2, 1)
                    .RegisterCustomItem();
                
                new GrenadeLauncher(ItemType.GunLogicer,1,  2)
                    .RegisterCustomItem();
                
                new SniperRifle(ItemType.GunE11SR, 1, 3)
                    .RegisterCustomItem();
                
                new Scp127(ItemType.GunCOM15, 12, 4)
                    .RegisterCustomItem();
                
                new ImplosionGrenade(ItemType.GrenadeFrag, 5)
                    .RegisterCustomItem();
                
                new EmpGrenade(ItemType.GrenadeFlash, 6)
                    .RegisterCustomItem();
                
                new LethalInjection(ItemType.Adrenaline, 7)
                    .RegisterCustomItem();
                
                new MediGun(ItemType.GunProject90, 30, 8)
                    .RegisterCustomItem();
                
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