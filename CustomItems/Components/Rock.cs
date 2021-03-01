// -----------------------------------------------------------------------
// <copyright file="Rock.cs" company="Galaxy199 and iopietro">
// Copyright (c) Galaxy199 and iopietro. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems.Components
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
        /// Gets the owner of the 'rock'.
        /// </summary>
        public GameObject Owner { get; private set; }

        /// <summary>
        /// Gets the <see cref="Side"/> of the rock, for determining friendly fire.
        /// </summary>
        public Side Side { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the rock can hurt allies or not.
        /// </summary>
        public bool FriendlyFire { get; private set; }

        /// <summary>
        /// Gets the thrown damage.
        /// </summary>
        public float ThrownDamage { get; private set; }

        /// <summary>
        /// Inits the rock.
        /// </summary>
        /// <param name="owner"><inheritdoc cref="Owner"/></param>
        /// <param name="side"><inheritdoc cref="Side"/></param>
        /// <param name="friendlyFire"><inheritdoc cref="FriendlyFire"/></param>
        /// <param name="thrownDamage"><inheritdoc cref="ThrownDamage"/></param>
        public void Init(GameObject owner, Side side, bool friendlyFire, float thrownDamage)
        {
            Owner = owner;
            Side = side;
            FriendlyFire = friendlyFire;
            ThrownDamage = thrownDamage;
        }

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

                if (Player.Get(collision.collider.GetComponentInParent<ReferenceHub>()) is Player target && (target.Side != Side || FriendlyFire))
                    target.Hurt(ThrownDamage, DamageTypes.Wall, "ROCK");

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