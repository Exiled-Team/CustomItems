// <copyright file="Rock.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CustomItems.Items
{
    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs;
    using Grenades;
    using MEC;
    using UnityEngine;

    /// <inheritdoc />
    public class Rock : CustomGrenade
    {
        /// <summary>
        /// The player hit layer mask.
        /// </summary>
        public int PlayerLayerMask = 1208246273;

        /// <inheritdoc />
        public Rock(ItemType type, uint itemId)
            : base(type, itemId)
        {
        }

        /// <inheritdoc/>
        public override string Name { get; } = Plugin.Singleton.Config.ItemConfigs.RockCfg.Name;

        /// <inheritdoc/>
        public override string Description { get; } = Plugin.Singleton.Config.ItemConfigs.RockCfg.Description;

        /// <inheritdoc/>
        protected override void OnThrowing(ThrowingGrenadeEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            ev.IsAllowed = false;
            if (ev.IsSlow)
            {
                Grenade grenadeComponent = ev.Player.GrenadeManager.availableGrenades[0].grenadeInstance.GetComponent<Grenade>();

                Timing.CallDelayed(1f, () =>
                {
                    Vector3 pos = ev.Player.CameraTransform.TransformPoint(grenadeComponent.throwStartPositionOffset);
                    GameObject grenade = Spawn(pos, ev.Player.CameraTransform.forward * Plugin.Singleton.Config.ItemConfigs.RockCfg.ThrowSpeed, 3f, GetGrenadeType(Type)).gameObject;
                    Object.Destroy(grenade.GetComponent<Scp018Grenade>());

                    CustomItems.Rock rock = grenade.AddComponent<CustomItems.Rock>();
                    rock.Owner = ev.Player.GameObject;
                    rock.Side = ev.Player.Side;
                    Tracked.Add(grenade);
                    ev.Player.RemoveItem(ev.Player.CurrentItem);
                });
            }

            Timing.CallDelayed(1.25f, () =>
            {
                Vector3 forward = ev.Player.CameraTransform.forward;

                if (!Physics.Linecast(ev.Player.CameraTransform.position, ev.Player.CameraTransform.position + (forward * 1.5f), out RaycastHit hit, PlayerLayerMask))
                    return;

                Log.Debug($"{ev.Player.Nickname} linecast is true!", Plugin.Singleton.Config.Debug);
                if (hit.collider == null)
                {
                    Log.Debug($"{ev.Player.Nickname} collider is null?", Plugin.Singleton.Config.Debug);
                    return;
                }

                Player target = Player.Get(hit.collider.GetComponentInParent<ReferenceHub>());
                if (target == null)
                {
                    Log.Debug($"{ev.Player.Nickname} target null", Plugin.Singleton.Config.Debug);
                    return;
                }

                if (ev.Player.Side == target.Side && !Plugin.Singleton.Config.ItemConfigs.RockCfg.FriendlyFire)
                    return;

                Log.Debug($"{ev.Player.Nickname} hit {target.Nickname}", Plugin.Singleton.Config.Debug);
                target.Hurt(Plugin.Singleton.Config.ItemConfigs.RockCfg.HitDamage, ev.Player, DamageTypes.Wall);
            });
        }
    }
}