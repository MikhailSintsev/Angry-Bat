using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public int currentLevelIndex;
    public bool theNextLevelExists;
    public Animator overlayAnimator;
    public Bat batScript;
    public TMP_Text scoreText;
    //public TMP_Text highScoreText;
    public TMP_Text centerMessage;
    public TMP_Text exitMessage;
    public TMP_Text topMessage;

    //private int _highScore;
    private Monster[] _monsters;
    private bool _showGameOverMessage;
    private bool _canShowExitMessage;

    private void OnEnable() => _monsters = FindObjectsOfType<Monster>();

    private void Start()
    {
        _canShowExitMessage = true;
        topMessage.SetText("Level " + currentLevelIndex);
        scoreText.SetText(batScript.BatCount.ToString());
        //highScoreText.SetText("Best: " + _highScore);
    }

    private void Update()
    {
        // перезагрузить сцену при нажатии кнопки 
        if (Input.GetKeyDown(KeyCode.R))
            SceneReloader();

        // закрыть приложение при нажатии кнопки
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            exitMessage.SetText("Back to Main Menu?");
            ShowOrHideExitMessage();
        }

        scoreText.SetText(batScript.BatCount.ToString());

        if (!_showGameOverMessage)
        {
            foreach (Monster monster in _monsters)
            {
                if (monster != null)
                    return;
            }
            batScript.stopRespawns = true;
            Debug.Log("It's all!");
            StartCoroutine(nameof(LevelComplete));
            _showGameOverMessage = true;
        }
    }

    public void SceneReloader() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    public void MainLevelLoader() => SceneManager.LoadScene("StartScene");

    public void LoadNextScene()
    {
        if (theNextLevelExists)
            SceneManager.LoadScene("Level" + ++currentLevelIndex);
        else
            StartCoroutine(nameof(SorryMessage));
    }

    public void ShowOrHideButtons() => overlayAnimator.SetTrigger("Switch");

    public void ShowOrHideExitMessage()
    {
        if (_canShowExitMessage)
        {
            centerMessage.alpha = 0;
            overlayAnimator.SetTrigger("Exit");
            _canShowExitMessage = false;
        }
        else
        {
            overlayAnimator.SetTrigger("Cancel Exit");
            _canShowExitMessage = true;
            if (batScript.stopRespawns == true)
            {
                centerMessage.alpha = 1;
            }
        }
    }

    public void ShowOrHideToolTip(TMP_Text toolTip)
    {
        if (toolTip.alpha == 1)
            toolTip.alpha = 0;
        else
            toolTip.alpha = 1;
    }

    private IEnumerator SorryMessage()
    {
        topMessage.SetText("The next level does not exist. Sorry!");
        yield return new WaitForSeconds(3f);
        topMessage.SetText("Level " + currentLevelIndex);
    }

    private IEnumerator LevelComplete()
    {
        yield return new WaitForSeconds(5f);
        centerMessage.SetText("Level " + currentLevelIndex + " Completed");
        centerMessage.alpha = 1;
        scoreText.SetText(batScript.BatCount.ToString());

        //if (_highScore < batScript.BatCount)
        //{
        //    _highScore = batScript.BatCount;
        //    highScoreText.SetText("Best: " + _highScore);
        //}
    }
}
