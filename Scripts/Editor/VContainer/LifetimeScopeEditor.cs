namespace GameCore.Editor.VContainer
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using global::VContainer;
    using global::VContainer.Unity;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(LifetimeScope))]
    public class LifetimeScopeEditor : Editor
    {
        private LifetimeScope scope;

        private SerializedProperty autoInjectGameObjectsProps;

        private void OnEnable()
        {
            this.scope                      = (LifetimeScope)this.target;
            this.autoInjectGameObjectsProps = this.serializedObject.FindProperty("autoInjectGameObjects");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (this.scope.IsRoot) return;
            if (this.autoInjectGameObjectsProps.isArray)
            {
                if (GUILayout.Button("Find Inject Object"))
                {
                    this.autoInjectGameObjectsProps.ClearArray();

                    var listAutoInjectObj = new List<GameObject>();
                    var allObjs           = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
                    foreach (var obj in allObjs)
                    {
                        var monoComponents = obj.GetComponents<MonoBehaviour>();
                        if (monoComponents.Length == 0) continue;
                        const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic;
                        foreach (var mono in monoComponents)
                        {
                            if (mono is LifetimeScope) continue;
                            if (mono.GetType().GetFields(bindingFlags).Any(info => !info.GetCustomAttributes(typeof(InjectAttribute)).Any())) continue;
                            if (mono.GetType().GetProperties(bindingFlags).Any(info => !info.GetCustomAttributes(typeof(InjectAttribute)).Any())) continue;
                            if (mono.GetType().GetMethods(bindingFlags).Any(info => !info.GetCustomAttributes(typeof(InjectAttribute)).Any())) continue;
                            listAutoInjectObj.Add(mono.gameObject);
                            break;
                        }
                    }

                    for (var i = 0; i < listAutoInjectObj.Count; i++)
                    {
                        this.autoInjectGameObjectsProps.InsertArrayElementAtIndex(i);
                        var element = this.autoInjectGameObjectsProps.GetArrayElementAtIndex(i);
                        element.objectReferenceValue = listAutoInjectObj[i];
                    }

                    this.autoInjectGameObjectsProps.serializedObject.ApplyModifiedProperties();
                }
            }
        }
    }
}