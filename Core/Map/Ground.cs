﻿using System;
using System.Collections.Generic;
using MeshSplit.Scripts;
using ROIO;
using ROIO.Models.FileTypes;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

public class Ground {
    private const int MAX_VERTICES = int.MaxValue;

    public Mesh[] meshes { get; private set; }
    private Texture2D atlas;
    private Texture2D lightmap;
    private Texture2D tintmap;

    public Ground() { }

    public Ground(GND.Mesh compiledMesh, RSW.WaterInfo waterInfo, bool splitIntoChunks) {
        BuildMesh(compiledMesh);
        if (meshes.Length > 0) {
            InitTextures(compiledMesh);
            Render(splitIntoChunks);
        }

        if (compiledMesh.waterVertCount > 0) {
            var waterBuilder = new WaterBuilder();
            waterBuilder.InitTextures(compiledMesh, waterInfo);
            waterBuilder.BuildMesh(compiledMesh);
        }
    }

    public struct Vertex {
        public Vector3 position;
        public Vector3 normal;
        public Vector2 texCoord;
        public Vector2 lightCoord;
        public Vector2 tileCoord;
    }

    public void Render(bool splitIntoChunks) {
        GameObject gameObject = new GameObject("_Ground");
        gameObject.transform.parent = GameObject.FindObjectOfType<GameMap>().transform;
        var material = Resources.Load<Material>("Materials/GroundMaterial");

        Mesh mesh = meshes[0];
        var mf = gameObject.AddComponent<MeshFilter>();
        mf.mesh = mesh;
        var mr = gameObject.AddComponent<MeshRenderer>();
        mr.sharedMaterial = material;
        mr.sharedMaterial.mainTexture = atlas;
        mr.sharedMaterial.SetTexture("_Tintmap", tintmap);
        mr.sharedMaterial.SetTexture("_Lightmap", lightmap);
        mr.shadowCastingMode = ShadowCastingMode.Off;

        // Vector3 scale = gameObject.transform.localScale;
        // scale.Set(1f, 1f, 1f);
        // gameObject.transform.localScale = scale;

        //smooth out mesh
        NormalSolver.RecalculateNormals(mf.sharedMesh, 60);

        //avoid z fighting between ground and models
        gameObject.transform.Translate(0, -0.002f, 0);
        gameObject.AddComponent<MeshCollider>();
        gameObject.layer = LayerMask.NameToLayer("Ground");
        gameObject.isStatic = true;

        if (splitIntoChunks)
        {
            var meshSplitController = gameObject.AddComponent<MeshSplitController>();
            meshSplitController.Parameters = new MeshSplitParameters {
                GenerateColliders = true,
                GridSize = 16,
                SplitAxes = new bool3(true, false, true),
                UseParentLayer = true,
            };
            meshSplitController.Split();

            gameObject.name = "_Ground.old";
            var newGround = new GameObject("_Ground") {
                transform = {
                    parent = GameObject.FindObjectOfType<GameMap>().transform,
                    localScale = Vector3.one,
                },
                layer = LayerMask.NameToLayer("Ground"),
            };
            foreach (var subMesh in gameObject.transform.GetChildren()) {
                subMesh.transform.SetParent(newGround.transform, true);
            }

            GameObject.DestroyImmediate(gameObject);
        }
    }

    public void InitTextures(GND.Mesh compiledMesh) {
        var material = Resources.Load<Material>("Materials/GroundMaterial");
        var textures = compiledMesh.textures;
        var count = textures.Length;
        var _width = Math.Round(Math.Sqrt(count));
        int width = (int)Math.Pow(2, Math.Ceiling(Math.Log(_width * 258) / Math.Log(2)));
        int height = (int)Math.Pow(2, Math.Ceiling(Math.Log(Math.Ceiling(Math.Sqrt(count)) * 258) / Math.Log(2)));

        RenderTexture renderTexture = RenderTexture.GetTemporary(width, height);

        RenderTexture.active = renderTexture;

        GL.Clear(false, true, Color.clear);

        material.SetPass(0);
        GL.PushMatrix();
        GL.LoadPixelMatrix(0, width, 0, height);

        for (int i = 0; i < count; i++) {
            // TODO remove this
            // Why tho? Can't remember now (16/09/2022)
            // Could be because this is only called when building the map using the grf so we can save as prefab
            var texture = FileManager.Load(textures[i]) as Texture2D;
            var x = (float)(i % _width) * 258;
            var y = (float)Math.Floor(i / _width) * 258;

            Graphics.DrawTexture(new Rect(x, y, 264, 264), texture);
            Graphics.DrawTexture(new Rect(x + 1, y + 1, 256, 256), texture);
        }

        GL.PopMatrix();
        GL.End();

        atlas = new Texture2D(width, height, TextureFormat.RGBAFloat, true);
        atlas.wrapMode = TextureWrapMode.Clamp;
        atlas.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        atlas.mipMapBias = -0.5f;

        atlas.Apply();

        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(renderTexture);

        lightmap = compiledMesh.lightmap.ConvertToTexture2D();
        tintmap = compiledMesh.tileColor.ConvertToTexture2D();
    }

    public void BuildMesh(GND.Mesh compiledMesh) {
        meshes = new Mesh[(int)Math.Ceiling(compiledMesh.meshVertCount / (float)MAX_VERTICES)];

        for (int nMesh = 0; nMesh < meshes.Length; nMesh++) {
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> uv = new List<Vector2>();
            List<Vector2> tintUv = new List<Vector2>();
            List<Vector2> lightmapUv = new List<Vector2>();

            float[] vertexData = new float[12];
            int v = 0, h = 0;
            for (int i = 0, ended = 0; vertices.Count < MAX_VERTICES && ended == 0; i++) {
                Vertex[] vs = new Vertex[4];
                for (int j = 0; j < 4; j++) {
                    var vIndex = i * 4 + j + nMesh * MAX_VERTICES;

                    if (vIndex * vertexData.Length >= compiledMesh.mesh.Length) {
                        ended = 1;
                        break;
                    }

                    Array.ConstrainedCopy(compiledMesh.mesh, vIndex * vertexData.Length, vertexData, 0, vertexData.Length);
                    Vertex vertex = BuildVertex(vertexData);
                    vs[j] = vertex;
                    vertices.Add(vertex.position);
                    normals.Add(vertex.normal);
                    uv.Add(vertex.texCoord);
                    lightmapUv.Add(vertex.lightCoord);
                    tintUv.Add(vertex.tileCoord);
                }

                if (ended == 0) {
                    if (vs[0].normal.z == 1) {
                        v++;
                        triangles.AddRange(new int[] {
                            i * 4 + 2, i * 4 + 1, i * 4 + 0, //left triangle                              
                            i * 4 + 0, i * 4 + 3, i * 4 + 2, //right triangle      
                        });
                    } else if (vs[0].normal.x == 1) {
                        v++;
                        triangles.AddRange(new int[] {
                            i * 4 + 1, i * 4 + 2, i * 4 + 0, //left triangle  
                            i * 4 + 1, i * 4 + 3, i * 4 + 2, //right triangle       
                        });
                    } else {
                        h++;
                        triangles.AddRange(new int[] {
                            i * 4 + 3, i * 4 + 2, i * 4 + 0, //left triangle  
                            i * 4 + 2, i * 4 + 1, i * 4 + 0, //right triangle                      
                        });
                    }
                }
            }

            Mesh mesh = new Mesh();
            mesh.indexFormat = IndexFormat.UInt32;
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.normals = normals.ToArray();
            mesh.uv = uv.ToArray();
            mesh.uv2 = lightmapUv.ToArray();
            mesh.uv3 = tintUv.ToArray();
            //mesh.RecalculateNormals();

            meshes[nMesh] = mesh;
        }
    }

    private Vertex BuildVertex(float[] vertexData) {
        Vertex v = new Vertex();

        v.position = new Vector3(vertexData[0], -vertexData[1], vertexData[2]);
        v.normal = new Vector3(vertexData[3], vertexData[4], vertexData[5]);
        v.texCoord = new Vector2(vertexData[6], vertexData[7]);
        v.lightCoord = new Vector2(vertexData[8], vertexData[9]);
        v.tileCoord = new Vector2(vertexData[10], vertexData[11]);

        return v;
    }
}