using System;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using Interactables.Interobjects.DoorUtils;
using UnityEngine;

namespace CustomItems.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class CalculatePosition : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(((CommandSender)sender).SenderId);
            string[] args = arguments.Array;
            Vector3 pos = Vector3.zero;


            if (DoorNametagExtension.NamedDoors.TryGetValue(args[1], out DoorNametagExtension nameTag))
                pos = nameTag.transform.position + (nameTag.transform.forward * 2);
            Log.Info($"{args[1]} - {pos}");


            if (pos == Vector3.zero)
            {
                response = "Object not found.";
                return false;
            }
            
            player.Position = pos;

            response = "printed to console.";
            return true;
        }

        public string Command { get; } = "calculatepos";
        public string[] Aliases { get; } = new[] { "cpos" };
        public string Description { get; } = "i hate this fucking game i hate this fucking game i hate this fucking game i hate this fucking game i hate this fucking game i hate this fucking game i hate this fucking game i hate this fucking game i hate this fucking game i hate this fucking game i hate this fucking game i hate this fucking game i hate this fucking game i hate this fucking game i hate this fucking game i hate this fucking game i hate this fucking game ";
    }
}