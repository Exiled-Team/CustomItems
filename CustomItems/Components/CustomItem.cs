using System.Collections.Generic;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using MEC;
using UnityEngine;

namespace CustomItems.Components
{
    public abstract class CustomItem : MonoBehaviour
    {
        public abstract ItemType ItemType { get; set; }

        public virtual int ModBarrel { get; set; } = 0;
        public virtual int ModSight { get; set; } = 0;
        public virtual int ModOther { get; set; } = 0;
        public virtual int ClipSize { get; set; } = 1;
        public virtual void LoadEvents(){}
        public virtual void UnloadEvents(){}
        
        public virtual void OnWaitingForPlayers()
        {
            ItemIds.Clear();
            ItemPickups.Clear();
        }

        public virtual void OnReloadingWeapon(ReloadingWeaponEventArgs ev)
        {
            Log.Debug($"{ev.Player.Nickname} is reloading..", Plugin.Singleton.Config.Debug);
            if (CheckItem(ev.Player.CurrentItem))
            {
                ev.IsAllowed = false;
                Log.Debug($"{ev.Player.Nickname} is reloading a sniper!", Plugin.Singleton.Config.Debug);
                int amount = (int) ev.Player.Ammo[ev.Player.ReferenceHub.weaponManager.weapons[ev.Player.ReferenceHub.weaponManager.curWeapon].ammoType] - 1;
                Log.Debug($"{ev.Player.Nickname}: {amount}", Plugin.Singleton.Config.Debug);
                if (amount >= 0)
                {
                    ev.Player.ReferenceHub.weaponManager.scp268.ServerDisable();
                    Reload(ev.Player);
                    ev.Player.Ammo[ev.Player.ReferenceHub.weaponManager.weapons[ev.Player.ReferenceHub.weaponManager.curWeapon].ammoType] = (uint)amount;
                    ev.Player.Inventory.items.ModifyDuration(ev.Player.Inventory.GetItemIndex(), 1f);
                    Log.Debug($"{ev.Player.Nickname} - {ev.Player.CurrentItem.durability}", Plugin.Singleton.Config.Debug);
                    Timing.CallDelayed(4.5f, () =>
                    {
                        Reload(ev.Player);
                    });
                }
            }
        }
        
        public virtual void OnDroppingItem(DroppingItemEventArgs ev)
        {
            if (CheckItem(ev.Item))
            {
                ev.IsAllowed = false;
                ItemPickups.Add(Exiled.API.Extensions.Item.Spawn(ev.Item.id, ev.Item.durability, ev.Player.Position, default, ev.Item.modSight, ev.Item.modBarrel, ev.Item.modOther));
                ev.Player.RemoveItem(ev.Item);
            }
        }

        public virtual void OnPickingUpItem(PickingUpItemEventArgs ev)
        {
            if (CheckItem(ev.Pickup) && ev.Player.Inventory.items.Count < 8)
            {
                ev.IsAllowed = false;
                Inventory._uniqId++;
                Inventory.SyncItemInfo rifle = new Inventory.SyncItemInfo()
                {
                    durability = ev.Pickup.durability,
                    id = ev.Pickup.itemId,
                    modBarrel = ev.Pickup.weaponMods.Barrel,
                    modOther = ev.Pickup.weaponMods.Other,
                    modSight = ev.Pickup.weaponMods.Sight,
                    uniq = Inventory._uniqId
                };
                
                ev.Player.Inventory.items.Add(rifle);
                ItemIds.Add(rifle.uniq);
                ev.Pickup.Delete();
            }
        }
        
        public List<int> ItemIds { get; set; } = new List<int>();
        public List<Pickup> ItemPickups { get; set; } = new List<Pickup>();
        
        public bool CheckItem(Pickup pickup) => ItemPickups.Contains(pickup);
        public bool CheckItem(Inventory.SyncItemInfo item) => ItemIds.Contains(item.uniq);

        private void Awake()
        {
            Exiled.Events.Handlers.Player.DroppingItem += OnDroppingItem;
            Exiled.Events.Handlers.Player.PickingUpItem += OnPickingUpItem;
            Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
            
            if (ItemType.IsWeapon())
                Exiled.Events.Handlers.Player.ReloadingWeapon += OnReloadingWeapon;
            
            LoadEvents();
        }

        private void OnDestroy()
        {
            Exiled.Events.Handlers.Player.DroppingItem -= OnDroppingItem;
            Exiled.Events.Handlers.Player.PickingUpItem -= OnPickingUpItem;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;

            if (ItemType.IsWeapon())
                Exiled.Events.Handlers.Player.ReloadingWeapon -= OnReloadingWeapon;

            UnloadEvents();
        }
        
        private void Reload(Player player) => player.ReferenceHub.weaponManager.RpcReload(player.ReferenceHub.weaponManager.curWeapon);

        public void GiveItem(Player player)
        {
            ++Inventory._uniqId;
            Inventory.SyncItemInfo syncItemInfo = new Inventory.SyncItemInfo()
            {
                durability = ClipSize,
                id = ItemType,
                uniq = Inventory._uniqId,
                modBarrel = ModBarrel,
                modSight = ModSight,
                modOther = ModOther
            };
            player.Inventory.items.Add(syncItemInfo);
            ItemIds.Add(syncItemInfo.uniq);
        }
    }
}