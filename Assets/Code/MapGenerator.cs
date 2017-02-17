using UnityEngine;

public class MapGenerator : MonoBehaviour {

    [SerializeField] private int Width;
    [SerializeField] private  int Height;
    [SerializeField] public string Seed;

    [Range(0, 100)]
    [SerializeField] private int Treshold;

    private int[,] Map;

    public void GenerateMap() {
        Map = new int[Width, Height];

        System.Random rand = new System.Random(Seed.GetHashCode());

        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                Map[x, y] = (rand.Next(0, 100) < Treshold) ? 1 : 0;
            }
        }
    }

    void OnValidate() {
        GenerateMap();
    }

    void OnDrawGizmos() {
        if (Map == null) return;
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                Gizmos.color = (Map[x, y] == 1) ? Color.black : Color.white;
                Vector3 pos = new Vector3(-Width / 2 + x + 0.5f, 0, -Height / 2 + y + 0.5f);
                Gizmos.DrawCube(pos, Vector3.one);
            }
        }
    }

}