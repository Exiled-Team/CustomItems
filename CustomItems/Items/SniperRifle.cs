// -----------------------------------------------------------------------
// <copyright file="SniperRifle.cs" company="Galaxy199 and iopietro">
// Copyright (c) Galaxy199 and iopietro. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems.Items
{
    using Exiled.CustomItems.API.Features;
    using Exiled.CustomItems.API.Spawn;
    using Exiled.Events.EventArgs;

    /// <inheritdoc />
    public class SniperRifle : CustomWeapon
    {
        /*/// <inheritdoc />
        public SniperRifle(ItemType type, uint clipSize, uint itemId)
            : base(type, itemId, clipSize)
        {
        }*/

        /// <inheritdoc/>
        public override string Name { get; } = CustomItems.Instance.Config.ItemConfigs.SniperCfg.Name;

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; protected set; } = CustomItems.Instance.Config.ItemConfigs.SniperCfg.SpawnProperties;

        /// <inheritdoc/>
        public override string Description { get; } = CustomItems.Instance.Config.ItemConfigs.SniperCfg.Description;

        /// <inheritdoc />
        public override Modifiers Modifiers { get; } = new Modifiers(3, 4, 0);

        /// <inheritdoc/>
        protected override void OnHurting(HurtingEventArgs ev)
        {
            if (Check(ev.Attacker.CurrentItem))
                ev.Amount *= CustomItems.Instance.Config.ItemConfigs.SniperCfg.DamageMultiplier;
        }
    }
}