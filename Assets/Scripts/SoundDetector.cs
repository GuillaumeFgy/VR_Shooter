using UnityEngine;
using System.Collections;

public class SoundDetector : MonoBehaviour
{
    [Header("Detection")]
    [Tooltip("Window size in milliseconds for level analysis.")]
    public float windowMs = 20f;
    [Tooltip("Minimum level in dBFS to count as a clap (higher = stricter).")]
    public float thresholdDb = -28f;
    [Tooltip("Minimum seconds between claps (debounce).")]
    public float minInterval = 0.45f;
    [Tooltip("How 'spiky' the sound must be (peak vs RMS). Higher rejects sustained noise.")]
    public float minPeakToRms = 3.0f;

    [Header("Mic")]
    [Tooltip("Leave empty to use default mic.")]
    public string micDevice = null;
    public int sampleRate = 44100;

    [Header("Debug")]
    public bool logClaps = false;

    AudioSource micSrc;
    AudioClip micClip;
    float lastClapTime = -10f;
    float[] sampleBuf;
    bool didClapThisFrame;

    float inhibitUntil = 0f;

    public void Inhibit(float seconds)
    {
        inhibitUntil = Mathf.Max(inhibitUntil, Time.time + Mathf.Max(0f, seconds));
    }

    public bool DidClapThisFrame()
    {
        return didClapThisFrame;
    }

    public bool ConsumeClap()
    {
        if (didClapThisFrame)
        {
            didClapThisFrame = false;
            return true;
        }
        return false;
    }

    IEnumerator Start()
    {
        // Runtime permission (Android 6+)
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.Microphone))
        {
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.Microphone);
            // Wait a few frames for the dialog result
            for (int i=0; i<60; i++) yield return null;
        }
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.Microphone))
        {
            Debug.LogWarning("Microphone permission denied.");
            yield break;
        }

        micSrc = gameObject.AddComponent<AudioSource>();
        micSrc.loop = true;
        micSrc.mute = true; // keep it silent

        // Use output sample rate if available
        int sr = (AudioSettings.outputSampleRate > 0) ? AudioSettings.outputSampleRate : sampleRate;
        micClip = Microphone.Start(micDevice, true, 1, sr);
        // Wait until the mic starts
        while (Microphone.GetPosition(micDevice) <= 0) yield return null;

        micSrc.clip = micClip;
        micSrc.Play();

        int windowSamples = Mathf.Max(64, Mathf.RoundToInt(sr * (windowMs / 1000f)));
        sampleBuf = new float[windowSamples];
    }

    void Update()
    {
        didClapThisFrame = false;
        if (micClip == null || sampleBuf == null || sampleBuf.Length == 0) return;

        int windowSamples = sampleBuf.Length;
        int micPos = Microphone.GetPosition(micDevice);
        if (micPos < windowSamples) return; // not enough data yet

        int start = micPos - windowSamples;
        if (start < 0) start += micClip.samples; // wrap around ring buffer

        micClip.GetData(sampleBuf, start);

        // Compute RMS and PEAK
        double sumSq = 0.0;
        float peak = 0f;
        for (int i = 0; i < windowSamples; i++)
        {
            float s = sampleBuf[i];
            sumSq += s * s;
            float a = Mathf.Abs(s);
            if (a > peak) peak = a;
        }
        float rms = Mathf.Sqrt((float)(sumSq / windowSamples));
        // dBFS: 0 dB = full scale (amplitude 1.0)
        float db = 20f * Mathf.Log10(Mathf.Max(1e-7f, rms));

        // Heuristic: must be loud enough AND spiky (peak>>rms)
        float peakToRms = (rms > 1e-6f) ? (peak / rms) : 999f;
        bool loud = db >= thresholdDb;
        bool spiky = peakToRms >= minPeakToRms;

        // Debounce + external inhibit gate
        bool cooledDown = (Time.time - lastClapTime) >= minInterval;
        bool notInhibited = Time.time >= inhibitUntil;

        if (loud && spiky && cooledDown && notInhibited)
        {
            lastClapTime = Time.time;
            didClapThisFrame = true;
            if (logClaps) Debug.Log($"Clap! db={db:F1} peak/rms={peakToRms:F2}");
        }
    }


    void OnDisable()
    {
        if (micSrc != null) micSrc.Stop();
        if (Microphone.IsRecording(micDevice))
            Microphone.End(micDevice);
    }
}
