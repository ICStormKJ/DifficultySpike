using System;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    //---------------GENERAL STUFF---------------
    [SerializeField] private Rigidbody2D playerRB;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    //---------------PHYSICS---------------
    [SerializeField] private PhysicsMaterial2D playerDefaultFriction;
    [SerializeField] private PhysicsMaterial2D playerLessFriction;
    [SerializeField] private PhysicsMaterial2D groundDefaultFriction;
    [SerializeField] private PhysicsMaterial2D groundHeavyFriction;

    //---------------MOVEMENT---------------
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float movementAcceleration;
    [SerializeField] private float moveSpeedSoftCap;
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dgrav = 1f;
    private float dgravCopy;
    [SerializeField] private float maxFallSpeed = 30f;

    //-------------ANIMATION----------------
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Animator playerAnimator;

    //---------------JUMP AND DASHING---------------
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
        dgravCopy = dgrav;
        //gm = FindAnyObjectByType<GlobalManager>();
    }


    void Update()
    {
        //Ground detection
        RaycastHit2D[] hit = Physics2D.BoxCastAll(groundCheck.position, new Vector2(1f, 0.02f), 0f, Vector2.up, 1f, groundLayer);
        groundNormal = Vector3.up;
        grounded = false;
        PhysicsMaterial2D playerMaterial = playerDefaultFriction;
        
        //logic to fix wall cling
        bool horizontalNormal = false;
        foreach(RaycastHit2D hits in hit){
            if (Mathf.Abs(hits.normal.x) == 1){
                horizontalNormal = true;
            }
            if (hits.normal.x == 0){
                horizontalNormal = false;
                break;
            }
        }

        if (hit.Length > 0 && !horizontalNormal)
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

        //Animation
        if(inputDirection.x == 0 && playerRB.linearVelocityX == 0 && playerRB.linearVelocityY == 0){
            playerAnimator.Play("Idle");
        }
        else{
            if(playerRB.linearVelocityY != 0){
                if(playerRB.linearVelocityY > 0){
                    //jumping animation
                    if(facingRight){
                        sprite.flipX = false;
                        playerAnimator.Play("Jump");
                    }
                    else{
                        //left facing jump
                        sprite.flipX = true;
                        playerAnimator.Play("Jump");
                    }
                }
                else{
                    //falling down animation
                    if(facingRight){
                        //right facing fall
                        sprite.flipX = false;
                        playerAnimator.Play("Fall");
                    }
                    else{   
                        //left facing fall
                        sprite.flipX = true;
                        playerAnimator.Play("Fall");
                    }
                }
            }
            else{
                if(inputDirection.x > -0.05 && inputDirection.x < 0.05){
                    //skidding animation
                    if(facingRight){
                        //right-facing skidding animation
                        sprite.flipX = false;
                        playerAnimator.Play("Skid");
                    }
                    else{
                        //left facing skidding animation
                        sprite.flipX = true;
                        playerAnimator.Play("Skid");
                    }
                }
                else{
                    if(facingRight){
                        sprite.flipX = false;
                        playerAnimator.Play("WalkRight");
                    }
                    else{
                        sprite.flipX = true;
                        playerAnimator.Play("WalkLeft");
                    }
                }
            }
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
        //NOTE FROM KAI: I am experimenting with the gravity gradually increasing instead of spiking for smoother falling.
        if (grounded || playerRB.linearVelocity.y >= 0f || dashTimer < dashDuration)
        {
            playerRB.gravityScale = 1f;
        }
        else if(playerRB.gravityScale < 3)
        {
            playerRB.gravityScale += Time.deltaTime * dgrav;
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
            //Play dash animation
            if(dashDirection == 1){
                //right dash animation
            }
            else{
                //left dash animation
            }
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
            //Debug.Log(jumpHoldDuration);
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
        moveSpeedCap *= 1f - (1f - xDirection) * (1f - xDirection);
        //Limit force applied to control speed
        bool movingInDirectionOfSpeed = Mathf.Sign(inputDirection.x) == Mathf.Sign(playerRB.linearVelocityX);
        if (movingInDirectionOfSpeed && Mathf.Abs(playerRB.linearVelocityX) >= moveSpeedCap) { movementMultiplier = 0f; }

        //Apply movement force
        playerRB.AddForce(moveDirection * movementAcceleration * Mathf.Abs(inputDirection.x) * movementMultiplier);

        

        if(playerRB.linearVelocityY <= maxFallSpeed){
            dgrav = 0;
        }
        if(grounded){
            dgrav = dgravCopy;
        }

    }
}
