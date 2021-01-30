using System;
using Exiled.API.Features;
using Exiled.Events.Extensions;
using Subclass;

namespace CustomItems.Events
{
    public class AddClassEvent
    {
        public static event Exiled.Events.Events.CustomEventHandler<AddClassEventArgs> AddClass;
        public static void OnAddingClass(AddClassEventArgs ev) => AddClass.InvokeSafely(ev);
    }

    public class AddClassEventArgs : EventArgs
    {
        public AddClassEventArgs(Player player, SubClass subClass)
        {
            Player = player;
            Subclass = subClass;
        }
        
        public Player Player { get; }
        public SubClass Subclass { get; }
    }
}