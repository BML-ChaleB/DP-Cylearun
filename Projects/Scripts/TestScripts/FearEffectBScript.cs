using Extension.Ext;
using Extension.Script;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripts.TestScripts
{
    [Serializable]
    [ScriptAlias(nameof(FearEffectBScript))]
    public class FearEffectBScript : BulletScriptable
    {
        public FearEffectBScript(BulletExt owner) : base(owner){ }

        public override void OnDetonate(Pointer<CoordStruct> pCoords)
        {
            Pointer<BulletClass> ownerBullet = Owner.OwnerObject;
            Pointer<TechnoClass> ownerTechno = ownerBullet.Ref.Owner;
            CoordStruct detonateCoords = pCoords.Ref;


            base.OnDetonate(pCoords);
        }
    }
}
