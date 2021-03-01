// <copyright file="Scp1499.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CustomItems.Items
{
    using System.Collections.Generic;
    using CustomItems.API;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;
    using MEC;
    using UnityEngine;

    /// <inheritdoc />
    public class Scp1499 : CustomItem
    {
        private readonly Vector3 scp1499DimensionPos = new Vector3(152.93f, 978.03f, 93.64f); // This position is where is unused terrain on the Surface

        /// <inheritdoc />
        public Scp1499(ItemType type, int itemId)
            : base(type, itemId)
        {
        }

        /// <summary>
        ///  Gets a <see cref="Dictionary{TKey,TValue}"/> of players in the 1499 dimension and their tp-back coordinates.
        /// </summary>
        public static Dictionary<Player, Vector3> Scp1499Players { get; } = new Dictionary<Player, Vector3>();

        /// <inheritdoc/>
        public override string Name { get; set; } = Plugin.Singleton.Config.ItemConfigs.Scp1499Cfg.Name;

        /// <inheritdoc/>
        public override Dictionary<SpawnLocation, float> SpawnLocations { get; set; } = Plugin.Singleton.Config.ItemConfigs.Scp1499Cfg.SpawnLocations;

        /// <inheritdoc/>
        public override string Description { get; set; } = Plugin.Singleton.Config.ItemConfigs.Scp1499Cfg.Description;

        /// <inheritdoc/>
        public override int SpawnLimit { get; set; } = Plugin.Singleton.Config.ItemConfigs.Scp1499Cfg.SpawnLimit;

        /// <inheritdoc/>
        protected override void LoadEvents()
        {
            Exiled.Events.Handlers.Player.MedicalItemDequipped += OnDequippedMedicalItem;
            base.LoadEvents();
        }

        /// <inheritdoc/>
        protected override void UnloadEvents()
        {
            Exiled.Events.Handlers.Player.MedicalItemDequipped -= OnDequippedMedicalItem;
            base.UnloadEvents();
        }

        /// <inheritdoc/>
        protected override void OnDroppingItem(DroppingItemEventArgs ev)
        {
            if (Scp1499Players.ContainsKey(ev.Player))
                ev.IsAllowed = false;

            if (!CheckItem(ev.Item))
                return;

            if (Scp1499Players.ContainsKey(ev.Player))
            {
                ev.IsAllowed = false;
                SendPlayerBack(ev.Player);
            }
            else
            {
                base.OnDroppingItem(ev);
            }
        }

        /// <inheritdoc/>
        protected override void OnWaitingForPlayers()
        {
            Scp1499Players.Clear();
            base.OnWaitingForPlayers();
        }

        private void OnDequippedMedicalItem(DequippedMedicalItemEventArgs ev)
        {
            if (!CheckItem(ev.Player.CurrentItem))
                return;

            if (ev.Player.CurrentRoom.Name == "PocketWorld")
                return;

            if (Scp1499Players.ContainsKey(ev.Player))
                Scp1499Players[ev.Player] = ev.Player.Position;
            else
                Scp1499Players.Add(ev.Player, ev.Player.Position);

            ev.Player.Position = scp1499DimensionPos;
            ev.Player.ReferenceHub.playerEffectsController.DisableEffect<CustomPlayerEffects.Scp268>();

            if (Plugin.Singleton.Config.ItemConfigs.Scp1499Cfg.Duration > 0)
            {
                Timing.CallDelayed(Plugin.Singleton.Config.ItemConfigs.Scp1499Cfg.Duration, () =>
                {
                    SendPlayerBack(ev.Player);
                });
            }
        }

        private void SendPlayerBack(Player player)
        {
            if (!Scp1499Players.ContainsKey(player))
                return;

            player.Position = Scp1499Players[player];

            bool shouldKill = false;
            if (Warhead.IsDetonated)
            {
                if (player.CurrentRoom.Zone != ZoneType.Surface)
                {
                    shouldKill = true;
                }
                else
                {
                    foreach (Lift lift in Map.Lifts)
                        if (lift.elevatorName.Contains("Gate"))
                            foreach (Lift.Elevator elevator in lift.elevators)
                                if (Vector3.Distance(player.Position, elevator.target.position) <= 3.5f)
                                {
                                    shouldKill = true;
                                    break;
                                }
                }

                if (shouldKill)
                    player.Kill(DamageTypes.Nuke);
            }
            else if (Map.IsLCZDecontaminated)
            {
                if (player.CurrentRoom.Zone == ZoneType.LightContainment)
                {
                    shouldKill = true;
                }
                else
                {
                    foreach (Lift lift in Map.Lifts)
                        if (lift.elevatorName.Contains("El"))
                            foreach (Lift.Elevator elevator in lift.elevators)
                                if (Vector3.Distance(player.Position, elevator.target.position) <= 3.5f)
                                {
                                    shouldKill = true;
                                    break;
                                }
                }

                if (shouldKill)
                    player.Kill(DamageTypes.Decont);
            }

            Scp1499Players.Remove(player);
        }
    }
}
