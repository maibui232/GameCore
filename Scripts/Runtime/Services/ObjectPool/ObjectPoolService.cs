namespace GameCore.Services.ObjectPool
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using GameCore.Services.GameAsset;
    using GameCore.Services.Logger;
    using UnityEngine;
    using VContainer;

    public interface IObjectPoolService
    {
        Poolable CreatePool(GameObject prefab, int size = 1, bool dontDestroyOnLoad = false);
        Poolable CreatePool<T>(T prefab, int size = 1, bool dontDestroyOnLoad = false) where T : Component;

        UniTask<Poolable> CreatePool(string addressableId, int size = 1, bool dontDestroyOnLoad = false);
        UniTask<Poolable> CreatePool<T>(string addressableId, int size = 1, bool dontDestroyOnLoad = false) where T : Component;

        GameObject Spawn(GameObject prefab);
        GameObject Spawn(GameObject prefab, Transform parent);
        GameObject Spawn(GameObject prefab, Vector3 position);
        GameObject Spawn(GameObject prefab, Vector3 position, Vector3 rotation);
        GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation);

        UniTask<GameObject> Spawn(string addressableId);
        UniTask<GameObject> Spawn(string addressableId, Transform parent);
        UniTask<GameObject> Spawn(string addressableId, Vector3 position);
        UniTask<GameObject> Spawn(string addressableId, Vector3 position, Vector3 rotation);
        UniTask<GameObject> Spawn(string addressableId, Vector3 position, Quaternion rotation);

        T Spawn<T>(T prefab) where T : Component;
        T Spawn<T>(T prefab, Transform parent) where T : Component;
        T Spawn<T>(T prefab, Vector3 position) where T : Component;
        T Spawn<T>(T prefab, Vector3 position, Vector3 rotation) where T : Component;
        T Spawn<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component;

        UniTask<T> Spawn<T>(string addressableId) where T : Component;
        UniTask<T> Spawn<T>(string addressableId, Transform parent) where T : Component;
        UniTask<T> Spawn<T>(string addressableId, Vector3 position) where T : Component;
        UniTask<T> Spawn<T>(string addressableId, Vector3 position, Vector3 rotation) where T : Component;
        UniTask<T> Spawn<T>(string addressableId, Vector3 position, Quaternion rotation) where T : Component;

        void Recycle(GameObject obj);
        void Recycle<T>(T obj) where T : Component;

        void RecycleAll(GameObject obj);
        void RecycleAll<T>(T obj) where T : Component;

        void CleanUp(GameObject obj, bool cleanUpAll = true);
        void CleanUp<T>(T obj, bool cleanUpAll = true) where T : Component;
    }

    public class ObjectPoolService : IObjectPoolService
    {
        #region Inject

        private readonly IGameAssetService gameAssetService;
        private readonly IObjectResolver   objectResolver;

        #endregion

        private Dictionary<GameObject, Poolable> prefabToPoolable = new();

        public static ObjectPoolService Instance;

        public ObjectPoolService
        (
            IGameAssetService gameAssetService,
            IObjectResolver objectResolver
        )
        {
            this.gameAssetService = gameAssetService;
            this.objectResolver   = objectResolver;

            Instance = this;
        }

        #region CreatePool

        public Poolable CreatePool(GameObject prefab, int size = 1, bool dontDestroyOnLoad = false)
        {
            if (this.prefabToPoolable.TryGetValue(prefab, out var pool))
            {
                LoggerService.Error($"{pool.name} already create!");
                return pool;
            }

            var poolableObj = new GameObject(prefab.name).AddComponent<Poolable>();
            this.objectResolver.Inject(poolableObj);
            poolableObj.DestroyEvent = () => this.prefabToPoolable.Remove(prefab);
            this.prefabToPoolable.Add(prefab, poolableObj);
            poolableObj.CreatePool(prefab, size, dontDestroyOnLoad);
            return poolableObj;
        }

        public Poolable CreatePool<T>(T prefab, int size = 1, bool dontDestroyOnLoad = false) where T : Component
        {
            return this.CreatePool(prefab.gameObject, size, dontDestroyOnLoad);
        }

        public async UniTask<Poolable> CreatePool(string addressableId, int size = 1, bool dontDestroyOnLoad = false)
        {
            var prefab = await this.gameAssetService.LoadAssetAsync<GameObject>(addressableId);
            return this.CreatePool(prefab, size, dontDestroyOnLoad);
        }

        public UniTask<Poolable> CreatePool<T>(string addressableId, int size = 1, bool dontDestroyOnLoad = false) where T : Component
        {
            return this.CreatePool(addressableId, size, dontDestroyOnLoad);
        }

        #endregion

        #region Spawn

        public GameObject Spawn(GameObject prefab)
        {
            if (!this.prefabToPoolable.TryGetValue(prefab, out _))
            {
                this.CreatePool(prefab);
            }

            return this.prefabToPoolable[prefab].Spawn(prefab, null);
        }

        public GameObject Spawn(GameObject prefab, Transform parent)
        {
            if (!this.prefabToPoolable.TryGetValue(prefab, out _))
            {
                this.CreatePool(prefab);
            }

            return this.prefabToPoolable[prefab].Spawn(prefab, parent);
        }

        public GameObject Spawn(GameObject prefab, Vector3 position)
        {
            var obj = this.Spawn(prefab);
            obj.transform.position = position;
            return obj;
        }

        public GameObject Spawn(GameObject prefab, Vector3 position, Vector3 rotation)
        {
            var obj          = this.Spawn(prefab);
            var objTransform = obj.transform;
            objTransform.position    = position;
            objTransform.eulerAngles = rotation;
            return obj;
        }

        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            var obj          = this.Spawn(prefab);
            var objTransform = obj.transform;
            objTransform.position = position;
            objTransform.rotation = rotation;
            return obj;
        }

        public async UniTask<GameObject> Spawn(string addressableId)
        {
            var prefab = await this.gameAssetService.LoadAssetAsync<GameObject>(addressableId);
            return this.Spawn(prefab);
        }

        public async UniTask<GameObject> Spawn(string addressableId, Transform parent)
        {
            var prefab = await this.gameAssetService.LoadAssetAsync<GameObject>(addressableId);
            return this.Spawn(prefab, parent);
        }

        public async UniTask<GameObject> Spawn(string addressableId, Vector3 position)
        {
            var prefab = await this.gameAssetService.LoadAssetAsync<GameObject>(addressableId);
            return this.Spawn(prefab, position);
        }

        public async UniTask<GameObject> Spawn(string addressableId, Vector3 position, Vector3 rotation)
        {
            var prefab = await this.gameAssetService.LoadAssetAsync<GameObject>(addressableId);
            return this.Spawn(prefab, position, rotation);
        }

        public async UniTask<GameObject> Spawn(string addressableId, Vector3 position, Quaternion rotation)
        {
            var prefab = await this.gameAssetService.LoadAssetAsync<GameObject>(addressableId);
            return this.Spawn(prefab, position, rotation);
        }

        public T Spawn<T>(T prefab) where T : Component
        {
            return this.Spawn(prefab.gameObject).GetComponent<T>();
        }

        public T Spawn<T>(T prefab, Transform parent) where T : Component
        {
            return this.Spawn(prefab.gameObject, parent).GetComponent<T>();
        }

        public T Spawn<T>(T prefab, Vector3 position) where T : Component
        {
            return this.Spawn(prefab.gameObject, position).GetComponent<T>();
        }

        public T Spawn<T>(T prefab, Vector3 position, Vector3 rotation) where T : Component
        {
            return this.Spawn(prefab.gameObject, position, rotation).GetComponent<T>();
        }

        public T Spawn<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component
        {
            return this.Spawn(prefab.gameObject, position, rotation).GetComponent<T>();
        }

        public async UniTask<T> Spawn<T>(string addressableId) where T : Component
        {
            var prefab = await this.gameAssetService.LoadAssetAsync<T>(addressableId);
            return this.Spawn(prefab);
        }

        public async UniTask<T> Spawn<T>(string addressableId, Transform parent) where T : Component
        {
            var prefab = await this.gameAssetService.LoadAssetAsync<T>(addressableId);
            return this.Spawn(prefab, parent);
        }

        public async UniTask<T> Spawn<T>(string addressableId, Vector3 position) where T : Component
        {
            var prefab = await this.gameAssetService.LoadAssetAsync<T>(addressableId);
            return this.Spawn(prefab, position);
        }

        public async UniTask<T> Spawn<T>(string addressableId, Vector3 position, Vector3 rotation) where T : Component
        {
            var prefab = await this.gameAssetService.LoadAssetAsync<T>(addressableId);
            return this.Spawn(prefab, position, rotation);
        }

        public async UniTask<T> Spawn<T>(string addressableId, Vector3 position, Quaternion rotation) where T : Component
        {
            var prefab = await this.gameAssetService.LoadAssetAsync<T>(addressableId);
            return this.Spawn(prefab, position, rotation);
        }

        #endregion

        #region Recycle

        public void Recycle(GameObject obj)
        {
            foreach (var (_, poolable) in this.prefabToPoolable)
            {
                if (!poolable.ContainsObj(obj)) continue;
                poolable.Recycle(obj);
                return;
            }
        }

        public void Recycle<T>(T obj) where T : Component
        {
            this.Recycle(obj.gameObject);
        }

        #endregion

        #region RecycleAll

        public void RecycleAll(GameObject obj)
        {
            foreach (var (_, poolable) in this.prefabToPoolable)
            {
                if (!poolable.ContainsObj(obj)) continue;
                poolable.RecycleAll();
                return;
            }
        }

        public void RecycleAll<T>(T obj) where T : Component
        {
            this.RecycleAll(obj.gameObject);
        }

        #endregion

        #region CleanUp

        public void CleanUp(GameObject obj, bool cleanUpAll = true)
        {
            foreach (var (_, poolable) in this.prefabToPoolable)
            {
                if (!poolable.ContainsObj(obj)) continue;
                poolable.CleanUp(cleanUpAll);
                return;
            }
        }

        public void CleanUp<T>(T obj, bool cleanUpAll = true) where T : Component
        {
            this.CleanUp(obj.gameObject, cleanUpAll);
        }

        #endregion
    }
}