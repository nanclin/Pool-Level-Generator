using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class QuadTree {

    public float x { get; private set; }

    public float y { get; private set; }

    public QuadTree Root;

    public QuadTree[] SubQuadTrees;

    public List<Cell> Cells;

    public float Size { get; private set; }

    public int Value { get; private set; }

    public float Depth { get; private set; }

    public float Quadrant { get; private set; }

    public int NodeHeight { get; private set; }

    public int TreeHeight { get { return Root.NodeHeight; } }

    //    public string HistoryString { get; private set; }

    public int CellsPerSide { get { return (int) Mathf.Pow(2, TreeHeight - Depth); } }

    public bool IsLeaf {
        get {
            return SubQuadTrees == null;
        }
    }

    public QuadTree(List<Cell> cells, float x, float y, float size, int maxTreeHeight, int maxCellPerQuad = 1, QuadTree root = null, int depth = 0, int quadrant = -1, string toString = "") {

        this.x = x;
        this.y = y;
        Size = size;
        Depth = depth;
        Quadrant = quadrant;

//        Root = (root == null) ? this : root;
        Root = root ?? this;

        bool fullQuad = cells.Count >= Mathf.Pow(Size, 2);
        Value = fullQuad ? 1 : 0;

        if (cells.Count > 1 && depth < maxTreeHeight && (!fullQuad || false)) {//split condition
//        if (cells.Count > maxCellPerQuad) {//split condition

            List<Cell> quadrant1Cells = new List<Cell>();
            List<Cell> quadrant2Cells = new List<Cell>();
            List<Cell> quadrant3Cells = new List<Cell>();
            List<Cell> quadrant4Cells = new List<Cell>();

            float sizeHalf = Size * 0.5f;
            float x1 = x;
            float y1 = y;
            float x2 = x + sizeHalf;
            float y2 = y;
            float x3 = x;
            float y3 = y + sizeHalf;
            float x4 = x + sizeHalf;
            float y4 = y + sizeHalf;

            for (int i = 0; i < cells.Count; i++) {
                Cell cell = cells[i];

                if (cell.X >= x1 && cell.X < x1 + sizeHalf && cell.Y >= y1 && cell.Y < y1 + sizeHalf) {
                    quadrant1Cells.Add(cell);
                } else if (cell.X >= x2 && cell.X < x2 + sizeHalf && cell.Y >= y1 && cell.Y < y1 + sizeHalf) {
                    quadrant2Cells.Add(cell);
                } else if (cell.X >= x3 && cell.X < x3 + sizeHalf && cell.Y >= y3 && cell.Y < y3 + sizeHalf) {
                    quadrant3Cells.Add(cell);
                } else if (cell.X >= x4 && cell.X < x4 + sizeHalf && cell.Y >= y4 && cell.Y < y4 + sizeHalf) {
                    quadrant4Cells.Add(cell);
                }
            }
            QuadTree quadTree1 = new QuadTree(quadrant1Cells, x1, y1, sizeHalf, maxTreeHeight, maxCellPerQuad, Root, depth + 1, 0);
            QuadTree quadTree2 = new QuadTree(quadrant2Cells, x2, y2, sizeHalf, maxTreeHeight, maxCellPerQuad, Root, depth + 1, 1);
            QuadTree quadTree3 = new QuadTree(quadrant3Cells, x3, y3, sizeHalf, maxTreeHeight, maxCellPerQuad, Root, depth + 1, 2);
            QuadTree quadTree4 = new QuadTree(quadrant4Cells, x4, y4, sizeHalf, maxTreeHeight, maxCellPerQuad, Root, depth + 1, 3);
            SubQuadTrees = new QuadTree[] {
                quadTree1,
                quadTree2,
                quadTree3,
                quadTree4,
            };

            for (int i = 0; i < SubQuadTrees.Length; i++) {
                if (SubQuadTrees[i].NodeHeight > NodeHeight)
                    NodeHeight = SubQuadTrees[i].NodeHeight;
            }
            NodeHeight = NodeHeight + 1;

        } else {
            Cells = cells;
            Value = cells.Count;
        }
    }

    public int ValueAtPosition(float x, float y, int depth = 0, QuadTree quadTree = null) {

        if (depth == 0)
            Assert.IsTrue(x >= 0 && y >= 0 && x < Size && y < Size, string.Format("Position out of bounds! x={0} y={1} depth={2} Size={3}", x, y, depth, Size));

        if (depth > 10) {
            return-1;
        }
        if (IsLeaf) return Value;

        int ix = Mathf.FloorToInt(x / (Size * 0.5f));
        int iy = Mathf.FloorToInt(y / (Size * 0.5f));

        if (ix == 0 && iy == 0) return SubQuadTrees[0].ValueAtPosition(x, y, depth + 1);
        if (ix == 1 && iy == 0) return SubQuadTrees[1].ValueAtPosition(x, y, depth + 1);
        if (ix == 0 && iy == 1) return SubQuadTrees[2].ValueAtPosition(x, y, depth + 1);
        if (ix == 1 && iy == 1) return SubQuadTrees[3].ValueAtPosition(x, y, depth + 1);

        return -1;
    }

    public QuadTree QuadTreeAtPosition(float x, float y, int depth = 0, QuadTree quadTree = null) {

        if (depth == 0)
            Assert.IsTrue(x >= 0 && y >= 0 && x < Size && y < Size, string.Format("Position out of bounds! x={0} y={1} depth={2} Size={3}", x, y, depth, Size));
        
        if (IsLeaf) return this;

        int ix = Mathf.FloorToInt(x / (Size * 0.5f));
        int iy = Mathf.FloorToInt(y / (Size * 0.5f));

        if (ix == 0 && iy == 0) return SubQuadTrees[0].QuadTreeAtPosition(x, y, depth + 1);
        if (ix == 1 && iy == 0) return SubQuadTrees[1].QuadTreeAtPosition(x, y, depth + 1);
        if (ix == 0 && iy == 1) return SubQuadTrees[2].QuadTreeAtPosition(x, y, depth + 1);
        if (ix == 1 && iy == 1) return SubQuadTrees[3].QuadTreeAtPosition(x, y, depth + 1);

        return null;
    }

#region Draw gizmos

    public void DrawQuadTreeGizmos(float size, int depth = 0) {

        Color color = Color.gray;

        Vector2 offset = -Vector2.one * Root.Size * 0.5f;

        Vector2 a = new Vector2(x, y) + offset;
        Vector2 b = new Vector2(x + Size, y) + offset;
        Vector2 c = new Vector2(x, y + Size) + offset;
        Vector2 d = new Vector2(x + Size, y + Size) + offset;

        if (depth == 0) {//draw frame

            Debug.DrawLine(a, b, color);
            Debug.DrawLine(c, d, color);
            Debug.DrawLine(a, c, color);
            Debug.DrawLine(b, d, color);
        }

        if (Cells == null) {// draw split
            float sizeHalf = Size * 0.5f;
            Debug.DrawLine(new Vector2(x, y + sizeHalf) + offset, new Vector2(x + Size, y + sizeHalf) + offset, color);
            Debug.DrawLine(new Vector2(x + sizeHalf, y) + offset, new Vector2(x + sizeHalf, y + Size) + offset, color);
        }

//        if (Cells != null) {//draw cell positions
//            Vector2 pixelOffset = Vector2.one * 0.5f;
//            for (int i = 0; i < Cells.Count; i++) {
//                Cell cell = Cells[i];
//                float ix = cell.X;
//                float iy = cell.Y;
//                float crossSize = 0.1f;
//                Debug.DrawLine(new Vector2(ix - crossSize, iy - crossSize) + pixelOffset, new Vector2(ix + crossSize, iy + crossSize) + pixelOffset, Color.green);
//                Debug.DrawLine(new Vector2(ix + crossSize, iy - crossSize) + pixelOffset, new Vector2(ix - crossSize, iy + crossSize) + pixelOffset, Color.green);
//            }
//        }

//        if (Value == 1) {//draw cross
//            Debug.DrawLine(a, d, Color.gray);
//            Debug.DrawLine(b, c, Color.gray);
//        }

        if (!IsLeaf) {
            SubQuadTrees[0].DrawQuadTreeGizmos(size, depth + 1);
            SubQuadTrees[1].DrawQuadTreeGizmos(size, depth + 2);
            SubQuadTrees[2].DrawQuadTreeGizmos(size, depth + 3);
            SubQuadTrees[3].DrawQuadTreeGizmos(size, depth + 4);
        }
    }

#endregion

    public override string ToString() {
        return QuadTreeToString();
    }

    private string QuadTreeToString(int depth = 0, string toString = "") {

        for (int t = 0; t < depth; t++)
            toString += "\t";
        
        toString += string.Format("[QuadTree: x={0}, y={1}, Size={2}, Value={3}, Depth={4}, Quadrant={5}, TreeHeight={6} NodeHeight={7}, CellsPerSide={8}, IsLeaf={9}]\n", x, y, Size, Value, Depth, Quadrant, TreeHeight, NodeHeight, CellsPerSide, IsLeaf);

        if (!IsLeaf) {
            toString += SubQuadTrees[0].QuadTreeToString(depth + 1);
            toString += SubQuadTrees[1].QuadTreeToString(depth + 1);
            toString += SubQuadTrees[2].QuadTreeToString(depth + 1);
            toString += SubQuadTrees[3].QuadTreeToString(depth + 1);
        }

        return toString;
    }
}
