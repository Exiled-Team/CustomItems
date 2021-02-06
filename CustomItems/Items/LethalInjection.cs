using System.Collections.Generic;
using CustomItems.API;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using MEC;
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
            Exiled.Events.Handlers.Player.UsingMedicalItem += OnMedicalItemUsed;
        }

        protected override void UnloadEvents()
        {
            Exiled.Events.Handlers.Player.UsingMedicalItem -= OnMedicalItemUsed;
        }

        private void OnMedicalItemUsed(UsingMedicalItemEventArgs ev)
        {
            Log.Debug($"{ev.Player.Nickname} used a medical item: {ev.Item}", Plugin.Singleton.Config.Debug);
            if (CheckItem(ev.Player.CurrentItem))
            {
                Timing.CallDelayed(1.5f, () =>
                {
                    Log.Debug($"{ev.Player.Nickname} used a {Name}", Plugin.Singleton.Config.Debug);
                    foreach (Player player in Player.List)
                        if (player.Role == RoleType.Scp096)
                        {
                            Log.Debug($"{ev.Player.Nickname} - {Name} found an 096: {player.Nickname}", Plugin.Singleton.Config.Debug);
                            if (player.CurrentScp is PlayableScps.Scp096 scp096)
                            {
                                Log.Debug($"{player.Nickname} 096 component found.", Plugin.Singleton.Config.Debug);
                                if (scp096.HasTarget(ev.Player.ReferenceHub) &&
                                    scp096.PlayerState == Scp096PlayerState.Enraged ||
                                    scp096.PlayerState == Scp096PlayerState.Enraging ||
                                    scp096.PlayerState == Scp096PlayerState.Attacking)
                                {
                                    Log.Debug($"{player.Nickname} 096 checks passed.", Plugin.Singleton.Config.Debug);
                                    scp096.ResetEnrage();
                                    ev.Player.Kill(DamageTypes.Poison);
                                    return;
                                }
                            }
                        }

                    if (Plugin.Singleton.Config.ItemConfigs.LethalCfg.KillOnFail)
                    {
                        Log.Debug($"{Name} kill on fail: {ev.Player.Nickname}", Plugin.Singleton.Config.Debug);
                        ev.Player.Kill(DamageTypes.Poison);
                    }
                });
                
                ev.Player.RemoveItem(ev.Player.CurrentItem);
                ev.IsAllowed = false;
            }
        }
    }
}