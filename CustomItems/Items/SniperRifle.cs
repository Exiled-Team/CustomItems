// <copyright file="SniperRifle.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CustomItems.Items
{
    using System.Collections.Generic;
    using Exiled.CustomItems.API;
    using Exiled.Events.EventArgs;

    /// <inheritdoc />
    public class SniperRifle : CustomWeapon
    {
        /// <inheritdoc />
        public SniperRifle(ItemType type, int clipSize, int itemId)
            : base(type, clipSize, itemId)
        {
        }

        /// <inheritdoc/>
        public override string Name { get; set; } = Plugin.Singleton.Config.ItemConfigs.SniperCfg.Name;

        /// <inheritdoc/>
        public override Dictionary<SpawnLocation, float> SpawnLocations { get; set; } = Plugin.Singleton.Config.ItemConfigs.GlCfg.SpawnLocations;

        /// <inheritdoc/>
        public override string Description { get; set; } = Plugin.Singleton.Config.ItemConfigs.SniperCfg.Description;

        /// <inheritdoc/>
        public override int SpawnLimit { get; set; } = Plugin.Singleton.Config.ItemConfigs.SniperCfg.SpawnLimit;

        /// <inheritdoc/>
        protected override int ModBarrel { get; set; } = 3;

        /// <inheritdoc/>
        protected override int ModSight { get; set; } = 4;

        /// <inheritdoc/>
        protected override void LoadEvents()
        {
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            base.LoadEvents();
        }

        /// <inheritdoc/>
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