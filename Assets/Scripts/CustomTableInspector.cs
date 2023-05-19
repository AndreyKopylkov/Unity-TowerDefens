using UnityEditor;

[CustomEditor(typeof(LevelInfo))]
public class CustomTableInspector : Editor {

    LevelInfo targetScript;

    void OnEnable()
    {
        targetScript = target as LevelInfo;
    }

    public override void OnInspectorGUI()
    {

        LevelInfo.X = EditorGUILayout.IntField(LevelInfo.X);
        LevelInfo.Y = EditorGUILayout.IntField(LevelInfo.Y);

        EditorGUILayout.BeginHorizontal ();
        for (int y = 0; y < LevelInfo.Y; y++) {
            EditorGUILayout.BeginVertical ();
            for (int x = 0; x < LevelInfo.X; x++)
            {
                targetScript.columns [x].rows[y] = EditorGUILayout.IntField (targetScript.columns [x].rows[y]);
            }
            
            EditorGUILayout.EndVertical ();
        }
        
        EditorGUILayout.EndHorizontal ();
    }
}