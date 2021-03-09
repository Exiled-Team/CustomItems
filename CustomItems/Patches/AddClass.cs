// -----------------------------------------------------------------------
// <copyright file="AddClass.cs" company="Galaxy119 and iopietro">
// Copyright (c) Galaxy119 and iopietro. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems.Patches
{
    using Events;
    using Exiled.API.Features;
    using HarmonyLib;
    using Subclass;

    /// <summary>
    /// Patch for <see cref="TrackingAndMethods.AddClass"/>.
    /// </summary>
    internal static class AddClass
    {
        /// <summary>
        /// Postfix - Called after base method finishes.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> being changed.</param>
        /// <param name="subClass">The <see cref="SubClass"/> being added.</param>
        /// <param name="is035">Whether or not they are SCP-035.</param>
        /// <param name="lite">Whether or not it is lite.</param>
        /// <param name="escaped">Whether or not it is an escape.</param>
        /// <param name="disguised">Whether or not the player is disguised.</param>
        public static void Postfix(Player player, SubClass subClass, bool is035 = false, bool lite = false, bool escaped = false, bool disguised = false)
        {
            AddClassEventArgs ev = new AddClassEventArgs(player, subClass);
            AddClassEvent.OnAddingClass(ev);
        }
    }
}