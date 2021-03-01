// -----------------------------------------------------------------------
// <copyright file="TranquilizerGun.cs" company="Galaxy199 and iopietro">
// Copyright (c) Galaxy199 and iopietro. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using System.Linq;

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

    /// <inheritdoc />
    public class TranquilizerGun : CustomWeapon
    {
        private readonly Dictionary<Player, int> tranquilizedPlayers = new Dictionary<Player, int>();

        /// <inheritdoc/>
        public override uint Id { get; set; } = 11;

        /// <inheritdoc/>
        public override string Name { get; set; } = "TG-119";

        /// <inheritdoc/>
        public override string Description { get; set; } = "This modifier USP fires non-lethal tranquilizing darts. Those affected will be rendered unconscious for a short duration. Unreliable against SCPs. Repeated tranquilizing of the same person will render them resistant to it's effect.";

        /// <inheritdoc />
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new DynamicSpawnPoint
                {
                    Chance = 50,
                    Location = SpawnLocation.InsideGr18,
                },
                new DynamicSpawnPoint
                {
                    Chance = 80,
                    Location = SpawnLocation.Inside173Armory,
                },
            },
        };

        /// <inheritdoc/>
        public override Modifiers Modifiers { get; set; } = default;

        /// <inheritdoc/>
        public override uint ClipSize { get; set; } = 2;

        /// <inheritdoc/>
        public override float Damage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not SCPs should be resistant to tranquilizers. (Being resistant gives them a chance to not be tranquilized when shot).
        /// </summary>
        [Description("Whether or not SCPs should be resistant to tranquilizers. (Being resistant gives them a chance to not be tranquilized when shot).")]
        public bool ResistantScps { get; set; } = true;

        /// <summary>
        /// Gets or sets the amount of time a successful tranquilization lasts for.
        /// </summary>
        [Description("The amount of time a successful tranquilization lasts for.")]
        public float Duration { get; set; } = 5f;

        /// <summary>
        /// Gets or sets the exponential modifier used to determine how much time is removed from the effect, everytime a player is tranquilized, they gain a resistance to further tranquilizations, reducing the duration of future effects.
        /// </summary>
        [Description("Everytime a player is tranquilized, they gain a resistance to further tranquilizations, reducing the duration of future effects. This number signifies the exponential modifier used to determine how much time is removed from the effect.")]
        public int ResistanceModifier { get; set; } = 2;

        /// <summary>
        /// Gets or sets a value indicating whether or not tranquilized targets should drop all of their items.
        /// </summary>
        [Description("Whether or not tranquilized targets should drop all of their items.")]
        public bool DropItems { get; set; } = true;

        /// <summary>
        /// Gets or sets the percent chance an SCP will resist being tranquilized. This has no effect if ResistantScps is false.
        /// </summary>
        [Description("The percent chance an SCP will resist being tranquilized. This has no effect if ResistantScps is false.")]
        public int ScpResistChance { get; set; } = 40;

        /// <inheritdoc/>
        protected override void OnHurting(HurtingEventArgs ev)
        {
            if (!Check(ev.Attacker.CurrentItem) || ev.Attacker == ev.Target)
                return;

            ev.Amount = Damage;

            if (ev.Target.Team == Team.SCP && ResistantScps)
                if (UnityEngine.Random.Range(1, 101) <= ScpResistChance)
                    return;

            float dur = Duration;
            if (!tranquilizedPlayers.ContainsKey(ev.Target))
                tranquilizedPlayers.Add(ev.Target, 0);

            dur -= tranquilizedPlayers[ev.Target] * ResistanceModifier;

            if (dur > 0f)
                Timing.RunCoroutine(DoTranquilize(ev.Target, dur));
        }

        private IEnumerator<float> DoTranquilize(Player player, float duration)
        {
            Vector3 pos = player.Position;

            foreach (Inventory.SyncItemInfo item in player.Inventory.items.ToList())
            {
                if (TryGet(item, out CustomItem cItem))
                    cItem.Spawn(player.Position);
                player.Inventory.items.Remove(item);
            }

            if (DropItems)
                player.DropItems();

            Ragdoll ragdoll = Map.SpawnRagdoll(player, DamageTypes.None, pos, allowRecall: false);
            player.Position = new Vector3(0, 0, 0);

            yield return Timing.WaitForSeconds(duration);

            player.Position = pos;
            Object.Destroy(ragdoll.gameObject);
        }
    }
}