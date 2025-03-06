using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityRO.Core.Editor.MapExtractorUtil
{
    public partial class MapExtractor
    {
        private static void ExtractWater(GameObject mapObject, string mapName)
        {
            var waterMeshes = mapObject.transform.FindRecursive("_Water");
            if (waterMeshes == null)
            {
                return;
            }

            for (int i = 0; i < waterMeshes.transform.childCount; i++)
            {
                var mesh = waterMeshes.transform.GetChild(i);
                mesh.gameObject.SetActive(true);
                var meshPath = Path.Combine(ROMapExtractor.GetBasePath(), mapName, "water", $"_{i}");
                Directory.CreateDirectory(meshPath);

                var progress = i * 1f / waterMeshes.transform.childCount;
                if (EditorUtility.DisplayCancelableProgressBar("UnityRO", $"Saving water meshes - {progress * 100}%",
                        progress))
                {
                    break;
                }

                var filters = mesh.GetComponentsInChildren<MeshFilter>();
                var renderers = mesh.GetComponentsInChildren<MeshRenderer>();
                for (int k = 0; k < filters.Length; k++)
                {
                    var filter = filters[k];
                    var material = renderers[k].sharedMaterial;
                    var mainTex = material.GetTexture("_MainTex") as Texture2D;

                    if (mainTex != null)
                    {
                        var path = Path.Combine(meshPath, "texture.png");
                        var bytes = mainTex.EncodeToPNG();
                        File.WriteAllBytes(path, bytes);

                        AssetDatabase.ImportAsset(path);
                        var tex = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
                        material.SetTexture("_MainTex", tex);
                        //tex.SetAddressableGroup("Maps", "Maps");
                    }

                    var materialPath = Path.Combine(meshPath, $"{filter.gameObject.name.SanitizeForAddressables()}.mat");
                    AssetDatabase.CreateAsset(material, materialPath);
                    AssetDatabase.ImportAsset(materialPath);

                    var filterPath = Path.Combine(meshPath, $"{filter.gameObject.name.SanitizeForAddressables()}.asset");
                    AssetDatabase.CreateAsset(filter.mesh, filterPath);
                    AssetDatabase.ImportAsset(filterPath);

                    var partPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(meshPath,
                        $"{filter.gameObject.name.SanitizeForAddressables()}.prefab"));
                    PrefabUtility.SaveAsPrefabAssetAndConnect(filter.gameObject, partPath,
                        InteractionMode.AutomatedAction);
                    AssetDatabase.ImportAsset(partPath);
                }
            }
        }
    }
}