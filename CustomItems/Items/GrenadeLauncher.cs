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
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.CustomItems.API;
    using Exiled.CustomItems.API.Features;
    using Exiled.CustomItems.API.Spawn;
    using Exiled.Events.EventArgs;
    using InventorySystem.Items.ThrowableProjectiles;
    using MEC;
    using Mirror;
    using UnityEngine;
    using CollisionHandler = Exiled.API.Features.Components.CollisionHandler;
    using Debug = System.Diagnostics.Debug;

    /// <inheritdoc />
    public class GrenadeLauncher : CustomWeapon
    {
        private CustomGrenade loadedCustomGrenade;

        private GrenadeType loadedGrenade = GrenadeType.FragGrenade;

        /// <inheritdoc/>
        public override uint Id { get; set; } = 1;

        /// <inheritdoc/>
        public override string Name { get; set; } = "GL-119";

        /// <inheritdoc/>
        public override string Description { get; set; } = "This weapon will launch grenades in the direction you are firing, instead of bullets. Requires Frag Grenades in your inventory to reload.";

        /// <inheritdoc/>
        public override float Weight { get; set; } = 2.95f;

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
        public override byte ClipSize { get; set; } = 1;

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

                if (!(ev.Player.CurrentItem is Firearm firearm) || firearm.Ammo >= ClipSize)
                    return;

                Log.Debug($"{ev.Player.Nickname} is reloading a {Name}!", CustomItems.Instance.Config.IsDebugEnabled);

                foreach (Item item in ev.Player.Items.ToList())
                {
                    if (item.Type != ItemType.GrenadeHe && item.Type != ItemType.GrenadeFlash && item.Type != ItemType.Scp018)
                        continue;
                    if (TryGet(item, out CustomItem cItem))
                    {
                        if (IgnoreModdedGrenades)
                            continue;

                        if (cItem is CustomGrenade customGrenade)
                            loadedCustomGrenade = customGrenade;
                    }

                    ev.Player.DisableEffect(EffectType.Invisible);
                    ev.Player.ReloadWeapon();

                    firearm.Ammo = ClipSize;
                    Log.Debug($"{ev.Player.Nickname} successfully reloaded a {Name}.", CustomItems.Instance.Config.IsDebugEnabled);
                    Timing.CallDelayed(4.5f, () => { ev.Player.ReloadWeapon(); });

                    ev.Player.RemoveItem(item);
                    loadedGrenade = item.Type == ItemType.GrenadeFlash ? GrenadeType.Flashbang :
                        item.Type == ItemType.GrenadeHe ? GrenadeType.FragGrenade : GrenadeType.Scp018;

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

            if (ev.Shooter.CurrentItem is Firearm firearm)
                firearm.Ammo -= 1;

            Vector3 pos = ev.Shooter.CameraTransform.TransformPoint(new Vector3(0.0715f, 0.0225f, 0.45f));
            ThrownProjectile projectile;

            if (loadedCustomGrenade != null)
            {
                projectile = (ThrownProjectile)loadedCustomGrenade.Throw(pos, GrenadeSpeed, FuseTime, loadedCustomGrenade.Type, ev.Shooter).Base;
                loadedCustomGrenade = null;
            }
            else
            {
                switch (loadedGrenade)
                {
                    case GrenadeType.Scp018:
                        projectile = ev.Shooter.ThrowGrenade(GrenadeType.Scp018).Projectile;
                        break;
                    case GrenadeType.Flashbang:
                        projectile = ev.Shooter.ThrowGrenade(GrenadeType.Flashbang).Projectile;
                        break;
                    default:
                        projectile = ev.Shooter.ThrowGrenade(GrenadeType.FragGrenade).Projectile;
                        break;
                }
            }

            projectile.gameObject.gameObject.AddComponent<CollisionHandler>().Init(ev.Shooter.GameObject, projectile);
        }
    }
}