using UnityEngine;
using System.Collections.Generic;
using SpriteMeshEngine;

namespace GUIEngine
{
    [ExecuteInEditMode]
    public class GUIPanel : SpriteMeshView
    {
        public string spriteSheetId = "GUI";

        public float editorScreenWidth = 1024;
        public float editorScreenHeight = 768;

        [HideInInspector]
        [System.NonSerialized]
        public Transform trans;

        [HideInInspector]
        [System.NonSerialized]
        public Vector3 position;

        private List<GUIObject> childrens = new List<GUIObject>();

        public override void Start()
        {
            UpdateChildren();

            base.Start();
        }

        public override void Update()
        {
            if (Application.isEditor && !Application.isPlaying)
            {
                SetDirty();

                UpdateChildren();
            }

            base.Update();
        }

        protected override SpriteSheet GetSpritesheet()
        {
            return SpriteSheetManager.GetSpriteSheet(spriteSheetId);
        }

        protected override void DrawMesh()
        {
            for (int i = 0; i < childrens.Count; i++)
                childrens[i].Draw(spriteMesh);
        }

        private void UpdateChildren()
        {
            trans = transform;
            childrens.Clear();

            UpdateChildren(transform);

            InitChildren();

            LayoutChildren();
        }

        private void UpdateChildren(Transform trans)
        {
            //Find all children
            for (int i = 0; i < trans.childCount; i++)
            {
                GUIObject child = trans.GetChild(i).GetComponent<GUIObject>();
                if (child)
                    childrens.Add(child);
            }

            //Add children's children
            for (int i = 0; i < trans.childCount; i++)
                UpdateChildren(trans.GetChild(i));
        }

        private void InitChildren()
        {
            for (int i = 0; i < childrens.Count; i++)
                if (!childrens[i].initialized || Application.isEditor && !Application.isPlaying)
                    childrens[i].Init(this);
        }

        private void LayoutChildren()
        {
            position = trans.position;

            for (int i = 0; i < childrens.Count; i++)
                childrens[i].Layout();

            //Sort by depth
            childrens.Sort(SortByDepth);
        }

        static private int SortByDepth(GUIObject g1, GUIObject g2)
        {
            if (g1.Depth > g2.Depth)
                return -1;
            else if (g1.Depth < g2.Depth)
                return 1;

            return 0;
        }

        public void OnDrawGizmosSelected()
        {
            OnDrawGizmos();
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;

            Gizmos.DrawWireCube(transform.position + new Vector3(editorScreenWidth / 2.0f, -editorScreenHeight / 2.0f, 200.0f), new Vector3(editorScreenWidth, editorScreenHeight, 1.0f));
        }
    }
}

