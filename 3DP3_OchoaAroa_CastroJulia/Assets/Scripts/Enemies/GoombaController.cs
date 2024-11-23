using UnityEngine;

public class GoombaController : MonoBehaviour, IRestartGameElement
{
    private CharacterController m_CharacterController;
    private Vector3 m_InitialPosition;
    private Quaternion m_InitialRotation;
    
    private void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();
    }
    
    private void Start()
    {
        m_InitialPosition = transform.position;
        m_InitialRotation = transform.rotation;
        GameManager.GetInstance().RegisterGameElement(this);
    }
    
    public void Kill()
    {
        gameObject.SetActive(false);
    }
    
    public void RestartGame()
    {
        gameObject.SetActive(true);
        m_CharacterController.enabled = false;
        transform.position = m_InitialPosition;
        transform.rotation = m_InitialRotation;
        m_CharacterController.enabled = true;
    }
}
