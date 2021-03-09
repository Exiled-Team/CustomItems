
namespace CustomItems.Items
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.CustomItems.API;
    using Exiled.CustomItems.API.Features;
    using Exiled.CustomItems.API.Spawn;
    using Exiled.Events.EventArgs;
    using Hints;
    using MEC;
    using UnityEngine;

    /// <inheritdoc />
    public class Scp5355 : CustomWeapon
    {
        private readonly Dictionary<Player, Vector3> sizedPlayers = new Dictionary<Player, Vector3>();
        private readonly Dictionary<Player, int> playerMode = new Dictionary<Player, int>();


        /// <inheritdoc/>
        public override uint Id { get; set; } = 16;

        /// <inheritdoc/>
        public override string Name { get; set; } = "SCP-5355";

        /// <inheritdoc/>
        public override string Description { get; set; } = "SCP-5355 is a gun that can change the size of a person depending on the setting";

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new DynamicSpawnPoint
                {
                    Chance = 100,
                    Location = SpawnLocation.Inside173Armory,
                },
            },
        };

        /// <inheritdoc/>
        public override Modifiers Modifiers { get; set; } = default;

        /// <inheritdoc/>
        public override float Damage { get; set; }

        /// <inheritdoc/>
        public override uint ClipSize { get; set; } = 10;

        /// <summary>
        /// Gets or sets the float modifier used to determine how much the player size is changed.
        /// </summary>
        [Description("The float modifier used to determine how much the player size is changed on each hit.")]
        public float SizeChange { get; set; } = 0.1f;

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.Destroying += OnDestroying;
            Exiled.Events.Handlers.Player.Died += OnDied;

            base.SubscribeEvents();
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.Destroying -= OnDestroying;
            Exiled.Events.Handlers.Player.Died -= OnDied;

            base.UnsubscribeEvents();
        }

        protected override void OnChanging(ChangingItemEventArgs ev)
        {
            if (ev.Player.Ammo[(int)AmmoType.Nato9] == 0)
            ev.Player.Ammo[(int)AmmoType.Nato9] = 1;
        }

        /// <inheritdoc/>
        protected override void OnReloading(ReloadingWeaponEventArgs ev)
        {
            if (ev.IsAnimationOnly)
            {
                playerMode[ev.Player]++;
                string modeText;
                switch (playerMode[ev.Player])
                {
                    case 1:
                        modeText = "Shrink";
                        break;
                    case 2:
                        modeText = "Enlarge";
                        break;
                    case 3:
                        modeText = "Return";
                        break;
                    default:
                        playerMode[ev.Player] = 1;
                        modeText = "Shrink";
                        break;
                }

                ev.Player.ShowHint(modeText);
            }
        }


        /// <inheritdoc/>
        protected override void OnShooting(ShootingEventArgs ev)
        {
            base.OnShooting(ev);
            ev.Shooter.SetWeaponAmmo(ev.Shooter.CurrentItem, (int)ClipSize - 1);
            Player Target = Player.Get(ev.Target);
            if (Target != null)
            {
                if (!sizedPlayers.ContainsKey(Target))
                {
                    sizedPlayers.Add(Target, Target.Scale);
                }

                Vector3 NewSize = Target.Scale;
                switch (playerMode[ev.Shooter])
                {
                    case 1:
                        NewSize -= new Vector3(SizeChange, SizeChange, SizeChange);
                        if (NewSize.x < .4)
                            NewSize = new Vector3(.4f, .4f, .4f);
                        break;
                    case 2:
                        NewSize += new Vector3(SizeChange, SizeChange, SizeChange);
                        if (NewSize.x > 1.5)
                            NewSize = new Vector3(1.5f, 1.5f, 1.5f);
                        break;
                    case 3:
                        if (sizedPlayers.ContainsKey(Target))
                        {
                            NewSize = sizedPlayers[Target];
                        }

                        break;
                }

                if (Target.Scale != NewSize)
                {
                    Target.Scale = NewSize;
                }

                ev.Shooter.ReferenceHub.weaponManager.RpcConfirmShot(true, ev.Shooter.ReferenceHub.weaponManager.curWeapon);
            }

            ev.IsAllowed = false;
        }

        /// <inheritdoc/>
        protected override void OnWaitingForPlayers()
        {
            sizedPlayers.Clear();

            base.OnWaitingForPlayers();
        }

        private void OnDied(DiedEventArgs ev)
        {
            if (sizedPlayers.ContainsKey(ev.Target))
                sizedPlayers.Remove(ev.Target);
        }

        private void OnDestroying(DestroyingEventArgs ev)
        {
            if (sizedPlayers.ContainsKey(ev.Player))
                sizedPlayers.Remove(ev.Player);
        }
    }
}
