using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class UIAudio : MonoBehaviour
{
    public static UIAudio I;

    [SerializeField] private AudioClip sfxClick; // перетягни click2.ogg сюди
    private AudioSource _asrc;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);          // <-- ключове

        _asrc = GetComponent<AudioSource>();
        _asrc.playOnAwake = false;
        _asrc.loop = false;
        _asrc.spatialBlend = 0f;
        _asrc.clip = null;                      // кліп не призначаємо сюди
    }

    public void PlayClick()
    {
        if (sfxClick) _asrc.PlayOneShot(sfxClick);
    }

    public static void Click()
    {
        if (I != null) I.PlayClick();
    }
}
