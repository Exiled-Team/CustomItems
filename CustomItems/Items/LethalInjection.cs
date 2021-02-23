// <copyright file="LethalInjection.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CustomItems.Items
{
    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;
    using Exiled.CustomItems.API.Spawn;
    using Exiled.Events.EventArgs;
    using MEC;
    using PlayableScps;
    using Player = Exiled.Events.Handlers.Player;
    using Scp096 = PlayableScps.Scp096;

    /// <inheritdoc />
    public class LethalInjection : CustomItem
    {
        /// <inheritdoc />
        public LethalInjection(ItemType type, uint itemId)
            : base(type, itemId)
        {
        }

        /// <inheritdoc/>
        public override string Name { get; } = Plugin.Singleton.Config.ItemConfigs.LethalCfg.Name;

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; set; } = Plugin.Singleton.Config.ItemConfigs.LethalCfg.SpawnProperties;

        /// <inheritdoc/>
        public override string Description { get; } = Plugin.Singleton.Config.ItemConfigs.LethalCfg.Description;

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            Player.UsingMedicalItem += OnMedicalItemUsed;
            base.SubscribeEvents();
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            Player.UsingMedicalItem -= OnMedicalItemUsed;
            base.UnsubscribeEvents();
        }

        private void OnMedicalItemUsed(UsingMedicalItemEventArgs ev)
        {
            Log.Debug($"{ev.Player.Nickname} used a medical item: {ev.Item}", Plugin.Singleton.Config.Debug);
            if (!Check(ev.Player.CurrentItem))
                return;

            Timing.CallDelayed(1.5f, () =>
            {
                Log.Debug($"{ev.Player.Nickname} used a {Name}", Plugin.Singleton.Config.Debug);
                foreach (Exiled.API.Features.Player player in Exiled.API.Features.Player.List)
                    if (player.Role == RoleType.Scp096)
                    {
                        Log.Debug($"{ev.Player.Nickname} - {Name} found an 096: {player.Nickname}", Plugin.Singleton.Config.Debug);
                        if (!(player.CurrentScp is Scp096 scp096))
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