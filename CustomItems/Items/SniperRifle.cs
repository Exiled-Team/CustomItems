// -----------------------------------------------------------------------
// <copyright file="SniperRifle.cs" company="Galaxy119 and iopietro">
// Copyright (c) Galaxy119 and iopietro. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems.Items
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Spawn;
    using Exiled.CustomItems.API;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs;
    using PlayerStatsSystem;
    using YamlDotNet.Serialization;

    /// <inheritdoc />
    public class SniperRifle : CustomWeapon
    {
        /// <inheritdoc/>
        public override uint Id { get; set; } = 10;

        /// <inheritdoc/>
        public override string Name { get; set; } = "SR-119";

        /// <inheritdoc/>
        public override string Description { get; set; } = "This modified E-11 Rifle fires high-velocity anti-personnel sniper rounds.";

        /// <inheritdoc/>
        public override float Weight { get; set; } = 3.25f;

        /// <inheritdoc/>
        public override byte ClipSize { get; set; } = 1;

        /// <inheritdoc/>
        public override bool ShouldMessageOnGban { get; } = true;

        /// <inheritdoc/>
        [YamlIgnore]
        public override float Damage { get; set; }

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new DynamicSpawnPoint
                {
                    Chance = 100,
                    Location = SpawnLocation.InsideHid,
                },
                new DynamicSpawnPoint
                {
                    Chance = 40,
                    Location = SpawnLocation.InsideHczArmory,
                },
            },
        };

        /// <inheritdoc />
        public override Modifiers Modifiers { get; set; } = new Modifiers(3, 4, 0);

        /// <summary>
        /// Gets or sets the amount of extra damage this weapon does, as a multiplier.
        /// </summary>
        [Description("The amount of extra damage this weapon does, as a multiplier.")]
        public float DamageMultiplier { get; set; } = 7.5f;

        /// <inheritdoc/>
        protected override void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker != ev.Target && ev.DamageHandler is FirearmDamageHandler firearmDamageHandler && firearmDamageHandler.WeaponType == ev.Attacker.CurrentItem.Type)
                ev.Amount *= DamageMultiplier;
        }
    }
}