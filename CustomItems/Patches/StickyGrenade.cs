namespace CustomItems.Patches
{
    using Grenades;
    using HarmonyLib;
    using Mirror;
    using System.CodeDom;
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
