namespace GameCore.Editor.Utils
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UIElements;

    [Serializable]
    public enum ToolbarZone
    {
        ToolbarZoneRightAlign,
        ToolbarZoneLeftAlign
    }

    [InitializeOnLoad]
    public static class SceneToolbarEditor
    {
        private static ScriptableObject toolbar;
        private static string[]         scenePaths;
        private static string[]         sceneNames;

        static SceneToolbarEditor()
        {
            EditorApplication.delayCall += () =>
            {
                EditorApplication.update -= Update;
                EditorApplication.update += Update;
            };
        }

        private static void Update()
        {
            if (toolbar == null)
            {
                var editorAssembly = typeof(Editor).Assembly;

                var toolbars = Resources.FindObjectsOfTypeAll(editorAssembly.GetType("UnityEditor.Toolbar"));
                toolbar = toolbars.Length > 0 ? (ScriptableObject)toolbars[0] : null;

                if (toolbar != null)
                {
                    var root    = toolbar.GetType().GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
                    var rawRoot = root.GetValue(toolbar);
                    var mRoot   = rawRoot as VisualElement;
                    RegisterCallback(ToolbarZone.ToolbarZoneRightAlign.ToString(), OnGUI);

                    void RegisterCallback(string root, Action cb)
                    {
                        var toolbarZone = mRoot.Q(root);

                        if (toolbarZone == null) return;
                        var parent = new VisualElement
                        {
                            style =
                            {
                                flexGrow      = 1,
                                flexDirection = FlexDirection.Row,
                            }
                        };

                        var container = new IMGUIContainer();
                        container.onGUIHandler += () => { cb?.Invoke(); };
                        parent.Add(container);
                        toolbarZone.Add(parent);
                    }
                }
            }

            if (scenePaths != null) return;
            var tempScenePaths = new List<string>();
            var tempSceneNames = new List<string>();

            var folderName   = Application.dataPath + "/Scenes";
            var dirInfo      = new DirectoryInfo(folderName);
            var allFileInfos = dirInfo.GetFiles("*.unity", SearchOption.AllDirectories);

            foreach (var fileInfo in allFileInfos)
            {
                var fullPath  = fileInfo.FullName.Replace(@"\", "/");
                var scenePath = "Assets" + fullPath.Replace(Application.dataPath, "");

                tempScenePaths.Add(scenePath);
                tempSceneNames.Add(Path.GetFileNameWithoutExtension(scenePath));
            }

            scenePaths = tempScenePaths.ToArray();
            sceneNames = tempSceneNames.ToArray();
        }

        private static void OnGUI()
        {
            using (new EditorGUI.DisabledScope(Application.isPlaying))
            {
                {
                    var sceneName  = SceneManager.GetActiveScene().name;
                    var sceneIndex = -1;

                    for (var i = 0; i < sceneNames.Length; ++i)
                    {
                        if (sceneName != sceneNames[i]) continue;
                        sceneIndex = i;

                        break;
                    }

                    var newSceneIndex = EditorGUILayout.Popup(sceneIndex, sceneNames, GUILayout.Width(200.0f));

                    if (newSceneIndex == sceneIndex) return;
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        EditorSceneManager.OpenScene(scenePaths[newSceneIndex], OpenSceneMode.Single);
                    }
                }
            }
        }
    }
}