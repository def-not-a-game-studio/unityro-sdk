using System;
using System.Collections.Generic;
using System.Linq;
using _3rdparty.unityro_sdk.Core.Effects;
using Cysharp.Threading.Tasks;
using ROIO.Models.FileTypes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityRO.Core;
using Object = System.Object;

namespace Core.Effects
{
    public class StrEffectRenderer : ManagedMonoBehaviour
    {
        [SerializeField] private STR Anim;
        [SerializeField] private int EffectId;
        [SerializeField] private bool Loop;
        [SerializeField] private bool ManualAdvanceFrame;
        [HideInInspector] [SerializeField] private int newFrame;

        private static Vector3[] outVertices = new Vector3[4];
        private static Vector3[] outNormals = new Vector3[4];
        private static int[] outTris = new int[6];
        private static Vector2[] outUvs = new Vector2[4];
        private static Color[] outColors = new Color[4];
        private static Vector2[] tempPositions2 = new Vector2[4];
        private static Vector2[] tempUvs2 = new Vector2[4];

        private bool isInit;
        private Dictionary<float, BlendMode> BlendModes = new();
        private Dictionary<int, List<EffectRenderInfo>> _effectRenderInfo = new();
        private Material _material;

        private float _time;
        private int _currentFrame;

        public UnityAction OnEnd;

        private EffectCache _effectCache;

        private void Awake()
        {
            _effectCache = FindAnyObjectByType<EffectCache>();
            _material = new Material(Shader.Find("Ragnarok/EffectShader"));
            _material.enableInstancing = true;
            _material.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
            _material.SetFloat("_DstBlend", (float)BlendMode.One);

            BlendModes[1] = BlendMode.Zero;
            BlendModes[2] = BlendMode.One;
            BlendModes[3] = BlendMode.SrcColor;
            BlendModes[4] = BlendMode.OneMinusSrcColor;
            BlendModes[5] = BlendMode.SrcAlpha;
            BlendModes[6] = BlendMode.OneMinusSrcAlpha;
            BlendModes[7] = BlendMode.DstAlpha;
            BlendModes[8] = BlendMode.OneMinusDstAlpha;
            BlendModes[9] = BlendMode.DstColor;
            BlendModes[10] = BlendMode.OneMinusDstColor;
            BlendModes[11] = BlendMode.One;
            BlendModes[12] = BlendMode.One;
            BlendModes[13] = BlendMode.Zero;
            BlendModes[14] = BlendMode.Zero;
            BlendModes[15] = BlendMode.Zero;
        }

        public void Initialize(STR animation, int effectId, Dictionary<int, List<EffectRenderInfo>> renderInfo)
        {
            isInit = false;
            Anim = animation;
            EffectId = effectId;

            _time = 0;
            _currentFrame = 0;
            _effectRenderInfo = renderInfo;
            isInit = true;
        }

        public void Initialize(STR animation, int effectId)
        {
            isInit = false;
            Anim = animation;
            EffectId = effectId;

            _time = 0;
            _currentFrame = 0;
            _material.SetTexture("_MainTex", Anim.Atlas);

            InitializeMeshes(Anim);
        }

        public override void ManagedUpdate()
        {
            if (!isInit)
                return;

            _time += Time.deltaTime;
            if (!ManualAdvanceFrame)
                newFrame = Mathf.FloorToInt(_time * Anim.fps);

            Render();

            if (newFrame == _currentFrame) return;
            _currentFrame = newFrame;
            if (_currentFrame <= Anim.maxKey) return;

            isInit = false;
            if (Loop)
            {
                _time = 0;
                _currentFrame = -1;
                isInit = true;
            }

            OnEnd?.Invoke();
        }

        private void Render()
        {
            if (!_effectRenderInfo.TryGetValue(_currentFrame, out var value)) return;
            foreach (var pInfo in value)
            {
                Graphics.RenderMesh(pInfo.RenderParams, pInfo.Mesh, 0, transform.localToWorldMatrix);
            }
        }

        private MeshBuilderInfo UpdateAnimationLayer(STR.Layer layer, int currentFrame)
        {
            var lastFrame = 0;
            var lastSource = 0;
            var startAnim = -1;
            var nextAnim = -1;

            for (var i = 0; i < layer.animations.Length; i++)
            {
                var a = layer.animations[i];
                if (a.frame < currentFrame)
                {
                    if (a.type == 0)
                        startAnim = i;
                    if (a.type == 1)
                        nextAnim = i;
                }

                lastFrame = Mathf.Max(lastFrame, a.frame);
                if (a.type == 0)
                    lastSource = Mathf.Max(lastSource, a.frame);
            }

            if (startAnim < 0 || (nextAnim < 0 && lastFrame < currentFrame))
                return null;

            var from = layer.animations[startAnim];
            STR.Animation to = null;

            if (nextAnim >= 0)
                to = layer.animations[nextAnim];

            var delta = currentFrame - from.frame;
            if (nextAnim != startAnim + 1 || to?.frame != from.frame)
            {
                if (to != null && lastSource <= from.frame)
                    return null;

                var fixedFrame = layer.texturesIds[(int)from.animFrame];
                return new MeshBuilderInfo
                {
                    vertex = from.xy,
                    position = from.position,
                    angle = from.angle,
                    imageId = fixedFrame,
                    color = from.color,
                    srcBlend = (int)from.srcAlpha,
                    dstBlend = (int)from.destAlpha,
                };
            }

            for (var i = 0; i < 4; i++)
            {
                tempPositions2[i] = from.xy[i] + to.xy[i] * delta;
                tempUvs2[i] = from.uv[i] + to.uv[i] * delta;
            }

            var pos = from.position + to.position * delta;
            var angle = from.angle + to.angle * delta;
            var color = from.color + to.color * delta;

            var frameId = to.animType switch
            {
                1 => Mathf.FloorToInt(from.animFrame + to.animFrame * delta),
                2 => Mathf.FloorToInt(Mathf.Min(from.animFrame + to.delay * delta, layer.textures.Length - 1)),
                3 => Mathf.FloorToInt((from.animFrame + to.delay * delta) % layer.textures.Length),
                4 => Mathf.FloorToInt((from.animFrame - to.delay * delta) % layer.textures.Length),
                _ => 0
            };

            var texIndex = layer.texturesIds[frameId];
            return new MeshBuilderInfo
            {
                vertex = tempPositions2,
                position = pos,
                angle = angle,
                imageId = texIndex,
                color = color,
                srcBlend = (int)from.srcAlpha,
                dstBlend = (int)from.destAlpha,
            };
        }

        private Mesh GenerateFrameMesh(MeshBuilderInfo part, int layerIndex)
        {
            var verts = part.vertex;
            var bounds = Anim.AtlasRects[part.imageId];
            for (var i = 0; i < 4; i++)
            {
                var vertex = verts[i];
                var v = Rotate(vertex, -part.angle * Mathf.Deg2Rad);
                var pos = new Vector2(part.position.x - 320f, -(part.position.y - 360f));
                outVertices[i] = (v + pos) / SPR.PIXELS_PER_UNIT;
                outColors[i] = part.color;
                outNormals[i] = Vector3.back;
            }

            outUvs[0] = new Vector3(bounds.xMin, bounds.yMax, layerIndex);
            outUvs[1] = new Vector3(bounds.xMax, bounds.yMax, layerIndex);
            outUvs[2] = new Vector3(bounds.xMin, bounds.yMin, layerIndex);
            outUvs[3] = new Vector3(bounds.xMax, bounds.yMin, layerIndex);

            outTris[0] = 0;
            outTris[1] = 1;
            outTris[2] = 2;
            outTris[3] = 1;
            outTris[4] = 3;
            outTris[5] = 2;

            var mesh = new Mesh
            {
                vertices = outVertices,
                triangles = outTris,
                colors = outColors,
                normals = outNormals,
                uv = outUvs,
            };
            mesh.Optimize();
            return mesh;
        }

        private Vector2 Rotate(Vector2 v, float delta)
        {
            return new Vector2(
                v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
                v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
            );
        }

        public void Replay()
        {
            if (Anim != null)
                Initialize(Anim, EffectId);
        }

        private async void InitializeMeshes(STR anim)
        {
            _effectRenderInfo.Clear();

            for (var frame = 0; frame < anim.maxKey; frame++)
            {
                PopulateCache(frame).Forget();
                await UniTask.Yield();
            }

            isInit = true;
            _effectCache.CacheEffect(EffectId, _effectRenderInfo);
        }

        private async UniTaskVoid PopulateCache(int frame)
        {
            var list = new List<EffectRenderInfo>();
            for (var index = 0; index < Anim.layers.Length; index++)
            {
                var layer = Anim.layers[index];
                if (layer.animations.Length == 0)
                    continue;

                var res = UpdateAnimationLayer(layer, frame);
                if (res == null) continue;

                var srcBlend = BlendModes[res.srcBlend];
                var dstBlend = BlendModes[res.dstBlend];
                
                if (srcBlend == BlendMode.SrcAlpha && dstBlend == BlendMode.DstAlpha)
                {
                    dstBlend = BlendMode.One;
                }

                var clone = Instantiate(_material);
                clone.SetFloat("_SrcBlend", (float)srcBlend);
                clone.SetFloat("_DstBlend", (float)dstBlend);
                var renderParams = new RenderParams(clone)
                {
                    receiveShadows = false,
                    lightProbeUsage = LightProbeUsage.Off,
                    shadowCastingMode = ShadowCastingMode.Off,
                };
                list.Add(new EffectRenderInfo
                {
                    RenderParams = renderParams,
                    Mesh = GenerateFrameMesh(res, index),
                });

                await UniTask.Yield();
            }

            _effectRenderInfo[frame] = list;
        }

        private class MeshBuilderInfo
        {
            public Vector2[] vertex;
            public Vector2 position;
            public float angle;
            public int imageId;
            public Color color;
            public int srcBlend;
            public int dstBlend;
        }
    }
}