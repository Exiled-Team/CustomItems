using System;
using System.Linq;
using Exiled.Loader;
using HarmonyLib;

namespace CustomItems
{
    public class Methods
    {
        private readonly Plugin plugin;
        public Methods(Plugin plugin) => this.plugin = plugin;

        internal void CheckAndPatchSubclassing()
        {
            if (Loader.Plugins.Any(p => p.Name == "Subclass"))
            {
                plugin.HarmonyInstance = new Harmony($"com.galaxy.CI=-{DateTime.UtcNow.Ticks}");
                plugin.HarmonyInstance.PatchAll();
            }
        }
    }
}