using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public sealed class PersistentVault : MonoBehaviour, IInteractable
{
    public static PersistentVault Instance { get; private set; }

    [SerializeField] private int capacity = 3;
    [SerializeField] private TMP_Text label;
    [SerializeField] private Animator animator;
    private readonly HashSet<string> stored = new();

    public string Prompt => stored.Count > 0 ? "exchange items with the memory chest" : "store carried items in the memory chest";

    private void Awake()
    {
        Instance = this;
        RefreshLabel();
    }

    public bool Contains(string itemId) => stored.Contains(itemId);
    public bool CanInteract(GameObject interactor) => interactor.GetComponentInParent<PlayerInventory>() != null;

    public void Interact(GameObject interactor)
    {
        PlayerInventory inventory = interactor.GetComponentInParent<PlayerInventory>();
        List<string> carried = inventory.Items.ToList();
        List<string> previouslyStored = stored.ToList();

        stored.Clear();
        foreach (string item in carried.Take(capacity)) stored.Add(item);
        inventory.ReplaceAll(previouslyStored);
        animator?.SetTrigger("Use");
        RefreshLabel();
    }

    private void RefreshLabel()
    {
        if (label == null) return;
        label.text = stored.Count == 0 ? "CHEST: EMPTY" : "CHEST: " + string.Join(", ", stored.Select(x => x.Replace('_', ' ')));
    }
}
