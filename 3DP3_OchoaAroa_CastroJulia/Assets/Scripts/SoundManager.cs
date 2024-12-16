using UnityEngine;

public enum SoundType
{
    COIN, 
    STAR, 
    GAMEOVER
}

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] m_Sounds;
    [SerializeField] private AudioClip m_Music;
    private static SoundManager m_Instance;
    private AudioSource m_AudioSource;
    private AudioSource m_MusicSource;

    private void Awake()
    {
        if(m_Instance == null)
        {
            m_Instance = this;
            DontDestroyOnLoad(this);
            
            
            m_MusicSource = gameObject.AddComponent<AudioSource>();
            m_MusicSource.loop = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
        
        if (m_Music != null)
        {
            PlayMusic();
        }
    }

    public static void PlaySound(SoundType soundType, float volume = 1)
    {
        m_Instance.m_AudioSource.PlayOneShot(m_Instance.m_Sounds[(int)soundType], volume);
    }
    
    public static void PlayMusic()
    {
        if (m_Instance.m_Music != null)
        {
            m_Instance.m_MusicSource.clip = m_Instance.m_Music;
            m_Instance.m_MusicSource.volume = 0.1f; 
            m_Instance.m_MusicSource.Play();
        }
    }

    public static void StopMusic()
    {
        if (m_Instance.m_MusicSource.isPlaying)
        {
            m_Instance.m_MusicSource.Stop();
        }
    }
}
