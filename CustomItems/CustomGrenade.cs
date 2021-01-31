using System.Collections.Generic;
using CustomItems.Items;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using Grenades;
using MEC;
using Mirror;
using UnityEngine;

namespace CustomItems
{
    public abstract class CustomGrenade : CustomItem
    {
        public CustomGrenade(ItemType type, int itemId) : base(type, itemId)
        {
        }

        public abstract override string ItemName { get; set; }
        protected abstract override string ItemDescription { get; set; }
        protected virtual bool ExplodeOnCollision { get; set; }

        protected override void LoadEvents()
        {
            Exiled.Events.Handlers.Player.ThrowingGrenade += OnThrowingGrenade;
        }
        
        protected override void UnloadEvents()
        {
            Exiled.Events.Handlers.Player.ThrowingGrenade -= OnThrowingGrenade;
        }

        protected override void OnWaitingForPlayers()
        {
            TrackedGrenades.Clear();
            base.OnWaitingForPlayers();
        }
        
        private List<GameObject> TrackedGrenades { get; } = new List<GameObject>();
        public bool CheckGrenade(GameObject grenade) => TrackedGrenades.Contains(grenade);

        public GrenadeType GetGrenadeType(ItemType type)
        {
            switch (type)
            {
                case ItemType.GrenadeFlash:
                    return GrenadeType.Flashbang;
                case ItemType.SCP018:
                    return GrenadeType.Scp018;
                default:
                    return GrenadeType.FragGrenade; ;
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

                    if (ExplodeOnCollision)
                    {
                        var grenade = SpawnGrenade(pos, ev.Player.CameraTransform.forward * 9f, 1.5f, GetGrenadeType(ItemType)).gameObject;
                        CollisionHandler collisionHandler = grenade.gameObject.AddComponent<CollisionHandler>();
                        collisionHandler.owner = ev.Player.GameObject;
                        collisionHandler.grenade = grenadeComponent;
                        TrackedGrenades.Add(grenade);
                    }
                    else
                        SpawnGrenade(pos, ev.Player.CameraTransform.forward * 9f, 3f, GetGrenadeType(ItemType));

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
    }
}