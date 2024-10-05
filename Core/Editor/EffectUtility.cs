using ROIO;
using ROIO.Loaders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Effects.EffectParts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using ROIO.Models.FileTypes;
using UnityEditor;
using UnityEngine;

public class EffectUtility
{
    private static string GENERATED_RESOURCES_PATH = Path.Combine("Assets", "Resources", "Effects");
    private static string DEFAULT_EFFECT_DIR = Path.Combine("data", "texture", "effect") + Path.DirectorySeparatorChar;

    [MenuItem("UnityRO/Utils/Extract/Effects/STR")]
    static void ExtractSTREffects()
    {
        FileManager.LoadGRF("D:\\Projetos\\Personal\\Ragnarok\\Unity\\", new List<string> { "kro_data.grf" });
        //FileManager.LoadGRF("../../ragnarok/", new List<string> { "data.grf" });

        try
        {
            var descriptors = DataUtility
                .FilterDescriptors(FileManager.GetFileDescriptors(), "data/texture/effect")
                .Where(it => Path.GetExtension(it) == ".str")
                .ToList();

            for (var i = 0; i < descriptors.Count; i++)
            {
                var progress = i * 1f / descriptors.Count;
                if (EditorUtility.DisplayCancelableProgressBar("UnityRO",
                        $"Extracting effects {i} of {descriptors.Count}\t\t{progress * 100}%",
                        progress))
                {
                    break;
                }

                try
                {
                    ExtractStr(descriptors[i]);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }
    }

    [MenuItem("UnityRO/Utils/Extract/Effects/SPR")]
    static void ExtractSPREffects()
    {
        FileManager.LoadGRF("D:\\Projetos\\Personal\\Ragnarok\\Unity\\", new List<string> { "kro_data.grf" });
        //FileManager.LoadGRF("../../ragnarok/", new List<string> { "data.grf" });

        try
        {
            var descriptors = DataUtility
                .FilterDescriptors(FileManager.GetFileDescriptors(), "data/sprite/ÀÌÆÑÆ®")
                .Where(it => Path.GetExtension(it) is ".str" or ".act" or ".spr")
                .ToList();

            for (var i = 0; i < descriptors.Count; i++)
            {
                var progress = i * 1f / descriptors.Count;
                if (EditorUtility.DisplayCancelableProgressBar("UnityRO",
                        $"Extracting effects {i} of {descriptors.Count}\t\t{progress * 100}%",
                        progress))
                {
                    break;
                }

                try
                {
                    var descriptor = descriptors[i];
                    switch (Path.GetExtension(descriptor))
                    {
                        case ".spr":
                            ExtractSpr(descriptor);
                            break;
                        case ".str":
                            ExtractStr(descriptors[i]);
                            break;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }
    }

    [MenuItem("UnityRO/Utils/Extract/Effects/Everything else")]
    static void ExtractTextureEffects()
    {
        FileManager.LoadGRF("D:\\Projetos\\Personal\\Ragnarok\\Unity\\", new List<string> { "kro_data.grf" });
        //FileManager.LoadGRF("../../ragnarok/", new List<string> { "data.grf" });

        try
        {
            AssetDatabase.StartAssetEditing();
            var descriptors = DataUtility
                .FilterDescriptors(FileManager.GetFileDescriptors(), "data/texture/effect")
                .Where(it =>
                    Path.GetDirectoryName(it) == Path.Combine("data", "texture", "effect") &&
                    Path.GetExtension(it) != ".str")
                .ToList();

            for (var i = 0; i < descriptors.Count; i++)
            {
                var progress = i * 1f / descriptors.Count;
                if (EditorUtility.DisplayCancelableProgressBar("UnityRO",
                        $"Extracting effects {i} of {descriptors.Count}\t\t{progress * 100}%",
                        progress))
                {
                    break;
                }

                try
                {
                    ExtractTexture(descriptors[i]);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }
    }

    private static void ExtractTexture(string descriptor)
    {
        var texture = FileManager.Load(descriptor) as Texture2D;
        if (texture == null) throw new Exception();

        var filenameWithoutExtension =
            Path.GetFileNameWithoutExtension(descriptor).SanitizeForAddressables();
        var dir = Path.GetDirectoryName(descriptor.Replace('/', Path.DirectorySeparatorChar)
            .Replace(DEFAULT_EFFECT_DIR, ""));

        var assetPath = Path.Combine(GENERATED_RESOURCES_PATH, "Textures", dir);
        Directory.CreateDirectory(assetPath);

        var bytes = texture.EncodeToPNG();
        var atlasPath = $"{assetPath}/{filenameWithoutExtension}.png";
        File.WriteAllBytes(atlasPath, bytes);
        AssetDatabase.ImportAsset(atlasPath);

        var importer = AssetImporter.GetAtPath(atlasPath) as TextureImporter;
        importer.alphaIsTransparency = true;
        importer.SaveAndReimport();
    }

    private static void ExtractStr(string descriptor)
    {
        var strEffect = EffectLoader.Load(FileManager.ReadSync(descriptor),
            Path.GetDirectoryName(descriptor).Replace("\\", "/"),
            path => FileManager.Load(path) as Texture2D);

        if (strEffect == null) return;

        var filenameWithoutExtension =
            Path.GetFileNameWithoutExtension(descriptor).SanitizeForAddressables();
        strEffect.name = filenameWithoutExtension;
        var dir = Path.GetDirectoryName(descriptor.Replace('/', Path.DirectorySeparatorChar)
            .Replace(DEFAULT_EFFECT_DIR, ""));

        var assetPath = Path.Combine(GENERATED_RESOURCES_PATH, "STR", dir);
        Directory.CreateDirectory(assetPath);

        var atlas = strEffect.Atlas;
        var bytes = atlas.EncodeToPNG();
        var atlasPath = $"{assetPath}/{filenameWithoutExtension}.png";

        File.WriteAllBytes(atlasPath, bytes);
        AssetDatabase.ImportAsset(atlasPath);

        var importer = AssetImporter.GetAtPath(atlasPath) as TextureImporter;
        importer.alphaIsTransparency = true;
        importer.SaveAndReimport();

        var diskAtlas = AssetDatabase.LoadAssetAtPath<Texture2D>(atlasPath);
        strEffect._Atlas = diskAtlas;

        strEffect.name = filenameWithoutExtension;

        var completePath = Path.Combine(assetPath, filenameWithoutExtension + ".asset");
        AssetDatabase.CreateAsset(strEffect, completePath);
    }

    private static void ExtractSpr(string descriptor)
    {
        var baseFileName = descriptor.Replace(".spr", "");

        var sprPath = baseFileName + ".spr";
        var actPath = baseFileName + ".act";

        var sprBytes = FileManager.ReadSync(sprPath).ToArray();
        var act = FileManager.Load(actPath) as ACT;

        var spriteLoader = new CustomSpriteLoader();
        var filename = Path.GetFileNameWithoutExtension(descriptor);

        var dir = Path.GetDirectoryName(descriptor.Replace('/', Path.DirectorySeparatorChar)
            .Replace("data\\sprite\\ÀÌÆÑÆ®\\", ""));
        var assetPath = Path.Combine(GENERATED_RESOURCES_PATH, "SPR", dir);
        var spriteDataPath = Path.Combine(assetPath, filename);
        Directory.CreateDirectory(assetPath);

        var spriteData = ScriptableObject.CreateInstance<SpriteData>();
        spriteData.act = act;

        spriteLoader.Load(sprBytes, filename, false);

        var atlas = spriteLoader.Atlas;
        var bytes = atlas.EncodeToPNG();
        var atlasPath = spriteDataPath + ".png";
        File.WriteAllBytes(atlasPath, bytes);

        AssetDatabase.ImportAsset(spriteDataPath + ".png");

        // transform texture into multiple sprites texture
        var importer = AssetImporter.GetAtPath(spriteDataPath + ".png") as TextureImporter;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        var textureSettings = new TextureImporterSettings();
        importer.ReadTextureSettings(textureSettings);
        textureSettings.spriteMeshType = SpriteMeshType.FullRect;
        textureSettings.spritePixelsPerUnit = SPR.PIXELS_PER_UNIT;

        var sheetMetaData = spriteLoader.Sprites.Select(it => new SpriteMetaData
        {
            rect = it.rect,
            name = it.name
        }).ToArray();

        importer.spritesheet = sheetMetaData;
        importer.SetTextureSettings(textureSettings);
        importer.SaveAndReimport();

        AssetDatabase.ImportAsset(spriteDataPath + ".png");
        spriteData.atlas = AssetDatabase.LoadAssetAtPath<Texture2D>(atlasPath);
        spriteData.sprites = AssetDatabase.LoadAllAssetsAtPath(spriteDataPath + ".png").OfType<Sprite>().ToArray();

        var fullAssetPath = spriteDataPath + ".asset";
        AssetDatabase.CreateAsset(spriteData, fullAssetPath);
    }
    
    [MenuItem("UnityRO/Utils/Effect/Assign atlas to Sprite effects")]
    static void AssignAtlas()
    {
        var spriteDatas = Resources.LoadAll<SpriteData>("Effects/SPR");
        try
        {
            AssetDatabase.StartAssetEditing();
            foreach (var spriteData in spriteDatas)
            {
                var assetPath = AssetDatabase.GetAssetPath(spriteData);
                spriteData.atlas = AssetDatabase.LoadAssetAtPath<Texture2D>(Path.ChangeExtension(assetPath, ".png"));
                EditorUtility.SetDirty(spriteData);
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
            AssetDatabase.SaveAssets();
        }
    }
    
    [MenuItem("UnityRO/Utils/Effect/Generate SPR Frame Ids")]
    static void GenerateFrameIds()
    {
        var spriteDatas = Resources.LoadAll<SpriteData>("Effects/SPR");
        try
        {
            AssetDatabase.StartAssetEditing();
            foreach (var spriteData in spriteDatas)
            {
                foreach (var action in spriteData.act.actions)
                {
                    foreach (var frame in action.frames)
                    {
                        frame.id = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
                    }
                }
                EditorUtility.SetDirty(spriteData);
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
            AssetDatabase.SaveAssets();
        }
    }

    [MenuItem("UnityRO/Utils/Generate/Effects Scriptables")]
    private static void GenerateEffects()
    {
        var databaseString = Resources.Load<TextAsset>("Effects/EffectTable").text;
        var databaseObj = JsonConvert.DeserializeObject<JObject>(databaseString);

        try
        {
            AssetDatabase.StartAssetEditing();

            foreach (var (key, effect) in databaseObj)
            {
                var effName = int.TryParse(key, out var id) ? ((EffectId)id).ToString() : key;

                var cylinder = new List<CylinderEffectPart>();
                var spr = new List<SprEffectPart>();
                var str = new List<StrEffectPart>();
                var _3d = new List<ThreeDEffectPart>();
                var _2d = new List<TwoDEffectPart>();

                foreach (var part in effect)
                {
                    switch (part["type"]?.ToString())
                    {
                        case "3D":
                            break;
                        case "2D":
                            break;
                        case "CYLINDER":
                            break;
                        case "FUNC":
                            break;
                        case "SPR":
                            spr.Add(GetSprPart(part));
                            break;
                        case "STR":
                            str.Add(GetStrPart(part));
                            break;
                        case "QuadHorn":
                            break;
                        default: //wav
                            break;
                    }
                }

                if (cylinder.Count <= 0 && spr.Count <= 0 && str.Count <= 0 && _3d.Count <= 0 &&
                    _2d.Count <= 0) continue;

                var scriptableEffect = ScriptableObject.CreateInstance<Effect>();
                scriptableEffect.EffectId = id;
                scriptableEffect.name = effName;
                scriptableEffect.CylinderParts = cylinder.ToArray();
                scriptableEffect.SPRParts = spr.ToArray();
                scriptableEffect.STRParts = str.ToArray();
                scriptableEffect.ThreeDParts = _3d.ToArray();
                scriptableEffect.TwoDParts = _2d.ToArray();

                AssetDatabase.CreateAsset(scriptableEffect,
                    $"Assets/3rdparty/unityro-resources/Resources/Database/Effects/Extracted/{effName}.asset");
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
        }
    }

    private static SprEffectPart GetSprPart(JToken src)
    {
        return new SprEffectPart
        {
            duration = src["duration"]?.Value<long>() ?? 0L,
            duplicates = src["duplicates"]?.Value<int>() ?? 0,
            timeBetweenDuplication = src["timeBetweenDuplication"]?.Value<float>() ?? 0f,
            attachedEntity = src["attachedEntity"]?.Value<bool>() ?? false,
            head = src["head"]?.Value<bool>() ?? false,
            stopAtEnd = src["stopAtEnd"]?.Value<bool>() ?? false,
            direction = src["direction"]?.Value<bool>() ?? false,
            wav = Resources.Load<AudioClip>($"Wav/{src["wav"]?.Value<string>()}"),
            file = Resources.Load<SpriteData>($"Effects/SPR/{src["file"]?.Value<string>()}"),
        };
    }

    private static StrEffectPart GetStrPart(JToken src)
    {
        var rand = src["rand"]?.Values<int>().ToArray() ?? new int[] { };
        var files = new List<STR>();
        var wavs = new List<AudioClip>();
        
        if (rand.Length > 1)
        {
            for (var i = rand[0]; i <= rand.Last(); i++)
            {
                var file = Resources.Load<STR>(
                    $"Effects/STR/{src["file"]?.Value<string>().Replace("%d", i.ToString())}");
                var wav = Resources.Load<AudioClip>($"Wav/{src["wav"]?.Value<string>().Replace("%d", i.ToString())}");

                if (wav != null)
                    wavs.Add(wav);

                if (file != null)
                    files.Add(file);
            }
        }

        return new StrEffectPart
        {
            attachedEntity = src["attachedEntity"]?.Value<bool>() ?? false,
            head = src["head"]?.Value<bool>() ?? false,
            repeat = src["head"]?.Value<bool>() ?? false,
            wav = Resources.Load<AudioClip>($"Wav/{src["wav"]?.Value<string>()}"),
            wavs = wavs.ToArray(),
            file = Resources.Load<STR>($"Effects/STR/{src["file"]?.Value<string>()}"),
            files = files.ToArray(),
            simplified = Resources.Load<STR>($"Effects/STR/{src["min"]?.Value<string>()}"),
        };
    }
}