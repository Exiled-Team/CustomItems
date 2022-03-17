namespace CustomItems.Patches
{
    using System.Linq;
    using Exiled.API.Features;
    using HarmonyLib;
    using InventorySystem.Items.Radio;
    using UnityEngine;

    [HarmonyPatch(typeof(RadioItem), nameof(RadioItem.ServerProcessCmd))]
    internal static class C4Detonator
    {
        private static void Postfix(RadioItem __instance, RadioMessages.RadioCommand command)
        {
            Player player = Player.Get(__instance.Owner);

            if (command != RadioMessages.RadioCommand.ChangeRange && player.IsSneaking)
            {
                int i = 0;

                foreach (var charge in Items.C4Charge.PlacedCharges.ToList())
                {
                    if (charge.Value != player)
                        continue;

                    float distance = Vector3.Distance(charge.Key.Position, player.Position);

                    if (distance < Items.C4Charge.Instance.MaxDistance)
                    {
                        Items.C4Charge.Instance.C4Handler(charge.Key);
                        i++;
                    }
                    else
                    {
                        player.SendConsoleMessage($"One of your charges is out of range. You need to get closer by {Mathf.Round(distance - Items.C4Charge.Instance.MaxDistance)} meters.", "yellow");
                    }
                }

                if (i == 1)
                    player.ShowHint($"<color=green>{i} C4 charge has been detonated!</color>");
                else
                    player.ShowHint($"<color=green>{i} C4 charges have been deonated!</color>");
            }
        }
    }
}
