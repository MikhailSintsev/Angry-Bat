using UnityEngine;
using System.Collections;

public class Bat : MonoBehaviour
{
    private SpriteRenderer _cached_Renderer;
    private Rigidbody2D _cached_Rigidbody;
    private Animator _cached_Animator;
    private AudioSource _cached_AudioSource;

    private Collider2D _cached_StickCollider;
    private Slingshot _slingshotScript;

    private Vector3 _directionToProjectilePosition;
    private Vector3 _projectilePosition;
    private bool _wasLaunched;
    private bool _played;
    private float _timer;
    private float _worldBound = 40f;

    [SerializeField] private AudioClip _bat_fly;
    [SerializeField] private AudioClip _stretch;

    [SerializeField] private float _prelaunchFlySpeed = 5f;
    [SerializeField] private float _maxDragDistance = 2f;

    [SerializeField] private GameObject _feathersParticlePrefab;
    [SerializeField] private GameObject _slingshot;
    [SerializeField] private GameObject _startPoint;

    [SerializeField]
    [Range(50, 1000)]
    [Tooltip("Set the launch power")]
    private int _launchPower;

    private void Awake()
    {
        _slingshotScript = _slingshot.GetComponent<Slingshot>();

        _cached_Renderer = GetComponent<SpriteRenderer>();
        _cached_Rigidbody = GetComponent<Rigidbody2D>();
        _cached_Animator = GetComponent<Animator>();
        _cached_AudioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _cached_StickCollider = _slingshotScript.Cached_StickCollider;
        _projectilePosition = _slingshot.transform.Find("ProjectilePosition").position;
        _slingshotScript.RubberBandJunctionPoint(_projectilePosition);

        StartCoroutine("Respawn");
    }

    private IEnumerator Respawn()
    {
        _wasLaunched = false;
        _cached_Rigidbody.gravityScale = 0;
        transform.position = _startPoint.transform.position;
        transform.rotation = _startPoint.transform.rotation;
        _cached_Animator.SetBool("CanFly", true);

        yield return new WaitForSeconds(0.4f);

        while (Vector3.Distance(transform.position, _projectilePosition) > float.Epsilon)
        {
            transform.position = Vector3.Lerp(transform.position, _projectilePosition, Time.deltaTime * _prelaunchFlySpeed);
            yield return null;
        }
    }

    private void Update()
    {
        if (_wasLaunched && _cached_Rigidbody.velocity.magnitude <= 0.05)
            _timer += Time.deltaTime;
        else
            _timer = 0;

        if (OutOfBounds() || TimeAfterStopIsUp())
        {
            Destroy(Instantiate(_feathersParticlePrefab, transform.position, Quaternion.identity), 3);
            StartCoroutine("Respawn");
        }
    }

    private bool OutOfBounds()
    {
        if (transform.position.x <= -_worldBound
            || transform.position.x >= _worldBound
            || transform.position.y <= -_worldBound
            || transform.position.y >= _worldBound)
        {
            return true;
        }
        return false;
    }

    private bool TimeAfterStopIsUp()
    {
        if (_timer >= 3)
            return true;
        return false;
    }

    private void OnMouseDown()
    {
        StopCoroutine("Respawn");
        if (!_wasLaunched)
        {
            _cached_Renderer.color = Color.red;
            _cached_Animator.enabled = true;
        }
    }

    private void OnMouseDrag()
    {
        if (!_wasLaunched)
        {
            if (!_played)
            {
                _cached_AudioSource.PlayOneShot(_stretch, 0.5f);
                _played = true;
            }

            Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 desiredPosition = new Vector3(newPosition.x, newPosition.y);

            float distance = Vector3.Distance(desiredPosition, _projectilePosition);

            if (distance >= _maxDragDistance)
            {
                Vector3 direction = desiredPosition - _projectilePosition;
                direction.Normalize();
                desiredPosition = _projectilePosition + (direction * _maxDragDistance);
            }

            if (desiredPosition.x >= _slingshotScript.LeftSideStickBound
                && desiredPosition.x <= _cached_StickCollider.transform.position.x
                && desiredPosition.y <= _slingshotScript.UpperSideStickBound)
            {
                desiredPosition.x = _slingshotScript.LeftSideStickBound;
            }
            else if (desiredPosition.x >= _cached_StickCollider.transform.position.x
                && desiredPosition.x <= _slingshotScript.RightSideStickBound
                && desiredPosition.y <= _slingshotScript.UpperSideStickBound)
            {
                desiredPosition.x = _slingshotScript.RightSideStickBound;
            }

            transform.position = desiredPosition;

            _slingshotScript.RubberBandJunctionPoint(transform.position);
        }
    }

    private void OnMouseUp()
    {
        if (!_wasLaunched)
        {
            _cached_AudioSource.Stop();
            _cached_AudioSource.PlayOneShot(_bat_fly, 0.5f);

            _slingshotScript.RubberBandJunctionPoint(_projectilePosition);

            _cached_Renderer.color = Color.white;
            _directionToProjectilePosition = _projectilePosition - transform.position;
            _cached_Rigidbody.gravityScale = 1;
            _cached_Rigidbody.AddForce(_directionToProjectilePosition * _launchPower);
            _wasLaunched = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _cached_Animator.SetBool("CanFly", false);
    }
}