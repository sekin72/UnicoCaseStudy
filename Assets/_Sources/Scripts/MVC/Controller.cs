using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UnicoCaseStudy.MVC
{
    public abstract class Controller<TD, TV> : IController
        where TV : View
        where TD : Data
    {
        public TD Data { get; private set; }
        public TV View { get; private set; }
        Data IController.Data => Data;
        View IController.View => View;

        public async UniTask InitializeController(Data data, View view, CancellationToken cancellationToken)
        {
            if (Data != null && Data.IsInitialized)
            {
                Debug.LogWarning($"Trying to initialize {View.name}, but it's already initialized");
                return;
            }

            Data = data as TD;
            View = view as TV;
            View.SetData(Data);

            Data.IsInitialized = true;
            Data.IsActivated = false;
            Data.IsDeactivated = false;
            Data.IsDisposed = false;

            await Initialize(cancellationToken);

            await View.Initialize(cancellationToken);
        }

        public async UniTask ActivateController(CancellationToken cancellationToken)
        {
            if (Data.IsActivated)
            {
                Debug.LogWarning($"Trying to activate {View.name}, but it's already activated");
                return;
            }

            Data.IsActivated = true;

            await Activate(cancellationToken);

            await View.Activate(cancellationToken);
        }

        public void DeactivateController()
        {
            if (Data.IsDeactivated)
            {
                Debug.LogWarning($"Trying to deactivate {View.name}, but it's already deactivated");
                return;
            }

            Data.IsDeactivated = true;
            Data.IsActivated = false;

            Deactivate();

            View.Deactivate();
        }

        public void DisposeController()
        {
            if (Data.IsDisposed)
            {
                Debug.LogWarning($"Trying to dispose {View.name}, but it's already disposed");
                return;
            }

            Data.IsInitialized = false;
            Data.IsActivated = false;
            Data.IsDeactivated = false;
            Data.IsDisposed = true;

            Dispose();

            View.Dispose();
        }

        public abstract UniTask Initialize(CancellationToken cancellationToken);
        public abstract UniTask Activate(CancellationToken cancellationToken);
        public abstract void Deactivate();
        public abstract void Dispose();
    }
}