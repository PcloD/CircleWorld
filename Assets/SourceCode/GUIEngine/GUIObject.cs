using UnityEngine;
using SpriteMeshEngine;

namespace GUIEngine
{
    public class GUIObject : MonoBehaviour
    {
        public GUIAlignHorizontal horizontalAlignment;
        public GUIAlignVertical verticalAlignment;

        [HideInInspector]
        [System.NonSerialized]
        public bool initialized;

        protected GUIPanel panel;
        protected Transform trans;

        protected Rect bounds;
        protected float depth;

        public void SetDirty()
        {
            if (panel != null)
                panel.SetDirty();
        }

        public Vector2 Position
        {
            get 
            { 
                return new Vector2(bounds.x, bounds.y);
            }

            set
            {
                trans.position = new Vector3(value.x, value.y, depth) - panel.position;

                SetDirty();
            }
        }

        public float Depth
        {
            get 
            { 
                return depth; 
            }

            set
            {
                trans.position = new Vector3(trans.position.x, trans.position.y, value);

                SetDirty();
            }
        }

        public void Init(GUIPanel panel)
        {
            this.panel = panel;

            trans = transform;

            initialized = true;

            OnInit();
        }

        public void Layout()
        {
            Vector2 size = OnGetSize();
            Vector2 position = OnGetPosition(size);

            depth = OnGetDepth();

            bounds = new Rect(position.x, position.y, size.x, size.y);

        }

        public void Draw(SpriteMesh spriteMesh)
        {
            Vector3 position = Position;

            spriteMesh.SetZ(depth);

            OnDraw(spriteMesh, position.x, position.y);
        }

        protected virtual void OnInit()
        {

        }

        protected virtual Vector2 OnGetSize()
        {
            return Vector2.zero;
        }

        protected virtual Vector2 OnGetPosition(Vector2 size)
        {
            Vector3 delta = trans.position - panel.position;

            Vector2 position = new Vector2(delta.x, -delta.y);

            switch (horizontalAlignment)
            {
                case GUIAlignHorizontal.Left:
                    //The default position is left
                    break;

                case GUIAlignHorizontal.Center:
                    position.x -= size.x * 0.5f;
                    break;

                case GUIAlignHorizontal.Right:
                    position.x -= size.x;
                    break;
            }

            switch (verticalAlignment)
            {
                case GUIAlignVertical.Top:
                    //The default position is top
                    break;

                case GUIAlignVertical.Center:
                    position.y -= size.y * 0.5f;
                    break;

                case GUIAlignVertical.Bottom:
                    position.y -= size.y;
                    break;
            }

            return position;
        }

        protected virtual float OnGetDepth()
        {
            return (trans.position - panel.position).z;
        }


        protected virtual void OnDraw(SpriteMesh spriteMesh, float x, float y)
        {

        }

        
        public virtual void OnDrawGizmosSelected()
        {
            OnDrawGizmos();
        }

        public virtual void OnDrawGizmos()
        {
            if (panel == null)
                return;

            Gizmos.color = new Color(255, 255, 255, 1);
            Gizmos.DrawCube(new Vector3(bounds.center.x + panel.position.x, -bounds.center.y + panel.position.y, depth + 4 + panel.position.z), new Vector3(bounds.width, bounds.height, 1));
        }

    }
}

