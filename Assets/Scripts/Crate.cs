using UnityEngine;

public class Crate : MonoBehaviour
{
    private AudioSource _cached_AudioSource;
    private Rigidbody2D _cached_Rigidbody;

    private void Awake()
    {
        _cached_AudioSource = GetComponent<AudioSource>();
        _cached_Rigidbody = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_cached_Rigidbody.velocity.magnitude >= 1.5f && !_cached_AudioSource.isPlaying)
            _cached_AudioSource.Play();
    }
}
