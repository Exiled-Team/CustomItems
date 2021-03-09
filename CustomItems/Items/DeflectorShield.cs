// -----------------------------------------------------------------------
// <copyright file="DeflectorShield.cs" company="Babyboucher20">
// Copyright (c) Babyboucher20. All rights reserved.
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
    using Exiled.CustomItems.API;
    using Exiled.CustomItems.API.Features;
    using Exiled.CustomItems.API.Spawn;
    using Exiled.Events.EventArgs;
    using MEC;
    using UnityEngine;

    /// <inheritdoc />
    public class DeflectorShield : CustomItem
    {
        private readonly List<Player> deflectorPlayers = new List<Player>();

        /// <inheritdoc/>
        public override uint Id { get; set; } = 18;

        /// <inheritdoc/>
        public override string Name { get; set; } = "Deflector shield";

        /// <inheritdoc/>
        public override string Description { get; set; } = "A deflector shield that relfects bullets back at the shooter";

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
            Exiled.Events.Handlers.Player.MedicalItemDequipped += OnMedicalItemDeEquipped;
            Exiled.Events.Handlers.Player.Destroying += OnDestroying;
            Exiled.Events.Handlers.Player.Died += OnDied;
            Exiled.Events.Handlers.Player.Hurting += OnHurt;

            base.SubscribeEvents();
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.MedicalItemDequipped -= OnMedicalItemDeEquipped;
            Exiled.Events.Handlers.Player.Destroying -= OnDestroying;
            Exiled.Events.Handlers.Player.Died -= OnDied;
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

        private void OnDied(DiedEventArgs ev)
        {
            if (deflectorPlayers.Contains(ev.Target))
                deflectorPlayers.Remove(ev.Target);
        }

        private void OnDestroying(DestroyingEventArgs ev)
        {
            if (deflectorPlayers.Contains(ev.Player))
                deflectorPlayers.Remove(ev.Player);
        }

        private void OnMedicalItemDeEquipped(DequippedMedicalItemEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            if (!deflectorPlayers.Contains(ev.Player))
                deflectorPlayers.Add(ev.Player);

            ev.Player.ReferenceHub.playerEffectsController.DisableEffect<Scp268>();

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
            if (deflectorPlayers.Contains(ev.Target) && ev.DamageType.isWeapon)
            {
                ev.IsAllowed = false;
                ev.Attacker.Hurt(ev.Amount * Multiplier, ev.Target, ev.DamageType);
            }
        }

    }
}
