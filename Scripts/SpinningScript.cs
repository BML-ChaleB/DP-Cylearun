﻿
using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DynamicPatcher;
using PatcherYRpp;
using Extension.Ext;
using Extension.Script;
using System.Threading.Tasks;
using PatcherYRpp.Utilities;
using Extension.Decorators;
using Extension.Utilities;
using System.Runtime.Serialization;

namespace Scripts
{
    [Serializable]
    public class SpinningScript : TechnoScriptable
    {
        public SpinningScript(TechnoExt owner) : base(owner) { }

        int frame = 0;
        static Random random = new Random(114514);
        const int ShotDelta = 5;

        public override void OnUpdate()
        {
            if (Owner.OwnerObject.Ref.Target.IsNull)
            {
                return;
            }

            Turn();
            if (MathEx.Repeat(frame++, ShotDelta + random.Next(0, 2)) < 1)
            {
                FireForward();
            }
        }

        float rad = 0;

        // define your turnning speed!
        private float GetGrowStep()
        {
            return (float)(Math.PI / 180);
        }
        private void Turn()
        {
            Pointer<TechnoClass> pTechno = Owner.OwnerObject;


            rad = MathEx.Wrap(rad + GetGrowStep(), 0, (float)Math.PI * 2);

            pTechno.Ref.TurretFacing.set(new DirStruct(rad));
        }


        static Pointer<WeaponTypeClass> Weapon => WeaponTypeClass.ABSTRACTTYPE_ARRAY.Find("RedEye2");
        static Pointer<WarheadTypeClass> Warhead => WarheadTypeClass.ABSTRACTTYPE_ARRAY.Find("BlimpHE");

        const double ShotRange = 3000;

        private void FireForward()
        {
            Pointer<TechnoClass> pTechno = Owner.OwnerObject;

            var forward = MathEx.GetForwardVector(pTechno, true);
            var location = pTechno.Ref.Base.Base.GetCoords().ToVector3D();
            var from = location + forward * 300;
            var to = location + forward * ShotRange;

            bool TryCreateBullet(out Pointer<BulletClass> pBullet)
            {
                if (MapClass.Instance.TryGetCellAt(to.ToCoordStruct(), out var pCell))
                {
                    Pointer<WeaponTypeClass> pWeapon = Weapon;
                    Pointer<WarheadTypeClass> pWarhead = Warhead;

                    pBullet = pWeapon.Ref.Projectile.Ref.CreateBullet(
                        pCell.Convert<AbstractClass>(), pTechno,
                        pWeapon.Ref.Damage, pWarhead,
                        1, pWeapon.Ref.Bright); // set speed to 1 to reducing ww's logic effect.
                    return true;
                }

                pBullet = Pointer<BulletClass>.Zero;
                return false;
            }

            if (TryCreateBullet(out var pBullet))
            {
                BulletExt bulletExt = BulletExt.ExtMap.Find(pBullet);
                if (bulletExt.Get(BulletDecorator.UniqueID) == null)
                {
                    bulletExt.CreateDecorator<BulletDecorator>(BulletDecorator.UniqueID, "Bullet Effect Decorator");
                }

                pBullet.Ref.MoveTo(from.ToCoordStruct(), default);
            }
        }


        [Serializable]
        public class BulletDecorator : EventDecorator
        {
            public static DecoratorId UniqueID = new DecoratorId(1919810);
            public BulletDecorator()
            {
            }

            static readonly string[] DebrisNames = new[] {
                "DBRIS1LG",
                "DBRIS2LG",
                "DBRIS3LG",
                "DBRIS4LG",
                "DBRIS5LG",
                "DBRIS6LG",
                "DBRIS7LG",
                "DBRIS8LG",
                "DBRIS9LG",
                "DBRS10LG",
                "DBRIS1SM",
                "DBRIS2SM",
                "DBRIS3SM",
                "DBRIS4SM",
                "DBRIS5SM",
                "DBRIS6SM",
                "DBRIS7SM",
                "DBRIS8SM",
                "DBRIS9SM",
                "DBRS10SM",
            };
            static Pointer<AnimTypeClass>[] Debris = AnimTypeClass.ABSTRACTTYPE_ARRAY.Finds(DebrisNames);
            // low performence loading game.
            // we should load again when loading game.
            [OnDeserializing]
            protected void OnDeserializing(StreamingContext context)
            {
                Debris = AnimTypeClass.ABSTRACTTYPE_ARRAY.Finds(DebrisNames);
            }

            static ColorStruct innerColor = new ColorStruct(208, 10, 10);
            static ColorStruct outerColor = new ColorStruct(88, 0, 5);
            static ColorStruct outerSpread = new ColorStruct(10, 10, 10);

            public override void OnUpdate()
            {
                BulletExt bulletExt = Decorative as BulletExt;
                Pointer<BulletClass> pBullet = bulletExt.OwnerObject;
                CoordStruct location = pBullet.Ref.Base.Base.GetCoords();

                if (MapClass.Instance.TryGetCellAt(location, out Pointer<CellClass> pCell))
                {
                    // brust random debris when move over something.
                    if (pCell.Ref.FirstObject.IsNull == false)
                    {
                        var pAnimType = Debris[random.Next(0, Debris.Length)];
                        if (pAnimType.IsNull == false)
                        {
                            Pointer<AnimClass> pAnim = YRMemory.Create<AnimClass>(pAnimType, location);
                            pAnim.Ref.Bounce.Velocity += new SingleVector3D(0, 0, 20);
                            //Console.WriteLine($"fly debris {(string)pAnimType.Ref.Base.Base.ID}!");
                        }
                    }
                }

                CoordStruct target = pBullet.Ref.TargetCoords;

                // custom bullet trajectory
                var direction = (target - location).ToVector3D();
                pBullet.Ref.Base.Location += 
                    (MathEx.GetNormalizedVector3D(direction) * MathEx.Clamp(direction.Magnitude(), 5, 200)).ToCoordStruct();

                Pointer<LaserDrawClass> pLaser = YRMemory.Create<LaserDrawClass>(location, target, innerColor, outerColor, outerSpread, 1);
                pLaser.Ref.Thickness = 1;
                pLaser.Ref.IsHouseColor = true;

            }
        }
    }
}