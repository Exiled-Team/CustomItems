using System;
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
                    Plugin.Singleton.SniperRifleComponent.GiveItem(player);
                    break;
                case "gl":
                    Plugin.Singleton.GrenadeLauncherComponent.GiveItem(player);
                    break;
                case "sg":
                    Plugin.Singleton.ShotgunManager.GiveItem(player);
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