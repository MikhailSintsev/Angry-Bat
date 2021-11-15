using UnityEngine;

public class Slingshot : MonoBehaviour
{
    // Передний и задний LineRenderer для отрисовки резиновой ленты
    private LineRenderer _cached_FrontRubberBandLR;
    private LineRenderer _cached_BackRubberBandLR;
    
    private Collider2D _cached_batCollider; // кешированный коллайдер объекта _bat

    [SerializeField] private GameObject _bat; // ссылка на объект Grey Bat

    // публичные свойства для передачи данных в другой скрипт
    public float LeftSideStickBound { get; private set; } // левая граница объекта
    public float RightSideStickBound { get; private set; } // правая граница объекта
    public float UpperSideStickBound { get; private set; } // верхняя граница объекта
    public Collider2D Cached_StickCollider { get; private set; } // кешированный коллайдер объекта

    private void Awake()
    {
        _cached_batCollider = _bat.GetComponent<Collider2D>(); // кешировать на коллайдер объекта _bat
        Cached_StickCollider = GetComponentInChildren<Collider2D>(); // кешировать коллайдер объекта
        
        // кешировать LineRenderer объекта 
        _cached_FrontRubberBandLR = transform.Find("LeftAnchorPoint").GetComponent<LineRenderer>();
        _cached_BackRubberBandLR = transform.Find("RightAnchorPoint").GetComponent<LineRenderer>();

        // включить использование мировых координат LineRenderer'ами, иначе координаты считаются относительно родительского
        // объекта (в режиме редактора отключено для удобства позиционирования)
        _cached_FrontRubberBandLR.useWorldSpace = true;
        _cached_BackRubberBandLR.useWorldSpace = true;

        StickBounds(); // рассчитать границы коллайдера объекта
    }

    // Получить границы коллайдера объекта Slingshot и учесть коллайдер объекта _bat, чтобы коллайдеры не 
    // пересекались.
    // К координате центра коллайдера объекта по определенной оси прибавляется или вычитается половина
    // ширины или высоты (влево или вниз - минус, вправо или вверх - плюс), затем прибавляется 
    // или вычитается половина ширины или высоты коллайдера объекта _bat.
    public void StickBounds()
    {
        LeftSideStickBound = Cached_StickCollider.transform.position.x
            - Cached_StickCollider.bounds.extents.x - _cached_batCollider.bounds.extents.x;

        RightSideStickBound = Cached_StickCollider.transform.position.x
            + Cached_StickCollider.bounds.extents.x + _cached_batCollider.bounds.extents.x;

        UpperSideStickBound = Cached_StickCollider.transform.position.y
            + Cached_StickCollider.bounds.extents.y + _cached_batCollider.bounds.extents.y;
    }

    /* Рассчитать положение начальных и конечных точек двух LineRenderer'ов.
     * Первым точкам присваиваются координаты "якорей" (объекты LeftAnchorPoint и RightAnchorPoint),
     * вторым точкам присваиваются координаты объекта _bat с учетом четверти его коллайдера
     * (коллайдер учитывать не обязательно, но так визуально более правильно)*/
    public void RubberBandJunctionPoint(Vector3 position)
    {
        _cached_FrontRubberBandLR.SetPosition(0, transform.Find("LeftAnchorPoint").position);
        _cached_FrontRubberBandLR.SetPosition(1, position - _cached_batCollider.bounds.extents / 2);

        _cached_BackRubberBandLR.SetPosition(0, transform.Find("RightAnchorPoint").position);
        _cached_BackRubberBandLR.SetPosition(1, position - _cached_batCollider.bounds.extents / 2);
    }
}
