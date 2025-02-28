using UnityEngine;

public class TriggerableCheckpoint : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("triggered");
        if (other.TryGetComponent<PlayerDeathManager>(out PlayerDeathManager pdm)){
            pdm.Checkpoint(transform);
        }
    }
}
