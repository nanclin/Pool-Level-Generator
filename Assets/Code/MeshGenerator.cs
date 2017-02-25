using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour {

    [SerializeField] private MeshFilter MeshFilter;
    [SerializeField] private MeshRenderer MeshRenderer;

    public void GenerateMesh() {
        Mesh mesh = MeshFilter.mesh;

        MeshData meshData = new MeshData();

        meshData.AddTriangle(new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(1, 1, 0));
        meshData.AddTriangle(new Vector3(0, 0, 0), new Vector3(1, 1, 0), new Vector3(1, 0, 0));

        meshData.AddQuad(0, 2, 1, 1);
        meshData.AddQuad(1, 2, 2, 2);
        meshData.AddQuad(3, 2, 1, 3);

        mesh.name = "newMesh";

        mesh.vertices = meshData.Vertices.ToArray();
        mesh.triangles = meshData.Triangles.ToArray();
        mesh.uv = meshData.UVs.ToArray();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}

public class MeshData {
    
    public List<Vector3> Vertices;
    public List<int> Triangles;
    public List<Vector2> UVs;

    private int VertexIndex;

    public MeshData() {
        VertexIndex = 0;
        Vertices = new List<Vector3>();
        Triangles = new List<int>();
        UVs = new List<Vector2>();
    }

    public void AddTriangle(Vector3 A, Vector3 B, Vector3 C) {
        Vertices.Add(A);
        Vertices.Add(B);
        Vertices.Add(C);

        Triangles.Add(VertexIndex);
        Triangles.Add(VertexIndex + 1);
        Triangles.Add(VertexIndex + 2);

        UVs.Add(new Vector2(0, 0));
        UVs.Add(new Vector2(0, 1));
        UVs.Add(new Vector2(1, 1));

        VertexIndex += 3;
    }

    public void AddQuad(float x, float y, float width, float height) {
        Vector3 a = new Vector3(x, y);
        Vector3 b = new Vector3(x + width, y);
        Vector3 c = new Vector3(x, y + height);
        Vector3 d = new Vector3(x + width, y + height);

        Vertices.Add(a);
        Vertices.Add(b);
        Vertices.Add(c);
        Vertices.Add(d);

        UVs.Add(new Vector2(0, 0));
        UVs.Add(new Vector2(1, 0));
        UVs.Add(new Vector2(0, 1));
        UVs.Add(new Vector2(1, 1));

        Triangles.Add(VertexIndex + 0);
        Triangles.Add(VertexIndex + 2);
        Triangles.Add(VertexIndex + 3);

        Triangles.Add(VertexIndex + 0);
        Triangles.Add(VertexIndex + 3);
        Triangles.Add(VertexIndex + 1);

        VertexIndex += 4;
    }
}
