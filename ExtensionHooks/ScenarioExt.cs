
using DynamicPatcher;
using Extension.Ext;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ExtensionHooks
{
    public class ScenarioExtHooks
    {
        [Hook(HookType.AresHook, Address = 0x683549, Size = 9)]
        static public unsafe UInt32 ScenarioClass_CTOR(REGISTERS* R)
        {
            return ScenarioExt.ScenarioClass_CTOR(R);
        }

        [Hook(HookType.AresHook, Address = 0x6BEB7D, Size = 6)]
        static public unsafe UInt32 ScenarioClass_DTOR(REGISTERS* R)
        {
            return ScenarioExt.ScenarioClass_DTOR(R);
        }

        [Hook(HookType.AresHook, Address = 0x689470, Size = 5)]
        [Hook(HookType.AresHook, Address = 0x689310, Size = 5)]
        static public unsafe UInt32 ScenarioClass_SaveLoad_Prefix(REGISTERS* R)
        {
            return ScenarioExt.ScenarioClass_SaveLoad_Prefix(R);
        }

        [Hook(HookType.AresHook, Address = 0x689669, Size = 6)]
        static public unsafe UInt32 ScenarioClass_Load_Suffix(REGISTERS* R)
        {
            return ScenarioExt.ScenarioClass_Load_Suffix(R);
        }

        [Hook(HookType.AresHook, Address = 0x68945B, Size = 8)]
        static public unsafe UInt32 ScenarioClass_Save_Suffix(REGISTERS* R)
        {
            return ScenarioExt.ScenarioClass_Save_Suffix(R);
        }
    }
}
