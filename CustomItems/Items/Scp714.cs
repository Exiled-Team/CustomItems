// <copyright file="Scp1499.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CustomItems.Items
{
    using System.Collections.Generic;
    using CustomItems.API;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;
    using MEC;
    using UnityEngine;

    /// <inheritdoc />
    public class Scp714 : CustomItem
    {
        /// <inheritdoc />
        public Scp714(ItemType type, int itemId)
            : base(type, itemId)
        {
        }


        /// <inheritdoc/>
        public override string Name { get; set; } = Plugin.Singleton.Config.ItemConfigs.Scp714Cfg.Name;

        /// <inheritdoc/>
        public override Dictionary<SpawnLocation, float> SpawnLocations { get; set; } = Plugin.Singleton.Config.ItemConfigs.Scp714Cfg.SpawnLocations;

        /// <inheritdoc/>
        public override string Description { get; set; } = Plugin.Singleton.Config.ItemConfigs.Scp714Cfg.Description;

        /// <inheritdoc/>
        public override int SpawnLimit { get; set; } = Plugin.Singleton.Config.ItemConfigs.Scp714Cfg.SpawnLimit;

        /// <inheritdoc/>
        protected override void LoadEvents()
        {
            Exiled.Events.Handlers.Player.ChangingItem += OnChangingItem;
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            base.LoadEvents();
        }

        /// <inheritdoc/>
        protected override void UnloadEvents()
        {
            Exiled.Events.Handlers.Player.ChangingItem -= OnChangingItem;
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
            base.UnloadEvents();
        }

        /// <inheritdoc/>
        protected override void OnWaitingForPlayers()
        {
            base.OnWaitingForPlayers();
        }

        protected override void OnDroppingItem(DroppingItemEventArgs ev)
        {
            if (CheckItem(ev.Item))
            {
                ev.Player.ShowHint(Plugin.Singleton.Config.ItemConfigs.Scp714Cfg.TakeOffMessage);

                foreach (string effect in Plugin.Singleton.Config.ItemConfigs.Scp714Cfg.Scp714Effects)
                {
                    ev.Player.ReferenceHub.playerEffectsController.ChangeByString(effect, 0);
                }
            }

            base.OnDroppingItem(ev);
        }

        private void OnChangingItem(ChangingItemEventArgs ev)
        {
            if (CheckItem(ev.NewItem))
            {
                ev.Player.ShowHint(Plugin.Singleton.Config.ItemConfigs.Scp714Cfg.PutOnMessage);

                foreach (string effect in Plugin.Singleton.Config.ItemConfigs.Scp714Cfg.Scp714Effects)
                {
                    ev.Player.ReferenceHub.playerEffectsController.EnableByString(effect, 999f, false);
                }
            }

            else

            if(CheckItem(ev.OldItem))
            {
                ev.Player.ShowHint(Plugin.Singleton.Config.ItemConfigs.Scp714Cfg.TakeOffMessage);

                foreach (string effect in Plugin.Singleton.Config.ItemConfigs.Scp714Cfg.Scp714Effects)
                {
                    ev.Player.ReferenceHub.playerEffectsController.ChangeByString(effect, 0);
                }
            }
        }

        private void OnHurting(HurtingEventArgs ev)
        {
            if (CheckItem(ev.Target.CurrentItem))
            {
                if (Plugin.Singleton.Config.ItemConfigs.Scp714Cfg.Scp714Roles.Contains(ev.Attacker.Role))
                {
                    ev.IsAllowed = false;
                }
            }
        }
    }
}
