#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

namespace KaanUtilities.Editor
{
    public class SceneLoaderMenuItem
    {

        [MenuItem("CerberusFramework/Scenes/LoadingScene", false, 50)]
        public static void LoadLoadingScene()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene("Assets/_Sources/Scenes/LoadingScene.unity", OpenSceneMode.Single);
            }
        }

        [MenuItem("CerberusFramework/Scenes/MainMenuScene", false, 51)]
        public static void LoadMainScene()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene("Assets/_Sources/Scenes/MainMenuScene.unity", OpenSceneMode.Single);
            }
        }

        [MenuItem("CerberusFramework/Scenes/LevelScene", false, 52)]
        public static void LoadLevelScene()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene("Assets/_Sources/Scenes/GameplayScene.unity", OpenSceneMode.Single);
            }
        }
    }
}
#endif