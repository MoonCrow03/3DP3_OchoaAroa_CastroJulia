using UnityEngine;

public abstract class Item : MonoBehaviour, IRestartGameElement
{
    public abstract bool CanPick();

    public virtual void Pick()
    {
        gameObject.SetActive(false);
    }

    public void HardRestartGame()
    {
        gameObject.SetActive(true);
    }

    public void RestartGame() { }

    public void PauseGame() { }
}