using DynamicPatcher;
using Extension.Cylearun.Utils;
using Extension.Ext;
using Extension.INI;
using Extension.Script;
using PatcherYRpp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripts.TestScripts
{
    [Serializable]
    public class RefreshSWScriptArgs : INIAutoConfig
    {
        [INIField(Key = "RefreshSWScript.EffectSWList")]
        public String[] effectSWList;

        //All、Owner、Allies、OnlyAllies、Enermy
        [INIField(Key = "RefreshSWScript.EffectHouse")]
        public String effectHouse = "Owner";

        [INIField(Key = "RefreshSWScript.RefreshTime")]
        public int refreshTime = -1000;

        [INIField(Key = "RefreshSWScript.HasLimit")]
        public bool hasLimit = true;

        [INIField(Key = "RefreshSWScript.CanRefreshReady")]
        public bool canRefreshReady = true;
    }

    [Serializable]
    [ScriptAlias(nameof(RefreshSWScript))]
    public class RefreshSWScript : SuperWeaponScriptable
    {
        public RefreshSWScript(SuperWeaponExt owner) : base(owner)
        {
            INIComponentWith<RefreshSWScriptArgs> ini = this.CreateRulesIniComponentWith<RefreshSWScriptArgs>(Owner.OwnerObject.Ref.Type.Ref.Base.ID);
            this.effectSWList = ini.Data.effectSWList.ToList();
            this.effectHouse = ini.Data.effectHouse;
            this.refreshTime = ini.Data.refreshTime;
            this.hasLimit = ini.Data.hasLimit;
            this.canRefreshReady = ini.Data.canRefreshReady;
        }

        private List<String> effectSWList;
        private String effectHouse;
        private int refreshTime;
        private bool hasLimit;
        private bool canRefreshReady;

        public override void OnLaunch(CellStruct cell, bool isPlayer)
        {
            Pointer<SuperClass> launchSuper = Owner.OwnerObject;
            Pointer<HouseClass> ownerHouse = launchSuper.Ref.Owner;

            List<int> targetHouseList = new List<int>();
            for (int i = 0; i < HouseClass.Array.Count; i++)
            {
                Pointer<HouseClass> house = HouseClass.Array.Get(i);
                if (!house.IsNull)
                {
                    String houseName = house.Ref.Type.Ref.Base.ID.ToString();
                    Logger.Log($"获取的全部阵营:{houseName}");
                    //跳过中立阵营
                    if (houseName != "Special" && houseName != "Neutral")
                    {
                        continue;
                    }
                    //根据标签筛选阵营
                    switch (effectHouse)
                    {
                        case "All":
                            {
                                targetHouseList.Add(house.Ref.ArrayIndex);
                                break;
                            }
                        case "Owner":
                            {
                                break;
                            }
                        case "OnlyAllies":
                        case "Allies":
                            {
                                if (house.Ref.IsAlliedWith(Owner.OwnerObject.Ref.Owner) && house.Ref.ArrayIndex != ownerHouse.Ref.ArrayIndex)
                                {
                                    targetHouseList.Add(house.Ref.ArrayIndex);
                                }
                                break;
                            }
                        case "Enermy":
                            {
                                if (!house.Ref.IsAlliedWith(Owner.OwnerObject.Ref.Owner) && house.Ref.ArrayIndex != ownerHouse.Ref.ArrayIndex)
                                {
                                    targetHouseList.Add(house.Ref.ArrayIndex);
                                }
                                break;
                            }
                        default:
                            break;
                    }
                }
            }
            //如果标签是Owner和Allies则添加自己阵营
            if ("Owner" == effectHouse || "Allies" == effectHouse)
            {
                targetHouseList.Add(ownerHouse.Ref.ArrayIndex);
            }

            //用于Debug
            Logger.Log($"能影响的阵营数量:{targetHouseList.Count}");
            for (int i = 0; i < targetHouseList.Count; i++)
            {
                Pointer<HouseClass> targetHouse = HouseClass.FindBySideIndex(targetHouseList[i]);
                String houseName = targetHouse.Ref.Type.Ref.Base.ID.ToString();
                Logger.Log($"能影响的阵营:{houseName}");
            }

            //遍历所有超武
            SuperWeaponTypeClass.ABSTRACTTYPE_ARRAY.ToList().ForEach(type =>
            {
                for (int i = 0; i < targetHouseList.Count; i++)
                {
                    Pointer<HouseClass> targetHouse = HouseClass.FindBySideIndex(targetHouseList[i]);
                    Pointer<SuperClass> pSuper = targetHouse.Ref.FindSuperWeapon(type);
                    //是否包含在标签限制的超武中
                    if (!effectSWList.Contains(type.Ref.Base.ID))
                    {
                        return;
                    }
                    Logger.Log($"变更前 - 超武:{type.Ref.Base.ID} --- 超武总CD：{type.Ref.RechargeTime} - 当前超武CD:{pSuper.Ref.RechargeTimer.GetTimeLeft()}");
                    //
                    if (!pSuper.Ref.Granted)
                    {
                        Logger.Log($"超武{type.Ref.Base.ID}还不存在");
                        return;
                    }
                    //是否有超武总CD限制
                    if (hasLimit)
                    {
                        if (type.Ref.RechargeTime < (pSuper.Ref.RechargeTimer.GetTimeLeft() + refreshTime))
                        {
                            Logger.Log($"超武{type.Ref.Base.ID}变更时间超过超武总CD限制");
                            pSuper.Ref.Reset();
                            return;
                        }
                    }
                    //是否能影响就绪的超武
                    if (canRefreshReady)
                    {
                        if (pSuper.Ref.IsCharged && refreshTime > 0)
                        {
                            pSuper.Ref.Reset();
                            pSuper.Ref.RechargeTimer.TimeLeft = refreshTime;
                            Logger.Log($"已就绪超武{type.Ref.Base.ID}更改时间，当前超武变更CD:{pSuper.Ref.RechargeTimer.GetTimeLeft()}");
                            return;
                        }
                    }
                    else
                    {
                        if (pSuper.Ref.IsCharged)
                        {
                            Logger.Log($"超武{type.Ref.Base.ID}已就绪，不能更改时间");
                            return;
                        }
                    }
                    //加上变更后当前超武时间是否小于0
                    if ((pSuper.Ref.RechargeTimer.GetTimeLeft() + refreshTime) < 0)
                    {
                        Logger.Log($"超武{type.Ref.Base.ID}变更时间小于0，刷新超武");
                        pSuper.Ref.IsCharged = true;
                        return;
                    }
                    //变更超武时间
                    pSuper.Ref.RechargeTimer.TimeLeft += refreshTime;
                    Logger.Log($"变更后 - 超武:{type.Ref.Base.ID} --- 超武总CD：{type.Ref.RechargeTime} - 当前超武变更CD:{pSuper.Ref.RechargeTimer.GetTimeLeft()}");
                }
            });
            base.OnLaunch(cell, isPlayer);
        }
    }
}
