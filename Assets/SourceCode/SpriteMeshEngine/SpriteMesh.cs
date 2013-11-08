using UnityEngine;
using System.Collections.Generic;

namespace SpriteMeshEngine
{
    public struct SpriteMeshInfo
    {
        public int triangles;
        public int vertices;
    }

    public class SpriteMesh
    {
        private Mesh mesh;

        private int[] triangles;
        private Vector2[] uvs;
        private Vector3[] positions;
        private Color32[] colors;

        private bool sizeOnly;

        private int trianglesOffset;
        private int verticesOffset;
        private float z;
        private float scale;

        //private int screenWidth;
        //private int screenHeight;

        public Mesh Mesh
        {
            get { return mesh; }
        }

        public SpriteMesh()
        {
            mesh = new Mesh();
        }

        public void SetZ(float z)
        {
            this.z = z;
        }

        public void SetScale(float scale)
        {
            this.scale = scale;
        }

        public void BeginCalculateSize()
        {
            sizeOnly = true;
            trianglesOffset = 0;
            verticesOffset = 0;
            z = 0.0f;
            scale = 1.0f;
        }

        public void Begin(SpriteMeshInfo info)
        {
            sizeOnly = false;
            trianglesOffset = 0;
            verticesOffset = 0;
            z = 0.0f;
            scale = 1.0f;

            //screenWidth = Screen.width;
            //screenHeight = Screen.height;

            if (triangles == null || triangles.Length != info.triangles)
            {
                triangles = new int[info.triangles];
                mesh.Clear();
            }

            if (uvs == null || uvs.Length != info.vertices)
                uvs = new Vector2[info.vertices];

            if (positions == null || positions.Length != info.vertices)
            {
                positions = new Vector3[info.vertices];
                mesh.Clear();
            }

            if (colors == null || colors.Length != info.vertices)
                colors = new Color32[info.vertices];
        }

        public SpriteMeshInfo End()
        {
            SpriteMeshInfo info = new SpriteMeshInfo();

            info.triangles = trianglesOffset;
            info.vertices = verticesOffset;

            return info;
        }

        public int AddVertice(float x, float y)
        {
            return AddVerticeUC(x, y, 0, 0, new Color32(255, 255, 255, 255));
        }

        public int AddVerticeC(
            float x, float y, 
            Color32 color)
        {
            return AddVerticeUC(x, y, 0, 0, color);
        }

        public int AddVerticeU(
            float x, float y, 
            float uvx, float uvy)
        {
            return AddVerticeUC(x, y, uvx, uvy, new Color32(255, 255, 255, 255));
        }

        public int AddVerticeUC(
            float x, float y, 
            float uvx, float uvy, 
            Color32 color)
        {
            if (!sizeOnly)
            {
                positions[verticesOffset] = new Vector3((int) (x * scale), -(int) (y * scale), z);
                colors[verticesOffset] = color;
                uvs[verticesOffset] = new Vector2(uvx, uvy);
            }

            return verticesOffset++;
        }

        public void AddTriangle(int v1, int v2, int v3)
        {
            if (!sizeOnly)
            {
                triangles[trianglesOffset + 0] = v1;
                triangles[trianglesOffset + 1] = v2;
                triangles[trianglesOffset + 2] = v3;
            }

            trianglesOffset += 3;
        }

        public void AddQuad(
            float x, float y, 
            float sizeX, float sizeY,
            float uvFromX, float uvFromY,
            float uvToX, float uvToY,
            Color32 color)
        {
            int v0 = AddVerticeUC(x, y, uvFromX, uvFromY, color);
            int v1 = AddVerticeUC(x + sizeX, y, uvToX, uvFromY, color);
            int v2 = AddVerticeUC(x + sizeX, y + sizeY, uvToX, uvToY, color);
            int v3 = AddVerticeUC(x, y + sizeY, uvFromX, uvToY, color);

            AddTriangle(v0, v1, v2);
            AddTriangle(v2, v3, v0);
        }

        public void AddSprite(
            Sprite sprite, 
            float x, float y, 
            float sizeX, float sizeY,
            Color32 color)
        {
            AddQuad(x, y,
                    sizeX, sizeY, 
                    sprite.UV.xMin, sprite.UV.yMax,
                    sprite.UV.xMax, sprite.UV.yMin,
                    color);
        }

        public void Apply()
        {
            mesh.vertices = positions;
            mesh.uv = uvs;
            mesh.colors32 = colors;

            mesh.triangles = triangles;
            mesh.Optimize();
            mesh.RecalculateBounds();
        }
    }
}

