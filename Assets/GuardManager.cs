using UnityEngine;
using UnityEngine.SceneManagement;

public class GuardManager : MonoBehaviour
{
    void Update()
    {
        if (Input.anyKeyDown)
        {
            SceneManager.LoadScene("DirectionScene"); //change this to next scene
        }
    }
}