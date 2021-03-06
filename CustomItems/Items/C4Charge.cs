namespace CustomItems.Items
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Exiled.CustomItems.API;
    using Exiled.CustomItems.API.Features;
    using Exiled.CustomItems.API.Spawn;
    using Exiled.Events.EventArgs;
    using Grenades;
    using Mirror;
    using YamlDotNet.Serialization;

    using PlayerEvent = Exiled.Events.Handlers.Player;
    using ServerEvent = Exiled.Events.Handlers.Server;
    using MapEvent = Exiled.Events.Handlers.Map;
    using UnityEngine;
    using Exiled.CustomItems;

    /// <inheritdoc/>
    public class C4Charge : CustomGrenade
    {
        /// <inheritdoc/>
        public static C4Charge Instance;

        /// <inheritdoc/>
        public static Dictionary<Grenade, Exiled.API.Features.Player> PlacedCharges = new Dictionary<Grenade, Exiled.API.Features.Player>();

        /// <inheritdoc/>
        public override uint Id { get; set; } = 15;

        /// <inheritdoc/>
        public override string Name { get; set; } = "C4-119";

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties
        {
            Limit = 5,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new DynamicSpawnPoint
                {
                    Chance = 10,
                    Location = SpawnLocation.InsideLczArmory,
                },

                new DynamicSpawnPoint
                {
                    Chance = 25,
                    Location = SpawnLocation.InsideHczArmory,
                },

                new DynamicSpawnPoint
                {
                    Chance = 50,
                    Location = SpawnLocation.InsideNukeArmory,
                },

                new DynamicSpawnPoint
                {
                    Chance = 50,
                    Location = SpawnLocation.Inside049Armory,
                },

                new DynamicSpawnPoint
                {
                    Chance = 100,
                    Location = SpawnLocation.InsideSurfaceNuke,
                },
            },
        };

        /// <inheritdoc/>
        public override string Description { get; set; } = "Explosive charge that can be remotly detonated.";


        /// <summary>
        /// Gets or sets a value indicating whether C4 charge should stick to walls / ceiling.
        /// </summary>
        [Description("Should C4 charge stick to walls / ceiling.")]
        public bool IsSticky { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating throwing force muliplier.
        /// </summary>
        [Description("Defines how strongly C4 will be thrown")]
        public float ThrowMultiplier { get; set; } = 40f;

        /// <summary>
        /// Gets or sets a value indicating whether C4 charge require a specific item to be detonated.
        /// </summary>
        [Description("Should C4 require a specific item to be detonated.")]
        public bool RequireDetonator { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the Detonator Item that will be used to detonate C4 Charges.
        /// </summary>
        [Description("The Detonator Item that will be used to detonate C4 Charges")]
        public ItemType DetonatorItem { get; set; } = ItemType.Radio;

        /// <summary>
        /// Gets or sets a value indicating whether C4 charges be defused and dropped as pickable item when the player who placed them dies.
        /// </summary>
        [Description("Should C4 charges be defused and dropped as pickable item when the player who placed them dies.")]
        public bool DropChargesOnDeath { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether maximum distance between C4 Charge and player to detonate.
        /// </summary>
        [Description("Maximum distance between C4 Charge and player to detonate.")]
        public float MaxDistance { get; set; } = 100f;

        /// <summary>
        /// Gets or sets time after which the C4 charge will automatically detonate.
        /// </summary>
        [Description("Time after which the C4 charge will automatically detonate.")]
        public override float FuseTime { get; set; } = 9999f;

        /// <inheritdoc/>
        [YamlIgnore]
        public override bool ExplodeOnCollision { get; set; } = false;


        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            Instance = this;

            ServerEvent.WaitingForPlayers += OnWaitingForPlayers;

            PlayerEvent.ThrowingGrenade += OnThrowing;
            PlayerEvent.Destroying += OnDestroying;
            PlayerEvent.Died += OnDied;

            MapEvent.ExplodingGrenade += OnExploding;

            base.SubscribeEvents();
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            Instance = null;

            ServerEvent.WaitingForPlayers -= OnWaitingForPlayers;

            PlayerEvent.ThrowingGrenade -= OnThrowing;
            PlayerEvent.Destroying -= OnDestroying;
            PlayerEvent.Died -= OnDied;

            MapEvent.ExplodingGrenade += OnExploding;

            base.UnsubscribeEvents();
        }

        /// <inheritdoc/>
        protected override void OnWaitingForPlayers()
        {
            PlacedCharges.Clear();

            base.OnWaitingForPlayers();
        }

        /// <inheritdoc/>
        protected override void OnThrowing(ThrowingGrenadeEventArgs ev)
        {
            if (Check(ev.Player.CurrentItem))
            {
                ev.IsAllowed = false;

                ev.Player.RemoveItem(ev.Player.CurrentItem);

                var c4 = Spawn(ev.Player.CameraTransform.position, (ev.Player.Rotation * ThrowMultiplier) + (Vector3.up * 1.5f), FuseTime);

                c4.NetworkthrowerGameObject = ev.Player.GameObject;
                c4.NetworkthrowerTeam = ev.Player.Team;

                if (!PlacedCharges.ContainsKey(c4))
                    PlacedCharges.Add(c4, ev.Player);
            }

            base.OnThrowing(ev);
        }

        private void OnDestroying(DestroyingEventArgs ev)
        {
            foreach (var charge in PlacedCharges.ToList())
            {
                if (charge.Value == ev.Player)
                {
                    if (DropChargesOnDeath)
                    {
                        TrySpawn((int)Id, charge.Key.transform.position);
                    }

                    NetworkServer.Destroy(charge.Key.gameObject);
                    PlacedCharges.Remove(charge.Key);
                }
            }
        }

        private void OnDied(DiedEventArgs ev)
        {
            foreach (var charge in PlacedCharges.ToList())
            {
                if (charge.Value == ev.Target)
                {
                    if (DropChargesOnDeath)
                    {
                        TrySpawn((int)Id, charge.Key.transform.position);
                    }

                    NetworkServer.Destroy(charge.Key.gameObject);
                    PlacedCharges.Remove(charge.Key);
                }
            }
        }

        private void OnExploding(ExplodingGrenadeEventArgs ev)
        {
            if (PlacedCharges.ContainsKey(ev.Grenade.GetComponent<Grenade>()))
            {
                PlacedCharges.Remove(ev.Grenade.GetComponent<Grenade>());
            }
        }
    }
}
