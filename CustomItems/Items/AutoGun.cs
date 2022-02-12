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
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Spawn;
    using Exiled.CustomItems.API;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs;
    using InventorySystem.Items.Firearms.Attachments;
    using PlayerStatsSystem;
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
        public override float Weight { get; set; } = 2.35f;

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
        public override AttachmentNameTranslation[] Attachments { get; set; }

        /// <inheritdoc/>
        public override float Damage { get; set; } = 25;

        /// <inheritdoc/>
        public override byte ClipSize { get; set; } = 5;

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
            if (ev.Shooter.CurrentItem is Firearm firearm)
            {
                int ammoUsed = 0;
                foreach (Player player in Player.List)
                {
                    if (firearm.Ammo == ammoUsed &&
                        (!PerHitAmmo || firearm.Ammo == 0 || player.Role == RoleType.Spectator ||
                         (player.Role.Side == ev.Shooter.Role.Side && (player.Role.Side != ev.Shooter.Role.Side || !TeamKill)) ||
                         player == ev.Shooter ||
                         !(Vector3.Distance(ev.Shooter.Position, player.Position) < MaxDistance) ||
                         Physics.Linecast(ev.Shooter.Position, player.Position, player.ReferenceHub.playerMovementSync.CollidableSurfaces)))
                        continue;

                    ammoUsed++;
                    player.Hurt(new FirearmDamageHandler(firearm.Base, Damage, player.Role.Side != Side.Scp));
                    if (player.IsDead)
                        player.ShowHint("<color=#FF0000>YOU HAVE BEEN KILLED BY AUTO AIM GUN</color>");
                    ev.Shooter.ShowHitMarker(1f);
                }

                if (PerHitAmmo)
                {
                    ammoUsed = 1;
                }

                firearm.Ammo -= (byte)ammoUsed;
                ev.IsAllowed = false;
            }
        }
    }
}
