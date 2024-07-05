using System.Data.Common;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using DynamicPatcher;
using PatcherYRpp;
using Extension.Ext;
using Extension.INI;
using Extension.Script;
using Extension.Utilities;
using Extension.CY.Network;

namespace Extension.CY.Network
{

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public delegate void DTORFunc(IntPtr pThis);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public delegate AnsiStringPointer GetNameFunc(IntPtr pThis);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public delegate UniStringPointer GetUINameFunc(IntPtr pThis);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public delegate Bool CheckInputFunc(IntPtr pThis, NewDummyCommand.WWKey input);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public delegate void ExecuteFunc(IntPtr pThis, NewDummyCommand.WWKey input);

    [Serializable]
    public class NewDummyCommand
    {
        public static List<NewDummyCommand> Array = new List<NewDummyCommand>();

        public static UniString CATEGORY_CY = new UniString("Cylearun");

        public static readonly IntPtr ArrayPointer = new IntPtr(0x87F658);

        public static YRPP.GLOBAL_DVC_ARRAY<Pointer<Command>> ABSTRACTTYPE_ARRAY = new YRPP.GLOBAL_DVC_ARRAY<Pointer<Command>>(ArrayPointer);

        [Flags]
        public enum WWKey
        {
            Shift = 0x100,
            Ctrl = 0x200,
            Alt = 0x400,
            Release = 0x800,
            VirtualKey = 0x1000,
            DoubleClick = 0x2000,
            Button = 0x8000
        }


        public NewDummyCommand()
        {
            Execute = (IntPtr pThis, WWKey input) => ExecuteProxy(pThis, input);
            DTOR = (IntPtr pThis) => DTORProxy(pThis);

            GetName = (IntPtr pThis) => new AnsiString(Name());
            GetUIName = (IntPtr pThis) => new UniString(UIName());
            GetUICategory = (IntPtr pThis) => new UniString(UICategory());
            GetUIDescription = (IntPtr pThis) => new UniString(UIDescription());

            PreventCombinationOverride = (IntPtr pThis, WWKey input) => PreventCombinationOverrideProxy(pThis, input);
            ExtraTriggerCondition = (IntPtr pThis, WWKey input) => ExtraTriggerConditionProxy(pThis, input);
            CheckLoop55E020 = (IntPtr pThis, WWKey input) => CheckLoop55E020Proxy(pThis, input);

            Array.Add(this);
        }

        public GetNameFunc GetName;
        public virtual string Name() => "";


        public GetUINameFunc GetUIName;
        public virtual string UIName() => "";


        public GetUINameFunc GetUICategory;
        public virtual string UICategory() => NewDummyCommand.CATEGORY_CY;


        public GetUINameFunc GetUIDescription;
        public virtual string UIDescription() => "";


        public ExecuteFunc Execute;
        public virtual void ExecuteProxy(IntPtr pThis, WWKey input) => Logger.Log($"{Game.CurrentFrame} Dummy ExecuteProxy {input}");


        public DTORFunc DTOR;
        public virtual void DTORProxy(IntPtr pThis) { }


        public CheckInputFunc PreventCombinationOverride;
        public virtual Bool PreventCombinationOverrideProxy(IntPtr pThis, WWKey input) => false;


        public CheckInputFunc ExtraTriggerCondition;
        public virtual Bool ExtraTriggerConditionProxy(IntPtr pThis, WWKey input) => !Convert.ToBoolean(input & WWKey.Release);


        public CheckInputFunc CheckLoop55E020;
        public virtual Bool CheckLoop55E020Proxy(IntPtr pThis, WWKey input) => false;
    }

    [Serializable]
    public class DummyNetWork : NewDummyCommand, IEventHandler
    {
        byte IEventHandler.Index => Index;

        uint IEventHandler.Lenth => Lenth;

        string IEventHandler.Name => EventName;

        void IEventHandler.Respond(Pointer<EventClass> pEvent) => Respond(pEvent);

        public virtual byte Index => 0;
        public virtual uint Lenth => 0;
        public virtual string EventName => "";
        public virtual void Respond(Pointer<EventClass> pEvent) { }

    }

    [Serializable]
    public class DummyNetWork<T> : DummyNetWork
    {
        public sealed override void Respond(Pointer<EventClass> pEvent)
        {
            OnRespond(pEvent, pEvent.GetHandlers<T>());
        }
        public virtual void OnRespond(Pointer<EventClass> pEvent, Pointer<T> pData) { }
    }

    [Serializable]
    public class EventHandler<T> : IEventHandler
    {
        byte IEventHandler.Index => Index;

        uint IEventHandler.Lenth => Lenth;

        string IEventHandler.Name => EventName;

        void IEventHandler.Respond(Pointer<EventClass> pEvent)
        {
            OnRespond(pEvent, pEvent.GetHandlers<T>());
        }

        public virtual byte Index => 0;
        public virtual uint Lenth => 0;
        public virtual string EventName => "";
        public virtual void OnRespond(Pointer<EventClass> pEvent, Pointer<T> pData) { }

    }

    [StructLayout(LayoutKind.Sequential, Size = 36)]
    public struct Command
    {

        public IntPtr VTableDTOR;
        public IntPtr VTableGetName;
        public IntPtr VTableGetUIName;
        public IntPtr VTableGetUICategory;
        public IntPtr VTableGetUIDescription;
        public IntPtr VTablePreventCombinationOverride;
        public IntPtr VTableExtraTriggerCondition;
        public IntPtr VTableCheckLoop55E020;
        public IntPtr VTableExecute;

        public static unsafe void Register<TCommand>() where TCommand : NewDummyCommand, new()
        {
            Pointer<Pointer<Command>> pVfptr = YRMemory.Allocate(4);

            Pointer<Command> VTable = Marshal.AllocHGlobal(sizeof(IntPtr) * 9);

            TCommand command = new TCommand();

            if (command is IEventHandler Handler) EventClassExt.Register(Handler);


            VTable.Ref.VTableDTOR = Marshal.GetFunctionPointerForDelegate(command.DTOR);
            VTable.Ref.VTableGetName = Marshal.GetFunctionPointerForDelegate(command.GetName);
            VTable.Ref.VTableGetUIName = Marshal.GetFunctionPointerForDelegate(command.GetUIName);
            VTable.Ref.VTableGetUICategory = Marshal.GetFunctionPointerForDelegate(command.GetUICategory);
            VTable.Ref.VTableGetUIDescription = Marshal.GetFunctionPointerForDelegate(command.GetUIDescription);
            VTable.Ref.VTablePreventCombinationOverride = Marshal.GetFunctionPointerForDelegate(command.PreventCombinationOverride);
            VTable.Ref.VTableExtraTriggerCondition = Marshal.GetFunctionPointerForDelegate(command.ExtraTriggerCondition);
            VTable.Ref.VTableCheckLoop55E020 = Marshal.GetFunctionPointerForDelegate(command.CheckLoop55E020);
            VTable.Ref.VTableExecute = Marshal.GetFunctionPointerForDelegate(command.Execute);

            pVfptr.Data = VTable;
            NewDummyCommand.ABSTRACTTYPE_ARRAY.Array.AddItem(pVfptr);
        }

        public static void Register()
        {
            Command.Register<TestCommand>();

        }
    }
}
