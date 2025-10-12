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

        private void RenderEmpty(Pointer<Surface> pSurface)
        {
            Rectangle = RectangleStruct.FromBottomLeft(Rectangle.BottomLeft, (100, 100));
            if (!PCXPointer.BlitCameo("lightconepcx.pcx", Location, pSurface))
            {
                Rectangle = RectangleStruct.FromBottomLeft(Rectangle.BottomLeft, (0, 0));
            }
        }

        protected override void OnRender(Pointer<Surface> pSurface)
        {
            if (ObjectClass.CurrentObjects.Count == 0)
            {
                RenderEmpty(pSurface);
                return;
            }


            Point2D pos = Location;



            int maxCount = Surface.ViewBound.Width / 60 - 3;
            Point2D mousePos = CCForm.GetMouseCoords();

            List<TypeValuePair> list = Get();
            if (ObjectClass.CurrentObjects.Count == 1)
            {
                Rectangle = RectangleStruct.FromBottomLeft(Rectangle.BottomLeft, (220, 100));
                Drawer.FillRectBevel(pSurface, Rectangle, (31, 31, 31));
                Drawer.FillRectBevel(pSurface, (Location.X, Location.Y, Rectangle.Width, 5), (61, 61, 61), buttomLeft: false, buttomRight: false);

                int health = ObjectClass.CurrentObjects[0].Ref.Health;
                int strength = ObjectClass.CurrentObjects[0].Ref.GetObjectType().Ref.Strength;

                pSurface.Ref.DrawText($"{health} / {strength}", Location.X + 10, Location.Y + 70, (59, 117, 75));
                pSurface.Ref.FillRect((Location.X + 10, Location.Y + 90, 200, 5), Drawing.Color16bit((61, 61, 61)));
                pSurface.Ref.FillRect((Location.X + 10, Location.Y + 90, (int)(health * 200 / strength), 5)
                    , Drawing.Color16bit((59, 117, 75)));


                RectangleStruct rect1 = (Location.X + 85, Location.Y + 15, 60, 48);
                RectangleStruct rect2 = (Location.X + 150, Location.Y + 15, 60, 48);

                Drawer.FillRectBevel(pSurface, rect1, rect1.InRect(mousePos) ? (100, 100, 100) : (61, 61, 61));
                Drawer.FillRectBevel(pSurface, rect2, rect2.InRect(mousePos) ? (100, 100, 100) : (61, 61, 61));

                if (list.Count == 1)
                {
                    list[0].Render(pSurface, Location + (10, 15));
                }
                else
                {
                    Drawer.FillRectBevel(pSurface, (Location.X + 10, Location.Y + 15, 60, 48), (61, 61, 61));
                    pSurface.Ref.DrawText("黑暗22", Location.X + 40, Location.Y + 32, (161, 161, 161), TextPrintType.NoShadow | TextPrintType.Center);
                }
                return;
            }

            if (list.Count == 0)
            {
                RenderEmpty(pSurface);
                return;
            }


            Rectangle = RectangleStruct.FromBottomLeft(Rectangle.BottomLeft, (list.Count * 60, 48));


            int i = 0;
            foreach (var p in list)
            {
                if (i > maxCount)
                {
                    i++;
                    break;
                }
                pos = p.Render(pSurface, pos);

            }


            if (IsMouseHoving(mousePos))
            {
                int index = (mousePos.X - 1) / 60;
                pSurface.Ref.DrawRect((index * 60 + 1, Location.Y, 60, 48), Drawing.Color16bit(Drawing.TooltipColor));
            }
        }

        public override void OnLeftRelease(Point2D mousePos)
        {
            if (ObjectClass.CurrentObjects.Count == 0)
            {
                return;
            }

            List<TypeValuePair> list = Get();
            if (list.Count == 0)
            {
                return;
            }

            if (list.Count == 1 && ObjectClass.CurrentObjects.Count == 1)
            {
                return;

            }


            Rectangle = RectangleStruct.FromBottomLeft(Rectangle.BottomLeft, (list.Count * 60, 48));

            bool ctrl = InputManager.IsForceFireKeyPressed();

            if (IsMouseHoving(mousePos))
            {
                int index = (mousePos.X - 1) / 60;

                var array = ObjectClass.CurrentObjects.Where(o =>
                {

                    if (o.IsNotNull && o.CastToTechno(out Pointer<TechnoClass> pTechno))
                    {
                        bool sth = pTechno.Ref.Type == list[index].GetTechnoType();

                        return ctrl ? !sth : sth;
                    }
                    return false;
                }).Select(o => o.Convert<TechnoClass>()).ToArray();

                MapClass.Instance.SetTogglePowerMode(0);
                MapClass.Instance.SetWaypointMode(0);
                MapClass.Instance.SetRepairMode(0);
                MapClass.Instance.SetSellMode(0);

                MapClass.UnselectAll();

                foreach (var item in array)
                {
                    item.Ref.Base.Select();
                }
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
                string str = Num.ToString();
                var lenstr = Surface.GetTextDimension(str, 60);
                Point2D textPos = new Point2D(pos.X + 60 - lenstr - 3, pos.Y);
                RectangleStruct rect = new RectangleStruct(pos.X + 60 - lenstr - 6, pos.Y, lenstr + 6, 17);
                pSurface.Ref.FillRect(rect, 0);
                pSurface.Ref.DrawText(str, Pointer<Point2D>.AsPointer(ref textPos), Drawing.TooltipColor);

            }

            return new Point2D(pos.X + 60, pos.Y);
        }
    }

}

