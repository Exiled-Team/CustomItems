using System.Collections.Generic;
using CustomItems.API;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using UnityEngine;

namespace CustomItems.Items
{
    public class MediGun : CustomWeapon
    {
        public MediGun(ItemType type, int clipSize, int itemId) : base(type, clipSize, itemId)
        {
        }

        public override string Name { get; set; } = "MG-119";
        public override Dictionary<SpawnLocation, float> SpawnLocations { get; set; } =
            Plugin.Singleton.Config.ItemConfigs.MediCfg.SpawnLocations;
        protected override string Description { get; set; } =
            "A specialized weapon that fires darts filled with a special mixture of Painkillers, Antibiotics, Antiseptics and other medicines. When fires at friendly targets, they will be healed. When fired at instances of SCP-049-2, they will be slowly converted back to human form. Does nothing when fired at anyone else.";

        protected override void LoadEvents()
        {
            Exiled.Events.Handlers.Player.Shooting += OnShooting;
            if (Plugin.Singleton.Config.ItemConfigs.MediCfg.HealZombies)
                Exiled.Events.Handlers.Scp049.FinishingRecall += OnFinishingRecall;
            base.LoadEvents();
        }

        protected override void UnloadEvents()
        {
            Exiled.Events.Handlers.Player.Shooting -= OnShooting;
            if (Plugin.Singleton.Config.ItemConfigs.MediCfg.HealZombies)
                Exiled.Events.Handlers.Scp049.FinishingRecall -= OnFinishingRecall;
            base.UnloadEvents();
        }

        protected override void OnWaitingForPlayers()
        {
            previousRoles.Clear();
            base.OnWaitingForPlayers();
        }

        Dictionary<Player, RoleType> previousRoles = new Dictionary<Player, RoleType>();

        private void OnShooting(ShootingEventArgs ev)
        {
            if (CheckItem(ev.Shooter.CurrentItem))
            {
                if (Player.Get(ev.Target) is Player player)
                {
                    float num3 = Vector3.Distance(ev.Shooter.CameraTransform.transform.position, ev.Target.transform.position);
                    float num4 = ev.Shooter.ReferenceHub.weaponManager.weapons[ev.Shooter.ReferenceHub.weaponManager.curWeapon].damageOverDistance.Evaluate(num3);
                    float damage = num4 * ev.Shooter.ReferenceHub.weaponManager.weapons[ev.Shooter.ReferenceHub.weaponManager.curWeapon].allEffects.damageMultiplier * ev.Shooter.ReferenceHub.weaponManager.overallDamagerFactor;
                    
                    if (player.Team.GetSide() == ev.Shooter.Team.GetSide())
                        player.Health += (damage * Plugin.Singleton.Config.ItemConfigs.MediCfg.HealingModifier);
                    else if (player.Role == RoleType.Scp0492 && Plugin.Singleton.Config.ItemConfigs.MediCfg.HealZombies)
                    {
                        player.MaxAdrenalineHealth = Plugin.Singleton.Config.ItemConfigs.MediCfg.ZombieHealingRequired;
                        player.AdrenalineHealth += damage;

                        if (player.AdrenalineHealth >= player.MaxAdrenalineHealth)
                            DoReviveZombie(player);
                    }
                }
            }
        }
        
        private void OnFinishingRecall(FinishingRecallEventArgs ev)
        {
            if (!previousRoles.ContainsKey(ev.Target))
                previousRoles.Add(ev.Target, ev.Target.Role);
            else
                previousRoles[ev.Target] = ev.Target.Role;
        }

        private void DoReviveZombie(Player player)
        {
            if (previousRoles.ContainsKey(player))
                player.SetRole(previousRoles[player], true, false);
        }
    }
}