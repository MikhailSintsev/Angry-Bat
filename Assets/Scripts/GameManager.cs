using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Update()
    {
        // ������������� ����� ��� ������� ������ 
        if (Input.GetKeyDown(KeyCode.R))
            Scene.ReloadScene(); // ��������� ����� ������ Scene

        // ������� ���������� ��� ������� ������
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }
}
