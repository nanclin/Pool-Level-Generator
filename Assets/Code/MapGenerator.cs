using UnityEngine;

public class MapGenerator : MonoBehaviour {

    [SerializeField] private MapRenderer MapRenderer;
    [SerializeField] private Texture2D InputTexture;
    [SerializeField] private int Width;
    [SerializeField] private  int Height;
    [Range(0, 1)] [SerializeField] private float Treshold;
    [SerializeField] public bool AutoUpdate;

    private float[,] Map;

    public void GenerateMap() {
        Map = new float[Width, Height];

        for (int y = 0; y < Height; y++) {
            for (int x = 0; x < Width; x++) {
                Map[x, y] = InputTexture.GetPixel(x, y).grayscale < Treshold ? 1 : 0;
            }
        }
    }

    public void RenderMap() {
        GenerateMap();
        MapRenderer.RenderHeightMap(Map);
    }
}