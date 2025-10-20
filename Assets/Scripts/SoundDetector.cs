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

    AudioSource micSrc;
    AudioClip micClip;
    float lastClapTime = -10f;
    float[] sampleBuf;
    bool didClapThisFrame;
    float inhibitUntil = 0f;

    // --- Public helpers -----------------------------------------------------

    /// <summary>
    /// Temporarily disables clap detection for a number of seconds.
    /// </summary>
    public void Inhibit(float seconds)
    {
        inhibitUntil = Mathf.Max(inhibitUntil, Time.time + Mathf.Max(0f, seconds));
    }

    /// <summary>
    /// Returns true once after a clap is detected, then resets the flag.
    /// </summary>
    public bool ConsumeClap()
    {
        if (didClapThisFrame)
        {
            didClapThisFrame = false;
            return true;
        }
        return false;
    }

    // --- Unity lifecycle ----------------------------------------------------

    IEnumerator Start()
    {
        yield return StartCoroutine(RequestMicPermission());
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.Microphone))
        {
            Debug.LogWarning("Microphone permission denied.");
            yield break;
        }

        InitializeMicrophone();
    }

    void Update()
    {
        didClapThisFrame = false;

        // Skip if no mic or buffer
        if (micClip == null || sampleBuf == null || sampleBuf.Length == 0) return;

        if (!FetchRecentSamples()) return;

        AnalyzeSamples();
    }

    void OnDisable()
    {
        StopMicrophone();
    }

    // --- Internal helpers ---------------------------------------------------

    /// <summary>
    /// Requests microphone permission on Android at runtime.
    /// </summary>
    IEnumerator RequestMicPermission()
    {
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.Microphone))
        {
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.Microphone);
            // Wait a few frames for the dialog result
            for (int i = 0; i < 60; i++) yield return null;
        }
    }

    /// <summary>
    /// Starts recording from the microphone and sets up buffers.
    /// </summary>
    void InitializeMicrophone()
    {
        micSrc = gameObject.AddComponent<AudioSource>();
        micSrc.loop = true;
        micSrc.mute = true; // keep it silent

        int sr = (AudioSettings.outputSampleRate > 0) ? AudioSettings.outputSampleRate : sampleRate;

        // Start recording
        micClip = Microphone.Start(micDevice, true, 1, sr);

        // Wait until microphone starts providing data
        StartCoroutine(WaitForMicStart(sr));
    }

    /// <summary>
    /// Coroutine that waits until the microphone starts and sets up the sample buffer.
    /// </summary>
    IEnumerator WaitForMicStart(int sr)
    {
        while (Microphone.GetPosition(micDevice) <= 0)
            yield return null;

        micSrc.clip = micClip;
        micSrc.Play();

        int windowSamples = Mathf.Max(64, Mathf.RoundToInt(sr * (windowMs / 1000f)));
        sampleBuf = new float[windowSamples];
    }

    /// <summary>
    /// Reads the most recent window of audio samples into sampleBuf.
    /// </summary>
    bool FetchRecentSamples()
    {
        int windowSamples = sampleBuf.Length;
        int micPos = Microphone.GetPosition(micDevice);

        if (micPos < windowSamples) return false; // not enough data yet

        int start = micPos - windowSamples;
        if (start < 0) start += micClip.samples; // wrap around ring buffer

        micClip.GetData(sampleBuf, start);
        return true;
    }

    /// <summary>
    /// Processes the current sampleBuf to detect if a clap occurred.
    /// </summary>
    void AnalyzeSamples()
    {
        // Compute RMS (average power) and Peak (max amplitude)
        double sumSq = 0.0;
        float peak = 0f;

        for (int i = 0; i < sampleBuf.Length; i++)
        {
            float s = sampleBuf[i];
            sumSq += s * s;
            float a = Mathf.Abs(s);
            if (a > peak) peak = a;
        }

        float rms = Mathf.Sqrt((float)(sumSq / sampleBuf.Length));

        // Convert to decibels (0 dBFS = max amplitude)
        float db = 20f * Mathf.Log10(Mathf.Max(1e-7f, rms));

        // Ratio between instantaneous peak and average level
        float peakToRms = (rms > 1e-6f) ? (peak / rms) : 999f;

        // Determine if sound qualifies as a "clap"
        bool loud = db >= thresholdDb;
        bool spiky = peakToRms >= minPeakToRms;

        // Check timing (debounce + inhibit)
        bool cooledDown = (Time.time - lastClapTime) >= minInterval;
        bool notInhibited = Time.time >= inhibitUntil;

        if (loud && spiky && cooledDown && notInhibited)
        {
            lastClapTime = Time.time;
            didClapThisFrame = true;
        }
    }

    /// <summary>
    /// Stops microphone recording and playback.
    /// </summary>
    void StopMicrophone()
    {
        if (micSrc != null) micSrc.Stop();
        if (Microphone.IsRecording(micDevice))
            Microphone.End(micDevice);
    }
}
