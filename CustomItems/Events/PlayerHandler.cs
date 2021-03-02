using System;
using System.Collections.Generic;
using Exiled.CustomItems.API.Features;
using MEC;

namespace CustomItems.Events
{
    using static CustomItems;

    /// <summary>
    /// Event Handlers.
    /// </summary>
    public class PlayerHandler
    {
        /// <summary>
        /// AddingClass handler.
        /// </summary>
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