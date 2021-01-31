using System;
using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using Grenades;
using MEC;
using Mirror;
using UnityEngine;
using Player = Exiled.API.Features.Player;
using Server = Exiled.API.Features.Server;

namespace CustomItems.Items
{
    public class ImplosionGrenade : CustomItem
    {
        public override string ItemName { get; set; } = "IG-119";
        public override string ItemDescription { get; set; } =
            "This grenade does almost 0 damage, however it will succ nearby players towards the center of the implosion area.";

        private List<CoroutineHandle> Coroutines { get; } = new List<CoroutineHandle>();

        private List<GameObject> TrackedGrenades { get; } = new List<GameObject>();
        private int layerMask = 0;

        protected override void LoadEvents()
        {
            Exiled.Events.Handlers.Map.ExplodingGrenade += OnExplodingGrenade;
            Exiled.Events.Handlers.Player.ThrowingGrenade += OnThrowingGrenade;
        }
        
        protected override void UnloadEvents()
        {
            Exiled.Events.Handlers.Map.ExplodingGrenade -= OnExplodingGrenade;
            Exiled.Events.Handlers.Player.ThrowingGrenade -= OnThrowingGrenade;

            foreach (CoroutineHandle handle in Coroutines)
                Timing.KillCoroutines(handle);
        }

        protected override void OnWaitingForPlayers()
        {
            TrackedGrenades.Clear();
            base.OnWaitingForPlayers();
        }

        private void OnExplodingGrenade(ExplodingGrenadeEventArgs ev)
        {
            if (TrackedGrenades.Contains(ev.Grenade.gameObject))
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
                    ev.TargetToDamages.Add(player, copiedList[player] * 0.1f);
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
            for (int i = 0; i < 90; i++)
            {
                Log.Debug($"{player.Nickname} suctioned?", Plugin.Singleton.Config.Debug);
                player.Position = Vector3.MoveTowards(player.Position, position, 0.125f);

                yield return Timing.WaitForSeconds(0.025f);
            }
        }

        private void OnThrowingGrenade(ThrowingGrenadeEventArgs ev)
        {
            if (CheckItem(ev.Player.CurrentItem))
            {
                ev.IsAllowed = false;
                Grenade grenadeComponent = ev.Player.GrenadeManager.availableGrenades[0].grenadeInstance.GetComponent<Grenade>();

                Timing.CallDelayed(1f, () =>
                {
                    Vector3 pos = ev.Player.CameraTransform.TransformPoint(grenadeComponent.throwStartPositionOffset);
                    var grenade = SpawnGrenade(pos, ev.Player.CameraTransform.forward * 9f, 1f).gameObject;
                    CollisionHandler collisionHandler = grenade.gameObject.AddComponent<CollisionHandler>();
                    collisionHandler.owner = ev.Player.GameObject;
                    collisionHandler.grenade = grenadeComponent;
                    TrackedGrenades.Add(grenade);

                    ev.Player.RemoveItem(ev.Player.CurrentItem);
                });
            }
        }

        public Grenades.Grenade SpawnGrenade(Vector3 position, Vector3 velocity, float fusetime = 3f, GrenadeType grenadeType = GrenadeType.FragGrenade, Player player = null)
        {
            if (player == null)
                player = Server.Host;

            GrenadeManager component = player.GrenadeManager;
            Grenade component2 = GameObject.Instantiate(component.availableGrenades[(int)grenadeType].grenadeInstance).GetComponent<Grenades.Grenade>();
            component2.FullInitData(component, position, Quaternion.Euler(component2.throwStartAngle), velocity, component2.throwAngularVelocity, player == Server.Host ? Team.RIP : player.Team);
            component2.NetworkfuseTime = NetworkTime.time + (double)fusetime;
            NetworkServer.Spawn(component2.gameObject);

            return component2;
        }

        public ImplosionGrenade(ItemType type, int itemId) : base(type, itemId)
        {
        }
    }
}