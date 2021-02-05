using System.Collections.Generic;
using CustomItems.API;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using PlayableScps;

namespace CustomItems.Items
{
    public class LethalInjection : CustomItem
    {
        public LethalInjection(ItemType type, int itemId) : base(type, itemId)
        {
        }

        public override string Name { get; set; } = Plugin.Singleton.Config.ItemConfigs.LethalCfg.Name;
        public override Dictionary<SpawnLocation, float> SpawnLocations { get; set; } = Plugin.Singleton.Config.ItemConfigs.LethalCfg.SpawnLocations;
        public override string Description { get; set; } = Plugin.Singleton.Config.ItemConfigs.LethalCfg.Description;
        public override int SpawnLimit { get; set; } = Plugin.Singleton.Config.ItemConfigs.LethalCfg.SpawnLimit;

        protected override void LoadEvents()
        {
            Exiled.Events.Handlers.Player.MedicalItemUsed += OnMedicalItemUsed;
        }

        protected override void UnloadEvents()
        {
            Exiled.Events.Handlers.Player.MedicalItemUsed -= OnMedicalItemUsed;
        }

        private void OnMedicalItemUsed(UsedMedicalItemEventArgs ev)
        {
            if (CheckItem(ev.Player.CurrentItem))
            {
                foreach (Player player in Player.List)
                    if (player.Role == RoleType.Scp096)
                    {
                        if (player.CurrentScp is PlayableScps.Scp096 scp096)
                        {
                            if (scp096.HasTarget(ev.Player.ReferenceHub) &&
                                scp096.PlayerState == Scp096PlayerState.Enraged ||
                                scp096.PlayerState == Scp096PlayerState.Enraging ||
                                scp096.PlayerState == Scp096PlayerState.Attacking)
                            {
                                scp096.ResetEnrage();
                                ev.Player.Kill(DamageTypes.Poison);
                                return;
                            }
                        }
                    }
                
                if (Plugin.Singleton.Config.ItemConfigs.LethalCfg.KillOnFail)
                    ev.Player.Kill(DamageTypes.Poison);
            }
        }
    }
}