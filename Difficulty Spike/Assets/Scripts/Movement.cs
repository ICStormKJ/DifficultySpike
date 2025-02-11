using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    //Variables
    //-------------------------------------------------------
    [SerializeField] private Rigidbody2D playerRB;
    [SerializeField] private Transform groundCheck;

    [SerializeField] private float jumpSpeed;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float dashSpeed = 15f;

    private float jumpHoldDuration = 0.0f;
    private float maxJumpHoldTime = 0.1f;


    private int maxDashes = 1;      
    private int dashesRemaining = 1;                //To be changed once a player uses a dash
    private float dashCooldown = 0f;                //Timer that counts time since dash usage, to be used for dash cooldown
    private float dashCooldownDuration = 0.6f;      
    private float dashDuration = 0.15f;              //Player is floaty during a dash
    private float dashTimer = 0f;                   //Timer that counts time since dash usage, to be used for behaviors during a player's dash

    private bool grounded = false;

    private Vector2 inputDirection;
    private bool jumpInputDown = false;
    private bool dashInputDown = false;

    private bool facingRight = true;


    //Movement Methods
    //-------------------------------------------------------
    public void movementInput(InputAction.CallbackContext context)
    {
        inputDirection = context.ReadValue<Vector2>();
        inputDirection.y = 0f;
        inputDirection = inputDirection.normalized;
    }

    public void jumpInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            jumpInputDown = true;
        }
        else if (context.canceled)
        {
            jumpInputDown = false;
        }
    }

    public void dashInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            dashInputDown = true;
        }else if (context.canceled)
        {
            dashInputDown = false;
        }
    }


    //Other Methods
    //-------------------------------------------------------


    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
    }


    void Update()
    {
        //Ground detection
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector3.down, 0.005f);
        grounded = false;
        if (hit) { grounded = hit.collider.gameObject.CompareTag("Ground"); }
        Debug.Log(grounded);
        //Reset jump & dash
        if (grounded)
        {
            jumpHoldDuration = 0.0f;
            dashesRemaining = maxDashes;
        }

        //Gravity shit
        if (grounded || playerRB.linearVelocity.y >= 0f || dashTimer < dashDuration)
        {
            playerRB.gravityScale = 1f;
        }
        else
        {
            playerRB.gravityScale = 3f;
        }

        //Player direction
        if(inputDirection.x > 0)
        {
            facingRight = true;
        }else if(inputDirection.x < 0)
        {
            facingRight = false;
        }

        //Dashing
        if (dashInputDown && dashesRemaining > 0 && dashCooldown >= dashCooldownDuration)
        {
            dashInputDown = false;
            dashesRemaining -= 1;
            dashCooldown = 0f;
            int dashDirection = 1;  //1 for facing right, -1 for facing left
            if (!facingRight) { dashDirection = -1; }
            if(Mathf.Sign(dashDirection) != Mathf.Sign(playerRB.linearVelocityX)) { playerRB.linearVelocityX = 0f; }
            dashTimer = 0f;
            if(playerRB.linearVelocityY < 0f) {
                playerRB.linearVelocityY = jumpSpeed;
            }
            playerRB.AddForce(transform.right * dashDirection * dashSpeed, ForceMode2D.Impulse);
        }
        if(!grounded && dashTimer < dashDuration)   //Give player a bit of weightlessness when dashing
        {
           // playerRB.AddForce(Vector2.up * -Physics2D.gravity * 0.5f, ForceMode2D.Force);
        }

        //Cooldowns
        dashCooldown += Time.deltaTime;
        dashTimer += Time.deltaTime;
    }
    void FixedUpdate()
    {
        //Jump & held jump
        if (jumpInputDown)
        {
            bool firstFrameOfJump = jumpHoldDuration == 0f;
            jumpHoldDuration += Time.deltaTime;
            if(jumpHoldDuration <= 0)
            {
                playerRB.linearVelocityY = 0f;
            }
            if (jumpHoldDuration <= maxJumpHoldTime)
            {
                float higherJumpMultiplier = (1f - (jumpHoldDuration / maxJumpHoldTime)) * 0.75f;
                higherJumpMultiplier = Mathf.Sqrt(1f - Mathf.Pow(higherJumpMultiplier - 1f, 2f)); //Higher jump multiplier follows circ-out easing curve
                                                                                                  //Meaning the longer the button is held the less force is applied
                if (firstFrameOfJump)   //More impulse on the first frame of the jump
                {
                    higherJumpMultiplier = 2f;
                }
                playerRB.AddForce(transform.up * jumpSpeed * higherJumpMultiplier, ForceMode2D.Impulse);
            }
        }

        //Movement
        float movementMultiplier = 1f;
        if (!grounded) { movementMultiplier = 0.1f; }  //Movement is reduced if airbourne
        bool movingInDirectionOfSpeed = Mathf.Sign(inputDirection.x) == Mathf.Sign(playerRB.linearVelocityX);
        if (movingInDirectionOfSpeed && Mathf.Abs(playerRB.linearVelocityX) >= 10f) { movementMultiplier = 0f; } 
        playerRB.AddForce(transform.right * moveSpeed * inputDirection.x * movementMultiplier);

        /*s
        if (Input.GetKey(KeyCode.A))
        {
            if (playerRB.linearVelocityX > -10)
            {
                if (grounded == true)
                {
                    transform.rotation = Quaternion.Euler(0, -180, 0);
                    playerRB.AddForce(transform.right * moveSpeed);
                }
                else
                {
                    transform.rotation = Quaternion.Euler(0, -180, 0);
                    playerRB.AddForce(transform.right * moveSpeed * 0.1f);
                }
            };

        }
        if (Input.GetKey(KeyCode.D))
        {
            if (playerRB.linearVelocityX < 10)
            {
                if (grounded == true)
                {
                    playerRB.AddForce(transform.right * moveSpeed);
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                else
                {
                    playerRB.AddForce(transform.right * moveSpeed * 0.1f);
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                }
            };
        }
        */

    }
}
