using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Robot : MonoBehaviour
{
    GameManager gameManager;

    private Rigidbody2D body;
    private bool shouldMove = false; // When true the robot moves right at 'speed'
    private bool hasEnemy = true;

    [SerializeField] private float speed = 50f;
    [SerializeField] private int currentInk = 10; // probably not gonna be a field for the robot, but we're lazy
    [SerializeField] private int inkPowerup = 5; // probably not gonna be a field for the robot, but we're lazy


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

        gameManager.OnRobotPausedToggled += () =>
        {
            shouldMove = !shouldMove;
            Debug.Log($"Robot movement toggled: {shouldMove}");
        };
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

        body.AddForce(new Vector2(shouldMove ? speed * Time.deltaTime : 0f, 0f), ForceMode2D.Force);

        // body.linearVelocity = new Vector2(shouldMove ? speed : 0f, body.linearVelocity.y);
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
            MoveToNextLevel(other);
        }

        if (other.tag == "Enemy")
        {
            CollideWithEnemy(other);
        }

        if (other.tag == "InkPowerup")
        {
            CollideWithInkPowerup(other);
        }

        

        Debug.Log($"Robot collided with '{other.name}' at {contactPoint} - relativeVelocity={collision.relativeVelocity}");
    }    

    private void OnTriggerStay2D(Collider2D collision)
    {
        var other = collision.gameObject;

        if(other.tag == "Ink")
        {
            body.gravityScale -= 0.3f * Time.deltaTime;
        }
    }

    void MoveToNextLevel(GameObject other)
    {
        // stop moving
        Debug.Log("Robot reached the level exit!");
        //shouldMove = false;
        //body.linearVelocity = Vector2.zero;
        // destroy the elements
        //Destroy(other);
        //Destroy(this.gameObject);
        Debug.Log("Transitioning to the next level...");
    }

    void CollideWithEnemy(GameObject enemy)
    {
        if (hasEnemy)
        {
            Debug.Log("Robot hit an enemy! Stopping movement.");
            //Destroy(this.gameObject); // ???
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
        // Placeholder for ink collision logic
        Debug.Log($"Robot collided with ink: {ink.name}");
    }
}
