using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ROIO;
using UnityEditor;
using UnityEngine;

namespace UnityRO.Core.Editor.MapExtractorUtil
{
    public partial class MapExtractor
    {
        private const string LOADED_TEXTURE_PREFIX = "data/texture/maptexture@";

        private static List<string> ExtractOriginalModels(GameObject mapObject, string overridePath = null)
        {
            var originalMeshes = mapObject.transform.FindRecursive("_Originals");
            // we need to set the new prefabs to another parent so we can delete the old meshes
            var newOriginalParent = new GameObject("_Original");
            newOriginalParent.transform.SetParent(originalMeshes.transform.parent);

            var children = originalMeshes.transform.GetChildren();
            var texturePaths = new List<string>();

            var i = 0;
            foreach (var mesh in children)
            {
                // var progress = i * 1f / originalMeshes.transform.childCount;
                // if (EditorUtility.DisplayCancelableProgressBar("UnityRO", $"Saving model meshes - {progress * 100}%", progress))
                // {
                //     break;
                // }

                mesh.gameObject.SetActive(true);

                try
                {
                    var textures = ExtractModelMesh(mesh.gameObject, newOriginalParent.transform, overridePath);
                    texturePaths.AddRange(textures);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    Debug.LogError($"Error extracting model {mesh.gameObject.name}");
                }
                finally
                {
                    i++;
                }
            }

            // EditorUtility.ClearProgressBar();
            return texturePaths;
        }

        private static void ExtractClonedModels(GameObject mapObject, string overridePath = null)
        {
            var modelsParent = mapObject.transform.FindRecursive("_Models");
            var copiesParent = mapObject.transform.FindRecursive("_Copies");
            var originalsParents = mapObject.transform.FindRecursive("_Originals");

            var originalPrefabs = new Dictionary<string, GameObject>();

            // Query for the original prefabs
            for (var i = 0; i < originalsParents.transform.childCount; i++)
            {
                var mesh = originalsParents.transform.GetChild(i);
                string meshPathWithoutExtension;
                if (Path.GetExtension(mesh.name) == "")
                {
                    meshPathWithoutExtension = mesh.name;
                }
                else
                {
                    meshPathWithoutExtension = mesh.name.Substring(0, mesh.name.IndexOf(Path.GetExtension(mesh.name)));
                }

                var meshPath = "";
                if (overridePath != null)
                {
                    meshPath = Path.Combine(overridePath, meshPathWithoutExtension);
                }
                else
                {
                    meshPath = Path.Combine(ROMapExtractor.GetBasePath(), "data", "model", meshPathWithoutExtension);
                }

                meshPathWithoutExtension = meshPathWithoutExtension.Replace('\\', Path.DirectorySeparatorChar);
                meshPath = meshPath.Replace('\\', Path.DirectorySeparatorChar);

                var prefab = AssetDatabase.LoadAssetAtPath(meshPath + ".prefab", typeof(GameObject)) as GameObject;
                if (!originalPrefabs.ContainsKey(meshPathWithoutExtension))
                {
                    originalPrefabs.Add(meshPathWithoutExtension, prefab);
                }
            }

            var cloned = new GameObject("_Cloned");
            cloned.transform.SetParent(modelsParent.transform);

            for (var i = 0; i < copiesParent.transform.childCount; i++)
            {
                var mesh = copiesParent.transform.GetChild(i);
                var originalMeshName = mesh.name.Substring(0, mesh.name.IndexOf("(Clone)")).Replace('\\', Path.DirectorySeparatorChar);
                var originalMeshExtension = Path.GetExtension(originalMeshName);
                var meshPathWithoutExtension = mesh.name.Substring(0, originalMeshName.IndexOf(originalMeshExtension))
                    .Replace('\\', Path.DirectorySeparatorChar);

                if (originalPrefabs.TryGetValue(meshPathWithoutExtension, out GameObject original))
                {
                    var prefab = PrefabUtility.InstantiatePrefab(original, cloned.transform) as GameObject;
                    prefab.transform.SetPositionAndRotation(mesh.transform.position, mesh.transform.rotation);
                    prefab.transform.localScale = mesh.transform.localScale;
                }
                else
                {
                    Debug.LogError($"Couldn't find original model {meshPathWithoutExtension}");
                }
            }

            GameObject.DestroyImmediate(copiesParent.gameObject);
            GameObject.DestroyImmediate(mapObject.transform.FindRecursive("_Original").gameObject);
        }

        private static void GroupModels(GameObject mapObject)
        {
            var modelsParent = mapObject.transform.FindRecursive("_Models");
            var originals =  mapObject.transform.FindRecursive("_Originals");
            var clones =  mapObject.transform.FindRecursive("_Cloned");
            var models = originals.GetChildren().Concat(clones.GetChildren());
            
            var staticParent = new GameObject("_Static");
            staticParent.transform.SetParent(modelsParent.transform);
            var dynamicParent = new GameObject("_Dynamic");
            dynamicParent.transform.SetParent(modelsParent.transform);

            foreach (var model in models)
            {
                model.transform.SetParent(model.childCount > 0 ? dynamicParent.transform : staticParent.transform, true);
            }

            foreach (var model in staticParent.transform.GetChildren())
            {
                model.gameObject.isStatic = true;
            }
            
            GameObject.DestroyImmediate(originals.gameObject);
            GameObject.DestroyImmediate(clones.gameObject);
        }

        private static List<string> ExtractModelMesh(GameObject mesh, Transform overrideParent = null, string overridePath = null)
        {
            var nodeTexturesPath = new List<string>();
            string meshPathWithoutExtension;
            if (Path.GetExtension(mesh.name).Length == 0)
            {
                meshPathWithoutExtension = mesh.name;
            }
            else
            {
                meshPathWithoutExtension = mesh.name.Substring(0, mesh.name.IndexOf(Path.GetExtension(mesh.name)));
            }

            string meshPath;
            if (mesh.name.Contains("data/model"))
            {
                meshPath = Path.Combine(ROMapExtractor.GetBasePath(), meshPathWithoutExtension);
            }
            else
            {
                meshPath = Path.Combine(ROMapExtractor.GetBasePath(), "data", "model", meshPathWithoutExtension);
            }

            if (overridePath != null)
            {
                meshPath = Path.Combine(overridePath, meshPathWithoutExtension);
            }

            meshPath = meshPath.Replace('\\', Path.DirectorySeparatorChar);

            Directory.CreateDirectory(meshPath);

            if (File.Exists(meshPath + ".prefab"))
            {
                var prefabObject = AssetDatabase.LoadAssetAtPath(meshPath + ".prefab", typeof(GameObject)) as GameObject;
                var prefab = PrefabUtility.InstantiatePrefab(prefabObject, mesh.transform.parent) as GameObject;
                prefab.transform.SetPositionAndRotation(mesh.transform.position, mesh.transform.rotation);
                prefab.transform.localScale = mesh.transform.localScale;
                if (overrideParent != null)
                {
                    prefab.transform.SetParent(overrideParent);
                }
            }
            else
            {
                try
                {
                    if (mesh.TryGetComponent<MeshCollider>(out var collider))
                    {
                        AssetDatabase.CreateAsset(collider.sharedMesh, $"{meshPath}_collider.asset");
                    }

                    var nodes = mesh.GetComponentsInChildren<NodeProperties>();
                    if (nodes.Length > 0)
                    {
                        foreach (var node in nodes)
                        {
                            if (!node.TryGetComponent<NodeAnimation>(out var anim))
                            {
                                //GameObjectUtility.SetStaticEditorFlags(node.gameObject, StaticEditorFlags.BatchingStatic);
                                node.gameObject.isStatic = true;
                            }

                            var filter = node.GetComponent<MeshFilter>();
                            var material = node.GetComponent<MeshRenderer>().material;

                            var nodeName = node.mainName.Length == 0 ? "node" : node.mainName;
                            var nodePath = Path.Combine(meshPath, $"{nodeName}_{node.nodeId}").Replace('\\', Path.DirectorySeparatorChar);
                            var partPath = AssetDatabase.GenerateUniqueAssetPath($"{nodePath}.asset");
                            var materialPath = AssetDatabase.GenerateUniqueAssetPath($"{nodePath}.mat");

                            var texture = FileManager.Load($"data/texture/{node.textureName}") as Texture2D;
                            if (texture != null)
                            {
                                var texturePath = $"{nodePath}.png";
                                nodeTexturesPath.Add(texturePath);
                                File.WriteAllBytes(texturePath, texture.EncodeToPNG());
                            }
                            else
                            {
                                Debug.LogError($"Texture data/texture/{node.textureName} not found");
                            }

                            AssetDatabase.CreateAsset(filter.sharedMesh, partPath);
                            AssetDatabase.CreateAsset(material, materialPath);
                        }
                    }
                    else
                    {
                        var filter = mesh.GetComponent<MeshFilter>();
                        var materials = mesh.GetComponent<MeshRenderer>().materials;

                        var partPath = AssetDatabase.GenerateUniqueAssetPath($"{meshPath}.asset");
                        for (var index = 0; index < materials.Length; index++)
                        {
                            var material = materials[index];
                            var texture = FileManager.Load($"data/texture/{material.mainTexture.name[LOADED_TEXTURE_PREFIX.Length..]}") as Texture2D;
                            var materialPath = AssetDatabase.GenerateUniqueAssetPath($"{meshPath}_mat_{index}.mat");

                            if (texture != null)
                            {
                                var texturePath = $"{meshPath}_mat_{index}.png";
                                nodeTexturesPath.Add(texturePath);
                                File.WriteAllBytes(texturePath, texture.EncodeToPNG());
                            }
                            else
                            {
                                Debug.LogError($"Texture data/texture/{material.mainTexture.name} not found");
                            }

                            AssetDatabase.CreateAsset(material, materialPath);
                        }

                        AssetDatabase.CreateAsset(filter.sharedMesh, partPath);
                    }

                    meshPath = AssetDatabase.GenerateUniqueAssetPath(meshPath + ".prefab");
                    PrefabUtility.SaveAsPrefabAssetAndConnect(mesh, meshPath, InteractionMode.AutomatedAction);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed extracting model {mesh.name}");
                    Debug.LogError(e);
                }
            }

            return nodeTexturesPath;
        }
    }
}