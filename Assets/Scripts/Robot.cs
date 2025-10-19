using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Robot : MonoBehaviour
{
    GameManager gameManager;
    GravityManager gravityManager;

    private Rigidbody2D body;
    private bool shouldMove = false; // When true the robot moves right at 'speed'
    private bool hasEnemy = true;

    [SerializeField] private float speed = 50f;
    [SerializeField] private int currentInk = 10; // probably not gonna be a field for the robot, but we're lazy
    [SerializeField] private int inkPowerup = 5; // probably not gonna be a field for the robot, but we're lazy


    [SerializeField]
    private CircleCollider2D groundCheckCollider;
    private bool isGrounded = false;

    public bool IsGrounded => isGrounded;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log($"Enemy is real: {hasEnemy}");
        Debug.Log($"Current ink is now {currentInk}");


        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        gameManager.PauseRobotToggled += OnPauseRobotToggled;

        gravityManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GravityManager>();
    }

    private void OnDestroy()
    {
        gameManager.PauseRobotToggled -= OnPauseRobotToggled;
    }

    void OnPauseRobotToggled()
    {
        shouldMove = !shouldMove;
        Debug.Log($"Robot movement toggled: {shouldMove}");
    }

    // Update is called once per frame
    void Update()
    {
        if(Keyboard.current.rKey.wasPressedThisFrame)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            hasEnemy = !hasEnemy;
            Debug.Log($"Enemy is real: {hasEnemy}");
        }        
    }

    private void FixedUpdate()
    {
        if (!shouldMove) return;
        
        if(groundCheckCollider != null)
        {
            isGrounded = groundCheckCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
            if (!isGrounded)
            {
                Debug.Log("Robot is not grounded, skipping movement this frame.");
                return;
            }
        }

        float moveSpeed = speed * Time.deltaTime;
        Vector2 direction = transform.right;
        body.AddForce(direction * moveSpeed, ForceMode2D.Force);
        
    }

    // Called when this Rigidbody2D collides with another Collider2D (non-trigger).
    void OnCollisionEnter2D(Collision2D collision)
    {
        var other = collision.gameObject;
        Vector2 contactPoint = Vector2.zero;
        if (collision.contactCount > 0)
            contactPoint = collision.GetContact(0).point;

        if (other.tag == "InkPowerup")
        {
            CollideWithInkPowerup(other);
        }

        if (other.tag == "Enemy")
        {
            CollideWithEnemy(other);
        }

        Debug.Log($"Robot collided with '{other.name}' at {contactPoint} - relativeVelocity={collision.relativeVelocity}");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var other = collision.gameObject;
        if (other.tag == "Ink")
        {
            CollideWithInk(other);
        }

        if (other.tag == "Finish")
        {
            MoveToNextLevel(other);
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var other = collision.gameObject;
        if (other.tag == "Ink")
        {
            ExitInkCollision(other);
        }
    }

    void MoveToNextLevel(GameObject other)
    {
        // stop moving
        Debug.Log("Robot reached the level exit!");

        gameManager.WinLevel();

        Debug.Log("Transitioning to the next level...");
    }

    void CollideWithEnemy(GameObject enemy)
    {
        if (hasEnemy)
        {
            Debug.Log("Robot hit an enemy! Stopping movement.");
            gameManager.FailLevel();
            GameObject.Destroy(this.gameObject);
        }
        else
        {
            Destroy(enemy.GetComponent<Rigidbody2D>());
            Destroy(enemy.GetComponent<Collider2D>());
            Debug.Log("Robot passed through the enemy safely.");
        }
    }

    void CollideWithInkPowerup(GameObject powerup)
    {
        currentInk += inkPowerup;
        Debug.Log($"Robot collected {powerup.name}, current ink is now {currentInk}");
        Destroy(powerup);
    }

    void CollideWithInk(GameObject ink)
    {
        LineScript lineScript = ink.GetComponent<LineScript>();

        gravityManager.AddModifier(lineScript.GetModifier());        

        Debug.Log($"Robot collided with ink: {ink.name}");
    }

    void ExitInkCollision(GameObject ink)
    {
        LineScript lineScript = ink.GetComponent<LineScript>();

        gravityManager.RemoveModifier(lineScript.GetModifier());
        lineScript.RemoveGravityModifier();

        Debug.Log("Robot exited ink collision.");
    }
}
