using UnityEngine;

public class ScoreZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.AddScore();

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayPoint();
            }
        }
    }
}