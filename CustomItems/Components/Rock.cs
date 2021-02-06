using System;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Grenades;
using UnityEngine;

namespace CustomItems
{
    public class Rock : Scp018Grenade
    {
        public GameObject Owner { get; set; }
        public Side Side { get; set; }

        public override void OnSpeedCollisionEnter(Collision collision, float relativeSpeed)
        {
            try
            {
                if (collision.gameObject == Owner || collision.gameObject.GetComponent<Grenade>() != null)
                    return;

                if (Player.Get(collision.collider.GetComponentInParent<ReferenceHub>()) is Player target && (target.Side != Side || Plugin.Singleton.Config.ItemConfigs.RockCfg.FriendlyFire))
                    target.Hurt(Plugin.Singleton.Config.ItemConfigs.RockCfg.ThrownDamage, DamageTypes.Wall, "ROCK");
                Destroy(gameObject);
                Plugin.Singleton.ItemManagers.First(i => i.Name == "Rock").SpawnItem(collision.GetContact(0).point + Vector3.up);
            }
            catch (Exception e)
            {
                Log.Error($"{e}\n{e.StackTrace}");
            }
        }
    }
}