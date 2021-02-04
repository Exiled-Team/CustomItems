using System;
using System.Collections.Generic;
using System.Linq;
using CustomItems.Events;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using Exiled.Loader;
using Interactables.Interobjects.DoorUtils;
using MEC;
using UnityEngine;
using Utf8Json.Resolvers.Internal;

namespace CustomItems.API
{
    public abstract class CustomItem
    {
        protected CustomItem(ItemType type, int itemId)
        {
            ItemType = type;
            ItemId = itemId;
        }
        
        
        public abstract string ItemName { get; set; }
        protected abstract string ItemDescription { get; set; }
        public virtual Dictionary<SpawnLocation, float> SpawnLocations { get; set; }
        
        protected virtual void LoadEvents(){}
        protected virtual void UnloadEvents(){}
        
        protected virtual void OnWaitingForPlayers()
        {
            ItemIds.Clear();
            ItemPickups.Clear();
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
        
        public virtual void SpawnItem(Vector3 position) => ItemPickups.Add(Exiled.API.Extensions.Item.Spawn(ItemType, 1, position));
        
        public virtual void GiveItem(Player player)
        {
            ++Inventory._uniqId;
            Inventory.SyncItemInfo syncItemInfo = new Inventory.SyncItemInfo()
            {
                durability = 1,
                id = ItemType,
                uniq = Inventory._uniqId,
            };
            player.Inventory.items.Add(syncItemInfo);
            ItemIds.Add(syncItemInfo.uniq);
            ShowMessage(player);
            
            ItemGiven(player);
        }

        public ItemType ItemType { get; set; }
        public int ItemId { get; set; }
        protected List<int> ItemIds { get; } = new List<int>();
        protected List<Pickup> ItemPickups { get; } = new List<Pickup>();

        protected bool CheckItem(Pickup pickup) => ItemPickups.Contains(pickup);
        protected bool CheckItem(Inventory.SyncItemInfo item) => ItemIds.Contains(item.uniq);

        public static Vector3 RoomLocation(string roomName)
        {
            foreach (Room room in Map.Rooms)
                if (room.Name == roomName)
                    return room.Position;
            
            return Vector3.zero;
        }

        public virtual void Init()
        {
            Exiled.Events.Handlers.Player.Dying += OnDying;
            Exiled.Events.Handlers.Player.Handcuffing += OnHandcuffing;
            Exiled.Events.Handlers.Player.DroppingItem += OnDroppingItem;
            Exiled.Events.Handlers.Player.PickingUpItem += OnPickingUpItem;
            Exiled.Events.Handlers.Scp914.UpgradingItems += OnUpgradingItems;
            Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;

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

        public virtual void Destroy()
        {
            Exiled.Events.Handlers.Player.Dying -= OnDying;
            Exiled.Events.Handlers.Player.Handcuffing -= OnHandcuffing;
            Exiled.Events.Handlers.Player.DroppingItem -= OnDroppingItem;
            Exiled.Events.Handlers.Player.PickingUpItem -= OnPickingUpItem;
            Exiled.Events.Handlers.Scp914.UpgradingItems -= OnUpgradingItems;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;

            try
            {
                CheckAndUnloadSubclassEvent();
            }
            catch (Exception)
            {
                //ignored
            }

            UnloadEvents();
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
                            Timing.CallDelayed(1.5f, () => GiveItem(ev.Player));
                    }
                }
            }
        }

        public override string ToString() => $"[{ItemName}] {ItemDescription} {ItemType}";
    }
}