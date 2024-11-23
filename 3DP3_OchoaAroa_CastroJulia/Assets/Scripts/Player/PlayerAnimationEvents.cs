using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    public void Step(AnimationEvent animationEvent)
    {
        AudioClip l_AudioClip = (AudioClip) animationEvent.objectReferenceParameter;
        AudioSource.PlayClipAtPoint(l_AudioClip, transform.position);
        Debug.Log("Step " + animationEvent.intParameter);
    }
}
