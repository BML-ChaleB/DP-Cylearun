using DynamicPatcher;
using Extension.CWUtilities;
using Extension.INI;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Extension.CY
{
    public partial class TechnoGlobalTypeExt
    {
        [INIField(Key = "SpecialTarget")]
        public String[] SpecialTarget;
    }

    public partial class GlobalSetting
    {
        [INIField(Key = "SpecialTarget.TotalWarhead")]
        public string TotalWarhead;
    }

    public partial class TechnoGlobalExtension
    {
        [ReceiveDamageAction]
        public unsafe void TechnoClass_ReceiveDamage_SpecialTarget(Pointer<int> pDamage, int DistanceFromEpicenter, Pointer<WarheadTypeClass> pWH, Pointer<ObjectClass> pAttacker, bool IgnoreDefenses, bool PreventPassengerEscape, Pointer<HouseClass> pAttackingHouse)
        {
            if (Data.SpecialTarget == null || Data.SpecialTarget.Length < 1)
            {
                return;
            }

            string attackWarhead = pWH.Ref.Base.ID;
            string receiveDamageTechno = Owner.OwnerObject.Ref.Type.Ref.Base.Base.ID;
            // Logger.Log($"脚本开始 --- 当前挨打单位{receiveDamageTechno},攻击的弹头{attackWarhead}");

            List<string> specialTargetList = Data.SpecialTarget.ToList();

            //排除结算弹头
            String totalWarhead = DpSettingINI.Data.TotalWarhead;
            if (attackWarhead == totalWarhead)
            {
                // Logger.Log($"当前攻击的弹头{attackWarhead}是结算弹头，被排除了");
                return;
            }

            //设置当前弹头Section
            WarheadINI.Section = attackWarhead;

            //排除脚本中打上的弹头
            List<string> readWarhead = new List<string>();
            List<string> readSpecialTarget = new List<string>();
    
            for (int i = 0; i < specialTargetList.Count; i++)
            {
                // Logger.Log($"当前读取的单位{specialTargetList[i]}");
                string targetWarhead = WarheadINI.Get<String>($"SpecialTarget.{specialTargetList[i]}.Warhead");
                // Logger.Log($"当前读取弹头的Key:{$"SpecialTarget.{specialTargetList[i]}.Warhead"} --- 当前读取到的弹头信息:{targetWarhead}");

                //排除未写特殊攻击的弹头
                if (string.IsNullOrEmpty(targetWarhead))
                {
                    // Logger.Log($"当前攻击的弹头{attackWarhead}不是特殊攻击弹头，被排除了");
                    continue;
                }

                readSpecialTarget.Add(specialTargetList[i]);
                readWarhead.Add(targetWarhead);
            }

            if (readWarhead.Contains(attackWarhead))
            {
                // Logger.Log($"当前攻击的弹头{attackWarhead}是本脚本中打出的弹头，被排除了");
                return;
            }

            //排除未写特殊攻击的弹头
            if (readWarhead.Count <= 0)
            {
                // Logger.Log($"当前攻击的弹头{attackWarhead}不是特殊攻击弹头，被排除了");
                return;
            }

            double totalDamage = 0;

            for (int i = 0; i < readWarhead.Count; i++)
            {
                Pointer<WarheadTypeClass> targetWarheadType = WarheadTypeClass.ABSTRACTTYPE_ARRAY.Find(readWarhead[i]);

                if (targetWarheadType.IsNotNull)
                {
                    int trueDamage = MapClass.GetTotalDamage(pDamage.Ref, pWH, Owner.OwnerObject.Ref.Type.Ref.Base.Armor, DistanceFromEpicenter);

                    pDamage.Ref = 1;

                    // Logger.Log($"给当前挨打单位{receiveDamageTechno}打上{readWarhead[i]}弹头");
                    Owner.OwnerObject.Ref.Base.TakeDamage(1, targetWarheadType, pAttacker, pAttackingHouse, false);

                    //设置当前弹头Section
                    WarheadINI.Section = attackWarhead;

                    double targetVerses = WarheadINI.Get<double>($"SpecialTarget.{readSpecialTarget[i]}.Verses");
                    // Logger.Log($"当前弹头{attackWarhead}真实造成的伤害为：{trueDamage} --- 当前读取target比如的key:SpecialTarget.{readSpecialTarget[i]}.Verses - target比例为：{targetVerses}");

                    if (0 == targetVerses)
                    {
                        targetVerses = 1;
                    }

                    totalDamage += (trueDamage * targetVerses);
                }
            }
            //之前会存在多个造成1点伤害的弹头
            totalDamage -= (readWarhead.Count * pDamage.Ref);

            Pointer<WarheadTypeClass> totalWarheadType = WarheadTypeClass.ABSTRACTTYPE_ARRAY.Find(totalWarhead);
            // Logger.Log($"当前单位{Owner.OwnerObject.Ref.Type.Ref.Base.Base.ID}结算总伤害为：{totalDamage} --- 结算弹头为:{totalWarhead}");
            
            Owner.OwnerObject.Ref.Base.TakeDamage((int)totalDamage, totalWarheadType, pAttacker, pAttackingHouse, false);
        }
    }
}
