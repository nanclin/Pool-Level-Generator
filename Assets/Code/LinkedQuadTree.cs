﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class LinkedQuadTree {

    public readonly LinkedQuadTreeNode[] Nodes;
    public readonly int Height;

    private LinkedQuadTreeNode[] LeafNodes;

    public LinkedQuadTree(Vector2 position, float size, int height, int depth = 0, int index = 0) {

        if (depth == 0) {
            Height = height;
            int length = GetNumberOfTreeNodes(height);
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

    public bool IsLeaf(LinkedQuadTreeNode node) {
        return node.Depth + 1 == Height;
    }

    public bool IsFull(LinkedQuadTreeNode node) {
        return node.Value == NumberOfCells(node);
    }

    public bool IsEmpty(LinkedQuadTreeNode node) {
        return node.Value == 0;
    }

    public int NumberOfCells(LinkedQuadTreeNode node) {
        return (int) Mathf.Pow(4, Height - 1 - node.Depth);
    }

    public int NumberOfCellsPerSide(LinkedQuadTreeNode node) {
        return (int) Mathf.Pow(2, Height - 1 - node.Depth);
    }

    public float NormalizedValue(LinkedQuadTreeNode node) {
        return (float) node.Value / NumberOfCells(node);
    }

    public LinkedQuadTreeNode[] GetLeafNodes() {

        if (LeafNodes != null) return LeafNodes;

        int leafNodesCount = (int) Mathf.Pow(4, Height - 1);
        int startIndex = Nodes.Length - leafNodesCount;

        LeafNodes = new LinkedQuadTreeNode[leafNodesCount];
        for (int i = 0; i < leafNodesCount; i++) {
            LeafNodes[i] = Nodes[startIndex + i];
        }

        return LeafNodes;
    }

    public void InsertValue(int value, LinkedQuadTreeNode node) {

        Assert.IsTrue(IsLeaf(node), "Value can be inserted only on leaf node!");

        node.Value = value;

        foreach (LinkedQuadTreeNode n in GetAllParentNodes(node)) {
            n.Value += value;
        }
    }

    public void InsertValuesFromMap(int[,] map) {

        int width = map.GetLength(0);
        int height = map.GetLength(1);
        int treeSize = NumberOfCells(Nodes[0]);
        int mapSize = width;

        Assert.IsTrue(width == height, "It must be a square map!");
        Assert.IsTrue(Mathf.IsPowerOfTwo(mapSize), "It must be a power of two map");

        int subCellsPerSide = (int) Mathf.Sqrt(mapSize / treeSize);

        LinkedQuadTreeNode[] leafNodes = GetLeafNodes();

        // loop through quad tree cells
        foreach (LinkedQuadTreeNode node in LeafNodes) {
            int[] coordinates = GetCoordinateOfLeafNode(node);
            int x = coordinates[0];
            int y = coordinates[1];

            // loop through image map subcells and sum their values
            int valueSum = 0;
            for (int iy = 0; iy < subCellsPerSide; iy++) {
                for (int ix = 0; ix < subCellsPerSide; ix++) {
                    int mx = x + ix;
                    int my = y + iy;
                    valueSum += map[mx, my];
                }
            }

            // inset sum to cell
            InsertValue(valueSum, node);
        }
    }

    public LinkedQuadTreeNode[] GetAllParentNodes(LinkedQuadTreeNode node, LinkedQuadTreeNode[] parentNodes = null) {

        if (parentNodes == null) {
            parentNodes = new LinkedQuadTreeNode[node.Depth];
        }

        if (node.Depth > 0) {
            LinkedQuadTreeNode parentNode = GetParentNode(node);
            parentNodes[node.Depth - 1] = parentNode;
            return GetAllParentNodes(parentNode, parentNodes);
        }

        return parentNodes;
    }

    public LinkedQuadTreeNode GetParentNode(LinkedQuadTreeNode node) {
        int parentIndex = Mathf.FloorToInt((node.Index - 1) / 4);
        return Nodes[parentIndex];
    }

    public int[] GetCoordinateOfLeafNode(LinkedQuadTreeNode node) {

        int sides = NumberOfCellsPerSide(Nodes[0]);

        int indexOffset = GetNumberOfTreeNodes(Height - 1);
        int leafIndex = node.Index - indexOffset;


        int y = Mathf.FloorToInt(leafIndex / sides);
        int x = leafIndex % sides;

        return new int[2]{ x, y };
    }

    public int GetNumberOfTreeNodes(int height) {
        int numberOfNodes = 0;
        for (int i = 0; i < height; i++) {
            numberOfNodes += (int) Mathf.Pow(4, i);
        }
        return numberOfNodes;
    }

#region Gizmos

    public void DrawQuadTreeGizmo() {

        for (int i = Nodes.Length - 1; i >= 0; i--) {

            LinkedQuadTreeNode node = Nodes[i];

            Color color = Color.Lerp(Color.black, Color.white, 1f - ((float) node.Depth / (float) Height));

            DrawNodeGizmo(node, color);
        }
    }

    public void DrawLeafNodesGizmo() {
        foreach (LinkedQuadTreeNode node in GetLeafNodes()) {
            DrawNodeGizmo(node, Color.white);
        }
    }

    public void DrawNodeGizmo(LinkedQuadTreeNode node, Color color) {
        Vector2 position = node.Position;
        float size = node.Size;

        Vector2 a = position;
        Vector2 b = position + Vector2.right * size;
        Vector2 c = position + Vector2.up * size;
        Vector2 d = position + Vector2.one * size;

        Debug.DrawLine(a, b, color);
        Debug.DrawLine(b, d, color);
        Debug.DrawLine(d, c, color);
        Debug.DrawLine(c, a, color);
    }

#endregion
}

public class LinkedQuadTreeNode {

    public readonly Vector2 Position;
    public readonly float Size;
    public readonly int Depth;
    public readonly int Index;
    public int Value;

    public LinkedQuadTreeNode(Vector2 position, float size, int depth, int index, int value = 0) {
        Position = position;
        Size = size;
        Depth = depth;
        Index = index;
        Value = value;
    }

    public override string ToString() {
        return string.Format("[Node] position={0}, size={1}, depth={2}, index={3}, value={4}", Position, Size, Depth, Index, Value);
    }
}
