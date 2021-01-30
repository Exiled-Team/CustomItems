using CustomItems.Components;

namespace CustomItems
{
    public static class API
    {
        public static void RegisterCustomItem(CustomItem item)
        {
            if (!Plugin.Singleton.ItemManagers.Contains(item))
            {
                Plugin.Singleton.ItemManagers.Add(item);
                item.Init();
            }
        }
    }
}