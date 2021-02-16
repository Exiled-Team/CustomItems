// <copyright file="LethalInjection.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CustomItems.Items
{
    using System.Collections.Generic;
    using Exiled.API.Features;
    using Exiled.CustomItems.API;
    using Exiled.Events.EventArgs;
    using MEC;
    using PlayableScps;

    /// <inheritdoc />
    public class LethalInjection : CustomItem
    {
        /// <inheritdoc />
        public LethalInjection(ItemType type, int itemId)
            : base(type, itemId)
        {
        }

        /// <inheritdoc/>
        public override string Name { get; set; } = Plugin.Singleton.Config.ItemConfigs.LethalCfg.Name;

        /// <inheritdoc/>
        public override Dictionary<SpawnLocation, float> SpawnLocations { get; set; } = Plugin.Singleton.Config.ItemConfigs.LethalCfg.SpawnLocations;

        /// <inheritdoc/>
        public override string Description { get; set; } = Plugin.Singleton.Config.ItemConfigs.LethalCfg.Description;

        /// <inheritdoc/>
        public override int SpawnLimit { get; set; } = Plugin.Singleton.Config.ItemConfigs.LethalCfg.SpawnLimit;

        /// <inheritdoc/>
        protected override void LoadEvents()
        {
            Exiled.Events.Handlers.Player.UsingMedicalItem += OnMedicalItemUsed;
        }

        /// <inheritdoc/>
        protected override void UnloadEvents()
        {
            Exiled.Events.Handlers.Player.UsingMedicalItem -= OnMedicalItemUsed;
        }

        private void OnMedicalItemUsed(UsingMedicalItemEventArgs ev)
        {
            Log.Debug($"{ev.Player.Nickname} used a medical item: {ev.Item}", Plugin.Singleton.Config.Debug);
            if (!CheckItem(ev.Player.CurrentItem))
                return;

            Timing.CallDelayed(1.5f, () =>
            {
                Log.Debug($"{ev.Player.Nickname} used a {Name}", Plugin.Singleton.Config.Debug);
                foreach (Player player in Player.List)
                    if (player.Role == RoleType.Scp096)
                    {
                        Log.Debug($"{ev.Player.Nickname} - {Name} found an 096: {player.Nickname}", Plugin.Singleton.Config.Debug);
                        if (!(player.CurrentScp is PlayableScps.Scp096 scp096))
                            continue;

                        Log.Debug($"{player.Nickname} 096 component found.", Plugin.Singleton.Config.Debug);
                        if ((!scp096.HasTarget(ev.Player.ReferenceHub) ||
                             scp096.PlayerState != Scp096PlayerState.Enraged) &&
                            scp096.PlayerState != Scp096PlayerState.Enraging &&
                            scp096.PlayerState != Scp096PlayerState.Attacking)
                            continue;

                        Log.Debug($"{player.Nickname} 096 checks passed.", Plugin.Singleton.Config.Debug);
                        scp096.ResetEnrage();
                        ev.Player.Kill(DamageTypes.Poison);
                        return;
                    }

                if (!Plugin.Singleton.Config.ItemConfigs.LethalCfg.KillOnFail)
                    return;

                Log.Debug($"{Name} kill on fail: {ev.Player.Nickname}", Plugin.Singleton.Config.Debug);
                ev.Player.Kill(DamageTypes.Poison);
            });

            ev.Player.RemoveItem(ev.Player.CurrentItem);
            ev.IsAllowed = false;
        }
    }
}