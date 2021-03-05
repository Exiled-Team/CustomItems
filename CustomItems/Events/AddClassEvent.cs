// -----------------------------------------------------------------------
// <copyright file="AddClassEvent.cs" company="Galaxy199 and iopietro">
// Copyright (c) Galaxy199 and iopietro. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems.Events
{
    using Exiled.Events.Extensions;
    using Subclass;

    /// <summary>
    /// Event for <see cref="TrackingAndMethods.AddClass"/>.
    /// </summary>
    public class AddClassEvent
    {
        /// <summary>
        /// The event.
        /// </summary>
        public static event Exiled.Events.Events.CustomEventHandler<AddClassEventArgs> AddClass;

        /// <summary>
        /// The method for calling the event safely.
        /// </summary>
        /// <param name="ev"><see cref="AddClassEventArgs"/>.</param>
        public static void OnAddingClass(AddClassEventArgs ev) => AddClass.InvokeSafely(ev);
    }
}