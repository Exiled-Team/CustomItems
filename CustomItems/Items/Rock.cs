// -----------------------------------------------------------------------
// <copyright file="Rock.cs" company="Galaxy119 and iopietro">
// Copyright (c) Galaxy119 and iopietro. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems.Items
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using Exiled.API.Features;
    using Exiled.CustomItems.API.Components;
    using Exiled.CustomItems.API.Features;
    using Exiled.CustomItems.API.Spawn;
    using Exiled.Events.EventArgs;
    using Grenades;
    using MEC;
    using Mirror;
    using UnityEngine;
    using YamlDotNet.Serialization;

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
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties();

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
        public override Grenade Spawn(Vector3 position, Vector3 velocity, float fuseTime = 3, ItemType grenadeType = ItemType.GrenadeFrag, Player player = null)
        {
            if (player == null)
                player = Server.Host;

            GrenadeManager grenadeManager = player.GrenadeManager;
            GrenadeSettings settings =
                grenadeManager.availableGrenades.FirstOrDefault(g => g.inventoryID == grenadeType);

            Grenade grenade = GameObject.Instantiate(settings.grenadeInstance).GetComponent<Grenade>();

            grenade.FullInitData(grenadeManager, position, Quaternion.Euler(grenade.throwStartAngle), velocity, grenade.throwAngularVelocity, player == Server.Host ? Team.RIP : player.Team);
            grenade.NetworkfuseTime = NetworkTime.time + fuseTime;

            Tracked.Add(grenade.gameObject);

            GameObject grenadeObject = grenade.gameObject;
            UnityEngine.Object.Destroy(grenadeObject.GetComponent<Scp018Grenade>());
            grenadeObject.AddComponent<Components.Rock>().Init(player.GameObject, player.Side, FriendlyFire, ThrownDamage);
            NetworkServer.Spawn(grenadeObject);

            if (ExplodeOnCollision)
                grenade.gameObject.AddComponent<CollisionHandler>().Init(player.GameObject, grenade);

            return grenade;
        }

        /// <summary>
        /// Handling the throwing event for this grenade.
        /// </summary>
        /// <param name="ev"><see cref="ThrowingGrenadeEventArgs"/>.</param>
        protected override void OnThrowing(ThrowingGrenadeEventArgs ev)
        {
            ev.IsAllowed = false;
            if (ev.IsSlow)
            {
                Timing.CallDelayed(1f, () =>
                {
                    try
                    {
                        Vector3 pos = ev.Player.CameraTransform.TransformPoint(new Vector3(0.0715f, 0.0225f, 0.45f));
                        Spawn(pos, ev.Player.CameraTransform.forward * ThrowSpeed, 3f, Type, ev.Player);

                        ev.Player.RemoveItem(ev.Player.CurrentItem);
                    }
                    catch (Exception e)
                    {
                        Log.Error($"{e}");
                    }
                });
            }
            else
            {
                Timing.CallDelayed(1.25f, () =>
                {
                    foreach (Inventory.SyncItemInfo item in ev.Player.Items)
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

                    Log.Debug($"{ev.Player.Nickname} linecast is true!", CustomItems.Instance.Config.IsDebugEnabled);
                    if (hit.collider == null)
                    {
                        Log.Debug($"{ev.Player.Nickname} collider is null?", CustomItems.Instance.Config.IsDebugEnabled);
                        return;
                    }

                    Player target = Player.Get(hit.collider.GetComponentInParent<ReferenceHub>());
                    if (target == null)
                    {
                        Log.Debug($"{ev.Player.Nickname} target null", CustomItems.Instance.Config.IsDebugEnabled);
                        return;
                    }

                    if (ev.Player.Side == target.Side && !FriendlyFire)
                        return;

                    Log.Debug($"{ev.Player.Nickname} hit {target.Nickname}", CustomItems.Instance.Config.IsDebugEnabled);

                    target.Hurt(HitDamage, ev.Player, DamageTypes.Wall);
                });
            }
        }
    }
}