using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeathManager : MonoBehaviour
{
    [SerializeField]
    DeathMechanic[] deathMechanicObjects;
    [SerializeField]
    Transform spawnpoint;
    [SerializeField] 
    string nextSceneName;
    [SerializeField]
    GlobalManager globalStats;
    
    void Death(){
        transform.position = spawnpoint.position;
        DeathMechanic obj = deathMechanicObjects[Random.Range(0, deathMechanicObjects.Length)];
        obj.Run();
    }

    void Win(){
       // globalStats.AddModifier(0.05f, 0.05f);
        Debug.Log("Win");
        //SceneManager.LoadScene(nextSceneName);
    }

    //Player dying
    void OnCollisionEnter2D(Collision2D other){
        if(other.gameObject.tag == "Spike"){
            Death();
        }
        else if (other.gameObject.tag == "Win"){
            Win();
        }
    }
}


