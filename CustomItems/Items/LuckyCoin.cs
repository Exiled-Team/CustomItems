// -----------------------------------------------------------------------
// <copyright file="LuckyCoin.cs" company="Galaxy119 and iopietro">
// Copyright (c) Galaxy119 and iopietro. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems.Items
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using Exiled.API.Features;
    using Exiled.CustomItems.API;
    using Exiled.CustomItems.API.Features;
    using Exiled.CustomItems.API.Spawn;
    using Exiled.Events.EventArgs;
    using MEC;
    using UnityEngine;
    using Player = Exiled.Events.Handlers.Player;
    using Server = Exiled.Events.Handlers.Server;

    /// <inheritdoc />
    public class LuckyCoin : CustomItem
    {
        private readonly List<PocketDimensionTeleport> teleports = new List<PocketDimensionTeleport>();
        private bool isDropped;
        private bool onCooldown;

        /// <inheritdoc/>
        public override uint Id { get; set; } = 4;

        /// <inheritdoc/>
        public override string Name { get; set; } = "LC-119";

        /// <inheritdoc/>
        public override string Description { get; set; } = "This coin has magical properties when it is dropped inside of SCP-106's pocket dimension.";

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new DynamicSpawnPoint
                {
                    Chance = 100,
                    Location = SpawnLocation.Inside012Locker,
                },
                new DynamicSpawnPoint
                {
                    Chance = 50,
                    Location = SpawnLocation.Inside173Armory,
                },
            },
        };

        /// <summary>
        /// Gets or sets how long the coin will stay spawned inside the PD.
        /// </summary>
        [Description("How long the coin will stay spawned inside the PD.")]
        public float Duration { get; set; } = 2;

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            Server.RoundStarted += OnRoundStart;
            Player.EnteringPocketDimension += OnEnterPocketDimension;
            base.SubscribeEvents();
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            Server.RoundStarted -= OnRoundStart;
            Player.EnteringPocketDimension -= OnEnterPocketDimension;
            base.UnsubscribeEvents();
        }

        /// <inheritdoc/>
        protected override void OnPickingUp(PickingUpItemEventArgs ev)
        {
            if (ev.Pickup.itemId == ItemType.Coin && ev.Player.CurrentRoom.Name == "PocketWorld")
                ev.IsAllowed = false;

            base.OnPickingUp(ev);
        }

        /// <inheritdoc/>
        protected override void OnDropping(DroppingItemEventArgs ev)
        {
            if (!Check(ev.Item))
                return;

            if (ev.Player.CurrentRoom.Name == "PocketWorld")
            {
                ev.IsAllowed = false;
                Log.Debug($"{Name} has been dropped in the Pocket Dimension.", CustomItems.Instance.Config.IsDebugEnabled);
                isDropped = true;
                ev.Player.RemoveItem(ev.Item);
            }
            else
            {
                base.OnDropping(ev);
            }
        }

        private void OnRoundStart()
        {
            teleports.Clear();

            foreach (PocketDimensionTeleport teleport in Object.FindObjectsOfType<PocketDimensionTeleport>())
            {
                teleports.Add(teleport);
                Log.Debug("Adding PD Teleport..", CustomItems.Instance.Config.IsDebugEnabled);
            }
        }

        private void OnEnterPocketDimension(EnteringPocketDimensionEventArgs ev)
        {
            Log.Debug($"{ev.Player.Nickname} Entering Pocket Dimension.", CustomItems.Instance.Config.IsDebugEnabled);
            if (onCooldown)
            {
                Log.Debug($"{ev.Player.Nickname} - Not spawning, on cooldown.", CustomItems.Instance.Config.IsDebugEnabled);
                return;
            }

            if (!isDropped || !(ev.Position.y < -1900f))
                return;

            Log.Debug($"{ev.Player.Nickname} - EPD checks passed.", CustomItems.Instance.Config.IsDebugEnabled);
            foreach (PocketDimensionTeleport teleport in teleports)
            {
                Log.Debug($"{ev.Player.Nickname} - Checking teleporter..", CustomItems.Instance.Config.IsDebugEnabled);
                if (teleport.type != PocketDimensionTeleport.PDTeleportType.Exit)
                    continue;

                onCooldown = true;
                Log.Debug($"{ev.Player.Nickname} - Valid exit found..", CustomItems.Instance.Config.IsDebugEnabled);
                Vector3 tpPos = teleport.transform.position;
                float dist = Vector3.Distance(tpPos, ev.Position);
                Vector3 spawnPos = Vector3.MoveTowards(tpPos, ev.Position, 15);
                Log.Debug($"{ev.Player.Nickname} - TP: {tpPos}, Dist: {dist}, Spawn: {spawnPos}", CustomItems.Instance.Config.IsDebugEnabled);

                Pickup coin = Exiled.API.Extensions.Item.Spawn(ItemType.Coin, 0f, spawnPos);

                Timing.CallDelayed(Duration, () => coin.Delete());
                Timing.CallDelayed(120f, () => onCooldown = false);
                break;
            }
        }
    }
}