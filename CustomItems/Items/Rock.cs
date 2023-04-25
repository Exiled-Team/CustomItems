// -----------------------------------------------------------------------
// <copyright file="Rock.cs" company="Joker119">
// Copyright (c) Joker119. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems.Items;
/*
    using System;
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
    using Footprinting;
    using InventorySystem.Items.Pickups;
    using InventorySystem.Items.ThrowableProjectiles;
    using MEC;
    using Mirror;
    using UnityEngine;
    using YamlDotNet.Serialization;
    using CollisionHandler = Exiled.API.Features.Components.CollisionHandler;

    /// <inheritdoc />
    public class Rock : CustomGrenade
    {
        private const int PlayerLayerMask = 1208246273;

        /// <inheritdoc/>
        public override uint Id { get; set; } = 6;

        /// <inheritdoc/>
        public override string Name { get; set; } = "Rock";

        /// <inheritdoc/>
        public override string Description { get; set; } = "It's a rock.";

        /// <inheritdoc/>
        public override float Weight { get; set; } = 0.1f;

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties
        {
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new DynamicSpawnPoint
                {
                    Chance = 100,
                    Location = SpawnLocationType.InsideLocker,
                },
            },
        };

        /// <summary>
        /// Gets or sets how much damage is done when hit with a rock in melee.
        /// </summary>
        [Description("How much damage is done when hit with a rock in melee.")]
        public float HitDamage { get; set; } = 10f;

        /// <summary>
        /// Gets or sets how much damage is done when hit with a thrown rock.
        /// </summary>
        [Description("How much damage is done when hit with a thrown rock.")]
        public float ThrownDamage { get; set; } = 20f;

        /// <summary>
        /// Gets or sets how fast rocks can be thrown.
        /// </summary>
        [Description("How fast rocks can be thrown.")]
        public float ThrowSpeed { get; set; } = 9f;

        /// <summary>
        /// Gets or sets a value indicating whether or not rocks will deal damage to friendly targets.
        /// </summary>
        [Description("Whether or not rocks will deal damage to friendly targets.")]
        public bool FriendlyFire { get; set; } = false;

        /// <inheritdoc/>
        [YamlIgnore]
        public override bool ExplodeOnCollision { get; set; } = false;

        /// <inheritdoc/>
        public override float FuseTime { get; set; } = int.MaxValue;

        /// <inheritdoc/>
        public override Pickup Throw(Vector3 position, float force, float fuseTime = 3f, ItemType grenadeType = ItemType.GrenadeHE, Player player = null)
        {
            if (player == null)
                player = Server.Host;

            Throwable throwable = grenadeType == ItemType.SCP018 || grenadeType == ItemType.GrenadeHE
                ? (Throwable)new ExplosiveGrenade(grenadeType)
                : new FlashGrenade(grenadeType);

            ThrownProjectile thrownProjectile = UnityEngine.Object.Instantiate(throwable.Projectile, position, throwable.Owner.CameraTransform.rotation);
            Transform transform = thrownProjectile.transform;
            PickupSyncInfo newInfo = new PickupSyncInfo()
            {
                ItemId = (global::ItemType)throwable.Type,
                Locked = !throwable.Base._repickupable,
                Serial = throwable.Serial,
                Weight = Weight,
                Position = transform.position,
                Rotation = new LowPrecisionQuaternion(transform.rotation),
            };
            thrownProjectile.NetworkInfo = newInfo;
            thrownProjectile.PreviousOwner = new Footprint(throwable.Owner.ReferenceHub);
            NetworkServer.Spawn(thrownProjectile.gameObject);
            thrownProjectile.InfoReceived(default, newInfo);
            Rigidbody component;
            if (thrownProjectile.TryGetComponent(out component))
                throwable.Base.PropelBody(component, throwable.Base.FullThrowSettings.StartTorque, force, throwable.Base.FullThrowSettings.UpwardsFactor);
            thrownProjectile.gameObject.AddComponent<Components.Rock>().Init(player.GameObject, player.Side, FriendlyFire, ThrownDamage);
            Tracked.Add(thrownProjectile);

            if (ExplodeOnCollision)
                thrownProjectile.gameObject.AddComponent<CollisionHandler>().Init(player.GameObject, (EffectGrenade)thrownProjectile);

            return Pickup.Get(thrownProjectile);
        }

        /// <summary>
        /// Handling the throwing event for this grenade.
        /// </summary>
        /// <param name="ev"><see cref="ThrowingItemEventArgs"/>.</param>
        protected override void OnThrowing(ThrowingItemEventArgs ev)
        {
            ev.IsAllowed = false;
            if (ev.RequestType == ThrowRequest.WeakThrow)
            {
                Timing.CallDelayed(1f, () =>
                {
                    try
                    {
                        Vector3 pos = ev.Player.CameraTransform.TransformPoint(new Vector3(0.0715f, 0.0225f, 0.45f));
                        Throw(pos, ThrowSpeed, 3f, Type, ev.Player);

                        ev.Player.RemoveItem(ev.Player.CurrentItem);
                    }
                    catch (Exception e)
                    {
                        Log.Error($"{e}");
                    }
                });
            }
            else if (ev.RequestType == ThrowRequest.FullForceThrow)
            {
                Timing.CallDelayed(1.25f, () =>
                {
                    foreach (Item item in ev.Player.Items)
                    {
                        if (Check(item))
                        {
                            ev.Player.CurrentItem = item;
                            break;
                        }
                    }

                    Vector3 forward = ev.Player.CameraTransform.forward;

                    if (!Physics.Linecast(ev.Player.CameraTransform.position, ev.Player.CameraTransform.position + (forward * 1.5f), out RaycastHit hit, PlayerLayerMask))
                        return;

                    Log.Debug($"{ev.Player.Nickname} linecast is true!");
                    if (hit.collider == null)
                    {
                        Log.Debug($"{ev.Player.Nickname} collider is null?");
                        return;
                    }

                    Player target = Player.Get(hit.collider.GetComponentInParent<ReferenceHub>());
                    if (target == null)
                    {
                        Log.Debug($"{ev.Player.Nickname} target null");
                        return;
                    }

                    if (ev.Player.Side == target.Side && !FriendlyFire)
                        return;

                    Log.Debug($"{ev.Player.Nickname} hit {target.Nickname}");

                    ev.Player.ShowHitMarker();
                    target.Hurt(HitDamage, ev.Player, DamageTypes.Wall);
                });
            }
        }
    }*/