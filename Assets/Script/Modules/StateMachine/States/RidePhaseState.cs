using UnityEngine;

public class RidePhaseState : ITurnState
{
    private readonly TurnStateMachine stateMachine;

    public RidePhaseState(TurnStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public void OnEnter()
    {
        Debug.Log("--- [Ride Phase] ---");

       // ActionSystem.Instance?.DrawCard(stateMachine.ActivePlayerId, 1);

        // 2. 드로우 연출 후 라이드 페이즈로 이동
        stateMachine.ChangeState(new MainPhaseState(stateMachine));
    }

    public void OnUpdate() { }

    public void OnExit() { }
}