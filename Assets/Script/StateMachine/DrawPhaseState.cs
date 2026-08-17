using UnityEngine;

public class DrawPhaseState : ITurnState
{
    private readonly TurnStateMachine stateMachine;

    public DrawPhaseState(TurnStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public void OnEnter()
    {
        Debug.Log("--- [Draw Phase] ---");

        // 1. 카드 1장 드로우
       // ActionSystem.Instance?.SystemDraw(stateMachine.ActivePlayerId, 1);

        // 2. 드로우 연출 후 라이드 페이즈로 이동
        stateMachine.ChangeState(new RidePhaseState(stateMachine));
    }

    public void OnUpdate() { }
    public void OnExit() { }
}