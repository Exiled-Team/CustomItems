using System;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;

namespace CustomItems.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class GiveItem : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(((CommandSender)sender).SenderId);
            string[] args = arguments.Array;
            switch (args[1])
            {
                case "sniper":
                    Plugin.Singleton.ItemManagers.FirstOrDefault(m => m.ItemName == "SR-119")?.GiveItem(player);
                    break;
                case "gl":
                    Plugin.Singleton.ItemManagers.FirstOrDefault(m => m.ItemName == "RL-119")?.GiveItem(player);
                    break;
                case "sg":
                    Plugin.Singleton.ItemManagers.FirstOrDefault(m => m.ItemName == "SG-119")?.GiveItem(player);
                    break;
            }

            response = "Done.";
            return true;
        }

        public string Command { get; } = "customgive";
        public string[] Aliases { get; } = new[] { "cgive" };
        public string Description { get; } = "Gives a custom item!";
    }
}