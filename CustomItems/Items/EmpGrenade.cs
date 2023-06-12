// -----------------------------------------------------------------------
// <copyright file="EmpGrenade.cs" company="Joker119">
// Copyright (c) Joker119. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems.Items;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Roles;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp079;
using Exiled.Events.Handlers;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Firearms.Attachments.Components;
using MEC;
using UnityEngine;
using Camera = Exiled.API.Features.Camera;
using CameraType = Exiled.API.Enums.CameraType;
using Item = Exiled.API.Features.Items.Item;
using KeycardPermissions = Interactables.Interobjects.DoorUtils.KeycardPermissions;
using Player = Exiled.API.Features.Player;

/// <inheritdoc />
[CustomItem(ItemType.GrenadeFlash)]
public class EmpGrenade : CustomGrenade
{
    private static readonly List<Room> LockedRooms079 = new();

    private readonly List<Door> lockedDoors = new();

    private readonly List<TeslaGate> disabledTeslaGates = new();

    /// <inheritdoc/>
    public override uint Id { get; set; } = 0;

    /// <inheritdoc/>
    public override string Name { get; set; } = "EM-119";

    /// <inheritdoc/>
    public override float Weight { get; set; } = 1.15f;

    /// <inheritdoc/>
    public override SpawnProperties? SpawnProperties { get; set; } = new()
    {
        Limit = 1,
        DynamicSpawnPoints = new List<DynamicSpawnPoint>
        {
            new()
            {
                Chance = 100,
                Location = SpawnLocationType.Inside173Gate,
            },
        },
        StaticSpawnPoints = new List<StaticSpawnPoint>
        {
            new()
            {
                Chance = 50,
                Name = "somewhere",
                Position = new Vector3(100, 25, 40),
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
    public HashSet<DoorType> BlacklistedDoorTypes { get; set; } = new();

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

        if (DisableTeslaGates)
            Exiled.Events.Handlers.Player.TriggeringTesla += OnTriggeringTesla;

        base.SubscribeEvents();
    }

    /// <inheritdoc/>
    protected override void UnsubscribeEvents()
    {
        Scp079.ChangingCamera -= OnChangingCamera;
        Scp079.TriggeringDoor -= OnInteractingDoor;

        if (DisableTeslaGates)
            Exiled.Events.Handlers.Player.TriggeringTesla -= OnTriggeringTesla;

        base.UnsubscribeEvents();
    }

    /// <inheritdoc/>
    protected override void OnExploding(ExplodingGrenadeEventArgs ev)
    {
        ev.IsAllowed = false;
        Room room = Room.FindParentRoom(ev.Projectile.GameObject);
        TeslaGate? gate = null;

        Log.Debug($"{ev.Projectile.GameObject.transform.position} - {room.Position} - {Room.List.Count()}");

        LockedRooms079.Add(room);

        room.TurnOffLights(Duration);

        if (DisableTeslaGates)
        {
            foreach (TeslaGate teslaGate in TeslaGate.List)
            {
                if (Room.FindParentRoom(teslaGate.GameObject) == room)
                {
                    disabledTeslaGates.Add(teslaGate);
                    gate = teslaGate;
                    break;
                }
            }
        }

        Log.Debug($"{room.Doors.Count()} - {room.Type}");

        foreach (Door door in room.Doors)
        {
            if (door == null ||
                BlacklistedDoorTypes.Contains(door.Type) ||
                (door.DoorLockType > 0 && !OpenLockedDoors) ||
                (door.RequiredPermissions.RequiredPermissions != KeycardPermissions.None && !OpenKeycardDoors) || door.Type.IsElevator())
                continue;

            Log.Debug("Opening a door!");

            door.IsOpen = true;
            door.ChangeLock(DoorLockType.NoPower);

            if (!lockedDoors.Contains(door))
                lockedDoors.Add(door);

            Timing.CallDelayed(Duration, () =>
            {
                door.Unlock();
                lockedDoors.Remove(door);
            });
        }

        foreach (Player player in Player.List)
        {
            if (player.Role.Is(out Scp079Role scp079))
            {
                if (scp079.Camera != null && scp079.Camera.Room == room)
                    scp079.Camera = Camera.Get(CameraType.Hcz079ContChamber);
            }

            if (player.CurrentRoom != room)
                continue;

            foreach (Item item in player.Items)
            {
                switch (item)
                {
                    case Radio radio:
                        radio.IsEnabled = false;
                        break;
                    case Flashlight flashlight:
                        flashlight.Active = false;
                        break;
                    case Firearm firearm:
                        {
                            foreach (Attachment attachment in firearm.Attachments)
                            {
                                if (attachment.Name == AttachmentName.Flashlight)
                                    attachment.IsEnabled = false;
                            }

                            break;
                        }
                }
            }
        }

        Timing.CallDelayed(Duration, () =>
        {
            try
            {
                LockedRooms079.Remove(room);
            }
            catch (Exception e)
            {
                Log.Debug($"REMOVING LOCKED ROOM: {e}");
            }

            if (gate != null)
            {
                try
                {
                    disabledTeslaGates.Remove(gate);
                }
                catch (Exception e)
                {
                    Log.Debug($"REMOVING DISABLED TESLA: {e}");
                }
            }
        });
    }

    private static void OnChangingCamera(ChangingCameraEventArgs ev)
    {
        Room room = ev.Camera.Room;

        if (room != null && LockedRooms079.Contains(room))
            ev.IsAllowed = false;
    }

    private void OnInteractingDoor(InteractingDoorEventArgs ev)
    {
        if (lockedDoors.Contains(ev.Door))
            ev.IsAllowed = false;
    }

    private void OnTriggeringTesla(TriggeringTeslaEventArgs ev)
    {
        foreach (TeslaGate gate in TeslaGate.List)
        {
            if (Room.FindParentRoom(gate.GameObject) == ev.Player.CurrentRoom && disabledTeslaGates.Contains(gate))
                ev.IsAllowed = false;
        }
    }
}