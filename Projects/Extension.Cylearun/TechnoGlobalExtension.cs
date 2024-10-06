using DynamicPatcher;
using Extension.CWUtilities;
using Extension.Ext;
using Extension.INI;
using Extension.Script;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.CY
{

    [Serializable]
    [GlobalScriptable(typeof(TechnoExt))]
    public partial class TechnoGlobalExtension : TechnoScriptable
    {
        public TechnoGlobalExtension(TechnoExt owner) : base(owner)
        {
            INI = this.CreateRulesIniComponentWith<TechnoGlobalTypeExt>(Owner.OwnerObject.Ref.Type.Ref.Base.Base.ID);
            ArtINI = this.CreateArtIniComponentWith<TechnoGlobalArtExt>(!string.IsNullOrEmpty(INI.Data.Image) ? INI.Data.Image : Owner.OwnerObject.Ref.Type.Ref.Base.Base.ID);
            WeaponINI = this.CreateRulesIniComponentWith<WeaponTypeExt>("Speical");
            WarheadINI = this.CreateRulesIniComponentWith<WarheadTypeExt>("Speical");
            DpSettingINI = this.CreateRulesIniComponentWith<GlobalSetting>("DPSetting");
            LogEnabled = DpSettingINI.Data.LogEnabled;
        }

        INIComponentWith<TechnoGlobalTypeExt> INI;

        INIComponentWith<TechnoGlobalArtExt> ArtINI;

        INIComponentWith<WeaponTypeExt> WeaponINI;

        INIComponentWith<WarheadTypeExt> WarheadINI;


        INIComponentWith<GlobalSetting> DpSettingINI;

        private bool LogEnabled = false;


        public TechnoGlobalTypeExt Data => INI.Data;
        public TechnoGlobalArtExt Art => ArtINI.Data;

        public void Log(string message)
        {
            if (LogEnabled)
            {
                Logger.Log(message);
            }
        }

        public override void Awake()
        {
            PartialHelper.TechnoAwakeAction(this);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            PartialHelper.TechnoUpdateAction(this);
        }

        public override void OnPut(CoordStruct coord, Direction faceDir)
        {
            base.OnPut(coord, faceDir);
            PartialHelper.TechnoPutAction(this, coord, faceDir);
        }

        public override void OnRemove()
        {
            base.OnRemove();
            PartialHelper.TechnoRemoveAction(this);
        }

        public override void OnFire(Pointer<AbstractClass> pTarget, int weaponIndex)
        {
            base.OnFire(pTarget, weaponIndex);
            PartialHelper.TechnoFireAction(this, pTarget, weaponIndex);
        }

        public override void OnReceiveDamage(Pointer<int> pDamage, int DistanceFromEpicenter, Pointer<WarheadTypeClass> pWH, Pointer<ObjectClass> pAttacker, bool IgnoreDefenses, bool PreventPassengerEscape, Pointer<HouseClass> pAttackingHouse)
        {
            base.OnReceiveDamage(pDamage, DistanceFromEpicenter, pWH, pAttacker, IgnoreDefenses, PreventPassengerEscape, pAttackingHouse);
            PartialHelper.TechnoReceiveDamageAction(this, pDamage, DistanceFromEpicenter, pWH, pAttacker, IgnoreDefenses, PreventPassengerEscape, pAttackingHouse);
        }

    }

    public partial class WeaponTypeExt : INIAutoConfig
    {

    }

    public partial class WarheadTypeExt : INIAutoConfig
    {

    }

    [Serializable]
    public partial class TechnoGlobalTypeExt : INIAutoConfig
    {
        [INIField(Key = "Image")]
        public string Image = string.Empty;

     
    }

    [Serializable]
    public partial class TechnoGlobalArtExt : INIAutoConfig
    {

    }


    public partial class GlobalSetting:INIAutoConfig
    {
        [INIField(Key = "LogEnabled")]
        public bool LogEnabled = false;
    }


}
