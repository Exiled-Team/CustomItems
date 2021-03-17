// -----------------------------------------------------------------------
// <copyright file="C4Charge.cs" company="Galaxy119 and iopietro">
// Copyright (c) Galaxy119 and iopietro. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems.Commands
{
    using System;
    using System.Linq;
    using CommandSystem;
    using Exiled.API.Features;
    using UnityEngine;

    /// <inheritdoc/>
    [CommandHandler(typeof(ClientCommandHandler))]
    public class C4Charge : ICommand
    {
        /// <inheritdoc/>
        public string Command { get; } = "detonate";

        /// <inheritdoc/>
        public string[] Aliases { get; } = new string[] { "det" };

        /// <inheritdoc/>
        public string Description { get; } = "Detonate command for detonating C4 charges";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player ply = Player.Get((sender as CommandSender).SenderId);

            if (!Items.C4Charge.PlacedCharges.ContainsValue(ply))
            {
                response = "\n<color=red>You've haven't placed any C4 charges!</color>";
                return false;
            }

            if (Items.C4Charge.Instance.RequireDetonator && ply.CurrentItem.id != Items.C4Charge.Instance.DetonatorItem)
            {
                response = $"\n<color=red>You need to have a Remote Detonator ({Items.C4Charge.Instance.DetonatorItem}) in your hand to detonate C4!</color>";
                return false;
            }

            int i = 0;

            foreach (var charge in Items.C4Charge.PlacedCharges.ToList())
            {
                if (charge.Value != ply)
                    continue;

                if (Vector3.Distance(charge.Key.transform.position, ply.Position) < Items.C4Charge.Instance.MaxDistance)
                {
                    Items.C4Charge.Instance.C4Handler(charge.Key);

                    i++;
                }
                else
                {
                    ply.SendConsoleMessage($"One of your charges is out of range. You need to get closer by {Math.Round(Vector3.Distance(charge.Key.transform.position, ply.Position) - Items.C4Charge.Instance.MaxDistance)} meters.", "yellow");
                }
            }

            if (i == 1)
                response = $"\n<color=green>{i} C4 charge has been detonated!</color>";
            else
                response = $"\n<color=green>{i} C4 charges have been deonated!</color>";

            return true;
        }
    }
}
