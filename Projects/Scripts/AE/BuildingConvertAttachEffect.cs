using Extension.Ext;
using Extension.INI;
using Extension.Script;
using PatcherYRpp;
using PatcherYRpp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Scripts.AE
{
    [ScriptAlias(nameof(BuildingConvertAttachEffect))]
    [Serializable]
    public class BuildingConvertAttachEffect : AttachEffectScriptable
    {
        public BuildingConvertAttachEffect(TechnoExt owner) : base(owner)
        {
        }


        private string warhead = string.Empty;

        private bool placed = false;

        public override void OnUpdate()
        {
            if (Duration <= 0)
            {
                bool shouldConvert = true;

                if (!string.IsNullOrWhiteSpace(warhead))
                {
                    var ini = Owner.GameObject.CreateRulesIniComponentWith<BuildingConvertAttachEffectData>(warhead);

                    if(!string.IsNullOrWhiteSpace(ini.Data.Check))
                    {
                        var allowed = ini.Data.Check.Split(',').ToList();
                        if(!allowed.Contains(Owner.OwnerObject.Ref.Type.Ref.Base.Base.ID))
                        {
                            shouldConvert = false;
                        }
                    }

                    if (shouldConvert && !placed)
                    {
                        var coord = Owner.OwnerObject.Ref.Base.Base.GetCoords();
                        Owner.OwnerObject.Ref.Base.Remove();
                        TechnoPlacer.PlaceTechnoNear(TechnoTypeClass.ABSTRACTTYPE_ARRAY.Find(ini.Data.ConvertTo), Owner.OwnerObject.Ref.Owner,CellClass.Coord2Cell(coord),ini.Data.BuildUp);
                        placed = true;
                        Owner.OwnerObject.Ref.Base.UnInit();
                    }
                }
            }
            base.OnUpdate();
        }


        public override void OnAttachEffectPut(Pointer<int> pDamage, Pointer<WarheadTypeClass> pWH, Pointer<ObjectClass> pAttacker, Pointer<HouseClass> pAttackingHouse)
        {
            warhead = pWH.Ref.Base.ID;
            base.OnAttachEffectPut(pDamage, pWH, pAttacker, pAttackingHouse);
        }

        public override void OnAttachEffectRecieveNew(int duration, Pointer<int> pDamage, Pointer<WarheadTypeClass> pWH, Pointer<ObjectClass> pAttacker, Pointer<HouseClass> pAttackingHouse)
        {
            return;
        }
    }

    public partial class BuildingConvertAttachEffectData : INIAutoConfig
    {
        [INIField(Key = "AttachEffectScript.ConvertTo")]
        public string ConvertTo = "";

        [INIField(Key = "AttachEffectScript.CheckType")]
        public string Check = "";

        [INIField(Key = "AttachEffectScript.BuildUp")]
        public bool BuildUp = true;
    }
}
