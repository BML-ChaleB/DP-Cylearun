using Extension.Ext;
using Extension.INI;
using Extension.Script;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripts.AE
{
    public class VeterancyChangeAttachEffectScript : AttachEffectScriptable
    {
        public VeterancyChangeAttachEffectScript(TechnoExt owner) : base(owner)
        {
        }

        private string warhead = string.Empty;

        public override void OnUpdate()
        {
            if(Duration <= 0)
            {
                if(!string.IsNullOrWhiteSpace(warhead))
                {
                    var ini = Owner.GameObject.CreateRulesIniComponentWith<VeterancyChangeAttachEffectData>(warhead);
                    var vertency = ini.Data.Vertency;

                    if(vertency != 0)
                    {
                        var current = Owner.OwnerObject.Ref.Veterancy.Veterancy;
                        var result = current + vertency;
                        if (result < 1)
                        {
                            Owner.OwnerObject.Ref.Veterancy.SetRookie(true);
                        }else if(result >= 1 && result < 2)
                        {
                            Owner.OwnerObject.Ref.Veterancy.SetVeteran(true);
                        }
                        else
                        {
                            Owner.OwnerObject.Ref.Veterancy.SetElite(true);
                        }
                    }
                }
            }
            base.OnUpdate();
        }


        public override void OnAttachEffectPut(Pointer<int> pDamage, Pointer<WarheadTypeClass> pWH, Pointer<ObjectClass> pAttacker, Pointer<HouseClass> pAttackingHouse)
        {
            var warhead = pWH.Ref.Base.ID;
            base.OnAttachEffectPut(pDamage, pWH, pAttacker, pAttackingHouse);
        }

        public override void OnAttachEffectRecieveNew(int duration, Pointer<int> pDamage, Pointer<WarheadTypeClass> pWH, Pointer<ObjectClass> pAttacker, Pointer<HouseClass> pAttackingHouse)
        {
            return;
        }
    }

    public partial class VeterancyChangeAttachEffectData : INIAutoConfig
    {
        [INIField(Key = "AttachEffectScript.Vertency")]
        public int Vertency = 0;
    }

}
