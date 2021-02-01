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

        public override Dictionary<Vector3, float> SpawnLocations { get; set; } = new Dictionary<Vector3, float>();
        public override string ItemName { get; set; } = "EM-119";
        protected override string ItemDescription { get; set; } =
            "This flashbang has been modified to emit a short-range EMP when it detonates. When detonated, any lights, doors, cameras and in the room, as well as all speakers in the facility, will be disabled for a short time.";

        protected override bool ExplodeOnCollision { get; set; } = true;

        protected override void LoadEvents()
        {
            foreach (KeyValuePair<string, float> kvp in Plugin.Singleton.Config.ItemConfigs.EmpCfg.SpawnLocations)
            {
                Vector3 pos = RoomLocation(kvp.Key);
                
                if (pos != Vector3.zero)
                    SpawnLocations.Add(pos, kvp.Value);
            }
            
            Exiled.Events.Handlers.Map.ExplodingGrenade += OnExplodingGrenade;
            base.LoadEvents();
        }

        protected override void UnloadEvents()
        {
            Exiled.Events.Handlers.Map.ExplodingGrenade -= OnExplodingGrenade;
            base.UnloadEvents();
        }

        private void OnExplodingGrenade(ExplodingGrenadeEventArgs ev)
        {
            if (CheckGrenade(ev.Grenade))
            {
                ev.IsAllowed = false;

                Room room = Map.FindParentRoom(ev.Grenade);
                
                room.TurnOffLights(Plugin.Singleton.Config.ItemConfigs.EmpCfg.Duration);
                foreach (DoorVariant door in room.Doors)
                {
                    door.NetworkTargetState = true;
                    door.ServerChangeLock(DoorLockReason.NoPower, true);

                    Timing.CallDelayed(Plugin.Singleton.Config.ItemConfigs.EmpCfg.Duration, () => door.ServerChangeLock(DoorLockReason.NoPower, false));
                    
                    foreach (Player player in Player.List)
                        if (player.Role == RoleType.Scp079)
                        {
                            if (player.Camera.Room() == room)
                            {
                                Room homeRoom = Map.Rooms.FirstOrDefault(r => r.Name.Contains("079"));
                                if (homeRoom == null)
                                {
                                    Log.Error($"HAH ROOM IS NULL BITCH");
                                    continue;
                                }

                                player.Camera = homeRoom.GetComponentInParent<Camera079>();
                            }

                            if (!string.IsNullOrEmpty(player.Speaker))
                                player.Speaker = string.Empty;

                            break;
                        }
                }
            }
        }
    }
}