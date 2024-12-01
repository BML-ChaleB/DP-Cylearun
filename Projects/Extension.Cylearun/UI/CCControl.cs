using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Cylearun.UI
{
    public abstract class CCControlBase
    {
        private RectangleStruct m_rectangle;
        private bool m_visible;
        private bool m_init;

        public RectangleStruct Rectangle
        {
            get => m_rectangle;
            set => m_rectangle = value;
        }

        public Point2D Location
        {
            get => m_rectangle.Location;
            set => m_rectangle.Location = value;
        }

        public Point2D Size
        {
            get => m_rectangle.Size;
            set => m_rectangle.Size = value;
        }

        public bool Visible
        {
            get => m_visible;
            set => m_visible = value;
        }

        public void Update()
        {
            if (!m_init)
            {
                OnLoad();
                m_init = true;
            }

            OnUpdate();
        }
        public void Render(Pointer<Surface> pSurface)
        {
            OnRender(pSurface);
        }
        protected virtual void OnLoad() { }
        protected virtual void OnUpdate() { }
        protected virtual void OnRender(Pointer<Surface> pSurface)
        {
            if (m_visible)
            {
                RectangleStruct rect = RectangleStruct.FromTopLeft(this.m_rectangle.Location, this.m_rectangle.Size);
                pSurface.Ref.FillRect(rect, 0);
                pSurface.Ref.DrawRect(rect, Drawing.Color16bit(Drawing.TooltipColor));
            }
        }
    }

    public abstract class CCControl : CCControlBase
    {
        public virtual bool IsMouseHoving(Point2D mousePos) { return Rectangle.InRect(mousePos); }
        public virtual void OnLeftRelease(Point2D mousePos) { }
    }
}
