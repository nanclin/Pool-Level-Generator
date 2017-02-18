using UnityEngine;
using System.Collections;

public class MapRenderer : MonoBehaviour {

    [SerializeField] private Renderer Renderer;

    public void RenderBitMap(int[,] map) {
        int width = map.GetLength(0);
        int height = map.GetLength(1);

        Texture2D texture = new Texture2D(width, height);

        Color[] colourMap = new Color[width * height];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                colourMap[y * width + x] = (map[x, y] == 1) ? Color.white : Color.black;
            }
        }

        texture.filterMode = FilterMode.Point;
        texture.SetPixels(colourMap);
        texture.Apply();

        Renderer.sharedMaterial.mainTexture = texture;
        Renderer.transform.localScale = new Vector3(width, 1, height) * 0.1f;
    }
}
