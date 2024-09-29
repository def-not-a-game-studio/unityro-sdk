﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ROIO.Loaders;
using ROIO.Models.FileTypes;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

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

        var ignoreChunks = !mapName.ContainsInvariantCultureIgnoreCase("_in");

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

        return gameMap;
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
            Transform transform;
            (transform = lightObj.transform).SetParent(lightsParent.transform, false);
            lightObj.color = new Color(light.color[0], light.color[1], light.color[2]);
            lightObj.range = light.range; //?? whatsup doddler?
            lightObj.intensity = light.range;
            lightObj.bounceIntensity = 1.5f;
            lightObj.shadows = LightShadows.Soft;
#if UNITY_EDITOR
            lightObj.lightmapBakeType = LightmapBakeType.Baked;
#endif
            var position = new Vector3(light.pos[0] + mapSize.x, -light.pos[1], light.pos[2] + mapSize.y);
            transform.position = position;
            lightObj.gameObject.isStatic = true;
            // var intensity = Mathf.Max(3, (light.range + 1) / 3f);

            index++;
        }
    }

    [MenuItem("Map/Recreate Light Probes")]
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

    [MenuItem("Map/Adjust Lights")]
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

    private static void CreateLightProbes(Transform parent, Altitude altitude, Transform[] lights)
    {
        var LightProbes = new GameObject("LightProbes");
        LightProbes.transform.SetParent(parent, false);
        var lightProbeGroup = LightProbes.AddComponent<LightProbeGroup>();

        // positions.Add(new Vector3(altitude.getWidth() / 2f, 0, altitude.getHeight() / 2f));
        // foreach (var light in lights)
        // {
        //     var groundHeight = (float)altitude.GetCellHeight(light.transform.position.x, light.transform.position.z);
        //     // var groundProbe = new Vector3(light.transform.position.x, 2.5f, light.transform.position.z);
        //     // var groundProbe = new Vector3(light.transform.position.x, light.transform.position.y / 2f, light.transform.position.z);
        //     // var groundProbe = new Vector3(light.transform.position.x, groundHeight + 0.5f, light.transform.position.z);
        //     var lightProbe = light.transform.position;
        //     // if (groundProbe != Vector3.zero)
        //     // {
        //     //     positions.Add(groundProbe);
        //     // }
        //
        //     // if (lightProbe != Vector3.zero)
        //     // {
        //     //     positions.Add(lightProbe);
        //     // }
        // }

        var positions = new List<Vector3>();
        for (var x = 0; x < altitude.getWidth() / 2; x++)
        {
            for (var y = 0; y < altitude.getHeight() / 2; y++)
            {
                var height = (float)altitude.GetCellHeight(x * 2, y * 2);
                var ground = new Vector3(x * 2, height, y * 2);
                var middle = new Vector3(x * 2, (height + 2), y * 2);
                var top = new Vector3(x * 2, height + 4, y * 2);

                positions.Add(ground);
                positions.Add(middle);
                positions.Add(top);
            }
        }

        lightProbeGroup.probePositions = positions.ToArray();
    }
}