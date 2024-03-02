using DynamicPatcher;
using Extension.Ext;
using Extension.Script;
using PatcherYRpp;
using System;

namespace ComponentHooks
{
    //移植自PhobosTrajectory
    //https://github.com/Phobos-developers/Phobos/pull/432
    public class TrajectoriesComponentHooks
    {

        [Hook(HookType.AresHook, Address = 0x468B72, Size = 5)]
        public static unsafe UInt32 BulletClass_Unlimbo_Trajectories(REGISTERS* R)
        {
            try
            {
                Pointer<BulletClass> pBullet = (IntPtr)(void*)R->EBX;
                Pointer<CoordStruct> pCoord = R->Stack<Pointer<CoordStruct>>(0x54 + 0x4);
                Pointer<BulletVelocity> pVelocity = R->Stack<Pointer<BulletVelocity>>(0x54 + 0x8);

                BulletExt ext = BulletExt.ExtMap.Find(pBullet);
                ext.GameObject.Foreach(c => (c as IBulletScriptable)?.OnUnlimbo(pCoord, pVelocity));
                return 0;

            }
            catch (Exception e)
            {
                Logger.PrintException(e);
                return 0;
            }
        }

        [Hook(HookType.AresHook, Address = 0x4666F7, Size = 6)]
        public static unsafe UInt32 BulletClass_AI_Trajectories(REGISTERS* R)
        {
            try
            {
                UInt32 Detonate = 0x467E53;
                Pointer<BulletClass> pBullet = (IntPtr)(void*)R->EBP;
                var ext = BulletExt.ExtMap.Find(pBullet);
                bool detonate = false;
                ITrajectories ITraj = ext.Trajectory;

                if (null != ITraj)
                    detonate = ITraj.OnAI();

                if (detonate && !pBullet.Ref.SpawnNextAnim)
                    return Detonate;

                return 0;
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
                return 0;
            }
        }

        [Hook(HookType.AresHook, Address = 0x467E53, Size = 6)]
        public static unsafe UInt32 BulletClass_AI_PreDetonation_Trajectories(REGISTERS* R)

        {
            try
            {
                Pointer<BulletClass> pBullet = (IntPtr)(void*)R->EBP;

                var ext = BulletExt.ExtMap.Find(pBullet);

                ext.GameObject.Foreach(c => (c as IBulletScriptable)?.OnAIPreDetonate());

                return 0;
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
                return 0;
            }
        }

        [Hook(HookType.AresHook, Address = 0x46745C, Size = 7)]
        public static unsafe UInt32 BulletClass_AI_Position_Trajectories(REGISTERS* R)
        {
            try
            {
                Pointer<BulletClass> pBullet = (IntPtr)(void*)R->EBP;
                Pointer<BulletVelocity> pSpeed = R->lea_Stack<Pointer<BulletVelocity>>(0x1AC - 0x11C);
                Pointer<BulletVelocity> pPosition = R->lea_Stack<Pointer<BulletVelocity>>(0x1AC - 0x144);


                var ext = BulletExt.ExtMap.Find(pBullet);

                ext.GameObject.Foreach(c => (c as IBulletScriptable)?.OnAIVelocity(pSpeed, pPosition));

                return 0;
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
                return 0;
            }
        }

        [Hook(HookType.AresHook, Address = 0x4677D3, Size = 5)]
        public static unsafe UInt32 BulletClass_AI_TargetCoordCheck_Trajectories(REGISTERS* R)

        {
            try
            {
                UInt32 SkipCheck = 0x4678F8, ContinueAfterCheck = 0x467879, Detonate = 0x467E53;

                Pointer<BulletClass> pBullet = (IntPtr)(void*)R->EBP;

                var ext = BulletExt.ExtMap.Find(pBullet);
                ITrajectories ITraj = ext.Trajectory;

                if (null != ITraj)
                {
                    switch (ITraj.OnAITargetCoordCheck())
                    {
                        case TrajectoryCheckReturnType.SkipGameCheck:
                            return SkipCheck;
                        case TrajectoryCheckReturnType.SatisfyGameCheck:
                            return ContinueAfterCheck;
                        case TrajectoryCheckReturnType.Detonate:
                            return Detonate;
                        default:
                            break;
                    }
                }
                return 0;
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
                return 0;
            }
        }

        [Hook(HookType.AresHook, Address = 0x467927, Size = 5)]
        public static unsafe UInt32 BulletClass_AI_TechnoCheck_Trajectories(REGISTERS* R)

        {
            try
            {
                UInt32 SkipCheck = 0x467A26, ContinueAfterCheck = 0x467514;

                Pointer<BulletClass> pBullet = (IntPtr)(void*)R->EBP;
                Pointer<TechnoClass> pTechno = (IntPtr)(void*)R->ESI;

                var ext = BulletExt.ExtMap.Find(pBullet);

                ITrajectories ITraj = ext.Trajectory;

                if (null != ITraj)
                {
                    switch (ITraj.OnAITechnoCheck(pTechno))
                    {
                        case TrajectoryCheckReturnType.SkipGameCheck:
                            return SkipCheck;
                        case TrajectoryCheckReturnType.SatisfyGameCheck:
                            return ContinueAfterCheck;
                        default:
                            break;
                    }
                }
                return 0;
            }
            catch (Exception e)
            {
                Logger.PrintException(e);
                return 0;
            }
        }
    }
}
