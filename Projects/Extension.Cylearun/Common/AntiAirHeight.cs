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

                if (gscript.Data.FlyingHeightInAir <= 0)
                    return true;

                var coords = Owner.OwnerObject.Ref.Base.Base.GetCoords();
                var currentHeight = coords.Z;
             

                WeaponINI.Section = pWeapon.Ref.Base.ID;
                var weaponHeight = WeaponINI.Data.AntiAirMaxHeight;
                if (weaponHeight == -1)
                    return true;

                Logger.Log($"当前高度:{currentHeight}，武器高度:{weaponHeight},飞行高度{gscript.Data.FlyingHeightInAir}");

                if (weaponHeight  + currentHeight < gscript.Data.FlyingHeightInAir)
                {
                    Logger.Log($"打不到");
                    return false;
                }
                Logger.Log($"打的到");
            }

            return true;
        }


    }




    public partial class TechnoGlobalTypeExt
    {
        [INIField(Key = "FlyingHeightInAir")]
        public int FlyingHeightInAir = 0;
    }

    public partial class WeaponTypeExt
    {
        [INIField(Key = "AntiAirMaxHeight")]

        public int AntiAirMaxHeight = -1;
    }
}
