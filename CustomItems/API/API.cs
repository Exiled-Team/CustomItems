using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Interactables.Interobjects.DoorUtils;
using UnityEngine;

namespace CustomItems.API
{
    public static class API
    {
        public static bool RegisterCustomItem(this CustomItem item)
        {
            if (!Plugin.Singleton.ItemManagers.Contains(item))
            {
                if (item.Name.Contains(":"))
                {
                    string newName = item.Name.Replace(":", "");
                    Log.Warn($"{item.Name} contains an invalid character and will be renamed to {newName}");
                    item.Name = newName;
                }

                if (Plugin.Singleton.ItemManagers.Any(i => i.Id == item.Id))
                {
                    Log.Error($"{item.Name} has tried to register with the same ItemID as another item: {item.Id}. It will not be registered.");

                    return false;
                }
                
                Plugin.Singleton.ItemManagers.Add(item);
                item.Init();
                Log.Debug($"{item.Name} ({item.Id}) has been successfully registered.", Plugin.Singleton.Config.Debug);
                return true;
            }

            Log.Warn($"Couldn't register {item} as it already exists.");
            return false;
        }

        public static void UnregisterCustomItem(this CustomItem item)
        {
            if (Plugin.Singleton.ItemManagers.Contains(item))
            {
                item.Destroy();
                Plugin.Singleton.ItemManagers.Remove(item);
            }
        }

        public static bool TryGetItem(string name, out CustomItem item)
        {
            foreach (CustomItem cItem in Plugin.Singleton.ItemManagers)
            {
                if (cItem.Name.Equals(name, System.StringComparison.OrdinalIgnoreCase))
                {
                    item = cItem;
                    return true;
                }
            }

            item = null;
            return false;
        }

        public static bool TryGetItem(int id, out CustomItem item)
        {
            foreach (CustomItem cItem in Plugin.Singleton.ItemManagers)
                if (cItem.Id == id)
                {
                    item = cItem;
                    return true;
                }

            item = null;
            return false;
        }

        public static void GiveItem(this Player player, CustomItem item) => item.GiveItem(player);

        public static bool GiveItem(this Player player, string name)
        {
            if (!TryGetItem(name, out CustomItem item)) 
                return false;
            
            item.GiveItem(player);
                
            return true;
        }

        public static bool GiveItem(this Player player, int id)
        {
            if (!TryGetItem(id, out CustomItem item))
                return false;
            
            item.GiveItem(player);

            return true;
        }

        public static void SpawnItem(this CustomItem item, Vector3 position) => item.SpawnItem(position);

        public static bool SpawnItem(string name, Vector3 position)
        {
            if (!TryGetItem(name, out CustomItem item)) 
                return false;
            
            item.SpawnItem(position);

            return true;
        }

        public static bool SpawnItem(int id, Vector3 position)
        {
            if (!TryGetItem(id, out CustomItem item))
                return false;
            
            item.SpawnItem(position);

            return true;
        }

        public static List<CustomItem> GetInstalledItems() => Plugin.Singleton.ItemManagers;
        

        public static Transform GetDoor(this SpawnLocation location)
        {
            if (SpawnLocationData.DoorNames.ContainsKey(location))
            {
                string doorName = SpawnLocationData.DoorNames[location];
                if (DoorNametagExtension.NamedDoors.TryGetValue(doorName, out var nametag))
                    return nametag.transform;
            }

            return null;
        }
        
        public static Vector3 TryGetLocation(this SpawnLocation location)
        {
            Vector3 pos = Vector3.zero;
            
            float modifier = SpawnLocationData.ReversedLocations.Contains(location) ? -3f : 3f;
            Transform transform = location.GetDoor();
            if (transform != null)
            {
                pos = (transform.position + (Vector3.up * 1.5f)) + (transform.forward * modifier);
            }

            return pos;
        }
    }
}