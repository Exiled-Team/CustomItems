// -----------------------------------------------------------------------
// <copyright file="StickyGrenade.cs" company="Galaxy119 and iopietro">
// Copyright (c) Galaxy119 and iopietro. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems.Patches
{
#pragma warning disable SA1313
    using Grenades;
    using HarmonyLib;
    using UnityEngine;

    /// <inheritdoc/>
    [HarmonyPatch(typeof(Grenade), nameof(Grenade.OnCollisionEnter))]
    public class StickyGrenade
    {
        /// <inheritdoc/>
        public static void Prefix(Grenade __instance)
        {
            if (Items.C4Charge.Instance.IsSticky && Items.C4Charge.PlacedCharges.ContainsKey(__instance))
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
