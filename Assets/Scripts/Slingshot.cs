using UnityEngine;

public class Slingshot : MonoBehaviour
{
    private LineRenderer _cached_FrontRubberBandLR;
    private LineRenderer _cached_BackRubberBandLR;
    
    private Collider2D _cached_batCollider;

    [SerializeField] private GameObject _bat;

    public float LeftSideStickBound { get; private set; }
    public float RightSideStickBound { get; private set; }
    public float UpperSideStickBound { get; private set; }
    public Collider2D Cached_StickCollider { get; private set; }

    private void Awake()
    {
        _cached_batCollider = _bat.GetComponent<Collider2D>();
        _cached_FrontRubberBandLR = transform.Find("LeftAnchorPoint").GetComponent<LineRenderer>();
        _cached_BackRubberBandLR = transform.Find("RightAnchorPoint").GetComponent<LineRenderer>();
        Cached_StickCollider = GetComponentInChildren<Collider2D>();

        StickBounds();
    }

    public void StickBounds()
    {
        LeftSideStickBound = Cached_StickCollider.transform.position.x
            - Cached_StickCollider.bounds.extents.x - _cached_batCollider.bounds.extents.x;

        RightSideStickBound = Cached_StickCollider.transform.position.x
            + Cached_StickCollider.bounds.extents.x + _cached_batCollider.bounds.extents.x;

        UpperSideStickBound = Cached_StickCollider.transform.position.y
            + Cached_StickCollider.bounds.extents.y + _cached_batCollider.bounds.extents.y;
    }

    public void RubberBandJunctionPoint(Vector3 position)
    {
        _cached_FrontRubberBandLR.SetPosition(0, transform.Find("LeftAnchorPoint").position);
        _cached_FrontRubberBandLR.SetPosition(1, position - _cached_batCollider.bounds.extents / 2);

        _cached_BackRubberBandLR.SetPosition(0, transform.Find("RightAnchorPoint").position);
        _cached_BackRubberBandLR.SetPosition(1, position - _cached_batCollider.bounds.extents / 2);
    }
}
