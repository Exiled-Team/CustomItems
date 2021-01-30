using CustomItems.Events;
using Exiled.API.Features;
using HarmonyLib;
using Subclass;

namespace CustomItems.Patches
{
    [HarmonyPatch(typeof(TrackingAndMethods), nameof(TrackingAndMethods.AddClass))]
    public class AddClass
    {
        public static void Postfix(Player player, SubClass subClass, bool is035 = false, bool lite = false,
            bool escaped = false, bool disguised = false)
        {
            AddClassEventArgs ev = new AddClassEventArgs(player, subClass);
            AddClassEvent.OnAddingClass(ev);
        }
    }
}