namespace GameCore.Services.ObjectPool
{
    using System;
    using System.Collections.Generic;
    using GameCore.Services.Logger;
    using UnityEngine;

    public class Poolable : MonoBehaviour
    {
        [SerializeField] private GameObject       prefabObj;
        [SerializeField] private List<GameObject> spawnedObjs = new();
        [SerializeField] private List<GameObject> cachedObjs  = new();

        public Action DestroyEvent;

        private void OnDestroy()
        {
            this.CleanUp();
            this.DestroyEvent?.Invoke();
        }

        public bool ContainsObj(GameObject obj) { return this.prefabObj.Equals(obj) || this.spawnedObjs.Contains(obj) || this.cachedObjs.Contains(obj); }

        public void CreatePool(GameObject prefab, int size, bool dontDestroyOnLoad = false)
        {
            if (dontDestroyOnLoad) DontDestroyOnLoad(this.gameObject);
            this.prefabObj = prefab;
            for (var i = 0; i < size; i++)
            {
                var obj = Instantiate(this.prefabObj, this.transform, false);
                obj.SetActive(false);
                obj.transform.localPosition = Vector3.zero;
                this.cachedObjs.Add(prefab);
            }

            LoggerService.Log($"Create Pool: {this.name}, size: {size}");
        }

        public GameObject Spawn(GameObject prefab, Transform parent)
        {
            GameObject obj;
            if (this.cachedObjs.Count == 0)
            {
                parent = parent == null ? this.transform : parent;
                obj    = Instantiate(this.prefabObj, parent);
            }
            else
            {
                obj = this.cachedObjs[0];
                this.cachedObjs.RemoveAt(0);
            }

            this.spawnedObjs.Add(obj);
            return obj;
        }

        public void Recycle(GameObject obj)
        {
            if (this.cachedObjs.Contains(obj))
            {
                LoggerService.Error($"{obj.name} has been recycled!");
                return;
            }

            if (!this.spawnedObjs.Contains(obj))
            {
                LoggerService.Error($"{obj.name} does not contain in {this.name}");
                return;
            }

            this.spawnedObjs.Remove(obj);
            this.ResetTransformObj(obj);
            this.cachedObjs.Add(obj);
        }

        public void RecycleAll()
        {
            foreach (var obj in this.spawnedObjs)
            {
                this.ResetTransformObj(obj);
                this.cachedObjs.Add(obj);
            }

            this.spawnedObjs.Clear();
            LoggerService.Log($"Recycle all: {this.name}", Color.green);
        }

        public void CleanUp(bool cleanUpAll = true)
        {
            if (cleanUpAll)
            {
                foreach (var obj in this.spawnedObjs)
                {
                    Destroy(obj);
                }

                this.spawnedObjs.Clear();
            }

            foreach (var obj in this.cachedObjs)
            {
                Destroy(obj);
            }

            this.cachedObjs.Clear();
            LoggerService.Log($"Clean Up: {this.name}", Color.green);
        }

        private void ResetTransformObj(GameObject obj)
        {
            obj.SetActive(false);
            var objTransform = obj.transform;
            objTransform.SetParent(this.transform, false);
            objTransform.localPosition    = Vector3.zero;
            objTransform.localEulerAngles = Vector3.zero;
        }
    }
}