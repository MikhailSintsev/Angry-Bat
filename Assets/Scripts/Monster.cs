using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField] private GameObject _cloudParticlePrefab; // ������ �� ������ ������� ������

    private Rigidbody2D _cached_Rigidbody; // ������������ Rigidbody2D �������
    private AudioSource _cached_AudioSource; // ������������ AudioSource �������
    private bool _isFalling; // �������� ������� �������

    private void Awake()
    {
        _cached_Rigidbody = GetComponent<Rigidbody2D>(); // ���������� Rigidbody2D
        _cached_AudioSource = GetComponent<AudioSource>(); // ���������� AudioSource �������
    }

    private void Update()
    {
        // ���� �������� ������� ������ ��� ����� ��������� �����
        if (_cached_Rigidbody.velocity.magnitude >= 4f)
            _isFalling = true; // �������, ��� ������ ������
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Bat bat = collision.collider.GetComponent<Bat>(); // �������� �������� ������ Bat � ������� ������������
        Debug.Log(collision.contacts[0].normal);

        // ���� ������ �� �������, ������� ������� ������ ��������� ����� � ���� �� ������������� ��������
        if (bat == null && _cached_Rigidbody.velocity.magnitude >= 0.3f && !_cached_AudioSource.isPlaying)
        {
            _cached_AudioSource.Play(); // ��������� ����
        }

        // ���� ������ �������
        if (bat != null)
        {
            Die(); // ��������� ����� Die()
            return; // � ����� �� ������ OnCollisionEnter2D
        }

        // ���� ������ ������
        if (_isFalling)
        {
            Die(); // ��������� ����� Die()
            return; // � ����� �� ������ OnCollisionEnter2D
        }

        // ���� ������� ������� ������������ � ������� ������� �� ��� Y ������ ��������� �������� (�� ������ ��� �� ������)
        if (collision.contacts[0].normal.y < -0.5f)
            Die(); // ��������� ����� Die()
    } 
    
    private void Die()
    {
        // ������� ������� ������ �� ������ ������� � ���������� ����� �������� �����
        Destroy(Instantiate(_cloudParticlePrefab, transform.position, Quaternion.identity), 3);
        Destroy(gameObject); // ���������� ������
    }
}
