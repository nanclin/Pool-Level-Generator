using UnityEngine;
using System.Collections.Generic;

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

    public int Value { get; private set; }

    public int MaxDepth { get; private set; }

    public bool IsLeaf {
        get {
            return
                QuadTree1 == null &&
            QuadTree2 == null &&
            QuadTree3 == null &&
            QuadTree4 == null;
        }
    }

    public  QuadTree(List<Cell> cells, float x, float y, float width, float height, int maxDepth, int maxCellPerQuad = 4, int d = 0) {

        X = x;
        Y = y;
        Width = width;
        Height = height;
        MaxDepth = maxDepth;

        bool fullQuad = cells.Count >= Mathf.Pow(Width, 2);
        Value = fullQuad ? 1 : 0;

        if (cells.Count > 1 && d < maxDepth && (!fullQuad || true)) {//split condition
            //        if (cells.Count > maxCellPerQuad) {//split condition

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
            QuadTree1 = new QuadTree(quadrant1Cells, x1, y1, widthHalf, heightHalf, maxDepth, maxCellPerQuad, d + 1);
            QuadTree2 = new QuadTree(quadrant2Cells, x2, y2, widthHalf, heightHalf, maxDepth, maxCellPerQuad, d + 1);
            QuadTree3 = new QuadTree(quadrant3Cells, x3, y3, widthHalf, heightHalf, maxDepth, maxCellPerQuad, d + 1);
            QuadTree4 = new QuadTree(quadrant4Cells, x4, y4, widthHalf, heightHalf, maxDepth, maxCellPerQuad, d + 1);
        } else {
            Cells = cells;
        }
    }

#region Draw gizmos

    public void DrawQuadTreeGizmos(float width, float height, int depth = 0) {

        Color color = Color.gray;

        Vector2 offset = new Vector2(-width * 0.5f, -height * 0.5f);

        Vector2 a = new Vector2(X, Y) + offset;
        Vector2 b = new Vector2(X + Width, Y) + offset;
        Vector2 c = new Vector2(X, Y + Height) + offset;
        Vector2 d = new Vector2(X + Width, Y + Height) + offset;

        if (depth == 0) {//draw frame

            Debug.DrawLine(a, b, color);
            Debug.DrawLine(c, d, color);
            Debug.DrawLine(a, c, color);
            Debug.DrawLine(b, d, color);
        }

        if (Cells == null) {//draw cross
            Debug.DrawLine(new Vector2(X, Y + Height * 0.5f) + offset, new Vector2(X + Width, Y + Height * 0.5f) + offset, color);
            Debug.DrawLine(new Vector2(X + Width * 0.5f, Y) + offset, new Vector2(X + Width * 0.5f, Y + Height) + offset, color);
        }

        //        if (Cells != null) {//draw cell positions
        //            Vector2 pixelOffset = Vector2.one * 0.5f;
        //            for (int i = 0; i < Cells.Count; i++) {
        //                Cell cell = Cells[i];
        //                float x = cell.X;
        //                float y = cell.Y;
        //                float crossSize = 0.3f;
        //                Debug.DrawLine(new Vector2(x - crossSize, y - crossSize) + offset + pixelOffset, new Vector2(x + crossSize, y + crossSize) + offset + pixelOffset, Color.green);
        //                Debug.DrawLine(new Vector2(x + crossSize, y - crossSize) + offset + pixelOffset, new Vector2(x - crossSize, y + crossSize) + offset + pixelOffset, Color.green);
        //            }
        //        }

        if (Value == 1) {
            Debug.DrawLine(a, d, Color.gray);
            Debug.DrawLine(b, c, Color.gray);
        }

        if (QuadTree1 != null) QuadTree1.DrawQuadTreeGizmos(width, height, depth + 1);
        if (QuadTree2 != null) QuadTree2.DrawQuadTreeGizmos(width, height, depth + 2);
        if (QuadTree3 != null) QuadTree3.DrawQuadTreeGizmos(width, height, depth + 3);
        if (QuadTree4 != null) QuadTree4.DrawQuadTreeGizmos(width, height, depth + 4);
    }

    public void DrawQuadTreeTriangulation(float width, float height) {

        if (Cells != null) {
            if (Cells.Count == 1) {
                Color color = Color.black;

                float widthHalf = width * 0.5f;
                float heightHalf = height * 0.5f;

                Vector2 offset = new Vector2(-widthHalf, -heightHalf);

                Vector2 a = new Vector2(X, Y) + offset;
                Vector2 b = new Vector2(X + Width, Y) + offset;
                Vector2 c = new Vector2(X, Y + Height) + offset;
                Vector2 d = new Vector2(X + Width, Y + Height) + offset;

                Vector2 point = Cells[0].GetPosition();

                Debug.DrawLine(a, point, color);
                Debug.DrawLine(b, point, color);
                Debug.DrawLine(c, point, color);
                Debug.DrawLine(d, point, color);
            }
        } else {
            QuadTree1.DrawQuadTreeTriangulation(width, height);
            QuadTree2.DrawQuadTreeTriangulation(width, height);
            QuadTree3.DrawQuadTreeTriangulation(width, height);
            QuadTree4.DrawQuadTreeTriangulation(width, height);
        }
    }

#endregion
}
