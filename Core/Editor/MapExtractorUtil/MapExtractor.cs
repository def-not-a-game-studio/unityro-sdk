using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityRO.Core.Editor.MapExtractorUtil
{
    public partial class MapExtractor
    {
        public static void SaveMap(GameObject mapObject)
        {
            var mapName = Path.GetFileNameWithoutExtension(mapObject.name);
            var localPath = Path.Combine(ROMapExtractor.GetBasePath());
            
            Directory.CreateDirectory(localPath);
            
            var texturePaths = new List<string>();

            try
            {
                AssetDatabase.StartAssetEditing();
                texturePaths = ExtractOriginalModels(mapObject, Path.Combine(ROMapExtractor.GetBasePath(), mapName, "models"));
                AssetDatabase.StopAssetEditing();
                AssetDatabase.Refresh();
                AssetDatabase.StartAssetEditing();
                ExtractClonedModels(mapObject, Path.Combine(ROMapExtractor.GetBasePath(), mapName, "models"));
                AssetDatabase.StopAssetEditing();

                ExtractGround(mapObject, mapName);
                ExtractWater(mapObject, mapName);

                AssetDatabase.Refresh();
                
                foreach (var importer in texturePaths.Select(AssetImporter.GetAtPath).OfType<TextureImporter>())
                {
                    importer.alphaIsTransparency = true;
                    importer.wrapMode = TextureWrapMode.Repeat;
                    importer.filterMode = FilterMode.Bilinear;
                    importer.mipmapEnabled = true;
                    importer.mipMapBias = 0.5f;
                    var textureSettings = new TextureImporterSettings();
                    importer.ReadTextureSettings(textureSettings);

                    importer.SetTextureSettings(textureSettings);
                    importer.SaveAndReimport();
                }
                
                AssetDatabase.Refresh();

                var meshesPathes = DataUtility
                    .GetFilesFromDir(Path.Combine(ROMapExtractor.GetBasePath(), mapName, "models"))
                    .Where(it => Path.GetExtension(it) == ".mat")
                    .Select(it => Path.ChangeExtension(it, ""))
                    .ToList();
                foreach (var mesh in meshesPathes)
                {
                    var material = AssetDatabase.LoadAssetAtPath<Material>(mesh + "mat");
                    var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(mesh + "png");

                    material.SetTexture("_MainTex", texture);
                }
                
                GroupModels(mapObject);

                AssetDatabase.Refresh();

                localPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(localPath, $"{mapName}.prefab"));
                PrefabUtility.SaveAsPrefabAssetAndConnect(mapObject, localPath, InteractionMode.AutomatedAction);

                var defaultScene = EditorSceneManager.GetActiveScene();
                var mapScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
                mapScene.name = mapName;
                var lightingSettings = AssetDatabase.LoadAssetAtPath<LightingSettings>("Assets/Configuration/Lighting Settings.lighting");
                Lightmapping.lightingSettings = lightingSettings;

                var volume = Resources.Load<GameObject>("Global Volume");
                var volumePrefab = PrefabUtility.InstantiatePrefab(volume) as GameObject;
                SceneManager.MoveGameObjectToScene(mapObject, mapScene);
                SceneManager.MoveGameObjectToScene(volumePrefab, mapScene);
                EditorSceneManager.MarkAllScenesDirty();
                EditorSceneManager.SaveScene(mapScene, $"Assets/3rdparty/unityro-resources/Scenes/{mapName}.unity");
                EditorSceneManager.CloseScene(defaultScene, true);

                // StaticOcclusionCulling.Compute();
                // Lightmapping.Bake();
                
                // EditorSceneManager.SaveOpenScenes();
                //
                // var original = EditorBuildSettings.scenes;
                // var newSettings = new EditorBuildSettingsScene[original.Length + 1];
                // Array.Copy(original, newSettings, original.Length);
                // var sceneToAdd = new EditorBuildSettingsScene(mapScene.path, true);
                // newSettings[newSettings.Length - 1] = sceneToAdd;
                // EditorBuildSettings.scenes = newSettings;
                //
                // EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");

                //ImportAssetAndApplyAddressableGroup(localPath, typeof(GameObject));
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
    }
}