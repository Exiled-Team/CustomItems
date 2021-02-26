// -----------------------------------------------------------------------
// <copyright file="Rock.cs" company="Galaxy199 and iopietro">
// Copyright (c) Galaxy199 and iopietro. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems
{
    using System;
    using System.Linq;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;
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
                if (collision.gameObject == Owner || !collision.gameObject.TryGetComponent<Grenade>(out _))
                    return;

                if (Player.Get(collision.collider.GetComponentInParent<ReferenceHub>()) is Player target &&
                    (target.Side != Side || CustomItems.Instance.Config.ItemConfigs.RockCfg.FriendlyFire))
                {
                    target.Hurt(CustomItems.Instance.Config.ItemConfigs.RockCfg.ThrownDamage, DamageTypes.Wall, "ROCK");
                }

                Destroy(gameObject);

                CustomItem.Registered.First(customItem => customItem.Name == "Rock").Spawn(collision.GetContact(0).point + Vector3.up);
            }
            catch (Exception exception)
            {
                Log.Error($"{nameof(OnSpeedCollisionEnter)} error: {exception}");
            }
        }
    }
}