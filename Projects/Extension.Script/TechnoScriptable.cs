using Extension.Ext;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extension.INI;

namespace Extension.Script
{
    public interface ITechnoScriptable : IObjectScriptable
    {
        void OnFire(Pointer<AbstractClass> pTarget, int weaponIndex);

        void OnStopCommand();
    }

    [Serializable]
    public abstract class TechnoScriptable : Scriptable<TechnoExt>, ITechnoScriptable
    {
        public TechnoScriptable(TechnoExt owner) : base(owner)
        {
        }

        public virtual void OnPut(CoordStruct coord, Direction faceDir) { }
        public virtual void OnRemove() { }
        public virtual void OnReceiveDamage(Pointer<int> pDamage, int DistanceFromEpicenter, Pointer<WarheadTypeClass> pWH,
            Pointer<ObjectClass> pAttacker, bool IgnoreDefenses, bool PreventPassengerEscape, Pointer<HouseClass> pAttackingHouse)
        { }

        public virtual void OnFire(Pointer<AbstractClass> pTarget, int weaponIndex) { }

        public virtual void OnStopCommand()
        {

        }
    }

    [Serializable]
    public class TechnoScriptable<TData> : TechnoScriptable where TData : INIAutoConfig, new()
    {
        public TechnoScriptable(TechnoExt owner) : base(owner)
        {
            Data = this.CreateRulesIniComponentWith<TData>(Owner.OwnerObject.Ref.Type.Ref.Base.Base.ID).Data;
        }
        public TData Data;

    }
}
