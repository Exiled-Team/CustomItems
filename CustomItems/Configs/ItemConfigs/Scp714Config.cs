﻿// <copyright file="Scp714Config.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// <auto-generated>
#pragma warning disable 1591

namespace CustomItems.ItemConfigs
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using CustomItems.API;

    public class Scp714Config
    {
        [Description("Which roles shouldn't be able to deal damage to the player that has SCP-714 put on.")]
        public List<RoleType> Scp714Roles { get; set; } = new List<RoleType>()
        {
            RoleType.Scp049,
            RoleType.Scp0492,
        };

        [Description("Which effects should be given to the player, when he will put on SCP-714.")]
        public List<string> Scp714Effects { get; set; } = new List<string>
        {
            "Asphyxiated",
        };

        [Description("Message shown to player, when he puts on SCP-714.")]
        public string PutOnMessage { get; set; } = "You've put on the ring. You feel dizy.";

        [Description("Message shown to player, when he takes off the SCP-714.")]
        public string TakeOffMessage { get; set; } = "You've taken off the ring.";

        [Description("Where on the map items should spawn, and their % chance of spawning in each location.")]
        public Dictionary<SpawnLocation, float> SpawnLocations { get; set; } = new Dictionary<SpawnLocation, float>
        {
            { SpawnLocation.Inside049Armory, 50 }
        };

        [Description("The Custom Item ID for this item.")]
        public int Id { get; set; } = 13;

        [Description("The description of this item show to players when they obtain it.")]
        public string Description { get; set; } = "This green ring protects you from SCP-049";

        [Description("The name of this item shown to players when they obtain it.")]
        public string Name { get; set; } = "SCP-714";

        [Description("How many of this item are allowed to naturally spawn on the map when a round starts. 0 = unlimited")]
        public int SpawnLimit { get; set; } = 1;
    }
}
