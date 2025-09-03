using UnityEngine;
using UnityEngine.SceneManagement;

public class EscToLauncher : MonoBehaviour
{
    [SerializeField] string launcherScene = "00_Launcher";

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Якщо вже в лаунчері — виходимо з програми
            if (SceneManager.GetActiveScene().name == launcherScene)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBGL
                Application.OpenURL("about:blank");
#else
                Application.Quit();
#endif
            }
            else
            {
                // Інакше повертаємось у лаунчер
                SceneManager.LoadScene(launcherScene, LoadSceneMode.Single);
            }
        }
    }
}
