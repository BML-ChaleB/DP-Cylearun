using DynamicPatcher;
using Extension.Ext;
using Extension.INI;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Extension.CY
{
    public partial class TechnoGlobalExtension
    {
        public bool CanFire(Pointer<AbstractClass> pTarget, Pointer<WeaponTypeClass> pWeapon)
        {
            if (pTarget.IsNull)
                return true;

            if (!pTarget.Ref.IsInAir())
                return true;

            if (pTarget.IsNull)
                return true;

            if (pTarget.CastToTechno(out var pTechno))
            {
                var technoExt = TechnoExt.ExtMap.Find(pTechno);
                if (technoExt.IsNullOrExpired())
                    return true;

                if (technoExt.IsNullOrExpired())
                    return true;

                var gscript = technoExt.GameObject.GetComponent<TechnoGlobalExtension>();
                if (gscript is null)
                    return true;

                if (gscript.Data.TooHighToBeAttacked == false)
                    return true;

                var coords = Owner.OwnerObject.Ref.Base.Base.GetCoords();
                var currentHeight = coords.Z;
             

                WeaponINI.Section = pWeapon.Ref.Base.ID;
                var weaponHeight = WeaponINI.Data.AntiAirMaxHeight;
                if (weaponHeight == -1)
                    return true;

                if (weaponHeight  + currentHeight < gscript.Data.FlyingZAdjust + Owner.OwnerObject.Ref.Base.Base.GetCoords().Z)
                {
                    return false;
                }
            }

            return true;
        }


    }




    public partial class TechnoGlobalTypeExt
    {
        /// <summary>
        /// 判断高度时的补偿值
        /// </summary>
        [INIField(Key = "FlyingZAdjust")]
        public int FlyingZAdjust = 0;

        /// <summary>
        /// 是否启用高空逻辑
        /// </summary>
        [INIField(Key = "TooHighToBeAttacked")]
        public bool TooHighToBeAttacked = false;
    }

    public partial class WeaponTypeExt
    {
        [INIField(Key = "AntiAirMaxHeight")]

        public int AntiAirMaxHeight = -1;
    }
}
