// -----------------------------------------------------------------------
// <copyright file="DeflectorShield.cs" company="Joker119">
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
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.EventArgs;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;

using MEC;
using PlayerStatsSystem;
using YamlDotNet.Serialization;

using DamageHandlerBase = Exiled.API.Features.DamageHandlers.DamageHandlerBase;

/// <inheritdoc />
[CustomItem(ItemType.SCP268)]
public class DeflectorShield : CustomItem
{
    private readonly List<Player> deflectorPlayers = new();

    private readonly ItemType type = ItemType.SCP268;

    /// <inheritdoc/>
    public override uint Id { get; set; } = 18;

    /// <inheritdoc/>
    [YamlIgnore]
    public override ItemType Type { get => type; set => throw new ArgumentException("You cannot change the ItemType of this item."); }

    /// <inheritdoc/>
    public override string Name { get; set; } = "Deflector shield";

    /// <inheritdoc/>
    public override string Description { get; set; } = "A deflector shield that reflects bullets back at the shooter";

    /// <inheritdoc/>
    public override float Weight { get; set; } = 1.65f;

    /// <inheritdoc/>
    public override SpawnProperties? SpawnProperties { get; set; } = new()
    {
        Limit = 1,
        DynamicSpawnPoints = new List<DynamicSpawnPoint>
        {
            new()
            {
                Chance = 10,
                Location = SpawnLocationType.InsideHid,
            },
        },
    };

    /// <summary>
    /// Gets or sets how long the deflector shield can be wore, before automaticly player takes it off. (set to 0 for no limit).
    /// </summary>
    [Description("How long the deflector shield can be wore, before automaticly player takes it off. (set to 0 for no limit)")]
    public float Duration { get; set; } = 15f;

    /// <summary>
    /// Gets or sets By what will the Damage be multiplied.
    /// </summary>
    [Description("By what will the Damage be multiplied")]
    public float Multiplier { get; set; } = 1f;

    /// <inheritdoc/>
    protected override void SubscribeEvents()
    {
        Exiled.Events.Handlers.Player.UsedItem += OnItemUsed;
        Exiled.Events.Handlers.Player.Destroying += OnDestroying;
        Exiled.Events.Handlers.Player.Hurting += OnHurt;

        base.SubscribeEvents();
    }

    /// <inheritdoc/>
    protected override void UnsubscribeEvents()
    {
        Exiled.Events.Handlers.Player.UsedItem -= OnItemUsed;
        Exiled.Events.Handlers.Player.Destroying -= OnDestroying;
        Exiled.Events.Handlers.Player.Hurting -= OnHurt;

        base.UnsubscribeEvents();
    }

    /// <inheritdoc/>
    protected override void OnDropping(DroppingItemEventArgs ev)
    {
        if (deflectorPlayers.Contains(ev.Player))
        {
            ev.IsAllowed = false;

            deflectorPlayers.Remove(ev.Player);
        }
    }

    /// <inheritdoc/>
    protected override void OnWaitingForPlayers()
    {
        deflectorPlayers.Clear();

        base.OnWaitingForPlayers();
    }

    /// <inheritdoc/>
    protected override void OnOwnerDying(OwnerDyingEventArgs ev)
    {
        if (deflectorPlayers.Contains(ev.Player))
            deflectorPlayers.Remove(ev.Player);
    }

    private void OnDestroying(DestroyingEventArgs ev)
    {
        if (deflectorPlayers.Contains(ev.Player))
            deflectorPlayers.Remove(ev.Player);
    }

    private void OnItemUsed(UsedItemEventArgs ev)
    {
        if (!Check(ev.Player.CurrentItem))
            return;

        if (!deflectorPlayers.Contains(ev.Player))
            deflectorPlayers.Add(ev.Player);

        ev.Player.DisableEffect(EffectType.Invisible);

        if (Duration > 0)
        {
            Timing.CallDelayed(Duration, () =>
            {
                deflectorPlayers.Remove(ev.Player);
            });
        }
    }

    private void OnHurt(HurtingEventArgs ev)
    {
        if (deflectorPlayers.Contains(ev.Player) && ev.DamageHandler.Base is FirearmDamageHandler && ev.Player != ev.Attacker)
        {
            ev.IsAllowed = false;
            ev.Attacker.Hurt(ev.Player, ev.Amount * Multiplier, ev.DamageHandler.Type, DamageHandlerBase.CassieAnnouncement.Default);
        }
    }
}