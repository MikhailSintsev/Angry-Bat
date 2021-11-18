using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int currentLevelIndex;
    public Animator overlayAnimator;
    public Bat batScript;
    public TMP_Text scoreText;

    private Monster[] _monsters;
    private bool _showGameOverMessage;

    private void OnEnable() => _monsters = FindObjectsOfType<Monster>();

    private void Update()
    {
        // перезагрузить сцену при нажатии кнопки 
        if (Input.GetKeyDown(KeyCode.R))
            Scene.ReloadScene(); // статичный метод класса Scene

        // закрыть приложение при нажатии кнопки
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        if (!_showGameOverMessage)
        {
            if (batScript.respawned)
            {
                scoreText.SetText(batScript.BatCount.ToString());

                foreach (Monster monster in _monsters)
                {
                    if (monster != null)
                        return;
                }
                Debug.Log("It's all!");
                _showGameOverMessage = true;
            }
        }
    }

    private void OnKillAllEnemies()
    {
        foreach (Monster monster in _monsters)
        {
            if (monster != null)
                return;
            else
                _showGameOverMessage = true;

            if (_showGameOverMessage)
            {
                Debug.Log("It's all!");
                _showGameOverMessage = false;
            }
        }
    }

    public void SceneReloader() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    public void MainLevelLoader() => SceneManager.LoadScene("StartScene");

    public void LoadNextScene() => SceneManager.LoadScene("Level" + (currentLevelIndex + 1));

    public void ShowOrHideButtons() => overlayAnimator.SetTrigger("Switch");

    public void ShowOrHideToolTip(TMP_Text toolTip)
    {
        if (toolTip.alpha == 1)
            toolTip.alpha = 0;
        else
            toolTip.alpha = 1;
    }
}
