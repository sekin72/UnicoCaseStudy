using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UnicoCaseStudy.Managers.Loading
{
    [CreateAssetMenu(fileName = "SceneReferencesHolder", menuName = "UnicoCaseStudy/Configs/SceneReferencesHolder", order = 1)]
    public class SceneReferencesHolder : ScriptableObject
    {
        public AssetReference MainMenuSceneReference;
        public AssetReference GameplaySceneReference;
    }
}