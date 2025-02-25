using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    public void Quit(){
        Application.Quit();
    }

    public void LoadLevel(string name){
        SceneManager.LoadScene(name);
    }
}
