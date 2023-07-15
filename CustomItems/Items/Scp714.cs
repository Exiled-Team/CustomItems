// -----------------------------------------------------------------------
// <copyright file="Scp714.cs" company="Joker119">
// Copyright (c) Joker119. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems.Items;

using System;
using System.Collections.Generic;
using System.ComponentModel;

using CustomPlayerEffects;

using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Pools;
using Exiled.API.Features.Roles;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp049;

using InventorySystem.Items.Usables.Scp330;

using MEC;

using PlayerRoles;
using PlayerRoles.FirstPersonControl;

using PlayerStatsSystem;

using UnityEngine;

/// <inheritdoc/>
[CustomItem(ItemType.Coin)]
public class Scp714 : CustomItem
{
    private HashSet<Player> equippedPlayers = new();
    private Dictionary<Player, CoroutineHandle> stamLimiters = new();
    private Dictionary<Player, List<(EffectType, float)>> existingEffects = new();

    /// <inheritdoc/>
    public override uint Id { get; set; } = 12;

    /// <inheritdoc/>
    public override string Name { get; set; } = "SCP-714";

    /// <inheritdoc/>
    public override string Description { get; set; } = "The jade ring that protects you from hazards.";

    /// <inheritdoc/>
    public override float Weight { get; set; } = 1.15f;

    /// <inheritdoc/>
    public override SpawnProperties? SpawnProperties { get; set; } = new()
    {
        Limit = 1,
        DynamicSpawnPoints = new List<DynamicSpawnPoint>
        {
            new()
            {
                Chance = 50,
                Location = SpawnLocationType.Inside049Armory,
            },
        },
    };

    /// <summary>
    /// Gets or sets which roles shouldn't be able to deal damage to the player that has SCP-714 put on.
    /// </summary>
    [Description("Which roles shouldn't be able to deal damage to the player that has SCP-714 put on.")]
    public List<RoleTypeId>? Scp714Roles { get; set; } = new()
    {
        RoleTypeId.Scp049,
        RoleTypeId.Scp0492,
    };

    /// <summary>
    /// Gets or sets which effects should be given to the player, when he will put on SCP-714.
    /// </summary>
    [Description("Which effects should be given to the player, when he will put on SCP-714.")]
    public List<EffectType> Scp714Effects { get; set; } = new()
    {
        EffectType.Asphyxiated,
    };

    public List<EffectType> PreventedEffects { get; set; } = new()
    {
        EffectType.AmnesiaItems,
        EffectType.AmnesiaVision,
        EffectType.Hypothermia,
        EffectType.Burned,
        EffectType.Concussed,
        EffectType.Blinded,
    };

    /// <summary>
    /// Gets or sets message shown to player, when he takes off the SCP-714.
    /// </summary>
    [Description("Message shown to player, when he takes off the SCP-714.")]
    public string TakeOffMessage { get; set; } = "You've taken off the ring.";

    public string PutOnMessage { get; set; } = "You have put on the ring.";

    public float Scp049Damage { get; set; } = 40f;

    public float PocketDimensionModifier { get; set; } = 0.75f;

    public float StamLimitModifier { get; set; } = 0.5f;

    public override bool Check(Player? player) => player is not null ? equippedPlayers.Contains(player) : base.Check(player);

    /// <inheritdoc/>
    protected override void SubscribeEvents()
    {
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
        Exiled.Events.Handlers.Scp049.Attacking += OnAttacking;
        Exiled.Events.Handlers.Player.FlippingCoin += OnFlippingCoin;
        Exiled.Events.Handlers.Player.ReceivingEffect += OnReceivingEffect;
        base.SubscribeEvents();
    }

    /// <inheritdoc/>
    protected override void UnsubscribeEvents()
    {
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;
        Exiled.Events.Handlers.Scp049.Attacking -= OnAttacking;
        Exiled.Events.Handlers.Player.FlippingCoin -= OnFlippingCoin;
        Exiled.Events.Handlers.Player.ReceivingEffect -= OnReceivingEffect;

        base.UnsubscribeEvents();
    }

    /// <inheritdoc/>
    protected override void OnDropping(DroppingItemEventArgs ev)
    {
        if (Check(ev.Player))
            SetRingState(ev.Player, false);

        base.OnDropping(ev);
    }

    private void OnFlippingCoin(FlippingCoinEventArgs ev)
    {
        if (!Check(ev.Player.CurrentItem))
            return;

        SetRingState(ev.Player, !Check(ev.Player));
    }

    private void OnAttacking(AttackingEventArgs ev)
    {
        if (Check(ev.Player) && (Scp714Roles?.Contains(ev.Player.Role.Type) ?? false))
        {
            if (ev.Target is not null)
            {
                ev.IsAllowed = false;
                ev.Target.Hurt(Scp049Damage);
            }
        }
    }

    private void OnHurting(HurtingEventArgs ev)
    {
        if (Check(ev.Player.CurrentItem))
        {
            if (ev.Attacker is not null && Scp714Roles is not null && Scp714Roles.Contains(ev.Attacker.Role))
                ev.IsAllowed = false;

            if (ev.DamageHandler.Type is DamageType.PocketDimension)
                ev.Amount *= PocketDimensionModifier;
        }
    }

    private void OnReceivingEffect(ReceivingEffectEventArgs ev)
    {
        if (Check(ev.Player) && PreventedEffects.Contains(ev.Effect.GetEffectType()))
            ev.IsAllowed = false;
    }

    private void SetRingState(Player player, bool equipped)
    {
        switch (equipped)
        {
            case true:
                List<(EffectType, float)> activeEffects = ListPool<(EffectType, float)>.Pool.Get();
                foreach (StatusEffectBase? active in player.ActiveEffects)
                    activeEffects.Add(new(active.GetEffectType(), active.TimeLeft));

                existingEffects[player] = activeEffects;
                ListPool<(EffectType, float)>.Pool.Return(activeEffects);
                foreach (EffectType effect in Scp714Effects)
                    player.EnableEffect(effect);
                stamLimiters[player] = Timing.RunCoroutine(LimitStamina(player));
                equippedPlayers.Add(player);
                player.ShowHint(PutOnMessage);

                break;
            case false:
                foreach (EffectType effect in Scp714Effects)
                    player.DisableEffect(effect);
                equippedPlayers.Remove(player);
                player.ShowHint(TakeOffMessage);
                Timing.KillCoroutines(stamLimiters[player]);
                stamLimiters.Remove(player);
                if (existingEffects.TryGetValue(player, out List<(EffectType, float)>? existingEffect))
                {
                    foreach ((EffectType type, float dur) in existingEffect)
                        player.EnableEffect(type, dur);
                    existingEffects.Remove(player);
                }

                break;
        }
    }

    private IEnumerator<float> LimitStamina(Player player)
    {
        while (Check(player))
        {
            if (player.Stamina > player.StaminaStat.MaxValue * StamLimitModifier)
                player.Stamina = player.StaminaStat.MaxValue * StamLimitModifier;

            yield return Timing.WaitForSeconds(0.15f);
        }
    }
}