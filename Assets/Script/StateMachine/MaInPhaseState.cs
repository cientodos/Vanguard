using UnityEngine;

public class MainPhaseState : ITurnState
{
    private readonly TurnStateMachine stateMachine;

    public MainPhaseState(TurnStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public void OnEnter()
    {
        Debug.Log("--- [Main Phase] ---");

        // UI에 "메인 페이즈 진행 중" 표시 및 [턴 종료] / [배틀 페이즈 진입] 버튼 활성화
      //  UIManager.Instance?.EnableMainPhaseUI(true);

        // UI 버튼 클릭 이벤트 바인딩 예시
      //  UIManager.Instance?.OnEndMainPhaseButtonClicked.AddListener(OnEndMainPhase);
    }

    public void OnUpdate()
    {
        // 메인 페이즈 중 매 프레임 검사할 로직이 있다면 작성
    }

    public void OnExit()
    {
        // UI 정리 및 이벤트 해제
      //  UIManager.Instance?.OnEndMainPhaseButtonClicked.RemoveListener(OnEndMainPhase);
      //  UIManager.Instance?.EnableMainPhaseUI(false);
    }

    // 유저가 배틀 페이즈로 넘어가거나 턴 종료를 눌렀을 때 호출될 메서드
    private void OnEndMainPhase()
    {
        // 배틀 페이즈로 전환
        stateMachine.ChangeState(new BattlePhaseState(stateMachine));
    }
}