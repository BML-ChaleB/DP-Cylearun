using Extension.Ext;
using Extension.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripts
{
    [Serializable]
    [ScriptAlias(nameof(SyncSpawnToAmmoScript))]
    public class SyncSpawnToAmmoScript : TechnoScriptable
    {
        public SyncSpawnToAmmoScript(TechnoExt owner) : base(owner)
        {
        }

        public override void OnUpdate()
        {
            if(Owner.OwnerObject.Ref.SpawnManager.IsNull)
                return;

            Owner.OwnerObject.Ref.Ammo = Owner.OwnerObject.Ref.SpawnManager.Ref.DrawState();
        }
    }
}
