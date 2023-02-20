using DynamicPatcher;
using Extension.Ext;
using Extension.INI;
using Extension.Script;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripts.TestScripts
{
    public class BlackCatTScriptArgs : INIAutoConfig
    {
        [INIField(Key = "BlackCatTScript.Count")]
        public int Count = 9;

        [INIField(Key = "BlackCatTScript.Time")]
        public int Time = 90;

        [INIField(Key = "BlackCatTScript.Anim")]
        public string Anim = "none";

        [INIField(Key = "BlackCatTScript.Decay")]
        public bool Decay = false;
    }

    [Serializable]
    [ScriptAlias(nameof(BlackCatTScript))]
    public class BlackCatTScript : TechnoScriptable
    {
        private int Count;
        private int Time;
        private string Anim;
        private bool Decay;

        public BlackCatTScript(TechnoExt owner) : base(owner)
        {
            INIComponentWith<BlackCatTScriptArgs> ini = this.CreateRulesIniComponentWith<BlackCatTScriptArgs>(Owner.OwnerObject.Ref.Type.Ref.Base.Base.ID);
            this.Count = ini.Data.Count;
            this.Time = ini.Data.Time;
            this.Anim = ini.Data.Anim;
            this.Decay = ini.Data.Decay;

            this.Count++;
        }

        private bool inImmortal = false;

        private int ImmortalTime = 90;

        private double HealthMultiplier = 1;


        public override void OnUpdate()
        {
            
            if (inImmortal && --ImmortalTime <= 0)
            {
                //Logger.Log($"锁血状态结束，此时ImmortalTime={ImmortalTime}，此时Count={Count}");
                inImmortal = false;
                ImmortalTime = Time;

                Pointer<TechnoClass> ownerTechno = Owner.OwnerObject;

                if (Decay)
                {
                    if (Count>0 && Count < 10)
                    {
                        HealthMultiplier = Count * 0.1;
                        //Logger.Log($"设置血量比例为HealthMultiplier={HealthMultiplier}");
                    }
                }

                ownerTechno.Ref.Base.Health = (int)(ownerTechno.Ref.Type.Ref.Base.Strength * HealthMultiplier);
                //Logger.Log($"设置血量Health={ownerTechno.Ref.Base.Health}");
            }
            
            base.OnUpdate();
        }

        public override void OnReceiveDamage(Pointer<int> pDamage, int DistanceFromEpicenter, Pointer<WarheadTypeClass> pWH, Pointer<ObjectClass> pAttacker, bool IgnoreDefenses, bool PreventPassengerEscape, Pointer<HouseClass> pAttackingHouse)
        {
            Pointer<TechnoClass> ownerTechno = Owner.OwnerObject;
            int trueDamage = MapClass.GetTotalDamage(pDamage.Ref, pWH, ownerTechno.Ref.Type.Ref.Base.Armor, DistanceFromEpicenter);

            //Logger.Log($"遭受到{pAttacker.Ref.GetObjectType().Ref.Base.ID}的攻击，此次伤害trueDamage={trueDamage}，当前单位血量Health={ownerTechno.Ref.Base.Health}");

            if (!inImmortal && trueDamage >= ownerTechno.Ref.Base.Health && --Count > 0)
            {
                pDamage.Ref = 0;

                ownerTechno.Ref.Base.Health = 10;

                //Logger.Log($"受到致命伤害，此时Count={Count}，inImmortal={inImmortal}，当前血量Health={ownerTechno.Ref.Base.Health}，受到的真实伤害trueDamage={trueDamage}");
                if ("Super" == pWH.Ref.Base.ID)
                {
                    //Logger.Log("受到游戏结算的伤害");
                    return;
                }

                inImmortal = true;

                if ("none" != Anim)
                {
                    Pointer<AnimTypeClass> pAnimType = AnimTypeClass.ABSTRACTTYPE_ARRAY.Find(Anim);
                    Pointer<AnimClass> pAnim = YRMemory.Create<AnimClass>(pAnimType, ownerTechno.Ref.Base.Base.GetCoords());
                    //Logger.Log("播放动画");
                }
               //Logger.Log($"开启锁血，当前伤害重置为Damage={pDamage.Ref},血量Health={ownerTechno.Ref.Base.Health}");
            }

            if (inImmortal)
            {
                pDamage.Ref = 0;

                ownerTechno.Ref.Base.Health = 10;
                //Logger.Log($"当前锁血，伤害重置为Damage={pDamage.Ref},当前血量Health={ownerTechno.Ref.Base.Health}");
            }

            base.OnReceiveDamage(pDamage, DistanceFromEpicenter, pWH, pAttacker, IgnoreDefenses, PreventPassengerEscape, pAttackingHouse);
        }

    }
}
