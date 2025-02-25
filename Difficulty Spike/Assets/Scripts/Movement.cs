using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    //Variables
    //-------------------------------------------------------
    [SerializeField] private Rigidbody2D playerRB;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private PhysicsMaterial2D playerDefaultFriction;
    [SerializeField] private PhysicsMaterial2D playerLessFriction;
    [SerializeField] private PhysicsMaterial2D groundDefaultFriction;
    [SerializeField] private PhysicsMaterial2D groundHeavyFriction;

    [SerializeField] private float jumpSpeed;
    [SerializeField] private float movementAcceleration;
    [SerializeField] private float moveSpeedSoftCap;
    [SerializeField] private float dashSpeed = 15f;

    private float jumpHoldDuration = 0.0f;    
    private float maxJumpHoldTime = 0.2f;           //How long in seconds it takes for a jump to reach its apex

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
    private bool firstFrameOfJump = false;

    private bool facingRight = true;
    private Vector3 groundNormal;


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
        //gm = FindAnyObjectByType<GlobalManager>();
    }


    void Update()
    {
        //Ground detection
        RaycastHit2D[] hit = Physics2D.BoxCastAll(groundCheck.position, new Vector2(1f, 0.02f), 0f, Vector2.up, 1f, groundLayer);
        groundNormal = Vector3.up;
        grounded = false;
        PhysicsMaterial2D playerMaterial = playerDefaultFriction;
        if (hit.Length > 0)
        {
            grounded = true;
            //Set the player's ground normal depending on whether they're on/off a slope or boarding a slope
            RaycastHit2D slopeHit = default;
            RaycastHit2D flatHit = default;
            bool slopeHitFound = false;
            bool flatHitFound = false;
            foreach(RaycastHit2D raycastHit in hit)
            {
                if (raycastHit.normal == Vector2.zero) { continue; }
                if (raycastHit.normal == Vector2.up) {
                    flatHit = raycastHit;
                    flatHitFound = true;
                } else { 
                    slopeHit = raycastHit;
                    slopeHitFound = true;
                }
            }
            bool onSlopeCorner = slopeHitFound && flatHitFound;
            //Debug.Log(flatHit.normal.ToString() + " / " + slopeHit.normal.ToString() + " / " + onSlopeCorner.ToString());
            if (!onSlopeCorner)
            {
                if (flatHitFound)
                {
                    groundNormal = flatHit.normal;
                }
                else
                {
                    groundNormal = slopeHit.normal;
                }
            }
            else
            {
                playerMaterial = playerLessFriction;
                if(Mathf.Sign(slopeHit.normal.x) == Mathf.Sign(inputDirection.x)) //Dismounting slope
                {
                    groundNormal = flatHit.normal;
                }
                else //Boarding slope
                {
                    groundNormal = slopeHit.normal;
                }
            }
            //Prevent player from sliding down slopes when not moving
            bool stoppedOnSlope = (slopeHitFound && inputDirection.x == 0f);
            PhysicsMaterial2D groundMaterial = groundDefaultFriction;
            if (stoppedOnSlope)
            {
                groundMaterial = groundHeavyFriction;
            }
            foreach (RaycastHit2D raycastHit in hit)
            {
                raycastHit.collider.sharedMaterial = groundMaterial;
            }
        }

        //Reduce player friction at times to add more fluid movement
        if (!grounded)
        {
            playerMaterial = playerLessFriction;
        }
        playerRB.sharedMaterial = playerMaterial;

        //Player direction
        if (inputDirection.x > 0)
        {
            facingRight = true;
        }
        else if (inputDirection.x < 0)
        {
            facingRight = false;
        }

        //Reset jump & dash
        bool jumping = jumpInputDown && jumpHoldDuration < maxJumpHoldTime && playerRB.linearVelocityY >= 0f;
        if (grounded && !jumping)
        {
            jumpHoldDuration = 0.0f;
            dashesRemaining = maxDashes;
            firstFrameOfJump = true;
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
        //JUMP
        ///////////////////////////////////////////////////
        if (jumpInputDown)
        {
            Debug.Log(jumpHoldDuration);
            jumpHoldDuration = Mathf.Min(jumpHoldDuration + Time.deltaTime, maxJumpHoldTime);
            if (jumpHoldDuration <= maxJumpHoldTime && (grounded || playerRB.linearVelocityY >= 0f))
            {
                float higherJumpMultiplier = (1f - (jumpHoldDuration / maxJumpHoldTime)) * 0.75f;
                higherJumpMultiplier = Mathf.Sqrt(1f - Mathf.Pow(higherJumpMultiplier - 1f, 2f)); //Higher jump multiplier follows circ-out easing curve
                                                                                                  //Meaning the longer the button is held the less force is applied
                if (firstFrameOfJump)   //More impulse on the first frame of the jump
                {
                    playerRB.linearVelocityY = 0f;
                    higherJumpMultiplier = 2f;
                }
                playerRB.AddForce(transform.up * jumpSpeed * higherJumpMultiplier, ForceMode2D.Impulse);
                //add gm.GetJumpHeightMod() later
            }
            firstFrameOfJump = false;
        }

        //MOVEMENT
        ////////////////////////////////////////////////////
        float movementMultiplier = 1f;

        //Apply force at angle if there is a slope
        Vector2 moveDirection;
        if (grounded)
        {
            float groundNormalAngle = Mathf.Atan2(groundNormal.y, groundNormal.x);
            float movementAngle = groundNormalAngle + ((Mathf.PI / 2f) * (facingRight ? -1f : 1f));
            moveDirection = new Vector2(Mathf.Cos(movementAngle), Mathf.Sin(movementAngle));
        }
        else
        {
            moveDirection = inputDirection;
        }

        //Reduce control of movement in air
        if (!grounded) { movementMultiplier = 0.1f; }
        //Reduce max speed when on slopes
        float moveSpeedCap = moveSpeedSoftCap;
        float xDirection = Mathf.Abs(moveDirection.x);
        moveSpeedCap *= (1f - (1f - xDirection) * (1f - xDirection));
        //Limit force applied to control speed
        bool movingInDirectionOfSpeed = Mathf.Sign(inputDirection.x) == Mathf.Sign(playerRB.linearVelocityX);
        if (movingInDirectionOfSpeed && Mathf.Abs(playerRB.linearVelocityX) >= moveSpeedCap) { movementMultiplier = 0f; }

        //Apply movement force
        playerRB.AddForce(moveDirection * movementAcceleration * Mathf.Abs(inputDirection.x) * movementMultiplier);

        //Debug.Log("Ground: " + Mathf.Atan2(groundNormal.y, groundNormal.x) * Mathf.Rad2Deg + " / Movement: " + movementAngle * Mathf.Rad2Deg);

    }
}
