using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
  
    public void StartGame()
    {
        SceneManager.LoadScene("DemoScene");
    }
    
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit game");
    }
}
