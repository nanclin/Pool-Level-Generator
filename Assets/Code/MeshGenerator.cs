using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour {

    [SerializeField] private MeshFilter MeshFilter;
    [SerializeField] private MeshRenderer MeshRenderer;
    [SerializeField] private float PerlinScale = 1;
    [SerializeField] private Vector2 PerlinOffset;
    [SerializeField] private float PerlinTreshold = 0.5f;

    private  int[,] CornerIndices = {
        { 0, 0 },
        { 1, 0 },
        { 1, 1 },
        { 0, 1 },
    };

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
                    int ix = x + CornerIndices[i, 0];
                    int iy = y + CornerIndices[i, 1];


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

        float rootSize = quadTree.Root.Size + tileSize;
        float u = x / rootSize - 0.5f;
        float v = y / rootSize - 0.5f;
        float quadSize = tileSize / rootSize;

        Vector3 a = new Vector2(x, y);
        Vector3 b = new Vector2(x + tileSize, y);
        Vector3 c = new Vector2(x, y + tileSize);
        Vector3 d = new Vector2(x + tileSize, y + tileSize);

        Vector3 aUV = new Vector2(u, v);
        Vector3 bUV = new Vector2(u + quadSize, v);
        Vector3 cUV = new Vector2(u, v + quadSize);
        Vector3 dUV = new Vector2(u + quadSize, v + quadSize);

        float tileSizeHalf = tileSize * 0.5f;
        float quadSizeHalf = quadSize * 0.5f;

        Vector3 s = new Vector2(x + tileSizeHalf, y);
        Vector3 n = new Vector2(x + tileSizeHalf, y + tileSize);
        Vector3 w = new Vector2(x, y + tileSizeHalf);
        Vector3 e = new Vector2(x + tileSize, y + tileSizeHalf);

        Vector3 sUV = new Vector2(u + quadSizeHalf, v);
        Vector3 nUV = new Vector2(u + quadSizeHalf, v + quadSize);
        Vector3 wUV = new Vector2(u, v + quadSizeHalf);
        Vector3 eUV = new Vector2(u + quadSize, v + quadSizeHalf);

        switch (tileIndex) {
            case 1:
                meshData.AddTriangle(a, w, s, new Vector2[]{ aUV, wUV, sUV });
                break;
            case 2:
                meshData.AddTriangle(s, e, b, new Vector2[]{ sUV, eUV, bUV });
                break;
            case 3:
                meshData.AddTriangle(a, w, e, new Vector2[]{ aUV, wUV, eUV });
                meshData.AddTriangle(a, e, b, new Vector2[]{ aUV, eUV, bUV });
                break;
            case 4:
                meshData.AddTriangle(n, d, e, new Vector2[]{ nUV, dUV, eUV });
                break;
            case 5:
                meshData.AddTriangle(a, w, n, new Vector2[]{ aUV, wUV, nUV });
                meshData.AddTriangle(a, n, d, new Vector2[]{ aUV, nUV, dUV });
                meshData.AddTriangle(a, d, e, new Vector2[]{ aUV, dUV, eUV });
                meshData.AddTriangle(a, e, s, new Vector2[]{ aUV, eUV, sUV });
                break;
            case 6:
                meshData.AddTriangle(s, n, d, new Vector2[]{ sUV, nUV, dUV });
                meshData.AddTriangle(s, d, b, new Vector2[]{ sUV, dUV, bUV });
                break;
            case 7:
                meshData.AddTriangle(a, w, n, new Vector2[]{ aUV, wUV, nUV });
                meshData.AddTriangle(a, n, d, new Vector2[]{ aUV, nUV, dUV });
                meshData.AddTriangle(a, d, e, new Vector2[]{ aUV, dUV, eUV });
                meshData.AddTriangle(a, e, b, new Vector2[]{ aUV, eUV, bUV });
                break;
            case 8:
                meshData.AddTriangle(w, c, n, new Vector2[]{ wUV, cUV, nUV });
                break;
            case 9:
                meshData.AddTriangle(a, c, n, new Vector2[]{ aUV, cUV, nUV });
                meshData.AddTriangle(a, n, s, new Vector2[]{ aUV, nUV, sUV });
                break;
            case 10:
                meshData.AddTriangle(w, c, n, new Vector2[]{ wUV, cUV, nUV });
                meshData.AddTriangle(w, n, e, new Vector2[]{ wUV, nUV, eUV });
                meshData.AddTriangle(w, e, b, new Vector2[]{ wUV, eUV, bUV });
                meshData.AddTriangle(w, b, s, new Vector2[]{ wUV, bUV, sUV });
                break;
            case 11:
                meshData.AddTriangle(a, c, n, new Vector2[]{ aUV, cUV, nUV });
                meshData.AddTriangle(a, n, e, new Vector2[]{ aUV, nUV, eUV });
                meshData.AddTriangle(a, e, b, new Vector2[]{ aUV, eUV, bUV });
                break;
            case 12:
                meshData.AddTriangle(w, c, d, new Vector2[]{ wUV, cUV, dUV });
                meshData.AddTriangle(w, d, e, new Vector2[]{ wUV, dUV, eUV });
                break;
            case 13:
                meshData.AddTriangle(a, c, d, new Vector2[]{ aUV, cUV, dUV });
                meshData.AddTriangle(a, d, e, new Vector2[]{ aUV, dUV, eUV });
                meshData.AddTriangle(a, e, s, new Vector2[]{ aUV, eUV, sUV });
                break;
            case 14:
                meshData.AddTriangle(w, c, d, new Vector2[]{ wUV, cUV, dUV });
                meshData.AddTriangle(w, d, b, new Vector2[]{ wUV, dUV, bUV });
                meshData.AddTriangle(w, b, s, new Vector2[]{ wUV, bUV, sUV });
                break;
            case 15:
                meshData.AddTriangle(a, c, d, new Vector2[]{ aUV, cUV, dUV });
                meshData.AddTriangle(a, d, b, new Vector2[]{ aUV, dUV, bUV });
                break;
        }
    }

    //    private void AddMarchingSquaresTileTriangles(int tileIndex){

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