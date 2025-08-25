using UnityEngine;

public class StartScreenController : MonoBehaviour
{
    public void Awake()
    { 
        Cursor.lockState = CursorLockMode.None;
    }

    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
    }
    
    public void ExitGame()
    {
        Application.Quit();
    }
}
