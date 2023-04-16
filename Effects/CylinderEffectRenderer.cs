using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityRO.core.Effects {
    public class CylinderEffectRenderer : MonoBehaviour {
        private void Start() {
            var meshRenderer = gameObject.AddComponent<MeshRenderer>();
            var meshFilter = gameObject.AddComponent<MeshFilter>();

            var totalCircleSides = 20;
            var circleSides = 20;
            var repeatTextureX = 1;
            
            var mesh = GenerateCylinder(totalCircleSides, circleSides, repeatTextureX);
            mesh.uv = UvCalculator.CalculateUVs(mesh.vertices, 1f);
            meshFilter.mesh = mesh;
            meshRenderer.material = new Material(Shader.Find("Custom/Cylinder"));
        }

        private Mesh GenerateCylinder(int totalCircleSides, int circleSides, int repeatTextureX) {
            Vector3[] vertices = new Vector3[(circleSides + 1) * 2];
            int[] triangles = new int[circleSides * 6];

            int vertexIndex = 0;
            int triangleIndex = 0;

            for (int i = 0; i <= circleSides; i++) {
                float a = (i + 0.0f) / totalCircleSides;
                float b = (i + 0.0f) / totalCircleSides;

                float bottomY = (float)Math.Sin(a * Math.PI * 2);
                float topY = (float)Math.Sin(b * Math.PI * 2);

                float bottomRadius = (float)Math.Cos(a * Math.PI * 2);
                float topRadius = (float)Math.Cos(b * Math.PI * 2);

                vertices[vertexIndex++] = new Vector3(bottomRadius, bottomY, 0f);
                vertices[vertexIndex++] = new Vector3(topRadius, topY, 1f);

                if (i < circleSides) {
                    triangles[triangleIndex++] = i * 2;
                    triangles[triangleIndex++] = i * 2 + 2;
                    triangles[triangleIndex++] = i * 2 + 1;

                    triangles[triangleIndex++] = i * 2 + 1;
                    triangles[triangleIndex++] = i * 2 + 2;
                    triangles[triangleIndex++] = i * 2 + 3;
                }
            }


            var mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            return mesh;
        }
    }
}