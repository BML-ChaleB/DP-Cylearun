using Extension.CWUtilities;
using Extension.INI;
using PatcherYRpp;
using PatcherYRpp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.CY
{
    public partial class TechnoGlobalExtension
    {

        [ReceiveDamageAction]
        public unsafe void TechnoClass_ReceiveDamage_BreakDown(Pointer<int> pDamage, int DistanceFromEpicenter, Pointer<WarheadTypeClass> pWH, Pointer<ObjectClass> pAttacker, bool IgnoreDefenses, bool PreventPassengerEscape, Pointer<HouseClass> pAttackingHouse)
        {
            if (Data.BreakDownResistance == -1)
                return;

            var x = Data.BreakDownResistance;
            WarheadINI.Section = pWH.Ref.Base.ID;
            var y = WarheadINI.Data.BreakDownLevel;
            var p = WarheadINI.Data.BreakDownChanceReduce;
            var t = WarheadINI.Data.OverBreakDownChanceReduce;
            var damageMultipler1 = WarheadINI.Data.NotBreakDown;
            var damageMultipler2 = WarheadINI.Data.BreakDown;
            var damageMultipler3 = WarheadINI.Data.OverBreakDown;

            if (y == -1)
                return;

            //无法击穿
            if (x > 2 * y)
            {
                pDamage.Ref = (int)(pDamage.Ref * GetBreakDownMultipler(damageMultipler1));
                var pAnim = AnimTypeClass.ABSTRACTTYPE_ARRAY.Find(DpSettingINI.Data.CantBreakDownAnim);
                if (pAnim.IsNotNull)
                {
                    YRMemory.Create<AnimClass>(pAnim, Owner.OwnerObject.Ref.Base.Base.GetCoords());
                }
                return;
            }


            //未击穿
            if (x < 2 * y && x > y)
            {
                int chance = x - y - p;
                if (chance <= 0) return;
                var rd = MathEx.Random.Next(0, 100);
                if (rd <= chance)
                {
                    pDamage.Ref = (int)(pDamage.Ref * GetBreakDownMultipler(damageMultipler1));
                    var pAnim = AnimTypeClass.ABSTRACTTYPE_ARRAY.Find(DpSettingINI.Data.NotBreakDownAnim);
                    if (pAnim.IsNotNull)
                    {
                        YRMemory.Create<AnimClass>(pAnim, Owner.OwnerObject.Ref.Base.Base.GetCoords());
                    }
                }
                return;
            }


            //击穿
            if (y < 2 * x && y > x)
            {
                int chance = y - x - p;
                if (chance <= 0) return;
                var rd = MathEx.Random.Next(0, 100);
                if (rd <= chance)
                {
                    pDamage.Ref = (int)(pDamage.Ref * GetBreakDownMultipler(damageMultipler2));
                    var pAnim = AnimTypeClass.ABSTRACTTYPE_ARRAY.Find(DpSettingINI.Data.BreakDownAnim);
                    if (pAnim.IsNotNull)
                    {
                        YRMemory.Create<AnimClass>(pAnim, Owner.OwnerObject.Ref.Base.Base.GetCoords());
                    }
                }
                return;
            }

            //过度击穿
            if (y > 2 * x)
            {
                int chance = y - x - t;
                if (chance <= 0) return;
                var rd = MathEx.Random.Next(0, 100);
                if (rd <= chance)
                {
                    pDamage.Ref = (int)(pDamage.Ref * GetBreakDownMultipler(damageMultipler3));
                    var pAnim = AnimTypeClass.ABSTRACTTYPE_ARRAY.Find(DpSettingINI.Data.OverBreakDownAnim);
                    if (pAnim.IsNotNull)
                    {
                        YRMemory.Create<AnimClass>(pAnim, Owner.OwnerObject.Ref.Base.Base.GetCoords());
                    }
                }
                return;
            }

        }


        private double GetBreakDownMultipler(double multilpler)
        {
            var intmult = (int)(multilpler * 100);
            return MathEx.Random.Next(Math.Min(intmult, 100), Math.Max(intmult, 100)) / 100d;
        }
    }

    public partial class TechnoGlobalTypeExt
    {
        [INIField(Key = "BreakDown.Resistance")]
        public int BreakDownResistance = -1;
    }

    public partial class WeaponTypeExt
    {
      
    }

    public partial class WarheadTypeExt 
    {
        [INIField(Key = "BreakDown.Level")]
        public int BreakDownLevel = -1;
        [INIField(Key = "DamageMultipler.NotBreakDown")]
        public double NotBreakDown = 1.0;
        [INIField(Key = "DamageMultipler.BreakDown")]
        public double BreakDown = 1.0;
        [INIField(Key = "DamageMultipler.OverBreakDown")]
        public double OverBreakDown = 1.0;
        [INIField(Key = "BreakDown.BreakDownChanceReduce")]
        public int BreakDownChanceReduce = 0;
        [INIField(Key = "BreakDown.OverBreakDownChanceReduce")]
        public int OverBreakDownChanceReduce = 0;
    }

    public partial class GlobalSetting 
    {
        [INIField(Key = "BreakDown.CannotBreanDownAnim")]
        public string CantBreakDownAnim;
        [INIField(Key = "BreakDown.NotBreakDownAnim")]
        public string NotBreakDownAnim;
        [INIField(Key = "BreakDown.BreakDownAnim")]
        public string BreakDownAnim;
        [INIField(Key = "BreakDown.OverBreakDownAnim")]
        public string OverBreakDownAnim;



    }
}
