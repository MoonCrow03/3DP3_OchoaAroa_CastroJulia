using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public abstract bool CanPick();

    public virtual void Pick()
    {
        gameObject.SetActive(false);
    }
}