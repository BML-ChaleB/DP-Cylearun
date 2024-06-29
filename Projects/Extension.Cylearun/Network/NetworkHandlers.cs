using System;
using PatcherYRpp;
using Extension.Ext;
using DynamicPatcher;
using System.Runtime.InteropServices;
using Extension.Utilities;
using uint8_t = System.Byte;

namespace Extension.CY.Network
{
    public static class Handlers
    {

        [StructLayout(LayoutKind.Explicit, Size = 104)]
        public struct EventHandlers
        {
            [FieldOffset(0)] public DummyData DummyData;

        }

        [StructLayout(LayoutKind.Explicit, Size = 104)]
        public struct DummyData
        {
            [FieldOffset(0)] public int sth;

        }

        public static Pointer<EventHandlers> GetHandlers(this Pointer<EventClass> pEvent)
        {
            return new IntPtr((uint)pEvent + 7);
        }

        public static Pointer<T> GetHandlers<T>(this Pointer<EventClass> pEvent)
        {
            return new IntPtr((uint)pEvent + 7);
        }



    }
    public static class HandlerHelps
    {

    }
}     


