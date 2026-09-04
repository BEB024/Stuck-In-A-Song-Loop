using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class RayInteractor : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float range = 2.7f;
    [SerializeField] private LayerMask interactionMask = ~0;
    [SerializeField] private TMP_Text promptText;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private float messageSeconds = 2f;

    private float hideMessageAt;

    private void Update()
    {
        if (messageText != null && Time.unscaledTime >= hideMessageAt)
            messageText.text = string.Empty;

        if (SongLoopDirector.InputBlocked)
        {
            promptText.text = string.Empty;
            return;
        }

        IInteractable interactable = FindTarget();
        promptText.text = interactable == null ? string.Empty : $"[E] {interactable.Prompt}";

        if (interactable != null && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (interactable.CanInteract(gameObject))
                interactable.Interact(gameObject);
            else
                ShowMessage("That cannot be used yet.");
        }
    }

    private IInteractable FindTarget()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (!Physics.Raycast(ray, out RaycastHit hit, range, interactionMask, QueryTriggerInteraction.Collide))
            return null;

        MonoBehaviour[] behaviours = hit.collider.GetComponentsInParent<MonoBehaviour>();
        foreach (MonoBehaviour behaviour in behaviours)
            if (behaviour is IInteractable interactable)
                return interactable;
        return null;
    }

    public void ShowMessage(string message)
    {
        if (messageText == null) return;
        messageText.text = message;
        hideMessageAt = Time.unscaledTime + messageSeconds;
    }
}
