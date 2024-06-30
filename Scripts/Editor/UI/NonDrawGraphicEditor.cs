namespace GameCore.Editor.UI
{
    using System;
    using GameCore.Utils.UIElement;
    using UnityEditor;
    using UnityEditor.UI;
    using UnityEngine;

    [CanEditMultipleObjects, CustomEditor(typeof(NonDrawingGraphic), false)]
    public class NonDrawingGraphicEditor : GraphicEditor
    {
        public override void OnInspectorGUI ()
        {
            this.serializedObject.Update();
            EditorGUILayout.PropertyField(this.m_Script, Array.Empty<GUILayoutOption>());
            this.RaycastControlsGUI();
            this.serializedObject.ApplyModifiedProperties();
        }
    }
}