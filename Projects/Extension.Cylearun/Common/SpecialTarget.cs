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
        [INIField(Key = "SpecialTarget.TotalProject")]
        public string TotalProject;
    }

    public partial class SpecialTargetArgs
    {
        public string target;

        public bool warheadFlag = false;

        public string warhead;

        public bool DamageFlag = false;

        public double Damage;

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
            Logger.Log($"脚本开始 --- 当前挨打单位{receiveDamageTechno},攻击的弹头{attackWarhead},攻击的伤害{pDamage.Ref}");

            List<string> specialTargetList = Data.SpecialTarget.ToList();

            //设置当前弹头Section
            WarheadINI.Section = attackWarhead;

            List<SpecialTargetArgs> argsList = new List<SpecialTargetArgs>();
    
            for (int i = 0; i < specialTargetList.Count; i++)
            {
                SpecialTargetArgs args = new SpecialTargetArgs();
                args.target = specialTargetList[i];

                //Logger.Log($"当前读取的目标类型{specialTargetList[i]}");

                string targetWarhead = WarheadINI.Get<String>($"SpecialTarget.{specialTargetList[i]}.Warhead");
                Logger.Log($"当前读取弹头的Key:{$"SpecialTarget.{specialTargetList[i]}.Warhead"} --- 读取到的弹头信息:{targetWarhead}");

                //排除未写特殊攻击的弹头
                if (!string.IsNullOrEmpty(targetWarhead))
                {
                    if (attackWarhead == targetWarhead)
                    {
                        Logger.Log($"当前攻击的弹头{attackWarhead}和读取弹头{targetWarhead}是套娃弹头,不许套娃");
                        continue;
                    }

                    args.warheadFlag = true;
                    args.warhead = targetWarhead;
                }

                double targetVerses = WarheadINI.Get<double>($"SpecialTarget.{specialTargetList[i]}.Verses");
                Logger.Log($"当前读取弹头的Key:{$"SpecialTarget.{specialTargetList[i]}.Verses"} --- 读取到的弹头比例:{targetVerses}");

                if (0 != targetVerses)
                {
                    args.DamageFlag = true;
                    args.Damage = targetVerses;
                }

                if (args.warheadFlag || args.DamageFlag)
                {
                    argsList.Add(args);
                }
            }

            //排除未写特殊攻击的弹头
            if (argsList.Count < 1)
            {
                Logger.Log($"当前攻击的弹头{attackWarhead}不是特殊攻击弹头，被排除了");
                return;
            }

            string totalProject = DpSettingINI.Data.TotalProject;
            Pointer<BulletTypeClass> totalBulletType = BulletTypeClass.ABSTRACTTYPE_ARRAY.Find(totalProject);

            List<string> readWarheadList = new List<string>();

            for (int i = 0; i < argsList.Count; i++)
            {
                SpecialTargetArgs args = argsList[i];
                Logger.Log($"当前读取的第:{i+1}个args: traget = {args.target}, DamageFlag = {args.DamageFlag}, Damage = {args.Damage}, warheadFlag = {args.warheadFlag}, warhead = {args.warhead}");

                if (args.DamageFlag)
                {
                    Logger.Log($"当前攻击伤害调整前为{pDamage.Ref},调整比例{args.Damage}");
                    pDamage.Ref = (int)(pDamage.Ref * args.Damage);
                    Logger.Log($"当前攻击伤害调整为{pDamage.Ref}");
                }

                if (args.warheadFlag)
                {
                    readWarheadList.Add(args.warhead);
                }
            }

            //记录总伤害
            double totalDamage = pDamage.Ref;
            Logger.Log($"计算后的总伤害{totalDamage}");

            if (readWarheadList.Count < 1)
            {
                Logger.Log($"当前不存在特殊弹头目标");
                return;
            }

            //去除原弹头的1点强制伤害
            totalDamage -= 1;

            for (int i = 0; i < readWarheadList.Count; i++)
            {
                Pointer<WarheadTypeClass> targetWarheadType = WarheadTypeClass.ABSTRACTTYPE_ARRAY.Find(readWarheadList[i]);

                if (targetWarheadType.IsNotNull)
                {
                    Logger.Log($"当前为第{i + 1}个弹头,还有{readWarheadList.Count - (i + 1)}个弹头未打");

                    double localDamage = 0;

                    if (i != readWarheadList.Count - 1)
                    {
                        localDamage = 1;

                        //去除当前弹头打出的强制1点伤害
                        totalDamage -= 1;

                    }
                    else
                    {
                        localDamage = totalDamage;
                    }

                    Logger.Log($"当前单位{Owner.OwnerObject.Ref.Type.Ref.Base.Base.ID}结算总伤害为：{localDamage} --- 结算抛射体为:{totalProject} 结算弹头为:{readWarheadList[i]}");

                    //取消原来直接打伤害
                    //Owner.OwnerObject.Ref.Base.TakeDamage((int)localDamage, totalWarheadType, pAttacker, pAttackingHouse, false);

                    Pointer<BulletClass> totalBullet = totalBulletType.Ref.CreateBullet(pAttacker.Convert<AbstractClass>(), pAttacker.Convert<TechnoClass>(), (int)localDamage, targetWarheadType, 100, false);
                    totalBullet.Ref.DetonateAndUnInit(Owner.OwnerObject.Ref.Base.Base.GetCoords());
                }
            }

            pDamage.Ref = 1;
        }
    }
}
