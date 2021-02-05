using System.Collections.Generic;
using System.Linq;
using CustomItems.API;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using Grenades;
using MEC;
using Mirror;
using UnityEngine;

namespace CustomItems.Items
{
    public class GrenadeLauncher : CustomWeapon
    {
        public GrenadeLauncher(ItemType type, int clipSize, int itemId) : base(type, clipSize, itemId)
        {
        }

        public override string Name { get; set; } = Plugin.Singleton.Config.ItemConfigs.GlCfg.Name;
        public override Dictionary<SpawnLocation, float> SpawnLocations { get; set; } =
            Plugin.Singleton.Config.ItemConfigs.GlCfg.SpawnLocations;
        public override string Description { get; set; } = Plugin.Singleton.Config.ItemConfigs.GlCfg.Description;
        public override int SpawnLimit { get; set; } = Plugin.Singleton.Config.ItemConfigs.GlCfg.SpawnLimit;

        protected override void LoadEvents()
        {
            Exiled.Events.Handlers.Player.Shooting += OnShooting;
            base.LoadEvents();
        }

        protected override void UnloadEvents()
        {
            Exiled.Events.Handlers.Player.Shooting -= OnShooting;
            base.UnloadEvents();
        }
        
        protected override void OnReloadingWeapon(ReloadingWeaponEventArgs ev)
        {
            if (CheckItem(ev.Player.CurrentItem))
            {
                if (Plugin.Singleton.Config.ItemConfigs.GlCfg.UseGrenades)
                {
                    ev.IsAllowed = false;
                    Log.Debug($"{ev.Player.Nickname} is reloading a {Name}!", Plugin.Singleton.Config.Debug);
                    foreach (Inventory.SyncItemInfo item in ev.Player.Inventory.items.ToList())
                    {
                        if (item.id == ItemType.GrenadeFrag)
                        {
                            ev.Player.ReferenceHub.weaponManager.scp268.ServerDisable();
                            Reload(ev.Player);

                            ev.Player.Inventory.items.ModifyDuration(ev.Player.Inventory.GetItemIndex(), ClipSize);
                            Log.Debug($"{ev.Player.Nickname} successfully reloaded a {Name}.",
                                Plugin.Singleton.Config.Debug);
                            Timing.CallDelayed(4.5f, () => { Reload(ev.Player); });
                            ev.Player.RemoveItem(item);

                            break;
                        }
                    }

                    Log.Debug($"{ev.Player.Nickname} was unable to reload their {Name} - No grenades in inventory.",
                        Plugin.Singleton.Config.Debug);
                }
                else
                    base.OnReloadingWeapon(ev);
            }
        }

        private void OnShooting(ShootingEventArgs ev)
        {
            if (CheckItem(ev.Shooter.CurrentItem))
            {
                ev.IsAllowed = false;
                ev.Shooter.SetWeaponAmmo(ev.Shooter.CurrentItem, (int)ev.Shooter.CurrentItem.durability - 1);

                Vector3 velocity = (ev.Position - ev.Shooter.Position) * Plugin.Singleton.Config.ItemConfigs.GlCfg.GrenadeSpeed;
                Grenade grenadeComponent = ev.Shooter.GrenadeManager.availableGrenades[0].grenadeInstance.GetComponent<Grenade>();
                Vector3 pos = ev.Shooter.CameraTransform.TransformPoint(grenadeComponent.throwStartPositionOffset);
                var grenade = SpawnGrenade(pos, velocity, Plugin.Singleton.Config.ItemConfigs.GlCfg.FuseTime, GrenadeType.FragGrenade, ev.Shooter);
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
            Grenade component2 = GameObject.Instantiate(component.availableGrenades[(int)grenadeType].grenadeInstance).GetComponent<Grenades.Grenade>();
            component2.FullInitData(component, position, Quaternion.Euler(component2.throwStartAngle), velocity, component2.throwAngularVelocity, player == Server.Host ? Team.RIP : player.Team);
            component2.NetworkfuseTime = NetworkTime.time + (double)fusetime;
            NetworkServer.Spawn(component2.gameObject);

            return component2;
        }
    }
}