using UnityEngine;

public class Bat : MonoBehaviour
{
    private Collider2D _cachedStickCollider;
    private LineRenderer _cachedFrontRubberBandLR;
    private LineRenderer _cachedBackRubberBandLR;
    private GameObject _cachedFrontRubberBand;
    private GameObject _cachedBackRubberBand;
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
    [SerializeField] private GameObject _slingshot;

    [SerializeField]
    [Range(50, 1000)]
    [Tooltip("Set the launch power")]
    private int _launchPower;

    private void Awake()
    {
        _cachedStickCollider = _slingshot.GetComponentInChildren<Collider2D>();
        _cachedCollider = GetComponent<Collider2D>();
        _cachedRenderer = GetComponent<SpriteRenderer>();
        _cachedRigidbody = GetComponent<Rigidbody2D>();
        _cachedAnimator = GetComponent<Animator>();

        _cachedFrontRubberBand = _slingshot.transform.Find("LeftAnchorPoint").gameObject;
        _cachedBackRubberBand = _slingshot.transform.Find("RightAnchorPoint").gameObject;
        _cachedFrontRubberBandLR = _cachedFrontRubberBand.GetComponent<LineRenderer>();
        _cachedBackRubberBandLR = _cachedBackRubberBand.GetComponent<LineRenderer>();

        _projectilePosition = _slingshot.transform.Find("ProjectilePosition").position;
        transform.position = _projectilePosition;
        //_cachedAnimator.enabled = false;

        StickBounds();
        RubberBandJunctionPoint(_projectilePosition);
    }

    private void Update()
    {
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
            + _cachedStickCollider.bounds.extents.y + _cachedCollider.bounds.extents.y;
    }

    private void RubberBandJunctionPoint(Vector3 position)
    {
        _cachedFrontRubberBandLR.SetPosition(0, _slingshot.transform.Find("LeftAnchorPoint").position);
        _cachedFrontRubberBandLR.SetPosition(1, position - _cachedCollider.bounds.extents / 2);

        _cachedBackRubberBandLR.SetPosition(0, _slingshot.transform.Find("RightAnchorPoint").position);
        _cachedBackRubberBandLR.SetPosition(1, position - _cachedCollider.bounds.extents / 2);
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
            Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 desiredPosition = new Vector3(newPosition.x, newPosition.y);

            float distance = Vector3.Distance(desiredPosition, _projectilePosition);            

            if (distance >= _maxDragDistance)
            {
                Vector3 direction = desiredPosition - _projectilePosition;
                direction.Normalize();
                desiredPosition = _projectilePosition + (direction * _maxDragDistance);
            }

            if (desiredPosition.x >= _leftSideStickBound
                && desiredPosition.x <= _cachedStickCollider.transform.position.x
                && desiredPosition.y <= _upperSideStickBound)
            {
                desiredPosition.x = _leftSideStickBound;
            }
            else if (desiredPosition.x >= _cachedStickCollider.transform.position.x
                && desiredPosition.x <= _rightSideStickBound
                && desiredPosition.y <= _upperSideStickBound)
            {
                desiredPosition.x = _rightSideStickBound;
            }
            
            transform.position = desiredPosition;

            RubberBandJunctionPoint(transform.position);
        }
    }

    private void OnMouseUp()
    {
        if (!_wasLaunched)
        {
            RubberBandJunctionPoint(_projectilePosition);

            _cachedRenderer.color = Color.white;
            _directionToInitialPosition = _projectilePosition - transform.position;
            _cachedRigidbody.gravityScale = 1;
            _cachedRigidbody.AddForce(_directionToInitialPosition * _launchPower);
            _wasLaunched = true;
        }
    }
}