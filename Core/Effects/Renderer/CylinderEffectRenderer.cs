using System;
using Core.Effects.EffectParts;
using UnityEngine;
using UnityRO.Core;

namespace Core.Effects
{
    public class CylinderEffectRenderer : EffectRendererPart
    {
        private CylinderEffectPart Part;

        private long DelayToStart = 0;
        private long startTick;
        private long endTick;
        private float updateRate = 30f;
        private float nextUpdate = 0f;

        private RenderParams _renderParams;
        private Mesh _mesh;

        public void SetPart(CylinderEffectPart part, long delayToStart)
        {
            Part = part;
            DelayToStart = delayToStart;

            Start();
        }

        private void Start()
        {
            _mesh = GenerateCylinder(Part.totalCircleSides, Part.circleSides, Part.repeatTextureX);
            _renderParams = new RenderParams(new Material(Shader.Find("Custom/Cylinder")));
            Part.texture.wrapMode = Part.repeatTextureX > 0 ? TextureWrapMode.Repeat : TextureWrapMode.Clamp;

            _renderParams.material.SetTexture("_MainTex", Part.texture);

            Init();
        }

        private void Init()
        {
            ResetTimers();

            _renderParams.material.SetFloat("_TopSize", Part.topSize);
            _renderParams.material.SetFloat("_BottomSize", Part.bottomSize);
            _renderParams.material.SetFloat("_Height", Part.height);
            _renderParams.material.SetVector("_Position", Part.position);

            if (Part.angleX != 0 || Part.angleY != 0 || Part.angleZ != 0)
            {
                var m = Matrix4x4.identity;
                m.SetTRS(Vector3.zero,
                    Quaternion.Euler(new Vector3(Part.angleX, -Part.angleY, Part.angleZ)),
                    Vector3.one);
                _renderParams.material.SetMatrix("_RotationMatrix", m);
                _renderParams.material.SetFloat("_Rotate", 2);
            }

            if (Part.fade)
            {
                Part.Color.a = 0f;
                _renderParams.material.SetColor("_Color", Part.Color);
            }

            if (Part.rotate)
            {
                _renderParams.material.SetFloat("_Rotate", 1);
            }

            _renderParams.material.SetFloat("_DstBlend", (int)Part.blendMode);
        }

        private void ResetTimers()
        {
            startTick = GameManager.Tick + DelayToStart;
            endTick = startTick + Part.duration;
        }

        public override void Update(Matrix4x4 matrix)
        {
            Graphics.RenderMesh(_renderParams, _mesh, 0, matrix);
            
            var timeInterval = Time.time - nextUpdate;
            if (timeInterval <= 1f / updateRate)
            {
                return;
            }

            nextUpdate = Time.time;
            UpdateRender();
        }

        private void UpdateRender()
        {
            var duration = endTick - startTick;
            if (startTick > GameManager.Tick) return;

            var renderCount = (GameManager.Tick - startTick) * 1f;

            if (GameManager.Tick > endTick)
            {
                if (Part.repeat)
                {
                    DelayToStart = 0;
                    ResetTimers();
                }
                else
                {
                    OnEnd?.Invoke(this);
                }
            }

            HandleFade(renderCount, duration);
            HandleAnimation(duration, renderCount);
        }

        private void HandleFade(float renderCount, long duration)
        {
            Part.Color.a = Part.alphaMax;

            if (Part.fade)
            {
                if (renderCount < duration / 4f)
                {
                    Part.Color.a = renderCount * Part.alphaMax / (duration / 4f);
                }
                else if (renderCount > duration / 2f + duration / 4f)
                {
                    Part.Color.a = (duration - renderCount) * Part.alphaMax / (duration / 4f);
                }
            }

            _renderParams.material.SetColor("_Color", Part.Color);
        }

        private void HandleAnimation(long duration, float renderCount)
        {
            float height = Part.height, topSize = Part.topSize, bottomSize = Part.bottomSize;

            switch (Part.animation)
            {
                case 1:
                    if (duration > 1000)
                    {
                        if (renderCount <= 1000)
                        {
                            height = renderCount / 1000 * Part.height;
                        }
                        else
                        {
                            height = Part.height;
                        }
                    }
                    else
                    {
                        height = renderCount / duration * Part.height;
                    }

                    topSize = Part.topSize;
                    break;
                case 2:
                    if (duration > 1000)
                    {
                        if (renderCount <= 1000)
                        {
                            topSize = renderCount / 1000 * Part.topSize;
                        }
                        else
                        {
                            topSize = Part.topSize;
                        }
                    }
                    else
                    {
                        topSize = renderCount / duration * Part.topSize;
                    }

                    break;
                case 3:
                    height = Part.height;
                    bottomSize = (1 - renderCount / duration) * Part.bottomSize;
                    topSize = (1 - renderCount / duration) * Part.topSize;
                    if (renderCount < duration / 2f)
                    {
                        height = renderCount * Part.height / (duration / 2f);
                    }
                    else if (renderCount > duration / 2f)
                    {
                        height = (duration - renderCount) * Part.height / (duration / 2f);
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
                    if (renderCount < duration / 2f)
                    {
                        height = renderCount * 2 / duration * Part.height;
                    }
                    else
                    {
                        height = (duration - renderCount) * Part.height / (duration / 2f);
                    }

                    topSize = Part.topSize;
                    break;
                default:
                    height = Part.height;
                    topSize = Part.topSize;
                    break;
            }

            _renderParams.material.SetFloat("_Height", height);
            _renderParams.material.SetFloat("_TopSize", topSize);
            _renderParams.material.SetFloat("_BottomSize", bottomSize);
        }

        private Mesh GenerateCylinder(int totalCircleSides, int circleSides, int repeatTextureX)
        {
            Vector3[] vertices = new Vector3[(circleSides + 1) * 2];
            int[] triangles = new int[circleSides * 6];
            Vector2[] uvs = new Vector2[vertices.Length];

            int vertexIndex = 0;
            int triangleIndex = 0;

            for (int i = 0; i <= circleSides; i++)
            {
                float a = (i + 0.0f) / totalCircleSides;
                float b = (i + 0.0f) / totalCircleSides;

                float bottomY = (float)Math.Sin(a * Math.PI * 2);
                float topY = (float)Math.Sin(b * Math.PI * 2);

                float bottomRadius = (float)Math.Cos(a * Math.PI * 2);
                float topRadius = (float)Math.Cos(b * Math.PI * 2);

                var bottomIndex = vertexIndex++;
                var topIndex = vertexIndex++;

                vertices[bottomIndex] = new Vector3(bottomRadius, bottomY, 0f);
                vertices[topIndex] = new Vector3(topRadius, topY, 1f);

                uvs[topIndex] = new Vector2(a * totalCircleSides / circleSides, 1);
                uvs[bottomIndex] = new Vector2(b * totalCircleSides / circleSides, 0);

                if (i < circleSides)
                {
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
            mesh.uv = uvs;
            mesh.RecalculateNormals();

            return mesh;
        }
    }
}