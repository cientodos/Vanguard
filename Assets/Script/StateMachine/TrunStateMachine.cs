using UnityEngine;

public class TurnStateMachine : MonoBehaviour
{
    public ITurnState CurrentState { get; private set; }

    // 현재 턴의 주체 (0: Player 1, 1: Player 2)
    public int ActivePlayerId { get; private set; } = 0;

    private void Update()
    {
        // 현재 상태의 Update 실행
        CurrentState?.OnUpdate();
    }

    /// <summary>
    /// 상태(페이즈) 전환 함수
    /// </summary>
    public void ChangeState(ITurnState newState)
    {
        // 1. 기존 상태 Exit 실행
        CurrentState?.OnExit();

        // 2. 새 상태 할당
        CurrentState = newState;

        // 3. 새 상태 Enter 실행
        Debug.Log($"<color=yellow>[Phase Change]</color> -> {CurrentState.GetType().Name}");
        CurrentState?.OnEnter();
    }

    /// <summary>
    /// 턴 주체 교체 (P1 <-> P2)
    /// </summary>
    public void SwitchTurnPlayer()
    {
        ActivePlayerId = (ActivePlayerId == 0) ? 1 : 0;
        Debug.Log($"<color=cyan>=== 플레이어 {ActivePlayerId}의 턴 시작 ===</color>");
    }
}