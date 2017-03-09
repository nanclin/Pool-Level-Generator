using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

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

    public void AddTriangle(Vector3 A, Vector3 B, Vector3 C, Vector2[] uvs = null) {

        if (uvs != null)
            Assert.IsTrue(uvs.Length == 3, "Number of uvs must match number of vertices!");

        Vertices.Add(A);
        Vertices.Add(B);
        Vertices.Add(C);

        Triangles.Add(VertexIndex);
        Triangles.Add(VertexIndex + 1);
        Triangles.Add(VertexIndex + 2);

        if (uvs == null) {
            UVs.Add(new Vector2(0, 0));
            UVs.Add(new Vector2(0, 1));
            UVs.Add(new Vector2(1, 1));
        } else {
            UVs.Add(uvs[0]);
            UVs.Add(uvs[1]);
            UVs.Add(uvs[2]);
        }

        VertexIndex += 3;
    }

    public void AddQuad(float x, float y, float size, Vector2[] uvs = null) {
        Vector3 a = new Vector3(x, y);
        Vector3 b = new Vector3(x + size, y);
        Vector3 c = new Vector3(x, y + size);
        Vector3 d = new Vector3(x + size, y + size);

        Vertices.Add(a);
        Vertices.Add(b);
        Vertices.Add(c);
        Vertices.Add(d);

        if (uvs == null) {
            UVs.Add(new Vector2(0, 0));
            UVs.Add(new Vector2(1, 0));
            UVs.Add(new Vector2(0, 1));
            UVs.Add(new Vector2(1, 1));
        } else {
            UVs.Add(uvs[0]);
            UVs.Add(uvs[1]);
            UVs.Add(uvs[2]);
            UVs.Add(uvs[3]);
        }

        Triangles.Add(VertexIndex + 0);
        Triangles.Add(VertexIndex + 2);
        Triangles.Add(VertexIndex + 3);

        Triangles.Add(VertexIndex + 0);
        Triangles.Add(VertexIndex + 3);
        Triangles.Add(VertexIndex + 1);

        VertexIndex += 4;
    }

    public void SetMesh(Mesh mesh, string name) {

        mesh.Clear();

        mesh.name = name;

        mesh.vertices = Vertices.ToArray();
        mesh.triangles = Triangles.ToArray();
        mesh.uv = UVs.ToArray();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}