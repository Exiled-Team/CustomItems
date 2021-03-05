namespace CustomItems.Items
{
    using System.Collections.Generic;
    using Exiled.API.Features;
    using Exiled.CustomItems.API;
    using Exiled.CustomItems.API.Features;
    using Exiled.CustomItems.API.Spawn;
    using Exiled.Events.EventArgs;
    using YamlDotNet.Serialization;

    /// <summary>
    /// A gun that kills you.
    /// </summary>
    public class Scp2818 : CustomWeapon
    {
        /// <inheritdoc/>
        public override uint Id { get; set; } = 14;

        /// <inheritdoc/>
        public override string Name { get; set; } = "SCP-2818";

        /// <inheritdoc/>
        public override string Description { get; set; } =
            "When this weapon is fired, it uses the biomass of the shooter as the bullet.";

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new DynamicSpawnPoint
                {
                    Chance = 100,
                    Location = SpawnLocation.InsideHid,
                },
                new DynamicSpawnPoint
                {
                    Chance = 40,
                    Location = SpawnLocation.InsideHczArmory,
                },
            },
        };

        /// <inheritdoc/>
        public override Modifiers Modifiers { get; set; } = new Modifiers(3, 4, 0);

        /// <inheritdoc/>
        [YamlIgnore]
        public override float Damage { get; set; } = float.MaxValue;

        /// <inheritdoc/>
        protected override void OnShooting(ShootingEventArgs ev)
        {
            if (Player.Get(ev.Target) == null)
                ev.Shooter.Kill(DamageTypes.Contain);
        }

        /// <inheritdoc/>
        protected override void OnHurting(HurtingEventArgs ev)
        {
            ev.Attacker.Kill(DamageTypes.Nuke);
            ev.Target.Kill(DamageTypes.Nuke);
        }
    }
}