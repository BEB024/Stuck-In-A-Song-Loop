using System.Collections.Generic;
using UnityEngine;

public sealed class PuzzleDoor : MonoBehaviour, IInteractable, ILoopResettable
{
    [SerializeField] private string doorName = "door";
    [SerializeField] private List<string> requiredItems = new();
    [SerializeField] private bool consumeItems;
    [SerializeField] private bool useTimeWindow;
    [SerializeField] private Vector2 openWindow = new(0f, 180f);
    [SerializeField] private Animator animator;
    [SerializeField] private Collider blocker;
    private bool open;

    public string Prompt => open ? $"close {doorName}" : $"open {doorName}";

    private void OnEnable() => SongLoopDirector.Register(this);
    private void OnDisable() => SongLoopDirector.Unregister(this);

    public bool CanInteract(GameObject interactor) => interactor.GetComponentInParent<PlayerInventory>() != null;

    public void Interact(GameObject interactor)
    {
        if (open)
        {
            SetOpen(false);
            return;
        }

        PlayerInventory inventory = interactor.GetComponentInParent<PlayerInventory>();
        if (useTimeWindow)
        {
            float songTime = SongLoopDirector.Instance.SongTime;
            if (songTime < openWindow.x || songTime > openWindow.y)
            {
                Notify(interactor, $"The {doorName} only releases during its musical cue.");
                return;
            }
        }

        foreach (string item in requiredItems)
        {
            if (!inventory.Contains(item))
            {
                Notify(interactor, $"The {doorName} still needs {item.Replace('_', ' ')}.");
                return;
            }
        }

        if (consumeItems)
            foreach (string item in requiredItems) inventory.Remove(item);
        SetOpen(true);
    }

    public void OnLoopReset() => SetOpen(false);

    private void SetOpen(bool value)
    {
        open = value;
        animator?.SetBool("Open", open);
        if (blocker != null) blocker.enabled = !open;
    }

    private static void Notify(GameObject interactor, string message)
    {
        RayInteractor rayInteractor = interactor.GetComponentInParent<RayInteractor>();
        if (rayInteractor != null) rayInteractor.ShowMessage(message);
    }
}
