using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public sealed class FirstPersonController : MonoBehaviour, ILoopResettable
{
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private float walkSpeed = 4.5f;
    [SerializeField] private float lookSensitivity = 0.12f;
    [SerializeField] private float gravity = -24f;

    private CharacterController controller;
    private Vector3 verticalVelocity;
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
    private float pitch;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SongLoopDirector.Register(this);
    }

    private void OnDisable() => SongLoopDirector.Unregister(this);

    private void Update()
    {
        if (Keyboard.current == null || Mouse.current == null || SongLoopDirector.InputBlocked)
            return;

        Vector2 look = Mouse.current.delta.ReadValue() * lookSensitivity;
        transform.Rotate(Vector3.up, look.x);
        pitch = Mathf.Clamp(pitch - look.y, -85f, 85f);
        cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        Vector2 input = Vector2.zero;
        if (Keyboard.current.wKey.isPressed) input.y += 1f;
        if (Keyboard.current.sKey.isPressed) input.y -= 1f;
        if (Keyboard.current.dKey.isPressed) input.x += 1f;
        if (Keyboard.current.aKey.isPressed) input.x -= 1f;
        input = Vector2.ClampMagnitude(input, 1f);

        Vector3 horizontal = (transform.forward * input.y + transform.right * input.x) * walkSpeed;
        if (controller.isGrounded && verticalVelocity.y < 0f)
            verticalVelocity.y = -2f;
        verticalVelocity.y += gravity * Time.deltaTime;
        controller.Move((horizontal + verticalVelocity) * Time.deltaTime);
    }

    public void OnLoopReset()
    {
        controller.enabled = false;
        transform.SetPositionAndRotation(spawnPosition, spawnRotation);
        controller.enabled = true;
        pitch = 0f;
        cameraPivot.localRotation = Quaternion.identity;
        verticalVelocity = Vector3.zero;
    }
}
