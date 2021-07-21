// -----------------------------------------------------------------------
// <copyright file="AutoGun.cs" company="Galaxy119 and iopietro">
// Copyright (c) Galaxy119 and iopietro. All rights reserved.
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
        public override bool ShouldMessageOnGban => true;

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
        /// Gets or sets a value indicating whether ammo will be taken per hit(true) or per shot(false).
        /// </summary>
        [Description("Gets or sets a value indicating whether ammo will be taken per hit(true) or per shot(false).")]
        public bool PerHitAmmo { get; set; } = true;

        /// <inheritdoc/>
        protected override void OnShooting(ShootingEventArgs ev)
        {
            int ammoUsed = 0;
            foreach (Player player in Player.List)
            {
                if (ev.Shooter.CurrentItem.durability != ammoUsed || (PerHitAmmo && ev.Shooter.CurrentItem.durability != 0))
                {
                    if (player.Side != ev.Shooter.Side || (player.Side == ev.Shooter.Side && TeamKill))
                    {
                        if (player != ev.Shooter && Vector3.Distance(ev.Shooter.Position, player.Position) < MaxDistance)
                        {
                            if (!Physics.Linecast(ev.Shooter.Position, player.Position, player.ReferenceHub.playerMovementSync.CollidableSurfaces))
                            {
                                ammoUsed++;
                                player.Hurt(Damage, ev.Shooter, DamageTypes.Com15);
                                if (player.IsDead)
                                    player.ShowHint("<color=#FF0000>YOU HAVE BEEN KILLED BY AUTO AIM GUN</color>");
                                ev.Shooter.ReferenceHub.weaponManager.RpcConfirmShot(true, ev.Shooter.ReferenceHub.weaponManager.curWeapon);
                            }
                        }
                    }
                }
            }

            if (PerHitAmmo)
            {
                ammoUsed = 1;
            }

            ev.Shooter.SetWeaponAmmo(ev.Shooter.CurrentItem, (int)ev.Shooter.CurrentItem.durability - (int)ammoUsed);
            ev.IsAllowed = false;
        }
    }
}
