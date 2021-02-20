// <copyright file="SniperRifle.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CustomItems.Items
{
    using Exiled.CustomItems.API;
    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    /// <inheritdoc />
    public class SniperRifle : CustomWeapon
    {
        /// <inheritdoc />
        public SniperRifle(ItemType type, int clipSize, int itemId)
            : base(type, clipSize, itemId)
        {
        }

        /// <inheritdoc/>
        public override string Name { get; } = Plugin.Singleton.Config.ItemConfigs.SniperCfg.Name;

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; set; } = Plugin.Singleton.Config.ItemConfigs.SniperCfg.SpawnProperties;

        /// <inheritdoc/>
        public override string Description { get; } = Plugin.Singleton.Config.ItemConfigs.SniperCfg.Description;

        /// <inheritdoc />
        public override Modifiers Modifiers { get; } = new Modifiers(3, 4, 0);

        /// <inheritdoc/>
        protected override void LoadEvents()
        {
            Player.Hurting += OnHurting;
            base.LoadEvents();
        }

        /// <inheritdoc/>
        protected override void UnloadEvents()
        {
            Player.Hurting -= OnHurting;
            base.UnloadEvents();
        }

        private void OnHurting(HurtingEventArgs ev)
        {
            if (Check(ev.Attacker.CurrentItem))
                ev.Amount *= Plugin.Singleton.Config.ItemConfigs.SniperCfg.DamageMultiplier;
        }
    }
}