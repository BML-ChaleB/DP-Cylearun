﻿using DynamicPatcher;
using Extension.Script;
using Extension.Utilities;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Extension.INI;

namespace Extension.Ext
{
    [Serializable]
    public partial class TechnoTypeExt : CommonTypeExtension<TechnoTypeExt, TechnoTypeClass>
    {
        public TechnoTypeExt(Pointer<TechnoTypeClass> OwnerObject) : base(OwnerObject)
        {

        }

        protected override void LoadFromINI(INIReader reader)
        {
            base.LoadFromINI(reader);

            string section = OwnerObject.Ref.BaseAbstractType.ID;

        }

        public override void SaveToStream(IStream stream)
        {
            base.SaveToStream(stream);
        }
        public override void LoadFromStream(IStream stream)
        {
            base.LoadFromStream(stream);
        }

        //[Hook(HookType.AresHook, Address = 0x711835, Size = 5)]
        static public unsafe UInt32 TechnoTypeClass_CTOR(REGISTERS* R)
        {
            var pItem = (Pointer<TechnoTypeClass>)R->ESI;

            TechnoTypeExt.ExtMap.FindOrAllocate(pItem);
            return 0;
        }

        //[Hook(HookType.AresHook, Address = 0x711AE0, Size = 5)]
        static public unsafe UInt32 TechnoTypeClass_DTOR(REGISTERS* R)
        {
            var pItem = (Pointer<TechnoTypeClass>)R->ECX;

            TechnoTypeExt.ExtMap.Remove(pItem);
            return 0;
        }

        //[Hook(HookType.AresHook, Address = 0x716132, Size = 5)]
        //[Hook(HookType.AresHook, Address = 0x716123, Size = 5)]
        static public unsafe UInt32 TechnoTypeClass_LoadFromINI(REGISTERS* R)
        {
            var pItem = (Pointer<TechnoTypeClass>)R->EBP;
            var pINI = R->Stack<Pointer<CCINIClass>>(0x380);

            TechnoTypeExt.ExtMap.LoadFromINI(pItem, pINI);
            return 0;
        }

        //[Hook(HookType.AresHook, Address = 0x716DC0, Size = 5)]
        //[Hook(HookType.AresHook, Address = 0x7162F0, Size = 6)]
        static public unsafe UInt32 TechnoTypeClass_SaveLoad_Prefix(REGISTERS* R)
        {
            var pItem = R->Stack<Pointer<TechnoTypeClass>>(0x4);
            var pStm = R->Stack<Pointer<IStream>>(0x8);
            IStream stream = Marshal.GetObjectForIUnknown(pStm) as IStream;

            TechnoTypeExt.ExtMap.PrepareStream(pItem, stream);
            return 0;
        }

        //[Hook(HookType.AresHook, Address = 0x716DAC, Size = 0xA)]
        static public unsafe UInt32 TechnoTypeClass_Load_Suffix(REGISTERS* R)
        {
            TechnoTypeExt.ExtMap.LoadStatic();
            return 0;
        }

        //[Hook(HookType.AresHook, Address = 0x717094, Size = 5)]
        static public unsafe UInt32 TechnoTypeClass_Save_Suffix(REGISTERS* R)
        {
            TechnoTypeExt.ExtMap.SaveStatic();
            return 0;
        }
        private IConfigWrapper<TechnoArtConfig> m_artData;
        public TechnoArtConfig ArtData
        {
            get
            {
                if (m_artData == null)
                {
                    m_artData = Ini.GetConfig<TechnoArtConfig>(Ini.ArtDependency, Data.Image == null ? OwnerRef.BaseAbstractType.ID : Data.Image);
                }
                return m_artData.Data;
            }
        }

        private IConfigWrapper<TechnoRulesConfig> m_data;
        public TechnoRulesConfig Data
        {
            get
            {
                if (m_data == null)
                {
                    m_data = Ini.GetConfig<TechnoRulesConfig>(Ini.RulesDependency, OwnerRef.BaseAbstractType.ID);
                }
                return m_data.Data;
            }
        }

    }

    [Serializable]
    public class TechnoArtConfig : INIAutoConfig
    {
        [INIField(Key = "Cameo")]
        public string Cameo;
        [INIField(Key = "AltCameo")]
        public string AltCameo;

        [INIField(Key = "CameoPCX")]
        public string CameoPCX;
        [INIField(Key = "AltCameoPCX")]
        public string AltCameoPCX;

    }

    [Serializable]
    public class TechnoRulesConfig : INIAutoConfig
    {
        [INIField(Key = "Image")]

        public string Image;

    }

}
