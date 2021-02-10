// <copyright file="ItemConfigs.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CustomItems.Configs
{
    using System.ComponentModel;
    using CustomItems.ItemConfigs;

    /// <summary>
    /// Item Configs.
    /// </summary>
    public class ItemConfigs
    {
        /// <summary>
        /// Gets or sets EMP Grenade Configs.
        /// </summary>
        [Description("EMP Grenade configs.")]
        public EmpGrenadeConfig EmpCfg { get; set; } = new EmpGrenadeConfig();

        /// <summary>
        /// Gets or sets Grenade Launcher configs.
        /// </summary>
        [Description("Grenade Launcher configs.")]
        public GrenadeLauncherConfig GlCfg { get; set; } = new GrenadeLauncherConfig();

        /// <summary>
        /// Gets or sets Implosion Grenade configs.
        /// </summary>
        [Description("Implosion Grenade configs.")]
        public ImplosionGrenadeConfig ImpCfg { get; set; } = new ImplosionGrenadeConfig();

        /// <summary>
        /// Gets or sets Lethal Injection configs.
        /// </summary>
        [Description("Lethal Injection configs.")]
        public LethalInjectionConfig LethalCfg { get; set; } = new LethalInjectionConfig();

        /// <summary>
        /// Gets or sets Lucky Coin Configs.
        /// </summary>
        [Description("Lucky Coin configs.")]
        public LuckCoinConfig LuckyCfg { get; set; } = new LuckCoinConfig();

        /// <summary>
        /// Gets or sets Medi-gun configs.
        /// </summary>
        [Description("Medi-Gun configs.")]
        public MediGunConfig MediCfg { get; set; } = new MediGunConfig();

        /// <summary>
        /// Gets or sets SCP-127 Configs.
        /// </summary>
        [Description("SCP-127 configs.")]
        public Scp127Config Scp127Cfg { get; set; } = new Scp127Config();

        /// <summary>
        /// Gets or sets Shotgun Configs.
        /// </summary>
        [Description("Shotgun configs.")]
        public ShotgunConfig ShotgunCfg { get; set; } = new ShotgunConfig();

        /// <summary>
        /// Gets or sets Sniper Rifle Configs.
        /// </summary>
        [Description("Sniper Rifle configs.")]
        public SniperRifleConfig SniperCfg { get; set; } = new SniperRifleConfig();

        /// <summary>
        /// Gets or sets TranqGun configs.
        /// </summary>
        [Description("Tranquilizer Gun configs.")]
        public TranqGunConfig TranqCfg { get; set; } = new TranqGunConfig();

        /// <summary>
        /// Gets or sets SCP-1499 configs.
        /// </summary>
        [Description("SCP-1499 configs.")]
        public Scp1499Config Scp1499Cfg { get; set; } = new Scp1499Config();

        /// <summary>
        /// Gets or sets Rock configs.
        /// </summary>
        [Description("Rock configs.")]
        public RockConfig RockCfg { get; set; } = new RockConfig();
    }
}
