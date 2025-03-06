#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ROIO;
using ROIO.Loaders;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityRO.Core.Editor.MapExtractorUtil;

[InitializeOnLoad]
public class ROMapExtractor : EditorWindow
{
    [SerializeField] private string grfRootPath = "C:/foo";
    [SerializeField] private List<string> grfPaths = new List<string>();
    [SerializeField] private string mapName = "prontera";

    private ReorderableList GrfReordableList;

    private GameMap CurrentGameMap;

    [MenuItem("Window/ROMapExtractor")]
    public static void ShowWindow()
    {
        GetWindow(typeof(ROMapExtractor));
    }

    async Task LoadMap(bool splitIntoChunks = true)
    {
        AsyncMapLoader.GameMapData gameMapData = await new AsyncMapLoader().Load($"{mapName}.rsw");
        CurrentGameMap = await new MapRenderer().RenderMap(gameMapData, mapName, splitIntoChunks);
    }

    public static string GetBasePath() => "Assets/3rdparty/unityro-resources/Resources/Maps/";
    
    public async void ExportGroundObj()
    {
        await LoadMap(false);
        var meshFilters = CurrentGameMap.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);

            i++;
        }

        Mesh mesh = new Mesh();
        mesh.CombineMeshes(combine);
        new GameObject().AddComponent<MeshFilter>().sharedMesh = mesh;

        ExportToOBJ(mesh, mapName);
    }

    private void OnEnable()
    {
        var data = EditorPrefs.GetString("ROMapExtractorWindow", JsonUtility.ToJson(this, false));
        // Then we apply them to this window
        JsonUtility.FromJsonOverwrite(data, this);

        if (grfPaths.Count > 0 && grfRootPath.Length > 0)
        {
            LoadGRF();
        }

        GrfReordableList = new ReorderableList(grfPaths, typeof(string));
        GrfReordableList.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "GRF List");
        GrfReordableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            rect.y += 2f;
            rect.height = EditorGUIUtility.singleLineHeight;

            GrfReordableList.list[index] = EditorGUI.TextField(rect, (string)GrfReordableList.list[index]);
        };
    }

    private void OnDisable()
    {
        var data = JsonUtility.ToJson(this, false);
        // And we save it
        EditorPrefs.SetString("ROMapExtractorWindow", data);
    }

    private void OnGUI()
    {
        GUILayout.Space(8);
        GUILayout.Label("GRF Settings", EditorStyles.boldLabel);
        grfRootPath = EditorGUILayout.TextField("GRF Root Path", grfRootPath);
        GUILayout.Space(8);
        GrfReordableList?.DoLayoutList();

        if (GUILayout.Button("Load GRF"))
        {
            LoadGRF();
        }

        GUILayout.Space(16);
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        mapName = EditorGUILayout.TextField("Map name", mapName);
        GUILayout.Space(8);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Load Map"))
        {
            LoadMap();
        }

        if (GUILayout.Button("Save Map") && CurrentGameMap != null)
        {
            MapExtractor.SaveMap(CurrentGameMap.gameObject);
        }

        if (GUILayout.Button("Export ground obj"))
        {
            ExportGroundObj();
        }

        EditorGUILayout.EndHorizontal();
    }

    private void LoadGRF()
    {
        if (grfPaths != null && grfPaths.Count > 0)
        {
            FileManager.LoadGRF(grfRootPath, grfPaths.Where(it => it.Length > 0).ToList());
        }
    }

    private void OnInspectorUpdate()
    {
        Repaint();
    }

    private static void ExportToOBJ(Mesh mesh, string name)
    {
        var nfi = new NumberFormatInfo
        {
            NumberDecimalSeparator = ".",
        };
        var path = EditorUtility.SaveFilePanel("Export OBJ", "", name, "obj");
        var sb = new StringBuilder();
        sb.AppendLine($"mtllib {name}.mtl");
        sb.AppendLine($"o {name}.obj");

        ConvertMeshToObj(mesh, name, sb, nfi);

        var writer = new StreamWriter(path);
        writer.Write(sb.ToString());
        writer.Close();
    }

    private static void ConvertMeshToObj(Mesh mesh, string name, StringBuilder sb, NumberFormatInfo nfi)
    {
        foreach (var v in mesh.vertices)
        {
            sb.AppendLine($"v {v.x.ToString(nfi)} {v.y.ToString(nfi)} {v.z.ToString(nfi)}");
        }

        foreach (var v in mesh.normals)
        {
            sb.AppendLine($"vn {v.x.ToString(nfi)} {v.y.ToString(nfi)} {v.z.ToString(nfi)}");
        }

        int t1, t2, t3;
        for (var material = 0; material < mesh.subMeshCount; material++)
        {
            sb.AppendLine($"g {name}");
            var triangles = mesh.GetTriangles(material);
            for (var i = 0; i < triangles.Length; i += 3)
            {
                t1 = triangles[i] + 1;
                t2 = triangles[i + 1] + 1;
                t3 = triangles[i + 2] + 1;

                sb.AppendLine(string.Format("f {0}/{0} {1}/{1} {2}/{2}", t1.ToString(nfi), t2.ToString(nfi),
                    t3.ToString(nfi)));
            }
        }
    }
}
#endif