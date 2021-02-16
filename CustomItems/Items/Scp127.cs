// <copyright file="Scp127.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CustomItems.Items
{
    using System.Collections.Generic;
    using Exiled.API.Features;
    using Exiled.CustomItems.API;
    using Exiled.Events.EventArgs;
    using MEC;

    /// <inheritdoc />
    public class Scp127 : CustomWeapon
    {
        /// <inheritdoc />
        public Scp127(ItemType type, int clipSize, int itemId)
            : base(type, clipSize, itemId)
        {
            Coroutines.Add(Timing.RunCoroutine(DoAmmoRegeneration()));
        }

        /// <inheritdoc/>
        public override string Name { get; set; } = Plugin.Singleton.Config.ItemConfigs.Scp127Cfg.Name;

        /// <inheritdoc/>
        public override Dictionary<SpawnLocation, float> SpawnLocations { get; set; } = Plugin.Singleton.Config.ItemConfigs.Scp127Cfg.SpawnLocations;

        /// <inheritdoc/>
        public override string Description { get; set; } = Plugin.Singleton.Config.ItemConfigs.Scp127Cfg.Description;

        /// <inheritdoc/>
        public override int SpawnLimit { get; set; } = Plugin.Singleton.Config.ItemConfigs.Scp127Cfg.SpawnLimit;

        private List<CoroutineHandle> Coroutines { get; } = new List<CoroutineHandle>();

        /// <inheritdoc/>
        protected override void LoadEvents()
        {
            Exiled.Events.Handlers.Player.PickingUpItem += OnPickingUp;
            base.LoadEvents();
        }

        /// <inheritdoc/>
        protected override void UnloadEvents()
        {
            foreach (CoroutineHandle handle in Coroutines)
                Timing.KillCoroutines(handle);

            Exiled.Events.Handlers.Player.PickingUpItem -= OnPickingUp;
            base.UnloadEvents();
        }

        /// <inheritdoc/>
        protected override void OnReloadingWeapon(ReloadingWeaponEventArgs ev)
        {
            if (CheckItem(ev.Player.CurrentItem))
                ev.IsAllowed = false;
        }

        /// <inheritdoc/>
        protected override void ItemGiven(Player player)
        {
            Coroutines.Add(Timing.RunCoroutine(DoInventoryRegeneration(player)));
        }

        private void OnPickingUp(PickingUpItemEventArgs ev)
        {
            if (CheckItem(ev.Pickup))
                Coroutines.Add(Timing.RunCoroutine(DoInventoryRegeneration(ev.Player)));
        }

        private IEnumerator<float> DoInventoryRegeneration(Player player)
        {
            for (; ;)
            {
                yield return Timing.WaitForSeconds(Plugin.Singleton.Config.ItemConfigs.Scp127Cfg.RegenDelay);

                bool hasItem = false;

                for (int i = 0; i < player.Inventory.items.Count; i++)
                {
                    if (!CheckItem(player.Inventory.items[i]))
                        continue;

                    hasItem = true;

                    if (!(player.Inventory.items[i].durability < ClipSize))
                        continue;

                    Inventory.SyncItemInfo newInfo = player.Inventory.items[i];
                    newInfo.durability += Plugin.Singleton.Config.ItemConfigs.Scp127Cfg.RegenAmount;
                    player.Inventory.items[i] = newInfo;
                }

                if (!hasItem)
                    yield break;
            }
        }

        private IEnumerator<float> DoAmmoRegeneration()
        {
            for (; ;)
            {
                yield return Timing.WaitForSeconds(Plugin.Singleton.Config.ItemConfigs.Scp127Cfg.RegenDelay);

                foreach (Pickup pickup in ItemPickups)
                    if (CheckItem(pickup) && pickup.durability < ClipSize)
                    {
                        pickup.durability += Plugin.Singleton.Config.ItemConfigs.Scp127Cfg.RegenAmount;

                        yield return Timing.WaitForSeconds(0.5f);
                    }
            }
        }
    }
}