//using Extension.Ext;
//using PatcherYRpp;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

using PatcherYRpp;
using System;

namespace Extension.Cylearun.Utils
{
    /// <summary>
    /// From Kratos   https://github.com/ChrisLv-CN/YRDynamicPatcher-Kratos/blob/main/DynamicPatcher/Projects/Extension/Kraotos/Utilities/FLHHelper.cs
    /// </summary>
    public static class GameUtil
    {

        public const double BINARY_ANGLE_MAGIC = -(360.0 / (65535 - 1)) * (Math.PI / 180);


        public static Direction Facing2Dir(FacingStruct facing)
        {
            var dirValue = facing.target().value();
            Direction dir = default;
            if (dirValue >= 0)
            {
                if (dirValue <= 4096)
                    dir = Direction.N;
                else if (dirValue <= 12288)
                    dir = Direction.NE;
                else if (dirValue <= 20480)
                    dir = Direction.E;
                else if (dirValue <= 28672)
                    dir = Direction.SE;
                else
                    dir = Direction.S;
            }
            else
            {
                if (dirValue > -4096)
                    dir = Direction.N;
                else if (dirValue > -12288)
                    dir = Direction.NW;
                else if (dirValue > -20480)
                    dir = Direction.W;
                else if (dirValue > -28672)
                    dir = Direction.SW;
                else
                    dir = Direction.S;
            }
            return dir;
        }

        public static FacingStruct Dir2Facing(Direction dir)
        {
            var intdir = (int)dir;
            if (intdir <= 4)
            {
                return new FacingStruct((short)(intdir * (32767 / 4)));
            }
            else
            {
                return new FacingStruct((short)(4 - intdir * (32767 / 4)));
            }
        }

        public static DirStruct DirNormalized(int index, int facing)
        {
            double radians = 180/Math.PI * ((-360 / facing * index)) ;
            DirStruct dir = new DirStruct();
            dir.SetValue((short)(radians / BINARY_ANGLE_MAGIC));
            return dir;
        }

        public static int Dir2FacingIndex(DirStruct dir, int facing)
        {
            uint bits = (uint)Math.Round(Math.Sqrt(facing), MidpointRounding.AwayFromZero);
            double face = dir.GetValue(bits);
            double x = (face / (1 << (int)bits)) * facing;
            int index = (int)Math.Round(x, MidpointRounding.AwayFromZero);
            return index;
        }

        public static short Dir2FacingShort(Direction dir)
        {
            var intdir = (int)dir;
            if (intdir <= 4)
            {
                return (short)(intdir * (32767 / 4));
            }
            else
            {
                return (short)(4 - intdir * (32767 / 4));
            }
        }

        public static Direction Point2Dir(CoordStruct location, CoordStruct targetPos)
        {
            Point2D p1 = new Point2D(location.X, location.Y);
            Point2D p2 = new Point2D(targetPos.X, targetPos.Y);
            return Point2Dir(p1, p2);
        }

        public static Direction Point2Dir(Point2D p1, Point2D p2)
        {
            Direction dir = Direction.N;
            var angle = Math.Atan2((p2.X - p1.X), (p2.Y - p1.Y));
            var theta = angle * (180 / Math.PI);
            if (theta >= 0)
            {
                if (theta <= 22.5)
                    dir = Direction.S;
                else if (theta <= 67.5)
                    dir = Direction.SE;
                else if (theta <= 112.5)
                    dir = Direction.E;
                else if (theta <= 157.5)
                    dir = Direction.NE;
                else
                    dir = Direction.N;
            }
            else
            {
                if (theta > -22.5)
                    dir = Direction.S;
                else if (theta > -67.5)
                    dir = Direction.SW;
                else if (theta > -112.5)
                    dir = Direction.W;
                else if (theta > -157.5)
                    dir = Direction.NW;
                else
                    dir = Direction.N;
            }
            return dir;
        }


        public static CoordStruct Direction2Vector(Direction dir)
        {
            CoordStruct vector = default;
            switch (dir)
            {
                case Direction.N:
                    vector = new CoordStruct(0, -1, 0);
                    break;
                case Direction.NE:
                    vector = new CoordStruct(1, -1, 0);
                    break;
                case Direction.E:
                    vector = new CoordStruct(1, 0, 0);
                    break;
                case Direction.SE:
                    vector = new CoordStruct(1, 1, 0);
                    break;
                case Direction.S:
                    vector = new CoordStruct(0, 1, 0);
                    break;
                case Direction.SW:
                    vector = new CoordStruct(-1, 1, 0);
                    break;
                case Direction.W:
                    vector = new CoordStruct(-1, 0, 0);
                    break;
                case Direction.NW:
                    vector = new CoordStruct(-1, -1, 0);
                    break;
            }
            return vector;
        }

        public static bool CanAffectTarget(Pointer<TechnoClass> techno, Pointer<TechnoClass> target)
        {
            var widx = techno.Ref.SelectWeapon(target.Convert<AbstractClass>());
            var weapon = techno.Ref.GetWeapon(widx);
            if (weapon.IsNull)
            {
                return false;
            }

            var warhead = weapon.Ref.WeaponType.Ref.Warhead;
            return MapClass.GetTotalDamage(10000, warhead, target.Ref.Type.Ref.Base.Armor, 0) > 0;
        }

        public static DirStruct DirAdjustAngle(this DirStruct dir, int angle)
        {
            if (angle == 0)
            {
                return dir;
            }
            var currentVal = (int)dir.Value;
            var adjust = (short.MaxValue - short.MinValue) / 360 * angle;

            var targetVal = currentVal + adjust;
            if (targetVal > short.MaxValue)
            {
                targetVal = short.MinValue + (targetVal - short.MaxValue);
            }
            else if (targetVal < short.MinValue)
            {
                targetVal = short.MaxValue - (short.MaxValue + (targetVal - short.MinValue));
            }

            return new DirStruct(targetVal);
        }


        public static double BigDistanceForm(this CoordStruct source, CoordStruct target)
        {
            var square = (source.X / 100d - target.X / 100d) * (source.X / 100d - target.X / 100d) + (source.Y / 100d - target.Y / 100d) * (source.Y / 100d - target.Y / 100d) + (source.Z / 100d - target.Z / 100d) * (source.Z / 100d - target.Z / 100d);
            return Math.Sqrt(square) * 100;

        }

        //public static CoordStruct GetFLH(this TechnoExt technoExt, CoordStruct flh, DirStruct dir, bool flip = false)
        //{
        //    return GetFLH(technoExt.OwnerObject.Ref.Base.Base.GetCoords(), flh, dir, flip);
        //}

        //public static CoordStruct GetFLH(CoordStruct source, CoordStruct flh, DirStruct dir, bool flip = false)
        //{
        //    CoordStruct res = source;
        //    if (null != flh && default != flh && null != dir)
        //    {
        //        double radians = dir.radians();

        //        double rF = flh.X;
        //        double xF = rF * Math.Cos(-radians);
        //        double yF = rF * Math.Sin(-radians);
        //        CoordStruct offsetF = new CoordStruct((int)xF, (int)yF, 0);

        //        double rL = flip ? flh.Y : -flh.Y;
        //        double xL = rL * Math.Sin(radians);
        //        double yL = rL * Math.Cos(radians);
        //        CoordStruct offsetL = new CoordStruct((int)xL, (int)yL, 0);

        //        res = source + offsetF + offsetL + new CoordStruct(0, 0, flh.Z);
        //    }
        //    return res;
        //}


        //public static CoordStruct ToCoordStruct(this SingleVector3D singleVector3D)
        //{
        //    return new CoordStruct((int)singleVector3D.X, (int)singleVector3D.Y, (int)singleVector3D.Z);
        //}

        //public static unsafe CoordStruct GetFLHAbsoluteCoords(CoordStruct source, CoordStruct flh, DirStruct dir, CoordStruct turretOffset = default)
        //{
        //    CoordStruct res = source;
        //    if (null != flh && default != flh)
        //    {
        //        SingleVector3D offset = GetFLHAbsoluteOffset(flh, dir, turretOffset);
        //        res += offset.ToCoordStruct();
        //    }
        //    return res;
        //}

        //public static unsafe SingleVector3D GetFLHAbsoluteOffset(CoordStruct flh, DirStruct dir, CoordStruct turretOffset)
        //{
        //    SingleVector3D offset = default;
        //    if (null != flh && default != flh)
        //    {
        //        Matrix3DStruct matrix3D = new Matrix3DStruct(true);
        //        matrix3D.Translate(turretOffset.X, turretOffset.Y, turretOffset.Z);
        //        matrix3D.RotateZ((float)dir.radians());
        //        offset = GetFLHOffset(ref matrix3D, flh);
        //    }
        //    return offset;
        //}



        //public static unsafe CoordStruct GetFLHAbsoluteCoords(Pointer<TechnoClass> pTechno, CoordStruct flh, bool isOnTurret = true, int flipY = 1)
        //{
        //    CoordStruct turretOffset = default;
        //    if (isOnTurret)
        //    {
        //        TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
        //        if (null != ext)
        //        {
        //            turretOffset = ext.Type.TurretOffset;
        //        }
        //        else
        //        {
        //            turretOffset.X = pTechno.Ref.Type.Ref.TurretOffset;
        //        }
        //    }
        //    return GetFLHAbsoluteCoords(pTechno, flh, isOnTurret, flipY, turretOffset);
        //}

        //public static unsafe CoordStruct GetFLHAbsoluteCoords(Pointer<TechnoClass> pTechno, CoordStruct flh, bool isOnTurret, int flipY, CoordStruct turretOffset)
        //{
        //    if (pTechno.Ref.Base.Base.WhatAmI() == AbstractType.Building)
        //    {
        //        // 建筑不能使用矩阵方法测算FLH
        //        return GetFLHAbsoluteCoords(pTechno.Ref.Base.Base.GetCoords(), flh, pTechno.Ref.Facing.current(), turretOffset);
        //    }
        //    else
        //    {
        //        SingleVector3D res = pTechno.Ref.Base.Base.GetCoords().ToSingleVector3D();

        //        // get nextframe location offset
        //        // Pointer<FootClass> pFoot = pTechno.Convert<FootClass>();
        //        // int speed = 0;
        //        // if (pFoot.Ref.Locomotor.Is_Moving() && (speed = pFoot.Ref.GetCurrentSpeed()) > 0)
        //        // {
        //        //     turretOffset += new CoordStruct(speed, 0, 0);
        //        // }

        //        if (null != flh && default != flh)
        //        {
        //            // Step 1: get body transform matrix
        //            Matrix3DStruct matrix3D = GetMatrix3D(pTechno);
        //            // Step 2: move to turrretOffset
        //            matrix3D.Translate(turretOffset.X, turretOffset.Y, turretOffset.Z);
        //            // Step 3: rotation
        //            RotateMatrix3D(ref matrix3D, pTechno, isOnTurret);
        //            // Step 4: apply FLH offset
        //            CoordStruct tempFLH = flh;
        //            if (pTechno.Convert<AbstractClass>().Ref.WhatAmI() == AbstractType.Building)
        //            {
        //                tempFLH.Z += Game.LevelHeight;
        //            }
        //            tempFLH.Y *= flipY;
        //            SingleVector3D offset = GetFLHOffset(ref matrix3D, tempFLH);
        //            // Step 5: offset techno location
        //            res += offset;
        //        }
        //        return res.ToCoordStruct();
        //    }
        //}




        //private static unsafe SingleVector3D GetFLHOffset(ref Matrix3DStruct matrix3D, CoordStruct flh)
        //{
        //    // Step 4: apply FLH offset
        //    matrix3D.Translate(flh.X, flh.Y, flh.Z);
        //    SingleVector3D result = Game.MatrixMultiply(matrix3D);
        //    // Resulting FLH is mirrored along X axis, so we mirror it back - Kerbiter
        //    result.Y *= -1;
        //    return result;
        //}
    }
}
