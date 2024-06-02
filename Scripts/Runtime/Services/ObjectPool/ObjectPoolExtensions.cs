namespace GameCore.Services.ObjectPool
{
    using UnityEngine;

    public static class ObjectPoolExtensions
    {
        public static Poolable CreatePool(this GameObject prefab, int size = 1, bool dontDestroyOnLoad = false)
        {
            return ObjectPoolService.Instance.CreatePool(prefab, size, dontDestroyOnLoad);
        }

        public static Poolable CreatePool<T>(this T prefab, int size = 1, bool dontDestroyOnLoad = false) where T : Component
        {
            return ObjectPoolService.Instance.CreatePool(prefab, size, dontDestroyOnLoad);
        }

        public static GameObject Spawn(this GameObject prefab)
        {
            return ObjectPoolService.Instance.Spawn(prefab);
        }

        public static GameObject Spawn(this GameObject prefab, Transform parent)
        {
            return ObjectPoolService.Instance.Spawn(prefab, parent);
        }

        public static GameObject Spawn(this GameObject prefab, Vector3 position)
        {
            return ObjectPoolService.Instance.Spawn(prefab, position);
        }

        public static GameObject Spawn(this GameObject prefab, Vector3 position, Vector3 rotation)
        {
            return ObjectPoolService.Instance.Spawn(prefab, position, rotation);
        }

        public static GameObject Spawn(this GameObject prefab, Vector3 position, Quaternion rotation)
        {
            return ObjectPoolService.Instance.Spawn(prefab, position, rotation);
        }

        public static T Spawn<T>(this T prefab) where T : Component
        {
            return ObjectPoolService.Instance.Spawn(prefab);
        }

        public static T Spawn<T>(this T prefab, Transform parent) where T : Component
        {
            return ObjectPoolService.Instance.Spawn(prefab, parent);
        }

        public static T Spawn<T>(this T prefab, Vector3 position) where T : Component
        {
            return ObjectPoolService.Instance.Spawn(prefab, position);
        }

        public static T Spawn<T>(this T prefab, Vector3 position, Vector3 rotation) where T : Component
        {
            return ObjectPoolService.Instance.Spawn(prefab, position, rotation);
        }

        public static T Spawn<T>(this T prefab, Vector3 position, Quaternion rotation) where T : Component
        {
            return ObjectPoolService.Instance.Spawn(prefab, position, rotation);
        }

        public static void Recycle(this GameObject obj)
        {
            ObjectPoolService.Instance.Recycle(obj);
        }

        public static void Recycle<T>(this T obj) where T : Component
        {
            ObjectPoolService.Instance.Recycle(obj);
        }

        public static void RecycleAll(this GameObject obj)
        {
            ObjectPoolService.Instance.RecycleAll(obj);
        }

        public static void RecycleAll<T>(this T obj) where T : Component
        {
            ObjectPoolService.Instance.RecycleAll(obj);
        }

        public static void CleanUp(this GameObject obj, bool cleanUpAll = true)
        {
            ObjectPoolService.Instance.CleanUp(obj, cleanUpAll);
        }

        public static void CleanUp<T>(this T obj, bool cleanUpAll = true) where T : Component
        {
            ObjectPoolService.Instance.CleanUp(obj, cleanUpAll);
        }
    }
}