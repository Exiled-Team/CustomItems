// -----------------------------------------------------------------------
// <copyright file="Scp2818.cs" company="Joker119">
// Copyright (c) Joker119. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems.Items;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;

using MEC;
using PlayerRoles;
using PlayerStatsSystem;
using UnityEngine;
using YamlDotNet.Serialization;

/// <summary>
/// A gun that kills you.
/// </summary>
[CustomItem(ItemType.GunE11SR)]
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
    public override float Weight { get; set; } = 3.95f;

    /// <inheritdoc/>
    [YamlIgnore]
    public override byte ClipSize { get; set; } = 1;

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
    public override SpawnProperties? SpawnProperties { get; set; } = new()
    {
        Limit = 1,
        DynamicSpawnPoints = new List<DynamicSpawnPoint>
        {
            new()
            {
                Chance = 60,
                Location = SpawnLocationType.InsideHid,
            },
            new()
            {
                Chance = 40,
                Location = SpawnLocationType.InsideHczArmory,
            },
        },
    };

    /// <inheritdoc/>
    [Description("The amount of damage the weapon deals when the projectile hits another player.")]
    public override float Damage { get; set; } = float.MaxValue;

    /// <inheritdoc/>
    protected override void OnShooting(ShootingEventArgs ev)
    {
        try
        {
            foreach (Item item in ev.Player.Items.ToList())
                if (Check(item))
                {
                    Log.Debug($"SCP-2818: Found a 2818 in inventory of shooter, removing.");
                    ev.Player.RemoveItem(item);
                }

            Player target = Player.Get(ev.TargetNetId);
            if (ev.ShotPosition == Vector3.zero || (ev.Player.Position - ev.ShotPosition).sqrMagnitude > 1000f)
            {
                ev.Player.Hurt(new UniversalDamageHandler(-1f, DeathTranslations.Warhead));
                ev.IsAllowed = false;
                return;
            }

            Timing.RunCoroutine(ShooterProjectile(ev.Player, ev.ShotPosition, target));
        }
        catch (Exception e)
        {
            Log.Error(e);
        }
    }

    private IEnumerator<float> ShooterProjectile(Player player, Vector3 targetPos, Player? target = null)
    {
        RoleTypeId playerRole = player.Role;

        // This is the camera transform used to make grenades appear like they are coming from the player's head instead of their stomach. We move them here so they aren't skidding across the floor.
        player.Position = player.CameraTransform.TransformPoint(new Vector3(0.0715f, 0.0225f, 0.45f));
        player.Scale = new Vector3(0.15f, 0.15f, 0.15f);
        if (target != null)
        {
            while (Vector3.Distance(player.Position, target.Position) > (MaxDistancePerTick + 0.15f))
            {
                if (player.Role != playerRole)
                    break;

                player.Position = Vector3.MoveTowards(player.Position, target.Position, MaxDistancePerTick);

                yield return Timing.WaitForSeconds(TickFrequency);
            }
        }
        else
        {
            while (Vector3.Distance(player.Position, targetPos) > 0.5f)
            {
                if (player.Role != playerRole)
                    break;

                player.Position = Vector3.MoveTowards(player.Position, targetPos, MaxDistancePerTick);

                yield return Timing.WaitForSeconds(TickFrequency);
            }
        }

        player.Scale = Vector3.one;

        // Make sure the scale is reset properly *before* killing them. That's important.
        yield return Timing.WaitForSeconds(0.01f);

        if (DespawnAfterUse)
        {
            Log.Debug($"inv count: {player.Items.Count}");
            foreach (Item item in player.Items)
            {
                if (Check(item))
                {
                    Log.Debug("found 2818 in inventory, doing funni");
                    player.RemoveItem(item);
                }
            }
        }

        if (player.Role != RoleTypeId.Spectator)
            player.Hurt(new UniversalDamageHandler(-1f, DeathTranslations.Warhead));
        if (target?.Role != RoleTypeId.Spectator)
            target?.Hurt(new UniversalDamageHandler(Damage, DeathTranslations.Warhead));
    }
}