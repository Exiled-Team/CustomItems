using System;
using System.Collections.Generic;
using CustomItems.API;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using Grenades;
using MEC;
using UnityEngine;
using Player = Exiled.API.Features.Player;

namespace CustomItems.Items
{
    public class ImplosionGrenade : CustomGrenade
    {
        public ImplosionGrenade(ItemType type, int itemId) : base(type, itemId)
        {
        }
        
        public override string Name { get; set; } = "IG-119";
        public override Dictionary<SpawnLocation, float> SpawnLocations { get; set; } =
            Plugin.Singleton.Config.ItemConfigs.ImpCfg.SpawnLocations;
        protected override string Description { get; set; } =
            "This grenade does almost 0 damage, however it will succ nearby players towards the center of the implosion area.";

        protected override bool ExplodeOnCollision { get; set; } = true;

        private List<CoroutineHandle> Coroutines { get; } = new List<CoroutineHandle>();
        private int layerMask = 0;

        protected override void LoadEvents()
        {
            Exiled.Events.Handlers.Map.ExplodingGrenade += OnExplodingGrenade;
            base.LoadEvents();
        }
        
        protected override void UnloadEvents()
        {
            Exiled.Events.Handlers.Map.ExplodingGrenade -= OnExplodingGrenade;

            foreach (CoroutineHandle handle in Coroutines)
                Timing.KillCoroutines(handle);
            base.UnloadEvents();
        }

        private void OnExplodingGrenade(ExplodingGrenadeEventArgs ev)
        {
            if (CheckGrenade(ev.Grenade))
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
                Log.Debug($"IG: List cleared.", Plugin.Singleton.Config.Debug);
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

        IEnumerator<float> DoSuction(Player player, Vector3 position)
        {
            Log.Debug($"{player.Nickname} Suction begin", Plugin.Singleton.Config.Debug);
            for (int i = 0; i < Plugin.Singleton.Config.ItemConfigs.ImpCfg.SuctionCount; i++)
            {
                Log.Debug($"{player.Nickname} suctioned?", Plugin.Singleton.Config.Debug);
                player.Position = Vector3.MoveTowards(player.Position, position, Plugin.Singleton.Config.ItemConfigs.ImpCfg.SuctionPerTick);
                
                yield return Timing.WaitForSeconds(Plugin.Singleton.Config.ItemConfigs.ImpCfg.SuctionTickRate);
            }
        }
    }
}