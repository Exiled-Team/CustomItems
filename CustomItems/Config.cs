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
            }
        };
        
        [Description("A list of each item and the subclasses that can spawn with it, and the % chance of them receiving it. **This is only used if Advanced Subclassing is installed!**")]
        public Dictionary<string, string> SubclassList { get; set; } = new Dictionary<string, string>();

        public bool GrenadeLauncherEnabled { get; set; } = true;
    }
}