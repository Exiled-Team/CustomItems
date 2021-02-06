using CustomItems.API;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using Grenades;
using MEC;
using UnityEngine;

namespace CustomItems.Items
{
    public class Rock : CustomGrenade
    {
        public Rock(ItemType type, int itemId) : base(type, itemId)
        {
        }

        public override string Name { get; set; } = Plugin.Singleton.Config.ItemConfigs.RockCfg.Name;
        public override string Description { get; set; } = Plugin.Singleton.Config.ItemConfigs.RockCfg.Description;
        public override int SpawnLimit { get; set; } = Plugin.Singleton.Config.ItemConfigs.RockCfg.SpawnLimit;

        public int PlayerLayerMask = 1208246273;

        protected override void OnThrowingGrenade(ThrowingGrenadeEventArgs ev)
        {
            if (CheckItem(ev.Player.CurrentItem))
            {
                ev.IsAllowed = false;
                if (ev.IsSlow)
                {
                    Grenade grenadeComponent = ev.Player.GrenadeManager.availableGrenades[0].grenadeInstance.GetComponent<Grenade>();

                    Timing.CallDelayed(1f, () =>
                    {
                        Vector3 pos = ev.Player.CameraTransform.TransformPoint(grenadeComponent.throwStartPositionOffset);
                        GameObject grenade = SpawnGrenade(pos, ev.Player.CameraTransform.forward * Plugin.Singleton.Config.ItemConfigs.RockCfg.ThrowSpeed, 3f, GetGrenadeType(ItemType)).gameObject;
                        Object.Destroy(grenade.GetComponent<Scp018Grenade>());

                        CustomItems.Rock rock = grenade.AddComponent<CustomItems.Rock>();
                        rock.Owner = ev.Player.GameObject;
                        rock.Side = ev.Player.Side;
                        TrackedGrenades.Add(grenade);
                        ev.Player.RemoveItem(ev.Player.CurrentItem);
                    });
                }

                Timing.CallDelayed(1.25f, () =>
                {
                    Vector3 forward = ev.Player.CameraTransform.forward;

                    if (Physics.Linecast(ev.Player.CameraTransform.position, ev.Player.CameraTransform.position + (forward * 1.5f), out RaycastHit hit, PlayerLayerMask))
                    {
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

                        if (ev.Player.Side != target.Side || Plugin.Singleton.Config.ItemConfigs.RockCfg.FriendlyFire)
                        {
                            Log.Debug($"{ev.Player.Nickname} hit {target.Nickname}", Plugin.Singleton.Config.Debug);
                            target.Hurt(Plugin.Singleton.Config.ItemConfigs.RockCfg.HitDamage, ev.Player, DamageTypes.Wall);
                        }
                    }
                });
            }
        }
    }
}