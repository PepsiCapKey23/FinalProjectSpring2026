using UnityEngine;

public class CollisionSFX : MonoBehaviour
{
    public AudioSource hitSource;

    private void Start()
    {
        if (hitSource == null) hitSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        bool isMarble = collision.gameObject.CompareTag("Marble");
        bool isPlayer = collision.gameObject.CompareTag("OG");

        if (isMarble || isPlayer)
        {
            if (this.gameObject.GetHashCode() < collision.gameObject.GetHashCode()) 
            {
                return; 
            }

            if (hitSource != null && hitSource.clip != null)
            {
                hitSource.PlayOneShot(hitSource.clip);
            }
        }
    }
}