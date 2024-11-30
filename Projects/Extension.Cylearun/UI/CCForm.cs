using PatcherYRpp;
using PatcherYRpp.FileFormats;
using System;
using System.Collections.Generic;
using System.Linq;
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

}
