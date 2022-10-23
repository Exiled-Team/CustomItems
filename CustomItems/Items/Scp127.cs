// -----------------------------------------------------------------------
// <copyright file="Scp127.cs" company="Galaxy119 and iopietro">
// Copyright (c) Galaxy119 and iopietro. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems.Items
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Spawn;
    using Exiled.CustomItems.API;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs;
    using InventorySystem.Items.Firearms;
    using MEC;
    using Firearm = Exiled.API.Features.Items.Firearm;

    /// <inheritdoc />
    [CustomItem(ItemType.GunCOM18)]
    public class Scp127 : CustomWeapon
    {
        /// <inheritdoc/>
        public override uint Id { get; set; } = 7;

        /// <inheritdoc/>
        public override string Name { get; set; } = "SCP-127";

        /// <inheritdoc/>
        public override string Description { get; set; } = "SCP-127 is a pistol that slowly regenerates it's ammo over time but cannot be reloaded normally.";

        /// <inheritdoc/>
        public override float Weight { get; set; } = 1.45f;

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; set; } = new ()
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new ()
                {
                    Chance = 100,
                    Location = SpawnLocation.Inside173Armory,
                },
            },
        };

        /// <inheritdoc/>
        public override float Damage { get; set; }

        /// <inheritdoc/>
        public override byte ClipSize { get; set; } = 25;

        /// <summary>
        /// Gets or sets how often ammo will be regenerated. Regeneration occurs at all times, however this timer is reset when the weapon is picked up or dropped.
        /// </summary>
        [Description("How often ammo will be regenerated. Regeneration occurs at all times, however this timer is reset when the weapon is picked up or dropped.")]
        public float RegenerationDelay { get; set; } = 10f;

        /// <summary>
        /// Gets or sets the amount of ammo that will be regenerated each regeneration cycle.
        /// </summary>
        [Description("The amount of ammo that will be regenerated each regeneration cycle.")]
        public byte RegenerationAmount { get; set; } = 2;

        private List<CoroutineHandle> Coroutines { get; } = new ();

        /// <inheritdoc/>
        public override void Destroy()
        {
            foreach (CoroutineHandle handle in Coroutines)
                Timing.KillCoroutines(handle);

            base.Destroy();
        }

        /// <inheritdoc/>
        protected override void OnHurting(HurtingEventArgs ev)
        {
            if (Damage > 0)
                ev.Amount *= Damage;
        }

        /// <inheritdoc/>
        protected override void OnReloading(ReloadingWeaponEventArgs ev) => ev.IsAllowed = false;

        /// <inheritdoc/>
        protected override void ShowPickedUpMessage(Player player)
        {
            Coroutines.Add(Timing.RunCoroutine(DoInventoryRegeneration(player)));

            base.ShowPickedUpMessage(player);
        }

        private IEnumerator<float> DoInventoryRegeneration(Player player)
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(RegenerationDelay);

                bool hasItem = false;

                foreach (Item item in player.Items)
                {
                    if (!Check(item) || !(item is Firearm firearm))
                        continue;
                    if (firearm.Ammo < RegenerationAmount)
                        firearm.Ammo += RegenerationAmount;
                    hasItem = true;
                }

                if (!hasItem)
                    yield break;
            }
        }

        private IEnumerator<float> DoAmmoRegeneration()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(RegenerationDelay);

                foreach (Pickup pickup in Map.Pickups)
                {
                    if (Check(pickup) && pickup.Base is FirearmPickup firearmPickup && firearmPickup.NetworkStatus.Ammo < ClipSize)
                    {
                        firearmPickup.NetworkStatus = new FirearmStatus((byte)(firearmPickup.NetworkStatus.Ammo + RegenerationAmount), firearmPickup.NetworkStatus.Flags, firearmPickup.NetworkStatus.Attachments);

                        yield return Timing.WaitForSeconds(0.5f);
                    }
                }
            }
        }
    }
}