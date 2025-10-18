using UnityEngine;
using UnityEngine.InputSystem;

public class Robot : MonoBehaviour
{
    private Rigidbody2D body;
    private bool shouldMove = false; // When true the robot moves right at 'speed'
    private bool hasEnemy = true;

    [SerializeField] private float speed = 5f;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log($"Enemy is real: {hasEnemy}");
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            shouldMove = !shouldMove;
        }
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            hasEnemy = !hasEnemy;
            Debug.Log($"Enemy is real: {hasEnemy}");
        }
        body.linearVelocity = new Vector2(shouldMove ? speed : 0f, body.linearVelocity.y);
    }

    // Called when this Rigidbody2D collides with another Collider2D (non-trigger).
    void OnCollisionEnter2D(Collision2D collision)
    {
        var other = collision.gameObject;
        Vector2 contactPoint = Vector2.zero;
        if (collision.contactCount > 0)
            contactPoint = collision.GetContact(0).point;

        if (other.tag == "Finish")
        {
            Debug.Log("Robot reached the level exit!");
            shouldMove = false;
            body.linearVelocity = Vector2.zero;
            MoveToNextLevel(other);
        }

        if (other.tag == "Enemy")
        {
            if (hasEnemy)
            {
                Debug.Log("Robot hit an enemy! Stopping movement.");
                Destroy(this.gameObject); // ???
            }
            else
            {
                Destroy(other.GetComponent<Rigidbody2D>());
                Destroy(other.GetComponent<Collider2D>());
                Debug.Log("Robot passed through the enemy safely.");
            }
        }

        Debug.Log($"Robot collided with '{other.name}' at {contactPoint} - relativeVelocity={collision.relativeVelocity}");
    }

    void MoveToNextLevel(GameObject other)
    {
        Destroy(other);
        Destroy(this.gameObject);
        Debug.Log("Transitioning to the next level...");
    }
}
