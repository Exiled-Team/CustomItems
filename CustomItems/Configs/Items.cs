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

    using Exiled.API.Enums;

    /// <summary>
    /// All item config settings.
    /// </summary>
    public class Items
    {
        /// <summary>
        /// Gets the list of emp greanades.
        /// </summary>
        [Description("The list of EMP grenades.")]
        public List<EmpGrenade> EmpGrenades { get; private set; } = new List<EmpGrenade>
        {
            new EmpGrenade { Type = ItemType.GrenadeFlash },
        };

        /// <summary>
        /// Gets the list of grenade launchers.
        /// </summary>
        [Description("The list of grenade launchers.")]
        public List<GrenadeLauncher> GrenadeLaunchers { get; private set; } = new List<GrenadeLauncher>
        {
            new GrenadeLauncher { Type = ItemType.GunLogicer },
        };

        /// <summary>
        /// Gets the list of implosion grenades.
        /// </summary>
        [Description("The list of implosion grenades.")]
        public List<ImplosionGrenade> ImplosionGrenades { get; private set; } = new List<ImplosionGrenade>
        {
            new ImplosionGrenade { Type = ItemType.GrenadeHE },
        };

        /// <summary>
        /// Gets the list of lethal injections.
        /// </summary>
        [Description("The list of lethal injections.")]
        public List<LethalInjection> LethalInjections { get; private set; } = new List<LethalInjection>
        {
            new LethalInjection { Type = ItemType.Adrenaline },
        };

        /// <summary>
        /// Gets the list of lucky coins.
        /// </summary>
        [Description("The list of lucky coins.")]
        public List<LuckyCoin> LuckyCoins { get; private set; } = new List<LuckyCoin>
        {
            new LuckyCoin { Type = ItemType.Coin },
        };

        /// <summary>
        /// Gets the list of mediGuns.
        /// </summary>
        [Description("The list of mediGuns.")]
        public List<MediGun> MediGuns { get; private set; } = new List<MediGun>
        {
            new MediGun { Type = ItemType.GunFSP9 },
        };

        /// <summary>
        /// Gets the list of Scp127s.
        /// </summary>
        [Description("The list of Scp127s.")]
        public List<Scp127> Scp127s { get; private set; } = new List<Scp127>
        {
            new Scp127 { Type = ItemType.GunCOM18 },
        };

        /// <summary>
        /// Gets the list of Scp1499s.
        /// </summary>
        [Description("The list of Scp1499s.")]
        public List<Scp1499> Scp1499s { get; private set; } = new List<Scp1499>
        {
            new Scp1499 { Type = ItemType.SCP268 },
        };

        /// <summary>
        /// Gets the list of sniper rifles.
        /// </summary>
        [Description("The list of sniper rifles.")]
        public List<SniperRifle> SniperRifle { get; private set; } = new List<SniperRifle>
        {
            new SniperRifle { Type = ItemType.GunE11SR },
        };

        /// <summary>
        /// Gets the list of tranquilizer guns.
        /// </summary>
        [Description("The list of tranquilizer guns.")]
        public List<TranquilizerGun> TranquilizerGun { get; private set; } = new List<TranquilizerGun>
        {
            new TranquilizerGun { Type = ItemType.GunCOM18 },
        };

        /// <summary>
        /// Gets the list of Scp714s.
        /// </summary>
        [Description("The list of Scp714s.")]
        public List<Scp714> Scp714s { get; private set; } = new List<Scp714>
        {
            new Scp714 { Type = ItemType.Coin },
        };

        /// <summary>
        /// Gets the list of Anti-Memetic Pills.
        /// </summary>
        [Description("The list of Anti-Memetic Pills.")]
        public List<AntiMemeticPills> AntiMemeticPills { get; private set; } = new List<AntiMemeticPills>
        {
            new AntiMemeticPills { Type = ItemType.SCP500 },
        };

        /// <summary>
        /// Gets the list of DeflectorSheilds.
        /// </summary>
        [Description("The list of DeflectorSheilds.")]
        public List<DeflectorShield> DeflectorSheilds { get; private set; } = new List<DeflectorShield>
        {
            new DeflectorShield(),
        };

        /// <summary>
        /// Gets the list of <see cref="Scp2818"/>s.
        /// </summary>
        [Description("The list of SCP-2818s.")]
        public List<Scp2818> Scp2818s { get; private set; } = new List<Scp2818>
        {
            new Scp2818 { Type = ItemType.GunE11SR, },
        };

        /// <summary>
        /// Gets the list of C4Charges.
        /// </summary>
        [Description("The list of C4Charges.")]
        public List<C4Charge> C4Charges { get; private set; } = new List<C4Charge>
        {
            new C4Charge { Type = ItemType.GrenadeHE },
        };

        /// <summary>
        /// Gets the list of AutoGuns.
        /// </summary>
        [Description("The list of AutoGuns.")]
        public List<AutoGun> AutoGuns { get; private set; } = new List<AutoGun>
        {
            new AutoGun { Type = ItemType.GunCOM15 },
        };
    }
}
