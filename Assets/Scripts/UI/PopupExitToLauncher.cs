using UnityEngine;
using UnityEngine.SceneManagement;

public class PopupExitToLauncher : MonoBehaviour
{
    [SerializeField] string launcherScene = "00_Launcher";
    [SerializeField] float delaySec = 0f; // if you want a tiny extra delay (optional)

    // Call this from an Animation Event at the end of PopUp_Close
    public void ExitToLauncher()
    {
        if (delaySec > 0f)
            StartCoroutine(LoadAfter(delaySec));
        else
            SceneManager.LoadScene(launcherScene, LoadSceneMode.Single);
    }

    System.Collections.IEnumerator LoadAfter(float t)
    {
        // use unscaled time to ignore timescale changes
        float elapsed = 0f;
        while (elapsed < t) { elapsed += Time.unscaledDeltaTime; yield return null; }
        SceneManager.LoadScene(launcherScene, LoadSceneMode.Single);
    }
}
