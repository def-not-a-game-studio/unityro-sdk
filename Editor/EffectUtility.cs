using ROIO;
using ROIO.Loaders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class EffectUtility {
    private static string GENERATED_RESOURCES_PATH = Path.Combine("Assets", "Resources", "Effects");
    private static string DEFAULT_EFFECT_DIR = Path.Combine("data", "texture", "effect") + Path.DirectorySeparatorChar;

    [MenuItem("UnityRO/Utils/Extract/Effects/STR")]
    static void ExtractSTREffects() {
        FileManager.LoadGRF("D:\\Projetos\\ragnarok\\test\\", new List<string> { "kro_data.grf" });
        //FileManager.LoadGRF("../../ragnarok/", new List<string> { "data.grf" });

        try {
            var descriptors = DataUtility
                              .FilterDescriptors(FileManager.GetFileDescriptors(), "data/texture/effect")
                              .Where(it => Path.GetExtension(it) == ".str")
                              .ToList();

            for (var i = 0; i < descriptors.Count; i++) {
                var progress = i * 1f / descriptors.Count;
                if (EditorUtility.DisplayCancelableProgressBar("UnityRO",
                                                               $"Extracting effects {i} of {descriptors.Count}\t\t{progress * 100}%",
                                                               progress)) {
                    break;
                }

                try {
                    ExtractStr(descriptors[i]);
                } catch (Exception e) {
                    Debug.LogException(e);
                }
            }
        } finally {
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }
    }
    
    [MenuItem("UnityRO/Utils/Extract/Effects/Everything else")]
    static void ExtractTextureEffects() {
        FileManager.LoadGRF("D:\\Projetos\\ragnarok\\test\\", new List<string> { "kro_data.grf" });
        //FileManager.LoadGRF("../../ragnarok/", new List<string> { "data.grf" });

        try {
            AssetDatabase.StartAssetEditing();
            var descriptors = DataUtility
                              .FilterDescriptors(FileManager.GetFileDescriptors(), "data/texture/effect")
                              .Where(it => Path.GetDirectoryName(it) == Path.Combine("data", "texture", "effect") && Path.GetExtension(it) != ".str")
                              .ToList();

            for (var i = 0; i < descriptors.Count; i++) {
                var progress = i * 1f / descriptors.Count;
                if (EditorUtility.DisplayCancelableProgressBar("UnityRO",
                                                               $"Extracting effects {i} of {descriptors.Count}\t\t{progress * 100}%",
                                                               progress)) {
                    break;
                }

                try {
                    ExtractTexture(descriptors[i]);
                } catch (Exception e) {
                    Debug.LogException(e);
                }
            }
        } finally {
            AssetDatabase.StopAssetEditing();
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }
    }

    private static void ExtractTexture(string descriptor) {
        var texture = FileManager.Load(descriptor) as Texture2D;
        if (texture == null) throw new Exception();
        
        var filenameWithoutExtension =
            Path.GetFileNameWithoutExtension(descriptor).SanitizeForAddressables();
        var dir = Path.GetDirectoryName(descriptor.Replace('/', Path.DirectorySeparatorChar).Replace(DEFAULT_EFFECT_DIR, ""));

        var assetPath = Path.Combine(GENERATED_RESOURCES_PATH, "Textures", dir);
        Directory.CreateDirectory(assetPath);

        var bytes = texture.EncodeToPNG();
        File.WriteAllBytes($"{assetPath}/{filenameWithoutExtension}.png", bytes);
        AssetDatabase.ImportAsset($"{assetPath}/{filenameWithoutExtension}.png");
    }

    private static void ExtractStr(string descriptor) {
        var strEffect = EffectLoader.Load(FileManager.ReadSync(descriptor),
                                          Path.GetDirectoryName(descriptor).Replace("\\", "/"),
                                          path => FileManager.Load(path) as Texture2D);

        if (strEffect == null) return;
        
        var filenameWithoutExtension =
            Path.GetFileNameWithoutExtension(descriptor).SanitizeForAddressables();
        strEffect.name = filenameWithoutExtension;
        var dir = Path.GetDirectoryName(descriptor.Replace('/', Path.DirectorySeparatorChar).Replace(DEFAULT_EFFECT_DIR, ""));

        var assetPath = Path.Combine(GENERATED_RESOURCES_PATH, "STR", dir);
        Directory.CreateDirectory(assetPath);

        var atlas = strEffect.Atlas;
        var bytes = atlas.EncodeToPNG();
        File.WriteAllBytes($"{assetPath}/{filenameWithoutExtension}.png", bytes);
        AssetDatabase.ImportAsset($"{assetPath}/{filenameWithoutExtension}.png");
        var diskAtlas =
            AssetDatabase.LoadAssetAtPath<Texture2D>($"{assetPath}/{filenameWithoutExtension}.png");
        strEffect._Atlas = diskAtlas;

        strEffect.name = filenameWithoutExtension;

        var completePath = Path.Combine(assetPath, filenameWithoutExtension + ".asset");
        AssetDatabase.CreateAsset(strEffect, completePath);
    }
}