using Extension.Ext;
using Extension.Utilities;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Cylearun.Utils
{
    public class Finder
    {
        public static List<TechnoExt> FindTechno(Pointer<HouseClass> pHouse, Func<Pointer<TechnoClass>, bool> expression, FindRange findRange)
        {
            ref DynamicVectorClass<Pointer<TechnoClass>> technos = ref TechnoClass.Array;
            List<TechnoExt> targets = new List<TechnoExt>();
            for (int i = technos.Count - 1; i >= 0; i--)
            {
                Pointer<TechnoClass> pTechno = technos.Get(i);
         
                if(IsValidTechno(pTechno,pHouse,expression,findRange))
                {
                    targets.Add(TechnoExt.ExtMap.Find(pTechno));
                }
            }
            return targets;
        }

        public static TechnoExt FineOneTechno(Pointer<HouseClass> pHouse, Func<Pointer<TechnoClass>, bool> expression, FindRange findRange)
        {
            ref DynamicVectorClass<Pointer<TechnoClass>> technos = ref TechnoClass.Array;
            TechnoExt tref = null;

            for (int i = technos.Count - 1; i >= 0; i--)
            {
                Pointer<TechnoClass> pTechno = technos.Get(i);

                if (IsValidTechno(pTechno, pHouse, expression, findRange))
                {
                    return TechnoExt.ExtMap.Find(pTechno);
                }
            }
            return tref;
        }

        private static bool IsValidTechno(Pointer<TechnoClass> pTechno,Pointer<HouseClass> pHouse, Func<Pointer<TechnoClass>, bool> expression, FindRange findRange)
        {
            var houseIndex = pHouse.Ref.ArrayIndex;

            if (pTechno.IsNull)
                return false;
            if (pTechno.Ref.Owner.IsNull)
                return false;

            switch (findRange)
            {
                case FindRange.All:
                    {
                        break;
                    }
                case FindRange.Owner:
                    {
                        if (pTechno.Ref.Owner.Ref.ArrayIndex != houseIndex)
                            return false;
                        break;
                    }
                case FindRange.Allies:
                    {
                        if (pTechno.Ref.Owner.Ref.ArrayIndex != houseIndex && !pHouse.Ref.IsAlliedWith(pTechno.Ref.Owner.Ref.ArrayIndex))
                            return false;
                        break;
                    }
                case FindRange.Enermy:
                    {
                        if (pTechno.Ref.Owner.Ref.ArrayIndex == houseIndex || pHouse.Ref.IsAlliedWith(pTechno.Ref.Owner.Ref.ArrayIndex) || !pTechno.Ref.Base.IsAlive || pTechno.Ref.Owner.Ref.Type.Ref.Base.ID == "Special" || pTechno.Ref.Owner.Ref.Type.Ref.Base.ID == "Neutral")
                            return false;
                        break;
                    }
                default:
                    break;
            }

            return expression(pTechno);

        }
    }

    public enum FindRange
    {
        All,
        Owner,
        Allies,
        Enermy
    }
}
