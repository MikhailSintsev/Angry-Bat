using UnityEngine;

public class RepeatBackground : MonoBehaviour
{
    private Vector3 _startPosition; // начальная позиция фона
    private float _repeatWidth; // половина ширины фона для расчета его смещения
    private float _speed = 0.3f; // скорость движения фона

    void Start()
    {
        _startPosition = transform.position; // присвоить стартовой позиции координаты текущей позиции
        _repeatWidth = GetComponent<BoxCollider>().size.x / 2; // расчитать половину ширины фона, используя Box Collider
    }

    void Update()
    {
        // если текущая позиция по оси X меньше, чем стартовая минус половина ширины фона
        if (transform.position.x < _startPosition.x - _repeatWidth)
            transform.position = _startPosition; // сместить фон на начальную позицию
        
        // смещать фон влево
        transform.Translate(Vector3.left * _speed * Time.deltaTime);
    }
}
