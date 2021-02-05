using System;
using Exiled.API.Features;
using Grenades;
using UnityEngine;

namespace CustomItems
{
    public class CollisionHandler : MonoBehaviour
    {
        public GameObject owner;
        public Grenade grenade;

        private void OnCollisionEnter(Collision collision)
        {
            try
            {
                if (collision.gameObject == owner || collision.gameObject.GetComponent<Grenade>() != null)
                    return;
                grenade.NetworkfuseTime = 0.1f;
            }
            catch (Exception e)
            {
                Log.Error($"CollisionHandler: {e.Message}\n{e.StackTrace}");
            }
        }
    }
}