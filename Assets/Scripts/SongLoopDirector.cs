using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public sealed class SongLoopDirector : MonoBehaviour
{
    public static SongLoopDirector Instance { get; private set; }
    public static bool InputBlocked => Instance != null && Instance.inputBlocked;

    [Header("Loop")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField, Min(10f)] private float loopLengthSeconds = 180f;
    [SerializeField] private float audioScheduleLead = 0.15f;
    [SerializeField] private float resetBlackoutSeconds = 0.8f;

    [Header("UI")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text loopText;
    [SerializeField] private CanvasGroup blackout;
    [SerializeField] private TMP_Text endingText;
    [SerializeField] private UnityEvent onEscape;

    private static readonly HashSet<ILoopResettable> Resettables = new();
    private double dspStart;
    private int loopNumber = 1;
    private bool resetting;
    private bool escaped;
    private bool inputBlocked;

    public float SongTime => escaped ? loopLengthSeconds : Mathf.Clamp((float)(AudioSettings.dspTime - dspStart), 0f, loopLengthSeconds);
    public float LoopLength => loopLengthSeconds;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        if (blackout != null) blackout.alpha = 0f;
        if (endingText != null) endingText.gameObject.SetActive(false);
    }

    private void Start() => ScheduleSong();

    private void Update()
    {
        if (escaped) return;

        float remaining = Mathf.Max(0f, loopLengthSeconds - SongTime);
        int minutes = Mathf.FloorToInt(remaining / 60f);
        int seconds = Mathf.FloorToInt(remaining % 60f);
        if (timerText != null) timerText.text = $"{minutes:00}:{seconds:00}";
        if (loopText != null) loopText.text = $"LOOP {loopNumber}";

        if (!resetting && SongTime >= loopLengthSeconds)
            StartCoroutine(ResetRoutine());
    }

    private void ScheduleSong()
    {
        musicSource.Stop();
        musicSource.loop = false;
        dspStart = AudioSettings.dspTime + audioScheduleLead;
        musicSource.PlayScheduled(dspStart);
    }

    private IEnumerator ResetRoutine()
    {
        resetting = true;
        inputBlocked = true;
        musicSource.Stop();

        if (blackout != null)
        {
            float elapsed = 0f;
            while (elapsed < resetBlackoutSeconds)
            {
                elapsed += Time.unscaledDeltaTime;
                blackout.alpha = Mathf.Clamp01(elapsed / resetBlackoutSeconds);
                yield return null;
            }
        }

        foreach (ILoopResettable resettable in new List<ILoopResettable>(Resettables))
            resettable?.OnLoopReset();

        loopNumber++;
        ScheduleSong();

        if (blackout != null)
        {
            float elapsed = 0f;
            while (elapsed < resetBlackoutSeconds)
            {
                elapsed += Time.unscaledDeltaTime;
                blackout.alpha = 1f - Mathf.Clamp01(elapsed / resetBlackoutSeconds);
                yield return null;
            }
        }

        inputBlocked = false;
        resetting = false;
    }

    public void Escape()
    {
        if (escaped) return;
        escaped = true;
        inputBlocked = true;
        musicSource.Stop();
        if (timerText != null) timerText.text = "ESCAPED";
        if (endingText != null)
        {
            endingText.gameObject.SetActive(true);
            endingText.text = "The song finally continues.";
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        onEscape?.Invoke();
    }

    public static void Register(ILoopResettable resettable) => Resettables.Add(resettable);
    public static void Unregister(ILoopResettable resettable) => Resettables.Remove(resettable);
}
