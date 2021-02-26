// -----------------------------------------------------------------------
// <copyright file="Scp127.cs" company="Galaxy199 and iopietro">
// Copyright (c) Galaxy199 and iopietro. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems.Items
{
    using System.Collections.Generic;
    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;
    using Exiled.CustomItems.API.Spawn;
    using Exiled.Events.EventArgs;
    using MEC;

    /// <inheritdoc />
    public class Scp127 : CustomWeapon
    {
        /*/// <inheritdoc />
        public Scp127(ItemType type, uint clipSize, uint itemId)
            : base(type, itemId, clipSize)
        {
            Coroutines.Add(Timing.RunCoroutine(DoAmmoRegeneration()));
        }*/

        /// <inheritdoc/>
        public override string Name { get; } = CustomItems.Instance.Config.ItemConfigs.Scp127Cfg.Name;

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; protected set; } = CustomItems.Instance.Config.ItemConfigs.Scp127Cfg.SpawnProperties;

        /// <inheritdoc/>
        public override string Description { get; } = CustomItems.Instance.Config.ItemConfigs.Scp127Cfg.Description;

        private List<CoroutineHandle> Coroutines { get; } = new List<CoroutineHandle>();

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            foreach (CoroutineHandle handle in Coroutines)
                Timing.KillCoroutines(handle);

            base.UnsubscribeEvents();
        }

        /// <inheritdoc/>
        protected override void OnReloading(ReloadingWeaponEventArgs ev)
        {
            if (Check(ev.Player.CurrentItem))
                ev.IsAllowed = false;
        }

        /// <inheritdoc/>
        protected override void ShowPickedUpMessage(Player player)
        {
            Coroutines.Add(Timing.RunCoroutine(DoInventoryRegeneration(player)));

            base.ShowPickedUpMessage(player);
        }

        /// <inheritdoc/>
        protected override void OnPickingUp(PickingUpItemEventArgs ev)
        {
            if (Check(ev.Pickup))
                Coroutines.Add(Timing.RunCoroutine(DoInventoryRegeneration(ev.Player)));

            base.OnPickingUp(ev);
        }

        private IEnumerator<float> DoInventoryRegeneration(Player player)
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(CustomItems.Instance.Config.ItemConfigs.Scp127Cfg.RegenDelay);

                bool hasItem = false;

                for (int i = 0; i < player.Inventory.items.Count; i++)
                {
                    if (!Check(player.Inventory.items[i]))
                        continue;

                    hasItem = true;

                    if (!(player.Inventory.items[i].durability < ClipSize))
                        continue;

                    Inventory.SyncItemInfo newInfo = player.Inventory.items[i];
                    newInfo.durability += CustomItems.Instance.Config.ItemConfigs.Scp127Cfg.RegenAmount;
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
                yield return Timing.WaitForSeconds(CustomItems.Instance.Config.ItemConfigs.Scp127Cfg.RegenDelay);

                foreach (Pickup pickup in Spawned)
                    if (Check(pickup) && pickup.durability < ClipSize)
                    {
                        pickup.durability += CustomItems.Instance.Config.ItemConfigs.Scp127Cfg.RegenAmount;

                        yield return Timing.WaitForSeconds(0.5f);
                    }
            }
        }
    }
}