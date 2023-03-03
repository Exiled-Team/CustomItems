// -----------------------------------------------------------------------
// <copyright file="Scp714.cs" company="Galaxy119 and iopietro">
// Copyright (c) Galaxy119 and iopietro. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems.Items
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Spawn;
    using Exiled.CustomItems.API;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs;
    using Exiled.Events.EventArgs.Player;
    using PlayerRoles;

    /// <inheritdoc/>
    [CustomItem(ItemType.Coin)]
    public class Scp714 : CustomItem
    {
        /// <inheritdoc/>
        public override uint Id { get; set; } = 12;

        /// <inheritdoc/>
        public override string Name { get; set; } = "SCP-714";

        /// <inheritdoc/>
        public override string Description { get; set; } = "The green ring that protects you from SCP-049.";

        /// <inheritdoc/>
        public override float Weight { get; set; } = 1.15f;

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 50,
                    Location = SpawnLocationType.Inside049Armory,
                },
            },
        };

        /// <summary>
        /// Gets or sets which roles shouldn't be able to deal damage to the player that has SCP-714 put on.
        /// </summary>
        [Description("Which roles shouldn't be able to deal damage to the player that has SCP-714 put on.")]
        public List<RoleTypeId> Scp714Roles { get; set; } = new()
        {
            RoleTypeId.Scp049,
            RoleTypeId.Scp0492,
        };

        /// <summary>
        /// Gets or sets which effects should be given to the player, when he will put on SCP-714.
        /// </summary>
        [Description("Which effects should be given to the player, when he will put on SCP-714.")]
        public List<string> Scp714Effects { get; set; } = new()
        {
            "Asphyxiated",
        };

        /// <summary>
        /// Gets or sets message shown to player, when he takes off the SCP-714.
        /// </summary>
        [Description("Message shown to player, when he takes off the SCP-714.")]
        public string TakeOffMessage { get; set; } = "You've taken off the ring.";

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.ChangingItem += OnChangingItem;
            Exiled.Events.Handlers.Player.Hurting += OnHurting;

            base.SubscribeEvents();
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.ChangingItem -= OnChangingItem;
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;

            base.UnsubscribeEvents();
        }

        /// <inheritdoc/>
        protected override void OnDropping(DroppingItemEventArgs ev)
        {
            ev.Player.ShowHint(TakeOffMessage);

            foreach (string effect in Scp714Effects)
            {
                try
                {
                    ev.Player.DisableEffect((EffectType)Enum.Parse(typeof(EffectType), effect, true));
                }
                catch (Exception)
                {
                    Log.Error($"\"{effect}\" is not a valid effect name.");
                    continue;
                }
            }

            base.OnDropping(ev);
        }

        private void OnChangingItem(ChangingItemEventArgs ev)
        {
            if (Check(ev.NewItem))
            {
                foreach (string effect in Scp714Effects)
                {
                    if (!ev.Player.EnableEffect(effect, 999f, false))
                    {
                        Log.Error($"\"{effect}\" is not a valid effect name.");
                    }
                }
            }
            else

            if (Check(ev.Player.CurrentItem))
            {
                if (!string.IsNullOrEmpty(TakeOffMessage))
                    ev.Player.ShowHint(TakeOffMessage);

                foreach (string effect in Scp714Effects)
                {
                    try
                    {
                        ev.Player.DisableEffect((EffectType)Enum.Parse(typeof(EffectType), effect, true));
                    }
                    catch (Exception)
                    {
                        Log.Error($"\"{effect}\" is not a valid effect name.");
                        continue;
                    }
                }
            }
        }

        private void OnHurting(HurtingEventArgs ev)
        {
            if (Check(ev.Player.CurrentItem))
            {
                if (Scp714Roles.Contains(ev.Attacker.Role))
                {
                    ev.IsAllowed = false;
                }
            }
        }
    }
}
