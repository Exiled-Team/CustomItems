// -----------------------------------------------------------------------
// <copyright file="LuckyCoin.cs" company="Galaxy199 and iopietro">
// Copyright (c) Galaxy199 and iopietro. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems.Items
{
    using System.Collections.Generic;
    using Exiled.API.Features;
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

        /*/// <inheritdoc />
        public LuckyCoin(ItemType type, uint itemId)
            : base(type, itemId)
        {
        }*/

        /// <inheritdoc/>
        public override string Name { get; } = CustomItems.Instance.Config.ItemConfigs.LuckyCfg.Name;

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; protected set; } = CustomItems.Instance.Config.ItemConfigs.LuckyCfg.SpawnProperties;

        /// <inheritdoc/>
        public override string Description { get; } = CustomItems.Instance.Config.ItemConfigs.LuckyCfg.Description;

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
                Log.Debug($"{Name} has been dropped in the Pocket Dimension.", CustomItems.Instance.Config.Debug);
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
            foreach (PocketDimensionTeleport teleport in Object.FindObjectsOfType<PocketDimensionTeleport>())
            {
                teleports.Add(teleport);
                Log.Debug("Adding PD Teleport..");
            }
        }

        private void OnEnterPocketDimension(EnteringPocketDimensionEventArgs ev)
        {
            Log.Debug($"{ev.Player.Nickname} Entering Pocket Dimension.", CustomItems.Instance.Config.Debug);
            if (onCooldown)
            {
                Log.Debug($"{ev.Player.Nickname} - Not spawning, on cooldown.", CustomItems.Instance.Config.Debug);
                return;
            }

            if (!isDropped || !(ev.Position.y < -1900f))
                return;

            Log.Debug($"{ev.Player.Nickname} - EPD checks passed.", CustomItems.Instance.Config.Debug);
            foreach (PocketDimensionTeleport teleport in teleports)
            {
                Log.Debug($"{ev.Player.Nickname} - Checking teleporter..", CustomItems.Instance.Config.Debug);
                if (teleport.type != PocketDimensionTeleport.PDTeleportType.Exit)
                    continue;

                onCooldown = true;
                Log.Debug($"{ev.Player.Nickname} - Valid exit found..", CustomItems.Instance.Config.Debug);
                Vector3 tpPos = teleport.transform.position;
                float dist = Vector3.Distance(tpPos, ev.Position);
                Vector3 spawnPos = Vector3.MoveTowards(tpPos, ev.Position, 15);
                Log.Debug($"{ev.Player.Nickname} - TP: {tpPos}, Dist: {dist}, Spawn: {spawnPos}", CustomItems.Instance.Config.Debug);

                Pickup coin = Exiled.API.Extensions.Item.Spawn(ItemType.Coin, 0f, spawnPos);

                Timing.CallDelayed(CustomItems.Instance.Config.ItemConfigs.LuckyCfg.Duration, () => coin.Delete());
                Timing.CallDelayed(120f, () => onCooldown = false);
                break;
            }
        }
    }
}