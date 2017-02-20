using System.Collections.Generic;
using UnityEngine;

public class Cell {

    private int Width = 100;
    private int Height = 100;

    public int X;
    public int Y;
    public int Value;

    public int Index{ get { return Y * 100 + X; } }

    public Cell(int x, int y, int value) {
        X = x;
        Y = y;
        Value = value;
    }

    public Vector2 GetPosition() {
        float x = X - Width / 2 + 0.5f;
        float y = Y - Height / 2 + 0.5f;
        return new Vector2(x, y);
    }

    public override string ToString() {
        return string.Format("[Cell: X={0} Y={1} Value={2} Index={3}]", X, Y, Value, Index);
    }
}

public class MapGenerator : MonoBehaviour {

    private Vector2[] FourWayOffsets = new Vector2[4] {
        new Vector2(0, 1),
        new Vector2(1, 0),
        new Vector2(0, -1),
        new Vector2(-1, 0)
    };

    private int[][] EightWayOffsets = new int[8][] {
        new int[] { 0, 1 },
        new int[] { 1, 0 },
        new int[] { 0, -1 },
        new int[] { -1, 0 },
        new int[] { 1, 1 },
        new int[] { 1, -1 },
        new int[] { -1, -1 },
        new int[] { -1, 1 },
    };

    [SerializeField] private MapRenderer InputMapRenderer;
    [SerializeField] private MapRenderer DebugMapRenderer;
    [SerializeField] private Texture2D InputImage;
    [SerializeField] private int Width;
    [SerializeField] private  int Height;
    [Range(0, 1)] [SerializeField] private float Treshold;
    [SerializeField] public bool AutoUpdateGizmos;
    [SerializeField] public bool DrawGizmoLine;
    [Range(1, 10)][SerializeField] public int OutlineDetail = 1;

    List<List<Cell>> Outlines;
    private Color[,] DebugMap;
    HashSet<int> Visited;

    public int[,] GenerateMap() {
        int[,] map = new int[Width, Height];
        Visited = new HashSet<int>();
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

    private List<List<Cell>> FindAllOutlines(int[,] map) {

        List<List<Cell>> outlines = new List<List<Cell>>();

        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {

                //                DebugMap[x, y] = Color.cyan;//mark visited

                Cell cell = new Cell(x, y, map[x, y]);

                if (!Visited.Contains(cell.Index)) {

                    if (map[x, y] == 0) {

                        if (HasEmptyNeighbour(map, cell)) {

                            List<Cell> outline = FollowEdge(map, cell);
                            outlines.Add(outline);
                            DebugMap[x, y] = Color.red;
                        }
                    }
                }
            }
        }

        return outlines;
    }

    private List<Cell> FollowEdge(int[,] map, Cell cell, List<Cell> outline = null) {
        
        if (outline == null) outline = new List<Cell>();

        Visited.Add(cell.Index);
        outline.Add(cell);
        DebugMap[cell.X, cell.Y] = Color.red;

        Cell nextCell = GetNextEdgeCell(map, cell);

        if (nextCell != null) {
            outline = FollowEdge(map, nextCell, outline);
        }

        return outline;
    }

    private Cell GetNextEdgeCell(int[,] map, Cell cell) {

        List<Cell> neighbourCells = GetNeighbourCells(map, cell);

        for (int i = 0; i < neighbourCells.Count; i++) {
            Cell neighbour = neighbourCells[i];

            if (Visited.Contains(neighbour.Index)) continue;

            if (neighbour.Value == 0) {
                if (HasEmptyNeighbour(map, neighbour)) {
                    return neighbour;
                }
            }
        }

        return null;
    }

    private List<Cell> GetNeighbourCells(int[,] map, Cell cell) {

        List<Cell> neighbourCells = new List<Cell>();

        for (int i = 0; i < EightWayOffsets.Length; i++) {

            int x = cell.X + EightWayOffsets[i][0];
            int y = cell.Y + EightWayOffsets[i][1];

            if (x < 0 || x >= Width || y < 0 || y >= Height) continue;

            neighbourCells.Add(new Cell(x, y, map[x, y]));
        }

        return neighbourCells;
    }

    private bool HasEmptyNeighbour(int[,] map, Cell cell) {

        List<Cell> neighbour = GetNeighbourCells(map, cell);

        for (int i = 0; i < neighbour.Count; i++) {
            if (neighbour[i].Value > 0) return true;
        }

        return false;
    }

    public void RenderMap() {
        int[,] map = GenerateMap();
        InputMapRenderer.RenderBitMap(map);

        Outlines = FindAllOutlines(map);

        DebugMapRenderer.RenderColourMap(DebugMap);
    }

    void OnDrawGizmos() {
        if (!DrawGizmoLine) return;
        if (Outlines == null) return;

        for (int i = 0; i < Outlines.Count; i++) {
            List<Cell> outline = Outlines[i];

            if (outline == null) continue;
            if (outline.Count == 0) continue;

            Cell firstPoint = outline[0];
            Cell point = firstPoint;
            Cell previousPoint = firstPoint;

            for (int j = 0; j < outline.Count; j += OutlineDetail) {
                point = outline[j];
                Debug.DrawLine(previousPoint.GetPosition(), point.GetPosition(), Color.green);
                previousPoint = point;
            }
            Debug.DrawLine(point.GetPosition(), firstPoint.GetPosition(), Color.red);
        }
    }
}