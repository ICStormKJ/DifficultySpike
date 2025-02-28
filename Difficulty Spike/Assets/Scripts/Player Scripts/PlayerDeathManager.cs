using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeathManager : MonoBehaviour
{
    [SerializeField]
    List<DeathMechanic> deathMechanicObjects;
    [SerializeField]
    Transform spawnpoint;
    [SerializeField] 
    string nextSceneName;
    [SerializeField]
    GlobalManager globalStats;
    
    //update a spawnpoint
    public void Checkpoint(Transform newpos){
        spawnpoint = newpos;
    }

    //respawn the player, trigger a random death mechanic and remove it from the list to not be chosen again.
    void Death(){
        transform.position = spawnpoint.position;
        if (deathMechanicObjects.Count == 0){return;}//prune when no (more) objects to trigger

        int index = Random.Range(0, deathMechanicObjects.Count);
        DeathMechanic obj = deathMechanicObjects[index];
        obj.Run();
        deathMechanicObjects.RemoveAt(index);
    }

    //add modifier, load next scene
    void Win(){
       // globalStats.AddModifier(0.05f, 0.05f);
        Debug.Log("Win");
        SceneManager.LoadScene(nextSceneName);
    }

    //Behaviour when colliding with either a spike or a winning item.
    void OnCollisionEnter2D(Collision2D other){
        if(other.gameObject.tag == "Spike"){
            Death();
        }
        else if (other.gameObject.tag == "Win"){
            Win();
        }
    }
}


