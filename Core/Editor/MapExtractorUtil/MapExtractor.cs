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
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }

            AssetDatabase.Refresh();

            foreach (var texture in texturePaths)
            {
                var importer = AssetImporter.GetAtPath(texture) as TextureImporter;
                if  (importer == null) continue;
                
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

            try
            {
                AssetDatabase.StartAssetEditing();
                ExtractClonedModels(mapObject, Path.Combine(ROMapExtractor.GetBasePath(), mapName, "models"));
                AssetDatabase.StopAssetEditing();

                ExtractGround(mapObject, mapName);
                ExtractWater(mapObject, mapName);

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

                var models = mapObject.transform.Find("_Models");
                var originals = models.transform.Find("_Originals");
                var cloned = models.transform.Find("_Cloned");

                foreach (var child in originals.transform.GetComponentsInChildren<Transform>())
                {
                    if (child.transform.GetComponent(typeof(NodeAnimation)) == null)
                    {
                        child.gameObject.isStatic = true;
                    }
                }

                foreach (var child in cloned.transform.GetComponentsInChildren<Transform>())
                {
                    if (child.transform.GetComponent(typeof(NodeAnimation)) == null)
                    {
                        child.gameObject.isStatic = true;
                    }
                }

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

                StaticOcclusionCulling.Compute();
                Lightmapping.Bake();

                EditorSceneManager.SaveOpenScenes();

                var original = EditorBuildSettings.scenes;
                var newSettings = new EditorBuildSettingsScene[original.Length + 1];
                Array.Copy(original, newSettings, original.Length);
                var sceneToAdd = new EditorBuildSettingsScene(mapScene.path, true);
                newSettings[newSettings.Length - 1] = sceneToAdd;
                EditorBuildSettings.scenes = newSettings;

                EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");

                //ImportAssetAndApplyAddressableGroup(localPath, typeof(GameObject));
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
    }
}