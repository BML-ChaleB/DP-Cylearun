
using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DynamicPatcher;
using PatcherYRpp;
using Extension.Ext;
using Extension.Script;
using Extension.CY;
using Scripts;

namespace ExtensionHooks
{
    public class TechnoExtHooks
    {
        [Hook(HookType.AresHook, Address = 0x6F3260, Size = 5)]
        static public unsafe UInt32 TechnoClass_CTOR(REGISTERS* R)
        {
            return TechnoExt.TechnoClass_CTOR(R);
        }

        [Hook(HookType.AresHook, Address = 0x6F4500, Size = 5)]
        static public unsafe UInt32 TechnoClass_DTOR(REGISTERS* R)
        {
            return TechnoExt.TechnoClass_DTOR(R);
        }
        
        [Hook(HookType.AresHook, Address = 0x70C250, Size = 8)]
        [Hook(HookType.AresHook, Address = 0x70BF50, Size = 5)]
        static public unsafe UInt32 TechnoClass_SaveLoad_Prefix(REGISTERS* R)
        {
            return TechnoExt.TechnoClass_SaveLoad_Prefix(R);
        }

        [Hook(HookType.AresHook, Address = 0x70C249, Size = 5)]
        static public unsafe UInt32 TechnoClass_Load_Suffix(REGISTERS* R)
        {
            return TechnoExt.TechnoClass_Load_Suffix(R);
        }

        [Hook(HookType.AresHook, Address = 0x70C264, Size = 5)]
        static public unsafe UInt32 TechnoClass_Save_Suffix(REGISTERS* R)
        {
            return TechnoExt.TechnoClass_Save_Suffix(R);
        }

                [Hook(HookType.AresHook, Address = 0x4CCB42, Size = 5)]
        public static unsafe UInt32 FlyLocomotionClass_Skip1(REGISTERS* R)
        {

            Pointer<Pointer<FootClass>> ppFoot = (IntPtr)(void*)R->ESP + 0x8 + 0x4 + 0x8;

            Pointer<TechnoClass> pTechno = ppFoot.Ref.Convert<TechnoClass>();

            TechnoExt ext = null;
            if (pTechno.IsNotNull && !SpawnScript.IsDeadOrInvisible(pTechno) && null != (ext = TechnoExt.ExtMap.Find(pTechno)) && ext.GameObject.TryGetComponent(out SpawnScript Comp) && Comp.InLanding)
            {
                return 0x4CCBCF;
            }

            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x6B74F0, Size = 10)]
        public static unsafe UInt32 BuildingClass_Sell_SetSpeedType2(REGISTERS* R)
        {
            Pointer<SpawnManagerClass> pSpawnManager = (IntPtr)(void*)R->ESI;

            uint Index = R->EBX;

            Pointer<TechnoClass> sth = pSpawnManager.Ref.SpawnedNodes[(int)Index].Ref.Unit;
            var ext = TechnoExt.ExtMap.Find(sth);
            if(null != ext && ext.GameObject.TryGetComponent(out SpawnScript Comp))
            {
                Comp.SpawnIndex = (int)Index;
            }
            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x43E7B0, Size = 5)]
        public static unsafe UInt32 BuildingClass_DrawVisible(REGISTERS* R)
        {
            Pointer<BuildingClass> pThis = (IntPtr)R->ECX;

            TechnoExt ext = TechnoExt.ExtMap.Find(pThis.Cast<TechnoClass>());
            var gscript = ext.GameObject.GetComponent<TechnoGlobalExtension>();
            if (gscript != null)
            {
                gscript.DrawFactoryProcess(R);
            }

            return 0;
        }


    }
}