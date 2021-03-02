// -----------------------------------------------------------------------
// <copyright file="AntiMemeticPills.cs" company="Galaxy199 and iopietro">
// Copyright (c) Galaxy199 and iopietro. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems.Items
{
    using System.Collections.Generic;
    using CustomPlayerEffects;
    using Exiled.API.Features;
    using Exiled.CustomItems.API;
    using Exiled.CustomItems.API.Features;
    using Exiled.CustomItems.API.Spawn;
    using Exiled.Events.EventArgs;
    using MEC;

    /// <inheritdoc />
    public class AntiMemeticPills : CustomItem
    {
        /// <inheritdoc/>
        public override uint Id { get; set; } = 13;

        /// <inheritdoc/>
        public override string Name { get; set; } = "AM-119";

        /// <inheritdoc/>
        public override string Description { get; set; } =
            "Drugs that make you forget things. If you use these while you are targeted by SCP-096, you will forget what his face looks like, and thus no longer be a target.";

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties
        {
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new DynamicSpawnPoint { Chance = 100, Location = SpawnLocation.Inside096 },
            },
        };

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.UsingMedicalItem += OnUsingMedicalItem;
            base.SubscribeEvents();
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.UsingMedicalItem -= OnUsingMedicalItem;
            base.UnsubscribeEvents();
        }

        /// <summary>
        /// Handles the using of this item.
        /// </summary>
        /// <param name="ev"><see cref="UsingMedicalItemEventArgs"/>.</param>
        private void OnUsingMedicalItem(UsingMedicalItemEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            IEnumerable<Player> scp096s = Player.Get(RoleType.Scp096);

            Timing.CallDelayed(1f, () =>
            {
                foreach (Player scp in scp096s)
                    if (scp.CurrentScp is PlayableScps.Scp096 scp096 && scp096.HasTarget(ev.Player.ReferenceHub))
                        scp096._targets.Remove(ev.Player.ReferenceHub);
                ev.Player.EnableEffect<Concussed>(5f, true);
            });
        }
    }
}