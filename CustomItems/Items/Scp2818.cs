// -----------------------------------------------------------------------
// <copyright file="Scp2818.cs" company="Galaxy119 and iopietro">
// Copyright (c) Galaxy119 and iopietro. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems.Items
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Exiled.API.Features;
    using Exiled.CustomItems.API;
    using Exiled.CustomItems.API.Features;
    using Exiled.CustomItems.API.Spawn;
    using Exiled.Events.EventArgs;
    using MEC;
    using UnityEngine;
    using YamlDotNet.Serialization;

    /// <summary>
    /// A gun that kills you.
    /// </summary>
    public class Scp2818 : CustomWeapon
    {
        /// <inheritdoc/>
        public override uint Id { get; set; } = 14;

        /// <inheritdoc/>
        public override string Name { get; set; } = "SCP-2818";

        /// <inheritdoc/>
        public override string Description { get; set; } =
            "When this weapon is fired, it uses the biomass of the shooter as the bullet.";

        /// <inheritdoc/>
        [YamlIgnore]
        public override uint ClipSize { get; set; } = 1;

        /// <summary>
        /// Gets or sets how often the <see cref="ShooterProjectile"/> coroutine will move the player.
        /// </summary>
        [Description("How frequently the shooter will be moved towards his target.\n# Note, a lower tick frequency, and lower MaxDistance will make the travel smoother, but be more stressful on your server.")]
        public float TickFrequency { get; set; } = 0.00025f;

        /// <summary>
        /// Gets or sets the max distance towards the target location the shooter can be moved each tick.
        /// </summary>
        [Description("The max distance towards the target location the shooter can be moved each tick.")]
        public float MaxDistancePerTick { get; set; } = 0.50f;

        /// <summary>
        /// Gets or sets a value indicating whether the gun should despawn instead of drop when it is fired.
        /// </summary>
        [Description("Whether or not the weapon should despawn itself after it's been used.")]
        public bool DespawnAfterUse { get; set; } = false;

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new DynamicSpawnPoint
                {
                    Chance = 60,
                    Location = SpawnLocation.InsideHid,
                },
                new DynamicSpawnPoint
                {
                    Chance = 40,
                    Location = SpawnLocation.InsideHczArmory,
                },
            },
        };

        /// <inheritdoc/>
        [YamlIgnore]
        public override Modifiers Modifiers { get; set; } = default;

        /// <inheritdoc/>
        [Description("The amount of damage the weapon deals when the projectile hits another player.")]
        public override float Damage { get; set; } = float.MaxValue;

        /// <inheritdoc/>
        protected override void OnShooting(ShootingEventArgs ev)
        {
            try
            {
                Player target = null;
                if (ev.Target != null)
                    target = Player.Get(ev.Target);
                if (ev.Position == Vector3.zero || Vector3.Distance(ev.Shooter.Position, ev.Position) > 100f)
                {
                    ev.Shooter.Kill(DamageTypes.Nuke);
                    ev.IsAllowed = false;
                    return;
                }

                Timing.RunCoroutine(ShooterProjectile(ev.Shooter, ev.Position, target));
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        private IEnumerator<float> ShooterProjectile(Player player, Vector3 targetPos, Player target = null)
        {
            RoleType playerRole = player.Role;

            // This is the camera transform used to make grenades appear like they are coming from the player's head instead of their stomach. We move them here so they aren't skidding across the floor.
            player.Position = player.CameraTransform.TransformPoint(new Vector3(0.0715f, 0.0225f, 0.45f));
            player.Scale = new Vector3(0.15f, 0.15f, 0.15f);
            if (target != null)
                while (Vector3.Distance(player.Position, target.Position) > (MaxDistancePerTick + 0.15f))
                {
                    if (player.Role != playerRole)
                        break;

                    player.Position = Vector3.MoveTowards(player.Position, target.Position, MaxDistancePerTick);

                    yield return Timing.WaitForSeconds(TickFrequency);
                }
            else
                while (Vector3.Distance(player.Position, targetPos) > 0.5f)
                {
                    if (player.Role != playerRole)
                        break;

                    player.Position = Vector3.MoveTowards(player.Position, targetPos, MaxDistancePerTick);

                    yield return Timing.WaitForSeconds(TickFrequency);
                }

            player.Scale = Vector3.one;

            // Make sure the scale is reset properly *before* killing them. That's important.
            yield return Timing.WaitForSeconds(0.01f);

            if (DespawnAfterUse)
                foreach (Inventory.SyncItemInfo item in player.Items.ToList())
                    if (Check(item))
                        player.RemoveItem(item);

            if (player.Role != RoleType.Spectator)
                player.Kill(DamageTypes.Nuke);
            if (target?.Role != RoleType.Spectator)
                target?.Hurt(Damage, DamageTypes.Nuke, player.Nickname, player.Id);
        }
    }
}