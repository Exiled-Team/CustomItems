// -----------------------------------------------------------------------
// <copyright file="Rock.cs" company="Galaxy199 and iopietro">
// Copyright (c) Galaxy199 and iopietro. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems.Items
{
    using System.ComponentModel;
    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;
    using Exiled.CustomItems.API.Spawn;
    using Exiled.Events.EventArgs;
    using Grenades;
    using MEC;
    using UnityEngine;

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
        public override bool ExplodeOnCollision { get; set; } = false;

        /// <inheritdoc/>
        public override float FuseTime { get; set; } = int.MaxValue;

        /// <inheritdoc/>
        protected override void OnThrowing(ThrowingGrenadeEventArgs ev)
        {
            if (ev.IsSlow)
            {
                Grenade grenadeComponent = ev.Player.GrenadeManager.availableGrenades[0].grenadeInstance.GetComponent<Grenade>();

                Timing.CallDelayed(1f, () =>
                {
                    Vector3 pos = ev.Player.CameraTransform.TransformPoint(grenadeComponent.throwStartPositionOffset);
                    GameObject grenade = Spawn(pos, ev.Player.CameraTransform.forward * ThrowSpeed, 3f, GetGrenadeType(Type)).gameObject;
                    Object.Destroy(grenade.GetComponent<Scp018Grenade>());

                    grenade.AddComponent<Components.Rock>().Init(ev.Player.GameObject, ev.Player.Side, FriendlyFire, ThrownDamage);

                    Tracked.Add(grenade);

                    ev.Player.RemoveItem(ev.Player.CurrentItem);
                });
            }

            Timing.CallDelayed(1.25f, () =>
            {
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