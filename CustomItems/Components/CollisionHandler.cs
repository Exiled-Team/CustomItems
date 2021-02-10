// <copyright file="CollisionHandler.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CustomItems
{
    using System;
    using Exiled.API.Features;
    using Grenades;
    using UnityEngine;

    /// <summary>
    /// Collision Handler for grenades.
    /// </summary>
    public class CollisionHandler : MonoBehaviour
    {
        /// <summary>
        /// The thrower of the grenade.
        /// </summary>
        public GameObject Owner;

        /// <summary>
        /// The grenade itself.
        /// </summary>
        public Grenade Grenade;

        private void OnCollisionEnter(Collision collision)
        {
            try
            {
                if (collision.gameObject == Owner || collision.gameObject.GetComponent<Grenade>() != null)
                    return;
                Grenade.NetworkfuseTime = 0.1f;
            }
            catch (Exception e)
            {
                Log.Error($"CollisionHandler: {e.Message}\n{e.StackTrace}");
            }
        }
    }
}