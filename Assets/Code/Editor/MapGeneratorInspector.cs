using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorInspector : Editor {
    public override void OnInspectorGUI() {
        MapGenerator MapGenerator = (MapGenerator) target;

        DrawDefaultInspector();
            
        if (MapGenerator.AutoUpdate) {
            MapGenerator.RenderMap();
            return;
        }

        if (GUILayout.Button("Generate")) {
            MapGenerator.RenderMap();
        }
    }
}
