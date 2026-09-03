using UnityEngine;

public class EndPhaseState : ITurnState
{
    private readonly TurnStateMachine stateMachine;

    public EndPhaseState(TurnStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public void OnEnter()
    {
        Debug.Log("--- [End Phase] ---");

        // 1. 턴 종료 시 효과 해제 및 정리
        //ActionSystem.Instance?.CleanupTurnEffects();

        // 2. 턴 주체 교체 (P1 -> P2)
        stateMachine.SwitchTurnPlayer();

        // 3. 다시 상대방의 스탠드 페이즈로 순환!
        stateMachine.ChangeState(new StandPhaseState(stateMachine));
    }

    public void OnUpdate() { }
    public void OnExit() { }
}