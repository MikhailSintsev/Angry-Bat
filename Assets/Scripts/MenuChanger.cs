using UnityEngine;

public class MenuChanger : MonoBehaviour
{
    public Animator canvasAnimator; // аниматор холста главной сцены

    // активировать триггер аниматора, чтобы запустить анимацию смены меню
    public void ChangeMenu() => canvasAnimator.SetTrigger("Change");
}
