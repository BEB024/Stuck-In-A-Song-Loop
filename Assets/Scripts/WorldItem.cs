using UnityEngine;

[RequireComponent(typeof(Collider))]
public sealed class WorldItem : MonoBehaviour, IInteractable, ILoopResettable
{
    [SerializeField] private string itemId = "brass_key";
    [SerializeField] private string displayName = "brass key";
    [SerializeField] private Renderer[] renderers;
    [SerializeField] private Collider interactionCollider;
    private bool collected;

    public string Prompt => $"take {displayName}";

    private void Reset()
    {
        interactionCollider = GetComponent<Collider>();
        renderers = GetComponentsInChildren<Renderer>();
    }

    private void OnEnable() => SongLoopDirector.Register(this);
    private void OnDisable() => SongLoopDirector.Unregister(this);

    public bool CanInteract(GameObject interactor) => !collected && interactor.GetComponentInParent<PlayerInventory>() != null;

    public void Interact(GameObject interactor)
    {
        PlayerInventory inventory = interactor.GetComponentInParent<PlayerInventory>();
        if (inventory.Add(itemId))
        {
            collected = true;
            SetVisible(false);
        }
    }

    public void OnLoopReset()
    {
        collected = PersistentVault.Instance != null && PersistentVault.Instance.Contains(itemId);
        SetVisible(!collected);
    }

    private void SetVisible(bool value)
    {
        foreach (Renderer itemRenderer in renderers) itemRenderer.enabled = value;
        interactionCollider.enabled = value;
    }
}
