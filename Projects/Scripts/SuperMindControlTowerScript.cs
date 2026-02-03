using Extension.Ext;
using Extension.Script;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripts
{
    [Serializable]
    [ScriptAlias(nameof(SuperMindControlTowerScript))]
    public class SuperMindControlTowerScript : TechnoScriptable
    {
        public SuperMindControlTowerScript(TechnoExt owner) : base(owner)
        {
        }

        public override void OnPut(CoordStruct coord, Direction faceDir)
        {
            var houseExt = HouseExt.ExtMap.Find(Owner.OwnerObject.Ref.Owner);
            houseExt.SuperMindControlTower = Owner;
        }

        public override void OnRemove()
        {
            var houseExt = HouseExt.ExtMap.Find(Owner.OwnerObject.Ref.Owner);
            houseExt.SuperMindControlTower = null;
        }

    }


    [Serializable]
    [ScriptAlias(nameof(SuperMindControlGuider))]
    public class SuperMindControlGuider : TechnoScriptable
    {
        public SuperMindControlGuider(TechnoExt owner) : base(owner)
        {
        }

        public override void OnFire(Pointer<AbstractClass> pTarget, int weaponIndex)
        {
            var houseExt = HouseExt.ExtMap.Find(Owner.OwnerObject.Ref.Owner); 
            if (!houseExt.SuperMindControlTower.IsNullOrExpired())
            {
                houseExt.SuperMindControlTower.OwnerObject.Ref.Ammo = 1;
                houseExt.SuperMindControlTower.OwnerObject.Ref.SetTarget(pTarget);
                var mission = houseExt.SuperMindControlTower.OwnerObject.Convert<MissionClass>();
                mission.Ref.ForceMission(Mission.Attack);
            }
        }
    }
}
