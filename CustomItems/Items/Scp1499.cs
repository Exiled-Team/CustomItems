// -----------------------------------------------------------------------
// <copyright file="Scp1499.cs" company="Galaxy119 and iopietro">
// Copyright (c) Galaxy119 and iopietro. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems.Items
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using CustomPlayerEffects;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Spawn;
    using Exiled.API.Structs;
    using Exiled.CustomItems.API;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs;
    using MEC;
    using PlayerStatsSystem;
    using UnityEngine;

    /// <inheritdoc />
    public class Scp1499 : CustomItem
    {
        // This position is where is unused terrain on the Surface
        private readonly Vector3 scp1499DimensionPos = new Vector3(152.93f, 978.03f, 93.64f);
        private readonly Dictionary<Player, Vector3> scp1499Players = new Dictionary<Player, Vector3>();

        /// <inheritdoc/>
        public override uint Id { get; set; } = 8;

        /// <inheritdoc/>
        public override string Name { get; set; } = "SCP-1499";

        /// <inheritdoc/>
        public override string Description { get; set; } = "The gas mask that temporarily teleports you to another dimension, when you put it on.";

        /// <inheritdoc/>
        public override float Weight { get; set; } = 1.5f;

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new DynamicSpawnPoint
                {
                    Chance = 10,
                    Location = SpawnLocation.InsideHid,
                },
            },
        };

        /// <summary>
        /// Gets or sets how long the SCP-1499 can be wore, before automaticly player takes it off. (set to 0 for no limit).
        /// </summary>
        [Description("How long the SCP-1499 can be wore, before automaticly player takes it off. (set to 0 for no limit)")]
        public float Duration { get; set; } = 15f;

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.UsedItem += OnUsedItem;
            Exiled.Events.Handlers.Player.Destroying += OnDestroying;
            Exiled.Events.Handlers.Player.Died += OnDied;

            base.SubscribeEvents();
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.UsedItem -= OnUsedItem;
            Exiled.Events.Handlers.Player.Destroying -= OnDestroying;
            Exiled.Events.Handlers.Player.Died -= OnDied;

            base.UnsubscribeEvents();
        }

        /// <inheritdoc/>
        protected override void OnDropping(DroppingItemEventArgs ev)
        {
            if (scp1499Players.ContainsKey(ev.Player) && Check(ev.Item))
            {
                ev.IsAllowed = false;

                SendPlayerBack(ev.Player);
            }
            else
            {
                base.OnDropping(ev);
            }
        }

        /// <inheritdoc/>
        protected override void OnWaitingForPlayers()
        {
            scp1499Players.Clear();

            base.OnWaitingForPlayers();
        }

        private void OnDied(DiedEventArgs ev)
        {
            if (scp1499Players.ContainsKey(ev.Target))
                scp1499Players.Remove(ev.Target);
        }

        private void OnDestroying(DestroyingEventArgs ev)
        {
            if (scp1499Players.ContainsKey(ev.Player))
                scp1499Players.Remove(ev.Player);
        }

        private void OnUsedItem(UsedItemEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            if (scp1499Players.ContainsKey(ev.Player))
                scp1499Players[ev.Player] = ev.Player.Position;
            else
                scp1499Players.Add(ev.Player, ev.Player.Position);

            ev.Player.Position = scp1499DimensionPos;
            ev.Player.ReferenceHub.playerEffectsController.DisableEffect<Invisible>();

            if (Duration > 0)
            {
                Timing.CallDelayed(Duration, () =>
                {
                    SendPlayerBack(ev.Player);
                });
            }
        }

        private void SendPlayerBack(Player player)
        {
            if (!scp1499Players.ContainsKey(player))
                return;

            player.Position = scp1499Players[player];

            bool shouldKill = false;
            if (Warhead.IsDetonated)
            {
                if (player.CurrentRoom.Zone != ZoneType.Surface)
                {
                    shouldKill = true;
                }
                else
                {
                    foreach (Lift lift in Lift.List)
                        if (lift.Name.Contains("Gate"))
                            foreach (Elevator elevator in lift.Elevators)
                                if (Vector3.Distance(player.Position, elevator.Target.position) <= 3.5f)
                                {
                                    shouldKill = true;
                                    break;
                                }
                }

                if (shouldKill)
                    player.Hurt(new WarheadDamageHandler());
            }
            else if (Map.IsLczDecontaminated)
            {
                if (player.CurrentRoom.Zone == ZoneType.LightContainment)
                {
                    shouldKill = true;
                }
                else
                {
                    foreach (Lift lift in Lift.List)
                        if (lift.Name.Contains("El"))
                            foreach (Elevator elevator in lift.Elevators)
                                if (Vector3.Distance(player.Position, elevator.Target.position) <= 3.5f)
                                {
                                    shouldKill = true;
                                    break;
                                }
                }

                if (shouldKill)
                    player.Hurt(new UniversalDamageHandler(-1f, DeathTranslations.Decontamination));
            }

            scp1499Players.Remove(player);
        }
    }
}
