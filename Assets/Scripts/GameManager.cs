using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public int currentLevelIndex; // индекс текущего уровня
    public bool theNextLevelExists; // проверка наличия следующего уровня
    public Animator overlayAnimator; // компонент Animator объекта Overlay Canvas
    public Bat batScript; // скрипт объекта Bat
    public TMP_Text scoreText; // компонент Text объекта Current Score Text
    //public TMP_Text highScoreText;
    public TMP_Text centerMessage; // компонент Text объекта Center Text
    public TMP_Text exitMessage; // компонент Text объекта Exit Text
    public TMP_Text topMessage; // компонент Text объекта Top Text

    //private int _highScore;
    private Monster[] _monsters; // создание массива монстров
    private bool _showGameOverMessage; // проверка необходимости показать сообщение о конце игры
    private bool _canShowExitMessage; // проверка необходимости показать сообщение о выходе в главное меню
    private bool _levelComplete; // проверка завершения уровня

    // заполнение массива монстров при запуске игры
    private void OnEnable() => _monsters = FindObjectsOfType<Monster>();

    private void Start()
    {
        _canShowExitMessage = true; // задать начальное значение
        _showGameOverMessage = false; // задать начальное значение
        _levelComplete = false; // задать начальное значение
        SetCurrentLevelTextMessage(); // отобразить номер уровня
        SetScore(); // отобразить начальное количество очков (0)
        //highScoreText.SetText("Best: " + _highScore);
    }

    private void Update()
    {
        // перезагрузить сцену при нажатии кнопки 
        if (Input.GetKeyDown(KeyCode.R))
            SceneReloader();

        // вызвать меню выхода при нажатии кнопки
        if (Input.GetKeyDown(KeyCode.Escape))
            ShowOrHideExitMessage();

        SetScore(); // отобразить количество очков
        batScript.stopRespawns = _levelComplete;

        if (!_showGameOverMessage) // если сообщение о конце игры не было показано
        {
            // проверять каждый элемент массива на null
            foreach (Monster monster in _monsters)
            {
                // если не null - выйти из метода Update
                if (monster != null)
                    return;
            }
            //batScript.stopRespawns = true; // остановить респавны
            Debug.Log("It's all!"); // сообщение в консоль
            StartCoroutine(nameof(LevelComplete)); // запустить корутину завершения уровня
            _levelComplete = true;
            _showGameOverMessage = true; // поменять значение, чтобы предотвратить повторное выполнение предыдущего кода
        }
    }

    // задать текст компонента, используя скрипт объекта Bat
    private void SetScore() => scoreText.SetText(batScript.BatCount.ToString());

    // задать текст компонента, используя текущий номер сцены
    private void SetCurrentLevelTextMessage() => topMessage.SetText("Level " + currentLevelIndex);

    // загрузить текущую сцену
    public void SceneReloader() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    // загрузить главную сцену
    public void MainLevelLoader() => SceneManager.LoadScene("StartScene");

    // загрузить следующую сцену
    public void LoadNextScene()
    {
        if (theNextLevelExists) // если следующая сцена существует
            // прибавить к текущему номеру сцены 1 и загрузить сцену
            SceneManager.LoadScene("Level" + ++currentLevelIndex);
        else
            // иначе - запустить корутину отображения сообщения об отсутствии следующей сцены
            StartCoroutine(nameof(SorryMessage));
    }

    public void ShowOrHideButtons()
    {
        if (_canShowExitMessage) // если меню выхода не показано 
            overlayAnimator.SetTrigger("Switch"); // активировать триггер аниматора Overlay Canvas
    }

    // показать/скрыть меню выхода в главное меню
    public void ShowOrHideExitMessage()
    {
        if (_canShowExitMessage) // если меню выхода не показано
        {
            if (_levelComplete) // и если уровень завершен
                overlayAnimator.SetBool("Level Complete", false); // скрыть сообщение о завершении уровня
            overlayAnimator.SetBool("Exit", true);
            _canShowExitMessage = false;
        }
        else
        {
            overlayAnimator.SetBool("Exit", false);
            // возвращать непрозрачность центрального текста, если респавны остановлены (игра окончена)
            if (_levelComplete)
                overlayAnimator.SetBool("Level Complete", true);
            _canShowExitMessage = true;
        }
    }

    // показать/скрыть всплывающую подсказку, меняя уровень прозрачности при входе/выходе курсора в область кнопки
    public void ShowOrHideToolTip(TMP_Text toolTip)
    {
        if (toolTip.alpha == 1)
            toolTip.alpha = 0;
        else
            toolTip.alpha = 1;
    }

    private IEnumerator SorryMessage()
    {
        // написать, что следующий уровень не существует
        topMessage.SetText("The next level does not exist. Sorry!");
        yield return new WaitForSeconds(3f); // подождать заданное количество времени
        SetCurrentLevelTextMessage(); // вернуть отображение текущего уровня
    }

    private IEnumerator LevelComplete()
    {
        yield return new WaitForSeconds(3f); // подождать заданное количество времени
        // написать, что уровень завершен (с указанием номера уровня)
        centerMessage.SetText("Level " + currentLevelIndex + " Completed");

        if (_canShowExitMessage) // если меню выхода не показано
            overlayAnimator.SetBool("Level Complete", true); // показать сообщение о завершении уровня
        else // иначе скрыть сообщение
            overlayAnimator.SetBool("Level Complete", false);

        //if (_highScore < batScript.BatCount)
        //{
        //    _highScore = batScript.BatCount;
        //    highScoreText.SetText("Best: " + _highScore);
        //}
    }
}
