using UnityEngine;

public class Crate : MonoBehaviour
{
    private AudioSource _cached_AudioSource; // кешированный AudioSource объекта
    private Rigidbody2D _cached_Rigidbody; // кешированный Rigidbody2D объекта

    private void Awake()
    {
        _cached_AudioSource = GetComponent<AudioSource>(); // кешировать AudioSource объекта
        _cached_Rigidbody = GetComponent<Rigidbody2D>(); // кешировать Rigidbody2D объекта
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // если скорость объекта после столкновения больше заданного числа и никакой звук не проигрывается
        if (_cached_Rigidbody.velocity.magnitude >= 1.5f && !_cached_AudioSource.isPlaying)
            _cached_AudioSource.Play(); // проиграть звук столкновения
    }
}
