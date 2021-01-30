using System.Collections.Generic;
using CustomItems.Components;
using Exiled.API.Features;
using UnityEngine;

namespace CustomItems
{
    public static class API
    {
        public static bool RegisterCustomItem(this CustomItem item)
        {
            if (!Plugin.Singleton.ItemManagers.Contains(item))
            {
                if (item.ItemName.Contains(":"))
                {
                    string newName = item.ItemName.Replace(":", "");
                    Log.Warn($"{item.ItemName} contains an invalid character and will be renamed to {newName}");
                    item.ItemName = newName;
                }
                Plugin.Singleton.ItemManagers.Add(item);
                item.Init();
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
                if (cItem.ItemName == name)
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

        public static void SpawnItem(this CustomItem item, Vector3 position) => item.SpawnItem(position);

        public static bool SpawnItem(string name, Vector3 position)
        {
            if (!TryGetItem(name, out CustomItem item)) 
                return false;
            
            item.SpawnItem(position);

            return true;

        }

        public static List<CustomItem> GetInstalledItems() => Plugin.Singleton.ItemManagers;
    }
}