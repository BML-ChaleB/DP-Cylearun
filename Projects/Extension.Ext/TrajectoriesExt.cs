using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DynamicPatcher;
using PatcherYRpp;

namespace Extension.Ext
{
    public partial class BulletExt
    {
        public ITrajectories Trajectory;
    }

    public interface ITrajectories
    {


        //public override void OnUnlimbo(Pointer<CoordStruct> pCoords, Pointer<BulletVelocity> pVelocity){}

        //public override void OnAIPreDetonate(){}

        //public override void OnAIVelocity(Pointer<BulletVelocity> pSpeed, Pointer<BulletVelocity> pVelocity){}

        public bool OnAI();
      
        public TrajectoryCheckReturnType OnAITargetCoordCheck();
      
        public TrajectoryCheckReturnType OnAITechnoCheck(Pointer<TechnoClass> pTechno);
      
        public void SetAttribute(int Import);

    }


    public enum  TrajectoryCheckReturnType
    {
        ExecuteGameCheck = 0,
	      SkipGameCheck = 1,
	      SatisfyGameCheck = 2,
	      Detonate = 3
    };
}
