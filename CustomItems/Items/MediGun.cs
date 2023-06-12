// -----------------------------------------------------------------------
// <copyright file="MediGun.cs" company="Joker119">
// Copyright (c) Joker119. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems.Items;

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;

using PlayerRoles;
using PlayerStatsSystem;
using UnityEngine;
using Firearm = Exiled.API.Features.Items.Firearm;

/// <inheritdoc />
[CustomItem(ItemType.GunFSP9)]
public class MediGun : CustomWeapon
{
    private readonly Dictionary<Player, RoleTypeId> previousRoles = new();

    /// <inheritdoc/>
    public override uint Id { get; set; } = 5;

    /// <inheritdoc/>
    public override string Name { get; set; } = "MG-119";

    /// <inheritdoc/>
    public override string Description { get; set; } = "A specialized weapon that fires darts filled with a special mixture of Painkillers, Antibiotics, Antiseptics and other medicines. When fires at friendly targets, they will be healed. When fired at instances of SCP-049-2, they will be slowly converted back to human form. Does nothing when fired at anyone else.";

    /// <inheritdoc/>
    public override float Weight { get; set; } = 1.95f;

    /// <inheritdoc/>
    public override float Damage { get; set; }

    /// <inheritdoc/>
    public override byte ClipSize { get; set; } = 10;

    /// <summary>
    /// Gets or sets a value indicating whether or not to allow friendly fire with this weapon on FF-enabled servers.
    /// </summary>
    public override bool FriendlyFire { get; set; } = true;

    /// <inheritdoc/>
    public override SpawnProperties? SpawnProperties { get; set; } = new()
    {
        Limit = 1,
        DynamicSpawnPoints = new List<DynamicSpawnPoint>
        {
            new()
            {
                Chance = 40,
                Location = SpawnLocationType.InsideGr18,
            },
            new()
            {
                Chance = 50,
                Location = SpawnLocationType.InsideGateA,
            },
            new()
            {
                Chance = 50,
                Location = SpawnLocationType.InsideGateB,
            },
        },
    };

    /// <summary>
    /// Gets or sets a value indicating whether or not zombies can be 'cured' by this weapon.
    /// </summary>
    [Description("Whether or not zombies can be 'cured' by this weapon.")]
    public bool HealZombies { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether or not zombies who are healed will become allies to the healer.
    /// </summary>
    [Description("Whether or not zombies who are healed will become teammates for the healer, or remain as their old class.")]
    public bool HealZombiesTeamCheck { get; set; } = true;

    /// <summary>
    /// Gets or sets the % of damage the weapon would normally deal, that is converted into healing. 1 = 100%, 0.5 = 50%, 0.0 = 0%.
    /// </summary>
    [Description("The % of damage the weapon would normally deal, that is converted into healing. 1 = 100%, 0.5 = 50%, 0.0 = 0%")]
    public float HealingModifier { get; set; } = 1f;

    /// <summary>
    /// Gets or sets the amount of total 'healing' a zombie will require before being cured.
    /// </summary>
    [Description("The amount of total 'healing' a zombie will require before being cured.")]
    public int ZombieHealingRequired { get; set; } = 200;

    /// <inheritdoc/>
    protected override void SubscribeEvents()
    {
        if (HealZombies)
            Exiled.Events.Handlers.Player.Dying += OnDying;

        base.SubscribeEvents();
    }

    /// <inheritdoc/>
    protected override void UnsubscribeEvents()
    {
        if (HealZombies)
            Exiled.Events.Handlers.Player.Dying -= OnDying;

        base.UnsubscribeEvents();
    }

    /// <inheritdoc/>
    protected override void OnWaitingForPlayers()
    {
        previousRoles.Clear();

        base.OnWaitingForPlayers();
    }

    /// <inheritdoc/>
    protected override void OnHurting(HurtingEventArgs ev)
    {
        if (Check(ev.Attacker.CurrentItem) && ev.Attacker != ev.Player && ev.DamageHandler.Base is FirearmDamageHandler firearmHandler && firearmHandler.WeaponType == ev.Attacker.CurrentItem.Type)
        {
            if (Damage > 0)
                ev.Amount = Damage;

            if (ev.Player.Role.Side == ev.Attacker.Role.Side)
            {
                float amount = ev.Amount * HealingModifier;
                ev.Player.Heal(amount);

                ev.IsAllowed = false;
            }
            else if (ev.Player.Role == RoleTypeId.Scp0492 && HealZombies)
            {
                if (!ev.Player.ActiveArtificialHealthProcesses.Any())
                    ev.Player.AddAhp(0, ZombieHealingRequired, persistant: true);
                ev.Player.ArtificialHealth += ev.Amount;

                if (ev.Player.ArtificialHealth >= ev.Player.MaxArtificialHealth)
                    DoReviveZombie(ev.Player, ev.Player);

                ev.IsAllowed = false;
            }
        }
    }

    private void OnDying(DyingEventArgs ev)
    {
        if (!ev.Player.IsHuman || (ev.Attacker != null && ev.Attacker.Role != RoleTypeId.Scp049))
            return;

        if (!previousRoles.ContainsKey(ev.Player))
            previousRoles.Add(ev.Player, RoleTypeId.None);

        previousRoles[ev.Player] = ev.Player.Role;
    }

    private void DoReviveZombie(Player target, Player healer)
    {
        Log.Debug($"Reviving {target.Nickname}");
        if (HealZombiesTeamCheck)
        {
            target.Role.Set(healer.Role.Side == Side.Mtf ? RoleTypeId.NtfPrivate : RoleTypeId.ChaosConscript, RoleSpawnFlags.None);
            return;
        }

        if (previousRoles.ContainsKey(target))
            target.Role.Set(previousRoles[target]);
    }
}