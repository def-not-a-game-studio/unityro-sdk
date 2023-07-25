using System;
using System.IO;
using System.Linq;
using ROIO;
using ROIO.Loaders;
using UnityEditor;
using UnityEngine;

namespace UnityRO.Core.Editor {
    public class WavUtility {
        private static string GENERATED_RESOURCES_PATH =
            Path.Combine("Assets", "3rdparty", "unityro-resources", "Resources", "Wav");

        [MenuItem("UnityRO/Utils/Extract/Wav")]
        static void ExtractWav() {
            var descriptors = DataUtility.FilterDescriptors(FileManager.GetFileDescriptors(), "data/wav")
                .Where(it => Path.GetExtension(it) == ".wav")
                .ToList();

            try {
                AssetDatabase.StartAssetEditing();
                for (int i = 0; i < descriptors.Count; i++) {
                    var progress = i * 1f / descriptors.Count;
                    if (EditorUtility.DisplayCancelableProgressBar("UnityRO", $"Extracting wavs {i} of {descriptors.Count}\t\t{progress * 100}%",
                            progress)) {
                        break;
                    }

                    var descriptor = descriptors[i];
                    try {
                        var audioClip = FileManager.Load(descriptor) as AudioClip;

                        if (audioClip != null) {
                            var filenameWithoutExtension = Path.GetFileNameWithoutExtension(descriptor).SanitizeForAddressables();
                            var dir = Path.GetDirectoryName(descriptor).Replace("data/wav/", "");
                            string assetPath = Path.Combine(GENERATED_RESOURCES_PATH, dir);
                            Directory.CreateDirectory(assetPath);

                            var completePath = Path.Combine(assetPath, filenameWithoutExtension + ".wav");
                            SavWav.Save(completePath, audioClip);
                        }
                    } catch (Exception e) {
                        Debug.LogError($"Failed to extract {descriptor} {e}");
                    }
                }
            } finally {
                AssetDatabase.StopAssetEditing();
                EditorUtility.ClearProgressBar();
                AssetDatabase.Refresh();
            }
        }
    }
}