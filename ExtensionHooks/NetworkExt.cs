
using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DynamicPatcher;
using PatcherYRpp;
using Extension.Ext;
using Extension.CY.Network;

namespace ExtensionHooks
{
    public class EventClassExtHooks
    {

        [Hook(HookType.AresHook, Address = 0x4C6CB0, Size = 0x6)]
        static public unsafe UInt32 Networking_RespondToEvent(REGISTERS* R)
        {
            return EventClassExt.EventClass_RespondToEvent(R);
        }

        [Hook(HookType.AresHook, Address = 0x4C65EF, Size = 0x7)]
        static public unsafe UInt32 Networking_sub_4C65E0_Log(REGISTERS* R)
        {
            return EventClassExt.sub_4C65E0_Log(R);
        }

        [Hook(HookType.AresHook, Address = 0x64BE7D, Size = 0x6)]
        static public unsafe UInt32 Networking_sub_64BDD0_GetEventSize1(REGISTERS* R)
        {
            return EventClassExt.sub_64BDD0_GetEventSize1(R);
        }

        [Hook(HookType.AresHook, Address = 0x64C30E, Size = 0x6)]
        static public unsafe UInt32 Networking_sub_64BDD0_GetEventSize2(REGISTERS* R)
        {
            return EventClassExt.sub_64BDD0_GetEventSize2(R);
        }

        [Hook(HookType.AresHook, Address = 0x64B6FE, Size = 0x6)]
        static public unsafe UInt32 Networking_sub_64B660_GetEventSize(REGISTERS* R)
        {
            return EventClassExt.sub_64B660_GetEventSize(R);
        }
    }
}