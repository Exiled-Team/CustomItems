// -----------------------------------------------------------------------
// <copyright file="AutoGun.cs" company="Joker119">
// Copyright (c) Joker119. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems.Items;

using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;

using PlayerRoles;
using PlayerStatsSystem;
using UnityEngine;

using Player = Exiled.API.Features.Player;

/// <inheritdoc />
[CustomItem(ItemType.GunCOM15)]
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
    public override SpawnProperties? SpawnProperties { get; set; } = new()
    {
        Limit = 1,
        DynamicSpawnPoints = new List<DynamicSpawnPoint>
        {
            new()
            {
                Chance = 100,
                Location = SpawnLocationType.Inside173Armory,
            },
        },
    };

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
        if (ev.Player.CurrentItem is Firearm firearm)
        {
            int ammoUsed = 0;
            foreach (Player player in Player.List)
            {
                if (player.Role == RoleTypeId.Scp079)
                    continue;

                Vector3 forward = ev.Player.CameraTransform.forward;
                if (firearm.Ammo == ammoUsed &&
                    (!PerHitAmmo || firearm.Ammo == 0 || player.Role == RoleTypeId.Spectator ||
                     (player.Role.Side == ev.Player.Role.Side && (player.Role.Side != ev.Player.Role.Side || !TeamKill)) ||
                     player == ev.Player ||
                     !(Vector3.Distance(ev.Player.Position, player.Position) < MaxDistance) ||
                     Physics.Raycast(ev.Player.CameraTransform.position + forward, forward, out var hit, MaxDistance)))
                    continue;

                ammoUsed++;
                player.Hurt(new FirearmDamageHandler(firearm.Base, Damage, player.Role.Side != Side.Scp));
                if (player.IsDead)
                    player.ShowHint("<color=#FF0000>YOU HAVE BEEN KILLED BY AUTO AIM GUN</color>");
                ev.Player.ShowHitMarker(1f);
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