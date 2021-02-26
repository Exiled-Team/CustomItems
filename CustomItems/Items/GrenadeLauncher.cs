// -----------------------------------------------------------------------
// <copyright file="GrenadeLauncher.cs" company="Galaxy199 and iopietro">
// Copyright (c) Galaxy199 and iopietro. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems.Items
{
    using System.Linq;
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.CustomItems.API;
    using Exiled.CustomItems.API.Components;
    using Exiled.CustomItems.API.Features;
    using Exiled.CustomItems.API.Spawn;
    using Exiled.Events.EventArgs;
    using Grenades;
    using MEC;
    using Mirror;
    using UnityEngine;

    /// <inheritdoc />
    public class GrenadeLauncher : CustomWeapon
    {
        /*/// <inheritdoc />
        public GrenadeLauncher(ItemType type, uint clipSize, uint itemId)
            : base(type, itemId, clipSize)
        {
        }*/

        /// <inheritdoc/>
        public override string Name { get; } = CustomItems.Instance.Config.ItemConfigs.GlCfg.Name;

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; protected set; } = CustomItems.Instance.Config.ItemConfigs.GlCfg.SpawnProperties;

        /// <inheritdoc/>
        public override string Description { get; } = CustomItems.Instance.Config.ItemConfigs.GlCfg.Description;

        /// <inheritdoc/>
        protected override void OnReloading(ReloadingWeaponEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            if (CustomItems.Instance.Config.ItemConfigs.GlCfg.UseGrenades)
            {
                ev.IsAllowed = false;
                Log.Debug($"{ev.Player.Nickname} is reloading a {Name}!", CustomItems.Instance.Config.Debug);
                foreach (Inventory.SyncItemInfo item in ev.Player.Inventory.items.ToList())
                {
                    if (item.id != ItemType.GrenadeFrag)
                        continue;

                    ev.Player.ReferenceHub.weaponManager.scp268.ServerDisable();
                    ev.Player.ReloadWeapon();

                    ev.Player.Inventory.items.ModifyDuration(ev.Player.Inventory.GetItemIndex(), ClipSize);
                    Log.Debug($"{ev.Player.Nickname} successfully reloaded a {Name}.", CustomItems.Instance.Config.Debug);
                    Timing.CallDelayed(4.5f, () => { ev.Player.ReloadWeapon(); });
                    ev.Player.RemoveItem(item);

                    break;
                }

                Log.Debug($"{ev.Player.Nickname} was unable to reload their {Name} - No grenades in inventory.", CustomItems.Instance.Config.Debug);
            }
            else
            {
                base.OnReloading(ev);
            }
        }

        /// <inheritdoc/>
        protected override void OnShooting(ShootingEventArgs ev)
        {
            if (!Check(ev.Shooter.CurrentItem))
                return;

            ev.IsAllowed = false;
            ev.Shooter.SetWeaponAmmo(ev.Shooter.CurrentItem, (int)ev.Shooter.CurrentItem.durability - 1);

            Vector3 velocity = (ev.Position - ev.Shooter.Position) * CustomItems.Instance.Config.ItemConfigs.GlCfg.GrenadeSpeed;
            Grenade grenadeComponent = ev.Shooter.GrenadeManager.availableGrenades[0].grenadeInstance.GetComponent<Grenade>();
            Vector3 pos = ev.Shooter.CameraTransform.TransformPoint(grenadeComponent.throwStartPositionOffset);
            Grenade grenade = SpawnGrenade(pos, velocity, CustomItems.Instance.Config.ItemConfigs.GlCfg.FuseTime, GrenadeType.FragGrenade, ev.Shooter);
            CollisionHandler collisionHandler = grenade.gameObject.AddComponent<CollisionHandler>();
            collisionHandler.Init(ev.Shooter.GameObject, grenadeComponent);
        }

        /// <summary>
        /// Spawns a live grenade object on the map.
        /// </summary>
        /// <param name="position">The <see cref="Vector3"/> to spawn the grenade at.</param>
        /// <param name="velocity">The <see cref="Vector3"/> directional velocity the grenade should move at.</param>
        /// <param name="fuseTime">The <see cref="float"/> fuse time of the grenade.</param>
        /// <param name="grenadeType">The <see cref="GrenadeType"/> of the grenade to spawn.</param>
        /// <param name="player">The <see cref="Player"/> to count as the thrower of the grenade.</param>
        /// <returns>The <see cref="Grenade"/> being spawned.</returns>
        ///
        /// I stole this from Synapse.Api.Map.SpawnGrenade -- Thanks Dimenzio, I was dreading having to find my super old version and adapting it to the new game version.
        private static Grenade SpawnGrenade(Vector3 position, Vector3 velocity, float fuseTime = 3f, GrenadeType grenadeType = GrenadeType.FragGrenade, Player player = null)
        {
            if (player == null)
                player = Server.Host;

            GrenadeManager component = player.GrenadeManager;
            Grenade component2 = GameObject.Instantiate(component.availableGrenades[(int)grenadeType].grenadeInstance).GetComponent<Grenade>();
            component2.FullInitData(component, position, Quaternion.Euler(component2.throwStartAngle), velocity, component2.throwAngularVelocity, player == Server.Host ? Team.RIP : player.Team);
            component2.NetworkfuseTime = NetworkTime.time + fuseTime;
            NetworkServer.Spawn(component2.gameObject);

            return component2;
        }
    }
}