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