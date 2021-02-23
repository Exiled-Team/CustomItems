// <copyright file="Rock.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CustomItems
{
    using System;
    using System.Linq;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Grenades;
    using UnityEngine;

    /// <summary>
    /// Special collision handler for rocks.
    /// </summary>
    public class Rock : Scp018Grenade
    {
        /// <summary>
        /// Gets or sets the owner of the 'rock'.
        /// </summary>
        public GameObject Owner { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Side"/> of the rock, for determining friendly fire.
        /// </summary>
        public Side Side { get; set; }

        /// <summary>
        /// The collision handler.
        /// </summary>
        /// <param name="collision">The <see cref="Collision"/> occuring.</param>
        /// <param name="relativeSpeed">The <see cref="float"/> indicating the speed.</param>
        public override void OnSpeedCollisionEnter(Collision collision, float relativeSpeed)
        {
            try
            {
                if (collision.gameObject == Owner || collision.gameObject.GetComponent<Grenade>() != null)
                    return;

                if (Player.Get(collision.collider.GetComponentInParent<ReferenceHub>()) is Player target && (target.Side != Side || Plugin.Singleton.Config.ItemConfigs.RockCfg.FriendlyFire))
                    target.Hurt(Plugin.Singleton.Config.ItemConfigs.RockCfg.ThrownDamage, DamageTypes.Wall, "ROCK");
                Destroy(gameObject);
                Exiled.CustomItems.CustomItems.Instance.ItemManagers.First(i => i.Name == "Rock").Spawn(collision.GetContact(0).point + Vector3.up);
            }
            catch (Exception e)
            {
                Log.Error($"{e}\n{e.StackTrace}");
            }
        }
    }
}