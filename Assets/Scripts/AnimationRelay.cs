using UnityEngine;

public sealed class AnimationRelay : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void SetRaised(bool value) => animator.SetBool("Raised", value);
    public void SetPresent(bool value) => animator.SetBool("Present", value);
    public void SetAligned(bool value) => animator.SetBool("Aligned", value);
    public void TriggerOpen() => animator.SetTrigger("Open");
}
