// <copyright file="MediGun.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CustomItems.Items
{
    using System.Collections.Generic;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.CustomItems.API;
    using Exiled.Events.EventArgs;
    using UnityEngine;

    /// <inheritdoc />
    public class MediGun : CustomWeapon
    {
        private readonly Dictionary<Player, RoleType> previousRoles = new Dictionary<Player, RoleType>();

        /// <inheritdoc />
        public MediGun(ItemType type, int clipSize, int itemId)
            : base(type, clipSize, itemId)
        {
        }

        /// <inheritdoc/>
        public override string Name { get; set; } = Plugin.Singleton.Config.ItemConfigs.MediCfg.Name;

        /// <inheritdoc/>
        public override Dictionary<SpawnLocation, float> SpawnLocations { get; set; } = Plugin.Singleton.Config.ItemConfigs.MediCfg.SpawnLocations;

        /// <inheritdoc/>
        public override string Description { get; set; } = Plugin.Singleton.Config.ItemConfigs.MediCfg.Description;

        /// <inheritdoc/>
        public override int SpawnLimit { get; set; } = Plugin.Singleton.Config.ItemConfigs.MediCfg.SpawnLimit;

        /// <inheritdoc/>
        protected override void LoadEvents()
        {
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            Exiled.Events.Handlers.Player.Shooting += OnShooting;
            if (Plugin.Singleton.Config.ItemConfigs.MediCfg.HealZombies)
                Exiled.Events.Handlers.Player.Dying += OnDyingMG;
            base.LoadEvents();
        }

        /// <inheritdoc/>
        protected override void UnloadEvents()
        {
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
            Exiled.Events.Handlers.Player.Shooting -= OnShooting;
            if (Plugin.Singleton.Config.ItemConfigs.MediCfg.HealZombies)
                Exiled.Events.Handlers.Player.Dying -= OnDyingMG;
            base.UnloadEvents();
        }

        /// <inheritdoc/>
        protected override void OnWaitingForPlayers()
        {
            previousRoles.Clear();
            base.OnWaitingForPlayers();
        }

        private void OnHurting(HurtingEventArgs ev)
        {
            if (CheckItem(ev.Attacker.CurrentItem))
                ev.Amount = 0f;
        }

        private void OnDyingMG(DyingEventArgs ev)
        {
            if (!ev.Target.IsHuman || ev.Killer.Role != RoleType.Scp049)
                return;

            if (!previousRoles.ContainsKey(ev.Target))
                previousRoles.Add(ev.Target, RoleType.None);

            previousRoles[ev.Target] = ev.Target.Role;
        }

        private void OnShooting(ShootingEventArgs ev)
        {
            if (!CheckItem(ev.Shooter.CurrentItem))
                return;

            if (!(Player.Get(ev.Target) is Player player))
                return;

            float num3 = Vector3.Distance(ev.Shooter.CameraTransform.transform.position, ev.Target.transform.position);
            float num4 = ev.Shooter.ReferenceHub.weaponManager.weapons[ev.Shooter.ReferenceHub.weaponManager.curWeapon].damageOverDistance.Evaluate(num3);
            float damage = num4 * ev.Shooter.ReferenceHub.weaponManager.weapons[ev.Shooter.ReferenceHub.weaponManager.curWeapon].allEffects.damageMultiplier * ev.Shooter.ReferenceHub.weaponManager.overallDamagerFactor;

            if (player.Team.GetSide() == ev.Shooter.Team.GetSide())
            {
                float amount = damage * Plugin.Singleton.Config.ItemConfigs.MediCfg.HealingModifier;
                if (player.Health + amount > player.MaxHealth)
                    player.Health = player.MaxHealth;
                else
                    player.Health += amount;
            }
            else if (player.Role == RoleType.Scp0492 && Plugin.Singleton.Config.ItemConfigs.MediCfg.HealZombies)
            {
                player.MaxAdrenalineHealth = Plugin.Singleton.Config.ItemConfigs.MediCfg.ZombieHealingRequired;
                player.AdrenalineHealth += damage;

                if (player.AdrenalineHealth >= player.MaxAdrenalineHealth)
                    DoReviveZombie(player);
            }
        }

        private void DoReviveZombie(Player player)
        {
            if (previousRoles.ContainsKey(player))
                player.SetRole(previousRoles[player], true);
        }
    }
}