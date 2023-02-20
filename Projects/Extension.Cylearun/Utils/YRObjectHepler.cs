using Extension.Utilities;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Cylearun.Utils
{
    public static class YRObjectHepler
    {
        public static CoordStruct ToCoordStruct(this SingleVector3D vector3D)
        {
            return new CoordStruct((int)vector3D.X, (int)vector3D.Y, (int)vector3D.Z);
        }

        public static SingleVector3D ToSingleVector3D(this CoordStruct coord)
        {
            return new SingleVector3D(coord.X, coord.Y, coord.Z);
        }

        public static DirStruct ToDirStruct(this FacingStruct facing)
        {
            var dir = GameUtil.Facing2Dir(facing);
            var dirs = new DirStruct(16, (short)DirStruct.TranslateFixedPoint(3, 16, (uint)dir));
            return dirs;
        }

        public static DirStruct ToDirStruct(this Direction dir)
        {
            var dirs = new DirStruct(16, (short)DirStruct.TranslateFixedPoint(3, 16, (uint)dir));
            return dirs;
        }
    }
}
