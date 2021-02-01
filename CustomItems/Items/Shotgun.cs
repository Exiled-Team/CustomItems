using System;
using CustomItems.API;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using UnityEngine;

namespace CustomItems.Items
{
    public class Shotgun : CustomWeapon
    {
        public Shotgun(ItemType type, int clipSize, int itemId) : base(type, clipSize, itemId)
        {
        }
        
        public override string ItemName { get; set; } = "SG-119";
        protected override string ItemDescription { get; set; } =
            "This modified MP-7 fires anti-personnel self-fragmenting rounds, that spreads into a cone of multiple projectiles infront of you.";

        protected override void LoadEvents()
        {
            Exiled.Events.Handlers.Player.Shooting += OnShooting;
            base.LoadEvents();
        }

        protected override void UnloadEvents()
        {
            Exiled.Events.Handlers.Player.Shooting += OnShooting;
            base.UnloadEvents();
        }

        private void OnShooting(ShootingEventArgs ev)
        {
            if (CheckItem(ev.Shooter.CurrentItem))
            {
                ev.IsAllowed = false;

                try
                {
                    int bullets = Plugin.Singleton.Config.WeaponConfigs.ShotgunCfg.SpreadCount;
                    if (ev.Shooter.CurrentItem.durability <= bullets)
                        bullets = (int) ev.Shooter.CurrentItem.durability;
                    Ray[] rays = new Ray[bullets];
                    for (int i = 0; i < rays.Length; i++)
                        rays[i] = new Ray(ev.Shooter.CameraTransform.position + ev.Shooter.CameraTransform.forward,
                            RandomAimcone() * ev.Shooter.CameraTransform.forward);

                    RaycastHit[] hits = new RaycastHit[bullets];
                    bool[] didHit = new bool[hits.Length];
                    for (int i = 0; i < hits.Length; i++)
                        didHit[i] = Physics.Raycast(rays[i], out hits[i], 500f, 1208246273);

                    WeaponManager component = ev.Shooter.ReferenceHub.weaponManager;
                    bool confirm = false;
                    for (int i = 0; i < hits.Length; i++)
                    {
                        try
                        {
                            if (!didHit[i]) continue;

                            HitboxIdentity hitbox = hits[i].collider.GetComponent<HitboxIdentity>();
                            if (hitbox != null)
                            {
                                Player target = Player.Get(hits[i].collider.GetComponentInParent<ReferenceHub>());

                                if (component.GetShootPermission(target.ReferenceHub.characterClassManager,
                                    Server.FriendlyFire))
                                {
                                    float damage = HitHandler(hitbox);
                                    if (target.Role == RoleType.Scp106)
                                        damage /= 10;

                                    float distance = Vector3.Distance(ev.Shooter.Position, target.Position);

                                    for (int f = 0; f < (int)distance; f++)
                                    {
                                        damage *= 0.9f;
                                    }

                                    target.Hurt(damage,
                                        DamageTypes.FromWeaponId(ev.Shooter.ReferenceHub.weaponManager.curWeapon),
                                        ev.Shooter.Nickname, ev.Shooter.Id);
                                    component.RpcPlaceDecal(true,
                                        (sbyte) target.ReferenceHub.characterClassManager.Classes.SafeGet(target.Role)
                                            .bloodType, hits[i].point + hits[i].normal * 0.01f,
                                        Quaternion.FromToRotation(Vector3.up, hits[i].normal));
                                    confirm = true;
                                }

                                continue;
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Error($"{e}\n{e.StackTrace}");
                        }

                        BreakableWindow window = hits[i].collider.GetComponent<BreakableWindow>();
                        if (window != null)
                        {
                            window.ServerDamageWindow(Plugin.Singleton.Config.WeaponConfigs.ShotgunCfg.BaseDamage);
                            confirm = true;
                            continue;
                        }

                        component.RpcPlaceDecal(false, component.curWeapon, hits[i].point + hits[i].normal * 0.01f,
                            Quaternion.FromToRotation(Vector3.up, hits[i].normal));
                    }

                    for (int i = 0; i < bullets; i++)
                        component.RpcConfirmShot(confirm, component.curWeapon);

                    ev.Shooter.SetWeaponAmmo(ev.Shooter.CurrentItem, (int) ev.Shooter.CurrentItem.durability - bullets);
                }
                catch (Exception e)
                {
                    Log.Error(e.ToString());
                }
            }
        }
        
        private static float HitHandler(HitboxIdentity box)
        {
	        switch (box.id)
	        {
		        case HitBoxType.HEAD:
			        return Plugin.Singleton.Config.WeaponConfigs.ShotgunCfg.BaseDamage * 1.25f;
		        case HitBoxType.LEG:
			        return Plugin.Singleton.Config.WeaponConfigs.ShotgunCfg.BaseDamage * 0.65f;
		        case HitBoxType.ARM:
			        return Plugin.Singleton.Config.WeaponConfigs.ShotgunCfg.BaseDamage * 0.55f;
		        default:
			        return Plugin.Singleton.Config.WeaponConfigs.ShotgunCfg.BaseDamage;
	        }
        }
        
        private Quaternion RandomAimcone()
        {
            return Quaternion.Euler(
                UnityEngine.Random.Range(-Plugin.Singleton.Config.WeaponConfigs.ShotgunCfg.AimconeSeverity, Plugin.Singleton.Config.WeaponConfigs.ShotgunCfg.AimconeSeverity),
                UnityEngine.Random.Range(-Plugin.Singleton.Config.WeaponConfigs.ShotgunCfg.AimconeSeverity, Plugin.Singleton.Config.WeaponConfigs.ShotgunCfg.AimconeSeverity),
                UnityEngine.Random.Range(-Plugin.Singleton.Config.WeaponConfigs.ShotgunCfg.AimconeSeverity, Plugin.Singleton.Config.WeaponConfigs.ShotgunCfg.AimconeSeverity)
            );
        }
    }
}