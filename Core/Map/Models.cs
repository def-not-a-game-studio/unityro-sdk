using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ROIO;
using ROIO.Models.FileTypes;
using ROIO.Utils.Extensions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;

public class Models
{
    private List<RSM.CompiledModel> models;
    
    private Material shadowOnlyMaterial;
    private Material doubleSidedMaterial;
    private Material transparentMaterial;
    private Material defaultMaterial;

    public Models(List<RSM.CompiledModel> models)
    {
        this.models = models;
    }

    public async Task BuildMeshesAsync(Action<float> OnProgress, bool ignorePrefabs, Vector2Int mapSize)
    {
        GameObject modelsParent = new GameObject("_Models");
        GameObject originals = new GameObject("_Originals");
        GameObject copies = new GameObject("_Copies");
        Dictionary<string, GameObject> prefabDict = new Dictionary<string, GameObject>();
        modelsParent.transform.SetParent(GameObject.FindObjectOfType<GameMap>().transform);
        originals.transform.SetParent(modelsParent.transform);
        copies.transform.SetParent(modelsParent.transform);
        
        shadowOnlyMaterial = Resources.Load<Material>("Materials/ShadowOnlyMaterial");
        doubleSidedMaterial = Resources.Load<Material>("Materials/ModelMaterial2Sided");
        transparentMaterial = Resources.Load<Material>("Materials/ModelMaterialTransparent");
        defaultMaterial = Resources.Load<Material>("Materials/ModelMaterial");

        int nodeId = 0;

        if (!ignorePrefabs)
        {
            var tasks = new List<Task<GameObject>>();
            foreach (var model in models)
            {
                var filenameWithoutExtension = model.rsm.filename.Substring(0, model.rsm.filename.IndexOf(".rsm"));
                tasks.Add(Addressables
                    .LoadAssetAsync<GameObject>(Path.Combine("data", "model", $"{filenameWithoutExtension}.prefab")
                        .SanitizeForAddressables()).Task);
            }

            var prefabs = await Task.WhenAll(tasks);
            for (int i = 0; i < prefabs.Length; i++)
            {
                var prefab = prefabs[i];
                var model = models[i];

                if (prefab != null)
                {
                    prefabDict.Add(model.rsm.filename, prefab);
                }
            }
        }

        for (var index = 0; index < models.Count; index++)
        {
            OnProgress?.Invoke(index / (float)models.Count);

            RSM.CompiledModel model = models[index];

            GameObject modelObj;
            if (prefabDict.TryGetValue(model.rsm.filename, out GameObject prefab))
            {
                modelObj = GameObject.Instantiate(prefab, originals.transform);
                modelObj.name = model.rsm.name;
            }
            else
            {
                modelObj = new GameObject(model.rsm.filename);
                modelObj.transform.SetParent(originals.transform);

                nodeId = CreateOriginalModel(nodeId, model, modelObj);
            }

            //instantiate model
            for (int i = 0; i < model.rsm.instances.Count; i++)
            {
                GameObject instanceObj;
                if (i == model.rsm.instances.Count - 1)
                {
                    //last instance
                    instanceObj = modelObj;
                }
                else
                {
                    instanceObj = UnityEngine.Object.Instantiate(modelObj);
                    instanceObj.transform.SetParent(copies.transform);
                    instanceObj.name += "[" + i + "]";
                }

                RSW.ModelDescriptor descriptor = model.rsm.instances[i];

                instanceObj.transform.Rotate(Vector3.forward, -descriptor.rotation[2]);
                instanceObj.transform.Rotate(Vector3.right, -descriptor.rotation[0]);
                instanceObj.transform.Rotate(Vector3.up, descriptor.rotation[1]);

                Vector3 scale = new Vector3(descriptor.scale[0], -descriptor.scale[1], descriptor.scale[2]);
                instanceObj.transform.localScale = scale;

                //avoid z fighting between models
                // float xRandom = UnityEngine.Random.Range(-0.002f, 0.002f);
                // float yRandom = UnityEngine.Random.Range(-0.002f, 0.002f);
                // float zRandom = UnityEngine.Random.Range(-0.002f, 0.002f);
                float xRandom = 0f, yRandom = 0f, zRandom = 0f;

                Vector3 position = new Vector3(descriptor.position[0] + xRandom, descriptor.position[1] + yRandom,
                    descriptor.position[2] + zRandom);
                position.x += mapSize.x;
                position.y *= -1;
                position.z += mapSize.y;
                instanceObj.transform.position = position;

                //setup hierarchy
                var propertiesComponents = instanceObj.GetComponentsInChildren<NodeProperties>();
                foreach (var properties in propertiesComponents)
                {
                    if (properties.isChild)
                    {
                        var nodeParent = instanceObj.transform.FindRecursive(properties.parentName);
                        properties.transform.parent = nodeParent;
                    }
                }

                instanceObj.SetActive(true);
            }
            //yield return null;
        }
        //yield return null;
    }

    public IEnumerator BuildMeshes(Action<float> OnProgress, bool ignorePrefabs, Vector2Int mapSize)
    {
        GameObject modelsParent = new GameObject("_Models");
        GameObject originals = new GameObject("_Originals");
        GameObject copies = new GameObject("_Copies");
        Dictionary<string, GameObject> prefabDict = new Dictionary<string, GameObject>();
        modelsParent.transform.SetParent(GameObject.FindObjectOfType<GameMap>().transform);
        originals.transform.SetParent(modelsParent.transform);
        copies.transform.SetParent(modelsParent.transform);

        int nodeId = 0;
        if (!ignorePrefabs)
        {
            for (int index = 0; index < models.Count; index++)
            {
                RSM.CompiledModel model = models[index];
                var filenameWithoutExtension = model.rsm.filename.Substring(0, model.rsm.filename.IndexOf(".rsm"));
                var prefabRequest =
                    Addressables.LoadAssetAsync<GameObject>(Path
                        .Combine("data", "model", $"{filenameWithoutExtension}.prefab")
                        .SanitizeForAddressables());
                while (!prefabRequest.IsDone)
                {
                    yield return prefabRequest;
                }

                if (prefabRequest.Result != null)
                {
                    prefabDict.Add(model.rsm.filename, prefabRequest.Result);
                }
            }
        }

        for (var index = 0; index < models.Count; index++)
        {
            OnProgress.Invoke(index / (float)models.Count);

            RSM.CompiledModel model = models[index];

            GameObject modelObj;
            if (prefabDict.TryGetValue(model.rsm.filename, out GameObject prefab))
            {
                modelObj = GameObject.Instantiate(prefab, originals.transform);
                modelObj.name = model.rsm.filename;
            }
            else
            {
                modelObj = new GameObject(model.rsm.filename);
                modelObj.transform.SetParent(originals.transform);

                nodeId = CreateOriginalModel(nodeId, model, modelObj);
            }

            //instantiate model
            for (int i = 0; i < model.rsm.instances.Count; i++)
            {
                GameObject instanceObj;
                if (i == model.rsm.instances.Count - 1)
                {
                    //last instance
                    instanceObj = modelObj;
                }
                else
                {
                    instanceObj = UnityEngine.Object.Instantiate(modelObj);
                    instanceObj.transform.SetParent(copies.transform);
                    instanceObj.name += "[" + i + "]";
                }

                RSW.ModelDescriptor descriptor = model.rsm.instances[i];

                instanceObj.transform.Rotate(Vector3.forward, -descriptor.rotation[2]);
                instanceObj.transform.Rotate(Vector3.right, -descriptor.rotation[0]);
                instanceObj.transform.Rotate(Vector3.up, descriptor.rotation[1]);

                Vector3 scale = new Vector3(descriptor.scale[0], -descriptor.scale[1], descriptor.scale[2]);
                instanceObj.transform.localScale = scale;

                //avoid z fighting between models
                float xRandom = UnityEngine.Random.Range(-0.002f, 0.002f);
                float yRandom = UnityEngine.Random.Range(-0.002f, 0.002f);
                float zRandom = UnityEngine.Random.Range(-0.002f, 0.002f);

                Vector3 position = new Vector3(descriptor.position[0] + xRandom, descriptor.position[1] + yRandom,
                    descriptor.position[2] + zRandom);
                position.x += mapSize.x;
                position.y *= -1;
                position.z += mapSize.y;
                instanceObj.transform.position = position;

                //setup hierarchy
                var propertiesComponents = instanceObj.GetComponentsInChildren<NodeProperties>();
                foreach (var properties in propertiesComponents)
                {
                    if (properties.isChild)
                    {
                        var nodeParent = instanceObj.transform.FindRecursive(properties.parentName);
                        properties.transform.parent = nodeParent;
                    }
                }

                instanceObj.SetActive(true);
            }

            yield return null;
        }

        yield return null;
    }

    private int CreateOriginalModel(int nodeId, RSM.CompiledModel model, GameObject modelObj)
    {
        // var baseMaterial = Resources.Load<Material>("Materials/ModelMaterial");
        // var baseMaterialTwoSided = Resources.Load<Material>("Materials/ModelMaterial2Sided");
        // var baseMaterialTransparent = Resources.Load<Material>("Materials/ModelMaterialTransparent");

        var canCombine = true;
        foreach (var nodeData in model.nodesData)
        {
            foreach (var meshesByTexture in nodeData)
            {
                var textureId = meshesByTexture.Key;
                var meshData = meshesByTexture.Value;
                var node = meshData.node;

                if (meshesByTexture.Value.vertices.Count == 0)
                {
                    continue;
                }

                for (var i = 0; i < meshData.vertices.Count; i += 3)
                {
                    meshData.triangles.AddRange(new int[]
                    {
                        i + 0, i + 1, i + 2
                    });
                }

                //create node unity mesh
                var mesh = new Mesh();
                mesh.vertices = meshData.vertices.ToArray();
                mesh.triangles = meshData.triangles.ToArray();
                //mesh.normals = meshData.normals.ToArray();
                mesh.uv = meshData.uv.ToArray();

                var nodeObj = new GameObject(node.name.Length == 0 ? $"node_{nodeId}" : node.name);
                nodeObj.transform.parent = modelObj.transform;

                var textureFile = model.rsm.textures[textureId];

                var meshFilter = nodeObj.AddComponent<MeshFilter>();
                meshFilter.mesh = mesh;
                var meshRenderer = nodeObj.AddComponent<MeshRenderer>();

                if (meshData.twoSided)
                {
                    meshRenderer.material = doubleSidedMaterial;
                }
                else if (model.rsm.alpha < 1f)
                {
                    meshRenderer.material = transparentMaterial;
                    meshRenderer.material.SetFloat("_Alpha", model.rsm.alpha);
                }
                else
                {
                    meshRenderer.material = defaultMaterial;
                }

                var texture = FileManager.Load($"data/texture/{textureFile}") as Texture2D;
                meshRenderer.material.mainTexture = texture;

                var properties = nodeObj.AddComponent<NodeProperties>();
                properties.SetTextureName(textureFile);

                if (model.rsm.shadeType == RSM.SHADING.SMOOTH)
                {
                    NormalSolver.RecalculateNormals(meshFilter.sharedMesh, 60);
                }
                else
                {
                    meshFilter.sharedMesh.RecalculateNormals();
                }

                var matrix = node.GetPositionMatrix();
                nodeObj.transform.position = matrix.ExtractPosition();
                var rotation = matrix.ExtractRotation();
                nodeObj.transform.rotation = rotation;
                nodeObj.transform.localScale = matrix.ExtractScale();

                properties.nodeId = nodeId;
                properties.mainName = model.rsm.mainNode.name;
                properties.parentName = node.parentName;

                if (node.posKeyframes.Count > 0 || node.rotKeyframes.Count > 0)
                {
                    canCombine = false;
                    var collider = nodeObj.AddComponent<MeshCollider>();
                    collider.sharedMesh = mesh;
                    if (mesh.vertexCount / 3 >= 3)
                    {
                        collider.convex = true;
                    }

                    var nodeAnimation = nodeObj.AddComponent<NodeAnimation>();
                    nodeAnimation.nodeId = nodeId;
                    var props = new AnimProperties()
                    {
                        posKeyframes = node.posKeyframes.Values.ToList(),
                        posKeyframesKeys = node.posKeyframes.Keys.ToList(),
                        rotKeyframes = node.rotKeyframes.Values.ToList(),
                        rotKeyframesKeys = node.rotKeyframes.Keys.ToList(),
                        animLen = model.rsm.animLen,
                        baseRotation = rotation,
                        isChild = properties.isChild
                    };
                    nodeAnimation.Initialize(props);
                }

                nodeId++;
            }
        }

        // Combine all meshes to generate a collider
        if (canCombine)
        {
            var modelObjFilter = modelObj.AddComponent<MeshFilter>();
            var modelObjRenderer = modelObj.AddComponent<MeshRenderer>();

            var filters = modelObjFilter.GetComponentsInChildren<MeshFilter>();
            var renderers = modelObjFilter.GetComponentsInChildren<MeshRenderer>();

            // fetch all materials from children
            var materials = new List<Material>();
            foreach (var renderer in renderers)
            {
                if (renderer.transform == modelObj.transform)
                    continue;

                var localMats = renderer.materials;
                foreach (var localMat in localMats)
                {
                    if (!materials.Contains(localMat))
                    {
                        materials.Add(localMat);
                    }
                }
            }

            var submeshes = new List<Mesh>();
            foreach (var material in materials)
            {
                var combiners = new List<CombineInstance>();
                foreach (var filter in filters)
                {
                    if (filter.transform == modelObj.transform) continue;

                    var renderer = filter.GetComponent<MeshRenderer>();
                    if (renderer == null)
                    {
                        Debug.LogError($"{filter.name} does not have a MeshRenderer component.");
                        continue;
                    }

                    var localMaterials = renderer.materials;
                    for (var materialIndex = 0; materialIndex < localMaterials.Length; materialIndex++)
                    {
                        if (localMaterials[materialIndex] != material)
                            continue;

                        var combineInstance = new CombineInstance();
                        combineInstance.mesh = filter.sharedMesh;
                        combineInstance.subMeshIndex = materialIndex;
                        combineInstance.transform = filter.transform.localToWorldMatrix;
                        combiners.Add(combineInstance);
                    }
                }

                var mesh = new Mesh();
                mesh.CombineMeshes(combiners.ToArray(), true);
                submeshes.Add(mesh);
            }

            var finalCombiners = new List<CombineInstance>();
            foreach (var mesh in submeshes)
            {
                var combineInstance = new CombineInstance();
                combineInstance.mesh = mesh;
                combineInstance.subMeshIndex = 0;
                combineInstance.transform = Matrix4x4.identity;
                finalCombiners.Add(combineInstance);
            }

            foreach (var child in modelObj.transform.GetChildren())
            {
                GameObject.DestroyImmediate(child.gameObject);
            }

            var combinersArray = finalCombiners.ToArray();
            var finalMesh = new Mesh();
            finalMesh.CombineMeshes(combinersArray, false);

            modelObjFilter.sharedMesh = finalMesh;
            modelObjRenderer.materials = materials.ToArray();
            modelObjRenderer.shadowCastingMode = ShadowCastingMode.Off;

            var shadowObj = new GameObject($"{modelObj.name}_ShadowProxy");
            shadowObj.transform.SetParent(modelObj.transform, false);
            
            var shadowFilter = shadowObj.AddComponent<MeshFilter>();
            var shadowRenderer = shadowObj.AddComponent<MeshRenderer>();
            
            var shadowMesh = new Mesh();
            shadowMesh.CombineMeshes(combinersArray, true);
            shadowFilter.sharedMesh = shadowMesh;
            shadowRenderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
            shadowRenderer.receiveShadows = false;
            shadowRenderer.material = shadowOnlyMaterial;
        }

        return nodeId;
    }
}