using System.Collections.Generic;
using UnityEngine;

public class Cell {

    private int Width = 128;
    private int Height = 128;

    public int X;
    public int Y;
    public int Value;

    public int Index{ get { return Y * Width + X; } }

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

public class QuadTree {

    public float X;
    public float Y;
    public float Width;
    public float Height;

    public List<Cell> Cells;

    public QuadTree QuadTree1;
    public QuadTree QuadTree2;
    public QuadTree QuadTree3;
    public QuadTree QuadTree4;

    public  QuadTree(List<Cell> cells, float x, float y, float width, float height, int maxCellPerQuad = 1) {

        X = x;
        Y = y;
        Width = width;
        Height = height;

        if (cells.Count > maxCellPerQuad) {

            List<Cell> quadrant1Cells = new List<Cell>();
            List<Cell> quadrant2Cells = new List<Cell>();
            List<Cell> quadrant3Cells = new List<Cell>();
            List<Cell> quadrant4Cells = new List<Cell>();

            float widthHalf = width * 0.5f;
            float heightHalf = height * 0.5f;
            float x1 = x;
            float y1 = y;
            float x2 = x + widthHalf;
            float y2 = y;
            float x3 = x;
            float y3 = y + heightHalf;
            float x4 = x + widthHalf;
            float y4 = y + heightHalf;

            for (int i = 0; i < cells.Count; i++) {
                Cell cell = cells[i];
            
                if (cell.X >= x1 && cell.X < x1 + widthHalf && cell.Y >= y1 && cell.Y < y1 + heightHalf) {
                    quadrant1Cells.Add(cell);
                } else if (cell.X >= x2 && cell.X < x2 + widthHalf && cell.Y >= y1 && cell.Y < y1 + heightHalf) {
                    quadrant2Cells.Add(cell);
                } else if (cell.X >= x3 && cell.X < x3 + widthHalf && cell.Y >= y3 && cell.Y < y3 + heightHalf) {
                    quadrant3Cells.Add(cell);
                } else if (cell.X >= x4 && cell.X < x4 + widthHalf && cell.Y >= y4 && cell.Y < y4 + heightHalf) {
                    quadrant4Cells.Add(cell);
                }
            }
            QuadTree1 = new QuadTree(quadrant1Cells, x1, y1, widthHalf, heightHalf, maxCellPerQuad);
            QuadTree2 = new QuadTree(quadrant2Cells, x2, y2, widthHalf, heightHalf, maxCellPerQuad);
            QuadTree3 = new QuadTree(quadrant3Cells, x3, y3, widthHalf, heightHalf, maxCellPerQuad);
            QuadTree4 = new QuadTree(quadrant4Cells, x4, y4, widthHalf, heightHalf, maxCellPerQuad);
        } else {
            Cells = cells;
        }
    }

    public void DrawQuadTreeGizmos(float width, float height, int d = 0) {
        
        Color color = Color.green;

        float widthHalf = width * 0.5f;
        float heightHalf = height * 0.5f;

        if (d == 0) {//draw frame
            Debug.DrawLine(new Vector2(X - widthHalf, Y - heightHalf), new Vector2(X + Width - widthHalf, Y - heightHalf), color);
            Debug.DrawLine(new Vector2(X - widthHalf, Y + Height - heightHalf), new Vector2(X + Width - widthHalf, Y + Height - heightHalf), color);
            Debug.DrawLine(new Vector2(X - widthHalf, Y - heightHalf), new Vector2(X - widthHalf, Y + Height - heightHalf), color);
            Debug.DrawLine(new Vector2(X + Width - widthHalf, Y - heightHalf), new Vector2(X + Width - widthHalf, Y + Height - heightHalf), color);
        }

        if (Cells == null) {//draw cross
            Debug.DrawLine(new Vector2(X - widthHalf, Y + Height * 0.5f - heightHalf), new Vector2(X + Width - widthHalf, Y + Height * 0.5f - heightHalf), color);
            Debug.DrawLine(new Vector2(X + Width * 0.5f - widthHalf, Y - heightHalf), new Vector2(X + Width * 0.5f - widthHalf, Y + Height - heightHalf), color);
        }

//        if (Cells != null) {//draw cell positions
//            for (int i = 0; i < Cells.Count; i++) {
//                Cell cell = Cells[i];
//                float x = cell.X;
//                float y = cell.Y;
//                float crossSize = 0.3f;
//                Debug.DrawLine(new Vector2(x - crossSize - widthHalf, y - crossSize - heightHalf), new Vector2(x + crossSize - widthHalf, y + crossSize - heightHalf), Color.green);
//                Debug.DrawLine(new Vector2(x + crossSize - widthHalf, y - crossSize - heightHalf), new Vector2(x - crossSize - widthHalf, y + crossSize - heightHalf), Color.green);
//            }
//        }
//
        if (QuadTree1 != null) QuadTree1.DrawQuadTreeGizmos(width, height, d + 1);
        if (QuadTree2 != null) QuadTree2.DrawQuadTreeGizmos(width, height, d + 2);
        if (QuadTree3 != null) QuadTree3.DrawQuadTreeGizmos(width, height, d + 3);
        if (QuadTree4 != null) QuadTree4.DrawQuadTreeGizmos(width, height, d + 4);
    }
}

public class MapGenerator : MonoBehaviour {

    //    private Vector2[] FourWayOffsets = new Vector2[4] {
    //        new Vector2(0, 1),
    //        new Vector2(1, 0),
    //        new Vector2(0, -1),
    //        new Vector2(-1, 0)
    //    };

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

    private List<List<Cell>> Outlines;
    private Color[,] DebugMap;
    private HashSet<int> Visited;
    private QuadTree QuadTree;

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

        List<Cell> allOutlineCells = new List<Cell>();
        for (int i = 0; i < Outlines.Count; i++) {
            for (int j = 0; j < Outlines[i].Count; j++) {
                allOutlineCells.Add(Outlines[i][j]);
            }
        }

        QuadTree = new QuadTree(allOutlineCells, 0, 0, Width, Height);
//        QuadTree = new QuadTree(
//            new List<Cell> {
//                new Cell(30, 30, 0),
//                new Cell(60, 60, 0),
//                new Cell(30, 60, 0),
//            },
//            0, 0, 128, 128);

        DebugMapRenderer.RenderColourMap(DebugMap);
    }

    private void DrawOutlinesGizmos(List<List<Cell>> outlines) {
        if (!DrawGizmoLine) return;
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
            QuadTree.DrawQuadTreeGizmos(Width, Height);
        }
    }
}