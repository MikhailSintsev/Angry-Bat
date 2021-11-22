using UnityEngine;

public class RepeatBackground : MonoBehaviour
{
    private Vector3 _startPosition;
    private float _repeatWidth;
    private float _speed = 0.3f;

    void Start()
    {
        _startPosition = transform.position;
        _repeatWidth = GetComponent<BoxCollider>().size.x / 2;
    }

    void Update()
    {
        if (transform.position.x < _startPosition.x - _repeatWidth)
            transform.position = _startPosition;
        
        transform.Translate(Vector3.left * _speed * Time.deltaTime);
    }
}
