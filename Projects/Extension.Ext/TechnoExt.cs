using DynamicPatcher;
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
    public partial class TechnoExt : CommonInstanceExtension<TechnoExt, TechnoClass, TechnoTypeExt, TechnoTypeClass>
    {
        public TechnoExt(Pointer<TechnoClass> OwnerObject) : base(OwnerObject)
        {
        }


        public ref AbstractClass BaseAbstract => ref OwnerRef.BaseAbstract;
        public ref ObjectClass BaseObject => ref OwnerRef.Base;
        public ref MissionClass BaseMission => ref OwnerRef.BaseMission;
        public ref FootClass FootRef => ref OwnerObject.Convert<FootClass>().Ref;
        public ref UnitClass UnitRef => ref OwnerObject.Convert<UnitClass>().Ref;
        public ref InfantryClass InRef => ref OwnerObject.Convert<InfantryClass>().Ref;
        public ref AircraftClass AirRef => ref OwnerObject.Convert<AircraftClass>().Ref;
        public ref BuildingClass BuildingRef => ref OwnerObject.Convert<BuildingClass>().Ref;
        public ref VeterancyStruct Veterancy => ref OwnerRef.Veterancy;
        public ref HouseClass HouseRef => ref OwnerRef.Owner.Ref;
        public int HouseIndex { get => HouseRef.ArrayIndex; }
        public bool Select { get => BaseObject.IsSelected; set => _select(value); }
        public CoordStruct Pos { get => OwnerRef.BaseAbstract.GetCoords(); set => OwnerRef.Base.SetLocation(value); }
        public Pointer<HouseClass> pHouse { get => OwnerRef.Owner; set => OwnerRef.SetOwningHouse(value); }
        public Pointer<AbstractClass> pTarget { get => OwnerRef.Target; set => OwnerRef.SetTarget(value); }
        public Mission CurrentMission { get => BaseMission.CurrentMission; }
        public ref AbstractClass TargetRef => ref pTarget.Ref;
        public ILocomotion Locomotor => FootRef.Locomotor;
        public void _select(bool select)
        {
            if (select)
                BaseObject.Select();
            else
                BaseObject.Deselect();
        }

        public override void OnDeserialization(object sender)
        {
            base.OnDeserialization(sender);
        }
        public override void SaveToStream(IStream stream)
        {
            base.SaveToStream(stream);
        }

        public override void LoadFromStream(IStream stream)
        {
            base.LoadFromStream(stream);
        }

        [OnSerializing]
        protected void OnSerializing(StreamingContext context) { }

        [OnSerialized]
        protected void OnSerialized(StreamingContext context) { }

        [OnDeserializing]
        protected void OnDeserializing(StreamingContext context) { }

        [OnDeserialized]
        protected void OnDeserialized(StreamingContext context) { }


        //[Hook(HookType.AresHook, Address = 0x6F3260, Size = 5)]
        static public unsafe UInt32 TechnoClass_CTOR(REGISTERS* R)
        {
            var pItem = (Pointer<TechnoClass>)R->ESI;

            TechnoExt.ExtMap.FindOrAllocate(pItem);
            return 0;
        }

        //[Hook(HookType.AresHook, Address = 0x6F4500, Size = 5)]
        static public unsafe UInt32 TechnoClass_DTOR(REGISTERS* R)
        {
            var pItem = (Pointer<TechnoClass>)R->ECX;

            TechnoExt.ExtMap.Remove(pItem);
            return 0;
        }

        //[Hook(HookType.AresHook, Address = 0x70C250, Size = 8)]
        //[Hook(HookType.AresHook, Address = 0x70BF50, Size = 5)]
        static public unsafe UInt32 TechnoClass_SaveLoad_Prefix(REGISTERS* R)
        {
            var pItem = R->Stack<Pointer<TechnoClass>>(0x4);
            var pStm = R->Stack<Pointer<IStream>>(0x8);
            IStream stream = Marshal.GetObjectForIUnknown(pStm) as IStream;

            TechnoExt.ExtMap.PrepareStream(pItem, stream);
            return 0;
        }

        //[Hook(HookType.AresHook, Address = 0x70C249, Size = 5)]
        static public unsafe UInt32 TechnoClass_Load_Suffix(REGISTERS* R)
        {
            TechnoExt.ExtMap.LoadStatic();
            return 0;
        }

        //[Hook(HookType.AresHook, Address = 0x70C264, Size = 5)]
        static public unsafe UInt32 TechnoClass_Save_Suffix(REGISTERS* R)
        {
            TechnoExt.ExtMap.SaveStatic();
            return 0;
        }
    }

}
