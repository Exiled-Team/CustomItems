using System.Collections.Generic;
using System.ComponentModel;
using CustomItems.API;

namespace CustomItems.ItemConfigs
{
    public class LethalInjectionConfig
    {
        [Description("Where on the map items should spawn, and their % chance of spawning in each location.")]
        public Dictionary<SpawnLocation, float> SpawnLocations { get; set; } = new Dictionary<SpawnLocation, float>();
        
        [Description("Whether the Lethal Injection shoudl always kill the user, regardless of if they stop SCP-096's enrage.")]
        public bool KillOnFail { get; set; } = true;

        [Description("The Custom Item ID for this item.")]
        public int Id { get; set; } = 4;
        
        [Description("The description of this item show to players when they obtain it.")]
        public string Description { get; set; } = "This is a Lethal Injection that, when used, will cause SCP-096 to immediately leave his enrage, regardless of how many targets he currently has, if you are one of his current targets. You always die when using this, even if there's no enrage to break, or you are not a target.";

        [Description("The name of this item shown to players when they obtain it.")]
        public string Name { get; set; } = "LJ-119";
    }
}