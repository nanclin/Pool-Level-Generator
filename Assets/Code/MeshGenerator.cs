﻿using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour {

    [SerializeField] private MeshFilter MeshFilter;
    [SerializeField] private MeshRenderer MeshRenderer;

    public void GenerateMesh() {
        Mesh mesh = MeshFilter.mesh;

        MeshData meshData = new MeshData();

        meshData.AddTriangle(new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(1, 1, 0));
        meshData.AddTriangle(new Vector3(0, 0, 0), new Vector3(1, 1, 0), new Vector3(1, 0, 0));

//        meshData.AddQuad(0, 2, 1, 1);
//        meshData.AddQuad(1, 2, 2, 2);
//        meshData.AddQuad(3, 2, 1, 3);

        meshData.SetMesh(MeshFilter.mesh, "newMesh");
    }

    public void GenerateMapMesh(int[,] map) {
    }

    public void GenerateQuadTreeMesh(QuadTree quadTree, int depth = 0, MeshData meshData = null) {

        if (meshData == null) {
            meshData = new MeshData();
        }

        if (quadTree.IsLeaf) {

            if (quadTree.Value != 0) return;

            // draw every tile at the same scale
//            int repeat = (int) Mathf.Pow(2, quadTree.MaxDepth - depth);
//            Vector2[] uvs = new Vector2[] {
//                new Vector2(0, 0) * repeat,
//                new Vector2(1, 0) * repeat,
//                new Vector2(0, 1) * repeat,
//                new Vector2(1, 1) * repeat,
//            };

            // draw whole texture
            float rootSize = quadTree.Root.Size;
            float rootSizeHalf = rootSize * 0.5f;
            float x = quadTree.x / rootSize - rootSizeHalf;
            float y = quadTree.y / rootSize - rootSizeHalf;
            float quadSize = quadTree.Size / rootSize;
            Vector2[] uvs = new Vector2[] {
                new Vector2(x, y),
                new Vector2(x + quadSize, y),
                new Vector2(x, y + quadSize),
                new Vector2(x + quadSize, y + quadSize),
            };

            meshData.AddQuad(quadTree.x - rootSizeHalf, quadTree.y - rootSizeHalf, quadTree.Size, uvs);

            return;
        } else {
            GenerateQuadTreeMesh(quadTree.SubQuadTrees[0], depth + 1, meshData);
            GenerateQuadTreeMesh(quadTree.SubQuadTrees[1], depth + 1, meshData);
            GenerateQuadTreeMesh(quadTree.SubQuadTrees[2], depth + 1, meshData);
            GenerateQuadTreeMesh(quadTree.SubQuadTrees[3], depth + 1, meshData);
        }

        if (depth == 0) {// do this only for the root
            meshData.SetMesh(MeshFilter.mesh, "QuadTreeMesh");
        }
    }
}