using UnityEngine;

public class TriggerableCheckpoint : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.TryGetComponent<PlayerDeathManager>(out PlayerDeathManager pdm)){
            pdm.Checkpoint(transform);
        }
    }
}
