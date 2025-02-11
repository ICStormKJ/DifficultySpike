using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class DeathMechanic : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Used for toggleappear and fall, which have 0 arguments")]
    UnityEvent deathMechanic;
    [SerializeField]
    [Tooltip("Unfortunately i had to make another event for move specifically")]
    UnityEvent<float, float> deathMechanic2Param;

    //Custom setting move death mechanic dependent on object
    [SerializeField]
    float moveHorizontal, moveVertical;

    // void Update(){
    //     if(Input.GetKeyDown(KeyCode.Space)){
    //         Run(0.5f, 0.5f);
    //     }
    // }

//General function to run when activating a death mechanic, by player death or by item.
    public void Run(){
        if (deathMechanic.GetPersistentEventCount() != 0){
            deathMechanic.Invoke();
        }
        else if (deathMechanic2Param.GetPersistentEventCount() != 0){
            deathMechanic2Param.Invoke(moveHorizontal, moveVertical);
        }
    }
    //Moves object by (xdis, ydis)
    public void Move(float xdis, float ydis){
        transform.position = new Vector2(transform.position.x + xdis, transform.position.y + ydis);
    }

//NOTE: TryGetComponent is a version of GetComponent that stores 
//a component into the variable in the parameter, and returns a bool on if it was successful

//Toggles the object to appear or disappear. 
    public void ToggleAppear(){
        if (gameObject.TryGetComponent<SpriteRenderer>(out SpriteRenderer r)){
            r.enabled = !r.enabled;
        }
    }

//Makes object affected by gravity
    public void Fall(){
        if (gameObject.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb)){
            rb.gravityScale = 1;
            transform.rotation = Quaternion.Euler(0, -180, 0);
        }
    }
    
}
