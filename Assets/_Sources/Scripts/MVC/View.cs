using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UnicoCaseStudy.MVC
{
    public abstract class View : MonoBehaviour, IView, ILifecycle
    {
        protected Data Data { get; private set; }

        public abstract UniTask Initialize(CancellationToken cancellationToken);

        public abstract UniTask Activate(CancellationToken cancellationToken);

        public abstract void Deactivate();

        public abstract void Dispose();

        public void SetData(Data data)
        {
            Data = data;
        }
    }
}