// -----------------------------------------------------------------------
// <copyright file="Shotgun.cs" company="Galaxy119 and iopietro">
// Copyright (c) Galaxy119 and iopietro. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems.Items
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.CustomItems.API;
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
        /// <inheritdoc/>
        public override uint Id { get; set; } = 9;

        /// <inheritdoc/>
        public override string Name { get; set; } = "SG-119";

        /// <inheritdoc/>
        public override string Description { get; set; } = "This modified MP-7 fires anti-personnel self-fragmenting rounds, that spreads into a cone of multiple projectiles in front of you.";

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new DynamicSpawnPoint
                {
                    Chance = 60,
                    Location = SpawnLocation.InsideLczArmory,
                },
            },
        };

        /// <inheritdoc/>
        public override Modifiers Modifiers { get; set; } = default;

        /// <inheritdoc/>
        public override float Damage { get; set; } = 13.5f;

        /// <inheritdoc/>
        public override uint ClipSize { get; set; } = 8;

        /// <summary>
        /// Gets or sets the amount of pellets fired at once. This amount of ammo will also be consumed from the weapons current clip. If the clip is lower than this amount, the amount in the clip is used instead.
        /// </summary>
        [Description("The amount of pellets fired at once. This amount of ammo will also be consumed from the weapons current clip. If the clip is lower than this amount, the amount in the clip is used instead.")]
        public uint SpreadCount { get; set; } = 12;

        /// <summary>
        /// Gets or sets the 'randomness' factor used for the aimcone. Higher numbers = wider aimcone, which means less accuracy.
        /// </summary>
        [Description("The 'randomness' factor used for the aimcone. Higher numbers = wider aimcone, which means less accuracy.")]
        public int AimconeSeverity { get; set; } = 5;

        /// <summary>
        /// Gets or sets the number of shots that are registered by other clients. (Used for shooting sound volume, setting this higher than default is not recommended.
        /// </summary>
        [Description("The number of shots that are registered by other clients. (Used for shooting sound volume, setting this higher than default is not recommended.")]
        public int BoomCount { get; set; } = 5;

        /// <summary>
        /// Gets or sets how much damage is 'carried over', damage is reduced for every 1f away from the shooter the target is. By default (0.9), every 1f further away, the damage each pellet can deal is reduced by 10%.
        /// </summary>
        [Description("Damage is reduced for every 1f away from the shooter the target is. This number signifies how much damage is 'carried over'. By default (0.9), every 1f further away, the damage each pellet can deal is reduced by 10%.")]
        public float DamageFalloffModifier { get; set; } = 0.9f;

        private Quaternion RandomAimCone
        {
            get => Quaternion.Euler(Random.Range(-AimconeSeverity, AimconeSeverity), Random.Range(-AimconeSeverity, AimconeSeverity), Random.Range(-AimconeSeverity, AimconeSeverity));
        }

        /// <inheritdoc/>
        protected override void OnShooting(ShootingEventArgs ev)
        {
            ev.IsAllowed = false;

            try
            {
                uint bullets = SpreadCount;
                if (ev.Shooter.CurrentItem.durability <= bullets)
                    bullets = (uint)ev.Shooter.CurrentItem.durability;
                Ray[] rays = new Ray[bullets];
                for (int i = 0; i < rays.Length; i++)
                {
                    Vector3 forward = ev.Shooter.CameraTransform.forward;
                    rays[i] = new Ray(ev.Shooter.CameraTransform.position + forward, RandomAimCone * forward);
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

                                for (int j = 0; j < (int)distance; j++)
                                    damage *= DamageFalloffModifier;

                                target.Hurt(damage, DamageTypes.Wall, ev.Shooter.Nickname, ev.Shooter.Id);
                                component.RpcPlaceDecal(true, (sbyte)target.ReferenceHub.characterClassManager.Classes.SafeGet(target.Role).bloodType, hits[i].point + (hits[i].normal * 0.01f), Quaternion.FromToRotation(Vector3.up, hits[i].normal));
                                confirm = true;
                            }

                            continue;
                        }
                    }
                    catch (Exception exception)
                    {
                        Log.Error($"{nameof(OnShooting)} error: {exception}");
                    }

                    BreakableWindow window = hits[i].collider.GetComponent<BreakableWindow>();
                    if (window != null)
                    {
                        window.ServerDamageWindow(Damage);
                        confirm = true;
                        continue;
                    }

                    component.RpcPlaceDecal(false, component.curWeapon, hits[i].point + (hits[i].normal * 0.01f), Quaternion.FromToRotation(Vector3.up, hits[i].normal));
                }

                for (int i = 0; i < BoomCount; i++)
                    component.RpcConfirmShot(confirm, component.curWeapon);

                ev.Shooter.SetWeaponAmmo(ev.Shooter.CurrentItem, (int)ev.Shooter.CurrentItem.durability - (int)bullets);
            }
            catch (Exception exception)
            {
                Log.Error($"{nameof(OnShooting)} error: {exception}");
            }
        }

        private float HitHandler(HitboxIdentity box)
        {
            switch (box.id)
            {
                case HitBoxType.HEAD:
                    return Damage * 1.25f;
                case HitBoxType.LEG:
                    return Damage * 0.65f;
                case HitBoxType.ARM:
                    return Damage * 0.55f;
                default:
                    return Damage;
            }
        }
    }
}