using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.Events.EventArgs;

namespace CustomItems.Items
{
    public class MediGun : CustomItem
    {
        public MediGun(ItemType type, int itemId) : base(type, itemId)
        {
        }

        public override string ItemName { get; set; } = "MG-119";

        public override string ItemDescription { get; set; } =
            "A specialized weapon that fires darts filled with a special mixture of Painkillers, Antibiotics, Antiseptics and other medicines. When fires at friendly targets, they will be healed. When fired at instances of SCP-049-2, they will be slowly converted back to human form. Does nothing when fired at anyone else.";

        protected override void LoadEvents()
        {
            Exiled.Events.Handlers.Player.Shot += OnShot;
            Exiled.Events.Handlers.Scp049.FinishingRecall += OnFinishingRecall;
        }

        protected override void UnloadEvents()
        {
            Exiled.Events.Handlers.Player.Shot -= OnShot;
        }

        protected override void OnWaitingForPlayers()
        {
            previousRoles.Clear();
            base.OnWaitingForPlayers();
        }

        Dictionary<Player, RoleType> previousRoles = new Dictionary<Player, RoleType>();

        private void OnShot(ShotEventArgs ev)
        {
            if (CheckItem(ev.Shooter.CurrentItem))
            {
                if (Player.Get(ev.Target) is Player player)
                {
                    if (player.Team == ev.Shooter.Team)
                        player.Health += ev.Damage;
                    else if (player.Role == RoleType.Scp0492)
                    {
                        player.MaxAdrenalineHealth = Plugin.Singleton.Config.MedigunZombieHealthRequired;
                        player.AdrenalineHealth += ev.Damage;

                        if (player.AdrenalineHealth >= player.MaxAdrenalineHealth)
                            DoReviveZombie(player);
                    }
                }

                ev.Damage = 0;
            }
        }
        
        private void OnFinishingRecall(FinishingRecallEventArgs ev)
        {
            if (!previousRoles.ContainsKey(ev.Target))
                previousRoles.Add(ev.Target, ev.Target.Role);
            else
                previousRoles[ev.Target] = ev.Target.Role;
        }

        private void DoReviveZombie(Player player)
        {
            if (previousRoles.ContainsKey(player))
                player.SetRole(previousRoles[player], true, false);
        }
    }
}