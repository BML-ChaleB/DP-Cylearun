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
        public int BackSpeed = 40;

        [INIField(Key = "LandingBuffer")]
        public int LandingBuffer = 15;

        [INIField(Key = "LandingRange")]
        public int LandingRange = 500;

        //[INIField(Key = "LandingOffset")]
        //public CoordStruct LandingOffset = new CoordStruct(0, 0, 0);
        //
        //[INIField(Key = "OnTurret")]
        //public bool OnTurret = false;
    }

    [Serializable]
    [ScriptAlias(nameof(SpawnScript))]
    public class SpawnScript : TechnoScriptable<SpawnScriptData>
    {
        public SpawnScript(TechnoExt owner) : base(owner)
        {
        }




        int _backSpeed => Data.BackSpeed;
        int _landingRange => Data.LandingRange;
        //CoordStruct _landingOffset => Data.LandingOffset;
        //bool _onTurret => Data.OnTurret;
        double _bufferMax => Data.LandingBuffer;

        public bool InLanding = false;

        public static bool IsDeadOrInvisible(Pointer<TechnoClass> pTechno)
        {
            return pTechno.IsNull || pTechno.Ref.Base.InLimbo || pTechno.Ref.Base.Health <= 0 || !pTechno.Ref.Base.IsAlive || pTechno.Ref.IsCrashing || pTechno.Ref.IsSinking;
        }

        public int SpawnIndex = -1;
        double buffer = 0;

        public void FastMove(CoordStruct Offset)
        {
            Owner.BaseObject.Mark(MarkType.UP);
            Owner.Pos = Owner.Pos + Offset;
            Owner.BaseObject.Mark(MarkType.DOWN);
        }
        public override void OnPut(CoordStruct coord, Direction faceDir)
        {
            buffer = _bufferMax;
        }


        public override void OnUpdate()
        {
            InLanding = false;
            Pointer<TechnoClass> pSpawnOwner;

            if (!IsDeadOrInvisible(pSpawnOwner = Owner.OwnerRef.SpawnOwner) 
                && (Owner.Locomotor.ToLocomotionClass<FlyLocomotionClass>()).IsNotNull 
                && Owner.FootRef.Destination.IsNotNull 
                && SpawnIndex >= 0 
                && pSpawnOwner.Ref.SpawnManager.Ref.SpawnedNodes[SpawnIndex].Ref.Status == 4)
            {
                var Target = /*default == _landingOffset ? */Owner.FootRef.Destination.Ref.GetCoords() /*: pSpawnOwner.Ref.GetFLHAbsoluteCoords(_landingOffset, _onTurret)*/;
                var Offset = Target - Owner.Pos;
                if (Game.CurrentFrame % 50 == 0)
                {
                    var Dir = MathEx.Point2Dir(-Offset);
                    Owner.OwnerRef.Facing.turn(Dir);
                    Owner.OwnerRef.TurretFacing.turn(Dir);
                }


                int BackSpeed = (int)(_backSpeed * Owner.FootRef.SpeedMultiplier * RulesClass.Instance.Ref.VeteranSpeed * Owner.HouseRef.Type.Ref.SpeedAircraftMult);

                Vector3 vector = Offset.ToVector3();

                var _offset = Offset;
                _offset.Z = 0;
                double Distance = _offset.Magnitude();
                if (Distance <= _landingRange)
                {

                    InLanding = true;

                    if (vector.Length() < BackSpeed)
                    {
                        FouceRetrun(pSpawnOwner.Ref.SpawnManager, pSpawnOwner.Ref.SpawnManager.Ref.SpawnedNodes[SpawnIndex]);
                        buffer = _bufferMax;
                        return;

                    }
                    else
                    {
                        vector.Z = MathEx.Lerp(Owner.Pos.Z, Target.Z, (_bufferMax - buffer) / _bufferMax) - Owner.Pos.Z;
                        FastMove((vector.Normalize() * BackSpeed).ToCoordStruct());

                        if (buffer > 0) buffer--;
                    }

                    if (Game.CurrentFrame % 30 == 0)
                    {

                        Owner.OwnerRef.Facing.turn(pSpawnOwner.Ref.Facing.current());
                        Owner.OwnerRef.TurretFacing.turn(pSpawnOwner.Ref.Facing.current());
                    }
                }
            }
        }



        public void FouceRetrun(Pointer<SpawnManagerClass> pManager, Pointer<SpawnNode> pNode)
        {
            pNode.Ref.Unit.Ref.Base.Remove();
            pNode.Ref.Status = 6;
            Pointer<TimerStruct> SpawnTimer = new IntPtr((uint)pNode + 8);
            SpawnTimer.Ref.Start(pManager.Ref.ReloadRate);
        }

    }
}