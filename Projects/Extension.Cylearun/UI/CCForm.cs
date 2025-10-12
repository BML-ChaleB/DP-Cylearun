using PatcherYRpp;
using PatcherYRpp.FileFormats;
using PatcherYRpp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Cylearun.UI
{
    public class CCForm
    {
        internal static CurrentObjectInfoControl CurrentObjectInfoControl;


        static CCForm()
        {
            CurrentObjectInfoControl = new CurrentObjectInfoControl() { Visible = true };
        }

        public static void Process()
        {
            CurrentObjectInfoControl.Update();

            CurrentObjectInfoControl.Render(Surface.Composite);
        }

        public static bool UpdateCursor()
        {
            Point2D mousePos = GetMouseCoords();
            if (CurrentObjectInfoControl.IsMouseHoving(mousePos))
            {
                return true;
            }
            return false;
        }

        public static void LeftRelease()
        {
            Point2D mousePos = GetMouseCoords();
            CurrentObjectInfoControl.OnLeftRelease(mousePos);
        }


        public static unsafe Point2D GetMouseCoords()
        {
            ref IntPtr pThis = ref new Pointer<Pointer<IntPtr>>(0x887640).Ref.Ref;
            Point2D pBuffer = default;
            var func = (delegate* unmanaged[Thiscall]<ref IntPtr, IntPtr, IntPtr>)pThis.GetVirtualFunctionPointer(13);
            func(ref pThis, Pointer<Point2D>.AsPointer(ref pBuffer));
            return pBuffer;
        }

    }

    public static class InputManager
    {
        private static IntPtr instance = new IntPtr(0x87F770);

        public static unsafe bool IsKeyPressed(int key)
        {
            var func = (delegate* unmanaged[Thiscall]<IntPtr, int, bool>)0x54F5C0;
            return func(instance, key);
        }

        public static bool IsForceFireKeyPressed()
        {
            return IsKeyPressed(GameOptionsClass.Instance.KeyForceFire1)
                || IsKeyPressed(GameOptionsClass.Instance.KeyForceFire2);
        }

        public static bool IsForceMoveKeyPressed()
        {
            return IsKeyPressed(GameOptionsClass.Instance.KeyForceMove1)
                || IsKeyPressed(GameOptionsClass.Instance.KeyForceMove2);
        }

        public static bool IsForceSelectKeyPressed()
        {
            return IsKeyPressed(GameOptionsClass.Instance.KeyForceSelect1)
                || IsKeyPressed(GameOptionsClass.Instance.KeyForceSelect2);
        }
    }


    public static class SHPCache
    {
        public static Dictionary<string, Pointer<SHPStruct>> _shps = new Dictionary<string, Pointer<SHPStruct>>();

        private static bool TryLoad(string name, out Pointer<SHPStruct> shp)
        {
            shp = FileSystem.LoadSHPFile(name);
            return shp.IsNull == false;
        }
        public static bool TryGet(string name, out Pointer<SHPStruct> shp)
        {
            if (_shps.TryGetValue(name, out shp))
            {
                return true;
            }

            lock (_shps)
            {
                if (TryLoad(name, out shp))
                {
                    _shps[name] = shp;
                    return true;
                }
            }

            shp = Pointer<SHPStruct>.Zero;
            return false;
        }
    }

    public static class Drawer
    {
        public static void FillRectBevel(Pointer<Surface> pSurface, RectangleStruct rect, ColorStruct color
            , bool topLeft = true
            , bool topRight = true
            , bool buttomLeft = true
            , bool buttomRight = true
            )
        {

            rect = Drawing.Intersect(pSurface.Ref.GetRect(), rect);
            int pitch = pSurface.Ref.GetPitch();
            ushort color16bit = (ushort)Drawing.RGB2DWORD(color);
            Pointer<byte> ptr = pSurface.Ref.Lock(rect.Location);

            Pointer<byte> p = ptr;

            if (topLeft || topRight)
            {
                Pointer<ushort> b = (IntPtr)p;
                b += topLeft ? 4 : 0;
                for (int j = topLeft ? 4 : 0; j < rect.Size.X - (topRight ? 4 : 0); j++)
                {
                    b.Ref = color16bit;
                    b += 1;
                }
                p += pitch;

                b = (IntPtr)p;
                b += topLeft ? 2 : 0;
                for (int j = topLeft ? 2 : 0; j < rect.Size.X - (topRight ? 2 : 0); j++)
                {
                    b.Ref = color16bit;
                    b += 1;
                }
                p += pitch;

                b = (IntPtr)p;
                b += topLeft ? 1 : 0;
                for (int j = topLeft ? 1 : 0; j < rect.Size.X - (topRight ? 1 : 0); j++)
                {
                    b.Ref = color16bit;
                    b += 1;
                }
                p += pitch;

                b = (IntPtr)p;
                b += topLeft ? 1 : 0;
                for (int j = topLeft ? 1 : 0; j < rect.Size.X - (topRight ? 1 : 0); j++)
                {
                    b.Ref = color16bit;
                    b += 1;
                }
                p += pitch;

            }


            for (int i = 0; i < rect.Size.Y - ((topLeft || topRight ? 4 : 0) + (buttomLeft || buttomRight ? 4 : 0)); i++)
            {
                Pointer<ushort> b = (IntPtr)p;

                for (int j = 0; j < rect.Size.X; j++)
                {
                    b.Ref = color16bit;
                    b += 1;
                }
                p += pitch;
            }

            if (buttomLeft || buttomRight)
            {
                Pointer<ushort> b = (IntPtr)p;
                b += buttomLeft ? 1 : 0;
                for (int j = buttomLeft ? 1 : 0; j < rect.Size.X - (buttomRight ? 1 : 0); j++)
                {
                    b.Ref = color16bit;
                    b += 1;
                }
                p += pitch;

                b = (IntPtr)p;
                b += buttomLeft ? 1 : 0;
                for (int j = buttomLeft ? 1 : 0; j < rect.Size.X - (buttomRight ? 1 : 0); j++)
                {
                    b.Ref = color16bit;
                    b += 1;
                }
                p += pitch;

                b = (IntPtr)p;
                b += buttomLeft ? 2 : 0;
                for (int j = buttomLeft ? 2 : 0; j < rect.Size.X - (buttomRight ? 2 : 0); j++)
                {
                    b.Ref = color16bit;
                    b += 1;
                }
                p += pitch;

                b = (IntPtr)p;
                b += buttomLeft ? 4 : 0;
                for (int j = buttomLeft ? 4 : 0; j < rect.Size.X - (buttomRight ? 4 : 0); j++)
                {
                    b.Ref = color16bit;
                    b += 1;
                }
                p += pitch;
            }
            pSurface.Ref.Unlock();

        }

    }

}
