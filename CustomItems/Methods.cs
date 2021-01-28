using System.Collections.Generic;
using CustomItems.Components;
using MEC;
using Subclass;
using Player = Exiled.API.Features.Player;

namespace CustomItems
{
    public class Methods
    {
        private readonly Plugin plugin;
        public Methods(Plugin plugin) => this.plugin = plugin;
        
        public IEnumerator<float> GiveSniper(Player player)
        {
            while (!TrackingAndMethods.PlayersWithSubclasses.ContainsKey(player))
                yield return Timing.WaitForSeconds(0.75f);

            yield return Timing.WaitForSeconds(1f);
            
            plugin.SniperRifleComponent.GiveItem(player);
        }
    }
}