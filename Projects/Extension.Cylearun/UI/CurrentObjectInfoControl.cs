using DynamicPatcher;
using Extension.Ext;
using PatcherYRpp;
using PatcherYRpp.FileFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Cylearun.UI
{
    internal class CurrentObjectInfoControl : CCControl
    {
        protected override void OnLoad()
        {
            Rectangle = new RectangleStruct(1, Surface.ViewBound.Height - 49, 60, 48);
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnRender(Pointer<Surface> pSurface)
        {
            Point2D pos = Location;
            foreach (var p in Get())
            {
                pos = p.Render(pSurface, pos);
            }
        }

        public static List<TypeValuePair> Get()
        {
            var dic = new Dictionary<Pointer<TechnoTypeClass>, int>();
            foreach (var pObject in ObjectClass.CurrentObjects)
            {
                if (pObject.CastToTechno(out Pointer<TechnoClass> pTechno))
                {
                    if (dic.ContainsKey(pTechno.Ref.Type))
                    {
                        dic[pTechno.Ref.Type]++;
                    }
                    else
                    {
                        dic[pTechno.Ref.Type] = 1;
                    }
                }
            }
            return dic.Select(o => new TypeValuePair(o.Key, o.Value)).Where(o =>
            {
                var typeExt = TechnoTypeExt.ExtMap.Find(o.GetTechnoType());
                if (string.IsNullOrWhiteSpace(typeExt.ArtData.CameoPCX) && (string.IsNullOrWhiteSpace(typeExt.ArtData.Cameo) || !SHPCache.TryGet(typeExt.ArtData.Cameo + ".shp", out _)))
                {
                    return false;
                }
                return true;
            })
                .OrderBy(o => o.Type).ThenBy(o => o.Index).ToList();
        }
    }

    struct TypeValuePair
    {
        public TypeValuePair(Pointer<TechnoTypeClass> pType, int val) : this()
        {
            SetTechnoType(pType);
            Num = val;
        }

        public void SetTechnoType(Pointer<TechnoTypeClass> pType)
        {
            switch (pType.Ref.BaseAbstractType.Base.WhatAmI())
            {
                case AbstractType.BuildingType:
                    Type = AbstractType.BuildingType;
                    Index = pType.Cast<BuildingTypeClass>().Ref.ArrayIndex;
                    break;
                case AbstractType.UnitType:
                    Type = AbstractType.UnitType;
                    Index = pType.Cast<UnitTypeClass>().Ref.ArrayIndex;
                    break;
                case AbstractType.InfantryType:
                    Type = AbstractType.InfantryType;
                    Index = pType.Cast<InfantryTypeClass>().Ref.ArrayIndex;
                    break;
                case AbstractType.AircraftType:
                    Type = AbstractType.AircraftType;
                    Index = pType.Cast<AircraftTypeClass>().Ref.ArrayIndex;
                    break;
            }
        }
        public Pointer<TechnoTypeClass> GetTechnoType()
        {
            return GetTechnoType(Type, Index);

            unsafe Pointer<TechnoTypeClass> GetTechnoType(AbstractType abstractID, int idx)
            {
                var func = (delegate* unmanaged[Thiscall]<int, AbstractType, int, IntPtr>)ASM.FastCallTransferStation;
                return func(0x48DCD0, abstractID, idx);
            }

        }


        public int Index;
        public AbstractType Type;
        public int Num;

        public Point2D Render(Pointer<Surface> pSurface, Point2D pos)
        {
            var TypeExt = TechnoTypeExt.ExtMap.Find(GetTechnoType());
            //Console.WriteLine((TypeExt).Image);
            string XXicon = (AnsiStringPointer)new IntPtr(0x8204FC);
            if (!string.IsNullOrWhiteSpace(TypeExt.ArtData.CameoPCX))
            {
                PCXPointer.BlitCameo(TypeExt.ArtData.CameoPCX.ToLower(), pos, pSurface);
            }
            else if (!string.IsNullOrWhiteSpace(TypeExt.ArtData.Cameo) && SHPCache.TryGet(TypeExt.ArtData.Cameo + ".shp", out var pCameo))
            {
                pSurface.Ref.DrawSHP(pCameo, 0, FileSystem.CAMEO_PAL, pos, BlitterFlags.bf_400);

            }
            else
            {
                return pos;
            }
            if (Num > 1)
            {
                //string str = Num.ToString();
                //var lenstr = Surface.GetTextDimension(str, 60);
                //Point2D textPos = new Point2D(pos.X + 60 - lenstr - 3, pos.Y);
                //RectangleStruct rect = new RectangleStruct(pos.X + 60 - lenstr - 6, pos.Y, lenstr + 6, 17);
                //pSurface.Ref.FillRect(rect, 0);
                //pSurface.Ref.DrawText(str, Pointer<Point2D>.AsPointer(ref textPos), Drawing.TooltipColor);

            }

            return new Point2D(pos.X + 60, pos.Y);
        }
    }

}

