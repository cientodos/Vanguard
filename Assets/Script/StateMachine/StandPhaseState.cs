using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandPhaseState : ITurnState
{
    private readonly TurnStateMachine stateMachine;

    public StandPhaseState(TurnStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public void OnEnter()
    {
        Debug.Log("--- [Stand Phase] ---");

        // 1. 스탠드 실행 (유닛 일으키기)
        ActionSystem.Instance?.Stand(stateMachine.ActivePlayerId);

        // 2. 스탠드 연출 후 자동으로 드로우 페이즈로 이동
        // (실제 프로젝트에서는 애니메이션 종료 콜백이나 코루틴 대기 후 호출)
        stateMachine.ChangeState(new DrawPhaseState(stateMachine));
    }

    public void OnUpdate() { }

    public void OnExit() { }
}