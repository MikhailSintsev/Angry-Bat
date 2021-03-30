using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField] private GameObject _cloudParticlePrefab;

    private Rigidbody2D _cached_Rigidbody;
    private AudioSource _cached_AudioSource;
    private bool _isFalling;

    private void Awake()
    {
        _cached_Rigidbody = GetComponent<Rigidbody2D>();
        _cached_AudioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (_cached_Rigidbody.velocity.magnitude >= 4f)
            _isFalling = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Bat bat = collision.collider.GetComponent<Bat>();

        if (!bat && _cached_Rigidbody.velocity.magnitude >= 0.3f && !_cached_AudioSource.isPlaying)
        {
            _cached_AudioSource.Play();
        }

        if (bat != null)
        {
            Die();
            return;
        }

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
