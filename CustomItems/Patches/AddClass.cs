using Exiled.API.Features;
using HarmonyLib;
using MEC;
using Subclass;

namespace CustomItems.Patches
{
    [HarmonyPatch(typeof(TrackingAndMethods), nameof(TrackingAndMethods.AddClass))]
    public class AddClass
    {
        public static void Postfix(Player player, SubClass subClass, bool is035 = false, bool lite = false,
            bool escaped = false, bool disguised = false)
        {
            if (Plugin.Singleton.Config.SniperList.Contains(subClass.Name))
                Timing.RunCoroutine(Plugin.Singleton.Methods.GiveSniper(player));
        }
    }
}