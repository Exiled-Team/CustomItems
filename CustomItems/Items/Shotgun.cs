using System;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using Mirror;
using RemoteAdmin;
using UnityEngine;

namespace CustomItems.Items
{
    public class Shotgun : CustomItem
    {
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

                try
                {
	                int bullets = Plugin.Singleton.Config.ShotgunSpreadCount;
	                if (ev.Shooter.CurrentItem.durability < bullets)
		                bullets = (int) ev.Shooter.CurrentItem.durability;

	                Transform cam = ev.Shooter.CameraTransform.transform;
	                WeaponManager weps = ev.Shooter.ReferenceHub.weaponManager;
	                Ray[] rays = new Ray[bullets];
	                for (int i = 0; i < rays.Length; i++)
		                rays[i] = new Ray(cam.position + cam.forward, RandomAimcone() * cam.forward);

	                RaycastHit[] hits = new RaycastHit[bullets];
	                bool[] didHit = new bool[hits.Length];
	                for (int i = 0; i < hits.Length; i++)
		                didHit[i] = Physics.Raycast(rays[i], out hits[i], 500f, weps.raycastMask);

	                bool confirm = false;
	                for (int i = 0; i < hits.Length; i++)
	                {
		                if (!didHit[i])
			                continue;

		                HitboxIdentity hitbox = hits[i].collider.GetComponent<HitboxIdentity>();
		                if (hitbox != null)
		                {
			                GameObject parent;
			                CharacterClassManager hitCcm;
			                PlayerStats stats;
			                try
			                {
				                parent = hitbox.gameObject.GetComponent<NetworkIdentity>().gameObject;
				                hitCcm = parent.GetComponent<CharacterClassManager>();
				                stats = parent.GetComponent<PlayerStats>();
			                }
			                catch (Exception e)
			                {
				                //Log.Error(e.ToString());
				                continue;
			                }

			                if (weps.GetShootPermission(hitCcm))
			                {
				                Player target = Player.Get(parent);
				                float damage = target.Role == RoleType.Scp106 ? HitHandler(hitbox) / 2f : HitHandler(hitbox);
				                
				                target.Hurt(damage, DamageTypes.FromWeaponId(ev.Shooter.ReferenceHub.weaponManager.curWeapon), ev.Shooter.Nickname, ev.Shooter.Id);
				                weps.RpcPlaceDecal(true, (sbyte) hitCcm.Classes[(int) hitCcm.CurClass].bloodType, hits[i].point + hits[i].normal * 0.01f, Quaternion.FromToRotation(Vector3.up, hits[i].normal));
				                
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

		                weps.RpcPlaceDecal(false, weps.curWeapon, hits[i].point + hits[i].normal * 0.01f,
			                Quaternion.FromToRotation(Vector3.up, hits[i].normal));
	                }

	                for (int i = 0; i < bullets; i++)
		                weps.RpcConfirmShot(confirm, weps.curWeapon);

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
			        return Plugin.Singleton.Config.ShotgunHeadDamage;
		        case HitBoxType.LEG:
			        return Plugin.Singleton.Config.ShotgunLegDamage;
		        case HitBoxType.ARM:
			        return Plugin.Singleton.Config.ShotgunArmDamage;
		        default:
			        return Plugin.Singleton.Config.ShotgunBodyDamage;
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

        public Shotgun(ItemType type, int itemId) : base(type, itemId)
        {
        }
    }
}