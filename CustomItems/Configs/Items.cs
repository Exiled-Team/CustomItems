// -----------------------------------------------------------------------
// <copyright file="Items.cs" company="Galaxy119 and iopietro">
// Copyright (c) Galaxy119 and iopietro. All rights reserved.
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
        public EmpGrenade EmpGrenade { get; set; }

        /// <summary>
        /// Gets the list of grenade launchers.
        /// </summary>
        public GrenadeLauncher GrenadeLauncher { get; set; }

        /// <summary>
        /// Gets the list of implosion grenades.
        /// </summary>
        public ImplosionGrenade ImplosionGrenade { get; set; }

        /// <summary>
        /// Gets the list of lethal injections.
        /// </summary>
        public LethalInjection LethalInjection { get; set; }

        /// <summary>
        /// Gets the list of lucky coins.
        /// </summary>
        public LuckyCoin LuckyCoin { get; set; }

        /// <summary>
        /// Gets the list of mediGuns.
        /// </summary>
        public MediGun MediGun { get; set; }

        /// <summary>
        /// Gets the list of Scp127s.
        /// </summary>
        public Scp127 Scp127 { get; set; }

        /// <summary>
        /// Gets the list of Scp1499s.
        /// </summary>
        public Scp1499 Scp1499 { get; set; }

        /// <summary>
        /// Gets the list of sniper rifles.
        /// </summary>
        public SniperRifle SniperRifle { get; set; }

        /// <summary>
        /// Gets the list of tranquilizer guns.
        /// </summary>
        public TranquilizerGun TranquilizerGun { get; set; }

        /// <summary>
        /// Gets the list of Scp714s.
        /// </summary>
        public Scp714 Scp714 { get; set; }

        /// <summary>
        /// Gets the list of Anti-Memetic Pills.
        /// </summary>
        public AntiMemeticPills AntiMemeticPills { get; set; }

        /// <summary>
        /// Gets the list of DeflectorSheilds.
        /// </summary>
        public DeflectorShield DeflectorShield { get; set; }

        /// <summary>
        /// Gets the list of <see cref="Scp2818"/>s.
        /// </summary>
        public Scp2818 Scp2818 { get; set; }

        /// <summary>
        /// Gets the list of C4Charges.
        /// </summary>
        public C4Charge C4Charge { get; set; }

        /// <summary>
        /// Gets the list of AutoGuns.
        /// </summary>
        public AutoGun AutoGun { get; set; }
    }
}
