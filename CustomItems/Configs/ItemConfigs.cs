using CustomItems.ItemConfigs;

namespace CustomItems.Configs
{
    public class ItemConfigs
    {
        public EmpGrenadeConfig EmpCfg { get; set; } = new EmpGrenadeConfig();
        public SniperRifleConfig SniperCfg { get; set; } = new SniperRifleConfig();
        public ShotgunConfig ShotgunCfg { get; set; } = new ShotgunConfig();
        public Scp127Config Scp127Cfg { get; set; } = new Scp127Config();
        public MediGunConfig MediCfg { get; set; } = new MediGunConfig();
    }
}