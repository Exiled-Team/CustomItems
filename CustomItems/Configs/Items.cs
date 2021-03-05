// -----------------------------------------------------------------------
// <copyright file="Items.cs" company="Galaxy199 and iopietro">
// Copyright (c) Galaxy199 and iopietro. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable SA1200

using CustomItems.Items;

namespace CustomItems.Configs
{
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// All item config settings.
    /// </summary>
    public class Items
    {
        /// <summary>
        /// Gets the list of emp greanades.
        /// </summary>
        [Description("The list of SCP2818s.")]
        public List<SCP2818> SCP2818 { get; private set; } = new List<SCP2818>
        {
            new SCP2818 { Type = ItemType.GunE11SR },
        };

      
    }
}