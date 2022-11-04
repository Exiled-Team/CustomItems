// -----------------------------------------------------------------------
// <copyright file="TranquilizerGun.cs" company="Galaxy119 and iopietro">
// Copyright (c) Galaxy119 and iopietro. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems.Items
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using CustomPlayerEffects;
    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Spawn;
    using Exiled.CustomItems.API;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs;
    using MEC;
    using Mirror;
    using PlayerStatsSystem;
    using UnityEngine;
    using Ragdoll = Exiled.API.Features.Ragdoll;
    using Random = UnityEngine.Random;

    /// <inheritdoc />
    [CustomItem(ItemType.GunCOM18)]
    public class TranquilizerGun : CustomWeapon
    {
        private readonly Dictionary<Player, float> tranquilizedPlayers = new ();
        private readonly List<Player> activeTranqs = new ();

        /// <inheritdoc/>
        public override uint Id { get; set; } = 11;

        /// <inheritdoc/>
        public override string Name { get; set; } = "TG-119";

        /// <inheritdoc/>
        public override string Description { get; set; } = "This modifier USP fires non-lethal tranquilizing darts. Those affected will be rendered unconscious for a short duration. Unreliable against SCPs. Repeated tranquilizing of the same person will render them resistant to it's effect.";

        /// <inheritdoc/>
        public override float Weight { get; set; } = 1.55f;

        /// <inheritdoc />
        public override SpawnProperties SpawnProperties { get; set; } = new ()
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new ()
                {
                    Chance = 50,
                    Location = SpawnLocation.InsideGr18,
                },
                new ()
                {
                    Chance = 80,
                    Location = SpawnLocation.Inside173Armory,
                },
            },
        };

        /// <inheritdoc/>
        public override byte ClipSize { get; set; } = 2;

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
        public float ResistanceModifier { get; set; } = 1.2f;

        /// <summary>
        /// Gets or sets a value indicating how often player resistances are reduced.
        /// </summary>
        [Description("How often the plugin should reduce the resistance amount for players, in seconds.")]
        public float ResistanceFalloffDelay { get; set; } = 120f;

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
        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.PickingUpItem -= OnTranqPickingUpItem;
            activeTranqs.Clear();
            tranquilizedPlayers.Clear();
            Timing.KillCoroutines($"{nameof(TranquilizerGun)}-{Id}-reducer");
            base.UnsubscribeEvents();
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            Timing.RunCoroutine(ReduceResistances(), $"{nameof(TranquilizerGun)}-{Id}-reducer");
            Exiled.Events.Handlers.Player.PickingUpItem += OnTranqPickingUpItem;
            base.SubscribeEvents();
        }

        /// <inheritdoc/>
        protected override void OnHurting(HurtingEventArgs ev)
        {
            base.OnHurting(ev);

            if (ev.Attacker == ev.Target)
                return;

            if (ev.Target.Role.Team == Team.SCP)
            {
                int r = Random.Range(1, 101);
                Log.Debug($"{Name}: SCP roll: {r} (must be greater than {ScpResistChance})", CustomItems.Instance.Config.IsDebugEnabled);
                if (r <= ScpResistChance)
                {
                    Log.Debug($"{Name}: {r} is too low, no tranq.", CustomItems.Instance.Config.IsDebugEnabled);
                    return;
                }
            }

            float duration = Duration;

            if (!tranquilizedPlayers.TryGetValue(ev.Target, out _))
                tranquilizedPlayers.Add(ev.Target, 1);

            tranquilizedPlayers[ev.Target] *= ResistanceModifier;
            Log.Debug($"{Name}: Resistance Duration Mod: {tranquilizedPlayers[ev.Target]}", CustomItems.Instance.Config.IsDebugEnabled);

            duration -= tranquilizedPlayers[ev.Target];
            Log.Debug($"{Name}: Duration: {duration}", CustomItems.Instance.Config.IsDebugEnabled);

            if (duration > 0f)
                Timing.RunCoroutine(DoTranquilize(ev.Target, duration));
        }

        private IEnumerator<float> DoTranquilize(Player player, float duration)
        {
            activeTranqs.Add(player);
            Vector3 oldPosition = player.Position;
            Item previousItem = player.CurrentItem;
            Vector3 previousScale = player.Scale;
            float newHealth = player.Health - Damage;
            List<PlayerEffect> activeEffects = NorthwoodLib.Pools.ListPool<PlayerEffect>.Shared.Rent();

            if (newHealth <= 0)
                yield break;

            foreach (PlayerEffect effect in player.ReferenceHub.playerEffectsController.AllEffects.Values)
                if (effect.IsEnabled)
                    activeEffects.Add(effect);

            try
            {
                if (DropItems)
                {
                    if (player.Items.Count < 0)
                    {
                        foreach (Item item in player.Items.ToList())
                        {
                            if (TryGet(item, out CustomItem customItem))
                            {
                                customItem.Spawn(player.Position, item, player);
                                player.RemoveItem(item);
                            }
                        }

                        player.DropItems();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error($"{nameof(DoTranquilize)}: {e}");
            }

            Ragdoll ragdoll = new Ragdoll(player, new UniversalDamageHandler(0f, DeathTranslations.Warhead), true);

            player.IsInvisible = true;
            player.Scale = Vector3.one * 0.2f;
            player.Health = newHealth;
            player.IsGodModeEnabled = true;

            player.EnableEffect<Amnesia>(duration);
            player.EnableEffect<Ensnared>(duration);

            yield return Timing.WaitForSeconds(duration);

            try
            {
                if (ragdoll != null)
                    NetworkServer.Destroy(ragdoll.GameObject);

                if (player.GameObject == null)
                    yield break;

                newHealth = player.Health;

                player.IsGodModeEnabled = false;
                player.Scale = previousScale;
                player.Health = newHealth;
                player.IsInvisible = false;

                if (!DropItems)
                    player.CurrentItem = previousItem;

                foreach (PlayerEffect effect in activeEffects)
                    if ((effect.Duration - duration) > 0)
                        player.ReferenceHub.playerEffectsController.EnableEffect(effect, effect.Duration - duration);

                activeTranqs.Remove(player);
                NorthwoodLib.Pools.ListPool<PlayerEffect>.Shared.Return(activeEffects);
            }
            catch (Exception e)
            {
                Log.Error($"{nameof(DoTranquilize)}: {e}");
            }

            if (Warhead.IsDetonated && player.Position.y < 900)
            {
                player.Hurt(new UniversalDamageHandler(-1f, DeathTranslations.Warhead));
                yield break;
            }

            player.Position = oldPosition;
        }

        private IEnumerator<float> ReduceResistances()
        {
            for (; ;)
            {
                foreach (Player player in tranquilizedPlayers.Keys)
                    tranquilizedPlayers[player] = Mathf.Max(0, tranquilizedPlayers[player] / 2);

                yield return Timing.WaitForSeconds(ResistanceFalloffDelay);
            }
        }

        private void OnTranqPickingUpItem(PickingUpItemEventArgs ev)
        {
            if (activeTranqs.Contains(ev.Player))
                ev.IsAllowed = false;
        }
    }
}