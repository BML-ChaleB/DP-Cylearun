using Extension.CWUtilities;
using Extension.Cylearun.Utils;
using Extension.Ext;
using Extension.INI;
using Extension.Utilities;
using PatcherYRpp;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Extension.CY
{




    public partial class TechnoGlobalExtension
    {
        public bool HandleAirCommand = false;

        private bool isAreaProtecting = false;

        private CoordStruct areaProtectTo;

        private static List<CoordStruct> areaGuardCoords = new List<CoordStruct>()
        {
            new CoordStruct(-300,-300,0),
            new CoordStruct(-300,0,0),
            new CoordStruct(0,0,0),
            new CoordStruct(300,0,0),
            new CoordStruct(300,300,0),
            new CoordStruct(0,300,0),
        };

        private int currentAreaProtectedIndex = 0;

        private bool isAreaGuardReloading = false;

        private int areaGuardTargetCheckRof = 20;

        [AwakeAction]
        public void TechnoClass_Awake_Fighter_Area_Guard()
        {
            if (!(Data.FighterAreaGuard))
                return;

            var radius = Data.FighterGuardRadius * 256;

            areaGuardCoords = new List<CoordStruct>()
            {
                new CoordStruct(0,radius,0),
                new CoordStruct((int)(0.85*radius),(int)(0.85*radius),0),
                new CoordStruct(radius,0,0),
                new CoordStruct((int)(0.85*radius),(int)(-0.85*radius),0),
                new CoordStruct(0,-radius,0),
                new CoordStruct((int)(-0.85*radius),(int)(-0.85*radius),0),
                new CoordStruct(-radius,0,0),
                new CoordStruct((int)(-0.85*radius),(int)(0.85*radius),0),
            };
        }


        [UpdateAction]
        public void TechnoClass_Update_Fighter_Area_Guard()
        {
            if (!Owner.OwnerObject.Ref.Owner.Ref.ControlledByHuman())
                return;

            if (!Data.FighterAreaGuard)
            {
                return;
            }

            var mission = Owner.OwnerObject.Convert<MissionClass>();

            if (mission.Ref.CurrentMission == Mission.Move)
            {
                isAreaProtecting = false;
                isAreaGuardReloading = false;
                return;
            }

            if (Owner.OwnerObject.CastToFoot(out Pointer<FootClass> pfoot))
            {
                if (!isAreaProtecting)
                {
                    if (mission.Ref.CurrentMission == Mission.Area_Guard)
                    {
                        isAreaProtecting = true;

                        CoordStruct dest = pfoot.Ref.Locomotor.Destination();
                        areaProtectTo = dest;
                    }
                }


                if (isAreaProtecting)
                {
                    //没弹药的情况下返回机场
                    if(Owner.OwnerObject.Ref.Ammo==0 && !isAreaGuardReloading)
                    {
                        Owner.OwnerObject.Ref.SetTarget(default);
                        Owner.OwnerObject.Ref.SetDestination(default, false);
                        mission.Ref.ForceMission(Mission.Stop);
                        isAreaGuardReloading = true;
                        return;
                    }

                    //填弹完毕后继续巡航
                    if (isAreaGuardReloading)
                    {
                        if(Owner.OwnerObject.Ref.Ammo >= Data.FighterMaxAmmo)
                        {
                            isAreaGuardReloading = false;
                            mission.Ref.ForceMission(Mission.Area_Guard);
                        }
                        else
                        {
                            if (mission.Ref.CurrentMission != Mission.Sleep && mission.Ref.CurrentMission != Mission.Enter)
                            {
                                if (mission.Ref.CurrentMission == Mission.Guard)
                                {
                                    mission.Ref.ForceMission(Mission.Sleep);
                                }
                                else
                                {
                                    mission.Ref.ForceMission(Mission.Enter);
                                }
                                return;
                            }
                        }
                    }
                   
                    if(mission.Ref.CurrentMission != Mission.Attack)
                    {
                        HandleAirCommand = false;
                    }

                    if (mission.Ref.CurrentMission == Mission.Move)
                    {
                        isAreaProtecting = false;
                        return;
                    }
                    else if (mission.Ref.CurrentMission == Mission.Attack)
                    {
                        bool skip = true;
                        if (isAreaProtecting && Data.FighterChaseRange != -1 && areaProtectTo != null &&!HandleAirCommand)
                        {
                            var sourceDest = Data.FindRangeAroundSelf ? Owner.OwnerObject.Ref.Base.Base.GetCoords() : areaProtectTo;
                            if(!Owner.OwnerObject.Ref.Target.IsNull)
                            {
                                //超出追击距离停止追击
                                var distance = sourceDest.DistanceFrom(Owner.OwnerObject.Ref.Target.Ref.GetCoords());
                                if(distance == double.NaN || distance > Data.FighterChaseRange * 256)
                                {
                                    Owner.OwnerObject.Ref.SetTarget(default);
                                    mission.Ref.ForceMission(Mission.Stop);
                                }
                            }
                        }

                        if(skip)
                        {
                            return;
                        }
                    }
                    else if (mission.Ref.CurrentMission == Mission.Enter)
                    {
                        if(isAreaGuardReloading)
                        {
                            return;
                        }
                        else
                        {
                            mission.Ref.ForceMission(Mission.Stop);
                        }
                    }else if(mission.Ref.CurrentMission == Mission.Sleep)
                    {
                        if (isAreaGuardReloading)
                        {
                            return;
                        }
                    }


                    if (areaProtectTo != null)
                    {
                        var dest = areaProtectTo;

                        var house = Owner.OwnerObject.Ref.Owner;

                        if(Data.FighterAutoFire)
                        {
                            //if (areaProtectTo.DistanceFrom(Owner.OwnerObject.Ref.Base.Base.GetCoords()) <= 2000)
                            var targetDest = Data.FindRangeAroundSelf ? Owner.OwnerObject.Ref.Base.Base.GetCoords() : dest;

                            {
                                if (areaGuardTargetCheckRof-- <= 0)
                                {
                                    areaGuardTargetCheckRof = 20;

                                    var target = Finder.FineOneTechno(house, x =>
                                    {
                                        var coords = x.Ref.Base.Base.GetCoords();
                                        var height = x.Ref.Base.GetHeight();
                                        var type = x.Ref.Base.Base.WhatAmI();

                                        if (x.Ref.Base.InLimbo)
                                        {
                                            return false;
                                        }

                                        if(!GameUtil.CanAffectTarget(Owner.OwnerObject, x))
                                        {
                                            return false;
                                        }


                                        //if (x.Ref.Base.IsDisguised())
                                        //{
                                        //    var fakeHouse = x.Ref.Base.GetDisguiseHouse(true);
                                        //    if (!fakeHouse.IsNull)
                                        //    {
                                        //        if (fakeHouse.Ref.IsAlliedWith(house))
                                        //        {
                                        //            return false;
                                        //        }
                                        //    }
                                        //}

                                        var bounsRange = 0;
                                        if (x.Ref.Base.GetHeight() > 10)
                                        {
                                            bounsRange = Data.FighterGuardRange;
                                        }

                                        if ((coords - new CoordStruct(0, 0, height)).DistanceFrom(targetDest) <= (Data.FighterGuardRange * 256 + bounsRange) && type != AbstractType.Building)
                                        {
                                            return true;
                                        }
                                        return false;
                                    }, FindRange.Enermy);

                                    if (!target.IsNullOrExpired())
                                    {
                                        Owner.OwnerObject.Ref.SetTarget(target.OwnerObject.Convert<AbstractClass>());
                                        mission.Ref.ForceMission(Mission.Stop);
                                        mission.Ref.ForceMission(Mission.Attack);
                                        return;
                                    }
                                }

                            }
                        }
                       


                        //if (areaProtectTo.DistanceFrom(Owner.OwnerObject.Ref.Base.Base.GetCoords()) <= 2000)
                        //{
                            if (currentAreaProtectedIndex > areaGuardCoords.Count() - 1)
                            {
                                currentAreaProtectedIndex = 0;
                            }
                            dest += areaGuardCoords[currentAreaProtectedIndex];
                            if(FighterIsCloseEngouth(dest))
                            {
                                currentAreaProtectedIndex++;
                            }
                        //}

                        pfoot.Ref.Locomotor.Move_To(dest);
                        var cell = CellClass.Coord2Cell(dest);
                        if (MapClass.Instance.TryGetCellAt(cell, out Pointer<CellClass> pcell))
                        {
                            Owner.OwnerObject.Ref.SetDestination(pcell.Convert<AbstractClass>(), false);
                        }
                    }
                }
            }

        }

        private bool FighterIsCloseEngouth(CoordStruct coordstruct)
        {
            var ownerLocation = Owner.OwnerObject.Ref.Base.Base.GetCoords();
            var sameHeightCoord = new CoordStruct(coordstruct.X, coordstruct.Y, ownerLocation.Z);
            var disctance = ownerLocation.DistanceFrom(sameHeightCoord);
            return disctance == double.NaN ? false : disctance < 2000;
        }

    }

    public partial class TechnoGlobalTypeExt
    {

        [INIField(Key = "Fighter.AreaGuard")]
        public bool FighterAreaGuard = false;
        [INIField(Key = "Fighter.GuardRange")]
        public int FighterGuardRange = 5;
        [INIField(Key = "Fighter.AutoFire")]
        public bool FighterAutoFire = false;
        [INIField(Key = "Ammo")]
        public int FighterMaxAmmo = 0;
        [INIField(Key = "Fighter.GuardRadius")]
        public int FighterGuardRadius = 5;
        [INIField(Key = "Fighter.FindRangeAroundSelf")]
        public bool FindRangeAroundSelf = false;
        [INIField(Key = "Fighter.ChaseRange")]
        public int FighterChaseRange = 30;
    }

}
