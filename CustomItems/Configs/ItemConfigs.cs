using System.ComponentModel;
using CustomItems.ItemConfigs;

namespace CustomItems.Configs
{
    public class ItemConfigs
    {
        [Description("EMP Grenade configs.")]
        public EmpGrenadeConfig EmpCfg { get; set; } = new EmpGrenadeConfig();
        
        [Description("Grenade Launcher configs.")]
        public GrenadeLauncherConfig GlCfg { get; set; } = new GrenadeLauncherConfig();
        
        [Description("Implosion Grenade configs.")]
        public ImplosionGrenadeConfig ImpCfg { get; set; } = new ImplosionGrenadeConfig();
        
        [Description("Lethal Injection configs.")]
        public LethalInjectionConfig LethalCfg { get; set; } = new LethalInjectionConfig();
        
        [Description("Lucky Coin configs.")]
        public LuckCoinConfig LuckyCfg { get; set; } = new LuckCoinConfig();
        
        [Description("Medi-Gun configs.")]
        public MediGunConfig MediCfg { get; set; } = new MediGunConfig();
        
        [Description("SCP-127 configs.")]
        public Scp127Config Scp127Cfg { get; set; } = new Scp127Config();
        
        [Description("Shotgun configs.")]
        public ShotgunConfig ShotgunCfg { get; set; } = new ShotgunConfig();
        
        [Description("Sniper Rifle configs.")]
        public SniperRifleConfig SniperCfg { get; set; } = new SniperRifleConfig();
        
        [Description("Tranquilizer Gun configs.")]
        public TranqGunConfig TranqCfg { get; set; } = new TranqGunConfig();
    }
}