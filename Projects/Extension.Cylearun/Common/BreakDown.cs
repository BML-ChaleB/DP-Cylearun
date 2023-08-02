using Extension.CWUtilities;
using Extension.INI;
using PatcherYRpp;
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

            WarheadINI.Section = pWH.Ref.Base.ID;
            var whBreakDownLevel = WarheadINI.Data.BreakDownLevel;
            var damageMultipler1 = WarheadINI.Data.NotBreakDown;
            var damageMultipler2 = WarheadINI.Data.BreakDown;
            var damageMultipler3 = WarheadINI.Data.OverBreakDown;

            if (whBreakDownLevel == -1)
                return;

            if (Data.BreakDownResistance > 2 * whBreakDownLevel)
            {
                pDamage.Ref = (int)(pDamage.Ref * damageMultipler1);
            }

            
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
    }
}
