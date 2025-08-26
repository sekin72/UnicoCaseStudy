using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UnicoCaseStudy.SceneControllers
{
    public class LoadingSceneController : SceneControllerBase
    {
        public override UniTask Initialize(CancellationToken cancellationToken)
        {
            ProjectConfiguration();
            return base.Initialize(cancellationToken);
        }

        private void ProjectConfiguration()
        {
            Application.targetFrameRate = 60;
        }
    }
}