using UnityEngine;
using UnityEngine.Events;

public sealed class TimedEventActor : MonoBehaviour, ILoopResettable
{
    [SerializeField, Min(0f)] private float fireAtSeconds = 30f;
    [SerializeField, Min(0f)] private float revertAtSeconds = 45f;
    [SerializeField] private UnityEvent onFire;
    [SerializeField] private UnityEvent onRevert;
    private bool fired;
    private bool reverted;

    private void OnEnable() => SongLoopDirector.Register(this);
    private void OnDisable() => SongLoopDirector.Unregister(this);

    private void Update()
    {
        if (SongLoopDirector.Instance == null) return;
        float time = SongLoopDirector.Instance.SongTime;

        if (!fired && time >= fireAtSeconds)
        {
            fired = true;
            onFire?.Invoke();
        }

        if (fired && !reverted && revertAtSeconds > fireAtSeconds && time >= revertAtSeconds)
        {
            reverted = true;
            onRevert?.Invoke();
        }
    }

    public void OnLoopReset()
    {
        fired = false;
        reverted = false;
        onRevert?.Invoke();
    }
}
