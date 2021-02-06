using System.Collections.Generic;
using System.ComponentModel;
using CustomItems.API;

namespace CustomItems.ItemConfigs
{
    public class Scp1499Config
    {
        [Description("How long the SCP-1499 can be wore, before automaticly player takes it off. (set to 0 for no limit)")]
        public float Duration { get; set; } = 15f;
        
        [Description("Where on the map items should spawn, and their % chance of spawning in each location.")]
        public Dictionary<SpawnLocation, float> SpawnLocations { get; set; } = new Dictionary<SpawnLocation, float>
        {
            { SpawnLocation.InsideHid, 10 }
        };

        [Description("The Custom Item ID for this item.")]
        public int Id { get; set; } = 12;

        [Description("The description of this item show to players when they obtain it.")]
        public string Description { get; set; } = "The gas mask that temporarily teleports you to another dimension, when you put it on.";

        [Description("The name of this item shown to players when they obtain it.")]
        public string Name { get; set; } = "SCP-1499";

        [Description("How many of this item are allowed to naturally spawn on the map when a round starts. 0 = unlimited")]
        public int SpawnLimit { get; set; } = 1;
    }
}
