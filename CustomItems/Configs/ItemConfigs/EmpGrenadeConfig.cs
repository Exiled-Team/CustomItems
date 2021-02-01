using System.Collections.Generic;

namespace CustomItems.WeaponConfigs
{
    public class EmpGrenadeConfig
    {
        public float Duration { get; set; } = 20;
        public Dictionary<string, float> SpawnLocations { get; set; } = new Dictionary<string, float>
        {
            {
                "SCP-173", 100
            }
        };
    }
}