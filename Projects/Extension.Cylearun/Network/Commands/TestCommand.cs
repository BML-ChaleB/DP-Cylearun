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

namespace Extension.CY.Network
{

    [Serializable]
    public class TestCommand : NewDummyCommand, IEventHandler
    {
        public override string Name() => "TestCommand";
        public override string UIName() => "TestCommand";
        public override string UIDescription() => "TestCommand.";
        public override void ExecuteProxy(IntPtr pThis, WWKey input)
        {
            //do sthing
        }


        byte IEventHandler.Index { get => 0x92; }

        uint IEventHandler.Lenth { get => 17; }

        string IEventHandler.Name { get => "TestCommand"; }

        void IEventHandler.Respond(Pointer<EventClass> pEvent)
        {
            //do sthing
        }
    }

}