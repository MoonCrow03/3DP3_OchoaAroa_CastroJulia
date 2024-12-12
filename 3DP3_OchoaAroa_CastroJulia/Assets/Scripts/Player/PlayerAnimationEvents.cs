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
    
    public void PunchSound1(AnimationEvent animationEvent)
    {
        AudioClip l_AudioClip = (AudioClip) animationEvent.objectReferenceParameter;
        AudioSource.PlayClipAtPoint(l_AudioClip, transform.position);
        Debug.Log("PunchSound3 " + animationEvent.intParameter);
    }
    
    public void PunchSound2(AnimationEvent animationEvent)
    {
        AudioClip l_AudioClip = (AudioClip) animationEvent.objectReferenceParameter;
        AudioSource.PlayClipAtPoint(l_AudioClip, transform.position);
        Debug.Log("PunchSound3 " + animationEvent.intParameter);
    }
    
    public void PunchSound3(AnimationEvent animationEvent)
    {
        AudioClip l_AudioClip = (AudioClip) animationEvent.objectReferenceParameter;
        AudioSource.PlayClipAtPoint(l_AudioClip, transform.position);
        Debug.Log("PunchSound3 " + animationEvent.intParameter);
    }
    
    public void FinishPunch(AnimationEvent animationEvent)
    {
        AudioClip l_AudioClip = (AudioClip) animationEvent.objectReferenceParameter;
        AudioSource.PlayClipAtPoint(l_AudioClip, transform.position);
        Debug.Log("Punch " + animationEvent.intParameter);
    }
}
