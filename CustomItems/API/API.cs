// <copyright file="API.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CustomItems.API
{
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Features;
    using Interactables.Interobjects.DoorUtils;
    using UnityEngine;

    /// <summary>
    /// A collection of API methods.
    /// </summary>
    public static class API
    {
        /// <summary>
        /// Registers a <see cref="CustomItem"/> manager with the plugin.
        /// </summary>
        /// <param name="item">The <see cref="CustomItem"/> to register.</param>
        /// <returns>A <see cref="bool"/> indicating whether or not the item was registered.</returns>
        public static bool RegisterCustomItem(this CustomItem item)
        {
            if (!Plugin.Singleton.ItemManagers.Contains(item))
            {
                if (item.Name.Contains(":"))
                {
                    string newName = item.Name.Replace(":", string.Empty);
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

        /// <summary>
        /// Unregisters a <see cref="CustomItem"/> manager.
        /// </summary>
        /// <param name="item">The <see cref="CustomItem"/> to unregister.</param>
        public static void UnregisterCustomItem(this CustomItem item)
        {
            if (!Plugin.Singleton.ItemManagers.Contains(item))
                return;

            item.Destroy();
            Plugin.Singleton.ItemManagers.Remove(item);
        }

        /// <summary>
        /// Tries to get a <see cref="CustomItem"/> with a particular name.
        /// </summary>
        /// <param name="name">The <see cref="string"/> name of the item.</param>
        /// <param name="item">The <see cref="CustomItem"/> item found.</param>
        /// <returns>The <see cref="CustomItem"/> matching the search. Can be null.</returns>
        public static bool TryGetItem(string name, out CustomItem item)
        {
            foreach (CustomItem cItem in Plugin.Singleton.ItemManagers)
            {
                if (!cItem.Name.Equals(name, System.StringComparison.OrdinalIgnoreCase))
                    continue;

                item = cItem;
                return true;
            }

            item = null;
            return false;
        }

        /// <summary>
        /// Tries to get a <see cref="CustomItem"/> with a particular ID.
        /// </summary>
        /// <param name="id">The <see cref="int"/>ID of the item to look for.</param>
        /// <param name="item">The <see cref="CustomItem"/> found.</param>
        /// <returns>The <see cref="CustomItem"/> matching the search. Can be null.</returns>
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

        /// <summary>
        /// Gives the specified item to a player.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to give the item to.</param>
        /// <param name="item">The <see cref="CustomItem"/> to give to the player.</param>
        public static void GiveItem(this Player player, CustomItem item) => item.GiveItem(player);

        /// <summary>
        /// Gives the player a specified item.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to give the item to.</param>
        /// <param name="name">The <see cref="string"/> name of the item to give.</param>
        /// <returns>A <see cref="bool"/> indicating if the player was given the item or not.</returns>
        public static bool GiveItem(this Player player, string name)
        {
            if (!TryGetItem(name, out CustomItem item))
                return false;

            item.GiveItem(player);

            return true;
        }

        /// <summary>
        /// Gives the player a specified item.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to give the item to.</param>
        /// <param name="id">The <see cref="int"/> name of the item to give.</param>
        /// <returns>A <see cref="bool"/> indicating if the player was given the item or not.</returns>
        public static bool GiveItem(this Player player, int id)
        {
            if (!TryGetItem(id, out CustomItem item))
                return false;

            item.GiveItem(player);

            return true;
        }

        /// <summary>
        /// Spawns a specified item at a location.
        /// </summary>
        /// <param name="item">The <see cref="CustomItem"/> to spawn.</param>
        /// <param name="position">The <see cref="Vector3"/> location to spawn the item at.</param>
        public static void SpawnItem(this CustomItem item, Vector3 position) => item.SpawnItem(position);

        /// <summary>
        /// Spawns a specified item at a location.
        /// </summary>
        /// <param name="name">The <see cref="string"/> name of the item to spawn.</param>
        /// <param name="position">The <see cref="Vector3"/> location to spawn the item.</param>
        /// <returns>A <see cref="bool"/> value indicating whether or not the item was spawned.</returns>
        public static bool SpawnItem(string name, Vector3 position)
        {
            if (!TryGetItem(name, out CustomItem item))
                return false;

            item.SpawnItem(position);

            return true;
        }

        /// <summary>
        /// Spawns a specified item at a location.
        /// </summary>
        /// <param name="id">The <see cref="int"/> ID of the item to spawn.</param>
        /// <param name="position">The <see cref="Vector3"/> location to spawn the item.</param>
        /// <returns>A <see cref="bool"/> value indicating whether or not the item was spawned.</returns>
        public static bool SpawnItem(int id, Vector3 position)
        {
            if (!TryGetItem(id, out CustomItem item))
                return false;

            item.SpawnItem(position);

            return true;
        }

        /// <summary>
        /// Gets a list of all currently active Item Managers.
        /// </summary>
        /// <returns>A list of all <see cref="CustomItem"/>s.</returns>
        public static List<CustomItem> GetInstalledItems() => Plugin.Singleton.ItemManagers;

        /// <summary>
        /// Tries to get the <see cref="Transform"/> of the door used for a specific <see cref="SpawnLocation"/>.
        /// </summary>
        /// <param name="location">The <see cref="SpawnLocation"/> to check.</param>
        /// <returns>The <see cref="Transform"/> used for that spawn location. Can be null.</returns>
        public static Transform GetDoor(this SpawnLocation location)
        {
            if (!SpawnLocationData.DoorNames.ContainsKey(location))
                return null;

            string doorName = SpawnLocationData.DoorNames[location];
            return DoorNametagExtension.NamedDoors.TryGetValue(doorName, out var nametag) ? nametag.transform : null;
        }

        /// <summary>
        /// Tries to get the <see cref="Vector3"/> used for a specific <see cref="SpawnLocation"/>.
        /// </summary>
        /// <param name="location">The <see cref="SpawnLocation"/> to check.</param>
        /// <returns>The <see cref="Vector3"/> used for that spawn location. Can be <see cref="Vector3.zero"/>.</returns>
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