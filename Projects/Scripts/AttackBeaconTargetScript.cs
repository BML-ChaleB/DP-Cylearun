using Extension.Cylearun.Utils;
using Extension.Ext;
using Extension.INI;
using Extension.Script;
using Extension.Utilities;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DpLib.Scripts.China
{
    [Serializable]
    [ScriptAlias(nameof(AttackBeaconTargetScript))]
    public class AttackBeaconTargetScript : TechnoScriptable
    {
        public AttackBeaconTargetScript(TechnoExt owner) : base(owner) { }

        private bool Inited = false;

        INIComponentWith<AttackBeaconTargetData> ini;

        public override void Awake()
        {
            ini = this.CreateRulesIniComponentWith<AttackBeaconTargetData>(Owner.OwnerObject.Ref.Type.Ref.Base.Base.ID);
        }


        public override void OnUpdate()
        {
            base.OnUpdate();

            var location = Owner.OwnerObject.Ref.Base.Base.GetCoords();

            var attackerTypes = ini.Data.Attacker.Split(',').ToList();
            var range = ini.Data.Range;

            if (Inited == false)
            {
                Inited = true;
                //计算水平距离
                var technos = Finder.FindTechno(Owner.OwnerObject.Ref.Owner, t => 
                {

                    if (!attackerTypes.Contains(t.Ref.Type.Ref.Base.Base.ID))
                        return false;

                    if (t.Ref.Base.InLimbo)
                        return false;

                    if (range > 0)
                    {
                        if(t.Ref.Base.Base.GetCoords().BigDistanceForm(new CoordStruct(location.X, location.Y, t.Ref.Base.Base.GetCoords().Z)) > range * Game.CellSize)
                        {
                            return false;
                        }
                    }
                    return true;
                }
                ,FindRange.Owner);

                if (technos != null && technos.Count() > 0)
                {
                    for (var i = technos.Count - 1; i >= 0; i--)
                    {
                        var techno = technos[i];
                        techno.OwnerObject.Ref.SetTarget(default);
                        if (!techno.IsNullOrExpired())
                        {
                            techno.OwnerObject.Ref.SetTarget(Owner.OwnerObject.Convert<AbstractClass>());
                        }
                    }
                }
            }
        }



    }

    public class AttackBeaconTargetData : INIAutoConfig
    {
        /// <summary>
        /// 检索范围
        /// </summary>
        [INIField(Key = "AttackBeacon.Range")]
        public uint Range = 5;

        /// <summary>
        /// 检索延迟
        /// </summary>
        [INIField(Key = "AttackBeacon.Attacker")]
        public string Attacker = "";
    }
}
