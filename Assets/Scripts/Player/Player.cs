using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Estadisticas del Jugador")]
    public float maxHealth = 100f;
    public float currentHealth;
    public float initialSpeed = 5f;
    public float speed;
    public float jumpForce = 5f;

    private Vector2 lastMoveDirection = Vector2.right;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        speed = initialSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateMoveDirection(Vector2 newDirection)
    {
        if (newDirection != Vector2.zero)
        {
            lastMoveDirection = newDirection;
        }
    }
}
