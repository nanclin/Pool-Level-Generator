﻿using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UnitTester))]
public class UnitTesterEditor : Editor {

    private string SuccessLabel;
    private bool Success;

    public override void OnInspectorGUI() {

        DrawDefaultInspector();

        if (GUILayout.Button("RunTests")) {
            Success = RunTests();
            string message = Success ? "All tests passed!" : "Some tests failed! Look at the console.";
            EditorUtility.DisplayDialog("Tests run", message, "OK");
        }

        GUIStyle style = new GUIStyle();
        style.normal.textColor = Success ? Color.green : Color.red;
        string text = Success ? "SUCCESS" : "FAIL";
        EditorGUILayout.LabelField(text, style);
    }

    private bool RunTests() {
        bool success = true;

        success &= TestEmptyAndFull();
        success &= TestNormalizedValue();
        success &= TestGetAllParentNodes();
        success &= TestNumberOfCells();
        success &= TestGetCoordinateOfLeafNodeHeight1();
        success &= TestGetCoordinateOfLeafNodeHeight2();
        success &= TestGetCoordinateOfLeafNodeHeight3();

        return success;
    }

    private void TestAssert(bool condition, string errorMessage, ref bool successFlag, ref string errorMessages) {
        successFlag &= condition;
        if (condition != true) {
            errorMessages += errorMessage + "\n";
            Debug.LogError(errorMessage);
        }
    }

#region Tests

    private bool TestEmptyAndFull() {

        int size = 128;
        int height = 2;

        LinkedQuadTree linkedQuadTree = new LinkedQuadTree(new Vector2(-size / 2, -size / 2), size, height);
                
        LinkedQuadTreeNode lastNode = linkedQuadTree.Nodes[linkedQuadTree.Nodes.Length - 1];
        linkedQuadTree.InsertValue(1, lastNode);

        bool success = true;
        string messages = "";

        TestAssert(linkedQuadTree.IsFull(linkedQuadTree.Nodes[4]) == true, "Expected cell 4 to be full!", ref success, ref messages);
        TestAssert(linkedQuadTree.IsFull(linkedQuadTree.Nodes[0]) == false, "Expected cell 0 not to be full!", ref success, ref messages);
        TestAssert(linkedQuadTree.IsEmpty(linkedQuadTree.Nodes[4]) == false, "Expected cell 4 not to be empty!", ref success, ref messages);
        TestAssert(linkedQuadTree.IsEmpty(linkedQuadTree.Nodes[0]) == false, "Expected cell 0 to be empty!", ref success, ref messages);
        TestAssert(linkedQuadTree.IsEmpty(linkedQuadTree.Nodes[1]) == true, "Expected cell 1 to be emtpy!", ref success, ref messages);

        return success;
    }

    private bool TestNormalizedValue() {

        int size = 128;
        int height = 3;

        LinkedQuadTree linkedQuadTree = new LinkedQuadTree(new Vector2(-size / 2, -size / 2), size, height);

        linkedQuadTree.InsertValue(1, linkedQuadTree.Nodes[linkedQuadTree.Nodes.Length - 1]);
        linkedQuadTree.InsertValue(1, linkedQuadTree.Nodes[linkedQuadTree.Nodes.Length - 2]);
        linkedQuadTree.InsertValue(1, linkedQuadTree.Nodes[linkedQuadTree.Nodes.Length - 3]);
        linkedQuadTree.InsertValue(1, linkedQuadTree.Nodes[linkedQuadTree.Nodes.Length - 4]);

        bool success = true;
        string messages = "";

        TestAssert(linkedQuadTree.NormalizedValue(linkedQuadTree.Nodes[20]) == 1, "Expected node 20 to have value 1!", ref success, ref messages);
        TestAssert(linkedQuadTree.NormalizedValue(linkedQuadTree.Nodes[4]) == 1, "Expected node 4 to have value 1!", ref success, ref messages);
        TestAssert(linkedQuadTree.NormalizedValue(linkedQuadTree.Nodes[0]) == 0.25f, "Expected node 0 to have value 0.25!", ref success, ref messages);

        return success;
    }

    private bool TestGetAllParentNodes() {

        bool success = true;
        string messages = "";

        int size = 128;
        int height = 3;

        LinkedQuadTree linkedQuadTree = new LinkedQuadTree(new Vector2(-size / 2, -size / 2), size, height);
        LinkedQuadTreeNode lastNode = linkedQuadTree.Nodes[linkedQuadTree.Nodes.Length - 1];
        LinkedQuadTreeNode[] parentNodes = linkedQuadTree.GetAllParentNodes(lastNode);

        TestAssert(parentNodes.Length == 2, "Expected 2 parent nodes!", ref success, ref messages);
        TestAssert(parentNodes[0].Index == 0, "Expected top parent to be root!", ref success, ref messages);
        TestAssert(parentNodes[1].Index == 4, "Expected second parent have index 4!", ref success, ref messages);

        return success;
    }

    private bool TestNumberOfCells() {
        bool success = true;
        string messages = "";

        int size = 128;
        int height = 3;

        LinkedQuadTree linkedQuadTree = new LinkedQuadTree(new Vector2(-size / 2, -size / 2), size, height);
        LinkedQuadTreeNode lastNode = linkedQuadTree.Nodes[linkedQuadTree.Nodes.Length - 1];

        TestAssert(linkedQuadTree.NumberOfCells(linkedQuadTree.Nodes[0]) == 16, "Expected 4 cells at this depth!", ref success, ref messages);
        TestAssert(linkedQuadTree.NumberOfCells(linkedQuadTree.Nodes[1]) == 4, "Expected 2 cells at this depth!", ref success, ref messages);
        TestAssert(linkedQuadTree.NumberOfCells(lastNode) == 1, "Expected 1 cell at this depth!", ref success, ref messages);

        TestAssert(linkedQuadTree.NumberOfCellsPerSide(linkedQuadTree.Nodes[0]) == 4, "Expected 4 cells at this depth!", ref success, ref messages);
        TestAssert(linkedQuadTree.NumberOfCellsPerSide(linkedQuadTree.Nodes[1]) == 2, "Expected 2 cells at this depth!", ref success, ref messages);
        TestAssert(linkedQuadTree.NumberOfCellsPerSide(lastNode) == 1, "Expected 1 cell at this depth!", ref success, ref messages);

        return success;
    }

    private bool TestGetCoordinateOfLeafNodeHeight1() {
        bool success = true;
        string messages = "";

        int size = 128;
        int height = 1;

        LinkedQuadTree linkedQuadTree = new LinkedQuadTree(Vector2.zero, size, height);
        LinkedQuadTreeNode lastNode = linkedQuadTree.Nodes[linkedQuadTree.Nodes.Length - 1];

        int[] coordinates = linkedQuadTree.GetCoordinateOfLeafNode(lastNode);
        TestAssert(coordinates[0] == 0, "Expected 0!", ref success, ref messages);
        TestAssert(coordinates[1] == 0, "Expected 0!", ref success, ref messages);

        return success;
    }

    private bool TestGetCoordinateOfLeafNodeHeight2() {
        bool success = true;
        string messages = "";

        int size = 128;
        int height = 2;

        LinkedQuadTree linkedQuadTree = new LinkedQuadTree(Vector2.zero, size, height);
        LinkedQuadTreeNode lastNode = linkedQuadTree.Nodes[linkedQuadTree.Nodes.Length - 1];

        int[] coordinates = linkedQuadTree.GetCoordinateOfLeafNode(lastNode);
        TestAssert(coordinates[0] == 1, "Expected 1!", ref success, ref messages);
        TestAssert(coordinates[1] == 1, "Expected 1!", ref success, ref messages);
        
        return success;
    }

    private bool TestGetCoordinateOfLeafNodeHeight3() {
        bool success = true;
        string messages = "";

        int size = 128;
        int height = 3;

        LinkedQuadTree linkedQuadTree = new LinkedQuadTree(Vector2.zero, size, height);
        LinkedQuadTreeNode lastNode = linkedQuadTree.Nodes[linkedQuadTree.Nodes.Length - 1];

        int[] coordinates = linkedQuadTree.GetCoordinateOfLeafNode(lastNode);
        TestAssert(coordinates[0] == 3, "Expected 3!", ref success, ref messages);
        TestAssert(coordinates[1] == 3, "Expected 3!", ref success, ref messages);

        return success;
    }

    private bool TestInsertValuesFromMap() {
        bool success = true;
        string messages = "";

        int size = 4;
        int height = 2;

        int[,] imageMap = new int[,] {
            { 0, 0, 1, 1 },
            { 0, 0, 1, 1 },
            { 0, 1, 0, 0 },
            { 1, 1, 0, 1 },
        };

        LinkedQuadTree linkedQuadTree = new LinkedQuadTree(Vector2.zero, size, height);
        linkedQuadTree.InsertValuesFromMap(imageMap);

        TestAssert(linkedQuadTree.Nodes[1].Value == 3, "Expected value 3!", ref success, ref messages);
        TestAssert(linkedQuadTree.Nodes[2].Value == 2, "Expected value 2!", ref success, ref messages);
        TestAssert(linkedQuadTree.Nodes[4].Value == 4, "Expected value 4!", ref success, ref messages);

        TestAssert(linkedQuadTree.IsEmpty(linkedQuadTree.Nodes[1]) == false, "Not expected empty in node 1!", ref success, ref messages);
        TestAssert(linkedQuadTree.IsFull(linkedQuadTree.Nodes[1]) == false, "Not expected full in node 1!", ref success, ref messages);

        TestAssert(linkedQuadTree.IsEmpty(linkedQuadTree.Nodes[3]) == true, "Expected empty in node 3!", ref success, ref messages);
        TestAssert(linkedQuadTree.IsFull(linkedQuadTree.Nodes[4]) == true, "Expected full in node 4!", ref success, ref messages);

        return success;
    }

#endregion
}
