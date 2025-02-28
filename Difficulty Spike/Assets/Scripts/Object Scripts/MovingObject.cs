using UnityEngine;

public class MovingObject : MonoBehaviour
{
    [SerializeField]
    Vector2 startpos;
    [SerializeField]
    Vector2 endpos;
    [SerializeField]
    float speedMod;
    float t = 0.0f;

    void FixedUpdate()
    {
        if (t >= 1.0f){
            Vector2 temp = startpos;
            startpos = endpos;
            endpos = temp;
            t = 0.0f;
        }
        transform.position = Vector2.Lerp(startpos, endpos, t);
        t += Time.deltaTime * speedMod;
    }
}
