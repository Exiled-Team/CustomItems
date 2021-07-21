// -----------------------------------------------------------------------
// <copyright file="EmpGrenade.cs" company="Galaxy119 and iopietro">
// Copyright (c) Galaxy119 and iopietro. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems.Items
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.CustomItems.API;
    using Exiled.CustomItems.API.Features;
    using Exiled.CustomItems.API.Spawn;
    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;
    using Interactables.Interobjects.DoorUtils;
    using MEC;
    using Map = Exiled.Events.Handlers.Map;
    using Player = Exiled.API.Features.Player;

    /// <inheritdoc />
    public class EmpGrenade : CustomGrenade
    {
        private static readonly List<Room> LockedRooms079 = new List<Room>();

        private readonly List<DoorVariant> lockedDoors = new List<DoorVariant>();

        private readonly List<TeslaGate> disabledTeslaGates = new List<TeslaGate>();

        /// <inheritdoc/>
        public override uint Id { get; set; } = 0;

        /// <inheritdoc/>
        public override string Name { get; set; } = "EM-119";

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new DynamicSpawnPoint
                {
                    Chance = 100,
                    Location = SpawnLocation.Inside173Gate,
                },
            },
            StaticSpawnPoints = new List<StaticSpawnPoint>
            {
                new StaticSpawnPoint
                {
                    Chance = 50,
                    Name = "somewhere",
                    Position = new Vector(100, 25, 40),
                },
            },
        };

        /// <inheritdoc/>
        public override string Description { get; set; } = "This flashbang has been modified to emit a short-range EMP when it detonates. When detonated, any lights, doors, cameras and in the room, as well as all speakers in the facility, will be disabled for a short time.";

        /// <inheritdoc/>
        public override bool ExplodeOnCollision { get; set; } = true;

        /// <inheritdoc/>
        public override float FuseTime { get; set; } = 1.5f;

        /// <summary>
        /// Gets or sets a value indicating whether or not EMP grenades will open doors that are currently locked.
        /// </summary>
        [Description("Whether or not EMP grenades will open doors that are currently locked.")]
        public bool OpenLockedDoors { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether or not EMP grenades will open doors that require keycard permissions.
        /// </summary>
        [Description("Whether or not EMP grenades will open doors that require keycard permissions.")]
        public bool OpenKeycardDoors { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating what doors will never be opened by EMP grenades.
        /// </summary>
        [Description("A list of door names that will not be opened with EMP grenades regardless of the above configs.")]
        public HashSet<string> BlacklistedDoorNames { get; set; } = new HashSet<string>();

        /// <summary>
        /// Gets or sets a value indicating whether if tesla gates will get disabled.
        /// </summary>
        [Description("Whether or not EMP grenades disable tesla gates in the rooms the affect, for their duration.")]
        public bool DisableTeslaGates { get; set; } = true;

        /// <summary>
        /// Gets or sets how long the EMP effect should last on the rooms affected.
        /// </summary>
        [Description("How long the EMP effect should last on the rooms affected.")]
        public float Duration { get; set; } = 20f;

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            Scp079.ChangingCamera += OnChangingCamera;
            Scp079.TriggeringDoor += OnInteractingDoor;
            Map.ExplodingGrenade += OnExplodingGrenade;

            if (DisableTeslaGates)
                Exiled.Events.Handlers.Player.TriggeringTesla += OnTriggeringTesla;

            base.SubscribeEvents();
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            Scp079.ChangingCamera -= OnChangingCamera;
            Scp079.TriggeringDoor -= OnInteractingDoor;
            Map.ExplodingGrenade -= OnExplodingGrenade;

            if (DisableTeslaGates)
                Exiled.Events.Handlers.Player.TriggeringTesla -= OnTriggeringTesla;

            base.UnsubscribeEvents();
        }

        private static void OnChangingCamera(ChangingCameraEventArgs ev)
        {
            Room room = ev.Camera.Room();

            if (room != null && LockedRooms079.Contains(room))
                ev.IsAllowed = false;
        }

        private void OnInteractingDoor(InteractingDoorEventArgs ev)
        {
            if (lockedDoors.Contains(ev.Door))
                ev.IsAllowed = false;
        }

        private void OnExplodingGrenade(ExplodingGrenadeEventArgs ev)
        {
            if (!Check(ev.Grenade))
                return;

            ev.IsAllowed = false;

            Room room = Exiled.API.Features.Map.FindParentRoom(ev.Grenade);
            TeslaGate gate = null;

            Log.Debug($"{ev.Grenade.transform.position} - {room.Position} - {Exiled.API.Features.Map.Rooms.Count}", CustomItems.Instance.Config.IsDebugEnabled);

            LockedRooms079.Add(room);

            room.TurnOffLights(Duration);

            if (DisableTeslaGates)
            {
                foreach (TeslaGate teslaGate in Exiled.API.Features.Map.TeslaGates)
                    if (Exiled.API.Features.Map.FindParentRoom(teslaGate.gameObject) == room)
                    {
                        disabledTeslaGates.Add(teslaGate);
                        gate = teslaGate;
                        break;
                    }
            }

            Log.Debug($"{room.Doors.Count()} - {room.Type}", CustomItems.Instance.Config.IsDebugEnabled);

            foreach (DoorVariant door in room.Doors)
            {
                if (door == null ||
                    (!string.IsNullOrEmpty(door.GetNametag()) && BlacklistedDoorNames.Contains(door.GetNametag())) ||
                    (door.NetworkActiveLocks > 0 && !OpenLockedDoors) ||
                    (door.RequiredPermissions.RequiredPermissions != KeycardPermissions.None && !OpenKeycardDoors))
                    continue;

                Log.Debug("Opening a door!", CustomItems.Instance.Config.IsDebugEnabled);

                door.NetworkTargetState = true;
                door.ServerChangeLock(DoorLockReason.NoPower, true);

                if (!lockedDoors.Contains(door))
                    lockedDoors.Add(door);

                Timing.CallDelayed(Duration, () =>
                {
                    door.ServerChangeLock(DoorLockReason.NoPower, false);
                    lockedDoors.Remove(door);
                });
            }

            foreach (Player player in Player.List)
            {
                if (player.Role == RoleType.Scp079)
                    if (player.Camera != null && player.Camera.Room() == room)
                        player.SetCamera(198);
                if (player.CurrentRoom != room)
                    continue;

                foreach (Inventory.SyncItemInfo item in player.ReferenceHub.inventory.items)
                    if (item.id == ItemType.Radio)
                        player.ReferenceHub.inventory.items.ModifyDuration(player.ReferenceHub.GetComponent<Radio>().myRadio, 0f);
                    else if (player.ReferenceHub.weaponManager.syncFlash)
                        player.ReferenceHub.weaponManager.syncFlash = false;
            }

            Timing.CallDelayed(Duration, () =>
            {
                LockedRooms079.Remove(room);
                if (gate != null)
                    disabledTeslaGates.Remove(gate);
            });
        }

        private void OnTriggeringTesla(TriggeringTeslaEventArgs ev)
        {
            foreach (TeslaGate gate in Exiled.API.Features.Map.TeslaGates)
                if (Exiled.API.Features.Map.FindParentRoom(gate.gameObject) == ev.Player.CurrentRoom && disabledTeslaGates.Contains(gate))
                    ev.IsTriggerable = false;
        }
    }
}