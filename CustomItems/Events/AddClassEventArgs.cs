// -----------------------------------------------------------------------
// <copyright file="AddClassEventArgs.cs" company="Galaxy199 and iopietro">
// Copyright (c) Galaxy199 and iopietro. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems.Events
{
    using System;
    using Exiled.API.Features;
    using Subclass;

    /// <summary>
    /// The <see cref="EventArgs"/> for the <see cref="AddClassEvent"/>.
    /// </summary>
    public class AddClassEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddClassEventArgs"/> class.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> triggering the event.</param>
        /// <param name="subClass">The <see cref="SubClass"/> being assigned.</param>
        public AddClassEventArgs(Player player, SubClass subClass)
        {
            Player = player;
            Subclass = subClass;
        }

        /// <summary>
        /// Gets the <see cref="Player"/>.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the <see cref="SubClass"/>.
        /// </summary>
        public SubClass Subclass { get; }
    }
}