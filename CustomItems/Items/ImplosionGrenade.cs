// -----------------------------------------------------------------------
// <copyright file="ImplosionGrenade.cs" company="Galaxy199 and iopietro">
// Copyright (c) Galaxy199 and iopietro. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems.Items
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Exiled.API.Features;
    using Exiled.CustomItems.API;
    using Exiled.CustomItems.API.Features;
    using Exiled.CustomItems.API.Spawn;
    using Exiled.Events.EventArgs;
    using Grenades;
    using MEC;
    using UnityEngine;
    using Map = Exiled.Events.Handlers.Map;

    /// <inheritdoc />
    public class ImplosionGrenade : CustomGrenade
    {
        private int layerMask;

        /// <inheritdoc/>
        public override uint Id { get; set; } = 2;

        /// <inheritdoc/>
        public override string Name { get; set; } = "IG-119";

        /// <inheritdoc/>
        public override string Description { get; set; } = "This grenade does almost 0 damage, however it will succ nearby players towards the center of the implosion area.";

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new DynamicSpawnPoint
                {
                    Chance = 50,
                    Location = SpawnLocation.Inside012Locker,
                },
                new DynamicSpawnPoint
                {
                    Chance = 100,
                    Location = SpawnLocation.InsideHczArmory,
                },
            },
        };

        /// <inheritdoc/>
        public override bool ExplodeOnCollision { get; set; } = true;

        /// <inheritdoc/>
        public override float FuseTime { get; set; } = 1.5f;

        /// <summary>
        /// Gets or sets the % of normal frag grenade damage this grenade will deal to those in it's radius.
        /// </summary>
        [Description("The % of normal frag grenade damage this grenade will deal to those in it's radius.")]
        public float DamageModifier { get; set; } = 0.05f;

        /// <summary>
        /// Gets or sets the amount of suction ticks each grenade will generate.
        /// </summary>
        [Description("The amount of suction ticks each grenade will generate.")]
        public int SuctionCount { get; set; } = 90;

        /// <summary>
        /// Gets or sets the distance each tick will move players towards the center.
        /// </summary>
        [Description("The distance each tick will move players towards the center.")]
        public float SuctionPerTick { get; set; } = 0.125f;

        /// <summary>
        /// Gets or sets how often each suction tick will occus. Note: Setting the tick-rate and suction-per-tick to lower numbers maks for a 'smoother' suction movement, however causes more stress on your server. Adjust accordingly.
        /// </summary>
        [Description("How often each suction tick will occus. Note: Setting the tick-rate and suction-per-tick to lower numbers maks for a 'smoother' suction movement, however causes more stress on your server. Adjust accordingly.")]
        public float SuctionTickRate { get; set; } = 0.025f;

        private List<CoroutineHandle> Coroutines { get; set; } = new List<CoroutineHandle>();

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            Map.ExplodingGrenade += OnExplodingGrenade;

            base.SubscribeEvents();
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            Map.ExplodingGrenade -= OnExplodingGrenade;

            foreach (CoroutineHandle handle in Coroutines)
                Timing.KillCoroutines(handle);

            base.UnsubscribeEvents();
        }

        private IEnumerator<float> DoSuction(Player player, Vector3 position)
        {
            Log.Debug($"{player.Nickname} Suction begin", CustomItems.Instance.Config.IsDebugEnabled);
            for (int i = 0; i < SuctionCount; i++)
            {
                Log.Debug($"{player.Nickname} suctioned?", CustomItems.Instance.Config.IsDebugEnabled);
                Vector3 newPos = Vector3.MoveTowards(player.Position, position, SuctionPerTick);
                if (!Physics.Linecast(player.Position, newPos, player.ReferenceHub.playerMovementSync.CollidableSurfaces))
                    player.Position = Vector3.MoveTowards(player.Position, position, SuctionPerTick);

                yield return Timing.WaitForSeconds(SuctionTickRate);
            }
        }

        private void OnExplodingGrenade(ExplodingGrenadeEventArgs ev)
        {
            if (Check(ev.Grenade))
            {
                Log.Debug($"{ev.Thrower.Nickname} threw an implosion grenade!", CustomItems.Instance.Config.IsDebugEnabled);
                Dictionary<Player, float> copiedList = new Dictionary<Player, float>();
                foreach (KeyValuePair<Player, float> kvp in ev.TargetToDamages)
                {
                    if (kvp.Value > 0)
                    {
                        copiedList.Add(kvp.Key, kvp.Value);
                    }
                }

                ev.TargetToDamages.Clear();
                Log.Debug("IG: List cleared.", CustomItems.Instance.Config.IsDebugEnabled);
                foreach (Player player in copiedList.Keys)
                {
                    ev.TargetToDamages.Add(player, copiedList[player] * DamageModifier);
                    Log.Debug($"{player.Nickname} starting suction", CustomItems.Instance.Config.IsDebugEnabled);

                    try
                    {
                        if (layerMask == 0)
                            layerMask = ev.Grenade.GetComponent<FragGrenade>().hurtLayerMask;

                        foreach (Transform grenadePoint in player.ReferenceHub.playerStats.grenadePoints)
                        {
                            bool line = Physics.Linecast(ev.Grenade.transform.position, grenadePoint.position, layerMask);
                            Log.Debug($"{player.Nickname} - {line}", CustomItems.Instance.Config.IsDebugEnabled);
                            if (!line)
                            {
                                Coroutines.Add(Timing.RunCoroutine(DoSuction(player, ev.Grenade.transform.position + (Vector3.up * 1.5f))));
                                break;
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        Log.Error($"{nameof(OnExplodingGrenade)} error: {exception}");
                    }
                }
            }
        }
    }
}