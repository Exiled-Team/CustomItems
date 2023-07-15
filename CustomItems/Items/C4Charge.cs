// -----------------------------------------------------------------------
// <copyright file="C4Charge.cs" company="Joker119">
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
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;

using InventorySystem.Items.Pickups;
using InventorySystem.Items.ThrowableProjectiles;
using Mirror;
using UnityEngine;
using YamlDotNet.Serialization;

using Object = UnityEngine.Object;
using PlayerEvent = Exiled.Events.Handlers.Player;

/// <inheritdoc/>
[CustomItem(ItemType.GrenadeHE)]
public class C4Charge : CustomGrenade
{
    /// <summary>
    /// Enum containing methods indicating how C4 charge can be removed.
    /// </summary>
    public enum C4RemoveMethod
    {
        /// <summary>
        /// C4 charge will be removed without exploding.
        /// </summary>
        Remove = 0,

        /// <summary>
        /// C4 charge will be detonated.
        /// </summary>
        Detonate = 1,

        /// <summary>
        /// C4 charge will drop as a pickable item.
        /// </summary>
        Drop = 2,
    }

    /// <summary>
    /// Gets the instance of this item manager.
    /// </summary>
    public static C4Charge Instance { get; private set; } = null!;

    /// <summary>
    /// Gets all of the currently placed charges.
    /// </summary>
    public static Dictionary<Pickup, Player> PlacedCharges { get; } = new();

    /// <inheritdoc/>
    public override uint Id { get; set; } = 15;

    /// <inheritdoc/>
    public override string Name { get; set; } = "C4-119";

    /// <inheritdoc/>
    public override float Weight { get; set; } = 0.75f;

    /// <inheritdoc/>
    public override SpawnProperties? SpawnProperties { get; set; } = new()
    {
        Limit = 5,
        DynamicSpawnPoints = new List<DynamicSpawnPoint>
        {
            new()
            {
                Chance = 10,
                Location = SpawnLocationType.InsideLczArmory,
            },

            new()
            {
                Chance = 25,
                Location = SpawnLocationType.InsideHczArmory,
            },

            new()
            {
                Chance = 50,
                Location = SpawnLocationType.InsideNukeArmory,
            },

            new()
            {
                Chance = 50,
                Location = SpawnLocationType.Inside049Armory,
            },

            new()
            {
                Chance = 100,
                Location = SpawnLocationType.InsideSurfaceNuke,
            },
        },
    };

    /// <inheritdoc/>
    public override string Description { get; set; } = "Explosive charge that can be remotly detonated.";

    /// <summary>
    /// Gets or sets a value indicating whether C4 charge should stick to walls / ceiling.
    /// </summary>
    [Description("Should C4 charge stick to walls / ceiling.")]
    public bool IsSticky { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating throwing force muliplier.
    /// </summary>
    [Description("Defines how strongly C4 will be thrown")]
    public float ThrowMultiplier { get; set; } = 40f;

    /// <summary>
    /// Gets or sets a value indicating whether C4 charge require a specific item to be detonated.
    /// </summary>
    [Description("Should C4 require a specific item to be detonated.")]
    public bool RequireDetonator { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the Detonator Item that will be used to detonate C4 Charges.
    /// </summary>
    [Description("The Detonator Item that will be used to detonate C4 Charges")]
    public ItemType DetonatorItem { get; set; } = ItemType.Radio;

    /// <summary>
    /// Gets or sets a value indicating whether C4 charges will be detonated, destroyed or dropped as a pickup, when player who placed them dies/leaves the game.
    /// </summary>
    [Description("What happens with C4 charges placed by player, when he dies/leaves the game. (Remove / Detonate / Drop)")]
    public C4RemoveMethod MethodOnDeath { get; set; } = C4RemoveMethod.Drop;

    /// <summary>
    /// Gets or sets a value indicating whether C4 can be defused.
    /// </summary>
    [Description("Should shooting at C4 charges do something.")]
    public bool AllowShoot { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether C4 charges will be detonated, destroyed or dropped as a pickup, when they have been shot.
    /// </summary>
    [Description("What happens with C4 charges after they are shot. (Remove / Detonate / Drop)")]
    public C4RemoveMethod ShotMethod { get; set; } = C4RemoveMethod.Remove;

    /// <summary>
    /// Gets or sets a value indicating whether maximum distance between C4 Charge and player to detonate.
    /// </summary>
    [Description("Maximum distance between C4 Charge and player to detonate.")]
    public float MaxDistance { get; set; } = 100f;

    /// <summary>
    /// Gets or sets time after which the C4 charge will automatically detonate.
    /// </summary>
    [Description("Time after which the C4 charge will automatically detonate.")]
    public override float FuseTime { get; set; } = 9999f;

    /// <inheritdoc/>
    [YamlIgnore]
    public override bool ExplodeOnCollision { get; set; } = false;

    /// <inheritdoc/>
    [YamlIgnore]
    public override ItemType Type { get; set; } = ItemType.GrenadeHE;

    /// <summary>
    /// Handles the removal of C4 charges.
    /// </summary>
    /// <param name="charge"> The C4 charge to be handled.</param>
    /// <param name="removeMethod"> The method of removing the charge.</param>
    public void C4Handler(Pickup? charge, C4RemoveMethod removeMethod = C4RemoveMethod.Detonate)
    {
        if (charge?.Position is null)
            return;
        switch (removeMethod)
        {
            case C4RemoveMethod.Remove:
                {
                    break;
                }

            case C4RemoveMethod.Detonate:
                {
                    ExplosiveGrenade grenade = (ExplosiveGrenade)Item.Create(Type);
                    grenade.FuseTime = 0.1f;
                    grenade.SpawnActive(charge.Position);
                    break;
                }

            case C4RemoveMethod.Drop:
                {
                    TrySpawn(Id, charge.Position, out _);
                    break;
                }
        }

        PlacedCharges.Remove(charge);
        charge.Destroy();
    }

    /// <inheritdoc/>
    protected override void SubscribeEvents()
    {
        Instance = this;

        PlayerEvent.Destroying += OnDestroying;
        PlayerEvent.Died += OnDied;
        PlayerEvent.Shooting += OnShooting;
        Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;

        base.SubscribeEvents();
    }

    /// <inheritdoc/>
    protected override void UnsubscribeEvents()
    {
        PlayerEvent.Destroying -= OnDestroying;
        PlayerEvent.Died -= OnDied;
        PlayerEvent.Shooting -= OnShooting;

        base.UnsubscribeEvents();
    }

    /// <inheritdoc/>
    protected override void OnWaitingForPlayers()
    {
        PlacedCharges.Clear();

        base.OnWaitingForPlayers();
    }

    protected override void OnThrownProjectile(ThrownProjectileEventArgs ev)
    {
        if (!PlacedCharges.ContainsKey(ev.Projectile))
            PlacedCharges.Add(ev.Projectile, ev.Player);
        base.OnThrownProjectile(ev);
    }

    /// <inheritdoc/>
    protected override void OnExploding(ExplodingGrenadeEventArgs ev)
    {
        PlacedCharges.Remove(Pickup.Get(ev.Projectile.Base));
    }

    private void OnDestroying(DestroyingEventArgs ev)
    {
        foreach (var charge in PlacedCharges.ToList())
        {
            if (charge.Value == ev.Player)
            {
                C4Handler(charge.Key, C4RemoveMethod.Remove);
            }
        }
    }

    private void OnDied(DiedEventArgs ev)
    {
        foreach (var charge in PlacedCharges.ToList())
        {
            if (charge.Value == ev.Player)
            {
                C4Handler(charge.Key, MethodOnDeath);
            }
        }
    }

    private void OnShooting(ShootingEventArgs ev)
    {
        if (!AllowShoot)
            return;

        Vector3 forward = ev.Player.CameraTransform.forward;
        if (Physics.Raycast(ev.Player.CameraTransform.position + forward, forward, out var hit, 500))
        {
            EffectGrenade grenade = hit.collider.gameObject.GetComponentInParent<EffectGrenade>();
            if (grenade == null)
            {
                return;
            }

            if (PlacedCharges.ContainsKey(Pickup.Get(grenade)))
            {
                C4Handler(Pickup.Get(grenade), ShotMethod);
            }
        }
    }

    private void OnRoundEnded(RoundEndedEventArgs ev)
    {
        PlacedCharges.Clear();
    }
}