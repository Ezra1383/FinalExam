using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayDriving()
    {
        SceneManager.LoadScene("Prototype 1");
    }

    public void PlayFlying()
    {
        SceneManager.LoadScene("Challenge 1");
    }

    public void PlaySumo()
    {
        SceneManager.LoadScene("Prototype 4");
    }

    public void ExitGame()
    {
        Debug.Log("Exit pressed");   // Application.Quit does nothing in the Editor, so this proves it works
        Application.Quit();
    }
}
