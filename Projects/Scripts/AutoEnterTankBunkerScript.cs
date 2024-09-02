using Extension.Ext;
using Extension.INI;
using Extension.Script;
using PatcherYRpp;
using PatcherYRpp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripts
{
    [ScriptAlias(nameof(AutoEnterTankBunkerScript))]
    [Serializable]
    public class AutoEnterTankBunkerScript : TechnoScriptable
    {
        public AutoEnterTankBunkerScript(TechnoExt owner) : base(owner)
        {
        }

        INIComponentWith<AutoEnterTankBunkerData> dataINI;

        public override void Awake()
        {
            dataINI = this.CreateRulesIniComponentWith<AutoEnterTankBunkerData>(Owner.OwnerObject.Ref.Type.Ref.Base.Base.ID);
        }

        public override void OnPut(CoordStruct coord, Direction faceDir)
        {
            base.OnPut(coord, faceDir);
        }

        private int delay = 60;
        public override void OnUpdate()
        {
            if (delay < 0)
            {
                return;
            }

            delay--;
            if (delay == 0)
            {
                if (string.IsNullOrWhiteSpace(dataINI.Data.FreeUnit))
                    return;

                if(MapClass.Instance.TryGetCellAt(Owner.OwnerObject.Ref.Base.Base.GetCoords(),out var pcell))
                {
                    var techno = TechnoTypeClass.ABSTRACTTYPE_ARRAY.Find(dataINI.Data.FreeUnit).Ref.Base.CreateObject(Owner.OwnerObject.Ref.Owner).Convert<TechnoClass>();
                    var cell = CellClass.Coord2Cell(Owner.OwnerObject.Ref.Base.Base.GetCoords());
                    if(TechnoPlacer.PlaceTechnoNear(techno, cell))
                    {
                        var mission = techno.Convert<MissionClass>();
                        mission.Ref.ForceMission(Mission.Enter);
                        techno.Ref.SetDestination(Owner.OwnerObject.Convert<AbstractClass>());

                        if (dataINI.Data.FreeUnitLevel > 1)
                        {
                            if(dataINI.Data.FreeUnitLevel == 2)
                            {
                                techno.Ref.Veterancy.SetVeteran(true);
                            }
                            else
                            {
                                techno.Ref.Veterancy.SetElite(true);
                            }
                        }

                    }

                }


            }
        }

    }

    public class AutoEnterTankBunkerData : INIAutoConfig
    {
        [INIField(Key = "TankBunker.FreeUnit")]
        public string FreeUnit;

        [INIField(Key = "TankBunker.FreeUnitLevel")]
        public int FreeUnitLevel = 1;
    }
}
