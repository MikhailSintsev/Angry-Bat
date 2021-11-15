using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Update()
    {
        // перезагрузить сцену при нажатии кнопки 
        if (Input.GetKeyDown(KeyCode.R))
            Scene.ReloadScene(); // статичный метод класса Scene

        // закрыть приложение при нажатии кнопки
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }
}
