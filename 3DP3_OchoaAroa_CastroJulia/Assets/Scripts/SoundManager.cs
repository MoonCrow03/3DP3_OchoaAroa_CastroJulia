using UnityEngine;

public enum SoundType
{
    SHOOT, 
    HIT, 
    EXPLOSION,
    FOOTSEP,
    NOAMMO,
    RELOAD,
    DOOR
}

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] m_Sounds;
    private static SoundManager m_Instance;
    private AudioSource m_AudioSource;

    private void Awake()
    {
        if(m_Instance == null)
        {
            m_Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    public static void PlaySound(SoundType soundType, float volume = 1)
    {
        m_Instance.m_AudioSource.PlayOneShot(m_Instance.m_Sounds[(int)soundType], volume);
    }
}
