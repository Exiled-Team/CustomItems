# CustimItems
======
## Description
This plugin has 2 parts, all contained in a single DLL with no needed dependencies.
Firstly, This plugin adds a number of custom items (detailed below) for server hosts to use on their servers. Currently, these items can either be given to Subclasses (using: [Advanced Subclassing](https://github.com/steven4547466/AdvancedSubclassing)), spawned around the map at pre-defined locations (valid locations listed below), or via commands.
Secondly, This plugin also provides a very powerful API for other developers to use, for the creation of their own unique, custom items. This API includes item tracking, handles reloading weapons with non-standard clip sizes, and much, much more, automatically, with no concious effort from the developer using it. Overrideable methods are provided to hook your weapon into your own custom event handlers for handling it's logic as necessary.

### Item list
ItemName | ItemID | Description
:---: | :---: | :------
SG-119 | 1 | A shotgun. Fairly self-explanatory.
GL-119 | 2 | A grenade launcher. This weapon shoots grenades that explode on impact with anything, instead of bullets.
SR-119 | 3 | A sniper rifle. Also self-explanatory.
SCP-127 | 4 | A gun that slowly regenerates it's clip over time, but trying to reload it normally has no effect.
IM-119 | 5 | An Implosion Grenade. This grenade will act similar to a normal Frag grenade, however it has an extremely short fuse, and does very low damage. Upon exploding, anyone within the explosion will be quickly drawn in towards the center of the explosion for a few seconds.
EM-119 | 6 | An EMP Grenade. This grenade acts similar to an Implosion grenade, however when it detonates, all of the doors in the room it is in are locked open, and the lights disabled for a few seconds. If SCP-079 is present in the room, it will send him back to his spawn camera. Also disabled all speakers in the facility temporarily. 
LJ-119 | 7 | An injection of lethal chemicals that, when injected, immediately kills the user. If the user happens to be the target of a currently enraged SCP-096, the SCP-096 will immediately calm down, regardless of how many other targets they may or may not have.
MG-119 | 8 | This gun is modified to fire self-injecting projectile darts. When fired at friendly targets, it will heal them. When fired at SCP-049-2, it will slowly begin to 'cure' them, repeated applications will eventually revert the SCP-049-2 to their human state. Has no effect on other hostile targets.
TG-119 | 9 | This gun is also modified to fire self-injecting projectile darts. When fired at a hostile target, it will tranquilize them, rendering them unconcious for several seconds.
LC-119 | 10 | This coin, when dropped while inside the Pocket Dimension, will immediately vanish. For the remainder of the round, whenever a player enters the Pocket Dimension, the coin will spawn infront of one of the correct entrances for a few seconds before vanishing again. This effect has a cooldown.

### Item Configs
Config settings for the individual items will ***NOT*** be found in the default plugin config file. Instead they will be located in ~/.config/EXILED/Configs/CustomItems on Linux or %AppData%\EXILED\Configs\CustomItems on Winblows.
The default config file will be named "global.yml" however, the file used can be changed for each SCP server via that server's normal plugin config file, if you wish to run multiple servers with different custom item config settings.

Currently the item names, IDs and Descriptions are unable to be edited. I may add this ability in the future. The actual config values for the items should have descriptions and names that make them self-explanatory.

### Valid Spawn Location names
The following list of locations are the only ones that are able to be used in the SpawnLocation configs for each item:
(Their names must be typed EXACTLY as they are listed, otherwise you will probably break your item config file)
Inside012
Inside012Bottom
Inside012Locker
Inside049Armory
Inside079Secondary
Inside096
Inside173Armory
Inside173Bottom
Inside173Connector
InsideEscapePrimary
InsideEscapeSecondary
InsideIntercom
InsideLczArmory
InsideLczCafe
InsideNukeArmory
InsideSurfaceNuke
Inside079First
Inside173Gate
Inside914
InsideGateA
InsideGateB
InsideGr18
InsideHczArmory
InsideHid
InsideHidLeft
InsideHidRight
InsideLczWc
InsideServersBottom

### API Notes **(FOR DEVELOPERS ONLY)**
Tl;dr - All you need to do is make a class that inherits CustomItem, CustomWeapon or CustomGrenade (depending on what kind of item you're making), override LoadEvents() and UnloadEvents() to register event handlers to the weapon, then inside the event handlers, start everything with
```c#
if (CheckItem(ev.Player.CurrentItem))
```
CheckItem() will accept both a SyncItemInfo or a Pickup, and will return true if it's the custom item THAT BELONGS TO YOUR CLASS. Note that your class is considered a MANAGER, not an individual weapon.
Then instantiate the new class and register it as a custom item:
```c#
new SomeItem(ItemType, int).RegisterCustomItem();
```
The parameters needed will depend on what kind of item you're making, weapons will also require you to provide a clip size in the constructor.

**Long version:**

Each item class is a manager for all items of that type on the server. 
When you instantiate a new class that inherits from CustomItem, CustomWeapon or CustomGrenade, it will create a new list of both SyncItemInfos and Pickups. CheckItem() will check those lists to see if the object checked is in one of those lists. If it returns true, it's an item your manager is in charge of. If it returns false, it's not your item.

You cannot cast SyncItemInfo or Pickup as a custom item - IE:
```c#
TranqGun tranqGun = (TranqGun)ev.Player.CurrentItem();
```
Will result in an invalid cast exception. If you need to check if an item belongs to specific manager other than the class you're in already, do this:
```c#
foreach (CustomItem item in CustomItems.API.API.GetInstalledItems())
    if (item.ItemName == "TG-119" && item.CheckItem(ev.Player.CurrentItem())
    {
        // do stuff
        break;
    }
```

To create your own custom item, you'll first want to create the class, an example:
```c#
public class SomeItem : CustomItem
{
    public SomeItem(ItemType type, int id) : base(type,id)
    {}
    
    public override string ItemName = "SomeItemName";
    protected override string ItemDescription = "Some description of what your item is/does.";
    //If you want this item to have custom spawn locations on the map
    protected override Dictionary<SpawnLocation, float> SpawnLocations = YourPlugin.Singleton.Config.SpawnLocations;
    
    protected override void LoadEvents()
    {
        Exiled.Events.Handlers.Player.SomeEvent += OnSomeEvent;
        base.LoadEvents();
    }
    protected override void UnloadEvents()
    {
       Exiled.Events.Handlers.Player.SomeEvent -= OnSomeEvent;
       base.UnloadEvents();
    }
      
    public void OnSomeEvent(SomeEventEventArgs ev)
    {
      if (CheckItem(ev.Player.CurrentItem))
      {
          // this is your item, do whatever you want with it.
      }
    }
}
```

And then you would register it either in a way similar to how this plugin registers it's own custom items, or alternatively:
```c#
public Plugin : Plugin<Config>
{
    public override void OnEnabled()
    {
        Timing.CallDelayed(5f, () => new SomeItem(ItemType.SomeType, 42).RegisterCustomItem());
    }
}
```

Do note, if you're registering your item in your plugin's OnEnabled() method, you MUST add a minimum of 5sec delay before registering, to ensure all other plugins are loaded and that the server process has initialized all of it's components and such.