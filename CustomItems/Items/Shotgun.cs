// -----------------------------------------------------------------------
// <copyright file="Shotgun.cs" company="Galaxy199 and iopietro">
// Copyright (c) Galaxy199 and iopietro. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems.Items
{
    using System;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;
    using Exiled.CustomItems.API.Spawn;
    using Exiled.Events.EventArgs;
    using Mirror;
    using UnityEngine;
    using Random = UnityEngine.Random;
    using Server = Exiled.API.Features.Server;

    /// <inheritdoc />
    public class Shotgun : CustomWeapon
    {
        /*/// <inheritdoc />
        public Shotgun(ItemType type, uint clipSize, uint itemId)
            : base(type, itemId, clipSize)
        {
        }*/

        /// <inheritdoc/>
        public override string Name { get; } = CustomItems.Instance.Config.ItemConfigs.ShotgunCfg.Name;

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; protected set; } = CustomItems.Instance.Config.ItemConfigs.ShotgunCfg.SpawnProperties;

        /// <inheritdoc/>
        public override string Description { get; } = CustomItems.Instance.Config.ItemConfigs.ShotgunCfg.Description;

        /// <inheritdoc/>
        protected override void OnShooting(ShootingEventArgs ev)
        {
            if (!Check(ev.Shooter.CurrentItem))
                return;

            ev.IsAllowed = false;

            try
            {
                uint bullets = CustomItems.Instance.Config.ItemConfigs.ShotgunCfg.SpreadCount;
                if (ev.Shooter.CurrentItem.durability <= bullets)
                    bullets = (uint)ev.Shooter.CurrentItem.durability;
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
                            Exiled.API.Features.Player target = Exiled.API.Features.Player.Get(hitCcm._hub);

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
                                    damage *= CustomItems.Instance.Config.ItemConfigs.ShotgunCfg.DamageFalloffModifier;

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
                        window.ServerDamageWindow(CustomItems.Instance.Config.ItemConfigs.ShotgunCfg.BaseDamage);
                        confirm = true;
                        continue;
                    }

                    component.RpcPlaceDecal(false, component.curWeapon, hits[i].point + (hits[i].normal * 0.01f), Quaternion.FromToRotation(Vector3.up, hits[i].normal));
                }

                for (int i = 0; i < CustomItems.Instance.Config.ItemConfigs.ShotgunCfg.BoomCount; i++)
                    component.RpcConfirmShot(confirm, component.curWeapon);

                ev.Shooter.SetWeaponAmmo(ev.Shooter.CurrentItem, (int)ev.Shooter.CurrentItem.durability - (int)bullets);
            }
            catch (Exception e)
            {
                Log.Error($"{e}\n{e.StackTrace}");
            }
        }

        private static float HitHandler(HitboxIdentity box)
        {
            switch (box.id)
            {
                case HitBoxType.HEAD:
                    return CustomItems.Instance.Config.ItemConfigs.ShotgunCfg.BaseDamage * 1.25f;
                case HitBoxType.LEG:
                    return CustomItems.Instance.Config.ItemConfigs.ShotgunCfg.BaseDamage * 0.65f;
                case HitBoxType.ARM:
                    return CustomItems.Instance.Config.ItemConfigs.ShotgunCfg.BaseDamage * 0.55f;
                default:
                    return CustomItems.Instance.Config.ItemConfigs.ShotgunCfg.BaseDamage;
            }
        }

        private static Quaternion RandomAimCone() =>
            Quaternion.Euler(
                Random.Range(
                    -CustomItems.Instance.Config.ItemConfigs.ShotgunCfg.AimconeSeverity,
                    CustomItems.Instance.Config.ItemConfigs.ShotgunCfg.AimconeSeverity),
                Random.Range(
                    -CustomItems.Instance.Config.ItemConfigs.ShotgunCfg.AimconeSeverity,
                    CustomItems.Instance.Config.ItemConfigs.ShotgunCfg.AimconeSeverity),
                Random.Range(
                    -CustomItems.Instance.Config.ItemConfigs.ShotgunCfg.AimconeSeverity,
                    CustomItems.Instance.Config.ItemConfigs.ShotgunCfg.AimconeSeverity));
    }
}