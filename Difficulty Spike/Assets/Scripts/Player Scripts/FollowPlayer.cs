using UnityEngine;

public class FollowPlayer : MonoBehaviour
{

    public Transform player; // Reference to the player object
    public Vector3 offset; // Offset from the player's position
    public float smoothSpeed = 0.125f; // Smoothness of the camera movement


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    void LateUpdate()
    {
        Vector3 targetPosition = new Vector3(player.position.x + offset.x, player.position.y + offset.y, offset.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
    }
}
