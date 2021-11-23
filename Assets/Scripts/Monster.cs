using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField] private GameObject _cloudParticlePrefab; // ссылка на префаб системы частиц

    private Rigidbody2D _cached_Rigidbody; // кешированный Rigidbody2D объекта
    private AudioSource _cached_AudioSource; // кешированный AudioSource объекта
    private bool _isFalling; // проверка падения объекта

    private void Awake()
    {
        _cached_Rigidbody = GetComponent<Rigidbody2D>(); // кешировать Rigidbody2D
        _cached_AudioSource = GetComponent<AudioSource>(); // кешировать AudioSource объекта
    }

    private void Update()
    {
        // если скорость объекта больше или равна заданному числу
        if (_cached_Rigidbody.velocity.magnitude >= 4f)
            _isFalling = true; // считать, что объект падает
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Bat bat = collision.collider.GetComponent<Bat>(); // пытаться получить скрипт Bat у объекта столкновения

        // если скрипт не получен, скороть объекта больше заданного числа и звук не проигрывается объектом
        if (_cached_AudioSource.isActiveAndEnabled && bat == null && _cached_Rigidbody.velocity.magnitude >= 0.3f && !_cached_AudioSource.isPlaying)
        {
            _cached_AudioSource.Play(); // проиграть звук
        }

        // если скрипт получен
        if (bat != null)
        {
            Die(); // выполнить метод Die()
            return; // и выйти из метода OnCollisionEnter2D
        }

        // если объект падает
        if (_isFalling)
        {
            Die(); // выполнить метод Die()
            return; // и выйти из метода OnCollisionEnter2D
        }

        // если нормаль объекта столкновения к данному объекту по оси Y меньше заданного значения (на объект что то падает)
        if (collision.contacts[0].normal.y < -0.5f)
            Die(); // выполнить метод Die()
    } 
    
    private void Die()
    {
        // создать систему частиц на основе префаба и уничтожить через заданное время
        Destroy(Instantiate(_cloudParticlePrefab, transform.position, Quaternion.identity), 3);
        Destroy(gameObject); // уничтожить объект
    }
}
