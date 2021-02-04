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

        public override string ItemName { get; set; } = "LI-119";
        public override Dictionary<SpawnLocation, float> SpawnLocations { get; set; } =
            Plugin.Singleton.Config.ItemConfigs.LethalCfg.SpawnLocations;
        protected override string ItemDescription { get; set; } =
            "This is a Lethal Injection that, when used, will cause SCP-096 to immediately leave his enrage, regardless of how many targets he currently has, if you are one of his current targets. You always die when using this, even if there's no enrage to break, or you are not a target.";

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