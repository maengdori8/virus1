using UnityEngine;
using UnityEngine.SceneManagement;


public class UpperUISceneLoader : MonoBehaviour
{
    [SerializeField] private string upperUISceneName = "UpperUI";

    private void Awake()
    {
        if (string.IsNullOrEmpty(upperUISceneName)) return;
        if (IsSceneLoaded(upperUISceneName)) return;

        SceneManager.LoadScene(upperUISceneName, LoadSceneMode.Additive);
    }

    private static bool IsSceneLoaded(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name == sceneName)
                return true;
        }

        return false;
    }
}
