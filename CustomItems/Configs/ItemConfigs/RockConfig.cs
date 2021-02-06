using System.ComponentModel;

namespace CustomItems.ItemConfigs
{
    public class RockConfig
    {
        [Description("How much damage is done when hit with a rock in melee.")]
        public float HitDamage { get; set; } = 10f;
        
        [Description("How much damage is done when hit with a thrown rock.")]
        public float ThrownDamage { get; set; } = 20f;
        
        [Description("How fast rocks can be thrown.")]
        public float ThrowSpeed { get; set; } = 9f;
        
        [Description("Whether or not rocks will deal damage to friendly targets.")]
        public bool FriendlyFire { get; set; } = false;

        [Description("The Custom Item ID for this item.")]
        public int Id { get; set; } = 11;
        
        [Description("The name of this item shown to players when they obtain it.")]
        public string Name { get; set; } = "Rock";
        
        [Description("The description of this item show to players when they obtain it.")]
        public string Description { get; set; } = "It's a rock.";
        
        [Description("How many of this item are allowed to naturally spawn on the map when a round starts. 0 = unlimited")]
        public int SpawnLimit { get; set; } = 1;
    }
}