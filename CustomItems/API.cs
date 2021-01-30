using CustomItems.Components;
using Exiled.API.Features;

namespace CustomItems
{
    public static class API
    {
        public static void RegisterCustomItem(CustomItem item)
        {
            if (!Plugin.Singleton.ItemManagers.Contains(item))
            {
                if (item.ItemName.Contains(":"))
                {
                    string newName = item.ItemName.Replace(":", "");
                    Log.Warn($"{item.ItemName} contains an invalid character and will be renamed to {newName}");
                    item.ItemName = newName;
                }
                Plugin.Singleton.ItemManagers.Add(item);
                item.Init();
            }
        }
    }
}