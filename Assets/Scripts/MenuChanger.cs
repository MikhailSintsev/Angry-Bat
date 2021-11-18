using UnityEngine;

public class MenuChanger : MonoBehaviour
{
    public Animator canvasAnimator;

    public void ChangeMenu() => canvasAnimator.SetTrigger("Change");
}
