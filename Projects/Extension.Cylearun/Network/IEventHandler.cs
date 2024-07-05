using System;
using PatcherYRpp;
using Extension.Ext;
using DynamicPatcher;

namespace Extension.CY.Network
{
    public interface IEventHandler
    {
        byte Index { get; }
        uint Lenth { get; }
        string Name { get; }

        void Respond(Pointer<EventClass> pEvent);
    }
}


