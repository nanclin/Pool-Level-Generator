using UnityEngine;
using System.Collections;

public class LinkedQuadTree {

    public readonly LinkedQuadTreeNode[] Nodes;
    public readonly int Height;

    public LinkedQuadTree(Vector2 position, float size, int height, int depth = 0, int index = 0) {

        if (depth == 0) {
            Height = height;
            int length = 0;
            for (int i = 0; i < height; i++) {
                length += (int) Mathf.Pow(4, i);
            }

            Nodes = new LinkedQuadTreeNode[length];
        }

        BuildQuadTreeRecursively(position, size, height, depth, index, Nodes);
    }

    private void BuildQuadTreeRecursively(Vector2 position, float size, int height, int depth = 0, int index = 0, LinkedQuadTreeNode[] nodes = null) {

        nodes[index] = new LinkedQuadTreeNode(position, size, depth, index);

        if (depth + 1 >= height) {
            return;
        }

        float sizeHalf = size * 0.5f;
        Vector2 position1 = position;
        Vector2 position2 = position + Vector2.right * sizeHalf;
        Vector2 position3 = position + Vector2.up * sizeHalf;
        Vector2 position4 = position + Vector2.one * sizeHalf;

        int nextIndex = 4 * index;
        BuildQuadTreeRecursively(position1, sizeHalf, height, depth + 1, nextIndex + 1, nodes);
        BuildQuadTreeRecursively(position2, sizeHalf, height, depth + 1, nextIndex + 2, nodes);
        BuildQuadTreeRecursively(position3, sizeHalf, height, depth + 1, nextIndex + 3, nodes);
        BuildQuadTreeRecursively(position4, sizeHalf, height, depth + 1, nextIndex + 4, nodes);
    }

    public void DrawGizmos() {

        for (int i = Nodes.Length - 1; i >= 0; i--) {
            LinkedQuadTreeNode node = Nodes[i];

            Vector2 position = node.Position;
            float size = node.Size;
            Color colorFade = Color.Lerp(Color.black, Color.white, 1f - ((float) node.Depth / (float) Height));

            Vector2 a = position;
            Vector2 b = position + Vector2.right * size;
            Vector2 c = position + Vector2.up * size;
            Vector2 d = position + Vector2.one * size;

            Debug.DrawLine(a, b, colorFade);
            Debug.DrawLine(b, d, colorFade);
            Debug.DrawLine(d, c, colorFade);
            Debug.DrawLine(c, a, colorFade);
        }
    }
}

public class LinkedQuadTreeNode {

    public readonly Vector2 Position;
    public readonly float Size;
    public readonly int Depth;
    public readonly int Index;

    public LinkedQuadTreeNode(Vector2 position, float size, int depth, int index) {
        Position = position;
        Size = size;
        Depth = depth;
        Index = index;
    }

    public override string ToString() {
        return string.Format("[Node] position={0}, size={1}, depth={2}, index={3}", Position, Size, Depth, Index);
    }
}
