// -----------------------------------------------------------------------
// <copyright file="PlayerHandler.cs" company="Galaxy199 and iopietro">
// Copyright (c) Galaxy199 and iopietro. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems.Events
{
    using System;
    using System.Collections.Generic;
    using Exiled.CustomItems.API.Features;
    using MEC;
    using static CustomItems;

    /// <summary>
    /// Event Handlers.
    /// </summary>
    public class PlayerHandler
    {
        /// <summary>
        /// AddingClass handler.
        /// </summary>
        /// <param name="ev"><see cref="AddClassEventArgs"/>.</param>
        public void OnAddingSubclass(AddClassEventArgs ev)
        {
            if (!Instance.Config.SubclassItems.TryGetValue(ev.Subclass.Name, out List<Tuple<CustomItem, float>> customItems))
                return;

            foreach ((CustomItem item, float chance) in customItems)
            {
                int r = Instance.Rng.Next(100);
                if (r <= chance)
                    Timing.CallDelayed(1.5f, () => item.Give(ev.Player));
            }
        }
    }
}