// -----------------------------------------------------------------------
// <copyright file="LethalInjection.cs" company="Joker119">
// Copyright (c) Joker119. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems.Items;

using System.Collections.Generic;
using System.ComponentModel;
using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;

using MEC;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp096;
using PlayerStatsSystem;
using Player = Exiled.Events.Handlers.Player;
using Scp096Role = Exiled.API.Features.Roles.Scp096Role;

/// <inheritdoc />
[CustomItem(ItemType.Adrenaline)]
public class LethalInjection : CustomItem
{
    /// <inheritdoc/>
    public override uint Id { get; set; } = 3;

    /// <inheritdoc/>
    public override string Name { get; set; } = "LJ-119";

    /// <inheritdoc/>
    public override string Description { get; set; } = "This is a Lethal Injection that, when used, will cause SCP-096 to immediately leave his enrage, regardless of how many targets he currently has, if you are one of his current targets. You always die when using this, even if there's no enrage to break, or you are not a target.";

    /// <inheritdoc/>
    public override float Weight { get; set; } = 1f;

    /// <inheritdoc/>
    public override SpawnProperties? SpawnProperties { get; set; } = new()
    {
        Limit = 1,
        DynamicSpawnPoints = new List<DynamicSpawnPoint>
        {
            new()
            {
                Chance = 100,
                Location = SpawnLocationType.Inside096,
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
        Player.UsingItem += OnUsingItem;

        base.SubscribeEvents();
    }

    /// <inheritdoc/>
    protected override void UnsubscribeEvents()
    {
        Player.UsingItem -= OnUsingItem;

        base.UnsubscribeEvents();
    }

    private void OnUsingItem(UsedItemEventArgs ev)
    {
        Log.Debug($"{ev.Player.Nickname} used a medical item: {ev.Item}");
        if (!Check(ev.Player.CurrentItem))
            return;

        Timing.CallDelayed(1.5f, () =>
        {
            Log.Debug($"{ev.Player.Nickname} used a {Name}");
            foreach (Exiled.API.Features.Player player in Exiled.API.Features.Player.List)
            {
                if (player.Role == RoleTypeId.Scp096)
                {
                    Log.Debug($"{ev.Player.Nickname} - {Name} found an 096: {player.Nickname}");
                    if (player.Role is not Scp096Role scp096)
                        continue;

                    Log.Debug($"{player.Nickname} 096 component found.");
                    if ((!scp096.HasTarget(ev.Player) ||
                         scp096.RageState != Scp096RageState.Docile) &&
                        scp096.RageState != Scp096RageState.Calming)
                        continue;

                    Log.Debug($"{player.Nickname} 096 checks passed.");
                    scp096.RageManager.ServerEndEnrage();
                    ev.Player.Hurt(new UniversalDamageHandler(-1f, DeathTranslations.Poisoned));
                    return;
                }
            }

            if (!KillOnFail)
            {
                if (ev.Player.ArtificialHealth > 30)
                    ev.Player.ArtificialHealth -= 30;
                else
                    ev.Player.ArtificialHealth = 0;
                ev.Player.DisableEffect<Invigorated>();
                return;
            }

            Log.Debug($"{Name} kill on fail: {ev.Player.Nickname}");
            ev.Player.Hurt(new UniversalDamageHandler(-1f, DeathTranslations.Poisoned));
        });

        ev.Player.RemoveItem(ev.Player.CurrentItem);
    }
}