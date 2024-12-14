using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    public void Step(AnimationEvent animationEvent)
    {
        AudioClip l_AudioClip = (AudioClip) animationEvent.objectReferenceParameter;
        AudioSource.PlayClipAtPoint(l_AudioClip, transform.position);
    }
    
    public void PunchSound1(AnimationEvent animationEvent)
    {
        AudioClip l_AudioClip = (AudioClip) animationEvent.objectReferenceParameter;
        AudioSource.PlayClipAtPoint(l_AudioClip, transform.position);
    }
    
    public void PunchSound2(AnimationEvent animationEvent)
    {
        AudioClip l_AudioClip = (AudioClip) animationEvent.objectReferenceParameter;
        AudioSource.PlayClipAtPoint(l_AudioClip, transform.position);
    }
    
    public void PunchSound3(AnimationEvent animationEvent)
    {
        AudioClip l_AudioClip = (AudioClip) animationEvent.objectReferenceParameter;
        AudioSource.PlayClipAtPoint(l_AudioClip, transform.position);
    }
    
    public void FinishPunch(AnimationEvent animationEvent)
    {
        AudioClip l_AudioClip = (AudioClip) animationEvent.objectReferenceParameter;
        AudioSource.PlayClipAtPoint(l_AudioClip, transform.position);
    }
    
    public void Jump1(AnimationEvent animationEvent)
    {
        AudioClip l_AudioClip = (AudioClip) animationEvent.objectReferenceParameter;
        AudioSource.PlayClipAtPoint(l_AudioClip, transform.position);
    }
    
    public void Jump2(AnimationEvent animationEvent)
    {
        AudioClip l_AudioClip = (AudioClip) animationEvent.objectReferenceParameter;
        AudioSource.PlayClipAtPoint(l_AudioClip, transform.position);
    }
    
    public void Jump3(AnimationEvent animationEvent)
    {
        AudioClip l_AudioClip = (AudioClip) animationEvent.objectReferenceParameter;
        AudioSource.PlayClipAtPoint(l_AudioClip, transform.position);
    }
    
    public void LongJumpSound(AnimationEvent animationEvent)
    {
        AudioClip l_AudioClip = (AudioClip) animationEvent.objectReferenceParameter;
        AudioSource.PlayClipAtPoint(l_AudioClip, transform.position);
    }
    
    public void DeathSound(AnimationEvent animationEvent)
    {
        AudioClip l_AudioClip = (AudioClip) animationEvent.objectReferenceParameter;
        AudioSource.PlayClipAtPoint(l_AudioClip, transform.position);
    }
    
    public void HitSound1(AnimationEvent animationEvent)
    {
        AudioClip l_AudioClip = (AudioClip) animationEvent.objectReferenceParameter;
        AudioSource.PlayClipAtPoint(l_AudioClip, transform.position);
    }
}
