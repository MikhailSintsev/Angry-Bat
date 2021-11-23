using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    // загрузить уровень, переданный в качестве параметра метода
    public void LoadScene(string name) => SceneManager.LoadScene("Level" + name);

    // закрыть приложение
    public void ExitApplication() => Application.Quit();
}
