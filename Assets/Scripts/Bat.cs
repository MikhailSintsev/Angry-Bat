using UnityEngine;
using System.Collections;

public class Bat : MonoBehaviour
{
    public bool respawned; // проверка респавна
    public bool stopRespawns; // проверка необходимости остановки респавна

    private SpriteRenderer _cached_Renderer; // кешированный Renderer
    private Rigidbody2D _cached_Rigidbody; // кешированный Rigidbody
    private Animator _cached_Animator; // кешированный Animator
    private AudioSource _cached_AudioSource; // кешированный AudioSourse

    private Collider2D _cached_StickCollider; // кешированный Collider2D
    private Slingshot _slingshotScript; // скрипт Slingshot объекта Slingshot

    private Vector3 _directionToProjectilePosition; // направление к позиции "запуска" объекта
    private Vector3 _projectilePosition; // позиция "запуска" объекта
    private bool _stretchSoundWasPlayed; // переменная для проверки проигрывания звука
    private float _timer; // отсчет времени после остановки объекта
    private float _worldBound = 30f; // границы мира
    private float _prelaunchFlySpeed = 8f; // начальная скорость объекта к позиции "запуска"
    private float _maxDragDistance = 1.5f; // максимальное расстояние перетаскивания объекта от позиции "запуска"
    private int _maxBatCount = 99; // максимальное количество полученных очков

    [SerializeField] private bool _wasLaunched; // переменная для проверки "запуска"
    [SerializeField] private AudioClip _bat_fly; // звук "запуска"
    [SerializeField] private AudioClip _stretch; // звук натяжения резиновой ленты

    [SerializeField] private GameObject _feathersParticlePrefab; // префаб системы частиц
    [SerializeField] private GameObject _slingshot; // объект Slingshot
    [SerializeField] private GameObject _startPoint; // начальная точка появления объекта

    [SerializeField]
    [Tooltip("Текущее расстояние от объекта до точки запуска")]
    private float currentDistance = 0f; // текущее расстояние от объекта до точки "запуска"

    [SerializeField]
    [Range(50, 1000)]
    [Tooltip("Сила запуска")]
    private int _launchPower; // сила "запуска"

    public int BatCount { get; private set; } // свойства для получения/установки количества очков

    private void Awake()
    {
        _slingshotScript = _slingshot.GetComponent<Slingshot>(); // получить скрипт Slingshot

        _cached_Renderer = GetComponent<SpriteRenderer>(); // кешировать SpriteRenderer объекта
        _cached_Rigidbody = GetComponent<Rigidbody2D>(); // кешировать Rigidbody2D объекта
        _cached_Animator = GetComponent<Animator>(); // кешировать Animator объекта
        _cached_AudioSource = GetComponent<AudioSource>(); // кешировать AudioSourse объекта
    }

    private void Start()
    {
        stopRespawns = false;
        // получить коллайдер объекта Slingshot, кешированный в скрипте Slingshot
        _cached_StickCollider = _slingshotScript.Cached_StickCollider;
        // получить позицию "запуска", используя дочерний объект ProjectilePosition объекта Slingshot
        _projectilePosition = _slingshot.transform.Find("ProjectilePosition").position;
        // присвоить точке соединения двух LineRenderer (отрисовка резиновой ленты) координаты точки "запуска"
        _slingshotScript.RubberBandJunctionPoint(_projectilePosition);

        StartCoroutine(nameof(Respawn)); // запустить респавн
        BatCount = 0; // установить количество очков при старте
    }

    private IEnumerator Respawn()
    {
        _cached_Rigidbody.gravityScale = 0; // отключить гравитацию, чтобы объект не упал с начальной позиции
        /* Переместить объект по вертикали за пределы экрана. Таким образом, объект будет исчезать со сцены
         * и появляться с задержкой (и задержит камеру во время проигрывания системы частиц после исчезания объекта). */
        transform.position = new Vector3(transform.position.x, _startPoint.transform.position.y);

        if (stopRespawns) // если stopRespawns=true, остановить выполнение корутины
            yield break;

        yield return new WaitForSeconds(1f);

        _wasLaunched = false; // объект не был запущен
        _stretchSoundWasPlayed = false; // звук натяжения резиновой ленты не был проигран
        transform.SetPositionAndRotation(_startPoint.transform.position, _startPoint.transform.rotation); // изменить позицию и ротацию объекта на исходные
        _cached_Animator.SetBool("CanFly", true); // проигрывать анимацию полета
        respawned = true; // считать, что объект зареспавнен

        yield return new WaitForSeconds(0.4f);

        // Плавно переместить объект (с замедлением) к позиции "запуска" с начальной скоростью полета _prelaunchFlySpeed
        while (Vector3.Distance(transform.position, _projectilePosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, _projectilePosition, Time.deltaTime * _prelaunchFlySpeed);
            yield return null;
        }
    }

    private void Update()
    {
        // постоянно считать текущее расстояние от позиции объекта до позиции "запуска" (для отображения в инспекторе)
        currentDistance = Vector3.Distance(transform.position, _projectilePosition);
        // если объект был запущен и его скорость меньше определенного числа - запустить таймер
        if (_wasLaunched && _cached_Rigidbody.velocity.magnitude <= 0.05)
            _timer += Time.deltaTime;
        else
            _timer = 0;
        // если объект не респавнился после "запуска"
        if (_wasLaunched)
        {
            // если объект за пределами мира или время после его остановки вышло
            if (OutOfBounds() || TimeAfterStopIsUp())
            {
                _cached_Rigidbody.constraints = RigidbodyConstraints2D.None; // разрешить объекту двигаться и вращаться
                // создать префаб системы частиц на позиции объекта и уничтожить через определенное время
                Destroy(Instantiate(_feathersParticlePrefab, transform.position, Quaternion.identity), 3);
                StartCoroutine(nameof(Respawn)); // запустить корутину Respawn
                //respawned = true; // считать объект зареспавненым, чтобы предотвратить многократное выполнение предшествующего кода
                _wasLaunched = false;
            }
        }
    }

    private bool OutOfBounds()
    {
        // если объект за пределами границ по горизонтали или вертикали
        if (transform.position.x <= -_worldBound
            || transform.position.x >= _worldBound
            || transform.position.y <= -_worldBound
            || transform.position.y >= _worldBound)
        {
            // остановить движение и вращение объекта
            _cached_Rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            return true;
        }
        return false;
    }

    // вернуть true, если значение переменной _timer больше заданного
    private bool TimeAfterStopIsUp()
    {
        if (_timer >= 3)
            return true;
        return false;
    }

    private void OnMouseDown()
    {
        // остановить корутину, чтобы прекратить попытки объекта переместиться в _projectilePosition
        StopCoroutine(nameof(Respawn));

        if (!_wasLaunched) // если объект не был "запущен"
        {
            _cached_Renderer.color = Color.red; // изменить цвет объекта
            //_cached_Animator.enabled = true;
        }
    }

    private void OnMouseDrag()
    {
        if (!_wasLaunched)
        {
            // проиграть звук натяжения резиновой ленты, если он не был проигран
            if (!_stretchSoundWasPlayed)
            {
                _cached_AudioSource.PlayOneShot(_stretch, 0.5f);
                _stretchSoundWasPlayed = true;
            }
            // вычислить позицию курсора мыши
            Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // вычислить желаемую позицию объекта (без учета координаты по оси Z, сделать ее равной 0)
            Vector3 desiredPosition = new Vector3(newPosition.x, newPosition.y);
            // вычислить текущее расстояние между желаемой позицией объекта и точкой "запуска"
            float distance = Vector3.Distance(desiredPosition, _projectilePosition);

            // если текущее расстояние от объекта до позиции "запуска" больше или равно максимальному
            if (distance >= _maxDragDistance)
            {
                /* Здесь вычисляется позиция объекта, если пользователь пытается перетащить его дальше, чем позволяет игра.
                 * Нужно сохранить положение объекта с учетом максимального расстояния перетаскивания. */
                // вычислить направление от позиции "запуска" до объекта
                Vector3 direction = desiredPosition - _projectilePosition;
                direction.Normalize(); // нормализовать вектор направления (длина вектора будет равна 1)
                // вычислить желаемую позицию объекта и ограничить максимальное расстояние перетаскивания
                desiredPosition = _projectilePosition + (direction * _maxDragDistance);
            }

            /* Предотвратить пересечение коллайдера объекта с коллайдером Slingshot. 
             * Проверяется положение курсора мыши во время перетаскивания объекта и при пересечении с коллайдером Slingshot
             * желаемая позиция объекта ограничивается границами коллайдера Slingshot, которые просчитываются в скрипте Slingshot.
             * Границы расчитаны с учетом половины коллайдера объекта (bounds.extents).
             */
            // если желаемая позиция находится между левой границей коллайдера Slingshot и центром, а также ниже верхней границы
            if (desiredPosition.x >= _slingshotScript.LeftSideStickBound
                && desiredPosition.x <= _cached_StickCollider.transform.position.x
                && desiredPosition.y <= _slingshotScript.UpperSideStickBound)
            {
                // присвоить координате по оси X объекта координату по оси X левой границы коллайдера объекта Slingshot
                desiredPosition.x = _slingshotScript.LeftSideStickBound;
            }
            // если желаемая позиция находится между правой границей коллайдера Slingshot и центром, а также ниже верхней границы
            else if (desiredPosition.x <= _slingshotScript.RightSideStickBound
                && desiredPosition.x >= _cached_StickCollider.transform.position.x
                && desiredPosition.y <= _slingshotScript.UpperSideStickBound)
            {
                // присвоить координате по оси X объекта координату по оси X правой границы коллайдера объекта Slingshot
                desiredPosition.x = _slingshotScript.RightSideStickBound;
            }

            transform.position = desiredPosition; // присвоить объекту координаты желаемой позиции

            // присвоить точке соединения двух LineRenderer координаты объекта 
            _slingshotScript.RubberBandJunctionPoint(transform.position);
        }
    }

    private void OnMouseUp()
    {
        if (!_wasLaunched)
        {
            _cached_AudioSource.Stop(); // перестать проигрывать звук натяжения резиновой ленты
            _cached_AudioSource.PlayOneShot(_bat_fly, 0.5f); // проиграть звук полета

            // вернуть точке соединения двух LineRenderer координаты точки "запуска"
            _slingshotScript.RubberBandJunctionPoint(_projectilePosition);

            _cached_Renderer.color = Color.white; // вернуть объекту исходный цвет
            // расчитать направление от объекта до точки "запуска"
            _directionToProjectilePosition = _projectilePosition - transform.position;
            _cached_Rigidbody.gravityScale = 1; // включить гравитацию
            // "запуск" объекта по расчитанному направлению с заданной силой
            _cached_Rigidbody.AddForce(_directionToProjectilePosition * _launchPower);
            _wasLaunched = true; // считать, что объект был запущен
            respawned = false; // считать, что объект не был зареспавнен

            // прибавлять количество очков, пока не будет достигнуто максимальное количество 
            if (BatCount < _maxBatCount)
                BatCount++;
            else
                BatCount = _maxBatCount;
        }
    }

    // при пересечении с любым коллайдером остановить анимацию полета
    private void OnCollisionEnter2D(Collision2D collision) => _cached_Animator.SetBool("CanFly", false);
}