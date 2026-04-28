using UnityEngine;

public class RingSensor : MonoBehaviour
{
    public int score = 0;
    private AudioSource pointSource; 

    void Start()
    {
        pointSource = GetComponent<AudioSource>();
        
        if (pointSource == null)
        {
            Debug.LogError("");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Marble"))
        {
            score++;
            Debug.Log("+1! Score: " + score);
            
            if (pointSource != null && pointSource.clip != null)
            {
                pointSource.PlayOneShot(pointSource.clip);
            }

            Destroy(other.gameObject, 0.1f);
        }
    }
}