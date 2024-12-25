
using MoreMountains.Feedbacks;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MMF_Player), true)]
public class MMFeedbackEditor : MMF_PlayerEditor
{
    protected override void DrawDebugControls()
    {
        EditorGUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Preview", GUILayout.Height(30)))
        {
            (target as MMF_Player).PlayFeedbacks();
        }

        if (GUILayout.Button("Stop", GUILayout.Height(30)))
        {
            (target as MMF_Player).StopFeedbacks();
        }
        EditorGUILayout.EndHorizontal();
        base.DrawDebugControls();
        
        
        
    }
}
