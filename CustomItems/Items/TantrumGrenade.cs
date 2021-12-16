namespace CustomItems.Items
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Exiled.API.Features;
    using Exiled.API.Features.Spawn;
    using Exiled.CustomItems.API;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs;
    using InventorySystem.Items.ThrowableProjectiles;
    using MEC;
    using UnityEngine;
    using Scp106 = Exiled.Events.Handlers.Scp106;

    /// <inheritdoc />
    public class TantrumGrenade : CustomGrenade
    {
        /// <inheritdoc/>
        public override uint Id { get; set; } = 16;

        /// <inheritdoc/>
        public override string Name { get; set; } = "TG2-119";

        /// <inheritdoc/>
        public override string Description { get; set; } = "This grenade does almost 0 damage, creates a tantrum after an explosion";

        /// <inheritdoc/>
        public override float Weight { get; set; } = 0.5f;

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new DynamicSpawnPoint
                {
                    Chance = 50,
                    Location = SpawnLocation.Inside173Armory,
                },
                new DynamicSpawnPoint
                {
                    Chance = 100,
                    Location = SpawnLocation.Inside173Gate,
                },
            },
        };

        /// <inheritdoc/>
        public override bool ExplodeOnCollision { get; set; } = true;

        /// <inheritdoc/>
        public override float FuseTime { get; set; } = 1.5f;

        protected override void OnExploding(ExplodingGrenadeEventArgs ev)
        {
            ev.IsAllowed = false;
            Map.PlaceTantrum(ev.Grenade.transform.position);
            base.OnExploding(ev);
        }
    }
}
