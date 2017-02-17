using UnityEngine;

public class MapGenerator : MonoBehaviour {

    [SerializeField] private MapRenderer MapRenderer;
    [SerializeField] private int Width;
    [SerializeField] private  int Height;
    [SerializeField] public string Seed;
    [Range(0, 100)] [SerializeField] private int Treshold;
    [SerializeField] public bool AutoUpdate;

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

    public void RenderMap() {
        GenerateMap();
        MapRenderer.RenderMap(Map);
    }
}