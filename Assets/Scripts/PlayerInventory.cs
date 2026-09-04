using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public sealed class PlayerInventory : MonoBehaviour, ILoopResettable
{
    [SerializeField] private TMP_Text inventoryText;
    private readonly HashSet<string> itemIds = new();

    public IReadOnlyCollection<string> Items => itemIds;

    private void OnEnable() => SongLoopDirector.Register(this);
    private void OnDisable() => SongLoopDirector.Unregister(this);

    public bool Contains(string itemId) => itemIds.Contains(itemId);

    public bool Add(string itemId)
    {
        bool added = itemIds.Add(itemId);
        RefreshUI();
        return added;
    }

    public bool Remove(string itemId)
    {
        bool removed = itemIds.Remove(itemId);
        RefreshUI();
        return removed;
    }

    public void ReplaceAll(IEnumerable<string> items)
    {
        itemIds.Clear();
        foreach (string item in items) itemIds.Add(item);
        RefreshUI();
    }

    public void OnLoopReset()
    {
        itemIds.Clear();
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (inventoryText == null) return;
        inventoryText.text = itemIds.Count == 0
            ? "POCKETS: EMPTY"
            : "POCKETS: " + string.Join(", ", itemIds.OrderBy(x => x).Select(Pretty));
    }

    private static string Pretty(string id) => id.Replace('_', ' ').ToUpperInvariant();
}
