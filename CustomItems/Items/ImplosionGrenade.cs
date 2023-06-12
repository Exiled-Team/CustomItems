// -----------------------------------------------------------------------
// <copyright file="ImplosionGrenade.cs" company="Joker119">
// Copyright (c) Joker119. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems.Items;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Pools;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Scp106;
using InventorySystem.Items.ThrowableProjectiles;
using MEC;
using PlayerRoles;
using UnityEngine;
using Scp106 = Exiled.Events.Handlers.Scp106;

/// <inheritdoc />
[CustomItem(ItemType.GrenadeHE)]
public class ImplosionGrenade : CustomGrenade
{
    private int layerMask;

    private List<Player> effectedPlayers = new();

    /// <inheritdoc/>
    public override uint Id { get; set; } = 2;

    /// <inheritdoc/>
    public override string Name { get; set; } = "IG-119";

    /// <inheritdoc/>
    public override string Description { get; set; } = "This grenade does almost 0 damage, however it will succ nearby players towards the center of the implosion area.";

    /// <inheritdoc/>
    public override float Weight { get; set; } = 0.65f;

    /// <inheritdoc/>
    public override SpawnProperties? SpawnProperties { get; set; } = new()
    {
        Limit = 1,
        DynamicSpawnPoints = new List<DynamicSpawnPoint>
        {
            new()
            {
                Chance = 50,
                Location = SpawnLocationType.InsideLocker,
            },
            new()
            {
                Chance = 100,
                Location = SpawnLocationType.InsideHczArmory,
            },
        },
    };

    /// <inheritdoc/>
    public override bool ExplodeOnCollision { get; set; } = true;

    /// <inheritdoc/>
    public override float FuseTime { get; set; } = 1.5f;

    /// <summary>
    /// Gets or sets the % of normal frag grenade damage this grenade will deal to those in it's radius.
    /// </summary>
    [Description("The % of normal frag grenade damage this grenade will deal to those in it's radius.")]
    public float DamageModifier { get; set; } = 0.05f;

    /// <summary>
    /// Gets or sets the amount of suction ticks each grenade will generate.
    /// </summary>
    [Description("The amount of suction ticks each grenade will generate.")]
    public int SuctionCount { get; set; } = 90;

    /// <summary>
    /// Gets or sets the distance each tick will move players towards the center.
    /// </summary>
    [Description("The distance each tick will move players towards the center.")]
    public float SuctionPerTick { get; set; } = 0.125f;

    /// <summary>
    /// Gets or sets how often each suction tick will occurs. Note: Setting the tick-rate and suction-per-tick to lower numbers maks for a 'smoother' suction movement, however causes more stress on your server. Adjust accordingly.
    /// </summary>
    [Description("How often each suction tick will occus. Note: Setting the tick-rate and suction-per-tick to lower numbers maks for a 'smoother' suction movement, however causes more stress on your server. Adjust accordingly.")]
    public float SuctionTickRate { get; set; } = 0.025f;

    /// <summary>
    /// Gets or sets a list of roles unable to be affected by Implosion grenades.
    /// </summary>
    [Description("What roles will not be able to be affected by Implosion Grenades. Keeping SCP-173 on this list is highly recommended.")]
    public HashSet<RoleTypeId> BlacklistedRoles { get; set; } = new() { RoleTypeId.Scp173, RoleTypeId.Tutorial, };

    private List<CoroutineHandle> Coroutines { get; set; } = new();

    /// <inheritdoc/>
    protected override void SubscribeEvents()
    {
        Scp106.Teleporting += OnTeleporting;

        base.SubscribeEvents();
    }

    /// <inheritdoc/>
    protected override void UnsubscribeEvents()
    {
        Scp106.Teleporting -= OnTeleporting;

        foreach (CoroutineHandle handle in Coroutines)
            Timing.KillCoroutines(handle);

        base.UnsubscribeEvents();
    }

    /// <inheritdoc/>
    protected override void OnExploding(ExplodingGrenadeEventArgs ev)
    {
        ev.IsAllowed = false;
        Log.Debug($"{ev.Player.Nickname} threw an implosion grenade!");
        List<Player> copiedList = new();
        foreach (Player player in ev.TargetsToAffect)
        {
            copiedList.Add(player);
        }

        ev.TargetsToAffect.Clear();
        Log.Debug("IG: List cleared.");
        effectedPlayers = ListPool<Player>.Pool.Get();
        foreach (Player player in copiedList)
        {
            if (BlacklistedRoles.Contains(player.Role))
                continue;

            Log.Debug($"{player.Nickname} starting suction");

            try
            {
                if (layerMask == 0)
                {
                    if (ev.Projectile.Base is ExplosionGrenade explosionGrenade)
                        layerMask = explosionGrenade._detectionMask;
                }

                bool line = Physics.Linecast(ev.Projectile.Transform.position, player.Position, layerMask);
                Log.Debug($"{player.Nickname} - {line}");
                if (line)
                {
                    effectedPlayers.Add(player);
                    Coroutines.Add(Timing.RunCoroutine(DoSuction(player, ev.Projectile.Transform.position + (Vector3.up * 1.5f))));
                }
            }
            catch (Exception exception)
            {
                Log.Error($"{nameof(OnExploding)} error: {exception}");
            }
        }
    }

    private IEnumerator<float> DoSuction(Player player, Vector3 position)
    {
        Log.Debug($"{player.Nickname} Suction begin");
        for (int i = 0; i < SuctionCount; i++)
        {
            Log.Debug($"{player.Nickname} suctioned?");
            Vector3 alteredPosition = position + (1f * (player.Position - position).normalized);
            Vector3 newPos = Vector3.MoveTowards(player.Position, alteredPosition, SuctionPerTick);
            player.Position = newPos;

            yield return Timing.WaitForSeconds(SuctionTickRate);
        }

        ListPool<Player>.Pool.Return(effectedPlayers);
    }

    private void OnTeleporting(TeleportingEventArgs ev)
    {
        if (ev.Player == null || effectedPlayers == null)
            return;

        if (effectedPlayers.Contains(ev.Player))
            ev.IsAllowed = false;
    }
}