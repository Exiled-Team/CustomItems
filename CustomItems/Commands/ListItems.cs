using System;
using CommandSystem;
using CustomItems.API;
using Exiled.Permissions.Extensions;

namespace CustomItems.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class ListItems : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string message = string.Empty;
            foreach (CustomItem item in Plugin.Singleton.ItemManagers)
                message += $"{item.ItemName}({item.ItemId})\n";

            if (string.IsNullOrEmpty(message))
                response = "There are no custom items currently on this server.";
            response = message;

            return true;
        }

        public string Command { get; } = "citemlist";
        public string[] Aliases { get; } = new[] { "clist" };

        public string Description { get; } =
            "Gets a list of all custom items currently installed and enabled on the server.";
    }
}