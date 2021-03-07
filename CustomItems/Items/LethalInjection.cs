// -----------------------------------------------------------------------
// <copyright file="LethalInjection.cs" company="Galaxy119 and iopietro">
// Copyright (c) Galaxy119 and iopietro. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems.Items
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using CustomPlayerEffects;
    using Exiled.API.Features;
    using Exiled.CustomItems.API;
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
        /// <inheritdoc/>
        public override uint Id { get; set; } = 3;

        /// <inheritdoc/>
        public override string Name { get; set; } = "LJ-119";

        /// <inheritdoc/>
        public override string Description { get; set; } = "This is a Lethal Injection that, when used, will cause SCP-096 to immediately leave his enrage, regardless of how many targets he currently has, if you are one of his current targets. You always die when using this, even if there's no enrage to break, or you are not a target.";

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new DynamicSpawnPoint
                {
                    Chance = 100,
                    Location = SpawnLocation.Inside096,
                },
            },
        };

        /// <summary>
        /// Gets or sets a value indicating whether the Lethal Injection should always kill the user, regardless of if they stop SCP-096's enrage.
        /// </summary>
        [Description("Whether the Lethal Injection should always kill the user, regardless of if they stop SCP-096's enrage.")]
        public bool KillOnFail { get; set; } = true;

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            Player.UsingMedicalItem += OnUsingMedicalItem;

            base.SubscribeEvents();
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            Player.UsingMedicalItem -= OnUsingMedicalItem;

            base.UnsubscribeEvents();
        }

        private void OnUsingMedicalItem(UsingMedicalItemEventArgs ev)
        {
            Log.Debug($"{ev.Player.Nickname} used a medical item: {ev.Item}", CustomItems.Instance.Config.IsDebugEnabled);
            if (!Check(ev.Player.CurrentItem))
                return;

            Timing.CallDelayed(1.5f, () =>
            {
                Log.Debug($"{ev.Player.Nickname} used a {Name}", CustomItems.Instance.Config.IsDebugEnabled);
                foreach (Exiled.API.Features.Player player in Exiled.API.Features.Player.List)
                    if (player.Role == RoleType.Scp096)
                    {
                        Log.Debug($"{ev.Player.Nickname} - {Name} found an 096: {player.Nickname}", CustomItems.Instance.Config.IsDebugEnabled);
                        if (!(player.CurrentScp is Scp096 scp096))
                            continue;

                        Log.Debug($"{player.Nickname} 096 component found.", CustomItems.Instance.Config.IsDebugEnabled);
                        if ((!scp096.HasTarget(ev.Player.ReferenceHub) ||
                             scp096.PlayerState != Scp096PlayerState.Enraged) &&
                            scp096.PlayerState != Scp096PlayerState.Enraging &&
                            scp096.PlayerState != Scp096PlayerState.Attacking)
                            continue;

                        Log.Debug($"{player.Nickname} 096 checks passed.", CustomItems.Instance.Config.IsDebugEnabled);
                        scp096.EndEnrage();
                        ev.Player.Kill(DamageTypes.Poison);
                        return;
                    }

                if (!KillOnFail)
                {
                    if (ev.Player.AdrenalineHealth > 30)
                        ev.Player.AdrenalineHealth -= 30;
                    else
                        ev.Player.AdrenalineHealth = 0;
                    ev.Player.DisableEffect<Invigorated>();
                    return;
                }

                Log.Debug($"{Name} kill on fail: {ev.Player.Nickname}", CustomItems.Instance.Config.IsDebugEnabled);
                ev.Player.Kill(DamageTypes.Poison);
            });

            ev.Player.RemoveItem(ev.Player.CurrentItem);
        }
    }
}