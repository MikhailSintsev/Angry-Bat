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

    // заполнение массива монстров при запуске игры
    private void OnEnable() => _monsters = FindObjectsOfType<Monster>();

    private void Start()
    {
        _canShowExitMessage = true; // задать начальное значение
        _showGameOverMessage = false; // задать начальное значение
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

        if (!_showGameOverMessage) // если сообщение о конце игры не должно быть показано
        {
            // проверять каждый элемент массива на null
            foreach (Monster monster in _monsters)
            {
                // если не null - выйти из метода Update
                if (monster != null)
                    return;
            }
            batScript.stopRespawns = true; // остановить респавны
            Debug.Log("It's all!"); // сообщение в консоль
            StartCoroutine(nameof(LevelComplete)); // запустить корутину завершения уровня
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

    // активировать триггер аниматора Overlay Canvas
    public void ShowOrHideButtons() => overlayAnimator.SetTrigger("Switch");

    // показать/скрыть меню выхода в главное меню
    public void ShowOrHideExitMessage()
    {
        if (_canShowExitMessage) // если необходимо показать меню выхода
        {
            centerMessage.alpha = 0; // сделать центральное сообщение прозрачным
            overlayAnimator.SetTrigger("Exit"); // активировать триггер аниматора Overlay Canvas
            _canShowExitMessage = false;
        }
        else
        {
            overlayAnimator.SetTrigger("Cancel Exit"); // активировать триггер аниматора Overlay Canvas
            _canShowExitMessage = true;
            // возвращать непрозрачность центрального текста, если респавны остановлены (игра окончена)
            if (batScript.stopRespawns == true)
                centerMessage.alpha = 1;
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
        yield return new WaitForSeconds(5f); // подождать заданное количество времени
        // написать, что уровень завершен (с указанием номера уровня)
        centerMessage.SetText("Level " + currentLevelIndex + " Completed");
        centerMessage.alpha = 1; // сделать текст непрозрачным

        //if (_highScore < batScript.BatCount)
        //{
        //    _highScore = batScript.BatCount;
        //    highScoreText.SetText("Best: " + _highScore);
        //}
    }
}
