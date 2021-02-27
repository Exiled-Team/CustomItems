// -----------------------------------------------------------------------
// <copyright file="Scp127.cs" company="Galaxy199 and iopietro">
// Copyright (c) Galaxy199 and iopietro. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems.Items
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using Exiled.API.Features;
    using Exiled.CustomItems.API;
    using Exiled.CustomItems.API.Features;
    using Exiled.CustomItems.API.Spawn;
    using Exiled.Events.EventArgs;
    using MEC;

    /// <inheritdoc />
    public class Scp127 : CustomWeapon
    {
        /// <inheritdoc/>
        public override uint Id { get; set; } = 7;

        /// <inheritdoc/>
        public override string Name { get; set; } = "SCP-127";

        /// <inheritdoc/>
        public override string Description { get; set; } = "SCP-127 is a pistol that slowly regenerates it's ammo over time but cannot be reloaded normally.";

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new DynamicSpawnPoint
                {
                    Chance = 100,
                    Location = SpawnLocation.Inside173Armory,
                },
            },
        };

        /// <inheritdoc/>
        public override Modifiers Modifiers { get; set; } = default;

        /// <inheritdoc/>
        public override float Damage { get; set; }

        /// <summary>
        /// Gets or sets how often ammo will be regenerated. Regeneration occurs at all times, however this timer is reset when the weapon is picked up or dropped.
        /// </summary>
        [Description("How often ammo will be regenerated. Regeneration occurs at all times, however this timer is reset when the weapon is picked up or dropped.")]
        public float RegenerationDelay { get; set; } = 10f;

        /// <summary>
        /// Gets or sets the amount of ammo that will be regenerated each regeneration cycle.
        /// </summary>
        [Description("The amount of ammo that will be regenerated each regeneration cycle.")]
        public int RegenerationAmount { get; set; } = 2;

        private List<CoroutineHandle> Coroutines { get; } = new List<CoroutineHandle>();

        /// <inheritdoc/>
        public override void Destroy()
        {
            foreach (CoroutineHandle handle in Coroutines)
                Timing.KillCoroutines(handle);

            base.Destroy();
        }

        /// <inheritdoc/>
        protected override void OnReloading(ReloadingWeaponEventArgs ev) => ev.IsAllowed = false;

        /// <inheritdoc/>
        protected override void ShowPickedUpMessage(Player player)
        {
            Coroutines.Add(Timing.RunCoroutine(DoInventoryRegeneration(player)));

            base.ShowPickedUpMessage(player);
        }

        /// <inheritdoc/>
        protected override void OnPickingUp(PickingUpItemEventArgs ev) => Coroutines.Add(Timing.RunCoroutine(DoInventoryRegeneration(ev.Player)));

        private IEnumerator<float> DoInventoryRegeneration(Player player)
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(RegenerationDelay);

                bool hasItem = false;

                for (int i = 0; i < player.Inventory.items.Count; i++)
                {
                    if (!Check(player.Inventory.items[i]))
                        continue;

                    hasItem = true;

                    if (!(player.Inventory.items[i].durability < ClipSize))
                        continue;

                    Inventory.SyncItemInfo newInfo = player.Inventory.items[i];
                    newInfo.durability += RegenerationAmount;
                    player.Inventory.items[i] = newInfo;
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

                foreach (Pickup pickup in Spawned)
                {
                    if (Check(pickup) && pickup.durability < ClipSize)
                    {
                        pickup.durability += RegenerationAmount;

                        yield return Timing.WaitForSeconds(0.5f);
                    }
                }
            }
        }
    }
}