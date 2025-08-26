using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace UnicoCaseStudy.Managers.Asset
{
    public class AddressableManager : Manager
    {
        protected override UniTask Initialize(CancellationToken disposeToken)
        {
            return UniTask.CompletedTask;
        }

        public async UniTask<SceneInstance> LoadSceneAdditive(object key, CancellationToken cancellationToken)
        {
            SceneInstance sceneInstance = default;
            var isLoaded = false;
            try
            {
                sceneInstance = await Addressables.LoadSceneAsync(key, LoadSceneMode.Additive, false);
                isLoaded = true;
                cancellationToken.ThrowIfCancellationRequested();
            }
            catch (OperationCanceledException)
            {
                if (isLoaded)
                {
                    await Addressables.UnloadSceneAsync(sceneInstance);
                }

                throw;
            }

            return sceneInstance;
        }

        public async UniTask UnloadScene(SceneInstance sceneInstance, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Addressables.UnloadSceneAsync(sceneInstance);
            cancellationToken.ThrowIfCancellationRequested();
        }

        public async UniTask<T> LoadAssetAsync<T>(AssetReferenceT<T> assetReference, CancellationToken cancellationToken = default)
            where T : Object
        {
            return await LoadAssetAsyncInternal<T>(assetReference, cancellationToken);
        }

        public async UniTask<GameObject> InstantiateAssetAsync(AssetReferenceT<GameObject> assetReference,
            CancellationToken cancellationToken, Vector3 localPosition = default, Quaternion localQuaternion = default,
            Vector3 localScale = default, Transform parent = null)
        {
            return await InstantiateAssetAsyncInternal(
                assetReference, cancellationToken, localPosition, localQuaternion, localScale, parent
            );
        }

        public void ReleaseInstance<T>(T t) where T : Object
        {
            Addressables.Release(t);
        }

        #region Internal

        private static async UniTask<T> LoadAssetAsyncInternal<T>(object assetLocation,
            CancellationToken cancellationToken) where T : Object
        {
            T asset = null;
            try
            {
                asset = await Addressables.LoadAssetAsync<T>(assetLocation);
                cancellationToken.ThrowIfCancellationRequested();
            }
            catch (Exception e)
            {
                if (asset != null)
                {
                    Addressables.Release(asset);
                    asset = null;
                }

                if (e is OperationCanceledException)
                {
                    throw;
                }
            }

            return asset;
        }

        private static async UniTask<GameObject> InstantiateAsyncInternal(object assetLocation,
            InstantiationParameters parameters, CancellationToken cancellationToken)
        {
            GameObject created = null;
            try
            {
                created = await Addressables.InstantiateAsync(assetLocation, parameters);
                cancellationToken.ThrowIfCancellationRequested();
            }
            catch (Exception e)
            {
                if (created != null)
                {
                    Addressables.ReleaseInstance(created);
                    created = null;
                }

                if (e is OperationCanceledException)
                {
                    throw;
                }
            }

            return created;
        }

        private static async UniTask<GameObject> InstantiateAssetAsyncInternal(object assetReference,
            CancellationToken cancellationToken, Vector3 localPosition = default, Quaternion localQuaternion = default,
            Vector3 localScale = default, Transform parent = null)
        {
            var instantiateParameters = new InstantiationParameters(parent, false);
            var created = await InstantiateAsyncInternal(assetReference, instantiateParameters, cancellationToken);
            if (created == null)
            {
                return null;
            }

            created.transform.SetLocalPositionAndRotation(localPosition, localQuaternion);
            created.transform.localScale = localScale;

            return created;
        }

        #endregion Internal
    }
}