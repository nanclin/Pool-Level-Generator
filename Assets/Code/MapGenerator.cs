using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

    [SerializeField] private MapRenderer MapRenderer;
    [SerializeField] private MapRenderer ContourMapRenderer;
    [SerializeField] private Texture2D InputTexture;
    [SerializeField] private int Width;
    [SerializeField] private  int Height;
    [Range(0, 1)] [SerializeField] private float Treshold;
    [SerializeField] public bool AutoUpdate;

    private List<Vector2> Contour;

    public int[,] GenerateMap() {
        int[,] map = new int[Width, Height];

        for (int y = 0; y < Height; y++) {
            for (int x = 0; x < Width; x++) {
                map[x, y] = InputTexture.GetPixel(x, y).grayscale > Treshold ? 1 : 0;
            }
        }
        return map;
    }

    public List<Vector2> GetContour(int[,] inputMap) {
        List<Vector2> contour = new List<Vector2>();

        Vector2 startPos = new Vector2(0, -1);
        Vector2 previousPos = startPos;
        Vector2 currentPos = new Vector2(0, 0);
        int currentPosValue = -1;
        int directionIndex = -1;

        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                currentPos = new Vector2(x, y);
                currentPosValue = inputMap[x, y];
                directionIndex = GetDirectionIndex(currentPos, previousPos);
                if (currentPosValue == 1) {
                    startPos = currentPos;
                    Debug.Log(string.Format("startPos={0}", startPos));
                    break;
                }
                previousPos = currentPos;
            }
            if (currentPosValue == 1) break;
        }

        int iterationLimit = 1000;
        bool IsReachedStart = false;

        while (!IsReachedStart) {

            directionIndex = GetDirectionIndex(currentPos, previousPos);
            previousPos = currentPos;

            if (currentPosValue == 1) {// go left
                if (!contour.Contains(currentPos)) {
                    contour.Add(currentPos);
                    Debug.Log(string.Format("added to contour: currentPos={0}", currentPos));
                }
                currentPos += GetLeftOffset(directionIndex);
                Debug.Log(string.Format("going left, new pos={0}", currentPos));
            } else {// go right
                currentPos += GetRightOffset(directionIndex);
                Debug.Log(string.Format("going right, new pos={0}", currentPos));
            }
            int x = (int) currentPos.x;
            int y = (int) currentPos.y;
            currentPosValue = inputMap[x, y];

            IsReachedStart = currentPos == startPos;

            if (--iterationLimit <= 0) break;
        }

        return contour;
    }

    private int GetDirectionIndex(Vector2 currentPos, Vector2 previousPos) {
        Vector2 distance = currentPos - previousPos;
        if (distance.x == 0 && distance.y == 1) return 0;
        if (distance.x == 1 && distance.y == 0) return 1;
        if (distance.x == 0 && distance.y == -1) return 2;
        if (distance.x == -1 && distance.y == 0) return 3;
        return -1;
    }

    private Vector2 GetLeftOffset(int directionIndex) {
        if (directionIndex == 0) return new Vector2(-1, 0);
        if (directionIndex == 1) return new Vector2(0, 1);
        if (directionIndex == 2) return new Vector2(1, 0);
        if (directionIndex == 3) return new Vector2(0, -1);
        return Vector2.zero;
    }

    private Vector2 GetRightOffset(int directionIndex) {
        if (directionIndex == 0) return new Vector2(1, 0);
        if (directionIndex == 1) return new Vector2(0, -1);
        if (directionIndex == 2) return new Vector2(-1, 0);
        if (directionIndex == 3) return new Vector2(0, 1);
        return Vector2.zero;
    }

    public void RenderMap() {
        int[,] map = GenerateMap();
        MapRenderer.RenderBitMap(map);
        Contour = GetContour(map);
    }

    void OnDrawGizmos() {
        if (Contour == null) return;
        Vector2 previousPoint = Contour[0];
        float scale = 0.01f;
        previousPoint.x *= Width * scale;
        previousPoint.y *= Height * scale;
        previousPoint.x -= Width * 0.5f - 0.5f;
        previousPoint.y -= Height * 0.5f - 0.5f;
        for (int i = 0; i < Contour.Count; i++) {
            Vector2 point = Contour[i];
            point.x *= Width * scale;
            point.y *= Height * scale;
            point.x -= Width * 0.5f - 0.5f;
            point.y -= Height * 0.5f - 0.5f;
            Debug.DrawLine(previousPoint, point, Color.green);
            previousPoint = point;
        }
    }
}