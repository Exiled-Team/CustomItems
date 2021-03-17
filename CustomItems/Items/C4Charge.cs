﻿// -----------------------------------------------------------------------
// <copyright file="C4Charge.cs" company="Galaxy119 and iopietro">
// Copyright (c) Galaxy119 and iopietro. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems.Items
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Exiled.API.Features;
    using Exiled.CustomItems.API;
    using Exiled.CustomItems.API.Features;
    using Exiled.CustomItems.API.Spawn;
    using Exiled.Events.EventArgs;
    using Grenades;
    using Mirror;
    using UnityEngine;
    using YamlDotNet.Serialization;

    using PlayerEvent = Exiled.Events.Handlers.Player;

    /// <inheritdoc/>
    public class C4Charge : CustomGrenade
    {
        /// <inheritdoc/>
        public static C4Charge Instance;

        /// <inheritdoc/>
        public static Dictionary<Grenade, Player> PlacedCharges = new Dictionary<Grenade, Player>();

        /// <inheritdoc/>
        public override uint Id { get; set; } = 15;

        /// <inheritdoc/>
        public override string Name { get; set; } = "C4-119";

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties
        {
            Limit = 5,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new DynamicSpawnPoint
                {
                    Chance = 10,
                    Location = SpawnLocation.InsideLczArmory,
                },

                new DynamicSpawnPoint
                {
                    Chance = 25,
                    Location = SpawnLocation.InsideHczArmory,
                },

                new DynamicSpawnPoint
                {
                    Chance = 50,
                    Location = SpawnLocation.InsideNukeArmory,
                },

                new DynamicSpawnPoint
                {
                    Chance = 50,
                    Location = SpawnLocation.Inside049Armory,
                },

                new DynamicSpawnPoint
                {
                    Chance = 100,
                    Location = SpawnLocation.InsideSurfaceNuke,
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
        public override ItemType Type { get; set; } = ItemType.GrenadeFrag;

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            Instance = this;

            PlayerEvent.Destroying += OnDestroying;
            PlayerEvent.Died += OnDied;
            PlayerEvent.Shooting += OnShooting;

            base.SubscribeEvents();
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            Instance = null;

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

        /// <inheritdoc/>
        protected override void OnThrowing(ThrowingGrenadeEventArgs ev)
        {
            ev.IsAllowed = false;
            ev.Player.RemoveItem(ev.Player.CurrentItem);

            float slowThrowMultiplier = 0.1f;

            if (!ev.IsSlow)
                slowThrowMultiplier = 1f;

            Grenade c4 = Spawn(ev.Player.CameraTransform.position, (ev.Player.Rotation * ThrowMultiplier * slowThrowMultiplier) + (Vector3.up * 1.5f), FuseTime, ItemType.GrenadeFrag, ev.Player);

            if (!PlacedCharges.ContainsKey(c4))
                PlacedCharges.Add(c4, ev.Player);

            base.OnThrowing(ev);
        }

        /// <inheritdoc/>
        protected override void OnExploding(ExplodingGrenadeEventArgs ev)
        {
            if (ev.Grenade.TryGetComponent(out Grenade grenade))
            {
                PlacedCharges.Remove(grenade);
            }
        }

        private void OnDestroying(DestroyingEventArgs ev)
        {
            foreach (var charge in PlacedCharges.ToList())
            {
                if (charge.Value == ev.Player)
                {
                    C4Handler(charge.Key, MethodOnDeath);
                }
            }
        }

        private void OnDied(DiedEventArgs ev)
        {
            foreach (var charge in PlacedCharges.ToList())
            {
                if (charge.Value == ev.Target)
                {
                    C4Handler(charge.Key, MethodOnDeath);
                }
            }
        }

        private void OnShooting(ShootingEventArgs ev)
        {
            if (!AllowShoot)
                return;

            Vector3 forward = ev.Shooter.CameraTransform.forward;
            if (Physics.Raycast(ev.Shooter.CameraTransform.position + forward, forward, out var hit, 500))
            {
                Grenade grenade = hit.collider.gameObject.GetComponentInParent<Grenade>();
                if (grenade == null)
                {
                    return;
                }

                if (PlacedCharges.ContainsKey(grenade))
                {
                    C4Handler(grenade, ShotMethod);
                }
            }
        }

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

        /// <inheritdoc/>
        public void C4Handler(Grenade charge, C4RemoveMethod removeMethod = C4RemoveMethod.Detonate)
        {
            switch (removeMethod)
            {
                case C4RemoveMethod.Remove:
                    {
                        NetworkServer.Destroy(charge.gameObject);
                        break;
                    }

                case C4RemoveMethod.Detonate:
                    {
                        charge.NetworkfuseTime = 0.1f;
                        break;
                    }

                case C4RemoveMethod.Drop:
                    {
                        TrySpawn((int)Id, charge.transform.position);
                        NetworkServer.Destroy(charge.gameObject);
                        break;
                    }
            }

            PlacedCharges.Remove(charge);
        }
    }
}
