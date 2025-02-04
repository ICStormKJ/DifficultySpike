using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class Movement : MonoBehaviour
{
    Rigidbody2D player;
    public float jumpSpeed = 1f;
    public float moveSpeed = 1f;
    float jumpTimer = 0.0f;
    int dashCooldown = 1;

    Boolean isGround = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       player = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame

    void Update(){
        if(Input.GetKeyUp(KeyCode.Space)){
            jumpTimer = 1.0f;
            Debug.Log(jumpTimer + " GetKeyUp");
        }
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.down, 0.4f);

        if(hit){
            if(hit.collider.gameObject.CompareTag("Ground")){
                isGround = true;
            }
        }
        else{
            isGround = false;
        }

        if(isGround){
            jumpTimer = 0.0f;
            dashCooldown = 1;
        }

        if (player.linearVelocity.y >= 0){
            player.gravityScale = 1;
        }
        else if (player.linearVelocity.y < 0){
            player.gravityScale = 3;
        }
    }
    void FixedUpdate()
    {
        if(Input.GetKey(KeyCode.Space)){
            jumpTimer += Time.deltaTime;
            if(jumpTimer <= 0.1f){
                Debug.Log(jumpTimer + " Normal");
                player.AddForce(transform.up*jumpSpeed, ForceMode2D.Impulse);
            }
        }
        if(Input.GetKey(KeyCode.A)){
            if(player.linearVelocityX > -10){
                if(isGround == true){
                    transform.rotation = Quaternion.Euler(0,-180,0);
                    player.AddForce(transform.right*moveSpeed);
                }
                else{
                    transform.rotation = Quaternion.Euler(0,-180,0);
                    player.AddForce(transform.right*moveSpeed*0.1f);
                }
            };
            
        }
        if(Input.GetKey(KeyCode.D)){
            if(player.linearVelocityX < 10){
                if(isGround == true){
                    player.AddForce(transform.right*moveSpeed);
                    transform.rotation = Quaternion.Euler(0,0,0);
                }
                else{
                    player.AddForce(transform.right*moveSpeed*0.1f);
                    transform.rotation = Quaternion.Euler(0,0,0);
                }
            };
        }

        if(Input.GetKeyDown(KeyCode.LeftShift)){
            if(dashCooldown > 0){
                player.AddForce(transform.right*moveSpeed*0.66f, ForceMode2D.Impulse);
                dashCooldown--;
            }
        }
        
    }
}
