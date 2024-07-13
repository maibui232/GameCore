namespace GameCore.Editor.Utils
{
    using System;
    using System.Linq;
    using System.Reflection;
    using GameCore.Extensions.VContainer;
    using global::VContainer.Unity;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using Object = UnityEngine.Object;

    public static class VContainerContextCreatorEditor
    {
        private static string ProjectContextPath     => $"Assets/Resources/Config/{nameof(ProjectContext)}.prefab";
        private static string VContainerSettingsPath => $"Assets/Resources/Config/{nameof(VContainerSettings)}.asset";

        [MenuItem("GameObject/UI/RootUIView")]
        public static void CreateRootUIView()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/game.core/Prefabs/UISamples/RootUIView.prefab");
            PrefabUtility.InstantiatePrefab(prefab, SceneManager.GetActiveScene());
        }

        [MenuItem("Assets/Create/VContainer/ProjectContext")]
        public static void CreateProjectContext()
        {
            var newInstance = new GameObject(nameof(ProjectContext));
            newInstance.AddComponent<ProjectContext>();
            PrefabUtility.SaveAsPrefabAsset(newInstance, ProjectContextPath);
            Object.DestroyImmediate(newInstance);

            var assetProjectContext = AssetDatabase.LoadAssetAtPath<GameObject>(ProjectContextPath).GetComponent<ProjectContext>();
            if (VContainerSettings.Instance == null)
            {
                var vContainerSettings = ScriptableObject.CreateInstance<VContainerSettings>();
                AssetDatabase.Refresh();
                AssetDatabase.CreateAsset(vContainerSettings, VContainerSettingsPath);
                var preloadAssets = PlayerSettings.GetPreloadedAssets().ToList();
                foreach (var preloadAsset in preloadAssets.ToList().Where(preloadAsset => preloadAsset == null))
                    preloadAssets.Remove(preloadAsset);

                preloadAssets.Add(vContainerSettings);
                PlayerSettings.SetPreloadedAssets(preloadAssets.ToArray());

                vContainerSettings.RootLifetimeScope  = assetProjectContext;
                vContainerSettings.EnableDiagnostics  = true;
                vContainerSettings.RemoveClonePostfix = true;
                EditorUtility.SetDirty(vContainerSettings);
            }
            else
            {
                VContainerSettings.Instance.RootLifetimeScope = assetProjectContext;
                EditorUtility.SetDirty(VContainerSettings.Instance);
            }

            AssetDatabase.SaveAssets();
        }

        [MenuItem("GameObject/VContainer/SceneContext")]
        public static void CreateSceneContext()
        {
            if (VContainerSettings.Instance == null) CreateProjectContext();

            var newInstance  = new GameObject(nameof(SceneContext));
            var sceneContext = newInstance.AddComponent<SceneContext>();
            sceneContext.parentReference = ParentReference.Create<ProjectContext>();
        }

        [MenuItem("Assets/Create/GameCore/ScreenVariant")]
        public static void CreateScreenVariant()
        {
            var projectWindowUtilType = typeof(ProjectWindowUtil);
            var getActiveFolderPath   = projectWindowUtilType.GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
            if (getActiveFolderPath != null)
            {
                var obj                 = getActiveFolderPath.Invoke(null, Array.Empty<object>());
                var pathToCurrentFolder = obj.ToString();
                var originalPrefab      = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/game.core/Prefabs/UISamples/BaseScreenView.prefab");
                var variantPrefab       = PrefabUtility.InstantiatePrefab(originalPrefab) as GameObject;
                PrefabUtility.SaveAsPrefabAsset(variantPrefab, $"{pathToCurrentFolder}/ScreenViewVariant.prefab");
                Object.DestroyImmediate(variantPrefab);
                AssetDatabase.Refresh();
            }
        }
    }
}