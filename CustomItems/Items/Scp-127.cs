using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using MEC;

namespace CustomItems.Items
{
    public class Scp127 : CustomItem
    {
        public override string ItemName { get; set; } = "SCP-127";
        protected override int ClipSize { get; set; } = 12;
        private List<CoroutineHandle> Coroutines { get; } = new List<CoroutineHandle>();

        public override string ItemDescription { get; set; } =
            "SCP-127 is a pistol that slowly regenerates it's ammo over time but cannot be reloaded normally.";

        protected override void LoadEvents()
        {
            Exiled.Events.Handlers.Player.PickingUpItem += OnPickingUp;
        }

        protected override void UnloadEvents()
        {
            foreach (CoroutineHandle handle in Coroutines)
                Timing.KillCoroutines(handle);

            Exiled.Events.Handlers.Player.PickingUpItem -= OnPickingUp;
        }

        protected override void OnReloadingWeapon(ReloadingWeaponEventArgs ev)
        {
            if (CheckItem(ev.Player.CurrentItem))
                ev.IsAllowed = false;
        }

        protected override void ItemGiven(Player player)
        {
            Coroutines.Add(Timing.RunCoroutine(DoInventoryRegeneration(player)));
        }

        private void OnPickingUp(PickingUpItemEventArgs ev)
        {
            if (CheckItem(ev.Pickup))
                Coroutines.Add(Timing.RunCoroutine(DoInventoryRegeneration(ev.Player)));
        }

        IEnumerator<float> DoInventoryRegeneration(Player player)
        {
            for (;;)
            {
                yield return Timing.WaitForSeconds(Plugin.Singleton.Config.Scp127RegenerationDelay);

                bool hasItem = false;

                for (int i = 0; i < player.Inventory.items.Count; i++)
                {
                    if (CheckItem(player.Inventory.items[i]))
                    {
                        hasItem = true;

                        if (player.Inventory.items[i].durability < ClipSize)
                        {
                            Inventory.SyncItemInfo newInfo = player.Inventory.items[i];
                            newInfo.durability += Plugin.Singleton.Config.Scp127RegenerationAmount;
                            player.Inventory.items[i] = newInfo;
                        }
                    }
                }

                if (!hasItem)
                    yield break;
            }
        }

        IEnumerator<float> DoAmmoRegeneration()
        {
            for (;;)
            {
                yield return Timing.WaitForSeconds(Plugin.Singleton.Config.Scp127RegenerationDelay);

                foreach (Pickup pickup in ItemPickups)
                    if (CheckItem(pickup) && pickup.durability < ClipSize)
                    {
                        pickup.durability += Plugin.Singleton.Config.Scp127RegenerationAmount;

                        yield return Timing.WaitForSeconds(0.5f);
                    }
            }
        }

        public Scp127(ItemType type, int itemId) : base(type, itemId)
        {
            Coroutines.Add(Timing.RunCoroutine(DoAmmoRegeneration()));
        }
    }
}