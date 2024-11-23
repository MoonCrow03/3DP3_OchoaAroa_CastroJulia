using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchBehaviour : StateMachineBehaviour
{
    private PlayerController m_PlayerController;
    public EPunchType m_PunchType;

    public float m_EnablePunchStartAnimationPct;
    public float m_EnablePunchEndAnimationPct;
    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_PlayerController = animator.GetComponentInParent<PlayerController>();
        m_PlayerController.EnableHitCollider(m_PunchType, false);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bool l_Enabled = stateInfo.normalizedTime>=m_EnablePunchStartAnimationPct && stateInfo.normalizedTime<=m_EnablePunchEndAnimationPct;
        m_PlayerController.EnableHitCollider(m_PunchType, l_Enabled);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_PlayerController.EnableHitCollider(m_PunchType, false);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
