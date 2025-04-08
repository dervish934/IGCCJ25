using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    private Vector2 moveInput;
    private Rigidbody2D rb;
    private PlayerInputActions inputActions;
    private Player player;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    [Header("Suelo")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    private bool isGrounded = false;

    private bool isCrouching = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputActions = new PlayerInputActions();
        player = GetComponent<Player>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        if (inputActions == null) return;

        inputActions.Gameplay.Enable();
        inputActions.Gameplay.Move.performed += ctx => UpdateMoveInput(ctx.ReadValue<Vector2>());
        inputActions.Gameplay.Move.canceled += ctx => UpdateMoveInput(Vector2.zero);
        inputActions.Gameplay.Jump.performed += ctx => Jump();
        inputActions.Gameplay.Crouch.performed += ctx => StartCrouch();
        inputActions.Gameplay.Crouch.canceled += ctx => StopCrouch();
    }

    private void OnDisable()
    {
        if (inputActions == null) return;

        inputActions.Gameplay.Move.performed -= ctx => UpdateMoveInput(ctx.ReadValue<Vector2>());
        inputActions.Gameplay.Move.canceled -= ctx => UpdateMoveInput(Vector2.zero);
        inputActions.Gameplay.Jump.performed -= ctx => Jump();
        inputActions.Gameplay.Crouch.performed -= ctx => StartCrouch();
        inputActions.Gameplay.Crouch.canceled -= ctx => StopCrouch();

        inputActions.Gameplay.Disable();
    }

    private void UpdateMoveInput(Vector2 input)
    {
        moveInput = new Vector2(input.x, 0f);
        player.UpdateMoveDirection(moveInput);
    }

    private void FixedUpdate()
    {
        CheckGrounded();
        MovePlayer();
    }

    private void MovePlayer()
    {
        rb.linearVelocity = new Vector2(moveInput.x * player.speed, rb.linearVelocity.y);

        if (spriteRenderer != null && moveInput.x != 0)
        {
            spriteRenderer.flipX = moveInput.x < 0;
        }
    }

    private void Jump()
    {
        if (isGrounded && !isCrouching)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, player.jumpForce);
            isGrounded = false;
        }
    }

    private void StartCrouch()
    {
        isCrouching = true;
    }

    private void StopCrouch()
    {
        isCrouching = false;
    }

    //TODO Crear layer o validacion de lo que está abajo para no saltar sobre personajes por ejemplo
    private void CheckGrounded()
    {
        if (groundCheck != null)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundCheckRadius);

            isGrounded = false;

            foreach (var col in colliders)
            {
                if (!col.CompareTag("Player")) // Ignora el propio jugador
                {
                    isGrounded = true;
                    break;
                }
            }
        }
    }

#if UNITY_EDITOR
    // Para visualizar el área de groundCheck en el editor
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
#endif
}
