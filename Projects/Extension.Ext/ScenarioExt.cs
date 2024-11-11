using Extension.Components;
using Extension.INI;
using Microsoft.Win32;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DynamicPatcher;

namespace Extension.Ext
{
    [Serializable]
    public partial class ScenarioExt : SingleExtension<ScenarioExt, ScenarioClass>
    {
        public ScenarioExt(Pointer<ScenarioClass> OwnerObject) : base(OwnerObject) { }
        //[Hook(0x683549, 9)]
        static public unsafe UInt32 ScenarioClass_CTOR(REGISTERS* R)
        {
            var pItem = (Pointer<ScenarioClass>)R->EAX;
            ScenarioExt.ExtMap.FindOrAllocate(pItem);
            return 0;
        }

        //[Hook(0x6BEB7D, 6)]
        static public unsafe UInt32 ScenarioClass_DTOR(REGISTERS* R)
        {
            var pItem = (Pointer<ScenarioClass>)R->ESI;

            ScenarioExt.ExtMap.Remove(pItem);
            return 0;
        }

        //[Hook(0x689470, 5)]
        //[Hook(0x689310, 5)]
        static public unsafe UInt32 ScenarioClass_SaveLoad_Prefix(REGISTERS* R)
        {
            var pStm = R->Stack<Pointer<IStream>>(0x4);
            IStream stream = (Marshal.GetObjectForIUnknown(pStm) as IStream);

            ScenarioExt.ExtMap.PrepareStream(new Pointer<Pointer<ScenarioClass>>(0xA8B230).Data, stream);
            return 0;
        }

        //[Hook(0x689669, 6)]
        static public unsafe UInt32 ScenarioClass_Load_Suffix(REGISTERS* R)
        {
            ScenarioExt.ExtMap.LoadStatic();
            return 0;
        }

        //[Hook(0x68945B, 8)]
        static public unsafe UInt32 ScenarioClass_Save_Suffix(REGISTERS* R)
        {
            ScenarioExt.ExtMap.SaveStatic();
            return 0;
        }

        public override void OnAwake(GameObject gameObject)
        {
        }
    }
}
