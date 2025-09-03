using UnityEngine;

public class OctopusFKLinkedSynced : MonoBehaviour
{
    [System.Serializable]
    public class Chain
    {
        public string name = "t01";

        [Header("Segments (SPRs, not bones)")]
        public Transform start;   // e.g., tentacle-01 or tentacle-04-start
        public Transform middle;  // tentacle-04-middle (may be null)
        public Transform tip;     // tentacle-04-tip (may be null)

        [Header("Motion")]
        [Tooltip("Amplitude in degrees at the base (start)")]
        public float amplitude = 10f;
        [Range(0f, 1f), Tooltip("Attenuation toward the tip (start→middle→tip)")]
        public float falloff = 0.55f;
        [Tooltip("Extra phase in radians (in addition to lag)")]
        public float phaseRad = 0f;

        [Header("Lag vs breathing (60 fps scale)")]
        [Tooltip("Delay relative to the 'inhale' in frames (1 frame = 1/60 s)")]
        public int lagFrames = 0;

        [Header("Micro-noise (optional)")]
        [Tooltip("Micro-noise amplitude in degrees")]
        public float noiseAmp = 0f;
        [Tooltip("Micro-noise speed (cycles/sec)")]
        public float noiseSpeed = 0.2f;

        // Captured rest pose
        Vector3 sPos, mPos, tPos;
        float sRotZ, mRotZ, tRotZ;
        Vector3 s_to_m_local, m_to_t_local;
        float noiseSeed;

        public void Capture()
        {
            noiseSeed = Random.value * 1000f;

            if (start) { sPos = start.position; sRotZ = start.rotation.eulerAngles.z; }
            if (middle)
            {
                mPos = middle.position; mRotZ = middle.rotation.eulerAngles.z;
                if (start) s_to_m_local = Quaternion.Inverse(start.rotation) * (mPos - sPos);
            }
            if (tip)
            {
                tPos = tip.position; tRotZ = tip.rotation.eulerAngles.z;
                if (middle) m_to_t_local = Quaternion.Inverse(middle.rotation) * (tPos - mPos);
            }
        }

        public void Tick(float timeSec, float breathPeriodSec)
        {
            if (!start && !middle && !tip) return;

            // Base phase from breathing (2π every breathPeriodSec)
            float period = Mathf.Max(0.0001f, breathPeriodSec);
            float basePhase = (timeSec / period) * Mathf.PI * 2f;

            // Lag in seconds and conversion to radians
            float lagSec = Mathf.Max(0, lagFrames) / 60f;
            float lagPhase = (lagSec / period) * Mathf.PI * 2f;

            // Micro-noise (degrees)
            float noiseDeg = (noiseAmp != 0f)
                ? Mathf.Sin((timeSec + noiseSeed) * noiseSpeed * Mathf.PI * 2f) * noiseAmp
                : 0f;

            // Angle at the base (start)
            float a0 = Mathf.Sin(basePhase + lagPhase + phaseRad) * amplitude + noiseDeg;

            // START: reset base position and rotate
            if (start)
            {
                start.position = sPos;
                start.rotation = Quaternion.Euler(0, 0, sRotZ + a0);
            }

            // MIDDLE: attach to the end of START and rotate with falloff
            if (middle)
            {
                if (start)
                {
                    Vector3 worldJoint = start.position + start.rotation * s_to_m_local;
                    middle.position = worldJoint;
                }
                float z = mRotZ + a0 * falloff;
                middle.rotation = Quaternion.Euler(0, 0, z);
            }

            // TIP: attach to the end of MIDDLE and rotate with additional falloff
            if (tip)
            {
                if (middle)
                {
                    Vector3 worldJoint = middle.position + middle.rotation * m_to_t_local;
                    tip.position = worldJoint;
                }
                float z = tRotZ + a0 * falloff * falloff;
                tip.rotation = Quaternion.Euler(0, 0, z);
            }
        }
    }

    [Header("Sync with breathing")]
    [Tooltip("Duration of one inhale–exhale cycle (seconds)")]
    public float breathPeriodSec = 2f;

    [Tooltip("Use time from an animation clip (animate the field below in Idle)")]
    public bool useClipTime = false;

    [Tooltip("Clip time in seconds (animate 0→clip length linearly)")]
    public float clipTimeSec = 0f;

    [Header("Head (optional — better animate in clip)")]
    public Transform head;          // sprite head; leave null if unused
    public float headAmp = 0f;      // 0 = script does not affect the head
    public float headSpeed = 0.5f;  // if head is enabled: period ≈ 2 s

    [Header("Tentacles (fill your 4 chains)")]
    public Chain[] tentacles;

    float playStartTime;

    void Start()
    {
        playStartTime = Time.time;

        if (tentacles != null)
            foreach (var c in tentacles) c?.Capture();
    }

    void Update()
    {
        float tSec = useClipTime ? clipTimeSec : (Time.time - playStartTime);

        // Head (if needed; otherwise animate in clip)
        if (head && headAmp > 0f)
        {
            float z = Mathf.Sin(tSec * headSpeed * Mathf.PI * 2f + Mathf.PI * 0.5f) * headAmp;
            var e = head.localEulerAngles; e.z = z; head.localEulerAngles = e;
        }

        if (tentacles != null)
            foreach (var c in tentacles) c?.Tick(tSec, breathPeriodSec);
    }

    [ContextMenu("Re-capture rest pose")]
    void Recapture()
    {
        if (tentacles != null)
            foreach (var c in tentacles) c?.Capture();
    }

    void OnValidate()
    {
        breathPeriodSec = Mathf.Max(0.0001f, breathPeriodSec);
        if (tentacles != null)
            foreach (var c in tentacles)
                if (c != null)
                {
                    c.falloff = Mathf.Clamp01(c.falloff);
                    c.noiseSpeed = Mathf.Max(0f, c.noiseSpeed);
                    c.amplitude = Mathf.Max(0f, c.amplitude);
                    c.lagFrames = Mathf.Max(0, c.lagFrames);
                }
    }
}
