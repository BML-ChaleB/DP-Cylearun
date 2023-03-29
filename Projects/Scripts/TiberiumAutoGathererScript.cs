using Extension.Ext;
using Extension.INI;
using Extension.Script;
using PatcherYRpp;
using PatcherYRpp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripts
{
    [ScriptAlias(nameof(TiberiumAutoGathererScript))]
    [Serializable]
    public class TiberiumAutoGathererScript : TechnoScriptable
    {
        public TiberiumAutoGathererScript(TechnoExt owner) : base(owner)
        {
        }

        private INIComponentWith<TiberiumAutoGathererData> INI;

        public override void Awake()
        {
            INI = Owner.GameObject.CreateRulesIniComponentWith<TiberiumAutoGathererData>(Owner.OwnerRef.Type.Ref.Base.Base.ID);
            delay = INI.Data.Delay;
        }

        private int delay;

        public override void OnUpdate()
        {
            if (delay-- > 0)
            {
                return;
            }

            delay = INI.Data.Delay;

            //获取脚下的矿
            var coord = Owner.OwnerObject.Ref.Base.Base.GetCoords();
            var currentCell = CellClass.Coord2Cell(coord);
            var enumerator = new CellSpreadEnumerator(INI.Data.CellSpread);

            foreach (CellStruct offset in enumerator)
            {
         
                CoordStruct where = CellClass.Cell2Coord(currentCell + offset, coord.Z);

                if (MapClass.Instance.TryGetCellAt(where, out var pCell))
                {
                    var value = pCell.Ref.GetContainedTiberiumValue();
                    if (value > 0)
                    {

                        var index = pCell.Ref.GetContainedTiberiumIndex();
                        var amount = value / (index == 0 ? 25f : 50f);

                        int maxAmountOnce = INI.Data.Amount;

                        amount = (amount > maxAmountOnce) ? (maxAmountOnce - amount) : amount;

                        pCell.Ref.ReduceTiberium((int)amount);

                        var cash = amount * (index == 0 ? 25f : 50f) * INI.Data.GatherMultipler;

                        if(!string.IsNullOrEmpty(INI.Data.AnimTiberium))
                        {
                            YRMemory.Create<AnimClass>(AnimTypeClass.ABSTRACTTYPE_ARRAY.Find(INI.Data.AnimTiberium), where);
                        }

                        if(!string.IsNullOrEmpty(INI.Data.AnimGatherer))
                        {
                            YRMemory.Create<AnimClass>(AnimTypeClass.ABSTRACTTYPE_ARRAY.Find(INI.Data.AnimGatherer), Owner.OwnerRef.Base.Base.GetCoords() + new CoordStruct(0, 0, INI.Data.AnimZOffest));
                        }

                        Owner.OwnerRef.Owner.Ref.GiveMoney((int)cash);


                        break;
                    }
                }
            }


        }
    }

    public class TiberiumAutoGathererData : INIAutoConfig
    {
        /// <summary>
        /// 检索范围
        /// </summary>
        [INIField(Key = "TiberiumAutoGather.CellSpread")]
        public uint CellSpread = 5;

        /// <summary>
        /// 检索延迟
        /// </summary>
        [INIField(Key = "TiberiumAutoGather.Delay")]
        public int Delay = 100;

        /// <summary>
        /// 单次最大采矿数，普矿1=25刀
        /// </summary>
        [INIField(Key = "TiberiumAutoGather.Amount")]
        public int Amount = 5;

        /// <summary>
        /// 矿石经济转换比
        /// </summary>
        [INIField(Key = "TiberiumAutoGather.GatherMultipler")]
        public float GatherMultipler = 1f;

        /// <summary>
        /// 采矿动画
        /// </summary>
        [INIField(Key = "TiberiumAutoGather.AnimTiberium")]
        public string AnimTiberium;

        /// <summary>
        /// 采矿者动画
        /// </summary>
        [INIField(Key = "TiberiumAutoGather.AnimGatherer")]
        public string AnimGatherer;

        /// <summary>
        /// 采矿者动画Z轴偏移
        /// </summary>
        [INIField(Key = "TiberiumAutoGather.AnimZOffest")]
        public int AnimZOffest = 0;



    }
}
