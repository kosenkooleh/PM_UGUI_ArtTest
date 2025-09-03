using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PopupCloseOnClick : MonoBehaviour
{
    [SerializeField] private Animator targetAnimator;     // can be left empty
    [SerializeField] private string triggerName = "Close";

    void Awake()
    {
        if (!targetAnimator)
        {
            var selfAnimator = GetComponent<Animator>();
            var animators = GetComponentsInParent<Animator>(true);
            foreach (var a in animators)
            {
                if (!a || a == selfAnimator) continue;   // skip own Animator
                if (HasTrigger(a, triggerName)) { targetAnimator = a; break; }
            }
        }
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnDestroy() => GetComponent<Button>().onClick.RemoveListener(OnClick);

    void OnClick()
    {
        if (targetAnimator) targetAnimator.SetTrigger(triggerName);
        else Debug.LogWarning($"{name}: PopupCloseOnClick — Popup Animator not found in parents.");
    }

    static bool HasTrigger(Animator a, string trig)
    {
        foreach (var p in a.parameters)
            if (p.type == AnimatorControllerParameterType.Trigger && p.name == trig)
                return true;
        return false;
    }
}
