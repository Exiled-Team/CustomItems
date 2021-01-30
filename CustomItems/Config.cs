using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Interfaces;

namespace CustomItems
{
    public class Config : IConfig
    {
        [Description("Whether or not this plugin is enabled.")]
        public bool IsEnabled { get; set; } = true;

        [Description("Whether or not Sniper Rifles are to be used.")]
        public bool SniperEnabled { get; set; } = true;

        [Description("The damage multiplier to apply to Sniper Rifles.")]
        public float SniperDmgMult { get; set; } = 1.5f;
        
        [Description("A list of subclasses that should receive snipers when they spawn.")]
        public List<string> SniperList { get; set; } = new List<string>();

        public bool Debug { get; set; } = false;

        [Description("Which items are enabled.")]
        public Dictionary<string, bool> EnabledItems { get; set; } = new Dictionary<string, bool>
        {
            {
                "Sniper", true
            },
            {
                "GrenadeLauncher", true
            },
            {
                "Shotgun", true
            }
        };
        
        [Description("A list of each item and the subclasses that can spawn with it, and the % chance of them receiving it. **This is only used if Advanced Subclassing is installed!**")]
        public Dictionary<string, List<string>> SubclassList { get; set; } = new Dictionary<string, List<string>>
        {
            {
                "ExampleSubclass", new List<string>{"ExampleItem1", "ExampleItem2"}
            }
        };

        public bool GrenadeLauncherEnabled { get; set; } = true;
        public int ShotgunSpreadCount { get; set; } = 5;
        public float ShotgunAimCone { get; set; } = 5;
        public float ShotgunHeadDamage { get; set; } = 12.5f;
        public float ShotgunArmDamage { get; set; } = 6.75f;
        public float ShotgunLegDamage { get; set; } = 6.75f;
        public float ShotgunBodyDamage { get; set; } = 13.5f;
    }
}