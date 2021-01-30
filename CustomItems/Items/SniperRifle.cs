using CustomItems.Components;
using Exiled.API.Enums;
using Exiled.Events.EventArgs;

namespace CustomItems.Items
{
    public class SniperRifle : CustomItem
    {
        protected override int ClipSize { get; set; } = 1;
        public override ItemType ItemType { get; set; } = ItemType.GunE11SR;
        public override string ItemName { get; set; } = "SR-119";
        public override string ItemDescription { get; set; } =
            "This modified E-11 Rifle fires high-velocity anti-personnel sniper rounds.";
        protected override BarrelType ModBarrel { get; set; } = BarrelType.HeavyBarrel;
        protected override SightType ModSight { get; set; } = SightType.SniperScope;

        protected override void LoadEvents()
        {
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
        }

        protected override void UnloadEvents()
        {
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
        }

        private void OnHurting(HurtingEventArgs ev)
        {
            if (CheckItem(ev.Attacker.CurrentItem))
                ev.Amount *= Plugin.Singleton.Config.SniperDmgMult;
        }
    }
}