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
