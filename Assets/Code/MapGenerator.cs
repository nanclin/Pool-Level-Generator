using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MapGenerator : MonoBehaviour {

    private Vector2[] FourWayOffsets = new Vector2[4] {
        new Vector2(0, 1),
        new Vector2(1, 0),
        new Vector2(0, -1),
        new Vector2(-1, 0)
    };

    private Vector2[] EightWayOffsets = new Vector2[8] {
        new Vector2(0, 1),
        new Vector2(1, 1),
        new Vector2(1, 0),
        new Vector2(1, -1),
        new Vector2(0, -1),
        new Vector2(-1, -1),
        new Vector2(-1, 0),
        new Vector2(-1, 1)
    };

    [SerializeField] private MapRenderer InputMapRenderer;
    [SerializeField] private MapRenderer DebugMapRenderer;
    [SerializeField] private Texture2D InputImage;
    [SerializeField] private int Width;
    [SerializeField] private  int Height;
    [Range(0, 1)] [SerializeField] private float Treshold;
    [SerializeField] public bool AutoUpdate;
    [SerializeField] public bool DrawGizmoLine;

    private List<Vector2> Contour;
    private Color[,] DebugMap;
    int floodFillIterationLimit;

    public int[,] GenerateMap() {
        int[,] map = new int[Width, Height];
        DebugMap = new Color[Width, Height];

        for (int y = 0; y < Height; y++) {
            for (int x = 0; x < Width; x++) {
                if (x == 0 || x == Width - 1 || y == 0 || y == Height - 1) {
                    map[x, y] = 0;
                    continue;
                }
                map[x, y] = InputImage.GetPixel(x, y).grayscale > Treshold ? 1 : 0;
                DebugMap[x, y] = Color.clear;
            }
        }
        return map;
    }

    public List<Vector2> GetContour(int[,] inputMap, bool logs = false) {
        if (logs) Debug.Log("-----GET CONTOUR-----");

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
                DebugMap[x, y] = Color.cyan;
                if (currentPosValue == 1) {
                    startPos = currentPos;
                    if (logs) Debug.Log(string.Format("startPos={0}", startPos));
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
                bool isVisited = contour.Contains(currentPos);
                if (!isVisited) {
                    DebugMap[(int) currentPos.x, (int) currentPos.y] = Color.blue;
                    contour.Add(currentPos);
                    if (logs) Debug.Log(string.Format("added to contour: currentPos={0}", currentPos));
                }
                currentPos += GetLeftOffset(directionIndex);
                if (logs) Debug.Log(string.Format("going left, new pos={0}", currentPos));
            } else {// go right
                currentPos += GetRightOffset(directionIndex);
                if (logs) Debug.Log(string.Format("going right, new pos={0}", currentPos));
            }
            int x = (int) currentPos.x;
            int y = (int) currentPos.y;
            currentPosValue = inputMap[x, y];

            IsReachedStart = currentPos == startPos;
            if (IsReachedStart) if (logs) Debug.Log("Contour found!");

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

    private void FloodFill(int[,] map, Vector2 node, int target, int replacement, bool logs = false) {

        if (target == replacement) {
            if (logs) Debug.Log("Region already has given value!");
            return;
        }

        int x = (int) node.x;
        int y = (int) node.y;
    
        if (logs) Debug.Log(string.Format("visited x={0} y={1}", x, y));
    
        if (x < 0 || x >= Width || y < 0 || y >= Height) {
            if (logs) Debug.Log("Index out of range");
            return;
        }
    
        // mark visited
        //        DebugMap[x, y] = Color.magenta;
    
        if (--floodFillIterationLimit <= 0) {
            if (logs) Debug.Log("Too many iterations!");
            return;
        }
    
        if (map[x, y] == replacement) {
            if (logs) Debug.Log("Already has wanted value!");
            return;
        }

        if (DebugMap[x, y].a > 0) {
            if (logs) Debug.Log("Already visited!");
            return;
        }

        if (map[x, y] != target) {
            if (logs) Debug.Log("Pixel from another region!");
            return;
        }

        if (CountFillNeighbours(map, node) < 8) {
            if (logs) Debug.Log("Near the edge!");
            return;
        }

        if (logs) Debug.Log("Replaced");
        //        map[x, y] = replacement;
        DebugMap[x, y] = Color.cyan;

        if (logs) Debug.Log("Going up!");
        FloodFill(map, node + FourWayOffsets[0], target, replacement);
        if (logs) Debug.Log("Going right!");
        FloodFill(map, node + FourWayOffsets[1], target, replacement);
        if (logs) Debug.Log("Going down!");
        FloodFill(map, node + FourWayOffsets[2], target, replacement);
        if (logs) Debug.Log("Going left!");
        FloodFill(map, node + FourWayOffsets[3], target, replacement);
        return;
    }

    private int CountFillNeighbours(int[,] map, Vector2 position) {
        int count = 0;
        for (int i = 0; i < EightWayOffsets.Length; i++) {
            Vector2 newPos = position + EightWayOffsets[i];
            int x = (int) newPos.x;
            int y = (int) newPos.y;
            bool isFull = map[x, y] == 1;
            if (isFull) count++;
        }
        return count;
    }

    public void RenderMap() {
        int[,] map = GenerateMap();
        InputMapRenderer.RenderBitMap(map);

        Contour = GetContour(map);

        Vector2 floodFillSourcePos = Contour[0];
        for (int i = 0; i < EightWayOffsets.Length; i++) {
            Vector2 newPos = floodFillSourcePos + EightWayOffsets[i];
            int x = (int) newPos.x;
            int y = (int) newPos.y;
            if (DebugMap[x, y].a == 0) {
                floodFillSourcePos = newPos;
                break;
            }
        }

//        Vector2 floodFillSourcePos = new Vector2(5, 5);
        floodFillIterationLimit = 30000;
        FloodFill(map, floodFillSourcePos, 1, 0);

        DebugMapRenderer.RenderColourMap(DebugMap);
    }

    void OnDrawGizmos() {
        if (!DrawGizmoLine) return;
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