using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Numerics;
using PatcherYRpp;
using Extension.Ext;
using Extension.Script;
using PatcherYRpp.Utilities;
using Extension.Components;
using Extension.INI;

namespace Scripts
{
    [Serializable]
    public class SpawnScriptData : INIAutoConfig
    {
        [INIField(Key = "BackSpeed")]
        public int BackSpeed = 25;

        [INIField(Key = "ClimbSpeed")]
        public int ClimbSpeed = 15;

        [INIField(Key = "LandingRange")]
        public int LandingRange = 700;
    }

    [Serializable]
    [ScriptAlias(nameof(SpawnScript))]
    public class SpawnScript : TechnoScriptable
    {
        public SpawnScript(TechnoExt owner) : base(owner)
        {
            INIComponentWith<SpawnScriptData> ini = this.CreateRulesIniComponentWith<SpawnScriptData>(Owner.OwnerObject.Ref.Type.Ref.Base.Base.ID);
            Data = ini.Data;
        }



        SpawnScriptData Data;

        int _backSpeed => Data.BackSpeed;
        int _climbSpeed => Data.ClimbSpeed;
        int _landingRange => Data.LandingRange;

        public static bool IsDeadOrInvisible(Pointer<TechnoClass> pTechno)
        {
            return pTechno.IsNull || pTechno.Ref.Base.InLimbo || pTechno.Ref.Base.Health <= 0 || !pTechno.Ref.Base.IsAlive || pTechno.Ref.IsCrashing || pTechno.Ref.IsSinking;
        }

        public bool InLanding = false;
        public int SpawnIndex = -1;

        public void FastMove(CoordStruct Offset)
        {
            Owner.OwnerObject.Ref.Base.Mark(MarkType.UP);
            Owner.OwnerObject.Ref.Base.SetLocation(lastCoord + Offset);
            Owner.OwnerObject.Ref.Base.Mark(MarkType.DOWN);
        }

        CoordStruct lastCoord;

        byte buffer = 150;
        public override void OnUpdate()
        {
            InLanding = false;
            Pointer<TechnoClass> pTechno = Owner.OwnerObject;
            Pointer<TechnoClass> pSpawnOwner;
            Pointer<FlyLocomotionClass> pFly;

            if (!IsDeadOrInvisible(pSpawnOwner = Owner.OwnerRef.SpawnOwner) && (pFly = Owner.OwnerObject.Convert<FootClass>().Ref.Locomotor.ToLocomotionClass<FlyLocomotionClass>()).IsNotNull)
            {
                if (Owner.OwnerObject.Convert<FootClass>().Ref.Destination.IsNotNull && -1 != SpawnIndex && pSpawnOwner.Ref.SpawnManager.Ref.SpawnedNodes[SpawnIndex].Ref.Status == 4) 
                {
                    if (buffer > 0)
                    {
                        buffer--;
                    }
                    else
                    {
                        int BackSpeed = (int)(_backSpeed * Owner.OwnerObject.Convert<FootClass>().Ref.SpeedMultiplier * RulesClass.Instance.Ref.VeteranSpeed * Owner.OwnerObject.Ref.Owner.Ref.Type.Ref.SpeedAircraftMult);
                        int ClimbSpeed = (int)(_climbSpeed * Owner.OwnerObject.Convert<FootClass>().Ref.SpeedMultiplier * RulesClass.Instance.Ref.VeteranSpeed * Owner.OwnerObject.Ref.Owner.Ref.Type.Ref.SpeedAircraftMult);

                        InLanding = true;

                        var Offset = Owner.OwnerObject.Convert<FootClass>().Ref.Destination.Ref.GetCoords() - Owner.OwnerObject.Ref.Base.Base.GetCoords();
                        Vector3 vector = Offset.ToVector3();
                        vector.Z = 0;
                        if ((Offset.X > _landingRange) || (Offset.X < -_landingRange) || (Offset.Y > _landingRange) || (Offset.Y < -_landingRange))
                        {
                            var difference = pFly.Ref.FlightLevel - pTechno.Ref.Base.GetHeight();
                            var coord = (vector.Normalize() * BackSpeed).ToCoordStruct();
                            coord.Z = Math.Abs(difference) < ClimbSpeed ? difference : (difference < 0 ? -ClimbSpeed : ClimbSpeed);
                            FastMove(coord);
                            if (Game.CurrentFrame % 50 == 0)
                            {
                                var Dir = MathEx.Point2Dir(-Offset);
                                Owner.OwnerRef.Facing.turn(Dir);
                                Owner.OwnerRef.TurretFacing.turn(Dir);
                            }
                        }
                        else
                        {

                            if (vector != Vector3.Zero)
                            {
                                pTechno.Ref.Base.Mark(MarkType.UP);

                                int h = pTechno.Ref.Base.GetHeight();

                                float len = vector.Length();

                                pTechno.Ref.Base.SetLocation(lastCoord + (len > BackSpeed ? (vector * (1 / len) * BackSpeed) : vector).ToCoordStruct() - new CoordStruct(0, 0, h >= ClimbSpeed ? ClimbSpeed : h));

                                pTechno.Ref.Base.Mark(MarkType.DOWN);
                            }
                            else
                            {
                                int h = pTechno.Ref.Base.GetHeight();
                                pTechno.Ref.Base.Mark(MarkType.UP);
                                pTechno.Ref.Base.SetLocation(lastCoord - new CoordStruct(0, 0, h >= ClimbSpeed ? ClimbSpeed : h));
                                pTechno.Ref.Base.Mark(MarkType.DOWN);
                            }

                            Owner.OwnerRef.Facing.turn(pSpawnOwner.Ref.Facing.current());
                            Owner.OwnerRef.TurretFacing.turn(pSpawnOwner.Ref.Facing.current());
                        }
                    }
                }
                else if (buffer != 150)
                {
                    buffer = 150;
                }
            }

            lastCoord = Owner.OwnerObject.Ref.Base.Base.GetCoords();

        }
    }
}