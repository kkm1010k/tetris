using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridSpawner))]
public class GridButton : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GridSpawner generator = (GridSpawner)target;
        if (GUILayout.Button("Generate Grid"))
        {
            generator.MakeGridEditor();
        }
    }
}