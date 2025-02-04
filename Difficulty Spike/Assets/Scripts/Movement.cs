using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    //Variables
    //-------------------------------------------------------
    [SerializeField] private Rigidbody2D playerRB;
    [SerializeField] private Transform groundCheck;

    [SerializeField] private float jumpSpeed = 1f;
    [SerializeField] private float moveSpeed = 1f;

    private float jumpHoldDuration = 0.0f;
    private float maxJumpHoldTime = 0.1f;

    private int maxDashes = 1;
    private int dashesRemaining = 1;
    private float dashCooldown = 0f;
    private float dashCooldownDuration = 0.5f;

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
            jumpHoldDuration += maxJumpHoldTime;
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
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector3.down, 0.05f);
        grounded = false;
        if (hit) { grounded = hit.collider.gameObject.CompareTag("Ground"); }

        //Reset jump & dash
        if (grounded)
        {
            jumpHoldDuration = 0.0f;
            dashesRemaining = maxDashes;
        }

        //Gravity shit
        if (playerRB.linearVelocity.y >= 0)
        {
            playerRB.gravityScale = 1;
        }
        else
        {
            playerRB.gravityScale = 3;
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
            int dashDirection = 1;
            if (!facingRight) { dashDirection = -1; }
            if(Mathf.Sign(dashDirection) != Mathf.Sign(playerRB.linearVelocityX)) { playerRB.linearVelocityX = 0f; }
            playerRB.AddForce(transform.right * dashDirection * moveSpeed * 0.66f, ForceMode2D.Impulse);
        }

        //Cooldowns
        dashCooldown += Time.deltaTime;
    }
    void FixedUpdate()
    {
        //Jump & held jump
        if (jumpInputDown)
        {
            jumpHoldDuration += Time.deltaTime;
            if(jumpHoldDuration <= 0)
            {
                playerRB.linearVelocityY = 0f;
            }
            if (jumpHoldDuration <= maxJumpHoldTime)
            {
                playerRB.AddForce(transform.up * jumpSpeed, ForceMode2D.Impulse);
            }
        }

        //Movement
        float movementMultiplier = 1f;
        if (!grounded) { movementMultiplier = 0.1f; }  //Movement is reduced if airbourne
        if (Mathf.Sign(inputDirection.x) == Mathf.Sign(playerRB.linearVelocityX) && Mathf.Abs(playerRB.linearVelocityX) >= 10f) { movementMultiplier = 0f; }
        playerRB.AddForce(transform.right * moveSpeed * inputDirection.x * movementMultiplier);

        /*
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
