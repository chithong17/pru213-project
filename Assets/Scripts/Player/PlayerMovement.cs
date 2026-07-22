using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private PlayerInput playerInput;
    private InputAction moveAction;

    public Vector2 MoveInput
    {
        get { return moveInput; }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        if (playerInput == null || playerInput.actions == null)
        {
            return;
        }

        moveAction = playerInput.actions["Move"];
    }

    private void Update()
    {
        ReadMoveInput();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void ReadMoveInput()
    {
        if (moveAction == null)
        {
            moveInput = Vector2.zero;
            return;
        }

        moveInput = moveAction.ReadValue<Vector2>().normalized;
    }

    private void MovePlayer()
    {
        Vector2 movement = moveInput * moveSpeed * Time.fixedDeltaTime;

        if (rb != null)
        {
            rb.MovePosition(rb.position + movement);
            return;
        }

        transform.position += (Vector3)movement;
    }
}
