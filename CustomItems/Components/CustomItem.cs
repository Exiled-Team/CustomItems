using System;
using System.Collections.Generic;
using System.Linq;
using CustomItems.Events;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using Exiled.Loader;
using MEC;

namespace CustomItems.Components
{
    public abstract class CustomItem
    {
        public abstract ItemType ItemType { get; set; }
        public abstract string ItemName { get; set; }
        public abstract string ItemDescription { get; set; }

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
                
                ev.Player.ShowHint($"You have picked up a {ItemName}\n{ItemDescription}", 10f);
            }
        }
        
        public List<int> ItemIds { get; set; } = new List<int>();
        public List<Pickup> ItemPickups { get; set; } = new List<Pickup>();
        
        public bool CheckItem(Pickup pickup) => ItemPickups.Contains(pickup);
        public bool CheckItem(Inventory.SyncItemInfo item) => ItemIds.Contains(item.uniq);

        public void Init()
        {
            Exiled.Events.Handlers.Player.DroppingItem += OnDroppingItem;
            Exiled.Events.Handlers.Player.PickingUpItem += OnPickingUpItem;
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
                            GiveItem(ev.Player);
                    }
                }
            }
        }
    }
}