using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour {

    [SerializeField] private MeshFilter MeshFilter;
    [SerializeField] private MeshRenderer MeshRenderer;
    [SerializeField] private float PerlinScale = 1;
    [SerializeField] private Vector2 PerlinOffset;
    [SerializeField] private float PerlinTreshold = 0.5f;

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

    public void GenerateMarchingSquaresMesh(QuadTree quadTree) {
        
        MeshData meshData = new MeshData();

        float tileSize = quadTree.Size / quadTree.CellsPerSide;

        int cells = quadTree.CellsPerSide;

        for (int y = 0; y < cells + 1; y++) {
            for (int x = 0; x < cells + 1; x++) {
//                Debug.Log(string.Format("x={0} y={1}", x, y));

                int tileIndex = 0;

                for (int i = 0; i < 4; i++) {
                    int ix = x + i % 2;
                    int iy = y + i / 2;


//                    int value = -1;
//
//                    if (ix >= cells) value = 0;
//                    if (iy >= cells) value = 0;
//                    else {
//                        QuadTree quadTreeAtPosition = quadTree.QuadTreeAtPosition(ix, iy);
//                        Debug.Log(string.Format("x={0}, y={1} | ix={2} iy={3} \nquadTreeAtPosition={4}", x, y, ix, iy, quadTreeAtPosition));
//                        if (quadTreeAtPosition != null) {
//                            bool fullQuad = quadTreeAtPosition.Cells.Count >= Mathf.Pow(quadTreeAtPosition.Size, 2) / 2;
//                            value = fullQuad ? 1 : 0;
//                        }
//                    }
//
//
////                    Debug.Log(string.Format("value={0} x={1} y={2}", value, x, y));
//
//
//                    if (value == 1) {
//                        tileIndex += (int) Mathf.Pow(2, i);
//                    }

                    float p = Mathf.PerlinNoise(ix * PerlinScale, iy * PerlinScale);
                    int value = p > PerlinTreshold ? 1 : 0;
//                    Debug.Log(string.Format("x={0}, y={1} | ix={2} iy={3} p={4}", x, y, ix, iy, p));

                    if (value == 1) {
                        tileIndex += (int) Mathf.Pow(2, i);
                    }
                }

                float tileXPos = x * tileSize - quadTree.Size * 0.5f - tileSize * 0.5f;
                float tileYPos = y * tileSize - quadTree.Size * 0.5f - tileSize * 0.5f;
                GenerateMSTile(quadTree, tileIndex, tileXPos, tileYPos, tileSize, meshData);

            }
        }

        meshData.SetMesh(MeshFilter.mesh, "MarchingSquaresMesh");
    }

    public void GenerateMSTile(QuadTree quadTree, int tileIndex, float x, float y, float tileSize, MeshData meshData) {
        if (tileIndex == 0) return;

        // draw whole texture
        float rootSize = quadTree.Root.Size + tileSize;
        float u = x / rootSize - 0.5f;
        float v = y / rootSize - 0.5f;
        float quadSize = tileSize / rootSize;
        Vector2[] uvs = new Vector2[] {
            new Vector2(u, v),
            new Vector2(u + quadSize, v),
            new Vector2(u, v + quadSize),
            new Vector2(u + quadSize, v + quadSize),
        };

        meshData.AddQuad(x, y, tileSize, uvs);
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