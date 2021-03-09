// -----------------------------------------------------------------------
// <copyright file="AutoGun.cs" company="Babyboucher20">
// Copyright (c) Babyboucher20. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------
namespace CustomItems.Items
{
    using System.Collections.Generic;
    using System.ComponentModel;
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
    public class AutoGun : CustomWeapon
    {

        /// <inheritdoc/>
        public override uint Id { get; set; } = 17;

        /// <inheritdoc/>
        public override string Name { get; set; } = "AutoGun";

        /// <inheritdoc/>
        public override string Description { get; set; } = "In one triger pull, shoot every enemy around you";

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new DynamicSpawnPoint
                {
                    Chance = 100,
                    Location = SpawnLocation.Inside173Armory,
                },
            },
        };

        /// <inheritdoc/>
        public override Modifiers Modifiers { get; set; } = default;

        /// <inheritdoc/>
        public override float Damage { get; set; } = 25;

        /// <inheritdoc/>
        public override uint ClipSize { get; set; } = 5;

        /// <summary>
        /// Gets or sets a value indicating whether if the gun can kill people on the same team.
        /// </summary>
        [Description("If the gun can kill people on the same team")]
        public bool TeamKill { get; set; } = true;

        /// <summary>
        /// Gets or sets the max distance at which the auto gun can kill.
        /// </summary>
        [Description("The max distance at which the auto gun can kill")]
        public float MaxDistance { get; set; } = 100f;

        /// <summary>
        /// Gets or sets a value indicating If true 1 ammo is taken per a person hit, false it only takes oe ammo per a shot.
        /// </summary>
        [Description("If true 1 ammo is taken per a person hit, false it only takes one ammo per a shot")]
        public bool PerHitAmmo { get; set; } = true;

        /// <inheritdoc/>
        protected override void OnShooting(ShootingEventArgs ev)
        {
            int AmmoUsed = 0;
            foreach (Player player in Player.List)
            {
                if (ev.Shooter.CurrentItem.durability != AmmoUsed || PerHitAmmo && ev.Shooter.CurrentItem.durability != 0)
                {
                    if (player.Side != ev.Shooter.Side || (player.Side == ev.Shooter.Side && TeamKill))
                    {
                        if (player != ev.Shooter && Vector3.Distance(ev.Shooter.Position, player.Position) < MaxDistance)
                        {
                            if (!Physics.Linecast(ev.Shooter.Position, player.Position, player.ReferenceHub.playerMovementSync.CollidableSurfaces))
                            {
                                AmmoUsed++;
                                player.Hurt(Damage, ev.Shooter, DamageTypes.Com15);
                                ev.Shooter.ReferenceHub.weaponManager.RpcConfirmShot(true, ev.Shooter.ReferenceHub.weaponManager.curWeapon);
                            }
                        }
                    }
                }
            }

            if (PerHitAmmo)
            {
                AmmoUsed = 1;
            }

            ev.Shooter.SetWeaponAmmo(ev.Shooter.CurrentItem, ev.Shooter.CurrentItem.durability - AmmoUsed);
            ev.IsAllowed = false;
        }
    }
}
