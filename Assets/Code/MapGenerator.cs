using UnityEngine;

public class MapGenerator : MonoBehaviour {

    [SerializeField] private MapRenderer MapRenderer;
    [SerializeField] private Texture2D InputTexture;
    [SerializeField] private int Width;
    [SerializeField] private  int Height;
    [Range(0, 1)] [SerializeField] private float Treshold;
    [SerializeField] public bool AutoUpdate;

    public float[,] GenerateMap() {
        float[,] map = new float[Width, Height];

        for (int y = 0; y < Height; y++) {
            for (int x = 0; x < Width; x++) {
                map[x, y] = InputTexture.GetPixel(x, y).grayscale < Treshold ? 1 : 0;
            }
        }
        return map;
    }

    public void RenderMap() {
        float[,] map = GenerateMap();
        MapRenderer.RenderHeightMap(map);
    }
}