using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLauncher : MonoBehaviour
{
    [SerializeField] string popupSceneName   = "01_Popup_Demo";
    [SerializeField] string octopusSceneName = "02_Octopus_Demo";

    public void OpenPopup()   => SceneManager.LoadScene(popupSceneName,   LoadSceneMode.Single);
    public void OpenOctopus() => SceneManager.LoadScene(octopusSceneName, LoadSceneMode.Single);

    public void ExitApp()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBGL
        Application.OpenURL("about:blank");
#else
        Application.Quit();
#endif
    }
}
