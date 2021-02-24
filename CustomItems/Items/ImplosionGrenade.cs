// <copyright file="ImplosionGrenade.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CustomItems.Items
{
    using System;
    using System.Collections.Generic;
    using Exiled.API.Features;
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
        /// <summary>
        /// The layer mask used.
        /// </summary>
        private int layerMask;

        /// <inheritdoc />
        public ImplosionGrenade(ItemType type, uint itemId)
            : base(type, itemId)
        {
        }

        /// <inheritdoc/>
        public override string Name { get; } = Plugin.Singleton.Config.ItemConfigs.ImpCfg.Name;

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; protected set; } = Plugin.Singleton.Config.ItemConfigs.ImpCfg.SpawnProperties;

        /// <inheritdoc/>
        public override string Description { get; } = Plugin.Singleton.Config.ItemConfigs.ImpCfg.Description;

        /// <inheritdoc/>
        protected override bool ExplodeOnCollision { get; } = true;

        private List<CoroutineHandle> Coroutines { get; } = new List<CoroutineHandle>();

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

        private static IEnumerator<float> DoSuction(Player player, Vector3 position)
        {
            Log.Debug($"{player.Nickname} Suction begin", Plugin.Singleton.Config.Debug);
            for (int i = 0; i < Plugin.Singleton.Config.ItemConfigs.ImpCfg.SuctionCount; i++)
            {
                Log.Debug($"{player.Nickname} suctioned?", Plugin.Singleton.Config.Debug);
                player.Position = Vector3.MoveTowards(player.Position, position, Plugin.Singleton.Config.ItemConfigs.ImpCfg.SuctionPerTick);

                yield return Timing.WaitForSeconds(Plugin.Singleton.Config.ItemConfigs.ImpCfg.SuctionTickRate);
            }
        }

        private void OnExplodingGrenade(ExplodingGrenadeEventArgs ev)
        {
            if (Check(ev.Grenade))
            {
                Log.Debug($"{ev.Thrower.Nickname} threw an implosion grenade!", Plugin.Singleton.Config.Debug);
                Dictionary<Player, float> copiedList = new Dictionary<Player, float>();
                foreach (KeyValuePair<Player, float> kvp in ev.TargetToDamages)
                {
                    if (kvp.Value > 0)
                    {
                        copiedList.Add(kvp.Key, kvp.Value);
                    }
                }

                ev.TargetToDamages.Clear();
                Log.Debug("IG: List cleared.", Plugin.Singleton.Config.Debug);
                foreach (Player player in copiedList.Keys)
                {
                    ev.TargetToDamages.Add(player, copiedList[player] * Plugin.Singleton.Config.ItemConfigs.ImpCfg.DamageModifier);
                    Log.Debug($"{player.Nickname} starting suction", Plugin.Singleton.Config.Debug);

                    try
                    {
                        if (layerMask == 0)
                            layerMask = ev.Grenade.GetComponent<FragGrenade>().hurtLayerMask;

                        foreach (Transform grenadePoint in player.ReferenceHub.playerStats.grenadePoints)
                        {
                            bool line = Physics.Linecast(ev.Grenade.transform.position, grenadePoint.position, layerMask);
                            Log.Debug($"{player.Nickname} - {line}", Plugin.Singleton.Config.Debug);
                            if (!line)
                            {
                                Coroutines.Add(Timing.RunCoroutine(DoSuction(player, ev.Grenade.transform.position + (Vector3.up * 1.5f))));
                                break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error($"REEEE: {e.Message}\n{e.StackTrace}");
                    }
                }
            }
        }
    }
}