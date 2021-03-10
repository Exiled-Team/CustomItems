// -----------------------------------------------------------------------
// <copyright file="Scp865.cs" company="Babyboucher20">
// Copyright (c) Babyboucher20. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems.Items
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.CustomItems.API;
    using Exiled.CustomItems.API.Features;
    using Exiled.CustomItems.API.Spawn;
    using Exiled.Events.EventArgs;
    using Hints;
    using MEC;
    using UnityEngine;

    /// <inheritdoc />
    public class Scp865 : CustomWeapon
    {

        /// <inheritdoc/>
        public override uint Id { get; set; } = 19;

        /// <inheritdoc/>
        public override string Name { get; set; } = "SCP-865";

        /// <inheritdoc/>
        public override string Description { get; set; } = "SCP-865 is a gun that makes the person shot forget that they got shot";

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new DynamicSpawnPoint
                {
                    Chance = 100,
                    Location = SpawnLocation.InsideNukeArmory,
                },
            },
        };

        /// <inheritdoc/>
        public override Modifiers Modifiers { get; set; } = default;

        /// <inheritdoc/>
        public override float Damage { get; set; } = 25f;

        /// <inheritdoc/>
        public override uint ClipSize { get; set; } = 16;

        /// <inheritdoc/>
        protected override void OnShooting(ShootingEventArgs ev)
        {
            ev.Shooter.SetWeaponAmmo(ev.Shooter.CurrentItem, (int)ev.Shooter.CurrentItem.durability - 1);
            Player target = Player.Get(ev.Target);
            if (target != null)
                target.Hurt(Damage);
            ev.IsAllowed = false;
        }
    }
}
