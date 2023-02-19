#if UNITY_EDITOR
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class DataUtility {
    public static string[] GetFilesFromDir(string dir) {
        return Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories)
            .Where(it => Path.HasExtension(it) && !it.Contains(".meta"))
            .Select(it => it.Replace(Application.dataPath, "Assets"))
            .ToArray();
    }

    public static List<string> FilterDescriptors(Hashtable descriptors, string filter) {
        List<string> result = new List<string>();
        foreach(DictionaryEntry entry in descriptors) {
            string path = (entry.Key as string).Trim();
            if(path.StartsWith(filter)) {
                result.Add(path);
            }
        }

        return result;
    }
}
#endif