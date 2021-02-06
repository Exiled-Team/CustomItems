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
    }
}