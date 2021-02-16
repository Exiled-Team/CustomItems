// <copyright file="TranqGun.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CustomItems.Items
{
    using System.Collections.Generic;
    using Exiled.API.Features;
    using Exiled.CustomItems.API;
    using Exiled.Events.EventArgs;
    using MEC;
    using UnityEngine;

    /// <inheritdoc />
    public class TranqGun : CustomWeapon
    {
        private readonly Dictionary<Player, int> tranquilizedPlayers = new Dictionary<Player, int>();

        /// <inheritdoc />
        public TranqGun(ItemType type, int clipSize, int itemId)
            : base(type, clipSize, itemId)
        {
        }

        /// <inheritdoc/>
        public override string Name { get; set; } = Plugin.Singleton.Config.ItemConfigs.TranqCfg.Name;

        /// <inheritdoc/>
        public override string Description { get; set; } = Plugin.Singleton.Config.ItemConfigs.TranqCfg.Description;

        /// <inheritdoc/>
        public override int SpawnLimit { get; set; } = Plugin.Singleton.Config.ItemConfigs.TranqCfg.SpawnLimit;

        /// <inheritdoc/>
        protected override void LoadEvents()
        {
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            base.LoadEvents();
        }

        /// <inheritdoc/>
        protected override void UnloadEvents()
        {
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
            base.UnloadEvents();
        }

        private static IEnumerator<float> DoTranquilize(Player player, float duration)
        {
            Vector3 pos = player.Position;

            if (Plugin.Singleton.Config.ItemConfigs.TranqCfg.DropItems)
                player.DropItems();

            Ragdoll ragdoll = Map.SpawnRagdoll(player, DamageTypes.None, pos, allowRecall: false);
            player.Position = new Vector3(0, 0, 0);

            yield return Timing.WaitForSeconds(duration);

            player.Position = pos;
            Object.Destroy(ragdoll.gameObject);
        }

        private void OnHurting(HurtingEventArgs ev)
        {
            if (!CheckItem(ev.Attacker.CurrentItem))
                return;

            ev.Amount = 0;

            if (ev.Target.Team == Team.SCP && Plugin.Singleton.Config.ItemConfigs.TranqCfg.ResistantScps)
                if (Plugin.Singleton.Rng.Next(100) <= Plugin.Singleton.Config.ItemConfigs.TranqCfg.ScpResistChance)
                    return;

            float dur = Plugin.Singleton.Config.ItemConfigs.TranqCfg.Duration;
            if (!tranquilizedPlayers.ContainsKey(ev.Target))
                tranquilizedPlayers.Add(ev.Target, 0);

            dur -= tranquilizedPlayers[ev.Target] * Plugin.Singleton.Config.ItemConfigs.TranqCfg.ResistanceModifier;

            if (dur > 0f)
                Timing.RunCoroutine(DoTranquilize(ev.Target, dur));
        }
    }
}