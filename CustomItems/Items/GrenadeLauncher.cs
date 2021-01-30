using System;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using Grenades;
using Mirror;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CustomItems.Components
{
    public class GrenadeLauncher : CustomItem
    {
        public override int ClipSize { get; set; } = 1;
        public override ItemType ItemType { get; set; } = ItemType.GunLogicer;
        public override string ItemName { get; set; } = "RL-119";

        public override string ItemDescription { get; set; } =
            "This weapon will launch grenades in the direction you are firing, instead of bullets.";

        public override void LoadEvents()
        {
            Exiled.Events.Handlers.Player.Shooting += OnShooting;
        }

        public override void UnloadEvents()
        {
            Exiled.Events.Handlers.Player.Shooting -= OnShooting;
        }
        
        private void OnShooting(ShootingEventArgs ev)
        {
            if (CheckItem(ev.Shooter.CurrentItem))
            {
                ev.IsAllowed = false;
                ev.Shooter.SetWeaponAmmo(ev.Shooter.CurrentItem, (int)ev.Shooter.CurrentItem.durability - 1);

                Vector3 velocity = (ev.Position - ev.Shooter.Position) * 1f;
                Grenade grenadeComponent = ev.Shooter.GrenadeManager.availableGrenades[0].grenadeInstance.GetComponent<Grenade>();
                Vector3 pos = ev.Shooter.CameraTransform.TransformPoint(grenadeComponent.throwStartPositionOffset);
                var grenade = SpawnGrenade(pos, velocity, 1f);
                CollisionHandler collisionHandler = grenade.gameObject.AddComponent<CollisionHandler>();
                collisionHandler.owner = ev.Shooter.GameObject;
                collisionHandler.grenade = grenadeComponent;
            }
        }
        
        
        //I stole this from Synapse.Api.Map.SpawnGrenade -- Thanks Dimenzio, I was dreading having to find my super old version and adapting it to the new game version.
        public Grenades.Grenade SpawnGrenade(Vector3 position, Vector3 velocity, float fusetime = 3f, GrenadeType grenadeType = GrenadeType.FragGrenade, Player player = null)
        {
            if (player == null)
                player = Server.Host;

            GrenadeManager component = player.GrenadeManager;
            Grenade component2 = Instantiate(component.availableGrenades[(int)grenadeType].grenadeInstance).GetComponent<Grenades.Grenade>();
            component2.FullInitData(component, position, Quaternion.Euler(component2.throwStartAngle), velocity, component2.throwAngularVelocity, player == Server.Host ? Team.RIP : player.Team);
            component2.NetworkfuseTime = NetworkTime.time + (double)fusetime;
            NetworkServer.Spawn(component2.gameObject);

            return component2;
        }
    }

    public class CollisionHandler : MonoBehaviour
    {
        public GameObject owner;
        public Grenade grenade;

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject == owner || collision.gameObject.GetComponent<Grenade>() != null)
                return;
            grenade.NetworkfuseTime = 0.1f;
        }
    }
}