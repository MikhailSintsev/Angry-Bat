using UnityEngine.SceneManagement;

public static class Scene
{
    public static void ReloadScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    public static void LoadNextScene(string nextLevelName) => SceneManager.LoadScene(nextLevelName);
}
