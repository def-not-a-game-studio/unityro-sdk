#if UNITY_EDITOR
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;

public class DataUtility {
    public static string[] GetFilesFromDir(string dir) {
        return Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories)
            .Where(it => Path.HasExtension(it) && !it.Contains(".meta"))
            .Select(it => it.Replace(Application.dataPath, "Assets"))
            .ToArray();
    }

    public static List<string> FilterDescriptors(Hashtable descriptors, string filter) {
        return (from DictionaryEntry entry in descriptors select (entry.Key as string).Trim() into path where path.StartsWith(filter) select path).ToList();
    }
}
#endif