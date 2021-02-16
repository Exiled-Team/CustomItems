// <copyright file="AddClassEvent.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

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