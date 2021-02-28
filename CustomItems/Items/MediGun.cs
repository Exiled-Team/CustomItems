// -----------------------------------------------------------------------
// <copyright file="MediGun.cs" company="Galaxy199 and iopietro">
// Copyright (c) Galaxy199 and iopietro. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems.Items
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.CustomItems.API;
    using Exiled.CustomItems.API.Features;
    using Exiled.CustomItems.API.Spawn;
    using Exiled.Events.EventArgs;
    using UnityEngine;

    /// <inheritdoc />
    public class MediGun : CustomWeapon
    {
        private readonly Dictionary<Player, RoleType> previousRoles = new Dictionary<Player, RoleType>();

        /// <inheritdoc/>
        public override uint Id { get; set; } = 5;

        /// <inheritdoc/>
        public override string Name { get; set; } = "MG-119";

        /// <inheritdoc/>
        public override string Description { get; set; } = "A specialized weapon that fires darts filled with a special mixture of Painkillers, Antibiotics, Antiseptics and other medicines. When fires at friendly targets, they will be healed. When fired at instances of SCP-049-2, they will be slowly converted back to human form. Does nothing when fired at anyone else.";

        /// <inheritdoc/>
        public override Modifiers Modifiers { get; set; } = default;

        /// <inheritdoc/>
        public override float Damage { get; set; }

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new DynamicSpawnPoint
                {
                    Chance = 40,
                    Location = SpawnLocation.InsideGr18,
                },
                new DynamicSpawnPoint
                {
                    Chance = 50,
                    Location = SpawnLocation.InsideGateA,
                },
                new DynamicSpawnPoint
                {
                    Chance = 50,
                    Location = SpawnLocation.InsideGateB,
                },
            },
        };

        /// <summary>
        /// Gets or sets a value indicating whether or not zombies can be 'cured' by this weapon.
        /// </summary>
        [Description("Whether or not zombies can be 'cured' by this weapon.")]
        public bool HealZombies { get; set; } = true;

        /// <summary>
        /// Gets or sets the % of damage the weapon would normally deal, that is converted into healing. 1 = 100%, 0.5 = 50%, 0.0 = 0%.
        /// </summary>
        [Description("The % of damage the weapon would normally deal, that is converted into healing. 1 = 100%, 0.5 = 50%, 0.0 = 0%")]
        public float HealingModifier { get; set; } = 1f;

        /// <summary>
        /// Gets or sets the amount of total 'healing' a zombie will require before being cured.
        /// </summary>
        [Description("The amount of total 'healing' a zombie will require before being cured.")]
        public int ZombieHealingRequired { get; set; } = 200;

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            if (HealZombies)
                Exiled.Events.Handlers.Player.Dying += OnDying;

            base.SubscribeEvents();
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            if (HealZombies)
                Exiled.Events.Handlers.Player.Dying -= OnDying;

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
                float amount = damage * HealingModifier;
                if (player.Health + amount > player.MaxHealth)
                    player.Health = player.MaxHealth;
                else
                    player.Health += amount;
            }
            else if (player.Role == RoleType.Scp0492 && HealZombies)
            {
                player.MaxAdrenalineHealth = ZombieHealingRequired;
                player.AdrenalineHealth += damage;

                if (player.AdrenalineHealth >= player.MaxAdrenalineHealth)
                    DoReviveZombie(player);
            }
        }

        private void OnDying(DyingEventArgs ev)
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