using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField] private GameObject _cloudParticlePrefab;

    private Rigidbody2D _cachedRigidbody;
    private bool _isFalling;

    private void Awake() => _cachedRigidbody = GetComponent<Rigidbody2D>();

    private void Update()
    {
        if (_cachedRigidbody.velocity.magnitude >= 4f)
            _isFalling = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Bat bat = collision.collider.GetComponent<Bat>();

        if (bat != null)
        {
            Die();
            return;
        }

        //Monster monster = collision.collider.GetComponent<Monster>();

        //if (monster != null)
        //    return;

        if (_isFalling)
        {
            Die();
            return;
        }

        if (collision.contacts[0].normal.y < -0.5f)
            Die();
    } 
    
    private void Die()
    {
        Instantiate(_cloudParticlePrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
