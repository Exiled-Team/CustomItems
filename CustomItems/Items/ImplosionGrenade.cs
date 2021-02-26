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

        /*/// <inheritdoc />
        public ImplosionGrenade(ItemType type, uint itemId)
            : base(type, itemId)
        {
        }*/

        /// <inheritdoc/>
        public override string Name { get; } = CustomItems.Instance.Config.ItemConfigs.ImpCfg.Name;

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; protected set; } = CustomItems.Instance.Config.ItemConfigs.ImpCfg.SpawnProperties;

        /// <inheritdoc/>
        public override string Description { get; } = CustomItems.Instance.Config.ItemConfigs.ImpCfg.Description;

        /// <inheritdoc/>
        public override bool ExplodeOnCollision { get; protected set; } = true;

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
            Log.Debug($"{player.Nickname} Suction begin", CustomItems.Instance.Config.Debug);
            for (int i = 0; i < CustomItems.Instance.Config.ItemConfigs.ImpCfg.SuctionCount; i++)
            {
                Log.Debug($"{player.Nickname} suctioned?", CustomItems.Instance.Config.Debug);
                player.Position = Vector3.MoveTowards(player.Position, position, CustomItems.Instance.Config.ItemConfigs.ImpCfg.SuctionPerTick);

                yield return Timing.WaitForSeconds(CustomItems.Instance.Config.ItemConfigs.ImpCfg.SuctionTickRate);
            }
        }

        private void OnExplodingGrenade(ExplodingGrenadeEventArgs ev)
        {
            if (Check(ev.Grenade))
            {
                Log.Debug($"{ev.Thrower.Nickname} threw an implosion grenade!", CustomItems.Instance.Config.Debug);
                Dictionary<Player, float> copiedList = new Dictionary<Player, float>();
                foreach (KeyValuePair<Player, float> kvp in ev.TargetToDamages)
                {
                    if (kvp.Value > 0)
                    {
                        copiedList.Add(kvp.Key, kvp.Value);
                    }
                }

                ev.TargetToDamages.Clear();
                Log.Debug("IG: List cleared.", CustomItems.Instance.Config.Debug);
                foreach (Player player in copiedList.Keys)
                {
                    ev.TargetToDamages.Add(player, copiedList[player] * CustomItems.Instance.Config.ItemConfigs.ImpCfg.DamageModifier);
                    Log.Debug($"{player.Nickname} starting suction", CustomItems.Instance.Config.Debug);

                    try
                    {
                        if (layerMask == 0)
                            layerMask = ev.Grenade.GetComponent<FragGrenade>().hurtLayerMask;

                        foreach (Transform grenadePoint in player.ReferenceHub.playerStats.grenadePoints)
                        {
                            bool line = Physics.Linecast(ev.Grenade.transform.position, grenadePoint.position, layerMask);
                            Log.Debug($"{player.Nickname} - {line}", CustomItems.Instance.Config.Debug);
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