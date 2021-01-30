using CustomItems.Components;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using UnityEngine;

namespace CustomItems.Items
{
    public class Shotgun : CustomItem
    {
        public override ItemType ItemType { get; set; } = ItemType.GunMP7;
        public override string ItemName { get; set; } = "SG-119";

        public override string ItemDescription { get; set; } =
            "This modified MP-7 fires anti-personnel self-fragmenting rounds, that spreads into a cone of multiple projectiles infront of you.";

        protected override int ClipSize { get; set; } = 10;

        protected override void LoadEvents()
        {
            Exiled.Events.Handlers.Player.Shooting += OnShooting;
        }

        protected override void UnloadEvents()
        {
            Exiled.Events.Handlers.Player.Shooting += OnShooting;
        }

        private void OnShooting(ShootingEventArgs ev)
        {
            if (CheckItem(ev.Shooter.CurrentItem))
            {
                ev.IsAllowed = false;

                int bullets = Plugin.Singleton.Config.ShotgunSpreadCount;
                if (ev.Shooter.CurrentItem.durability < bullets)
                    bullets = (int)ev.Shooter.CurrentItem.durability;
                
                Ray[] rays = new Ray[bullets];
                Vector3 forward = ev.Shooter.CameraTransform.forward;
                
                for (int i = 0; i < rays.Length; i++)
                    rays[i] = new Ray(ev.Shooter.CameraTransform.position + forward, RandomAimcone() * forward);
                
                RaycastHit[] hits = new RaycastHit[bullets];
                bool[] didHit = new bool[hits.Length];
                for (int i = 0; i < hits.Length; i++)
                    didHit[i] = Physics.Raycast(rays[i], out hits[i], 500f, ev.Shooter.ReferenceHub.weaponManager.raycastMask);

                WeaponManager wepManager = ev.Shooter.ReferenceHub.weaponManager;
                bool confirm = false;

                for (int i = 0; i < hits.Length; i++)
                {
                    if (!didHit[i])
                        continue;

                    HitboxIdentity hitbox = hits[i].collider.GetComponent<HitboxIdentity>();

                    if (hitbox != null)
                    {
                        Player target = Player.Get(hits[i].collider.GetComponent<ReferenceHub>());

                        if (wepManager.GetShootPermission(target.ReferenceHub.characterClassManager))
                        {
                            float damage;
                            switch (hitbox.id)
                            {
                                case HitBoxType.HEAD:
                                    damage = Plugin.Singleton.Config.ShotgunHeadDamage;
                                    break;
                                case HitBoxType.ARM:
                                    damage = Plugin.Singleton.Config.ShotgunArmDamage;
                                    break;
                                case HitBoxType.BODY:
                                    damage = Plugin.Singleton.Config.ShotgunBodyDamage;
                                    break;
                                case HitBoxType.LEG:
                                    damage = Plugin.Singleton.Config.ShotgunLegDamage;
                                    break;
                                default:
                                    damage = 10f;
                                    break;
                            }

                            if (target.Role == RoleType.Scp106)
                                damage /= 10;
                            
                            target.Hurt(damage, DamageTypes.Mp7, ev.Shooter.Nickname, ev.Shooter.Id);
                            wepManager.RpcPlaceDecal(true, (sbyte)target.ReferenceHub.characterClassManager.Classes.SafeGet(target.Role).bloodType, hits[i].point + hits[i].normal * 0.01f, Quaternion.FromToRotation(Vector3.up, hits[i].normal));
                            confirm = true;
                        }

                        continue;
                    }

                    BreakableWindow window = hits[i].collider.GetComponent<BreakableWindow>();
                    if (window != null)
                    {
                        window.ServerDamageWindow(Plugin.Singleton.Config.ShotgunBodyDamage);
                        confirm = true;
                        
                        continue;
                    }
                    
                    wepManager.RpcPlaceDecal(false, wepManager.curWeapon, hits[i].point + hits[i].normal * 0.01f, Quaternion.FromToRotation(Vector3.up, hits[i].normal));
                }
                
                for (int i = 0; i < bullets; i++)
                    wepManager.RpcConfirmShot(confirm, wepManager.curWeapon);
                
                ev.Shooter.SetWeaponAmmo(ev.Shooter.CurrentItem, (int)ev.Shooter.CurrentItem.durability - bullets);
            }
        }
        
        private Quaternion RandomAimcone()
        {
            return Quaternion.Euler(
                UnityEngine.Random.Range(-Plugin.Singleton.Config.ShotgunAimCone, Plugin.Singleton.Config.ShotgunAimCone),
                UnityEngine.Random.Range(-Plugin.Singleton.Config.ShotgunAimCone, Plugin.Singleton.Config.ShotgunAimCone),
                UnityEngine.Random.Range(-Plugin.Singleton.Config.ShotgunAimCone, Plugin.Singleton.Config.ShotgunAimCone)
            );
        }
    }
}