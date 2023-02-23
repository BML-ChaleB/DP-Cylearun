using Extension.INI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.CY
{
    public partial class TechnoGlobalTypeExt
    {
        [INIField(Key = "Render.ForceLayer")]
        public string RenderLayer = "";
    }
}
