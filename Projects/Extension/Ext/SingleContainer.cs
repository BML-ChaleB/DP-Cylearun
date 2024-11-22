using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicPatcher;

namespace Extension.Ext
{
    public class SingleContainer<TExt, TBase> : Container<TExt, TBase> where TExt : Extension<TBase>
    {
        Pointer<TBase> m_base;
        TExt m_instance;


        ExtensionFactory<TExt, TBase> m_Factory;

        public TExt Instance => m_instance;

        public SingleContainer(string name, ExtensionFactory<TExt, TBase> factory = null) : base(name)
        {
            m_Factory = factory ?? new LambdaExtensionFactory<TExt, TBase>();
        }

        public override TExt Find(Pointer<TBase> key)
        {
            if(m_base == key)
            {
                return m_instance;
            }
            return null;
        }

        protected override TExt Allocate(Pointer<TBase> key)
        {
            m_base = key;
            return m_instance = m_Factory.Create(key);
        }

        protected override void SetItem(Pointer<TBase> key, TExt ext)
        {
            m_base = key;
            m_instance = ext;
        }

        public override void RemoveItem(Pointer<TBase> key)
        {
            if (m_base == key)
            {
                m_base = default;
                m_instance = null;
            }
        }

        public override void Clear()
        {
            if (m_base != default)
            {
                Logger.Log($"Cleared {Name}.\n");
                m_base = default;
                m_instance = null;
            }
        }

    }


}
