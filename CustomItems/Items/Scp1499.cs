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
        public static Dictionary<Player, Vector3> Scp1499Players => new Dictionary<Player, Vector3>();

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
                ev.Player.Position = Scp1499Players[ev.Player];

                if (Warhead.IsDetonated && Scp1499Players[ev.Player].y < 800)
                {
                    ev.Player.Kill(DamageTypes.Nuke);
                }
                else
                if (Map.IsLCZDecontaminated && Scp1499Players[ev.Player].y > -500)
                {
                    ev.Player.Kill(DamageTypes.Decont);
                }

                Scp1499Players.Remove(ev.Player);
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
                    if (!Scp1499Players.ContainsKey(ev.Player))
                        return;

                    ev.Player.Position = Scp1499Players[ev.Player];

                    bool shouldKill = false;
                    if (Warhead.IsDetonated)
                    {
                        if (ev.Player.CurrentRoom.Zone != ZoneType.Surface)
                        {
                            shouldKill = true;
                        }
                        else
                        {
                            foreach (Lift lift in Map.Lifts)
                                if (lift.elevatorName.Contains("Gate"))
                                    if (Vector3.Distance(ev.Player.Position, lift.transform.position) <= 4.5f)
                                    {
                                        shouldKill = true;
                                        break;
                                    }
                        }

                        if (shouldKill)
                            ev.Player.Kill(DamageTypes.Nuke);
                    }
                    else if (Map.IsLCZDecontaminated)
                    {
                        if (ev.Player.CurrentRoom.Zone == ZoneType.LightContainment)
                        {
                            shouldKill = true;
                        }
                        else
                        {
                            foreach (Lift lift in Map.Lifts)
                                if (lift.elevatorName.Contains("El"))
                                    if (Vector3.Distance(ev.Player.Position, lift.transform.position) <= 4.5f)
                                    {
                                        shouldKill = true;
                                        break;
                                    }
                        }

                        if (shouldKill)
                            ev.Player.Kill(DamageTypes.Decont);
                    }

                    Scp1499Players.Remove(ev.Player);
                });
            }
        }
    }
}
