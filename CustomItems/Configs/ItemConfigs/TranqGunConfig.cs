using System.Collections.Generic;
using System.ComponentModel;
using CustomItems.API;

namespace CustomItems.ItemConfigs
{
    public class TranqGunConfig
    {
        [Description("The number of rounds usable after a single reload.")]
        public int ClipSize { get; set; } = 2;
        
        [Description("The base weapon this one is modeled after.")]
        public ItemType ItemType { get; set; } = ItemType.GunUSP;
        
        [Description("Where on the map items should spawn, and their % chance of spawning in each location.")]
        public Dictionary<SpawnLocation, float> SpawnLocations { get; set; } = new Dictionary<SpawnLocation, float>
        {
            { SpawnLocation.InsideGr18, 50 }, 
            { SpawnLocation.Inside173Armory, 80}
        };
        
        [Description("Whether or not SCPs should be resistant to tranquilizers. (Being resistant gives them a chance to not be tranquilized when shot).")]
        public bool ResistantScps { get; set; } = true;
        
        [Description("The amount of time a successful tranquilization lasts for.")]
        public float Duration { get; set; } = 5f;
        
        [Description("Everytime a player is tranquilized, they gain a resistance to further tranquilizations, reducing the duration of future effects. This number signifies the exponential modifier used to determine how much time is removed from the effect.")]
        public int ResistanceModifier { get; set; } = 2;
        
        [Description("Whether or not tranquilized targets should drop all of their items.")]
        public bool DropItems { get; set; } = true;
        
        [Description("The percent chance an SCP will resist being tranquilized. This has no effect if ResistantScps is false.")]
        public int ScpResistChance { get; set; } = 40;

        [Description("The Custom Item ID for this item.")]
        public int Id { get; set; } = 10;
        
        [Description("The description of this item show to players when they obtain it.")]
        public string Description { get; set; } = "This modifier USP fires non-lethal tranquilizing darts. Those affected will be rendered unconscious for a short duration. Unreliable against SCPs. Repeated tranquilizing of the same person will render them resistant to it's effect.";

        [Description("The name of this item shown to players when they obtain it.")]
        public string Name { get; set; } = "TG-119";
        
        [Description("How many of this item are allowed to naturally spawn on the map when a round starts. 0 = unlimited")]
        public int SpawnLimit { get; set; } = 1;
    }
}