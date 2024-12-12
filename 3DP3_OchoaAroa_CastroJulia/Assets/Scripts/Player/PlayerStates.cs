using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EPlayerState
{
    IDLE,
    WALK,
    RUN,
    JUMP,
    FALL,
    PUNCH,
    DEAD
}

public class IdleState : BState<EPlayerState>
{
    private PlayerStateMachine m_PlayerStateMachine;
    
    public IdleState(PlayerStateMachine playerController) : base (EPlayerState.IDLE)
    {
        m_PlayerStateMachine = playerController;
    }

    public override void OnEnter()
    {
        m_PlayerStateMachine.m_Animator.SetFloat(PlayerStateMachine.Speed, 0.0f);
    }

    public override void OnExit() { }

    public override void OnUpdate()
    {
        if (InputManager.Instance.Up.Hold || InputManager.Instance.Down.Hold ||
            InputManager.Instance.Left.Hold || InputManager.Instance.Right.Hold)
        {
            Debug.Log("Moving");
            m_PlayerStateMachine.QueueNextState(new WalkingState(m_PlayerStateMachine));
        }
        
        if(InputManager.Instance.Space.Tap && m_PlayerStateMachine.CanJump())
        {
            m_PlayerStateMachine.QueueNextState(new JumpingState(m_PlayerStateMachine));
        }
    }

    public override EPlayerState OnNextState() => EPlayerState.IDLE;
}

public class WalkingState : BState<EPlayerState>
{
    private PlayerStateMachine m_PlayerStateMachine;
    public WalkingState(PlayerStateMachine playerController) : base(EPlayerState.WALK)
    {
        m_PlayerStateMachine = playerController;
    }

    public override void OnEnter()
    {
        m_PlayerStateMachine.m_Animator.SetFloat(PlayerStateMachine.Speed, 0.5f);
    }

    public override void OnExit() { }

    public override void OnUpdate()
    {
        Vector3 l_forward = m_PlayerStateMachine.m_Camera.transform.forward;
        Vector3 l_right = m_PlayerStateMachine.m_Camera.transform.right;

        l_forward.y = 0f;
        l_right.y = 0f;
        l_forward.Normalize();
        l_right.Normalize();

        Vector3 movement = Vector3.zero;

        if (InputManager.Instance.Right.Hold)
            movement += l_right;
        if (InputManager.Instance.Left.Hold)
            movement -= l_right;
        if (InputManager.Instance.Up.Hold)
            movement += l_forward;
        if (InputManager.Instance.Down.Hold)
            movement -= l_forward;

        if (movement.sqrMagnitude > 0.0f)
        {
            if (InputManager.Instance.Shift.Hold)
            {
                m_PlayerStateMachine.QueueNextState(new RunningState(m_PlayerStateMachine));
                return;
            }

            m_PlayerStateMachine.Move(movement, m_PlayerStateMachine.m_WalkSpeed);
            m_PlayerStateMachine.Rotate(movement);
        }
        else
        {
            m_PlayerStateMachine.QueueNextState(new IdleState(m_PlayerStateMachine));
        }

        if (InputManager.Instance.Space.Tap && m_PlayerStateMachine.CanJump())
        {
            m_PlayerStateMachine.QueueNextState(new JumpingState(m_PlayerStateMachine));
        }
    }

    public override EPlayerState OnNextState() => EPlayerState.WALK;
}

public class RunningState : BState<EPlayerState>
{
    private PlayerStateMachine m_PlayerStateMachine;
    public RunningState(PlayerStateMachine playerController) : base(EPlayerState.RUN)
    {
        m_PlayerStateMachine = playerController;
    }

    public override void OnEnter()
    {
        m_PlayerStateMachine.m_Animator.SetFloat(PlayerStateMachine.Speed, 1.0f);
    }

    public override void OnExit() { }

    public override void OnUpdate()
    {
        Vector3 l_forward = m_PlayerStateMachine.m_Camera.transform.forward;
        Vector3 l_right = m_PlayerStateMachine.m_Camera.transform.right;

        l_forward.y = 0f;
        l_right.y = 0f;
        l_forward.Normalize();
        l_right.Normalize();

        Vector3 movement = Vector3.zero;

        if (InputManager.Instance.Right.Hold)
            movement += l_right;
        if (InputManager.Instance.Left.Hold)
            movement -= l_right;
        if (InputManager.Instance.Up.Hold)
            movement += l_forward;
        if (InputManager.Instance.Down.Hold)
            movement -= l_forward;

        if (movement.sqrMagnitude > 0.0f)
        {
            if (!InputManager.Instance.Shift.Hold)
            {
                m_PlayerStateMachine.QueueNextState(new WalkingState(m_PlayerStateMachine));
                return;
            }

            m_PlayerStateMachine.Move(movement, m_PlayerStateMachine.m_RunSpeed);
            m_PlayerStateMachine.Rotate(movement);
        }
        else
        {
            m_PlayerStateMachine.QueueNextState(new IdleState(m_PlayerStateMachine));
        }

        if (InputManager.Instance.Space.Tap && m_PlayerStateMachine.CanJump())
        {
            m_PlayerStateMachine.QueueNextState(new JumpingState(m_PlayerStateMachine));
        }
    }

    public override EPlayerState OnNextState() => EPlayerState.RUN;
}

public class JumpingState : BState<EPlayerState>
{
    private PlayerStateMachine m_PlayerStateMachine;
    private bool m_IsFalling;
    
    public JumpingState(PlayerStateMachine playerController) : base(EPlayerState.JUMP)
    {
        m_PlayerStateMachine = playerController;
    }

    public override void OnEnter()
    {
        m_PlayerStateMachine.ExecuteJump();
        m_IsFalling = false;
    }

    public override void OnExit() { }

    public override void OnUpdate()
    {
        m_PlayerStateMachine.m_VerticalSpeed += Physics.gravity.y * Time.deltaTime;
        Vector3 movement = new Vector3(0, m_PlayerStateMachine.m_VerticalSpeed * Time.deltaTime, 0);
        CollisionFlags collisionFlags = m_PlayerStateMachine.m_CharacterController.Move(movement);

        if ((collisionFlags & CollisionFlags.Above) != 0)
        {
            m_PlayerStateMachine.m_VerticalSpeed = 0;
        }

        if ((collisionFlags & CollisionFlags.Below) != 0)
        {
            if (m_IsFalling)
            {
                m_PlayerStateMachine.QueueNextState(new IdleState(m_PlayerStateMachine));
                return;
            }
        }

        if (m_PlayerStateMachine.m_VerticalSpeed < 0)
        {
            m_IsFalling = true;
        }
    }

    public override EPlayerState OnNextState() => EPlayerState.JUMP;
}

public class PunchingState : BState<EPlayerState>
{
    private PlayerStateMachine m_PlayerStateMachine;
    
    public PunchingState(PlayerStateMachine playerController) : base(EPlayerState.PUNCH)
    {
        m_PlayerStateMachine = playerController;
    }

    public override void OnEnter()
    {
        m_PlayerStateMachine.ExecutePunch();
    }

    public override void OnExit() { }

    public override void OnUpdate()
    {
        
    }

    public override EPlayerState OnNextState() => EPlayerState.PUNCH;
}