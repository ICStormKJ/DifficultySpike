using UnityEngine;

public class Paused : MonoBehaviour
{
    [SerializeField]
    GameObject pauseMenu;
    bool paused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)){
            paused = !paused;
            Time.timeScale = 0f;
            pauseMenu.SetActive(paused);
        }
    }

    public bool isPaused(){return paused;}
}
