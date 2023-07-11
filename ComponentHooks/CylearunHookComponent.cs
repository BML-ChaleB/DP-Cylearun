using DynamicPatcher;
using Extension.Ext;
using Extension.Script;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extension.CY;
using PatcherYRpp.FileFormats;
using static System.Net.Mime.MediaTypeNames;

namespace ComponentHooks
{
    public class CylearunComponentHooks
    {
        [Hook(HookType.AresHook, Address = 0x4DB7F7, Size = 6)]
        public static unsafe UInt32 FootClass_In_Which_Layer(REGISTERS* R)
        {
            Pointer<TechnoClass> pTechno = (IntPtr)(void*)R->ESI;
            TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
            var gscript = ext.GameObject.GetComponent<TechnoGlobalExtension>();
            if (gscript != null)
            {
                var layer = gscript.Data.RenderLayer;

                if (!string.IsNullOrEmpty(layer))
                {
                    if (layer == "air")
                    {
                        R->EAX = (uint)Layer.Air;
                    }
                    else if (layer == "top")
                    {
                        R->EAX = (uint)Layer.Top;
                    }
                    else if (layer == "ground")
                    {
                        R->EAX = (uint)Layer.Ground;
                    }
                    return 0x4DB803;
                }
            }
            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x6FC339, Size = 6)]
        public static unsafe UInt32 TechnoClass_CanFire(REGISTERS* R)
        {
            Pointer<TechnoClass> pTechno = (IntPtr)(void*)R->ESI;
            Pointer<WeaponTypeClass> pWeapon = (IntPtr)(void*)R->EDI;
            Pointer<AbstractClass> pTarget = R->Stack<Pointer<AbstractClass>>(0x20 - (-0x4));
            TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
            var gscript = ext.GameObject.GetComponent<TechnoGlobalExtension>();
            UInt32 cannotFire = 0x6FCB7E;

            if (gscript != null)
            {
                if (!gscript.CanFire(pTarget, pWeapon))
                {
                    return cannotFire;
                }
            }
            return 0;
        }


        //[Hook(HookType.AresHook, Address = 0x73C5FC, Size = 6)]
        //public static unsafe UInt32 UnitClass_DrawSHP(REGISTERS* R)
        //{
        //    Pointer<UnitClass> unit = (IntPtr)(void*)R->EBP;
        //    var techno = unit.Convert<TechnoClass>();
        //    TechnoExt ext = TechnoExt.ExtMap.Find(techno);
        //    //techno.Ref.Base.
        //    //Pointer<SHPStruct> Image = techno.Ref.


        //    //if (UnitTypeClass * pCustomType = pData->GetUnitType())
        //    //{
        //    //    Image = pCustomType->GetImage();
        //    //}

        //    //if (Image)
        //    //{
        //    //    R->EAX<SHPStruct*>(Image);
        //    //    return 0x73C602;
        //    //}
        //    //return 0x73CE00;

        //    return 0;
        //}


     

    }
}
