using System.Collections.Generic;
using UnityEngine;

public sealed class SongExit : MonoBehaviour, IInteractable
{
    [SerializeField] private List<string> requiredItems = new() { "fuse", "iron_crank" };
    [SerializeField] private Vector2 escapeWindow = new(140f, 165f);

    public string Prompt => "open the final gate";
    public bool CanInteract(GameObject interactor) => interactor.GetComponentInParent<PlayerInventory>() != null;

    public void Interact(GameObject interactor)
    {
        PlayerInventory inventory = interactor.GetComponentInParent<PlayerInventory>();
        foreach (string item in requiredItems)
        {
            if (!inventory.Contains(item))
            {
                Notify(interactor, $"The gate requires {item.Replace('_', ' ')}.");
                return;
            }
        }

        float songTime = SongLoopDirector.Instance.SongTime;
        if (songTime < escapeWindow.x || songTime > escapeWindow.y)
        {
            Notify(interactor, "The mechanism is not aligned with the song.");
            return;
        }

        SongLoopDirector.Instance.Escape();
    }

    private static void Notify(GameObject interactor, string message)
    {
        RayInteractor rayInteractor = interactor.GetComponentInParent<RayInteractor>();
        if (rayInteractor != null) rayInteractor.ShowMessage(message);
    }
}
