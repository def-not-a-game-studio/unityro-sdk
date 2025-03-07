﻿using System;
using System.Collections.Generic;
using Core.Path;
using ROIO.Models.FileTypes;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class GameMap : MonoBehaviour
{
    [SerializeField] private Vector2Int _size;
    public Vector2Int Size => _size;

    [SerializeField] [HideInInspector] public Light DirectionalLight;

    [field: SerializeField]
    [HideInInspector]
    public Altitude Altitude { get; private set; }

    [SerializeField] [HideInInspector] private List<MapSound> Sounds;
    [SerializeField] private RSW.LightInfo LightInfo;
    [SerializeField] private Color Ambient;
    [SerializeField] private Color Diffuse;
    [field: SerializeField] public Texture2D ROLightMap { get; private set; }

    private PathFinder PathFinder;

    private void Awake()
    {
        InitWorldLight();
        InitPathFinder();
    }

    private void InitPathFinder()
    {
        PathFinder = gameObject.GetOrAddComponent<PathFinder>();
        PathFinder.SetMap(Altitude);
    }

    private void InitWorldLight()
    {
        if (DirectionalLight == null)
        {
            var worldLightGameObject = new GameObject("Light");
            worldLightGameObject.transform.SetParent(gameObject.transform);
            DirectionalLight = worldLightGameObject.GetOrAddComponent<Light>();
            SetupWorldLight();
        }
    }

    private void SetupWorldLight()
    {
        if (LightInfo == null)
        {
            return;
        }

        GraphicsSettings.lightsUseColorTemperature = true;
        GraphicsSettings.lightsUseLinearIntensity = true;

        DirectionalLight.type = LightType.Directional;
#if UNITY_EDITOR
        DirectionalLight.lightmapBakeType = LightmapBakeType.Mixed;
        DirectionalLight.shadowStrength = .7f;
#endif
        DirectionalLight.shadows = LightShadows.Soft;

        var rotation = Quaternion.Euler(LightInfo.longitude, LightInfo.latitude, 0);
        DirectionalLight.transform.rotation = rotation;

        var ambient = Color.white;
        var diffuse = Color.white;
        if (LightInfo.ambient.Length > 0)
        {
            ambient = new Color(LightInfo.ambient[0], LightInfo.ambient[1], LightInfo.ambient[2]);
        }

        if (LightInfo.diffuse.Length > 0)
        {
            diffuse = new Color(LightInfo.diffuse[0], LightInfo.diffuse[1], LightInfo.diffuse[2]);
        }

        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientIntensity = 1f;
        var color = new Color(ambient.r + diffuse.r * LightInfo.intensity, ambient.g + diffuse.g * LightInfo.intensity,
            ambient.b + diffuse.b * LightInfo.intensity, ambient.a);
        RenderSettings.ambientLight = color;
        DirectionalLight.color = diffuse;
        DirectionalLight.intensity = LightInfo.intensity;

        Ambient = ambient;
        Diffuse = diffuse;

        Shader.SetGlobalColor("_RoAmbientColor", Ambient);
        Shader.SetGlobalColor("_RoDiffuseColor", Diffuse);
    }

    public void SetMapSize(int width, int height)
    {
        _size = new Vector2Int(width, height);
    }

    public void SetMapLightInfo(RSW.LightInfo lightInfo)
    {
        LightInfo = lightInfo;

        if (DirectionalLight == null)
        {
            InitWorldLight();
        }
    }

    public void SetMapAltitude(Altitude altitude)
    {
        Altitude = altitude;
        PathFinder?.SetMap(Altitude);
    }

    public void SetMapSounds(List<MapSound> sounds)
    {
        Sounds = sounds;
    }

    public PathFinder GetPathFinder()
    {
        if (PathFinder == null)
        {
            InitPathFinder();
        }

        return PathFinder;
    }

    public void Update()
    {
        var now = Time.realtimeSinceStartup;

        foreach (var p in Sounds)
        {
            if (p.playAt <= now && p.source != null)
            {
                p.source.Play();
                p.playAt = now + p.info.cycle;
            }
        }
    }
}