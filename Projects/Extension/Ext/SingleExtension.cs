using Extension.Components;
using Extension.Decorators;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{
    [Serializable]
    public abstract class SingleExtension<TExt, TBase> : Extension<TBase> where TExt : Extension<TBase>
    {
        public static SingleContainer<TExt, TBase> ExtMap = new SingleContainer<TExt, TBase>(typeof(TBase).Name);
        public SingleExtension(Pointer<TBase> ownerObject) : base(ownerObject) 
        {
            
            m_GameObject = new GameObject(s_GameObjectName);

            m_GameObject.SetTransform(new YRTransform(this));
            m_GameObject.OnAwake += () =>
            {
                OnAwake(m_GameObject);
            };
        }

        public ref TBase OwnerRef => ref OwnerObject.Ref;

        private static string s_GameObjectName = $"{typeof(TExt).Name}'s GameObject";
        public GameObject GameObject => m_GameObject.GetAwaked();
        public DecoratorComponent DecoratorComponent
        {
            get
            {
                if (m_DecoratorComponent == null)
                {
                    m_DecoratorComponent = new DecoratorComponent();
                    m_DecoratorComponent.AttachToComponent(m_GameObject);
                }

                return m_DecoratorComponent;
            }
        }

        public override void SaveToStream(IStream stream)
        {
            base.SaveToStream(stream);

            m_GameObject.Foreach(c => c.SaveToStream(stream));
        }

        public override void LoadFromStream(IStream stream)
        {
            base.LoadFromStream(stream);

            m_GameObject.Foreach(c => c.LoadFromStream(stream));
        }

        public virtual void OnAwake(GameObject gameObject)
        {

        }

        public override void OnExpire()
        {
            GameObject.Destroy(m_GameObject);
        }

        private GameObject m_GameObject;
        private DecoratorComponent m_DecoratorComponent;

    }
}
