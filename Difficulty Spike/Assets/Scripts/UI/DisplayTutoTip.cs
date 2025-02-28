using UnityEngine;

public class DisplayTutoTip : MonoBehaviour
{
    [SerializeField]
    GameObject player;
    [SerializeField, Tooltip("How close you must be to display tip")]
    float range;
    //Note: You can put a single empty game object as the child, and put multiple speech bubbles under that.
    GameObject speechBubble;

    void Start()
    {
        speechBubble = transform.GetChild(0).gameObject;
    }
    void FixedUpdate()
    {
        if(Vector2.Distance(transform.position, player.transform.position) <= range){
            speechBubble.SetActive(true);
        }else{
            speechBubble.SetActive(false);
        }
    }
}
