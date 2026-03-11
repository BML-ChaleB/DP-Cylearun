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

                        var cell = CellClass.Coord2Cell(coord);

                        if (!string.IsNullOrWhiteSpace(ini.Data.Offset))
                        {
                            var offsetStr = ini.Data.Offset;
                            var offset = offsetStr.Split(',').Select(x => int.Parse(x)).ToList();
                            cell = cell + new CellStruct(offset[0], offset[1]);
                        }

                        var health = Owner.OwnerObject.Ref.Base.Health;
                        var strength = Owner.OwnerObject.Ref.Type.Ref.Base.Strength;
                        var veterancy = Owner.OwnerObject.Ref.Veterancy.Veterancy;

                        var type = TechnoTypeClass.ABSTRACTTYPE_ARRAY.Find(ini.Data.ConvertTo);
                        var techno = type.Ref.Base.CreateObject(Owner.OwnerObject.Ref.Owner).Convert<TechnoClass>();

                        var placeResult = TechnoPlacer.PlaceTechnoNear(techno,cell,ini.Data.BuildUp);
                        placed = true;
                        if(ini.Data.Inhert && placeResult)
                        {
                            techno.Ref.Veterancy.Veterancy = veterancy;
                            techno.Ref.Base.Health = (int)Math.Round(techno.Ref.Type.Ref.Base.Strength * ((double)health / (double)strength));

                            var passengers = new List<Pointer<FootClass>>();

                            if (Owner.OwnerObject.Ref.Passengers.NumPassengers > 0) 
                            {
                                while(Owner.OwnerObject.Ref.Passengers.GetFirstPassenger().IsNotNull)
                                {
                                    var pfoot = Owner.OwnerObject.Ref.Passengers.GetFirstPassenger();
                                    Owner.OwnerObject.Ref.Passengers.RemoveFirstPassenger();
                                    passengers.Add(pfoot);
                                }

                                if (passengers.Count > 0) 
                                {
                                    passengers.Reverse();
                                    foreach (var passenger in passengers)
                                    {
                                        techno.Ref.AddPassenger(passenger);
                                    }
                                }
                            }

                        }
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

        [INIField(Key = "AttachEffectScript.Offset")]
        public string Offset = string.Empty;
        /// <summary>
        /// 是否继承血量等级
        /// </summary>
        [INIField(Key = "AttachEffectScript.Inhert")]
        public bool Inhert = true;
    }
}
