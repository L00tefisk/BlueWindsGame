using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour
{
    [HideInInspector]
    public bool
        facingRight = true;         // For determining which way the player is currently facing.
    [HideInInspector]
    public bool
        jump = false;               // Condition for whether the player should jump.
    

    [HideInInspector]
    public float
        moveForce = 150f;       // Amount of force added to move the player left and right.
    [HideInInspector]
    public float
        runSpeed = 10f;     // The fastest the player can travel in the x axis.
    public float maxSpeed;
    public float jumpForce = 3 * 200f;          // Amount of force added when the player jumps.

    private float gravScale;
    private Transform groundCheck;          // A position marking where to check if the player is grounded.
    private Transform wallCheck;
    private bool grounded = false;  // Whether or not the player is grounded.
    private bool walled = false;
    private bool falling = false;
    private Animator anim;

    void Awake ()
    {
        // Setting up references.
        gravScale = rigidbody2D.gravityScale;
        groundCheck = transform.Find ("groundCheck");
        wallCheck = transform.Find ("wallCheck");
        anim = GetComponent<Animator> ();
    }

    void Update ()
    {
        // The player is grounded if a linecast to the groundcheck position hits anything on the ground layer.
        grounded = Physics2D.Linecast (transform.position, groundCheck.position, 1 << LayerMask.NameToLayer ("Ground"));  
        walled = Physics2D.Linecast (transform.position, wallCheck.position, 1 << LayerMask.NameToLayer ("Walls")); 

        anim.SetBool ("Walled", walled);

        anim.SetBool ("Grounded", grounded);
        anim.SetBool ("Falling", falling);

        // If the jump button is pressed and the player is grounded then the player should jump.
        if (Input.GetButtonDown ("Jump") && (grounded || walled)) {
            jump = true;
            falling = false;
        }
        
        if (Input.GetButton ("Sprint") && grounded) {
            maxSpeed = runSpeed * 1.5f;
        } else if (grounded) {
            maxSpeed = runSpeed;
        }
    }

    void FixedUpdate ()
    {
        //If the player is stuck to the wall and falling
        if (walled && falling) {
            //Reduce the gravity by 200%
            rigidbody2D.gravityScale = gravScale / 4;
        } else {
            //Return it to normal
            rigidbody2D.gravityScale = gravScale;
        }
        // Cache the horizontal input.
        float h = Input.GetAxis ("Horizontal");

        // The Speed animator parameter is set to the absolute value of the horizontal input.
        anim.SetFloat ("Speed", Mathf.Abs (h * 2));

        //If the player is changing direction (h has a different sign to velocity.x) or hasn't reached maxSpeed yet...
        if (h * rigidbody2D.velocity.x < maxSpeed && (Mathf.Abs (rigidbody2D.velocity.x) < maxSpeed)) {
            // ... add a force to the player.
            if (grounded) {
                rigidbody2D.AddForce (Vector2.right * h * moveForce);
            } else { 
                rigidbody2D.AddForce ((Vector2.right * h * moveForce) / 5);
            }
        }

        // If the player's horizontal velocity is greater than the maxSpeed...
        if (Mathf.Abs (rigidbody2D.velocity.x) > maxSpeed) {
            // ... set the player's velocity to the maxSpeed in the x axis.
            rigidbody2D.velocity = new Vector2 (Mathf.Sign (rigidbody2D.velocity.x) * maxSpeed, rigidbody2D.velocity.y);
        }

        //TODO 
        else if (h == 0 && grounded) {
            rigidbody2D.velocity = new Vector2 (rigidbody2D.velocity.x * 0.90f, rigidbody2D.velocity.y);
        } else if (h == 0) {
            rigidbody2D.velocity = new Vector2 (rigidbody2D.velocity.x * 0.985f, rigidbody2D.velocity.y);
        }
        
        if (grounded) {
            // If the input is moving the player right and the player is facing left...
            if (h > 0 && !facingRight) {
                // ... flip the player.
                Flip ();
                // Otherwise if the input is moving the player left and the player is facing right...
            } else if (h < 0 && facingRight) {
                // ... flip the player.
                Flip ();
            }
        } else {
            if (h > 0 && rigidbody2D.velocity.x > 1 && !facingRight) {
                Flip ();
                Debug.Log("flip flap "+Mathf.Round(rigidbody2D.velocity.x));
                // Otherwise if the input is moving the player left and the player is facing right...
            } else if (h < 0 && rigidbody2D.velocity.x < -1 && facingRight) {
                Debug.Log("flip flap "+Mathf.Round(rigidbody2D.velocity.x));
                   // ... flip the player.
                Flip ();
            }
        }

        // If the player should jump...
        if (jump) {
            //While on the ground:
            if (grounded) {
                // Set the Jump animator trigger parameter.
                anim.SetTrigger ("Jump");

                // Add a vertical force to the player.
                rigidbody2D.AddForce (new Vector2 (0f, jumpForce));

                // Make sure the player can't jump again until the jump conditions from Update are satisfied.
                jump = false;
            //While in the air and attached to a wall
            } else if (walled) {
                //Disable sprinting in air
                maxSpeed = runSpeed;
                //Reset the speed before the jump
                rigidbody2D.velocity = new Vector2 (0, 0);
                anim.SetTrigger ("Jump");
                if (facingRight) {
                    rigidbody2D.AddForce (new Vector2 (-jumpForce * 2.5f, jumpForce / 1.2f));
                } else {
                    rigidbody2D.AddForce (new Vector2 (jumpForce * 2.5f, jumpForce / 1.2f));
                }
                Flip ();
                jump = false;
            }

        } else if (rigidbody2D.velocity.y < -1) {
            falling = true;
        } else if (grounded) {
            falling = false;
        }

    }
    
    void Flip ()
    {
        // Switch the way the player is labelled as facing.
        facingRight = !facingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
