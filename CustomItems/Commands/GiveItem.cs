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
            if (args == null)
            {
                response = "This error is an easter egg because it can't happen.";

                return false;
            }
            
            if (args.Length < 2)
            {
                response = "You must define an item name.";
                
                return false;
            }
            
            if (args.Length > 2)
            {
                string identifier = string.Empty;
                foreach (string s in args.Skip(2))
                    identifier += $"{s} ";
                identifier = identifier.Trim();
                
                player = Player.Get(identifier);
                if (player == null)
                {
                    response = $"Unable to find player: {identifier}";

                    return false;
                }
            }

            if (int.TryParse(args[1], out int id))
            {
                player.GiveItem(id);

                response = "Done.";

                return true;
            }
            
            if (player.GiveItem(args[1]))
            {
                response = "Done.";
                
                return true;
            }

            response = "Item not found.";
            
            return false;
        }

        public string Command { get; } = "customgive";
        public string[] Aliases { get; } = new[] { "cgive" };
        public string Description { get; } = "Gives a custom item!";
    }
}