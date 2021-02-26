﻿// -----------------------------------------------------------------------
// <copyright file="Scp1499.cs" company="Galaxy199 and iopietro">
// Copyright (c) Galaxy199 and iopietro. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems.Items
{
    using System.Collections.Generic;
    using CustomPlayerEffects;
    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;
    using Exiled.CustomItems.API.Spawn;
    using Exiled.Events.EventArgs;
    using MEC;
    using UnityEngine;

    /// <inheritdoc />
    public class Scp1499 : CustomItem
    {
        private readonly Vector3 scp1499DimensionPos = new Vector3(152.93f, 978.03f, 93.64f); // This position is where is unused terrain on the Surface
        private readonly Dictionary<Player, Vector3> scp1499Players = new Dictionary<Player, Vector3>();

        /*/// <inheritdoc />
        public Scp1499(ItemType type, uint itemId)
            : base(type, itemId)
        {
        }*/

        /// <inheritdoc/>
        public override string Name { get; } = CustomItems.Instance.Config.ItemConfigs.Scp1499Cfg.Name;

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; protected set; } = CustomItems.Instance.Config.ItemConfigs.Scp1499Cfg.SpawnProperties;

        /// <inheritdoc/>
        public override string Description { get; } = CustomItems.Instance.Config.ItemConfigs.Scp1499Cfg.Description;

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
                ev.IsAllowed = false;

            if (!Check(ev.Item))
                return;

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

            if (CustomItems.Instance.Config.ItemConfigs.Scp1499Cfg.Duration > 0)
            {
                Timing.CallDelayed(CustomItems.Instance.Config.ItemConfigs.Scp1499Cfg.Duration, () =>
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
