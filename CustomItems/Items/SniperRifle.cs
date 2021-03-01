// -----------------------------------------------------------------------
// <copyright file="SniperRifle.cs" company="Galaxy199 and iopietro">
// Copyright (c) Galaxy199 and iopietro. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems.Items
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using Exiled.CustomItems.API;
    using Exiled.CustomItems.API.Features;
    using Exiled.CustomItems.API.Spawn;
    using Exiled.Events.EventArgs;

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
        public override uint ClipSize { get; set; } = 1;

        /// <inheritdoc/>
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
            if (ev.Attacker != ev.Target && ev.DamageType == DamageTypes.FromWeaponId(ev.Attacker.ReferenceHub.weaponManager.curWeapon))
                ev.Amount *= DamageMultiplier;
        }
    }
}