// <copyright file="Shotgun.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CustomItems.Items
{
    using System;
    using System.Collections.Generic;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.CustomItems.API;
    using Exiled.Events.EventArgs;
    using Mirror;
    using UnityEngine;

    /// <inheritdoc />
    public class Shotgun : CustomWeapon
    {
        /// <inheritdoc />
        public Shotgun(ItemType type, int clipSize, int itemId)
            : base(type, clipSize, itemId)
        {
        }

        /// <inheritdoc/>
        public override string Name { get; set; } = Plugin.Singleton.Config.ItemConfigs.ShotgunCfg.Name;

        /// <inheritdoc/>
        public override Dictionary<SpawnLocation, float> SpawnLocations { get; set; } = Plugin.Singleton.Config.ItemConfigs.ShotgunCfg.SpawnLocations;

        /// <inheritdoc/>
        public override string Description { get; set; } = Plugin.Singleton.Config.ItemConfigs.ShotgunCfg.Description;

        /// <inheritdoc/>
        public override int SpawnLimit { get; set; } = Plugin.Singleton.Config.ItemConfigs.ShotgunCfg.SpawnLimit;

        /// <inheritdoc/>
        protected override void LoadEvents()
        {
            Exiled.Events.Handlers.Player.Shooting += OnShooting;
            base.LoadEvents();
        }

        /// <inheritdoc/>
        protected override void UnloadEvents()
        {
            Exiled.Events.Handlers.Player.Shooting -= OnShooting;
            base.UnloadEvents();
        }

        private static float HitHandler(HitboxIdentity box)
        {
            switch (box.id)
            {
                case HitBoxType.HEAD:
                    return Plugin.Singleton.Config.ItemConfigs.ShotgunCfg.BaseDamage * 1.25f;
                case HitBoxType.LEG:
                    return Plugin.Singleton.Config.ItemConfigs.ShotgunCfg.BaseDamage * 0.65f;
                case HitBoxType.ARM:
                    return Plugin.Singleton.Config.ItemConfigs.ShotgunCfg.BaseDamage * 0.55f;
                default:
                    return Plugin.Singleton.Config.ItemConfigs.ShotgunCfg.BaseDamage;
            }
        }

        private static Quaternion RandomAimCone() =>
            Quaternion.Euler(
                UnityEngine.Random.Range(
                    -Plugin.Singleton.Config.ItemConfigs.ShotgunCfg.AimconeSeverity,
                    Plugin.Singleton.Config.ItemConfigs.ShotgunCfg.AimconeSeverity),
                UnityEngine.Random.Range(
                    -Plugin.Singleton.Config.ItemConfigs.ShotgunCfg.AimconeSeverity,
                    Plugin.Singleton.Config.ItemConfigs.ShotgunCfg.AimconeSeverity),
                UnityEngine.Random.Range(
                    -Plugin.Singleton.Config.ItemConfigs.ShotgunCfg.AimconeSeverity,
                    Plugin.Singleton.Config.ItemConfigs.ShotgunCfg.AimconeSeverity));

        private void OnShooting(ShootingEventArgs ev)
        {
            if (!CheckItem(ev.Shooter.CurrentItem))
                return;

            ev.IsAllowed = false;

            try
            {
                int bullets = Plugin.Singleton.Config.ItemConfigs.ShotgunCfg.SpreadCount;
                if (ev.Shooter.CurrentItem.durability <= bullets)
                    bullets = (int)ev.Shooter.CurrentItem.durability;
                Ray[] rays = new Ray[bullets];
                for (int i = 0; i < rays.Length; i++)
                {
                    Vector3 forward = ev.Shooter.CameraTransform.forward;
                    rays[i] = new Ray(ev.Shooter.CameraTransform.position + forward, RandomAimCone() * forward);
                }

                RaycastHit[] hits = new RaycastHit[bullets];
                bool[] didHit = new bool[hits.Length];
                for (int i = 0; i < hits.Length; i++)
                    didHit[i] = Physics.Raycast(rays[i], out hits[i], 500f, ev.Shooter.ReferenceHub.weaponManager.raycastMask);

                WeaponManager component = ev.Shooter.ReferenceHub.weaponManager;
                bool confirm = false;
                for (int i = 0; i < hits.Length; i++)
                {
                    try
                    {
                        if (!didHit[i])
                        {
                            continue;
                        }

                        HitboxIdentity hitBox = hits[i].collider.GetComponent<HitboxIdentity>();
                        if (hitBox != null)
                        {
                            var parent = hits[i].collider.GetComponentInParent<NetworkIdentity>().gameObject;
                            var hitCcm = parent.GetComponent<CharacterClassManager>();
                            Player target = Player.Get(hitCcm._hub);

                            if (target == null)
                            {
                                continue;
                            }

                            if (component.GetShootPermission(target.ReferenceHub.characterClassManager, Server.FriendlyFire))
                            {
                                float damage = HitHandler(hitBox);
                                if (target.Role == RoleType.Scp106)
                                {
                                    damage /= 10;
                                }

                                float distance = Vector3.Distance(ev.Shooter.Position, target.Position);

                                for (int f = 0; f < (int)distance; f++)
                                    damage *= Plugin.Singleton.Config.ItemConfigs.ShotgunCfg.DamageFalloffModifier;

                                target.Hurt(damage, DamageTypes.Wall, ev.Shooter.Nickname, ev.Shooter.Id);
                                component.RpcPlaceDecal(true, (sbyte)target.ReferenceHub.characterClassManager.Classes.SafeGet(target.Role).bloodType, hits[i].point + (hits[i].normal * 0.01f), Quaternion.FromToRotation(Vector3.up, hits[i].normal));
                                confirm = true;
                            }

                            continue;
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error($"{e} - {e.Message}\n{e.StackTrace}");
                    }

                    BreakableWindow window = hits[i].collider.GetComponent<BreakableWindow>();
                    if (window != null)
                    {
                        window.ServerDamageWindow(Plugin.Singleton.Config.ItemConfigs.ShotgunCfg.BaseDamage);
                        confirm = true;
                        continue;
                    }

                    component.RpcPlaceDecal(false, component.curWeapon, hits[i].point + (hits[i].normal * 0.01f), Quaternion.FromToRotation(Vector3.up, hits[i].normal));
                }

                for (int i = 0; i < Plugin.Singleton.Config.ItemConfigs.ShotgunCfg.BoomCount; i++)
                    component.RpcConfirmShot(confirm, component.curWeapon);

                ev.Shooter.SetWeaponAmmo(ev.Shooter.CurrentItem, (int)ev.Shooter.CurrentItem.durability - bullets);
            }
            catch (Exception e)
            {
                Log.Error($"{e}\n{e.StackTrace}");
            }
        }
    }
}