using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [SerializeField] private AudioClip flapSound;
    [SerializeField] private AudioClip pointSound;
    [SerializeField] private AudioClip gameOverSound;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayFlap()
    {
        sfxSource.PlayOneShot(flapSound);
    }

    public void PlayPoint()
    {
        sfxSource.PlayOneShot(pointSound);
    }

    public void PlayGameOver()
    {
        sfxSource.PlayOneShot(gameOverSound);
    }
}