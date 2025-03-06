using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityRO.Core.Editor.MapExtractorUtil
{
    public partial class MapExtractor
    {
        private static void ExtractGround(GameObject mapObject, string mapName)
        {
            var ground = mapObject.transform.FindRecursive("_Ground");

            ExtractGroundTextures(mapName, ground, out var lightmapTexture, out var tintmapTexture, out var mainTexture, out var material);

            void ExtractGroundInner(Transform mesh)
            {
                mesh.gameObject.SetActive(true);
                var meshPath = Path.Combine(ROMapExtractor.GetBasePath(), mapName, "ground", mesh.gameObject.name);
                Directory.CreateDirectory(meshPath);

                var filter = mesh.GetComponent<MeshFilter>();
                mesh.GetComponent<MeshRenderer>().material = material;

                var partPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(meshPath,
                    $"{filter.gameObject.name.SanitizeForAddressables()}.asset"));
                AssetDatabase.CreateAsset(filter.sharedMesh, partPath);

                var prefabPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(meshPath,
                    $"{filter.gameObject.name.SanitizeForAddressables()}.prefab"));
                PrefabUtility.SaveAsPrefabAssetAndConnect(filter.gameObject, prefabPath,
                    InteractionMode.AutomatedAction);

                AssetDatabase.ImportAsset(partPath);
                AssetDatabase.ImportAsset(prefabPath);
            }

            try
            {
                AssetDatabase.StartAssetEditing();
                if (ground.transform.childCount == 0)
                {
                    ExtractGroundInner(ground);
                }
                else
                {
                    for (var i = 0; i < ground.transform.childCount; i++)
                    {
                        var mesh = ground.transform.GetChild(i);
                        var progress = i * 1f / ground.transform.childCount;
                        if (EditorUtility.DisplayCancelableProgressBar("UnityRO",
                                $"Saving ground meshes - {progress * 100}%",
                                progress))
                        {
                            break;
                        }

                        ExtractGroundInner(mesh);
                    }
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }
        }

        private static void ExtractGroundTextures(string mapName,
            Transform groundMeshes,
            out Texture2D lightmapTexture,
            out Texture2D tintmapTexture,
            out Texture2D mainTex,
            out Material material
        )
        {
            // Extract first textures only
            var firstMesh = groundMeshes.transform;
            firstMesh.gameObject.SetActive(true);
            var firstMeshMeshPath = Path.Combine(ROMapExtractor.GetBasePath(), mapName, "ground");
            Directory.CreateDirectory(firstMeshMeshPath);
            material = firstMesh.transform.childCount > 0
                ? firstMesh.transform.GetChild(0).GetComponent<MeshRenderer>().material
                : firstMesh.GetComponent<MeshRenderer>().material;

            mainTex = material.GetTexture("_MainTex") as Texture2D;
            lightmapTexture = material.GetTexture("_Lightmap") as Texture2D;
            tintmapTexture = material.GetTexture("_Tintmap") as Texture2D;

            if (mainTex != null)
            {
                if (!mainTex.isReadable)
                {
                    mainTex = DuplicateTexture(mainTex);
                }

                var path = Path.Combine(firstMeshMeshPath, "texture.png");
                var bytes = mainTex.EncodeToPNG();
                File.WriteAllBytes(path, bytes);

                AssetDatabase.ImportAsset(path);
                mainTex = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
                material.SetTexture("_MainTex", mainTex);
            }

            if (lightmapTexture != null)
            {
                if (!lightmapTexture.isReadable)
                {
                    lightmapTexture = DuplicateTexture(lightmapTexture);
                }

                var path = Path.Combine(firstMeshMeshPath, "lightmap.png");
                var bytes = lightmapTexture.EncodeToPNG();
                File.WriteAllBytes(path, bytes);

                AssetDatabase.ImportAsset(path);
                lightmapTexture = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
                material.SetTexture("_Lightmap", lightmapTexture);
            }

            if (tintmapTexture != null)
            {
                if (!tintmapTexture.isReadable)
                {
                    tintmapTexture = DuplicateTexture(tintmapTexture);
                }

                var path = Path.Combine(firstMeshMeshPath, "tintmap.png");
                var bytes = tintmapTexture.EncodeToPNG();
                File.WriteAllBytes(path, bytes);

                AssetDatabase.ImportAsset(path);
                tintmapTexture = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
                material.SetTexture("_Tintmap", tintmapTexture);
            }

            var materialPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(firstMeshMeshPath, $"{mapName}.mat"));
            AssetDatabase.CreateAsset(material, materialPath);
        }

        internal static Texture2D DuplicateTexture(Texture2D source)
        {
            RenderTexture renderTex = RenderTexture.GetTemporary(
                source.width,
                source.height,
                0,
                RenderTextureFormat.Default,
                RenderTextureReadWrite.Linear);

            Graphics.Blit(source, renderTex);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            Texture2D readableText = new Texture2D(source.width, source.height);
            readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);
            return readableText;
        }
    }
}