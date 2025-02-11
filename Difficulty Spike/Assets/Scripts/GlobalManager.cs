using UnityEngine;

public class GlobalManager : MonoBehaviour
{
    [SerializeField]
    float moveSpeedModifier, jumpHeightModifier = 1.0f;
    void Awake(){
        DontDestroyOnLoad(gameObject);
    }

    public void AddModifier(float addSpeed, float addJump){
        moveSpeedModifier += addSpeed;
        jumpHeightModifier += addJump;
    }

    public float GetMoveSpeedMod() { return moveSpeedModifier; }
    public float GetJumpHeightMod() { return jumpHeightModifier; }

}
