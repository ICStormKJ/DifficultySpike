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
    [Tooltip("Unfortunately i had to make another ")]
    public UnityEvent<float, float> deathMechanic2Param;

    public void Move(float xdis, float ydis){
        transform.position = new Vector2(transform.position.x + xdis, transform.position.y + ydis);
    }

    public void ToggleAppear(){
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void Fall(){
        Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
        if (rb == null){return;}
        rb.gravityScale = 1;
    }
    
}
