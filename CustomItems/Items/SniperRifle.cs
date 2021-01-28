using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using MEC;
using UnityEngine;

namespace CustomItems.Components
{
    public class SniperRifle : CustomItem
    {
        public override int ClipSize { get; set; } = 1;
        public override ItemType ItemType { get; set; } = ItemType.GunE11SR;
        public override int ModBarrel { get; set; } = 3;
        public override int ModSight { get; set; } = 4;

        public override void LoadEvents()
        {
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
        }

        public override void UnloadEvents()
        {
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
        }

        private void OnHurting(HurtingEventArgs ev)
        {
            if (CheckItem(ev.Attacker.CurrentItem))
                ev.Amount *= Plugin.Singleton.Config.SniperDmgMult;
        }
    }
}