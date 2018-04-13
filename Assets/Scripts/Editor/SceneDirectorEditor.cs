using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace KinectExercise
{
    [CustomEditor(typeof(SceneDirector))]
    public class SceneDirectorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            SceneDirector myDirector = (SceneDirector)target;
            if (GUILayout.Button("Generate Goals"))
            {
                myDirector.GenerateGoals();
            }
        }
    }
}
#endif