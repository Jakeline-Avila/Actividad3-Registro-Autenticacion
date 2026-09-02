using UnityEngine;
using UnityEngine.InputSystem;

public class BirdController : MonoBehaviour
{
    [SerializeField] private float jumpForce = 5f;

    private Rigidbody2D rb;
    private bool dead = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (dead)
            return;

        if (Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Jump();
        }

        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            Jump();
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(0, jumpForce);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("🐦 COLISIÓN CON: " + collision.gameObject.name);

        if (dead)
            return;

        dead = true;

        GameManager.Instance.GameOver();
    }
}