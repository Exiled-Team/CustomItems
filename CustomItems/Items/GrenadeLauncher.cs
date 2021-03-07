// -----------------------------------------------------------------------
// <copyright file="GrenadeLauncher.cs" company="Galaxy119 and iopietro">
// Copyright (c) Galaxy119 and iopietro. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems.Items
{
    using System.Collections.Generic;
    using System.ComponentModel;
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
        private CustomGrenade loadedCustomGrenade;

        /// <inheritdoc/>
        public override uint Id { get; set; } = 1;

        /// <inheritdoc/>
        public override string Name { get; set; } = "GL-119";

        /// <inheritdoc/>
        public override string Description { get; set; } = "This weapon will launch grenades in the direction you are firing, instead of bullets. Requires Frag Grenades in your inventory to reload.";

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new DynamicSpawnPoint
                {
                    Chance = 50,
                    Location = SpawnLocation.Inside049Armory,
                },
                new DynamicSpawnPoint
                {
                    Chance = 40,
                    Location = SpawnLocation.InsideHczArmory,
                },
            },
        };

        /// <inheritdoc/>
        public override Modifiers Modifiers { get; set; } = default;

        /// <inheritdoc/>
        public override float Damage { get; set; }

        /// <inheritdoc/>
        public override uint ClipSize { get; set; } = 1;

        /// <summary>
        /// Gets or sets a value indicating whether or not players will need actual frag grenades in their inventory to use as ammo. If false, the weapon's base ammo type is used instead.
        /// </summary>
        [Description("Whether or not players will need actual frag grenades in their inventory to use as ammo. If false, the weapon's base ammo type is used instead.")]
        public bool UseGrenades { get; set; } = true;

        /// <summary>
        /// Gets or sets the speed of grenades when they shoot out of the weapon.
        /// </summary>
        [Description("The speed of grenades when they shoot out of the weapon.")]
        public float GrenadeSpeed { get; set; } = 1.5f;

        /// <summary>
        /// Gets or sets the max duration of the fuse of grenades shot from the weapon. Note, these grenades will always explode immediatly when they collide with something, but this can be used with slow-moving grenades to cause mid-air explosions.
        /// </summary>
        [Description("The max duration of the fuse of grenades shot from the weapon. Note, these grenades will always explode immediatly when they collide with something, but this can be used with slow-moving grenades to cause mid-air explosions.")]
        public float FuseTime { get; set; } = 1f;

        /// <summary>
        /// Gets or sets a value indicating whether the GL should ignore modded grenades.
        /// </summary>
        [Description("Whether or not the Grenade Launcher will consider modded frag grenades as viable grenades for reloading.")]
        public bool IgnoreModdedGrenades { get; set; } = false;

        /// <inheritdoc/>
        protected override void OnReloading(ReloadingWeaponEventArgs ev)
        {
            if (UseGrenades)
            {
                ev.IsAllowed = false;

                if (ev.Player.CurrentItem.durability >= ClipSize)
                    return;

                Log.Debug($"{ev.Player.Nickname} is reloading a {Name}!", CustomItems.Instance.Config.IsDebugEnabled);

                foreach (Inventory.SyncItemInfo item in ev.Player.Inventory.items.ToList())
                {
                    if (item.id != ItemType.GrenadeFrag)
                        continue;
                    if (TryGet(item, out CustomItem cItem))
                    {
                        if (IgnoreModdedGrenades)
                            continue;

                        if (cItem is CustomGrenade customGrenade)
                            loadedCustomGrenade = customGrenade;
                    }

                    ev.Player.ReferenceHub.weaponManager.scp268.ServerDisable();
                    ev.Player.ReloadWeapon();

                    ev.Player.Inventory.items.ModifyDuration(ev.Player.Inventory.GetItemIndex(), ClipSize);
                    Log.Debug($"{ev.Player.Nickname} successfully reloaded a {Name}.", CustomItems.Instance.Config.IsDebugEnabled);
                    Timing.CallDelayed(4.5f, () => { ev.Player.ReloadWeapon(); });

                    ev.Player.RemoveItem(item);

                    break;
                }

                Log.Debug($"{ev.Player.Nickname} was unable to reload their {Name} - No grenades in inventory.", CustomItems.Instance.Config.IsDebugEnabled);
            }
            else
            {
                base.OnReloading(ev);
            }
        }

        /// <inheritdoc/>
        protected override void OnShooting(ShootingEventArgs ev)
        {
            ev.IsAllowed = false;

            ev.Shooter.SetWeaponAmmo(ev.Shooter.CurrentItem, (int)ev.Shooter.CurrentItem.durability - 1);

            Vector3 velocity = (ev.Position - ev.Shooter.Position) * GrenadeSpeed;
            Vector3 pos = ev.Shooter.CameraTransform.TransformPoint(new Vector3(0.0715f, 0.0225f, 0.45f));
            Grenade grenade;

            if (loadedCustomGrenade != null)
            {
                grenade = loadedCustomGrenade.Spawn(pos, velocity, FuseTime, loadedCustomGrenade.Type, ev.Shooter);
                loadedCustomGrenade = null;
            }
            else
            {
                grenade = SpawnGrenade(pos, velocity, FuseTime, GrenadeType.FragGrenade, ev.Shooter);
            }

            grenade.gameObject.AddComponent<CollisionHandler>().Init(ev.Shooter.GameObject, grenade);
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
        private Grenade SpawnGrenade(Vector3 position, Vector3 velocity, float fuseTime = 3f, GrenadeType grenadeType = GrenadeType.FragGrenade, Player player = null)
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