using UnityEngine;

public class TriggerableCheckpoint : MonoBehaviour
{
    private SpriteRenderer sprite;
    void OnTriggerEnter2D(Collider2D other){
        if (other.TryGetComponent<PlayerDeathManager>(out PlayerDeathManager pdm)){
            pdm.Checkpoint(transform);
            Color reed = Color.red;
            reed.a = 0.38f;
            sprite.color = reed;
        }
    }

    void Start(){
        sprite = GetComponent<SpriteRenderer>();
    }
}
