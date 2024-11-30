using DynamicPatcher;
using Extension.Cylearun.UI;
using Extension.Ext;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.CY
{
    public partial class TechnoGlobalExtension
    {
        public unsafe void DrawFactoryProcess(REGISTERS* R)
        {

            AbstractType factoryType = Owner.BuildingRef.Type.Ref.Factory;
            bool isObserver = HouseClass.Player == HouseClass.Observer;
            bool isPlayer = HouseClass.Player == Owner.pHouse;
            bool isAlliedWith = Owner.HouseRef.IsAlliedWith(HouseClass.Player);
            bool isControlledByHuman = Owner.HouseRef.ControlledByHuman();



            if (AbstractType.None == factoryType)
            {
                return;
            }
            if (isAlliedWith)
            {
                if (!isPlayer && !(Owner.Select || Owner.OwnerRef.IsMouseHovering))
                {
                    return;
                }
            }
            else
            {
                if (!isObserver)
                {
                    return;
                }
            }


            Pointer<FactoryClass> pFactory = Owner.BuildingRef.Factory;

            if (isControlledByHuman)
            {
                if (!Owner.OwnerRef.IsPrimaryFactory)
                {
                    return;
                }
                pFactory = GetPrimayFactory(Owner.pHouse, Owner.BuildingRef.Type.Ref.Factory, Owner.BuildingRef.Type.Ref.Base.Naval, BuildCat.DontCare);
            }
            bool hasFactory = pFactory.IsNotNull;
            bool factoryInProgress = hasFactory && pFactory.Ref.Object.IsNotNull;

            Pointer<FactoryClass> pSecondFactory = IntPtr.Zero;
            bool hasSecondFactory = false;
            bool secondFactoryInProgress = false;

            if (isControlledByHuman && AbstractType.BuildingType == factoryType)
            {
                pSecondFactory = GetPrimayFactory(Owner.pHouse, factoryType, Owner.BuildingRef.Type.Ref.Base.Naval, BuildCat.Combat);
                hasSecondFactory = pSecondFactory.IsNotNull;
                secondFactoryInProgress = hasSecondFactory && pSecondFactory.Ref.Object.IsNotNull;
            }

            if(!factoryInProgress && !secondFactoryInProgress)
            {
                return;
            }



            Point2D centerlocation = TacticalClass.Instance.Ref.CoordsToClient(Owner.Pos);
            Point2D location = R->Stack<Pointer<Point2D>>(4).Ref + (0, 49);// TacticalClass.Instance.Ref.CoordsToClient(Owner.BaseObject.Location) + (0, 20);
            CoordStruct coord = Owner.OwnerRef.Type.Ref.Base.Dimension2();
            int l = coord.Y / 256;
            int length = l / 2 * 15 + (l & 1) * 7;

            coord = (-coord.X / 2, -coord.Y / 2, coord.Z);
            Point2D pos = TacticalClass.CoordsToScreen(coord) + (3, 4) + centerlocation;

            if (secondFactoryInProgress)
            {
                Point2D position = pos - (12, 6);
                int totalLength = (int)Math.Round(pSecondFactory.Ref.Production.Value / 54.0 * length);
                for (int index = totalLength; index > 0; (index, position.X, position.Y) = (index - 1, position.X - 4, position.Y + 2))
                {
                    Surface.Current.Ref.DrawSHP(FileSystem.PIPS_SHP, 5, FileSystem.PALETTE_PAL, position, (BlitterFlags)0x600);
                }
                for (int index = length - totalLength; index > 0; (index, position.X, position.Y) = (index - 1, position.X - 4, position.Y + 2))
                {
                    Surface.Current.Ref.DrawSHP(FileSystem.PIPS_SHP, 0, FileSystem.PALETTE_PAL, position, (BlitterFlags)0x600);
                }

                if (!isObserver)
                {
                    TechnoTypeExt TypeExt = TechnoTypeExt.ExtMap.Find(pSecondFactory.Ref.Object.Ref.Type);
                    if (!string.IsNullOrWhiteSpace(TypeExt.ArtData.CameoPCX))
                    {
                        PCXPointer.BlitCameo(TypeExt.ArtData.CameoPCX.ToLower(), factoryInProgress ? location - (30, 24) + (30, 0) : location - (30, 24), Surface.Current);
                    }
                    else if (!string.IsNullOrWhiteSpace(TypeExt.ArtData.Cameo) && SHPCache.TryGet(TypeExt.ArtData.Cameo + ".shp", out var pCameo))
                    {
                        Surface.Current.Ref.DrawSHP(pCameo, 0, FileSystem.CAMEO_PAL, factoryInProgress ? location + (30, 0) : location, BlitterFlags.bf_400);
                    }
                }
            }

            if (factoryInProgress)
            {
                Point2D position = pos - (6, 3);
                //Console.WriteLine(pFactory);
                int totalLength = (int)Math.Round(pFactory.Ref.Production.Value / 54.0 * length);
                for (int index = totalLength; index > 0; (index, position.X, position.Y) = (index - 1, position.X - 4, position.Y + 2))
                {
                    Surface.Current.Ref.DrawSHP(FileSystem.PIPS_SHP, 5, FileSystem.PALETTE_PAL, position, (BlitterFlags)0x600);
                }
                for (int index = length - totalLength; index > 0; (index, position.X, position.Y) = (index - 1, position.X - 4, position.Y + 2))
                {
                    Surface.Current.Ref.DrawSHP(FileSystem.PIPS_SHP, 0, FileSystem.PALETTE_PAL, position, (BlitterFlags)0x600);
                }

                if (!isObserver)
                {
                    TechnoTypeExt TypeExt = TechnoTypeExt.ExtMap.Find(pFactory.Ref.Object.Ref.Type);

                    if (!string.IsNullOrWhiteSpace(TypeExt.ArtData.CameoPCX))
                    {
                        PCXPointer.BlitCameo(TypeExt.ArtData.CameoPCX.ToLower(), secondFactoryInProgress ? location - (60, 24) : location - (30, 24), Surface.Current);
                    }
                    else if (!string.IsNullOrWhiteSpace(TypeExt.ArtData.Cameo) && SHPCache.TryGet(TypeExt.ArtData.Cameo + ".shp", out var pCameo))
                    {
                        Surface.Current.Ref.DrawSHP(pCameo, 0, FileSystem.CAMEO_PAL, secondFactoryInProgress ? location - (30, 0) : location, BlitterFlags.bf_400 | BlitterFlags.Centered);

                    }
                }

            }

            //bool hasSecond = false;

            //if (Owner.HouseRef.ControlledByHuman() && AbstractType.BuildingType == factoryType || AbstractType.Building == Owner.BuildingRef.Type.Ref.Factory)
            //{
            //    var combat = GetPrimayFactory(Owner.pHouse, factoryType, Owner.BuildingRef.Type.Ref.Base.Naval, BuildCat.Combat);
            //    Point2D position = pos - (12, 6);
            //    if (combat.IsNotNull)
            //    {
            //        int totalLength = (int)Math.Round(combat.Ref.Production.Value / 54.0 * length);
            //        for (int index = totalLength; index > 0; (index, position.X, position.Y) = (index - 1, position.X - 4, position.Y + 2))
            //        {
            //            Surface.Current.Ref.DrawSHP(FileSystem.PIPS_SHP, 5, FileSystem.PALETTE_PAL, position, (BlitterFlags)0x600);
            //        }
            //        for (int index = length - totalLength; index > 0; (index, position.X, position.Y) = (index - 1, position.X - 4, position.Y + 2))
            //        {
            //            Surface.Current.Ref.DrawSHP(FileSystem.PIPS_SHP, 0, FileSystem.PALETTE_PAL, position, (BlitterFlags)0x600);
            //        }

            //        if (!isObserver && combat.Ref.Object.IsNotNull)
            //        {
            //            bool hasPrimay = pFactory.IsNotNull && pFactory.Ref.Object.IsNotNull;

            //            TechnoTypeExt TypeExt = TechnoTypeExt.ExtMap.Find(combat.Ref.Object.Ref.Type);
            //            if (!string.IsNullOrWhiteSpace(TypeExt.ArtData.CameoPCX))
            //            {
            //                PCXPointer.BlitCameo(TypeExt.ArtData.CameoPCX.ToLower(), hasPrimay ? location - (60, 24) : location - (30, 24), Surface.Current);
            //                hasSecond = true;
            //            }
            //            else if (!string.IsNullOrWhiteSpace(TypeExt.ArtData.Cameo) && SHPCache.TryGet(TypeExt.ArtData.Cameo + ".shp", out var pCameo))
            //            {
            //                Surface.Current.Ref.DrawSHP(pCameo, 0, FileSystem.CAMEO_PAL, hasPrimay ? location - (30, 0) : location, BlitterFlags.bf_400);
            //                hasSecond = true;
            //            }
            //        }


            //    }
            //}
            //if (pFactory.IsNotNull)
            //{
            //    Point2D position = pos - (6, 3);
            //    //Console.WriteLine(pFactory);
            //    int totalLength = (int)Math.Round(pFactory.Ref.Production.Value / 54.0 * length);
            //    for (int index = totalLength; index > 0; (index, position.X, position.Y) = (index - 1, position.X - 4, position.Y + 2))
            //    {
            //        Surface.Current.Ref.DrawSHP(FileSystem.PIPS_SHP, 5, FileSystem.PALETTE_PAL, position, (BlitterFlags)0x600);
            //    }
            //    for (int index = length - totalLength; index > 0; (index, position.X, position.Y) = (index - 1, position.X - 4, position.Y + 2))
            //    {
            //        Surface.Current.Ref.DrawSHP(FileSystem.PIPS_SHP, 0, FileSystem.PALETTE_PAL, position, (BlitterFlags)0x600);
            //    }

            //    if (!isObserver && pFactory.Ref.Object.IsNotNull)
            //    {
            //        TechnoTypeExt TypeExt = TechnoTypeExt.ExtMap.Find(pFactory.Ref.Object.Ref.Type);



            //        if (!string.IsNullOrWhiteSpace(TypeExt.ArtData.CameoPCX))
            //        {
            //            PCXPointer.BlitCameo(TypeExt.ArtData.CameoPCX.ToLower(), hasSecond ? location - (30, 24) + (30, 0) : location - (30, 24), Surface.Current);
            //        }
            //        else if (!string.IsNullOrWhiteSpace(TypeExt.ArtData.Cameo) && SHPCache.TryGet(TypeExt.ArtData.Cameo + ".shp", out var pCameo))
            //        {
            //            Surface.Current.Ref.DrawSHP(pCameo, 0, FileSystem.CAMEO_PAL, hasSecond ? location + (30, 0) : location, BlitterFlags.bf_400 | BlitterFlags.Centered);

            //        }
            //    }

            //}

        }

        public static Pointer<FactoryClass> GetPrimayFactory(Pointer<HouseClass> pOwner, AbstractType abs, bool naval, BuildCat buildCat)
        {
            switch (abs)
            {
                case AbstractType.Unit:
                case AbstractType.UnitType:
                    return naval ? pOwner.Ref.PrimaryForShips : pOwner.Ref.PrimaryForVehicles;
                case AbstractType.Aircraft:
                case AbstractType.AircraftType:
                    return pOwner.Ref.PrimaryForAircraft;
                case AbstractType.Infantry:
                case AbstractType.InfantryType:
                    return pOwner.Ref.PrimaryForInfantry;
                case AbstractType.Building:
                case AbstractType.BuildingType:
                    return BuildCat.Combat == buildCat ? pOwner.Ref.PrimaryForDefenses : pOwner.Ref.PrimaryForBuildings;
                default:
                    return IntPtr.Zero;


            }
        }
    }
}
