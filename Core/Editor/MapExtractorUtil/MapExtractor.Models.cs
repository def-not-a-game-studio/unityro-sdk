using System;
using System.Collections.Generic;
using System.IO;
using ROIO;
using UnityEditor;
using UnityEngine;

namespace UnityRO.Core.Editor.MapExtractorUtil
{
    public partial class MapExtractor
    {
        // TODO explore this one
        // public void AdvancedMerge()
        // {
        //     // All our children (and us)
        //     MeshFilter[] filters = GetComponentsInChildren(false);
        //
        //     // All the meshes in our children (just a big list)
        //     List materials = new List();
        //     MeshRenderer[] renderers = GetComponentsInChildren(false); // <-- you can optimize this
        //     foreach (MeshRenderer renderer in renderers)
        //     {
        //         if (renderer.transform == transform)
        //             continue;
        //         Material[] localMats = renderer.sharedMaterials;
        //         foreach (Material localMat in localMats)
        //             if (!materials.Contains(localMat))
        //                 materials.Add(localMat);
        //     }
        //
        //     // Each material will have a mesh for it.
        //     List submeshes = new List();
        //     foreach (Material material in materials)
        //     {
        //         // Make a combiner for each (sub)mesh that is mapped to the right material.
        //         List combiners = new List();
        //         foreach (MeshFilter filter in filters)
        //         {
        //             if (filter.transform == transform) continue;
        //             // The filter doesn't know what materials are involved, get the renderer.
        //             MeshRenderer renderer = filter.GetComponent(); // <-- (Easy optimization is possible here, give it a try!)
        //             if (renderer == null)
        //             {
        //                 Debug.LogError(filter.name + " has no MeshRenderer");
        //                 continue;
        //             }
        //
        //             // Let's see if their materials are the one we want right now.
        //             Material[] localMaterials = renderer.sharedMaterials;
        //             for (int materialIndex = 0; materialIndex < localMaterials.Length; materialIndex++)
        //             {
        //                 if (localMaterials[materialIndex] != material)
        //                     continue;
        //                 // This submesh is the material we're looking for right now.
        //                 CombineInstance ci = new CombineInstance();
        //                 ci.mesh = filter.sharedMesh;
        //                 ci.subMeshIndex = materialIndex;
        //                 ci.transform = Matrix4x4.identity;
        //                 combiners.Add(ci);
        //             }
        //         }
        //
        //         // Flatten into a single mesh.
        //         Mesh mesh = new Mesh();
        //         mesh.CombineMeshes(combiners.ToArray(), true);
        //         submeshes.Add(mesh);
        //     }
        //
        //     // The final mesh: combine all the material-specific meshes as independent submeshes.
        //     List finalCombiners = new List();
        //     foreach (Mesh mesh in submeshes)
        //     {
        //         CombineInstance ci = new CombineInstance();
        //         ci.mesh = mesh;
        //         ci.subMeshIndex = 0;
        //         ci.transform = Matrix4x4.identity;
        //         finalCombiners.Add(ci);
        //     }
        //
        //     Mesh finalMesh = new Mesh();
        //     finalMesh.CombineMeshes(finalCombiners.ToArray(), false);
        //     myMeshFilter.sharedMesh = finalMesh;
        //     Debug.Log("Final mesh has " + submeshes.Count + " materials.");
        // }

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
                var progress = i * 1f / originalMeshes.transform.childCount;
                if (EditorUtility.DisplayCancelableProgressBar("UnityRO", $"Saving model meshes - {progress * 100}%",
                        progress))
                {
                    break;
                }

                mesh.gameObject.SetActive(true);

                try
                {
                    var textures = ExtractModelMesh(mesh.gameObject, newOriginalParent.transform, overridePath);
                    texturePaths.AddRange(textures);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    Debug.LogError($"Error extracting model {mesh.gameObject.name}");
                }
                finally
                {
                    i++;
                }
            }

            EditorUtility.ClearProgressBar();
            return texturePaths;
        }

        private static void ExtractClonedModels(GameObject mapObject, string overridePath = null)
        {
            var models = mapObject.transform.FindRecursive("_Models");
            var clonedMeshes = mapObject.transform.FindRecursive("_Copies");
            var originalMeshes = mapObject.transform.FindRecursive("_Originals");
            var originalPrefabs = new Dictionary<string, GameObject>();

            // Query for the original prefabs
            for (int i = 0; i < originalMeshes.transform.childCount; i++)
            {
                var mesh = originalMeshes.transform.GetChild(i);
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

                var prefab = AssetDatabase.LoadAssetAtPath(meshPath + ".prefab", typeof(GameObject)) as GameObject;
                if (!originalPrefabs.ContainsKey(meshPathWithoutExtension))
                {
                    originalPrefabs.Add(meshPathWithoutExtension, prefab);
                }
            }

            var cloned = new GameObject("_Cloned");
            cloned.transform.SetParent(models.transform);

            for (int i = 0; i < clonedMeshes.transform.childCount; i++)
            {
                var mesh = clonedMeshes.transform.GetChild(i);
                var originalMeshName = mesh.name.Substring(0, mesh.name.IndexOf("(Clone)"));
                var meshPathWithoutExtension =
                    mesh.name.Substring(0, originalMeshName.IndexOf(Path.GetExtension(originalMeshName)));

                var prefab =
                    PrefabUtility.InstantiatePrefab(originalPrefabs[meshPathWithoutExtension], cloned.transform)
                        as GameObject;
                prefab.transform.SetPositionAndRotation(mesh.transform.position, mesh.transform.rotation);
                prefab.transform.localScale = mesh.transform.localScale;
            }

            GameObject.DestroyImmediate(clonedMeshes.gameObject);
            //GameObject.DestroyImmediate(originalMeshes.gameObject);
        }

        public static List<string> ExtractModelMesh(GameObject mesh, Transform overrideParent = null, string overridePath = null)
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
                        var partPath =
                            AssetDatabase.GenerateUniqueAssetPath(Path.Combine(meshPath,
                                $"{nodeName}_{node.nodeId}.asset"));
                        var materialPath =
                            AssetDatabase.GenerateUniqueAssetPath(Path.Combine(meshPath, $"{nodeName}_{node.nodeId}.mat"));

                        var texture = FileManager.Load($"data/texture/{node.textureName}") as Texture2D;
                        if (texture is not null)
                        {
                            var texturePath = Path.Combine(meshPath, $"{nodeName}_{node.nodeId}.png");
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

                    meshPath = AssetDatabase.GenerateUniqueAssetPath(meshPath + ".prefab");
                    PrefabUtility.SaveAsPrefabAssetAndConnect(mesh, meshPath, InteractionMode.AutomatedAction);
                }
                catch (Exception)
                {
                    Debug.LogError($"Failed extracting model {mesh.name}");
                }
            }

            return nodeTexturesPath;
        }
    }
}