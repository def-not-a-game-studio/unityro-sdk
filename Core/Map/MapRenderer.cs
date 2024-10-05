#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _3rdparty.unityro_sdk.Core.Effects;
using Core.Effects;
using ROIO.Loaders;
using ROIO.Models.FileTypes;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityRO.Core.Sprite;

/// <summary>
/// Rendering of the map
/// 
/// @author Guilherme Hernandez
/// Based on ROBrowser by Vincent Thibault (robrowser.com)
/// </summary>
public class MapRenderer
{
    internal static Action<float> OnProgress;

    public static AudioMixerGroup SoundsMixerGroup;

    public Light WorldLight;

    private RSW world;
    private WaterBuilder water;
    private Models models;
    private Sounds sounds = new();
    private Sky sky;

    /*public class Fog {
        public Fog(bool use) { this.use = use; }
        //bool use = MapPreferences.useFog; TODO
        bool use;
        bool exist = true;
        int far = 30;
        int near = 180;
        float factor = 1.0f;
        float[] color = new float[]{1, 1, 1};
    }

    public Fog fog = new Fog(false);*/

    public async Task<GameMap> RenderMap(AsyncMapLoader.GameMapData gameMapData, string mapName, bool splitIntoChunks)
    {
        var gameMap = new GameObject(mapName).AddComponent<GameMap>();
        gameMap.tag = "Map";
        gameMap.SetMapLightInfo(gameMapData.World.light);
        gameMap.SetMapSize((int)gameMapData.Ground.width, (int)gameMapData.Ground.height);
        gameMap.SetMapAltitude(new Altitude(gameMapData.Altitude));

        var ignoreChunks = !mapName.Contains("_in");

        var ground = new Ground(gameMapData.CompiledGround, gameMapData.World.water, splitIntoChunks && ignoreChunks);

        for (int i = 0; i < gameMapData.World.sounds.Count; i++)
        {
            gameMapData.World.sounds[i].pos[0] += gameMap.Size.x;
            gameMapData.World.sounds[i].pos[1] *= -1;
            gameMapData.World.sounds[i].pos[2] += gameMap.Size.y;
            //world.sounds[i].pos[2] = tmp;
            gameMapData.World.sounds[i].range *= 0.3f;
            gameMapData.World.sounds[i].tick = 0;
        }

        await new Models(gameMapData.CompiledModels.ToList()).BuildMeshesAsync(null, true, gameMap.Size);

        InitializeSounds(gameMapData.World);
        gameMap.SetMapSounds(sounds.playing);
        MaybeInitSky(gameMap.transform, mapName);
        CreateLightPoints(gameMap.transform, gameMapData.World, gameMap.Size);
        if (gameMapData.World.lights.Count > 0)
        {
            CreateLightProbes(gameMap.transform, gameMap.Altitude,
                gameMap.transform.Find("_lights").GetComponentsInChildren<Transform>());
        }

        MaybeInitEffects(gameMap.transform, gameMap.Size, gameMapData.World.effects);

        return gameMap;
    }

    private static void MaybeInitEffects(Transform parent, Vector2Int mapSize, List<RSW.Effect> worldEffects)
    {
        if (worldEffects is { Count: 0 }) return;
        var effectParent = new GameObject("_effects");
        effectParent.transform.SetParent(parent, false);

        foreach (var effect in worldEffects)
        {
            if (effect.id == 47)
            {
                var effectObj = new GameObject(effect.name);
                effectObj.transform.SetParent(effectParent.transform, false);
                effectObj.transform.position = new Vector3(
                    effect.pos[0] + mapSize.x + .2f,
                    -effect.pos[1] + 2f,
                    effect.pos[2] + mapSize.y);
                var spriteEffect = effectObj.AddComponent<SpriteEffectRenderer>();
                var spriteData = Resources.Load<SpriteData>("Effects/SPR/torch_01");
                var atlas = Resources.Load<Texture2D>("Effects/SPR/torch_01");
                spriteData.atlas = atlas;
                spriteEffect.Init(spriteData, null, ViewerType.Effect);
            }
        }
    }

    private void InitializeSounds(RSW world)
    {
        //add sounds to playlist (and cache)
        foreach (var sound in world.sounds)
        {
            sounds.Add(sound, null);
        }
    }

    private void MaybeInitSky(Transform parent, string mapname)
    {
        if (WeatherEffect.HasMap(mapname))
        {
            //create sky
            var skyObject = new GameObject("_sky");
            skyObject.transform.SetParent(parent, false);
            sky = skyObject.AddComponent<Sky>();
            sky.Initialize(mapname);
        }
        else
        {
            //no weather effects, set sky color to blueish
            //Camera.main.backgroundColor = new Color(0.4f, 0.6f, 0.8f, 1.0f);
        }
    }

    private static void CreateLightPoints(Transform parent, RSW world, Vector2Int mapSize)
    {
        GameObject lightsParent = new GameObject("_lights");
        lightsParent.transform.SetParent(parent, false);
        var index = 0;
        foreach (var light in world.lights)
        {
            var lightObj = new GameObject(light.name).AddComponent<Light>();
            var color = new Color(light.color[0], light.color[1], light.color[2]);
            var height = -light.pos[1];
            var position = new Vector3(light.pos[0] + mapSize.x, height, light.pos[2] + mapSize.y);
            var groundHeight = (float)parent.GetComponent<GameMap>().Altitude.GetCellHeight(position.x, position.z);
            var groundPoint = new Vector3(position.x, groundHeight, position.z);
            var distance = (position - groundPoint).magnitude;
            if (distance > 13f)
            {
                position.y = Mathf.Abs(groundHeight + 13f);
            }

            var range = light.range / 5f;
            var intensity = Mathf.Max(3, (light.range + 1) / 3f);

            Color.RGBToHSV(color, out var hue, out var saturation, out var value);
            color = Color.HSVToRGB(hue, saturation, value * 3f);

            lightObj.transform.SetParent(lightsParent.transform, false);
            lightObj.transform.position = position;

            lightObj.gameObject.isStatic = true;
            lightObj.type = LightType.Point;
            lightObj.range = range;
            lightObj.color = color;
            lightObj.intensity = intensity;
            lightObj.shadows = LightShadows.Soft;
            // lightObj.cullingMask = 0;

            var subLight = new GameObject($"{light.name}_sub").AddComponent<Light>();
            subLight.transform.SetParent(lightObj.transform, false);
            subLight.gameObject.isStatic = false;
            subLight.type = LightType.Point;
            subLight.range = range / 2f;
            subLight.color = color;
            subLight.intensity = intensity;
            subLight.shadows = LightShadows.Soft;
            // subLight.cullingMask = 0;

#if UNITY_EDITOR
            lightObj.lightmapBakeType = LightmapBakeType.Baked;
            subLight.lightmapBakeType = LightmapBakeType.Baked;
#endif

            index++;
        }
    }

    [UnityEditor.MenuItem("Map/Recreate Light Probes")]
    private static void ReCreateLightProbes()
    {
        var parent = GameObject.Find(SceneManager.GetActiveScene().name);
        if (parent == null)
        {
            return;
        }

        var altitude = parent.GetComponent<GameMap>().Altitude;
        CreateLightProbes(parent.transform, altitude,
            parent.transform.Find("_lights").GetComponentsInChildren<Transform>());
    }

    [UnityEditor.MenuItem("Map/Adjust Lights")]
    private static async void AdjustLights()
    {
        var parent = GameObject.Find(SceneManager.GetActiveScene().name);
        if (parent == null)
        {
            return;
        }

        var mapName = parent.GetComponent<GameMap>().name;
        AsyncMapLoader.GameMapData gameMapData = await new AsyncMapLoader().Load($"{mapName}.rsw");

        CreateLightPoints(parent.transform, gameMapData.World, parent.GetComponent<GameMap>().Size);
    }

    [UnityEditor.MenuItem("Map/Recreate Effects")]
    private static async void RecreateEffects()
    {
        var parent = GameObject.Find(SceneManager.GetActiveScene().name);
        if (parent == null)
        {
            return;
        }

        var mapName = parent.GetComponent<GameMap>().name;
        AsyncMapLoader.GameMapData gameMapData = await new AsyncMapLoader().Load($"{mapName}.rsw");

        MaybeInitEffects(parent.transform, parent.GetComponent<GameMap>().Size, gameMapData.World.effects);
    }

    private static void CreateLightProbes(Transform parent, Altitude altitude, Transform[] lights)
    {
        var LightProbes = new GameObject("LightProbes");
        LightProbes.transform.SetParent(parent, false);
        var lightProbeGroup = LightProbes.AddComponent<LightProbeGroup>();

        var positions = new List<Vector3>();
        for (var x = 0; x < altitude.getWidth() / 2; x++)
        {
            for (var y = 0; y < altitude.getHeight() / 2; y++)
            {
                // if (!altitude.IsCellWalkable(x * 2, y * 2)) continue;
                var height = (float)altitude.GetCellHeight(x * 2, y * 2) + 0.5f;

                var ground = new Vector3(x * 2, height, y * 2);
                var middle = new Vector3(x * 2, height + 2.5f, y * 2);
                var top = new Vector3(x * 2, height + 5f, y * 2);

                positions.Add(ground);
                positions.Add(middle);
                positions.Add(top);
            }
        }

        lightProbeGroup.probePositions = positions.ToArray();
    }
}
#endif