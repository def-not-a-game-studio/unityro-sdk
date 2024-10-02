using System.Collections.Generic;
using System.Linq;
using ROIO.Models.FileTypes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityRO.Core;

namespace Core.Effects
{
    public class StrEffectRenderer2 : ManagedMonoBehaviour
    {
        [SerializeField] private STR Anim;
        [SerializeField] private BlendMode srcBlend;
        [SerializeField] private BlendMode dstBlend;
        [SerializeField] private bool Loop;
        [SerializeField] private bool ManualAdvanceFrame;
        [HideInInspector] [SerializeField] private int newFrame;

        private static List<Vector3> outVertices = new(512);
        private static List<Vector3> outNormals = new(512);
        private static List<int> outTris = new(1024);
        private static List<Vector2> outUvs = new(512);
        private static List<Color> outColors = new(512);
        private static Vector2[] tempPositions2 = new Vector2[4];
        private static Vector2[] tempUvs2 = new Vector2[4];

        private bool isInit;
        private Dictionary<float, BlendMode> BlendModes = new();
        private Dictionary<int, List<RenderParams>> _rpCache = new();
        private Dictionary<int, List<Mesh>> _meshCache = new();

        private float time;
        private int _currentFrame;

        private Transform LayersParent;

        public UnityAction OnEnd;

        public void Initialize(STR animation)
        {
            Anim = animation;

            time = 0;
            _currentFrame = 0;

            _rpCache.Clear();
            _meshCache.Clear();

            isInit = true;
        }

        private void Start()
        {
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

        private void Awake()
        {
            LayersParent = new GameObject("Layers").transform;
            LayersParent.SetParent(transform, false);

            if (Anim != null)
                Initialize(Anim);
        }

        public override void ManagedUpdate()
        {
            if (!isInit)
                return;

            time += Time.deltaTime;
            if (!ManualAdvanceFrame)
                newFrame = Mathf.FloorToInt(time * Anim.fps);
            
            UpdateAnimationFrame();
            for (var index = 0; index < _meshCache[_currentFrame].Count; index++)
            {
                var mesh = _meshCache[_currentFrame][index];
                var rp = _rpCache[_currentFrame][index];
                Graphics.RenderMesh(rp, mesh, 0, transform.localToWorldMatrix);
            }

            if (newFrame == _currentFrame) return;
            _currentFrame = newFrame;

            if (_currentFrame > Anim.maxKey)
            {
                isInit = false;
                if (Loop)
                {
                    time = 0;
                    _currentFrame = -1;
                    isInit = true;
                }

                OnEnd?.Invoke();
            }
        }

        private void UpdateAnimationFrame()
        {
            if (!_meshCache.ContainsKey(_currentFrame))
            {
                _meshCache.Add(_currentFrame, new List<Mesh>());
                _rpCache.Add(_currentFrame, new List<RenderParams>());
                for (var index = 0; index < Anim.layers.Length; index++)
                {
                    var layer = Anim.layers[index];
                    if (layer.animations.Length == 0)
                        continue;

                    var res = UpdateAnimationLayer(layer);
                    if (res != null)
                    {
                        var material = new Material(Shader.Find("Ragnarok/EffectShader"));
                        var srcBlend = BlendModes[res.srcBlend];
                        var dstBlend = BlendModes[res.dstBlend];

                        if (srcBlend == BlendMode.SrcAlpha && dstBlend == BlendMode.DstAlpha)
                        {
                            dstBlend = BlendMode.One;
                        }

                        material.SetFloat("_SrcBlend", (float)srcBlend);
                        material.SetFloat("_DstBlend", (float)dstBlend);
                        material.SetTexture("_MainTex", Anim.Atlas);
                        var renderParams = new RenderParams(material)
                        {
                            receiveShadows = false,
                            lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off,
                            shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                        };
                        _rpCache[_currentFrame].Add(renderParams);
                        _meshCache[_currentFrame].Add(GenerateFrameMesh(res, index));
                    }
                }
            }
        }

        private MeshBuilderInfo UpdateAnimationLayer(STR.Layer layer)
        {
            var lastFrame = 0;
            var lastSource = 0;
            var startAnim = -1;
            var nextAnim = -1;

            for (var i = 0; i < layer.animations.Length; i++)
            {
                var a = layer.animations[i];
                if (a.frame < _currentFrame)
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

            if (startAnim < 0 || (nextAnim < 0 && lastFrame < _currentFrame))
                return null;

            var from = layer.animations[startAnim];
            STR.Animation to = null;

            if (nextAnim >= 0)
                to = layer.animations[nextAnim];

            var delta = _currentFrame - from.frame;
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
            var tIndex = 0;
            outNormals.Clear();
            outVertices.Clear();
            outTris.Clear();
            outUvs.Clear();
            outColors.Clear();

            var verts = part.vertex;
            var bounds = Anim.AtlasRects[part.imageId];
            for (var i = 0; i < 4; i++)
            {
                var vertex = verts[i];
                var v = Rotate(vertex, -part.angle * Mathf.Deg2Rad);
                var pos = new Vector2(part.position.x - 320f, -(part.position.y - 360f));
                outVertices.Add((v + pos) / SPR.PIXELS_PER_UNIT);
                outColors.Add(part.color);
                outNormals.Add(new Vector3(0, 0, -1));
            }

            outUvs.Add(new Vector3(bounds.xMin, bounds.yMax, layerIndex));
            outUvs.Add(new Vector3(bounds.xMax, bounds.yMax, layerIndex));
            outUvs.Add(new Vector3(bounds.xMin, bounds.yMin, layerIndex));
            outUvs.Add(new Vector3(bounds.xMax, bounds.yMin, layerIndex));

            outTris.Add(tIndex);
            outTris.Add(tIndex + 1);
            outTris.Add(tIndex + 2);
            outTris.Add(tIndex + 1);
            outTris.Add(tIndex + 3);
            outTris.Add(tIndex + 2);

            var mesh = new Mesh
            {
                vertices = outVertices.ToArray(),
                triangles = outTris.ToArray(),
                colors = outColors.ToArray(),
                normals = outNormals.ToArray(),
                uv = outUvs.ToArray(),
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
                Initialize(Anim);
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