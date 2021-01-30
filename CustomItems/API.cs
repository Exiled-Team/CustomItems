using CustomItems.Components;
using Exiled.API.Features;
using UnityEngine;

namespace CustomItems
{
    public static class API
    {
        public static void RegisterCustomItem(CustomItem item)
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
            }
        }

        public static void UnregisterCustomItem(CustomItem item)
        {
            if (Plugin.Singleton.ItemManagers.Contains(item))
            {
                item.Destroy();
                Plugin.Singleton.ItemManagers.Remove(item);
            }
        }

        public static CustomItem GetItem(string name)
        {
            foreach (CustomItem item in Plugin.Singleton.ItemManagers)
                if (item.ItemName == name)
                    return item;
            return null;
        }

        public static void GiveItem(Player player, CustomItem item) => item.GiveItem(player);
        public static void GiveItem(Player player, string name) => GetItem(name)?.GiveItem(player);
        public static void SpawnItem(CustomItem item, Vector3 position) => item.SpawnItem(position);
        public static void SpawnItem(string name, Vector3 position) => GetItem(name)?.SpawnItem(position);
    }
}