// -----------------------------------------------------------------------
// <copyright file="Items.cs" company="Joker119">
// Copyright (c) Joker119. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable SA1200

using CustomItems.Items;

namespace CustomItems.Configs;

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
    [Description("The list of EMP grenades.")]
    public List<EmpGrenade> EmpGrenades { get; private set; } = new()
    {
        new EmpGrenade(),
    };

    /// <summary>
    /// Gets the list of grenade launchers.
    /// </summary>
    [Description("The list of grenade launchers.")]
    public List<GrenadeLauncher> GrenadeLaunchers { get; private set; } = new()
    {
        new GrenadeLauncher(),
    };

    /// <summary>
    /// Gets the list of implosion grenades.
    /// </summary>
    [Description("The list of implosion grenades.")]
    public List<ImplosionGrenade> ImplosionGrenades { get; private set; } = new()
    {
        new ImplosionGrenade(),
    };

    /// <summary>
    /// Gets the list of lethal injections.
    /// </summary>
    [Description("The list of lethal injections.")]
    public List<LethalInjection> LethalInjections { get; private set; } = new()
    {
        new LethalInjection(),
    };

    /// <summary>
    /// Gets the list of lucky coins.
    /// </summary>
    [Description("The list of lucky coins.")]
    public List<LuckyCoin> LuckyCoins { get; private set; } = new()
    {
        new LuckyCoin(),
    };

    /// <summary>
    /// Gets the list of mediGuns.
    /// </summary>
    [Description("The list of mediGuns.")]
    public List<MediGun> MediGuns { get; private set; } = new()
    {
        new MediGun(),
    };

    /// <summary>
    /// Gets the list of Scp127s.
    /// </summary>
    [Description("The list of Scp127s.")]
    public List<Scp127> Scp127s { get; private set; } = new()
    {
        new Scp127(),
    };

    /// <summary>
    /// Gets the list of Scp1499s.
    /// </summary>
    [Description("The list of Scp1499s.")]
    public List<Scp1499> Scp1499s { get; private set; } = new()
    {
        new Scp1499(),
    };

    /// <summary>
    /// Gets the list of sniper rifles.
    /// </summary>
    [Description("The list of sniper rifles.")]
    public List<SniperRifle> SniperRifle { get; private set; } = new()
    {
        new SniperRifle(),
    };

    /// <summary>
    /// Gets the list of tranquilizer guns.
    /// </summary>
    [Description("The list of tranquilizer guns.")]
    public List<TranquilizerGun> TranquilizerGun { get; private set; } = new()
    {
        new TranquilizerGun(),
    };

    /// <summary>
    /// Gets the list of Scp714s.
    /// </summary>
    [Description("The list of Scp714s.")]
    public List<Scp714> Scp714s { get; private set; } = new()
    {
        new Scp714(),
    };

    /// <summary>
    /// Gets the list of Anti-Memetic Pills.
    /// </summary>
    [Description("The list of Anti-Memetic Pills.")]
    public List<AntiMemeticPills> AntiMemeticPills { get; private set; } = new()
    {
        new AntiMemeticPills(),
    };

    /// <summary>
    /// Gets the list of DeflectorSheilds.
    /// </summary>
    [Description("The list of DeflectorSheilds.")]
    public List<DeflectorShield> DeflectorSheilds { get; private set; } = new()
    {
        new DeflectorShield(),
    };

    /// <summary>
    /// Gets the list of <see cref="Scp2818"/>s.
    /// </summary>
    [Description("The list of SCP-2818s.")]
    public List<Scp2818> Scp2818s { get; private set; } = new()
    {
        new Scp2818(),
    };

    /// <summary>
    /// Gets the list of C4Charges.
    /// </summary>
    [Description("The list of C4Charges.")]
    public List<C4Charge> C4Charges { get; private set; } = new()
    {
        new C4Charge(),
    };

    /// <summary>
    /// Gets the list of AutoGuns.
    /// </summary>
    [Description("The list of AutoGuns.")]
    public List<AutoGun> AutoGuns { get; private set; } = new()
    {
        new AutoGun(),
    };
}