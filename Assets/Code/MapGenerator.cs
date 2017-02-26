using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

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

    [SerializeField] private MapRenderer ImageMapRenderer;
    [SerializeField] private MapRenderer DebugMapRenderer;
    [SerializeField] private Texture2D InputImage;
    [SerializeField] private MeshGenerator MeshGenerator;
    [SerializeField] private int Width;
    [SerializeField] private  int Height;
    [Range(0, 1)] [SerializeField] private float Treshold;
    [SerializeField] public bool AutoUpdateGizmos;
    [SerializeField] public bool DrawOutlines;
    [SerializeField] public bool DrawTriangulationLines;
    [SerializeField] public bool DrawQuadTree;
    [SerializeField] public int MaxDepth = 5;
    [Range(1, 10)][SerializeField] public int OutlineDetail = 1;

    private List<List<Cell>> Outlines;
    private Color[,] DebugMap;
    private HashSet<int> Visited;
    private QuadTree QuadTree;

    public int[,] GenerateMapFromImage(Texture2D inputImage) {
        int[,] map = new int[Width, Height];
        Visited = new HashSet<int>();
        DebugMap = new Color[Width, Height];

        for (int y = 0; y < Height; y++) {
            for (int x = 0; x < Width; x++) {
                if (x == 0 || x == Width - 1 || y == 0 || y == Height - 1) {
                    map[x, y] = 0;
                    continue;
                }
                map[x, y] = inputImage.GetPixel(x, y).grayscale > Treshold ? 1 : 0;
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

    private List<Cell> FollowEdge(int[,] map, Cell cell, List<Cell> outline = null, int d = 0) {
        
        if (outline == null) outline = new List<Cell>();

        Visited.Add(cell.Index);

        if (d % 5 == 0) {
            outline.Add(cell);
            DebugMap[cell.X, cell.Y] = Color.red;
        }

        Cell nextCell = GetNextEdgeCell(map, cell);

        if (nextCell != null) {
            outline = FollowEdge(map, nextCell, outline, d + 1);
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
//        MeshGenerator.GenerateMesh();
//        return;

        int[,] map = GenerateMapFromImage(InputImage);
        ImageMapRenderer.RenderBitMap(map);

//        Outlines = FindAllOutlines(map);
//
//        List<Cell> allOutlineCells = new List<Cell>();
//        for (int i = 0; i < Outlines.Count; i++) {
//            for (int j = 0; j < Outlines[i].Count; j++) {
//                allOutlineCells.Add(Outlines[i][j]);
//            }
//        }

        List<Cell> fillPixels = new List<Cell>();
        for (int y = 0; y < Height; y++) {
            for (int x = 0; x < Width; x++) {
                if (map[x, y] == 0)
                    fillPixels.Add(new Cell(x, y, 0));
            }
        }

        QuadTree = new QuadTree(fillPixels, 0, 0, Width, Height, MaxDepth);

        DebugMapRenderer.RenderColourMap(DebugMap);

        MeshGenerator.GenerateQuadTreeMesh(QuadTree);
    }

    private void DrawOutlinesGizmos(List<List<Cell>> outlines) {
        if (!DrawOutlines) return;
        if (outlines == null) return;

        for (int i = 0; i < outlines.Count; i++) {
            List<Cell> outline = outlines[i];

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

    void OnDrawGizmos() {
        DrawOutlinesGizmos(Outlines);

        if (QuadTree != null) {
            if (DrawQuadTree)
                QuadTree.DrawQuadTreeGizmos(Width, Height);
            if (DrawTriangulationLines)
                QuadTree.DrawQuadTreeTriangulation(Width, Height);
        }
    }
}