using System;
using System.Collections.Generic;
using Core.Effects.EffectParts;
using Cysharp.Threading.Tasks;
using ROIO.Models.FileTypes;
using UnityEngine;
using UnityEngine.Rendering;

namespace _3rdparty.unityro_sdk.Core.Effects
{
    public class EffectLayerInfo
    {
        public RenderParams RenderParams;
        public Mesh Mesh;
    }
    
    public class EffectRenderInfo {
        public Dictionary<int, List<EffectLayerInfo>> Frames;
        public float Fps;
        public int EffectId;
        public AudioClip AudioClip;
        public int MaxKey;
    }

    public class EffectCache : MonoBehaviour
    {
        public Dictionary<int, EffectRenderInfo> EffectRenderInfos = new();
        public Dictionary<int, Effect> Effects = new();

        private Material _material;

        private void Awake()
        {
            DontDestroyOnLoad(this);

            _material = new Material(Shader.Find("Ragnarok/EffectShader"));
            _material.enableInstancing = true;
            _material.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
            _material.SetFloat("_DstBlend", (float)BlendMode.One);

            var effects = Resources.LoadAll<Effect>("Database/Effects/Extracted");
            foreach (var effect in effects)
            {
                Effects.TryAdd(effect.EffectId, effect);
            }
        }

        /// <summary>
        /// Builds asynchronously the meshes need to display the effect.
        /// Only supports STR for now
        /// </summary>
        /// <param name="effectId"></param>
        /// <returns></returns>
        public async UniTask<EffectRenderInfo> GetRenderInfo(int effectId)
        {
            if (!EffectRenderInfos.TryGetValue(effectId, out var effectRenderInfo))
            {
                var effect = Effects[effectId];
                // TODO render more parts
                var part = effect.STRParts[0];
                effectRenderInfo = await StrEffectBuilder.InitializeMeshes(part.file, effectId);
                effectRenderInfo.AudioClip = part.wav;
                EffectRenderInfos.Add(effectId, effectRenderInfo);
            }

            return effectRenderInfo;
        }

        public async UniTask<EffectRenderInfo> GetRenderInfo(int effectId, StrEffectPart part)
        {
            if (!EffectRenderInfos.TryGetValue(effectId, out var effectRenderInfo))
            {
                effectRenderInfo = await StrEffectBuilder.InitializeMeshes(part.file, effectId);
                effectRenderInfo.AudioClip = part.wav;
                EffectRenderInfos.Add(effectId, effectRenderInfo);
            }

            return effectRenderInfo;
        }
    }

    public static class StrEffectBuilder
    {
        private static Vector3[] outVertices = new Vector3[4];
        private static Vector3[] outNormals = new Vector3[4];
        private static int[] outTris = new int[6];
        private static Vector2[] outUvs = new Vector2[4];
        private static Color[] outColors = new Color[4];
        private static Vector2[] tempPositions2 = new Vector2[4];
        private static Vector2[] tempUvs2 = new Vector2[4];
        private static Dictionary<float, BlendMode> _blendModes = new();

        private static Dictionary<int, List<EffectLayerInfo>> _effectRenderInfo = new();
        private static Material _material;

        static StrEffectBuilder()
        {
            _blendModes[1] = BlendMode.Zero;
            _blendModes[2] = BlendMode.One;
            _blendModes[3] = BlendMode.SrcColor;
            _blendModes[4] = BlendMode.OneMinusSrcColor;
            _blendModes[5] = BlendMode.SrcAlpha;
            _blendModes[6] = BlendMode.OneMinusSrcAlpha;
            _blendModes[7] = BlendMode.DstAlpha;
            _blendModes[8] = BlendMode.OneMinusDstAlpha;
            _blendModes[9] = BlendMode.DstColor;
            _blendModes[10] = BlendMode.OneMinusDstColor;
            _blendModes[11] = BlendMode.One;
            _blendModes[12] = BlendMode.One;
            _blendModes[13] = BlendMode.Zero;
            _blendModes[14] = BlendMode.Zero;
            _blendModes[15] = BlendMode.Zero;

            _material = new Material(Shader.Find("Ragnarok/EffectShader"));
            _material.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
            _material.SetFloat("_DstBlend", (float)BlendMode.One);
        }

        public static async UniTask<EffectRenderInfo> InitializeMeshes(STR anim, int id)
        {
            _effectRenderInfo.Clear();
            _material.SetTexture("_MainTex", anim.Atlas);

            for (var frame = 0; frame < anim.maxKey; frame++)
            {
                PopulateCache(anim, frame).Forget();
                await UniTask.Yield();
            }

            return new EffectRenderInfo
            {
                Fps = anim.fps,
                EffectId = id,
                Frames = _effectRenderInfo,
                MaxKey = (int)anim.maxKey,
            };
        }

        private static async UniTaskVoid PopulateCache(STR anim, int frame)
        {
            var list = new List<EffectLayerInfo>();
            for (var index = 0; index < anim.layers.Length; index++)
            {
                var layer = anim.layers[index];
                if (layer.animations.Length == 0)
                    continue;

                var res = UpdateAnimationLayer(layer, frame);
                if (res == null) continue;

                var srcBlend = _blendModes[res.srcBlend];
                var dstBlend = _blendModes[res.dstBlend];

                if (srcBlend == BlendMode.SrcAlpha && dstBlend == BlendMode.DstAlpha)
                {
                    dstBlend = BlendMode.One;
                }

                var clone = GameObject.Instantiate(_material);
                clone.SetFloat("_SrcBlend", (float)srcBlend);
                clone.SetFloat("_DstBlend", (float)dstBlend);
                var renderParams = new RenderParams(clone)
                {
                    receiveShadows = false,
                    lightProbeUsage = LightProbeUsage.Off,
                    shadowCastingMode = ShadowCastingMode.Off,
                };
                list.Add(new EffectLayerInfo
                {
                    RenderParams = renderParams,
                    Mesh = GenerateFrameMesh(res, index, anim),
                });

                await UniTask.Yield();
            }

            _effectRenderInfo[frame] = list;
        }

        private static MeshBuilderInfo UpdateAnimationLayer(STR.Layer layer, int currentFrame)
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

        private static Mesh GenerateFrameMesh(MeshBuilderInfo part, int layerIndex, STR anim)
        {
            var verts = part.vertex;
            var bounds = anim.AtlasRects[part.imageId];
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

        private static Vector2 Rotate(Vector2 v, float delta)
        {
            return new Vector2(
                v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
                v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
            );
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