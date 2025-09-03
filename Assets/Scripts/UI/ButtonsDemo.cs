using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[ExecuteAlways]
public class ButtonsDemo : MonoBehaviour
{
    [Header("References")]
    public RectTransform container;     // drag in ButtonsContainer (RectTransform)
    public GameObject buttonPrefab;     // drag in PopUpButton.prefab

    [Header("Config")]
    [Range(1, 3)] public int count = 1;
    public string[] labels = { "SHARE", "COLLECT", "CLOSE" };

    bool busy;
    bool pendingEditRebuild;            // so we don't call DestroyImmediate inside OnValidate

    void OnEnable()
    {
        // flag rebuild when the component refreshes/enables
        pendingEditRebuild = true;
    }

    void OnValidate()
    {
        // in the editor just mark "rebuild needed"
        if (!Application.isPlaying) pendingEditRebuild = true;
    }

    void Update()
    {
        // Safe rebuild in Edit Mode (not inside OnValidate)
        if (!Application.isPlaying && pendingEditRebuild && !busy)
        {
            pendingEditRebuild = false;
            RebuildImmediate(); // DestroyImmediate is safe here
        }
    }

    [ContextMenu("Rebuild (Play)")]
    public void Rebuild()
    {
        if (busy || !container || !buttonPrefab) return;
        if (Application.isPlaying) StartCoroutine(RebuildRuntime());
        else pendingEditRebuild = true; // just in case
    }

    // ---------- EDIT MODE ----------
    void RebuildImmediate()
    {
        if (busy || !container || !buttonPrefab) return;
        busy = true;

        Debug.Log($"[ButtonsDemo] (Edit) Before clear: {container.childCount}", this);

        for (int i = container.childCount - 1; i >= 0; i--)
            DestroyImmediate(container.GetChild(i).gameObject);

        Debug.Log($"[ButtonsDemo] (Edit) After clear: {container.childCount}", this);

        int n = Mathf.Clamp(count, 1, 3);
        Build(n);

        Debug.Log($"[ButtonsDemo] (Edit) Built: {n}", this);

        LayoutRebuilder.ForceRebuildLayoutImmediate(container);
        busy = false;
    }

    // ---------- PLAY MODE ----------
    IEnumerator RebuildRuntime()
    {
        if (busy || !container || !buttonPrefab) yield break;
        busy = true;

        Debug.Log($"[ButtonsDemo] (Play) Before clear: {container.childCount}", this);

        for (int i = container.childCount - 1; i >= 0; i--)
            Destroy(container.GetChild(i).gameObject);

        yield return null; // wait for actual destruction

        Debug.Log($"[ButtonsDemo] (Play) After clear: {container.childCount}", this);

        int n = Mathf.Clamp(count, 1, 3);
        Build(n);

        Debug.Log($"[ButtonsDemo] (Play) Built: {n}", this);

        LayoutRebuilder.ForceRebuildLayoutImmediate(container);
        busy = false;
    }

    // ---------- BUILD ----------
    void Build(int n)
    {
        for (int i = 0; i < n; i++)
        {
            var go = Instantiate(buttonPrefab, container);

            var le = go.GetComponent<LayoutElement>() ?? go.AddComponent<LayoutElement>();
            le.minHeight = 96;
            le.flexibleWidth = 1;

            var txt = go.GetComponentInChildren<TextMeshProUGUI>();
            if (txt)
            {
                if (n == 1) txt.text = "COLLECT";            // single-button case from the brief
                else txt.text = labels[Mathf.Min(i, labels.Length - 1)];
            }
        }
    }
}
