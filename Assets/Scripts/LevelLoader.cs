using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public void LoadScene(string name) => SceneManager.LoadScene("Level" + name);

    public void ExitApplication() => Application.Quit();
}
