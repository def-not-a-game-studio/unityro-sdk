using System;
using Assets.Scripts.Renderer.Map.Effects.EffectParts;
using UnityEngine;
using UnityEngine.Serialization;

namespace UnityRO.core.Effects {
    public class CylinderEffectRenderer : MonoBehaviour {
        public CylinderEffectPart Part;
        public long DelayToStart = 0;

        private MeshRenderer _meshRenderer;

        private long startTick;
        private long endTick;

        private void Start() {
            _meshRenderer = gameObject.AddComponent<MeshRenderer>();
            var meshFilter = gameObject.AddComponent<MeshFilter>();

            var mesh = GenerateCylinder(Part.totalCircleSides, Part.circleSides, Part.repeatTextureX);
            mesh.uv = UvCalculator.CalculateUVs(mesh.vertices, 1f);
            mesh.RecalculateNormals();
            meshFilter.mesh = mesh;
            _meshRenderer.material = new Material(Shader.Find("Custom/Cylinder"));

            Part.texture.wrapMode = Part.repeatTextureX > 0 ? TextureWrapMode.Repeat : TextureWrapMode.Clamp;

            _meshRenderer.material.SetTexture("_MainTex", Part.texture);

            Init();
        }

        private void Init() {
            ResetTimers();

            _meshRenderer.material.SetFloat("_TopSize", Part.topSize);
            _meshRenderer.material.SetFloat("_BottomSize", Part.bottomSize);
            _meshRenderer.material.SetFloat("_Height", Part.height);
            _meshRenderer.material.SetVector("_Position", Part.position);

            if (Part.angleX != 0 || Part.angleY != 0 || Part.angleZ != 0) {
                var m = Matrix4x4.identity;
                m.SetTRS(Vector3.zero,
                    Quaternion.Euler(new Vector3(Part.angleX, -Part.angleY, Part.angleZ)),
                    Vector3.one);
                _meshRenderer.material.SetMatrix("_RotationMatrix", m);
                _meshRenderer.material.SetFloat("_Rotate", 1);
            }

            if (Part.fade) {
                Part.Color.a = 0f;
                _meshRenderer.material.SetColor("_Color", Part.Color);
            }

            _meshRenderer.material.SetFloat("_DstBlend", Part.blendMode - 1);
        }

        private void ResetTimers() {
            startTick = GameManager.Tick + DelayToStart;
            endTick = startTick + Part.duration;
        }

        public void Update() {
            var duration = endTick - startTick;
            if (startTick > GameManager.Tick) return;

            var renderCount = (GameManager.Tick - startTick) * 1f;

            if (GameManager.Tick > endTick) {
                if (Part.repeat) {
                    DelayToStart = 0;
                    ResetTimers();
                }
            }

            Part.Color.a = Part.alphaMax;

            if (Part.fade) {
                if (renderCount < duration / 4) {
                    Part.Color.a = renderCount * Part.alphaMax / (Part.duration / 4);
                } else if (renderCount > duration / 2 + duration / 4) {
                    Part.Color.a = (duration - renderCount) * Part.alphaMax / (duration / 4);
                }

                _meshRenderer.material.SetColor("_Color", Part.Color);
            }

            float height = 0, topSize = 0, bottomSize = 0;

            switch (Part.animation) {
                case 1:
                    if (duration > 1000) {
                        if (renderCount <= 1000) {
                            height = renderCount / 1000 * Part.height;
                        } else {
                            height = Part.height;
                        }
                    } else {
                        height = renderCount / duration * Part.height;
                    }

                    topSize = Part.topSize;
                    break;
                case 2:
                    if (duration > 1000) {
                        if (renderCount <= 1000) {
                            topSize = renderCount / 1000 * Part.topSize;
                        } else {
                            topSize = Part.topSize;
                        }
                    } else {
                        topSize = renderCount / duration * Part.topSize;
                    }

                    break;
                case 3:
                    height = Part.height;
                    bottomSize = (1 - renderCount / duration) * Part.bottomSize;
                    topSize = (1 - renderCount / duration) * Part.topSize;
                    if (renderCount < duration / 2) {
                        height = renderCount * Part.height / (duration / 2);
                    } else if (renderCount > duration / 2) {
                        height = (duration - renderCount) * Part.height / (duration / 2);
                    }

                    break;
                case 4:
                    height = Part.height;
                    bottomSize = renderCount / duration * bottomSize;
                    if (bottomSize < 0) bottomSize = 0;
                    topSize = renderCount / duration * Part.topSize;
                    if (topSize < 0) topSize = 0;
                    break;
                case 5:
                    if (renderCount < duration / 2) {
                        height = renderCount * 2 / duration * Part.height;
                    } else {
                        height = (duration - renderCount) * Part.height / (duration / 2);
                    }

                    topSize = Part.topSize;
                    break;
                default:
                    height = Part.height;
                    topSize = Part.topSize;
                    break;
            }

            _meshRenderer.material.SetFloat("_Height", height);
            _meshRenderer.material.SetFloat("_TopSize", topSize);
            _meshRenderer.material.SetFloat("_BottomSize", bottomSize);
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

            return mesh;
        }
    }
}