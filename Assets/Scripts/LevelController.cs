using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    private Monster[] _monsters;
    private static int _currentLevelIndex = 1;
    private bool _canLoadNextLevel;
    private bool _showGameOverMessage;

    [SerializeField]
    private int _maxLevel = 2;

    private void OnEnable() => _monsters = FindObjectsOfType<Monster>();

    private void Update()
    {
        OnKillAllEnemies();
        LoadNextLevel();
    }

    private void OnKillAllEnemies()
    {
        foreach (Monster monster in _monsters)
        {
            if (monster != null)
                return;
        }
        _canLoadNextLevel = true;
    }

    private void LoadNextLevel()
    {
        if (_canLoadNextLevel)
        {
            if (_currentLevelIndex + 1 <= _maxLevel)
            {
                _currentLevelIndex++;
                string _nextLevelName = "Level" + _currentLevelIndex;
                Scene.LoadNextScene(_nextLevelName);
            }
            else
            {
                if (!_showGameOverMessage)
                {
                    Debug.Log("It is all!");
                    _showGameOverMessage = true;
                }
            }
        }
        return;
    }
}
