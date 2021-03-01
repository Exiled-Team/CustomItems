// -----------------------------------------------------------------------
// <copyright file="Scp1499.cs" company="Galaxy199 and iopietro">
// Copyright (c) Galaxy199 and iopietro. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems.Items
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using CustomPlayerEffects;
    using Exiled.API.Features;
    using Exiled.CustomItems.API;
    using Exiled.CustomItems.API.Features;
    using Exiled.CustomItems.API.Spawn;
    using Exiled.Events.EventArgs;
    using MEC;
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
            Exiled.Events.Handlers.Player.MedicalItemUsed += OnUsedMedicalItem;

            base.SubscribeEvents();
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.MedicalItemUsed -= OnUsedMedicalItem;

            base.UnsubscribeEvents();
        }

        /// <inheritdoc/>
        protected override void OnDropping(DroppingItemEventArgs ev)
        {
            if (scp1499Players.ContainsKey(ev.Player))
            {
                ev.IsAllowed = false;
                ev.Player.Position = scp1499Players[ev.Player];

                scp1499Players.Remove(ev.Player);
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

        private void OnUsedMedicalItem(UsedMedicalItemEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            if (scp1499Players.ContainsKey(ev.Player))
                scp1499Players[ev.Player] = ev.Player.Position;
            else
                scp1499Players.Add(ev.Player, ev.Player.Position);

            ev.Player.Position = scp1499DimensionPos;
            ev.Player.ReferenceHub.playerEffectsController.DisableEffect<Scp268>();

            if (Duration > 0)
            {
                Timing.CallDelayed(Duration, () =>
                {
                    if (!scp1499Players.ContainsKey(ev.Player))
                        return;

                    ev.Player.Position = scp1499Players[ev.Player];

                    scp1499Players.Remove(ev.Player);
                });
            }
        }
    }
}
