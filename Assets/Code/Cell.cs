using UnityEngine;

public class Cell {

    public int QuadTreeSize { get; private set; }

    public int X;
    public int Y;
    public int Value;

    public int Index{ get { return Y * QuadTreeSize + X; } }

    public Cell(int x, int y, int quadTreeSize, int value) {
        X = x;
        Y = y;
        Value = value;
    }

    public Vector2 GetPosition() {
        float QuadTreeSizeHalf = QuadTreeSize * 0.5f;
        float x = X - QuadTreeSizeHalf + 0.5f;
        float y = Y - QuadTreeSizeHalf + 0.5f;
        return new Vector2(x, y);
    }

    public override string ToString() {
        return string.Format("[Cell: X={0} Y={1} Value={2} Index={3}]", X, Y, Value, Index);
    }
}
