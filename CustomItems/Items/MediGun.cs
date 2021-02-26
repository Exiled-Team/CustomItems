// -----------------------------------------------------------------------
// <copyright file="MediGun.cs" company="Galaxy199 and iopietro">
// Copyright (c) Galaxy199 and iopietro. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems.Items
{
    using System.Collections.Generic;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;
    using Exiled.CustomItems.API.Spawn;
    using Exiled.Events.EventArgs;
    using UnityEngine;

    /// <inheritdoc />
    public class MediGun : CustomWeapon
    {
        private readonly Dictionary<Player, RoleType> previousRoles = new Dictionary<Player, RoleType>();

        /*/// <inheritdoc />
        public MediGun(ItemType type, uint clipSize, uint itemId)
            : base(type, itemId, clipSize)
        {
        }*/

        /// <inheritdoc/>
        public override string Name { get; } = CustomItems.Instance.Config.ItemConfigs.MediCfg.Name;

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; protected set; } = CustomItems.Instance.Config.ItemConfigs.MediCfg.SpawnProperties;

        /// <inheritdoc/>
        public override string Description { get; } = CustomItems.Instance.Config.ItemConfigs.MediCfg.Description;

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            if (CustomItems.Instance.Config.ItemConfigs.MediCfg.HealZombies)
                Exiled.Events.Handlers.Player.Dying += OnDyingMG;
            base.SubscribeEvents();
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            if (CustomItems.Instance.Config.ItemConfigs.MediCfg.HealZombies)
                Exiled.Events.Handlers.Player.Dying -= OnDyingMG;
            base.UnsubscribeEvents();
        }

        /// <inheritdoc/>
        protected override void OnWaitingForPlayers()
        {
            previousRoles.Clear();
            base.OnWaitingForPlayers();
        }

        /// <inheritdoc/>
        protected override void OnHurting(HurtingEventArgs ev)
        {
            if (Check(ev.Attacker.CurrentItem))
                ev.Amount = 0f;
        }

        /// <inheritdoc/>
        protected override void OnShooting(ShootingEventArgs ev)
        {
            if (!Check(ev.Shooter.CurrentItem))
                return;

            if (!(Player.Get(ev.Target) is Player player))
                return;

            float num3 = Vector3.Distance(ev.Shooter.CameraTransform.transform.position, ev.Target.transform.position);
            float num4 = ev.Shooter.ReferenceHub.weaponManager.weapons[ev.Shooter.ReferenceHub.weaponManager.curWeapon].damageOverDistance.Evaluate(num3);
            float damage = num4 * ev.Shooter.ReferenceHub.weaponManager.weapons[ev.Shooter.ReferenceHub.weaponManager.curWeapon].allEffects.damageMultiplier * ev.Shooter.ReferenceHub.weaponManager.overallDamagerFactor;

            if (player.Team.GetSide() == ev.Shooter.Team.GetSide())
            {
                float amount = damage * CustomItems.Instance.Config.ItemConfigs.MediCfg.HealingModifier;
                if (player.Health + amount > player.MaxHealth)
                    player.Health = player.MaxHealth;
                else
                    player.Health += amount;
            }
            else if (player.Role == RoleType.Scp0492 && CustomItems.Instance.Config.ItemConfigs.MediCfg.HealZombies)
            {
                player.MaxAdrenalineHealth = CustomItems.Instance.Config.ItemConfigs.MediCfg.ZombieHealingRequired;
                player.AdrenalineHealth += damage;

                if (player.AdrenalineHealth >= player.MaxAdrenalineHealth)
                    DoReviveZombie(player);
            }
        }

        private void OnDyingMG(DyingEventArgs ev)
        {
            if (!ev.Target.IsHuman || ev.Killer.Role != RoleType.Scp049)
                return;

            if (!previousRoles.ContainsKey(ev.Target))
                previousRoles.Add(ev.Target, RoleType.None);

            previousRoles[ev.Target] = ev.Target.Role;
        }

        private void DoReviveZombie(Player player)
        {
            if (previousRoles.ContainsKey(player))
                player.SetRole(previousRoles[player], true);
        }
    }
}