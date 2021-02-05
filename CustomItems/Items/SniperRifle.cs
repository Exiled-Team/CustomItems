using System.Collections.Generic;
using CustomItems.API;
using Exiled.Events.EventArgs;

namespace CustomItems.Items
{
    public class SniperRifle : CustomWeapon
    {
        public SniperRifle(ItemType type, int clipSize, int itemId) : base(type, clipSize, itemId)
        {
        }
        
        public override string Name { get; set; } = "SR-119";
        public override Dictionary<SpawnLocation, float> SpawnLocations { get; set; } =
            Plugin.Singleton.Config.ItemConfigs.GlCfg.SpawnLocations;
        protected override string Description { get; set; } =
            "This modified E-11 Rifle fires high-velocity anti-personnel sniper rounds.";
        protected override int ModBarrel { get; set; } = 3;
        protected override int ModSight { get; set; } = 4;

        protected override void LoadEvents()
        {
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            base.LoadEvents();
        }

        protected override void UnloadEvents()
        {
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
            base.UnloadEvents();
        }

        private void OnHurting(HurtingEventArgs ev)
        {
            if (CheckItem(ev.Attacker.CurrentItem))
                ev.Amount *= Plugin.Singleton.Config.ItemConfigs.SniperCfg.DamageMultiplier;
        }
    }
}