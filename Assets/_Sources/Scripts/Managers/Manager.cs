using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UnicoCaseStudy.Managers
{
    public abstract class Manager : MonoBehaviour, IManager, IDisposable
    {
        public bool IsInitialized { get; private set; } = false;
        protected CancellationToken DisposeToken;
        private CancellationTokenSource _disposeTokenSource;

        public async UniTask StartAsync(CancellationToken cancellation)
        {
            _disposeTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellation);
            DisposeToken = _disposeTokenSource.Token;

            await WaitDependencies(DisposeToken);
            await Initialize(DisposeToken);

            IsInitialized = true;
            SaveData();
        }

        protected virtual UniTask WaitDependencies(CancellationToken disposeToken)
        {
            return UniTask.CompletedTask;
        }

        protected abstract UniTask Initialize(CancellationToken disposeToken);

        public virtual void Dispose()
        {
            SaveData();

            _disposeTokenSource?.Cancel();
            _disposeTokenSource?.Dispose();
        }

        #region Data

        protected virtual void SaveData() { }

        protected virtual void LoadData() { }

        #endregion Data
    }
}