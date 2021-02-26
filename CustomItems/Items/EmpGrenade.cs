// -----------------------------------------------------------------------
// <copyright file="EmpGrenade.cs" company="Galaxy199 and iopietro">
// Copyright (c) Galaxy199 and iopietro. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems.Items
{
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
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

        /// <summary>
        /// A list of doors locked by the EMP Grenades.
        /// </summary>
        private readonly List<DoorVariant> lockedDoors = new List<DoorVariant>();

        /// <inheritdoc/>
        public override string Name { get; set; } = "EM-119";

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties();

        /// <inheritdoc/>
        public override string Description { get; set; } = "asd";

        /// <inheritdoc/>
        public override bool ExplodeOnCollision { get; set; }

        /// <inheritdoc/>
        public override float FuseTime { get; set; }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            Scp079.ChangingCamera += OnChangingCamera;
            Scp079.InteractingDoor += OnInteractingDoor;
            Map.ExplodingGrenade += OnExplodingGrenade;

            base.SubscribeEvents();
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            Scp079.ChangingCamera -= OnChangingCamera;
            Scp079.InteractingDoor -= OnInteractingDoor;
            Map.ExplodingGrenade -= OnExplodingGrenade;

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
            Log.Debug($"{ev.Grenade.transform.position} - {room.Position} - {Exiled.API.Features.Map.Rooms.Count}", CustomItems.Instance.Config.Debug);

            LockedRooms079.Add(room);
            room.TurnOffLights(CustomItems.Instance.Config.ItemConfigs.EmpCfg.Duration);
            Log.Debug($"{room.Doors.Count()} - {room.Type}", CustomItems.Instance.Config.Debug);
            foreach (DoorVariant door in room.Doors)
            {
                if (door.NetworkActiveLocks > 0 && !CustomItems.Instance.Config.ItemConfigs.EmpCfg.OpenLockedDoors)
                    continue;

                if (door.RequiredPermissions.RequiredPermissions != KeycardPermissions.None && !CustomItems.Instance.Config.ItemConfigs.EmpCfg.OpenKeycardDoors)
                    continue;

                Log.Debug("Opening a door!", CustomItems.Instance.Config.Debug);
                door.NetworkTargetState = true;
                door.ServerChangeLock(DoorLockReason.NoPower, true);
                if (lockedDoors.Contains(door))
                    lockedDoors.Add(door);

                Timing.CallDelayed(CustomItems.Instance.Config.ItemConfigs.EmpCfg.Duration, () =>
                {
                    door.ServerChangeLock(DoorLockReason.NoPower, false);
                    lockedDoors.Remove(door);
                });
            }

            foreach (Player player in Player.Get(RoleType.Scp079))
                if (player.Camera != null && player.Camera.Room() == room)
                    player.SetCamera(198);

            Timing.CallDelayed(CustomItems.Instance.Config.ItemConfigs.EmpCfg.Duration, () => LockedRooms079.Remove(room));
        }
    }
}