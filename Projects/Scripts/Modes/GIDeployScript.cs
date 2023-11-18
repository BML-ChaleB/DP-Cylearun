using DynamicPatcher;
using Extension.Ext;
using Extension.Script;
using PatcherYRpp;
using System;

namespace Script
{
    [ScriptAlias(nameof(GIDeployScript))]
    [Serializable]
    public class GIDeployScript : TechnoScriptable
    {

        public GIDeployScript(TechnoExt owner) : base(owner)
        {
        }

        private int delay = -1;

        private bool starterd = false;

        public override void OnUpdate()
        {
            var mission = Owner.OwnerObject.Convert<MissionClass>();


            if(!starterd) 
            {
                if (mission.Ref.CurrentMission == Mission.Hunt)
                    starterd = true;
                else
                    return;
            }

            if (delay > 0)
            {
                delay--;
                if (delay <= 0)
                {
                    mission.Ref.ForceMission(Mission.Unload);
                }
            }
            if(Owner.OwnerObject.Ref.Target.IsNull && mission.Ref.CurrentMission != Mission.Unload)
            {
                mission.Ref.ForceMission(Mission.Hunt);
            }
        }

        public override void OnFire(Pointer<AbstractClass> pTarget, int weaponIndex)
        {
            if (weaponIndex == 0)
            {
                var mission = Owner.OwnerObject.Convert<MissionClass>();
                mission.Ref.ForceMission(Mission.Unload);
                delay = 300;
            }
            base.OnFire(pTarget, weaponIndex);
        }
    }
}
