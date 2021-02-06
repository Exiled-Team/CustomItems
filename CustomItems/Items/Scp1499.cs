using System.Collections.Generic;
using CustomItems.API;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using MEC;
using UnityEngine;

namespace CustomItems.Items
{
    public class Scp1499 : CustomItem
    {
        public Scp1499(ItemType type, int itemId) : base(type, itemId)
        {
        }

        public override string Name { get; set; } = Plugin.Singleton.Config.ItemConfigs.Scp1499Cfg.Name;
        public override Dictionary<SpawnLocation, float> SpawnLocations { get; set; } = Plugin.Singleton.Config.ItemConfigs.Scp1499Cfg.SpawnLocations;
        public override string Description { get; set; } = Plugin.Singleton.Config.ItemConfigs.Scp1499Cfg.Description;
        public override int SpawnLimit { get; set; } = Plugin.Singleton.Config.ItemConfigs.Scp1499Cfg.SpawnLimit;
        

        private Vector3 Scp1499DimensionPos = new Vector3(152.93f, 978.03f, 93.64f); //This position is where is unused terain on the Surface
        
        private Dictionary<Player, Vector3> scp1499Players = new Dictionary<Player, Vector3>();


        protected override void LoadEvents()
        {
            Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers1499;
            Exiled.Events.Handlers.Player.MedicalItemUsed += OnUsedMedicalItem;
            Exiled.Events.Handlers.Player.DroppingItem += OnItemDropping;
            base.LoadEvents();
        }

        protected override void UnloadEvents()
        {
            Exiled.Events.Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers1499;
            Exiled.Events.Handlers.Player.MedicalItemUsed -= OnUsedMedicalItem;
            Exiled.Events.Handlers.Player.DroppingItem -= OnItemDropping;
            base.UnloadEvents();
        }


        private void OnWaitingForPlayers1499()
        {
            scp1499Players.Clear();
        }

        private void OnUsedMedicalItem(UsedMedicalItemEventArgs ev)
        {
            if(CheckItem(ev.Player.CurrentItem))
            {
                scp1499Players.Add(ev.Player, ev.Player.Position);

                ev.Player.Position = new Vector3(Scp1499DimensionPos);
                ev.Player.ReferenceHub.playerEffectsController.DisableEffect<CustomPlayerEffects.Scp268>();

                if (Plugin.Singleton.Config.ItemConfigs.Scp1499Cfg.Duration > 0)
                {
                    Timing.CallDelayed(Plugin.Singleton.Config.ItemConfigs.Scp1499Cfg.Duration, () =>
                    {
                        if (scp1499Players.ContainsKey(ev.Player))
                        {
                            ev.Player.Position = scp1499Players[ev.Player];
                            scp1499Players.Remove(ev.Player);
                        }
                    });
                }
            }
        }

        private void OnItemDropping(DroppingItemEventArgs ev)
        {
            if (scp1499Players.ContainsKey(ev.Player))
            {
                ev.IsAllowed = false;

                if(CheckItem(ev.Item))
                {
                    ev.Player.RemoveItem(ev.Item);

                    ev.Player.Position = scp1499Players[ev.Player];
                    GiveItem(ev.Player);

                    scp1499Players.Remove(ev.Player);
                }
            }
        }
    }
}
