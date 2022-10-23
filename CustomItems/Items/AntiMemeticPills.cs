// -----------------------------------------------------------------------
// <copyright file="AntiMemeticPills.cs" company="Galaxy119 and iopietro">
// Copyright (c) Galaxy119 and iopietro. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems.Items
{
    using System.Collections.Generic;
    using CustomPlayerEffects;
    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Spawn;
    using Exiled.CustomItems.API;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs;
    using MEC;

    /// <inheritdoc />
    [CustomItem(ItemType.SCP500)]
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
        public override float Weight { get; set; } = 1f;

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; set; } = new ()
        {
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new DynamicSpawnPoint { Chance = 100, Location = SpawnLocation.Inside096 },
            },
        };

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.UsingItem += OnUsingItem;
            base.SubscribeEvents();
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.UsingItem -= OnUsingItem;
            base.UnsubscribeEvents();
        }

        private void OnUsingItem(UsingItemEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            IEnumerable<Player> scp096S = Player.Get(RoleType.Scp096);

            Timing.CallDelayed(1f, () =>
            {
                foreach (Player scp in scp096S)
                    if (scp.CurrentScp is PlayableScps.Scp096 scp096 && scp096.HasTarget(ev.Player.ReferenceHub))
                        scp096._targets.Remove(ev.Player.ReferenceHub);
                ev.Player.EnableEffect<Amnesia>(10f, true);
            });
        }
    }
}