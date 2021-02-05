using System;
using CommandSystem;
using CustomItems.API;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using Mirror;
using UnityEngine;

namespace CustomItems.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class SpawnItem : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("citems.spawn"))
            {
                response = "Permission Denied.";

                return false;
            }
            
            string[] args = arguments.Array;

            if (args == null || args.Length < 3)
            {
                response = "You must specify an item to spawn and a location/player to spawn it at.";
                return false;
            }

            if (!API.API.TryGetItem(args[1], out CustomItem item))
            {
                response = $"Invalid item: {args[1]}";
                return false;
            }
            
            Vector3 spawnPos = Vector3.zero;
            if (TryParseVector3(args[2], out Vector3 pos))
                spawnPos = pos;
            else if (Enum.TryParse(args[2], out SpawnLocation location))
                spawnPos = location.TryGetLocation();
            else if (Player.Get(args[2]) is Player player)
                spawnPos = player.Position;

            if (spawnPos == Vector3.zero)
            {
                response = $"Unable to find spawn location: {args[2]}";
                return false;
            }
            
            item.SpawnItem(spawnPos);
            response = $"{item.ItemName} has been spawned at {spawnPos}.";
            
            return true;
        }

        public bool TryParseVector3(string s, out Vector3 vector)
        {
            vector = Vector3.zero;
            s = s.Replace("(", "").Replace(")", "");
            string[] split = s.Split(',');

            if (!float.TryParse(split[0], out float x) || !float.TryParse(split[1], out float y) || !float.TryParse(split[2], out float z)) 
                return false;
            
            vector = new Vector3(x, y, z);
            
            return true;
        }

        public string Command { get; } = "citemspawn";
        public string[] Aliases { get; } = new[] { "cspawn" };

        public string Description { get; } =
            "Spawn an item at the specified Spawn Location, coordinates, or at the designated player's feet.";
    }
}