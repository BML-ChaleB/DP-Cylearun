﻿using DynamicPatcher;
using Extension.Components;
using Extension.Decorators;
using Extension.Script;
using Extension.Utilities;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{
    [Serializable]
    public partial class BulletExt : CommonInstanceExtension<BulletExt, BulletClass, BulletTypeExt, BulletTypeClass>
    {
        public BulletExt(Pointer<BulletClass> OwnerObject) : base(OwnerObject)
        {
        }

        public ref AbstractClass BaseAbstract => ref OwnerRef.Base.Base;
        public ref ObjectClass BaseObject => ref OwnerRef.Base;

        public Pointer<AbstractClass> pAbstract => OwnerObject.Convert<AbstractClass>();
        public Pointer<ObjectClass> pObject => OwnerObject.Convert<ObjectClass>();

        public bool HasOwner => OwnerRef.Owner.IsNotNull;
        public ref Pointer<TechnoClass> TechnoOwner => ref OwnerRef.Owner;
        public ref TechnoClass TechnoRef => ref OwnerRef.Owner.Ref;
        public ref HouseClass HouseRef => ref OwnerRef.Owner.Ref.Owner.Ref;
        public Pointer<HouseClass> pHouse => OwnerRef.Owner.Ref.Owner;
        public int HouseIndex => HouseRef.ArrayIndex;
        public CoordStruct Pos { get => BaseAbstract.GetCoords(); set => OwnerRef.Base.SetLocation(value); }
        public ref BulletVelocity Velocity => ref OwnerRef.Velocity;
        public ref CoordStruct SourceCoords => ref OwnerRef.SourceCoords;
        public ref CoordStruct TargetCoords => ref OwnerRef.TargetCoords;

        public Pointer<AbstractClass> pTarget { get => OwnerRef.Target; set => OwnerRef.SetTarget(value); }
        public ref AbstractClass TargetRef => ref pTarget.Ref;
        public bool HasTarget => OwnerRef.Target.IsNotNull;

        public CoordStruct TargetPos => pTarget.Ref.GetCoords();



        public override void SaveToStream(IStream stream)
        {
            base.SaveToStream(stream);
        }

        public override void LoadFromStream(IStream stream)
        {
            base.LoadFromStream(stream);
        }

        //[Hook(HookType.AresHook, Address = 0x4664BA, Size = 5)]
        static public unsafe UInt32 BulletClass_CTOR(REGISTERS* R)
        {
            var pItem = (Pointer<BulletClass>)R->ESI;

            BulletExt.ExtMap.FindOrAllocate(pItem);
            return 0;
        }

        //[Hook(HookType.AresHook, Address = 0x4665E9, Size = 0xA)]
        static public unsafe UInt32 BulletClass_DTOR(REGISTERS* R)
        {
            var pItem = (Pointer<BulletClass>)R->ESI;

            BulletExt.ExtMap.Remove(pItem);
            return 0;
        }

        //[Hook(HookType.AresHook, Address = 0x46AFB0, Size = 8)]
        //[Hook(HookType.AresHook, Address = 0x46AE70, Size = 5)]
        static public unsafe UInt32 BulletClass_SaveLoad_Prefix(REGISTERS* R)
        {
            var pItem = R->Stack<Pointer<BulletClass>>(0x4);
            var pStm = R->Stack<Pointer<IStream>>(0x8);
            IStream stream = Marshal.GetObjectForIUnknown(pStm) as IStream;

            BulletExt.ExtMap.PrepareStream(pItem, stream);
            return 0;
        }

        //[Hook(HookType.AresHook, Address = 0x46AF97, Size = 7)]
        //[Hook(HookType.AresHook, Address = 0x46AF9E, Size = 7)]
        static public unsafe UInt32 BulletClass_Load_Suffix(REGISTERS* R)
        {
            BulletExt.ExtMap.LoadStatic();
            return 0;
        }

        //[Hook(HookType.AresHook, Address = 0x46AFC4, Size = 3)]
        static public unsafe UInt32 BulletClass_Save_Suffix(REGISTERS* R)
        {
            BulletExt.ExtMap.SaveStatic();
            return 0;
        }
    }
}
