// -----------------------------------------------------------------------
// <copyright file="StickyGrenade.cs" company="Galaxy119 and iopietro">
// Copyright (c) Galaxy119 and iopietro. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems.Patches
{
#pragma warning disable SA1313
    using Exiled.API.Features.Items;
    using HarmonyLib;
    using InventorySystem.Items.Pickups;
    using InventorySystem.Items.ThrowableProjectiles;
    using UnityEngine;

    /// <summary>
    /// Patches <see cref="CollisionDetectionPickup.OnCollisionEnter"/>.
    /// </summary>
    [HarmonyPatch(typeof(CollisionDetectionPickup), nameof(CollisionDetectionPickup.OnCollisionEnter))]
    internal class StickyGrenade
    {
        private static void Prefix(EffectGrenade __instance)
        {
            if (Items.C4Charge.Instance.IsSticky && Items.C4Charge.PlacedCharges.ContainsKey(Pickup.Get(__instance)))
            {
                var rigidbody = __instance.gameObject.GetComponent<Rigidbody>();
                rigidbody.isKinematic = false;
                rigidbody.useGravity = false;
                rigidbody.velocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
                rigidbody.freezeRotation = true;
                rigidbody.mass = 100000;
            }
        }
    }
}
