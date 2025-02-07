using UnityEngine;

public class PlayerDeathManager : MonoBehaviour
{
    public GameObject[] deathMechanicObjects;
    void OnCollisionEnter2D(Collision2D other){
        if(other.gameObject.tag == "Spike"){
            GameObject obj = deathMechanicObjects[Random.Range(0, deathMechanicObjects.Length)];
            DeathMechanic dm;
            if (obj.TryGetComponent<DeathMechanic>(out dm)){
                dm.Run();
            }
        }
    }
}


