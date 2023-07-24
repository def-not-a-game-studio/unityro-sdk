using System;
using Core.Path;
using ROIO.Models.FileTypes;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class GameMap : MonoBehaviour {
    [SerializeField] private Vector2Int _size;
    public Vector2Int Size => _size;

    [SerializeField] [HideInInspector] public Light DirectionalLight;
    [SerializeField] [HideInInspector] private RSW.LightInfo LightInfo;
    [SerializeField] [HideInInspector] private Altitude Altitude;

    private PathFinder PathFinder;

    private void Awake() {
        InitWorldLight();
        InitPathFinder();
    }

    private void InitPathFinder() {
        PathFinder = gameObject.GetOrAddComponent<PathFinder>();
        PathFinder.SetMap(Altitude);
    }

    private void InitWorldLight() {
        if (DirectionalLight == null) {
            var worldLightGameObject = new GameObject("Light");
            worldLightGameObject.transform.SetParent(gameObject.transform);
            DirectionalLight = worldLightGameObject.GetOrAddComponent<Light>();
        }

        SetupWorldLight();
    }

    private void SetupWorldLight() {
        if (LightInfo == null) {
            return;
        }

        DirectionalLight.type = LightType.Directional;
        DirectionalLight.shadows = LightShadows.Soft;
        DirectionalLight.shadowStrength = 1f;
        DirectionalLight.intensity = 1f + LightInfo.intensity;

        var rotation = Quaternion.Euler(LightInfo.longitude, LightInfo.latitude, 0);
        DirectionalLight.transform.rotation = rotation;

        var ambient = Color.white;
        var diffuse = Color.white;
        if (LightInfo.ambient.Length > 0) {
            ambient = new Color(LightInfo.ambient[0], LightInfo.ambient[1], LightInfo.ambient[2]);
        }

        if (LightInfo.diffuse.Length > 0) {
            diffuse = new Color(LightInfo.diffuse[0], LightInfo.diffuse[1], LightInfo.diffuse[2]);
        }

        RenderSettings.ambientMode = AmbientMode.Skybox;
        RenderSettings.ambientIntensity = 1f;
        RenderSettings.sun = DirectionalLight;
        DirectionalLight.color = ambient;

        Shader.SetGlobalColor("_RoAmbientColor", ambient);
        Shader.SetGlobalColor("_RoDiffuseColor", diffuse);
    }

    public void SetMapSize(int width, int height) {
        _size = new Vector2Int(width, height);
    }

    public void SetMapLightInfo(RSW.LightInfo lightInfo) {
        LightInfo = lightInfo;

        if (DirectionalLight == null) {
            InitWorldLight();
        }

        SetupWorldLight();
    }

    public void SetMapAltitude(Altitude altitude) {
        Altitude = altitude;
        PathFinder?.SetMap(Altitude);
    }

    public PathFinder GetPathFinder() {
        if (PathFinder == null) {
            InitPathFinder();
        }

        return PathFinder;
    }
}