using System.Collections.Generic;
using CustomItems.API;
using Exiled.Events.EventArgs;
using Exiled.API.Features;
using MEC;
using UnityEngine;

namespace CustomItems.Items
{
    public class TranqGun : CustomWeapon
    {
        public TranqGun(ItemType type, int clipSize, int itemId) : base(type, clipSize, itemId)
        {
        }

        public override string ItemName { get; set; }
        protected override string ItemDescription { get; set; }

        protected override void LoadEvents()
        {
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            base.LoadEvents();
        }

        protected override void UnloadEvents()
        {
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
            base.UnloadEvents();
        }

        private void OnHurting(HurtingEventArgs ev)
        {
            if (CheckItem(ev.Attacker.CurrentItem))
            {
                ev.Amount = 0;

                Timing.RunCoroutine(DoTranquilize(ev.Target));
            }
        }

        private IEnumerator<float> DoTranquilize(Player player)
        {
            Vector3 pos = player.Position;
            player.DropItems();

            Map.SpawnRagdoll(player, DamageTypes.None, pos, allowRecall: false);
            player.Position = new Vector3(0, 0, 0);

            yield return Timing.WaitForSeconds(3f);

            player.Position = pos;
        }
    }
}