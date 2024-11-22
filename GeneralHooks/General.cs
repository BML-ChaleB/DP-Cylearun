using DynamicPatcher;
using PatcherYRpp;
using PatcherYRpp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extension.Components;
using Extension.EventSystems;
using Extension.CY.Network;

namespace GeneralHooks
{
    public class General
    {
        static General()
        {
            EventSystem.General.AddPermanentHandler(EventSystem.General.ScenarioStartEvent, MathExHandler);
        }

        private static void MathExHandler(object sender, EventArgs e)
        {
            // ensure network synchronization
            MathEx.SetRandomSeed(0);
            //Logger.Log("set random seed!");
        }

        [Hook(HookType.AresHook, Address = 0x52BA60, Size = 5)]
        public static unsafe UInt32 YR_Boot(REGISTERS* R)
        {

            return 0;
        }

        // in progress: Initializing Tactical display
        [Hook(HookType.AresHook, Address = 0x6875F3, Size = 6)]
        public static unsafe UInt32 Scenario_Start1(REGISTERS* R)
        {
            EventSystem.General.Broadcast(EventSystem.General.ScenarioStartEvent, EventArgs.Empty);

            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x55AFB3, Size = 6)]
        public static unsafe UInt32 LogicClass_Update(REGISTERS* R)
        {
            EventSystem.General.Broadcast(EventSystem.General.LogicClassUpdateEvent, new LogicClassUpdateEventArgs(true));

            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x55B719, Size = 5)]
        public static unsafe UInt32 LogicClass_Update_Late(REGISTERS* R)
        {
            EventSystem.General.Broadcast(EventSystem.General.LogicClassUpdateEvent, new LogicClassUpdateEventArgs(false));

            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x685659, Size = 0xA)]
        public static unsafe UInt32 Scenario_ClearClasses(REGISTERS* R)
        {
            EventSystem.General.Broadcast(EventSystem.General.ScenarioClearClassesEvent, EventArgs.Empty);

            return 0;
        }


        [Hook(HookType.AresHook, Address = 0x7CD8EF, Size = 9)]
        public static unsafe UInt32 ExeTerminate(REGISTERS* R)
        {

            return 0;
        }

        [Hook(HookType.AresHook, Address = 0x533066, Size = 6)]
        public static unsafe UInt32 CommandClassCallback_Register(REGISTERS* R)
        {
            Command.Register();
            return 0;
        }

        static readonly ColorStruct colorFill = new(0, 60, 176);
        static readonly ColorStruct colorOutLine = new(64, 92, 240);
        static readonly int trans = 30;

        [Hook(HookType.AresHook, Address = 0x6DA217, Size = 7)]
        public static unsafe UInt32 Box_Selection_FillTransb(REGISTERS* R)
        {
            static void fillTrans(Pointer<Surface> pSurface, RectangleStruct rect, ColorStruct color, int trans)
            {
                var func = (delegate* unmanaged[Thiscall]<ref Surface, ref RectangleStruct, ref ColorStruct, int, Bool>)((Pointer<Pointer<IntPtr>>)(IntPtr)pSurface)[0][7];
                func(ref pSurface.Ref, ref rect, ref color, trans);
            }

            Pointer<RectangleStruct> pRect = R->lea_Stack<IntPtr>(28 + -16);

            fillTrans(Surface.Current, pRect.Data, colorFill, trans);

            Surface.Current.Ref.DrawRectEx(Surface.ViewBound, pRect.Ref, Drawing.Color16bit(colorOutLine));
            pRect.Ref.Width = 0;
            pRect.Ref.Height = 0;
            pRect.Ref.X = 0;
            pRect.Ref.Y = 0;
            return 0;
        }
    }
}
