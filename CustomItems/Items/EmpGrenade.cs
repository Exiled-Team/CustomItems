using System.Collections.Generic;
using System.Linq;
using CustomItems.API;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using Interactables.Interobjects.DoorUtils;
using MEC;
using UnityEngine;

namespace CustomItems.Items
{
    public class EmpGrenade : CustomGrenade
    {
        public EmpGrenade(ItemType type, int itemId) : base(type, itemId)
        {
        }
        
        public override string Name { get; set; } = Plugin.Singleton.Config.ItemConfigs.EmpCfg.Name;
        public override Dictionary<SpawnLocation, float> SpawnLocations { get; set; } = Plugin.Singleton.Config.ItemConfigs.EmpCfg.SpawnLocations;
        public override string Description { get; set; } = Plugin.Singleton.Config.ItemConfigs.EmpCfg.Description;
        public override int SpawnLimit { get; set; } = Plugin.Singleton.Config.ItemConfigs.EmpCfg.SpawnLimit;

        protected override bool ExplodeOnCollision { get; set; } = Plugin.Singleton.Config.ItemConfigs.EmpCfg.ExplodeOnCollision;
        protected override float FuseTime { get; set; } = Plugin.Singleton.Config.ItemConfigs.EmpCfg.FuseDuration;

        protected override void LoadEvents()
        {
            Exiled.Events.Handlers.Scp079.ChangingCamera += OnChangingCamera;
            Exiled.Events.Handlers.Scp079.InteractingDoor += OnInteractingDoor;
            Exiled.Events.Handlers.Map.ExplodingGrenade += OnExplodingGrenade;
            base.LoadEvents();
        }


        private List<DoorVariant> lockedDoors = new List<DoorVariant>();
        private void OnInteractingDoor(InteractingDoorEventArgs ev)
        {
            if (lockedDoors.Contains(ev.Door))
                ev.IsAllowed = false;
        }

        protected override void UnloadEvents()
        {
            Exiled.Events.Handlers.Map.ExplodingGrenade -= OnExplodingGrenade;
            base.UnloadEvents();
        }
        
        private void OnChangingCamera(ChangingCameraEventArgs ev)
        {
            Room room = ev.Camera.Room();
            if (room != null && room.LightsOff)
                ev.IsAllowed = false;
        }

        private void OnExplodingGrenade(ExplodingGrenadeEventArgs ev)
        {
            if (CheckGrenade(ev.Grenade))
            {
                ev.IsAllowed = false;

                Room room = Map.FindParentRoom(ev.Grenade);
                
                room.TurnOffLights(Plugin.Singleton.Config.ItemConfigs.EmpCfg.Duration);
                Log.Debug($"{room.Doors.Count()} - {room.Type}");
                foreach (DoorVariant door in room.Doors)
                {
                    Log.Debug($"Opening a door!", Plugin.Singleton.Config.Debug);
                    door.NetworkTargetState = true;
                    door.ServerChangeLock(DoorLockReason.NoPower, true);
                    if (lockedDoors.Contains(door))
                        lockedDoors.Add(door);

                    Timing.CallDelayed(Plugin.Singleton.Config.ItemConfigs.EmpCfg.Duration, () =>
                    {
                        door.ServerChangeLock(DoorLockReason.NoPower, false);
                        lockedDoors.Remove(door);
                    });
                }
                
                foreach (Player player in Player.List)
                    if (player.Role == RoleType.Scp079)
                    {
                        if (player.Camera.Room() == room)
                            player.SetCamera(198);

                        break;
                    }
            }
        }
    }
}