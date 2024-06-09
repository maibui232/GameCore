namespace Editor
{
    using UnityEditor;
    using UnityEngine;

    public static class PrefabCreatorEditor
    {
        [MenuItem("GameObject/UI/JoystickView")]
        public static void CreateJoystick()
        {
            var selectedObj = Selection.activeGameObject;
            var prefab      = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/game.core/Prefabs/UISamples/JoystickView.prefab");
            PrefabUtility.InstantiatePrefab(prefab, selectedObj.transform);
        }
    }
}