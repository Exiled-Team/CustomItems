using System.Collections.Generic;
using System.ComponentModel;
using CustomItems.API;

namespace CustomItems.ItemConfigs
{
    public class LuckCoinConfig
    {
        [Description("Where on the map items should spawn, and their % chance of spawning in each location.")]
        public Dictionary<SpawnLocation, float> SpawnLocations { get; set; } = new Dictionary<SpawnLocation, float>();

        [Description("The Custom Item ID for this item.")]
        public int Id { get; set; } = 5;
        
        [Description("The description of this item show to players when they obtain it.")]
        public string Description { get; set; } = "This coin has magical properties when it is dropped inside of SCP-106's pocket dimension.";

        [Description("The name of this item shown to players when they obtain it.")]
        public string Name { get; set; } = "LC-119";
    }
}