using System;
using System.Collections.Generic;
using System.Linq;
using CustomItems.Events;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using Exiled.Loader;
using MEC;
using UnityEngine;

namespace CustomItems
{
    public abstract class CustomItem
    {
        protected CustomItem(ItemType type, int itemId)
        {
            ItemType = type;
            ItemId = itemId;
        }
        
        public abstract string ItemName { get; set; }
        public abstract string ItemDescription { get; set; }

        protected virtual int ModBarrel { get; set; } = 0;
        protected virtual int ModSight { get; set; } = 0;
        protected virtual int ModOther { get; set; } = 0;
        protected virtual int ClipSize { get; set; } = 1;
        protected virtual void LoadEvents(){}
        protected virtual void UnloadEvents(){}
        
        protected virtual void OnWaitingForPlayers()
        {
            ItemIds.Clear();
            ItemPickups.Clear();
        }

        protected virtual void OnReloadingWeapon(ReloadingWeaponEventArgs ev)
        {
            if (CheckItem(ev.Player.CurrentItem))
            {
                ev.IsAllowed = false;
                Log.Debug($"{ev.Player.Nickname} is reloading a {ItemName}!", Plugin.Singleton.Config.Debug);
                int remainingInClip = ClipSize - (int) ev.Player.CurrentItem.durability;
                int currentAmmoAmount = (int)ev.Player.Ammo[ev.Player.ReferenceHub.weaponManager.weapons[ev.Player.ReferenceHub.weaponManager.curWeapon].ammoType];
                int amountToReload = ClipSize - remainingInClip;
                if (currentAmmoAmount >= 0)
                {
                    ev.Player.ReferenceHub.weaponManager.scp268.ServerDisable();
                    Reload(ev.Player);
                    
                    int amountAfterReload = currentAmmoAmount - amountToReload;
                    if (amountAfterReload < 0)
                        ev.Player.Ammo[ev.Player.ReferenceHub.weaponManager.weapons[ev.Player.ReferenceHub.weaponManager.curWeapon].ammoType] = 0;
                    else
                        ev.Player.Ammo[ev.Player.ReferenceHub.weaponManager.weapons[ev.Player.ReferenceHub.weaponManager.curWeapon].ammoType] = (uint)(currentAmmoAmount - amountToReload);
                    
                    ev.Player.Inventory.items.ModifyDuration(ev.Player.Inventory.GetItemIndex(), ClipSize);
                    Log.Debug($"{ev.Player.Nickname} - {ev.Player.CurrentItem.durability} - {ev.Player.Ammo[ev.Player.ReferenceHub.weaponManager.weapons[ev.Player.ReferenceHub.weaponManager.curWeapon].ammoType]}", Plugin.Singleton.Config.Debug);
                    Timing.CallDelayed(4.5f, () =>
                    {
                        Reload(ev.Player);
                    });
                }
            }
        }
        
        protected virtual void OnDroppingItem(DroppingItemEventArgs ev)
        {
            if (CheckItem(ev.Item))
            {
                ev.IsAllowed = false;
                ItemPickups.Add(Exiled.API.Extensions.Item.Spawn(ev.Item.id, ev.Item.durability, ev.Player.Position, default, ev.Item.modSight, ev.Item.modBarrel, ev.Item.modOther));
                ev.Player.RemoveItem(ev.Item);
            }
        }

        protected virtual void OnPickingUpItem(PickingUpItemEventArgs ev)
        {
            if (CheckItem(ev.Pickup) && ev.Player.Inventory.items.Count < 8)
            {
                ev.IsAllowed = false;
                Inventory._uniqId++;
                Inventory.SyncItemInfo item = new Inventory.SyncItemInfo()
                {
                    durability = ev.Pickup.durability,
                    id = ev.Pickup.itemId,
                    modBarrel = ev.Pickup.weaponMods.Barrel,
                    modOther = ev.Pickup.weaponMods.Other,
                    modSight = ev.Pickup.weaponMods.Sight,
                    uniq = Inventory._uniqId
                };
                
                ev.Player.Inventory.items.Add(item);
                ItemIds.Add(item.uniq);
                ev.Pickup.Delete();
                
                ShowMessage(ev.Player);
            }
        }
        
        protected virtual void OnUpgradingItems(UpgradingItemsEventArgs ev)
        {
            Vector3 outPos = ev.Scp914.output.position - ev.Scp914.intake.position;
            
            foreach (Pickup pickup in ev.Items.ToList())
                if (CheckItem(pickup))
                {
                    pickup.transform.position += outPos;
                    ev.Items.Remove(pickup);
                }
            
            Dictionary<Player, Inventory.SyncItemInfo> itemsToSave = new Dictionary<Player, Inventory.SyncItemInfo>();
            
            foreach (Player player in ev.Players)
                foreach (Inventory.SyncItemInfo item in player.Inventory.items.ToList())
                    if (CheckItem(item))
                    {
                        itemsToSave.Add(player, item);
                        player.Inventory.items.Remove(item);
                    }

            Timing.CallDelayed(3.5f, () =>
            {
                foreach (KeyValuePair<Player, Inventory.SyncItemInfo> kvp in itemsToSave)
                    kvp.Key.Inventory.items.Add(kvp.Value);
            });
        }

        protected virtual void OnHandcuffing(HandcuffingEventArgs ev)
        {
            foreach (Inventory.SyncItemInfo item in ev.Target.Inventory.items)
                if (CheckItem(item))
                {
                    ItemPickups.Add(Exiled.API.Extensions.Item.Spawn(item.id, item.durability, ev.Target.Position, default, item.modSight, item.modBarrel, item.modOther));
                    ev.Target.RemoveItem(item);
                }
        }

        protected virtual void OnDying(DyingEventArgs ev)
        {
            foreach (Inventory.SyncItemInfo item in ev.Target.Inventory.items.ToList())
                if (CheckItem(item))
                {
                    ItemPickups.Add(Exiled.API.Extensions.Item.Spawn(item.id, item.durability, ev.Target.Position, default, item.modSight, item.modBarrel, item.modOther));
                    ev.Target.RemoveItem(item);
                }
        }

        protected virtual void ShowMessage(Player player) => player.ShowHint($"You have picked up a {ItemName}\n{ItemDescription}", 10f);

        protected virtual void ItemGiven(Player player){}

        public ItemType ItemType { get; set; }
        public int ItemId { get; set; }
        protected List<int> ItemIds { get; } = new List<int>();
        protected List<Pickup> ItemPickups { get; } = new List<Pickup>();
        
        protected bool CheckItem(Pickup pickup) => ItemPickups.Contains(pickup);
        protected bool CheckItem(Inventory.SyncItemInfo item) => ItemIds.Contains(item.uniq);

        public void Init()
        {
            Exiled.Events.Handlers.Player.Dying += OnDying;
            Exiled.Events.Handlers.Player.Handcuffing += OnHandcuffing;
            Exiled.Events.Handlers.Player.DroppingItem += OnDroppingItem;
            Exiled.Events.Handlers.Player.PickingUpItem += OnPickingUpItem;
            Exiled.Events.Handlers.Scp914.UpgradingItems += OnUpgradingItems;
            Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
            
            if (ItemType.IsWeapon())
                Exiled.Events.Handlers.Player.ReloadingWeapon += OnReloadingWeapon;

            try
            {
                CheckAndLoadSubclassEvent();
            }
            catch (Exception)
            {
                //ignored
            }

            LoadEvents();
        }

        public void Destroy()
        {
            Exiled.Events.Handlers.Player.DroppingItem -= OnDroppingItem;
            Exiled.Events.Handlers.Player.PickingUpItem -= OnPickingUpItem;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;

            try
            {
                CheckAndUnloadSubclassEvent();
            }
            catch (Exception)
            {
                //ignored
            }

            if (ItemType.IsWeapon())
                Exiled.Events.Handlers.Player.ReloadingWeapon -= OnReloadingWeapon;

            UnloadEvents();
        }
        
        private void Reload(Player player) => player.ReferenceHub.weaponManager.RpcReload(player.ReferenceHub.weaponManager.curWeapon);

        public void SpawnItem(Vector3 position) => ItemPickups.Add(Exiled.API.Extensions.Item.Spawn(ItemType, ClipSize, position, default, (int)ModSight, (int)ModBarrel, (int)ModOther));

        private void CheckAndLoadSubclassEvent()
        {
            if (Loader.Plugins.Any(p => p.Name == "Subclass"))
                AddClassEvent.AddClass += OnAddingClass;
        }
        private void CheckAndUnloadSubclassEvent()
        {
            if (Loader.Plugins.Any(p => p.Name == "Subclass"))
                AddClassEvent.AddClass -= OnAddingClass;
        }

        private void OnAddingClass(AddClassEventArgs ev)
        {
            if (Plugin.Singleton.Config.SubclassItems.ContainsKey(ev.Subclass.Name))
            {
                foreach (Tuple<CustomItem, float> item in Plugin.Singleton.Config.SubclassItems[ev.Subclass.Name])
                {
                    if (item.Item1.ItemName == ItemName)
                    {
                        int r = Plugin.Singleton.Rng.Next(100);
                        if (r < item.Item2)
                            Timing.CallDelayed(1.5f, () => GiveItem(ev.Player));
                    }
                }
            }
        }
        
        public void GiveItem(Player player)
        {
            ++Inventory._uniqId;
            Inventory.SyncItemInfo syncItemInfo = new Inventory.SyncItemInfo()
            {
                durability = ClipSize,
                id = ItemType,
                uniq = Inventory._uniqId,
                modBarrel = (int)ModBarrel,
                modSight = (int)ModSight,
                modOther = (int)ModOther
            };
            player.Inventory.items.Add(syncItemInfo);
            ItemIds.Add(syncItemInfo.uniq);
            ShowMessage(player);
            
            ItemGiven(player);
        }

        public override string ToString() => $"[{ItemName}] {ItemDescription} {ItemType}";
    }
}