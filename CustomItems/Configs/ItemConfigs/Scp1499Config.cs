using System.Collections.Generic;
using System.ComponentModel;
using CustomItems.API;

namespace CustomItems.ItemConfigs
{
    public class Scp1499Config
    {
        [Description("Where on the map items should spawn, and their % chance of spawning in each location.")]
        public Dictionary<SpawnLocation, float> SpawnLocations { get; set; } = new Dictionary<SpawnLocation, float>
        {
            { SpawnLocation.Inside173Armory, 100 }
        };

        [Description("The Custom Item ID for this item.")]
        public int Id { get; set; } = 11;

        [Description("The description of this item show to players when they obtain it.")]
        public string Description { get; set; } = "The gas mask that teleports you to another dimmension, when you put it on.";

        [Description("The name of this item shown to players when they obtain it.")]
        public string Name { get; set; } = "SCP-1499";

        [Description("How many of this item are allowed to naturally spawn on the map when a round starts. 0 = unlimited")]
        public int SpawnLimit { get; set; } = 1;
    }
}