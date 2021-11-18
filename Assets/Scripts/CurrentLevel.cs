using UnityEngine;
using UnityEngine.SceneManagement;

public class CurrentLevel : MonoBehaviour
{
    void Start()
    {
        Debug.Log(SceneManager.GetActiveScene().buildIndex);
    }

}
