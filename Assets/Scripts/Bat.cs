using UnityEngine;

public class Bat : MonoBehaviour
{
    private Collider2D _cachedStickCollider;
    private LineRenderer _cachedLineRenderer;
    private SpriteRenderer _cachedRenderer;
    private Rigidbody2D _cachedRigidbody;
    private Animator _cachedAnimator;
    private Collider2D _cachedCollider;
    private Vector3 _directionToInitialPosition;
    private Vector3 _projectilePosition;
    private bool _wasLaunched;
    private float _timer;
    private float _bound = 30f;
    private float _leftSideStickBound;
    private float _rightSideStickBound;
    private float _upperSideStickBound;

    [SerializeField] private float _maxDragDistance = 2f;
    [SerializeField] private GameObject _lineRenderer;
    [SerializeField] private GameObject _slingshot;

    [SerializeField]
    [Range(50, 1000)]
    [Tooltip("Set the launch power")]
    private int _launchPower;

    private void Awake()
    {
        _cachedStickCollider = _slingshot.GetComponentInChildren<CapsuleCollider2D>();
        _cachedCollider = GetComponent<Collider2D>();
        _cachedLineRenderer = _lineRenderer.GetComponent<LineRenderer>();
        _cachedRenderer = GetComponent<SpriteRenderer>();
        _cachedRigidbody = GetComponent<Rigidbody2D>();
        _cachedAnimator = GetComponent<Animator>();

        _projectilePosition = GameObject.Find("ProjectilePosition").transform.position;
        transform.position = _projectilePosition;
        _cachedAnimator.enabled = false;

        StickBounds();
    }

    private void Update()
    {
        _cachedLineRenderer.SetPosition(0, transform.position);
        _cachedLineRenderer.SetPosition(1, _projectilePosition);

        if (_wasLaunched && _cachedRigidbody.velocity.magnitude <= 0.05)
            _timer += Time.deltaTime;
        else
            _timer = 0;

        if (OutOfBounds() || TimeAfterStopIsUp())
            Scene.ReloadScene();
    }

    private bool OutOfBounds()
    {
        if (transform.position.x <= -_bound 
            || transform.position.x >= _bound 
            || transform.position.y <= -_bound 
            || transform.position.y >= _bound)
        {
            return true;
        }
        return false;
    }

    private bool TimeAfterStopIsUp()
    {
        if (_timer >= 4)
            return true;
        return false;
    }

    private void StickBounds()
    {
        _leftSideStickBound = _cachedStickCollider.transform.position.x 
            - _cachedStickCollider.bounds.extents.x - _cachedCollider.bounds.extents.x;

        _rightSideStickBound = _cachedStickCollider.transform.position.x 
            + _cachedStickCollider.bounds.extents.x + _cachedCollider.bounds.extents.x;

        _upperSideStickBound = _cachedStickCollider.transform.position.y 
            + _cachedStickCollider.bounds.extents.y;
    }

    private void OnMouseDown()
    {
        if (!_wasLaunched)
        {
            _cachedRenderer.color = Color.red;
            _cachedAnimator.enabled = true;
        }
    }

    private void OnMouseDrag()
    {
        if (!_wasLaunched)
        {
            _cachedLineRenderer.enabled = true;
            Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 desiredPosition = new Vector3(newPosition.x, newPosition.y);

            float distance = Vector3.Distance(desiredPosition, _projectilePosition);

            if (distance >= _maxDragDistance)
            {
                Vector3 direction = desiredPosition - _projectilePosition;
                direction.Normalize();
                desiredPosition = _projectilePosition + (direction * _maxDragDistance);
            }

            if (newPosition.x > _leftSideStickBound
                && newPosition.x < _cachedStickCollider.transform.position.x
                && newPosition.y < _upperSideStickBound)
            {
                desiredPosition.x = _leftSideStickBound;
            }
            else if (newPosition.x > _cachedStickCollider.transform.position.x
                && newPosition.x < _rightSideStickBound
                && newPosition.y < _upperSideStickBound)
            {
                desiredPosition.x = _rightSideStickBound;
            }
            transform.position = desiredPosition;
        }
    }

    private void OnMouseUp()
    {
        if (!_wasLaunched)
        {
            _cachedRenderer.color = Color.white;
            _directionToInitialPosition = _projectilePosition - transform.position;
            _cachedRigidbody.gravityScale = 1;
            _cachedRigidbody.AddForce(_directionToInitialPosition * _launchPower);
            _wasLaunched = true;
            _cachedLineRenderer.enabled = false;
        }
    }
}