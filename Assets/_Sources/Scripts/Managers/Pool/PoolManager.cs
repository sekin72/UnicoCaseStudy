using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnicoCaseStudy.Managers.Asset;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace UnicoCaseStudy.Managers.Pool
{
    public class PoolManager : Manager
    {
        [field: SerializeField] public PoolDataHolder PoolDataHolder { get; private set; }
        private AddressableManager _addressableManager;

        private ulong _creationId;

        public Dictionary<PoolKeys, GameObjectPoolContainer> PoolCollection = new();

        protected override async UniTask WaitDependencies(CancellationToken disposeToken)
        {
            _addressableManager = AppManager.GetManager<AddressableManager>();

            await UniTask.WaitUntil(() => _addressableManager.IsInitialized, cancellationToken: disposeToken);
        }

        protected override async UniTask Initialize(CancellationToken disposeToken)
        {
            await InitializeLevelItemPools(disposeToken);
        }

        private async UniTask InitializeLevelItemPools(CancellationToken cancellationToken)
        {
            var initializeList = new List<UniTask>();

            foreach (var enumType in Enum.GetValues(typeof(PoolKeys)))
            {
                var key = (PoolKeys)enumType;
                var def = PoolDataHolder.GetPoolDefinition(key);

                if (key == PoolKeys.None || ContainsPool(key))
                {
                    continue;
                }

                initializeList.Add(
                    InitPool(
                        key,
                        cancellationToken,
                        def.AssetReference,
                        OnGetObjectFromPool,
                        defaultCapacity: def.DefaultCapacity,
                        preWarm: false,
                        awaitWarmup: false
                    )
                );
            }

            await UniTask.WhenAll(initializeList);
        }

        #region Pool Maintenance

        public async UniTask InitPool(
            PoolKeys poolKey,
            CancellationToken cancellationToken,
            AssetReferenceT<GameObject> assetReference,
            Action<GameObject> onObjectGet = null,
            Action<GameObject> onObjectRelease = null,
            int defaultCapacity = 1,
            int maxCapacity = 100,
            bool preWarm = false,
            bool awaitWarmup = false)
        {
            if (defaultCapacity < 1)
            {
                defaultCapacity = 1;
            }

            if (PoolCollection.ContainsKey(poolKey))
            {
                return;
            }

            GameObject prefab = await _addressableManager.LoadAssetAsync(assetReference, cancellationToken);

            if (PoolCollection.ContainsKey(poolKey))
            {
                return;
            }

            var newPool = new ObjectPool<GameObject>(
                () => CreatePoolObject(poolKey),
                go => OnPoolObjectGet(poolKey, go),
                go => OnPoolObjectReleased(poolKey, go),
                DisposePoolObject,
                true,
                defaultCapacity,
                maxCapacity
            );

            PoolCollection.Add(
                poolKey, new GameObjectPoolContainer(poolKey, prefab, newPool, onObjectGet, onObjectRelease)
            );

            if (preWarm)
            {
                if (awaitWarmup)
                {
                    await WarmupPool(poolKey, defaultCapacity, cancellationToken);
                }
                else
                {
                    WarmupPool(poolKey, defaultCapacity, cancellationToken).Forget();
                }
            }
        }

        public async UniTask WarmupPool(PoolKeys poolKey, int defaultCapacity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var createdGameObjects = ListPool<GameObject>.Get();

            try
            {
                await UniTask.NextFrame(cancellationToken);
                for (var x = 0; x < defaultCapacity; x++)
                {
                    var go = CreatePoolObject(poolKey);
                    go.SetActive(false);
                    createdGameObjects.Add(go);
                    await UniTask.NextFrame(cancellationToken);
                }
            }
            finally
            {
                foreach (var objectToRelease in createdGameObjects)
                {
                    ReleaseObject(poolKey, objectToRelease);
                }

                ListPool<GameObject>.Release(createdGameObjects);
            }
        }

        private void DisposePool(PoolKeys poolKey)
        {
            if (ContainsPool(poolKey))
            {
                _addressableManager.ReleaseInstance(GetPoolPrefab(poolKey));
                PoolCollection[poolKey].Dispose();
                PoolCollection.Remove(poolKey);
            }
            else
            {
                Debug.LogWarning("Tried to release a pool which was not there:" + poolKey);
            }
        }

        public void SafeDisposePool(PoolKeys poolKey)
        {
            DisposePool(poolKey);
        }

        #endregion Pool Maintenance

        #region Pool Helper Methods

        private GameObject CreatePoolObject(PoolKeys poolKey)
        {
            var original = PoolCollection[poolKey].Prefab;
            if (original == null)
            {
                Debug.LogError("Given prefab for " + poolKey + " is null");
                return null;
            }

            var go = Object.Instantiate(original, Vector3.zero, Quaternion.identity, transform);
            go.name = $"{original.name}-{++_creationId}";
            go.transform.localScale = original.transform.localScale;
            return go;
        }

        private static void DisposePoolObject(GameObject poolObject)
        {
            Object.Destroy(poolObject);
        }

        private void OnPoolObjectGet(PoolKeys poolKey, GameObject getObject)
        {
            getObject.SetActive(true);
            PoolCollection[poolKey].OnGetCallback?.Invoke(getObject);
        }

        private static void OnGetObjectFromPool(GameObject objectFromPool)
        {
            objectFromPool.transform.SetParent(null, false);
            objectFromPool.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        private void OnPoolObjectReleased(PoolKeys poolKey, GameObject releasedObject)
        {
            releasedObject.SetActive(false);
            releasedObject.transform.SetParent(transform, false);
            PoolCollection[poolKey].OnReleaseCallback?.Invoke(releasedObject);
        }

        #endregion Pool Helper Methods

        #region Pool Interface

        public GameObject GetGameObject(PoolKeys poolKey)
        {
            return PoolCollection[poolKey].Pool.Get();
        }

        private void ReleaseObject(PoolKeys poolKey, GameObject releasedObject)
        {
            PoolCollection[poolKey].Pool.Release(releasedObject);
        }

        public bool SafeReleaseObject(PoolKeys poolKey, GameObject releasedObject)
        {
            if (ContainsPool(poolKey))
            {
                ReleaseObject(poolKey, releasedObject);
                return true;
            }

            Object.Destroy(releasedObject);
            return false;
        }

        public bool ContainsPool(PoolKeys poolKey)
        {
            return PoolCollection.ContainsKey(poolKey);
        }

        public GameObject GetPoolPrefab(PoolKeys poolKey)
        {
            return PoolCollection[poolKey].Prefab;
        }

        public int GetPoolCount(PoolKeys poolKey)
        {
            return PoolCollection[poolKey].Pool.CountAll;
        }

        #endregion Pool Interface
    }
}