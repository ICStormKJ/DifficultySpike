using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class DeathMechanic : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Used for toggleappear and fall, which have 0 arguments")]
    public UnityEvent deathMechanic;
    [SerializeField]
    [Tooltip("Unfortunately i had to make another event for move specifically")]
    public UnityEvent<float, float> deathMechanic2Param;

    void FixedUpdate(){
        if(Input.GetKeyDown(KeyCode.Space)){
            Run(5.0f, 5.0f);
        }
    }
    void Run(float xdis = 0, float ydis = 0){
        Debug.Log("Hi");
        if (deathMechanic != null){
            deathMechanic.Invoke();
        }
        else if (deathMechanic2Param != null){
            deathMechanic2Param.Invoke(xdis, ydis);
        }
    }
    public void Move(float xdis, float ydis){
        transform.position = new Vector2(transform.position.x + xdis, transform.position.y + ydis);
    }

//Toggles the object to appear or disappear. 
    public void ToggleAppear(){
        gameObject.SetActive(!gameObject.activeSelf);
    }

//Toggles gravity for this object.
    public void Fall(){
        Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
        if (rb == null){return;}
        rb.gravityScale = 1;
        transform.rotation = Quaternion.Euler(0, -180, 0);
    }
    
}
