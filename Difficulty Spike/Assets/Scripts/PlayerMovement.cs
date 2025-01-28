using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    //References
    [Header("References")]
    [SerializeField] private Transform groundCheck;
    private Rigidbody2D characterRB;

    //Variable Variables
    private float jumpPower = 2f;

    private float coyoteTimerDefault = 0.1f;
    private float coyoteTimer;

    //Backend Variables
    private Vector2 inputDirection;
    private bool jumpInputDown = false;



    //=================================================


    //Movement Methods
    public void movementInput(InputAction.CallbackContext context)
    {
        inputDirection = context.ReadValue<Vector2>();
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


    private bool checkGrounded()
    {
        Collider2D obj = Physics2D.OverlapCircle(groundCheck.position, 0.05f);
        if (!obj) { return false; }
        return obj.gameObject.tag == "Ground";
    }

    //===================================================


    private void Start()
    {
        characterRB = GetComponent<Rigidbody2D>();
        coyoteTimer = coyoteTimerDefault;
    }

    private void Update()
    {
        bool grounded = checkGrounded();
        Debug.Log(grounded);

        //Coyote Time
        if (!grounded)
        {
            coyoteTimer -= Time.deltaTime;
        }
        else
        {
            coyoteTimer = coyoteTimerDefault;
        }

        //Jumping
        if (jumpInputDown)
        {
            if(!grounded && coyoteTimer < 0f) { return; }
            characterRB.AddForce(new Vector2(0f, jumpPower), ForceMode2D.Impulse);
            coyoteTimer = 0f;
        }
    }
}
