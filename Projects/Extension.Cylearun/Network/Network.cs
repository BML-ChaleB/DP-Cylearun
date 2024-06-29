using System;
using PatcherYRpp;
using Extension.Ext;
using DynamicPatcher;
using System.Collections.Generic;
using uint8_t = System.Byte;

namespace Extension.CY.Network
{
    public static class EventClassExt
    {
        public enum Events : byte
        {
            Test = 0x92
        };


        public static bool EventLength(byte Events, out uint Length)
        {
            if (Map.TryGetValue(Events, out IEventHandler Handler))
            {
                Length = Handler.Lenth;
                return true;
            }
            Length = 0;
            return false;
        }
        //public static bool SoonEvent(this Events Events) => Events <= Events.Last && Events >= Events.First;
        //public static bool SoonEvent(this byte Event) => (Events)Event <= Events.Last && (Events)Event >= Events.First;
        public static unsafe UInt32 EventClass_RespondToEvent(REGISTERS* R)
        {
            try
            {
                Pointer<EventClass> pEvent = (IntPtr)R->ECX;
                var eventType = (uint8_t)pEvent.Ref.Type;

                if (Map.TryGetValue(eventType, out IEventHandler Handler))
                {
                    Handler.Respond(pEvent);
                }

            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }

            return 0;


        }


        public static unsafe UInt32 sub_4C65E0_Log(REGISTERS* R)
        {
            if (Map.TryGetValue((uint8_t)R->EAX, out IEventHandler Handler))
            {
                R->ECX = (uint)(IntPtr)new AnsiString(Handler.Name);
                return 0x4C65F6;
            }

            return 0;
        }


        public static unsafe UInt32 sub_64BDD0_GetEventSize1(REGISTERS* R)
        {
            var nSize = (uint8_t)R->EDI;

            if (EventLength(nSize, out uint Length))
            {
                R->ECX = Length;
                R->EBP = Length;
                R->Stack(0x20, Length);

                return 0x64BE97;
            }

            return 0;
        }


        public static unsafe UInt32 sub_64BDD0_GetEventSize2(REGISTERS* R)
        {
            var nSize = (uint8_t)R->ESI;

            if (EventLength(nSize, out uint Length))
            {
                R->ECX = Length;
                R->EBP = Length;
                return 0x64C321;
            }
            return 0;
        }


        public static unsafe UInt32 sub_64B660_GetEventSize(REGISTERS* R)
        {
            var nSize = (uint8_t)R->EDI;

            if (EventLength(nSize, out uint Length))
            {
                R->EDX = Length;
                R->EBP = Length;
                return 0x64B71D;
            }

            return 0;
        }

        public static Dictionary<byte, IEventHandler> Map = Register();

        public static void Register<T>() where T : IEventHandler, new()
        {

            T t = new T();
            Map.Add(t.Index, t);

        }
        public static void Register(IEventHandler Handler)
        {

            Map.Add(Handler.Index, Handler);
        }

        public static Dictionary<byte, IEventHandler> Register()
        {
            Dictionary<byte, IEventHandler> Map = new Dictionary<byte, IEventHandler>();
            return Map;
        }
    }
}
