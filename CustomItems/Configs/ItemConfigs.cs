using CustomItems.ItemConfigs;

namespace CustomItems.Configs
{
    public class ItemConfigs
    {
        public EmpGrenadeConfig EmpCfg { get; set; } = new EmpGrenadeConfig();
        public GrenadeLauncherConfig GlCfg { get; set; } = new GrenadeLauncherConfig();
        public ImplosionGrenadeConfig ImpCfg { get; set; } = new ImplosionGrenadeConfig();
        public LethalInjectionConfig LethalCfg { get; set; } = new LethalInjectionConfig();
        public LuckCoinConfig LuckyCfg { get; set; } = new LuckCoinConfig();
        public MediGunConfig MediCfg { get; set; } = new MediGunConfig();
        public Scp127Config Scp127Cfg { get; set; } = new Scp127Config();
        public ShotgunConfig ShotgunCfg { get; set; } = new ShotgunConfig();
        public SniperRifleConfig SniperCfg { get; set; } = new SniperRifleConfig();
        public TranqGunConfig TranqCfg { get; set; } = new TranqGunConfig();
    }
}